/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import React, { PropTypes, PureComponent } from 'react'
import Channel from 'oskari-rpc'
import { Map } from 'immutable'
import shortId from 'shortid'
import { connect } from 'react-redux'
import { compose } from 'redux'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl, defineMessages } from 'react-intl'
import 'js-polyfills'

// actions
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { loadMap,
  mapIsLoaded,
  enableMapEdit,
  disableMapEdit } from 'reducers/maps'

// selectors
import { getMapIsLoading,
  getIsEditEnabled } from 'selectors/maps'

import { getMapDNSURL } from 'selectors'

// components
import { PTVPreloader } from 'Components'

const ADD_MARKER = 'MapModulePlugin.AddMarkerRequest'
const REMOVE_MARKER = 'MapModulePlugin.RemoveMarkersRequest'
const SHOW_MARKER_INFO = 'InfoBox.ShowInfoBoxRequest'
const PopupActions = {
  ENABLE_EDIT: 'ENABLE_EDIT',
  DISABLE_EDIT: 'DISABLE_EDIT',
  RETURN_TO_MAIN: 'RETURN_TO_MAIN'
}
let componentData = Map()

const messages = defineMessages({
  popupInfoTitle: {
    id: 'MapComponent.PopupInfo.Title',
    defaultMessage: 'Karttapiste tiedot'
  },
  coordinatesInfoTitle: {
    id: 'MapComponent.CoordinatesInfo.Title',
    defaultMessage: 'Karttakoodinaatit:'
  },
  editCoordinateLinkTitle: {
    id: 'MapComponent.EditCoordinateLink.Title',
    defaultMessage: 'Muokkaa karttapistettä'
  },
  returnMainCoordinateBackLinkTitle: {
    id: 'MapComponent.ReturnMainCoordinateBack.Title',
    defaultMessage: 'Palauta alkuperäinen karttapiste'
  },
  saveCoordinateLinkTitle: {
    id: 'MapComponent.SaveCoordinateLink.Title',
    defaultMessage: 'Lock the point for save'
  },
  addNewUserPointTitle: {
    id: 'MapComponent.AddNewUserPoint.Title',
    defaultMessage: 'Voint antaa sisäänkäynnin tarkan sijainin klikkaamalla kartta.'
  },
  movedPointTitle: {
    id: 'MapComponent.MovedPoint.Title',
    defaultMessage: 'Käyntiosoitteen sijaintikoodinaatti on siirretty osoittamaan tarkkaa sisäänkäyntiä:'
  },
  savedMovedPointTitle: {
    id: 'MapComponent.SavedMovedPoint.Title',
    defaultMessage: 'Käyntiosoitteen uusi karttapiste on tallennettu:'
  }
})

class MapComponent extends PureComponent {
  setData = (property, value) => {
    if (!property) {
      componentData = componentData.setIn([this.props.id, this.props.contentLanguage, this.props.mapComponentId], value)
    } else {
      componentData = componentData.setIn([this.props.id, this.props.contentLanguage,
        this.props.mapComponentId, property], value)
    }
  }

  getData = (property) => {
    return componentData.getIn([this.props.id, this.props.contentLanguage, this.props.mapComponentId, property])
  }

  onReadyCallback = () => {
    this.setData('init', true)
    // this.showSupportedFunctionality()
    this.props.actions.mapIsLoaded(this.props.id)
    this.addHandlingOfEvents()
    this.addAllPoints(this.props, true)
    this.setData('userPointId', this.props.coordinates.size > 1 &&
      this.props.coordinates.filter(point => !point.get('isMain')).first().get('id'))
    this.setData('init', false)
  }

  componentWillMount () {
    this.props.actions.loadMap(this.props.id)
  }

  componentWillUnmount () {
    if (componentData) {
      console.log('PTV:cleaning')
      const localMap = this.getData('map')
      localMap && localMap.destroy()
    }
  }

  componentWillReceiveProps (nextProps) {
    if (!nextProps.isFetching && (!this.props.isLoading || !nextProps.isLoading)) {
      this.updatePoints(nextProps)
    }
  }

  updatePoints = (props) => {
    const localMap = this.getData('map')
    const isInit = this.getData('init')
    if (!isInit && localMap != null && localMap.isReady()) {
      console.log('PTV update points')
      localMap.postRequest(REMOVE_MARKER, [])
      this.addAllPoints(props)
      this.setData('userPointId', props.coordinates.size > 1 &&
        props.coordinates.filter(point => !point.get('isMain')).first().get('id'))
    }
  }

  handleEditOnClick = () => (event) => {
    this.props.actions.enableMapEdit(this.props.id)
  }

  addAllPoints = (props, moveToPoint) => {
    const localMap = this.getData('map')
    const point = props.coordinates.size === 1
     ? props.coordinates.first() : props.coordinates.filter(p => !p.get('isMain')).first()
    if (point) {
      localMap.postRequest(ADD_MARKER, [{ x: point.get('longtitude'),
        y: point.get('latitude'),
        color: point.get('isMain') ? '000000' : '007e00',
        msg : '',
        shape: 2, // icon number (0-6)
        size: 7 }, point.get('id')])

      this.showDialogForMarker(props, point.get('id'))

      if (moveToPoint) {
        localMap.postRequest('MapMoveRequest', [point.get('longtitude'), point.get('latitude'), 11])
      }
    } else {
      console.error('No point provided')
    }
  }

  showDialogForMarker = (props, pointId) => {
    const point = props.coordinates.filter(p => p.get('id') === pointId).first()
    const { addressInfo: { streetName,
      streetNumber,
      postalCode,
      municipality
    },
    intl: { formatMessage },
    isEditEnabled,
    disabled
    } = props

    const isMain = point.get('isMain')
    const street = (streetName || '') + ' ' + (streetNumber || '')
    const municipalityInfo = (postalCode.get('code') || '') + ' ' + (municipality.get('name') || '')
    const coordinatesInfo = (point.get('longtitude') || '') + ', ' + (point.get('latitude') || '')
    const action1 = isEditEnabled ? PopupActions.DISABLE_EDIT : PopupActions.ENABLE_EDIT
    const action1Message = isEditEnabled
     ? formatMessage(messages.saveCoordinateLinkTitle) : formatMessage(messages.editCoordinateLinkTitle)
    const content = [{
      html: `<div>` +
              `<b>${street}</b>` +
              `<p>${municipalityInfo}</p>` +
              (!isMain ? isEditEnabled ? `<p>${formatMessage(messages.movedPointTitle)}</p>`
                : `<p>${formatMessage(messages.savedMovedPointTitle)}</p>` : '') +
              `<p>${formatMessage(messages.coordinatesInfoTitle)}</p>` +
              `<a style='color: inherit; text-decoration: none; cursor: default;'>${coordinatesInfo}</a>` +
            `</div>`
    } ]

    !disabled && (content[0].actions = [
      {
        name: `${action1Message}`,
        type: 'link',
        action: {
          action: action1
        }
      }
    ])
    // if is main - user should see message, that he can change the entrance
    !disabled &&
      isMain &&
      content.push({ html: `<div>${formatMessage(messages.addNewUserPointTitle)}</div>` })

    !disabled && !isMain && isEditEnabled && content[0].actions.push({
      name: `${formatMessage(messages.returnMainCoordinateBackLinkTitle)}`,
      type: 'link',
      action: {
        action: PopupActions.RETURN_TO_MAIN
      }
    })
    const infoboxData = [
      'markerInfoBox',
      `${formatMessage(messages.popupInfoTitle)}`,
      content,
      {
        marker: point.get('id')
      },
      {
        mobileBreakpoints: {
          width: 0,
          height: 0
        },
        hidePrevious: true
      }
    ]

    const localMap = this.getData('map')
    localMap.postRequest(SHOW_MARKER_INFO, infoboxData)
  }

  // shouldComponentUpdate (nextProps, nextState) {
  //   return !this.getData('iframe')
  // }

  showSupportedFunctionality = () => {
    const localMap = this.getData('map')
    localMap.getSupportedEvents((supported) => {
      console.log('Available events', supported)
    })

    localMap.getSupportedRequests((supported) => {
      console.log('Available requests', supported)
    })

    localMap.getSupportedFunctions((supported) => {
      console.log('Available functions', supported)
    })
      // supported functions can also be detected by
    if (typeof localMap.getAllLayers === 'function') {
      localMap.getAllLayers(function (layers) {
        console.log('Available layers', layers)
      })
    }
  }

  addHandlingOfEvents = () => {
    const localMap = this.getData('map')
    if (localMap.isReady()) {
      localMap.handleEvent('AfterAddMarkerEvent', this.handleAfterAddMarkerEvent)
      localMap.handleEvent('AfterMapMoveEvent', this.handleAfterMapMoveEvent)
      localMap.handleEvent('DrawingEvent', this.handleDrawingEvent)
      localMap.handleEvent('FeatureEvent', this.handleFeatureEvent)
      localMap.handleEvent('FeedbackResultEvent', this.handleFeedbackResultEvent)
      localMap.handleEvent('MapClickedEvent', this.handleMapClickedEvent)
      localMap.handleEvent('MarkerClickEvent', this.handleMarkerClickEvent)
      localMap.handleEvent('SearchResultEvent', this.handleSearchResultEvent)
      localMap.handleEvent('InfoboxActionEvent', this.handleInfoboxActionEvent)
      localMap.handleEvent('UserLocationEvent', this.handleAfterUserLocationEvent)
    }
  }

  handleAfterAddMarkerEvent = (data) => {

  }

  handleAfterMapMoveEvent = (data) => {

  }

  handleInfoboxActionEvent = (data) => {
    const { actionParams: { action } } = data
    switch (action) {
      case PopupActions.ENABLE_EDIT: {
        this.props.actions.enableMapEdit(this.props.id)
        break
      }
      case PopupActions.DISABLE_EDIT: {
        this.props.actions.disableMapEdit(this.props.id)
        break
      }
      case PopupActions.RETURN_TO_MAIN: {
        this.props.onResetToDefaultClick(this.props.id)
        break
      }
    }
  }

  handleDrawingEvent = (data) => {

  }

  handleFeatureEvent = (data) => {

  }

  handleFeedbackResultEvent = (data) => {

  }

  markerClicked = false
  handleMapClickedEvent = (data) => {
    if (!this.markerClicked) {
      if (this.props.isEditEnabled && !this.props.disabled) {
        const localUserPointId = this.setData('userPointId')
        const point = { longtitude: data.lon,
          latitude: data.lat,
          id: !localUserPointId ? shortId.generate() : localUserPointId }
        this.props.onMapClick(point)
      }
    } else {
      this.markerClicked = false
    }
  }

  handleMarkerClickEvent = (data) => {
    this.markerClicked = true
    this.showDialogForMarker(this.props, data.id)
  }

  handleSearchResultEvent = (data) => {

  }

  handleAfterUserLocationEvent = (data) => {
  }

  initMapObject = props => {
    let localMap = this.getData('map')
    if (localMap && localMap.isReady()) {
      console.log('PTV:destroyForInit')
      localMap.destroy()
    }
    console.log('PTV:initMapObject:' + props.target.id)
    const iFrame = this.getData('iframe')
    console.log('PTV:isIframe:', iFrame ? true : false)
    localMap = Channel.connect(iFrame, this.props.mapDNS.origin)
    this.setData('map', localMap)
    localMap.onReady(this.onReadyCallback)
  }

  mapref = input => {
    if (input && input.id) {
      this.setData(null, Map({ iframe: input }))
    }
  }

  render () {
    return (
      <div key={'container_' + this.props.id} id={'container_id_' + this.props.id}>
        {this.props.isLoading && <PTVPreloader />}
        <iframe key={'map_' + this.props.id} onLoad={this.initMapObject}
          style={{ border: 'none', width: '100%', height: '500px' }}
          ref={this.mapref}
          id={'map_id_' + this.props.id} src={this.props.mapDNS.href} />
      </div>
    )
  }
}

MapComponent.propTypes = {
  coordinates: ImmutablePropTypes.list.isRequired,
  onMapClick: PropTypes.func.isRequired,
  onResetToDefaultClick: PropTypes.func.isRequired,
  addressInfo: PropTypes.object.isRequired,
  mapDNS: PropTypes.object.isRequired,
  actions: PropTypes.object.isRequired,
  id: PropTypes.string.isRequired,
  mapComponentId: PropTypes.string.isRequired,
  contentLanguage: PropTypes.string.isRequired,
  isLoading: PropTypes.bool.isRequired,
  isFetching: PropTypes.bool.isRequired,
  disabled: PropTypes.bool.isRequired,
  isEditEnabled: PropTypes.bool.isRequired,
  intl: PropTypes.object.isRequired
}

export default compose(
  connect(
    (state, ownProps) => ({
      isLoading: getMapIsLoading(state, ownProps),
      isEditEnabled: getIsEditEnabled(state, ownProps),
      mapDNS: getMapDNSURL(state, ownProps)
    }),
    mapDispatchToProps([{ loadMap,
      mapIsLoaded,
      enableMapEdit,
      disableMapEdit }])
  ),
  injectIntl
)(MapComponent)
