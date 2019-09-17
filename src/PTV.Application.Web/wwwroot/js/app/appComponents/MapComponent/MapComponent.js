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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import Channel from 'oskari-rpc'
import { Map, List } from 'immutable'
import shortId from 'shortid'
import { connect } from 'react-redux'
import { compose } from 'redux'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import 'js-polyfills'

// actions
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { loadMap,
  mapIsLoaded } from 'reducers/maps'

// selectors
import { getMapIsLoading } from 'selectors/maps'
import { getLocalizedStreetName } from './selectors'
import { getMapDNSURL } from 'selectors'

// components
import { Spinner } from 'sema-ui-components'

const MapActions = {
  ADD_MARKER: 'MapModulePlugin.AddMarkerRequest',
  REMOVE_MARKER: 'MapModulePlugin.RemoveMarkersRequest',
  MOVE_TO_POINT: 'MapMoveRequest',
  SHOW_MARKER_INFO: 'InfoBox.ShowInfoBoxRequest',
  HIDE_MARKER_INFO: 'InfoBox.HideInfoBoxRequest',
  SEARCH_ADDRESS: 'SearchRequest'
}

const PopupActions = {
  ENABLE_EDIT: 'ENABLE_EDIT',
  DISABLE_EDIT: 'DISABLE_EDIT',
  RETURN_TO_MAIN: 'RETURN_TO_MAIN'
}

const Colors = {
  ACTIVE_COLOR: '3265AA',
  INACTIVE_COLOR: '797E81'
}

const MarkerSizes = {
  ACTIVE_MARKER: 9,
  INACTIVE_MARKER: 7
}

let componentData = Map()

const ComponentDataType = {
  MAP: 'map',
  // it means, that if it is true, the map is init process and can't be used, because rpc is not initialized
  // when false, the map is initialized and we can call operations on it
  INIT: 'init',
  ACTIVE_COORDINATE: 'activeCoordinate',
  IFRAME: 'iframe',
  POINTS_EDIT_AVAILABILITY: 'pointsActionAvailability',
  ONCLICK: 'onclick'
}

/**
   * Store the data for given property to component state
   * @param {?string} property - the identifier of needed data
   * @param {?} value - data to be stored
   * @param {?string} idOfMapComponent - id of the map component
   */
const setData = (property, value, idOfMapComponent) => {
  if (idOfMapComponent) {
    componentData = componentData.setIn([idOfMapComponent, property], value)
  } else {
    console.error('SetData: No idOfMapComponent provided!')
  }
}

/**
   * Gets the data from component state
   * @param {?string} property - the identifier of needed data
   * @param {?string} idOfMapComponent - id of the map component
   * @return {?} data for given property
   */
const getData = (property, idOfMapComponent) => {
  if (idOfMapComponent) {
    return componentData.getIn([idOfMapComponent, property])
  } else {
    console.error('GetData: No idOfMapComponent provided!')
  }
}

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
    defaultMessage:
      'Käyntiosoitteen sijaintikoodinaatti on siirretty osoittamaan tarkkaa sisäänkäyntiä: {latitude}, {longitude}'
  },
  savedMovedPointTitle: {
    id: 'MapComponent.SavedMovedPoint.Title',
    defaultMessage: 'Käyntiosoitteen uusi karttapiste on tallennettu.'
  }
})

class MapComponent extends PureComponent {
  onReadyCallback = () => {
    setData(ComponentDataType.INIT, true, this.props.id)
    this.props.actions.mapIsLoaded(this.props.id)
    this.addHandlingOfEvents()
    // if coordinate of active address is valid add all points of all addresses
    if (this.isSomeCoordinateValid(this.props)) {
      this.addAllPoints(this.props, true)
    } else {
      // if there is no any valid coordinate of the active address,
      // add the other points and try to search and move to the address with given street and municipality
      this.addAllPoints(this.props, true, true)
      this.trySearchAndMove(this.props)
    }

    setData(ComponentDataType.INIT, false, this.props.id)
  }

  /**
   * get coordinates of address or active address by default
   * @param {object} props - properties provided to the component
   * @param {number} index - index of address for what coordinates should be returned
   * @return {List} coordinates or empty list if active address does not exist
   */
  getCoordinatesOfAddress = (props, index) => {
    let address
    if (props && props.addresses) {
      address = typeof index === 'number' && index > -1 && props.addresses.get(index) ||
        props.activeIndex !== -1 && props.addresses.get(props.activeIndex) || null
      return address && address.get('coordinates') || List()
    }

    return List()
  }

  /**
   * get address on index or active by default
   * @param {object} props - properties received within the component
   * @param {?number} index - index of address to be returned
   * @return {Map} if index provided, then the map of that index is returned,
   *  otherwise active address by default is returned, if active not exist, then null
   */
  getAddress = (props, index) => {
    if (props && props.addresses) {
      return typeof index === 'number' && index > -1 && props.addresses.get(index) ||
        props.activeIndex !== -1 && props.addresses.get(props.activeIndex) || null
    }

    return null
  }

  /**
   * Sets the active marker, do all needed steps for that
   * @param {object} props = properties provided
   */
  setActiveMarker = (props, index) => {
    if (props) {
      const lastActiveCoordinate = getData(ComponentDataType.ACTIVE_COORDINATE, props.id)
      lastActiveCoordinate && this.removeMarker(props.id, lastActiveCoordinate.get('id'))
      lastActiveCoordinate && this.addMarker(props.id, lastActiveCoordinate)
      const coordinates = this.getCoordinatesOfAddress(props, index)
      const coordinate = this.getCoordinateToBeAddedOnMap(coordinates)
      if (coordinate) {
        this.removeMarker(props.id, coordinate.get('id'))
        this.addMarker(props.id, coordinate, Colors.ACTIVE_COLOR, MarkerSizes.ACTIVE_MARKER)
        setData(ComponentDataType.ACTIVE_COORDINATE, coordinate, props.id)
        this.showDialogForMarker(props, coordinate.get('id'))
        // workaround, show dialog does a automatic move only for first added point ever on the map, but not for others
        // and when move is called for first index, then the move is randomly wrong
        props.activeIndex !== 0 && this.moveToPoint(props.id, coordinate)
      }
    } else {
      console.log('SetActiveMarker: No properties provided!')
    }
  }

  /**
   * Gets the coordiante based on fallback - AR , if AR not exist then user point,
   * if user point not exist then main, if not main then undefined
   * @param {?List} coordinates - list of coordinates
   * @return {Map} coordinate - if not exist then undefined
   */
  getCoordinateToBeAddedOnMap = (coordinates) => {
    if (coordinates) {
      const mainCoordinate = coordinates.filter(c => c.get('isMain')).first()
      const enteredByuser = coordinates.filter(c => c.get('coordinateState').toLowerCase() === 'enteredbyuser').first()
      const enteredByAR = coordinates.filter(c => c.get('coordinateState').toLowerCase() === 'enteredbyar').first()
      return enteredByAR || enteredByuser || mainCoordinate
    }

    return undefined
  }

  /**
   * Remove marker for given marker id or all markers
   * @param {?string} idOfMapComponent - id of the map component
   * @param {?string} markerId = id of the marker
   */
  removeMarker = (idOfMapComponent, markerId) => {
    if (idOfMapComponent) {
      const localMap = getData(ComponentDataType.MAP, idOfMapComponent)
      const dataForRemove = markerId && [markerId] || []
      localMap.postRequest(MapActions.REMOVE_MARKER, dataForRemove)
    } else {
      console.error('RemoveMarker: No idOfMapComponent provided!')
    }
  }

  /**
   * Add marker to the map component
   * @param {?string} idOfMapComponent - id of the map component
   * @param {?Map} point - point, that will be add to map component
   * @param {?string} color - color of the marker, default is inactive
   * @param {?string} size - size of the marker, default is 7
   */
  addMarker = (idOfMapComponent, point, color = Colors.INACTIVE_COLOR, size = MarkerSizes.INACTIVE_MARKER) => {
    if (idOfMapComponent) {
      const localMap = getData(ComponentDataType.MAP, idOfMapComponent)
      if (point) {
        localMap.postRequest(MapActions.ADD_MARKER, [{ x: point.get('longitude'),
          y: point.get('latitude'),
          color: color,
          msg : '',
          shape: 2, // icon number (0-6)
          size: size }, point.get('id')])
      } else {
        console.error('AddMarker: No point provided!')
      }
    } else {
      console.error('AddMarker: No idOfMapComponent provided!')
    }
  }

  /**
   * Add all points of all addresses to the map
   * @param {?Map} props = properties provided
   * @param {?boolean} moveToActivePoint - indicates, whether it should move to active point or not
   * @param {?boolean} omitActive - indicates, whether active point should be added or not
   */
  addAllPoints = (props, moveToActivePoint, omitActive) => {
    setData(ComponentDataType.ACTIVE_COORDINATE, undefined, props.id)
    if (props.addresses) {
      let anyCoordinates
      props.addresses.forEach((address, key) => {
        if (address) {
          const isActive = props.activeIndex === key
          if (omitActive && isActive) {
            return true
          }
          const coordinates = address.get('coordinates')
          anyCoordinates = !anyCoordinates && coordinates || anyCoordinates
          this.addPointsOfAddress(coordinates, props.id, isActive)
        }
      })

      // active coordinate should be added as last to be shown on front of all other markers
      const activeCoordinate = getData(ComponentDataType.ACTIVE_COORDINATE, props.id)
      !omitActive && activeCoordinate && this.addMarker(props.id, activeCoordinate,
        Colors.ACTIVE_COLOR, MarkerSizes.ACTIVE_MARKER)

      activeCoordinate && !omitActive && moveToActivePoint &&
        this.showDialogForMarker(props, activeCoordinate.get('id'))
      activeCoordinate && !omitActive && moveToActivePoint && this.moveToPoint(props.id, activeCoordinate)

      if (props.disabled) {
        const anyCoordinate = anyCoordinates &&
          this.getCoordinateToBeAddedOnMap(anyCoordinates.filter(coordinate => this.isCoordinateValid(coordinate)))
        anyCoordinate && this.moveToPoint(props.id, anyCoordinate)
      }
    }
  }

  /**
   * Move to given point
   * @param {?string} idOfMapComponent - id of the map component
   * @param {?Map} point - point, that will be add to map component
   */
  moveToPoint = (idOfMapComponent, point) => {
    const localMap = getData(ComponentDataType.MAP, idOfMapComponent)
    if (point) {
      setTimeout(function () {
        localMap.postRequest(MapActions.MOVE_TO_POINT, [point.get('longitude'), point.get('latitude'), 11])
      }, 1000)
    } else {
      console.log('MoveToPoint: No coordinate provided!')
    }
  }

  /**
   * Add point from given list of points
   * @param {?List} coordinates - list of coordinates (basically coordinates of one address)
   * @param {?string} idOfMapComponent - id of the map component
   * @param {?boolean} isActive - indicates, whether coordinates belogns to active address
   */
  addPointsOfAddress = (coordinates, idOfMapComponent, isActive) => {
    if (coordinates) {
      const coordinate = this.getCoordinateToBeAddedOnMap(coordinates)

      if (coordinate) {
        isActive ? setData(ComponentDataType.ACTIVE_COORDINATE, coordinate, idOfMapComponent)
          : this.addMarker(idOfMapComponent, coordinate)
      }
    }
  }

  /**
   * Gets the index of the address for given point id
   * @param {List} addresses - list of the addresses
   * @param {string} pointId - id of the point
   * @return {number} index of the address, otherwise null
   */
  getAddressIndexOfPointId = (addresses, pointId) => {
    let activeIndex = -1
    if (addresses) {
      addresses.forEach((address, addressIndex) => {
        const coordinates = address.get('coordinates')
        if (coordinates) {
          return coordinates.forEach((coordinate) => {
            if (pointId === coordinate.get('id')) {
              activeIndex = addressIndex
              return false
            }
          })
        }
      })
    }

    return activeIndex !== -1 ? activeIndex : null
  }

  /**
   * Tries to search position on the map, according the basic information
   * @param {?Map} props = properties provided
   */
  trySearchAndMove = (props) => {
    const activeAddress = this.getAddress(props)
    if (activeAddress) {
      const { street: streetId,
        postalCode: postalCodeId
      } = activeAddress.toJS()
      const postalCode = props.postalCodes.get(postalCodeId) || Map()
      const municpalityId = postalCode.get('municipalityId') || ''
      const centerCoordinate = postalCode.get('centerCoordinate')
      const municipality = props.municipalities.get(municpalityId)
      const localMap = getData(ComponentDataType.MAP, props.id)

      if (centerCoordinate) {
        setTimeout(function () {
          localMap.postRequest(MapActions.MOVE_TO_POINT,
            [centerCoordinate.get('longitude'), centerCoordinate.get('latitude'), 11]
          )
        }, 1000)
      } else if (municipality) {
        const streetName = this.props.translateStreetName(streetId)
        const query = `${streetName} ${municipality.get('name')}`
        /* (props.addressInfo.streetNumber ? props.addressInfo.streetNumber + ' ' : '') + */

        localMap.postRequest(MapActions.SEARCH_ADDRESS, [query])
      } else {
        console.log('PTV: No municipality provided')
        this.moveToFirstValid(props)
      }
    } else {
      console.log('PTV: No address provided')
      this.moveToFirstValid(props)
    }
  }

  /**
   * Moves to the first point from address
   * @param {?Map} props = properties provided
   */
  moveToFirst = props => {
    const coordinates = this.getCoordinatesOfAddress(props)
    if (coordinates.size > 0) {
      const localMap = getData(ComponentDataType.MAP, props.id)
      const coordinate = coordinates.size > 1 ? coordinates.filter(c => !c.get('isMain')).first()
        : coordinates.first()

      setTimeout(function () {
        localMap.postRequest(MapActions.MOVE_TO_POINT, [coordinate.get('longitude'), coordinate.get('latitude'), 11])
      }, 1000)
    }
  }

  /**
   * Checks whether given coordinate has been properly loaded and is valid
   * @param {?Map} coordiante = given coordinate
   */
  isCoordinateValid = coordinate => {
    const state = coordinate.get('coordinateState').toLowerCase()
    return state === 'ok' || state === 'enteredbyuser' || state === 'enteredbyar'
  }

  /**
   * Gets the first valid coordinate if any exists
   * @param {?Map} props = properties provided
   */
  getFirstValidCoordinate = props => {
    if (!props || !props.addresses || props.addresses.size < 1) {
      return null
    }

    for (let i = 0; i < props.addresses.size; i++) {
      const coordinates = this.getCoordinatesOfAddress(props, i)
      const validCoordinates = coordinates && coordinates.filter(coordinate => this.isCoordinateValid(coordinate))

      if (validCoordinates && validCoordinates.size > 0) {
        return validCoordinates.first()
      }
    }

    return null
  }

  /**
   * Moves to the first valid point on the map
   * @param {?Map} props = properties provided
   */
  moveToFirstValid = props => {
    const firstValidCoordinate = this.getFirstValidCoordinate(props)

    if (firstValidCoordinate) {
      const localMap = getData(ComponentDataType.MAP, props.id)

      setTimeout(function () {
        localMap.postRequest(MapActions.MOVE_TO_POINT, [firstValidCoordinate.get('longitude'), firstValidCoordinate.get('latitude'), 11])
      }, 1000)
    }
  }

  /**
   * Checks whether coordinates are valid
   * @param {?Map} props = properties provided
   */
  isSomeCoordinateValid = (props) => {
    if (props.addresses) {
      const activeAddress = props.addresses.get(props.activeIndex)
      if (activeAddress) {
        const streetType = (activeAddress.get('streetType') || '').toLowerCase()
        const coordinates = this.getCoordinatesOfAddress(props)
        if (streetType === 'other') {
          return coordinates && coordinates.some(c => {
            const state = c.get('coordinateState').toLowerCase()
            return state === 'ok' || state === 'enteredbyuser'
          })
        }

        return coordinates && coordinates.some(c => this.isCoordinateValid(c))
      }
    }
    return false
  }

  componentWillMount () {
    this.props.actions.loadMap(this.props.id)
  }

  componentWillUnmount () {
    if (componentData) {
      console.log('PTV:cleaning')
      const localMap = getData(ComponentDataType.MAP, this.props.id)
      localMap && localMap.destroy()
      componentData = componentData.set(this.props.id, Map())
      setData(ComponentDataType.INIT, true, this.props.id)
    }
  }

  /**
   * Initials all points in the map
   * @param {?Map} props = properties provided
   * @param {?boolean} moveToActivePoint - indicates, whether it should move to active point or not
   * @param {?boolean} omitActive - indicates, whether active point should be added or not
   */
  intitAllPoints = (props, moveToActivePoint, omitActive) => {
    this.removeMarker(props.id)
    this.hideMarekerInfo(props.id)
    this.addAllPoints(props, moveToActivePoint, omitActive)
  }

  componentWillReceiveProps (nextProps) {
    const isMapInitialized = getData(ComponentDataType.INIT, nextProps.id)
    if (!nextProps.disabled && isMapInitialized !== undefined && !isMapInitialized) {
      if (this.props.activeIndex !== nextProps.activeIndex) {
        this.setActiveMarker(nextProps)
      } else if (getData(ComponentDataType.ONCLICK, nextProps.id)) {
        const oldCoordinate = this.getCoordinateToBeAddedOnMap(this.getCoordinatesOfAddress(this.props))
        const newCoordinate = this.getCoordinateToBeAddedOnMap(this.getCoordinatesOfAddress(nextProps))
        oldCoordinate && this.removeMarker(this.props.id, oldCoordinate.get('id'))
        newCoordinate && this.addMarker(nextProps.id, newCoordinate, Colors.ACTIVE_COLOR, MarkerSizes.ACTIVE_MARKER)
        newCoordinate && this.showDialogForMarker(nextProps, newCoordinate.get('id'))
        setData(ComponentDataType.ONCLICK, false, nextProps.id)
        newCoordinate && setData(ComponentDataType.ACTIVE_COORDINATE, newCoordinate, nextProps.id)
      } else if ((this.props.addresses.size > nextProps.addresses.size) || this.isSomeCoordinateValid(nextProps)) {
        this.intitAllPoints(nextProps, true)
      } else {
        this.hideMarekerInfo(nextProps.id)
        this.intitAllPoints(nextProps, false, true)
        this.trySearchAndMove(nextProps)
      }
    }
  }

  /**
   * Hide popup for the dialog
   * @param {?string} idOfMapComponent - id of the map component
   * @param {string} markerId - id of the point for what the dialog should be shown
   */
  hideMarekerInfo = (idOfMapComponent, markerId) => {
    if (idOfMapComponent) {
      const localMap = getData(ComponentDataType.MAP, idOfMapComponent)
      const dataForRemove = markerId && [markerId] || []
      localMap.postRequest(MapActions.HIDE_MARKER_INFO, dataForRemove)
    }
  }

  /**
   * Shows popup for the dialog
   * @param {?Map} props = properties provided to component
   * @param {string} pointId - id of the point for what the dialog should be shown
   */
  showDialogForMarker = (props, pointId) => {
    const addressIndexOfThePoint = this.getAddressIndexOfPointId(props.addresses, pointId)
    const address = this.getAddress(props, addressIndexOfThePoint)

    if (address) {
      const coordinates = address.get('coordinates') || List()
      const isSomeMain = coordinates.some(c => c.get('isMain'))
      const point = coordinates.filter(p => p.get('id') === pointId).first()
      if (point) {
        const { intl: { formatMessage },
          disabled
        } = props
        const isEditEnabled = this.getIisEdditingEnabledForIndex(props)
        const { street: streetId,
          streetNumber,
          postalCode,
          streetType
          // municipality
        } = address.toJS()
        const postalCodeEntity = props.postalCodes.get(postalCode) || Map()
        const isMain = point.get('isMain')
        const street = (streetId || streetNumber) &&
          (this.props.translateStreetName(streetId) + ' ' + (streetNumber || ''))
        const municipalityInfo = postalCodeEntity && ((postalCodeEntity.get('code') || '') + ' ' +
          (postalCodeEntity.get('postOffice') /* (municipality.get('name') */ || ''))
        const coordinatesInfo = (point.get('latitude') || '') + ', ' + (point.get('longitude') || '')
        const action1 = isEditEnabled ? PopupActions.DISABLE_EDIT : PopupActions.ENABLE_EDIT
        const movedMessage = formatMessage(messages.movedPointTitle, {
          latitude: point.get('latitude') || '',
          longitude: point.get('longitude') || ''
        })
        const displayStreet = (streetType || 'street').toLowerCase() === 'street'

        const action1Message = isEditEnabled
          ? formatMessage(messages.saveCoordinateLinkTitle) : formatMessage(messages.editCoordinateLinkTitle)
        const content = [{
          html: `<div>` +
          ((displayStreet && street) ? `<b>${street}</b>` : '') +
          (municipalityInfo ? `<p>${municipalityInfo}</p>` : '') +
              (!isMain ? isEditEnabled ? `<p>${movedMessage}</p>`
                : `<p>${formatMessage(messages.savedMovedPointTitle)}</p>` +
                  `<p>${formatMessage(messages.coordinatesInfoTitle)}</p>` +
                  `<a style='color: inherit; text-decoration: none; cursor: default;'>${coordinatesInfo}</a>`
                : `<p>${formatMessage(messages.coordinatesInfoTitle)}</p>` +
              `<a style='color: inherit; text-decoration: none; cursor: default;'>${coordinatesInfo}</a>`) +
            `</div>`
        } ]

        !disabled && (content[0].actions = [
          {
            name: `${action1Message}`,
            type: 'link',
            action: {
              action: action1,
              pointId: point.get('id')
            }
          }
        ])
        // if is main - user should see message, that he can change the entrance
        !disabled &&
      isMain &&
      content.push({ html: `<div>${formatMessage(messages.addNewUserPointTitle)}</div>` })

        !disabled && !isMain && isEditEnabled && isSomeMain && content[0].actions.push({
          name: `${formatMessage(messages.returnMainCoordinateBackLinkTitle)}`,
          type: 'link',
          action: {
            action: PopupActions.RETURN_TO_MAIN,
            pointId: point.get('id')
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

        const localMap = getData(ComponentDataType.MAP, props.id)
        localMap.postRequest(MapActions.SHOW_MARKER_INFO, infoboxData)
      }
    }
  }

  showSupportedFunctionality = () => {
    const localMap = getData(ComponentDataType.MAP, this.props.id)
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

  /**
   * Adds handling of the events
   */
  addHandlingOfEvents = () => {
    const localMap = getData(ComponentDataType.MAP, this.props.id)
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
    // this.showDialogForMarker(getData('props'), data.id)
  }

  handleAfterMapMoveEvent = (data) => {

  }

  /**
   * Handles actions done on popup
   * @param {object} data - data, that containse type of action and the point id
   */
  handleInfoboxActionEvent = (data) => {
    const { actionParams: { action, pointId } } = data
    switch (action) {
      case PopupActions.ENABLE_EDIT: {
        this.setEdditingSateForIndex(this.props, true)
        break
      }
      case PopupActions.DISABLE_EDIT: {
        this.setEdditingSateForIndex(this.props)
        break
      }
      case PopupActions.RETURN_TO_MAIN: {
        this.props.onResetToDefaultClick(this.props.id)
        break
      }
    }
    this.showDialogForMarker(this.props, pointId)
  }

  handleDrawingEvent = (data) => {

  }

  handleFeatureEvent = (data) => {

  }

  handleFeedbackResultEvent = (data) => {

  }

  /**
   * Gets the availability of edditing for index, by default active index is used
   * @param {object} props - properties given to the component
   * @param {number} index - index of address
   * @return true if edit is enabled otherwise false
   */
  getIisEdditingEnabledForIndex = (props, index) => {
    const innerIndex = typeof index === 'number' ? index : props.activeIndex
    const editAvailability = getData(ComponentDataType.POINTS_EDIT_AVAILABILITY, props.id) || List()
    if (innerIndex > -1) {
      const coordinates = this.getCoordinatesOfAddress(props, innerIndex)
      return !props.disabled && (editAvailability.get(innerIndex) || coordinates.size === 0)
    }

    return false
  }

  /**
   * Sets the availability of edditing, for index or to active index by default
   * @param {object} props - properties given to the component
   * @param {boolean} availability - true or false, default is false
   * @param {number} index - index of address
   */
  setEdditingSateForIndex = (props, availability = false, index) => {
    const innerIndex = typeof index === 'number' ? index : props.activeIndex
    let editAvailability = getData(ComponentDataType.POINTS_EDIT_AVAILABILITY, props.id) || List()
    editAvailability = editAvailability.set(innerIndex, availability)
    setData(ComponentDataType.POINTS_EDIT_AVAILABILITY, editAvailability, props.id)
  }

  /**
   * Handle click on map
   * @param {object} data - contains longitude and latitude of the point
   */
  handleMapClickedEvent = (data) => {
    if (this.getIisEdditingEnabledForIndex(this.props)) {
      setData(ComponentDataType.ONCLICK, true, this.props.id)
      const id = shortId.generate()
      const point = { longitude: data.lon,
        latitude: data.lat,
        id: id }
      this.setEdditingSateForIndex(this.props)
      this.props.onMapClick(point)
    }
  }

  /**
   * Handle click on marker
   * @param {object} data - contains id of the marker
   */
  handleMarkerClickEvent = (data) => {
    const activeIndex = this.getAddressIndexOfPointId(this.props.addresses, data.id)
    if (!this.props.disabled) {
      if (typeof activeIndex === 'number' && this.props.setActiveAddress) {
        if (activeIndex === this.props.activeIndex) {
          this.showDialogForMarker(this.props, data.id)
        } else {
          this.props.setActiveAddress(activeIndex)
        }
      }
    } else {
      this.setActiveMarker(this.props, activeIndex)
    }
  }

  /**
   * Handle result of the  search
   * @param {object} - data - data that, contains info about search event
   */
  handleSearchResultEvent = (data) => {
    if (data.success) {
      let point = data.result.locations[0]
      if (point) {
        const localMap = getData(ComponentDataType.MAP, this.props.id)
        localMap.postRequest(MapActions.MOVE_TO_POINT, [point.lon, point.lat, 11])
        this.setEdditingSateForIndex(this.props, true)
      }
    }
  }

  handleAfterUserLocationEvent = (data) => {
  }

  initMapObject = props => {
    let localMap = getData(ComponentDataType.MAP, this.props.id)
    if (localMap && localMap.isReady()) {
      console.log('PTV:destroyForInit')
      localMap.destroy()
    }
    console.log('PTV:initMapObject:' + props.target.id)
    const iFrame = getData(ComponentDataType.IFRAME, this.props.id)
    localMap = Channel.connect(iFrame, this.props.mapDNS.origin)
    setData(ComponentDataType.MAP, localMap, this.props.id)
    localMap.onReady(this.onReadyCallback)
  }

  mapref = input => {
    setData(ComponentDataType.INIT, true, this.props.id)
    if (input && input.id) {
      setData(ComponentDataType.IFRAME, input, this.props.id)
    }
  }

  render () {
    return (
      <div key={'container_' + this.props.id} id={'container_id_' + this.props.id} className={styles.map}>
        {this.props.isLoading && <Spinner />}
        <iframe key={'map_' + this.props.id} onLoad={this.initMapObject}
          style={{ border: 'none', width: '100%', height: '500px' }}
          ref={this.mapref}
          id={'map_id_' + this.props.id} src={this.props.mapDNS.href} />
      </div>
    )
  }
}

MapComponent.propTypes = {
  addresses: ImmutablePropTypes.list.isRequired,
  onMapClick: PropTypes.func.isRequired,
  onResetToDefaultClick: PropTypes.func.isRequired,
  activeIndex: PropTypes.number,
  mapDNS: PropTypes.object.isRequired,
  actions: PropTypes.object.isRequired,
  id: PropTypes.string.isRequired,
  setActiveAddress: PropTypes.func.isRequired,
  isLoading: PropTypes.bool.isRequired,
  disabled: PropTypes.bool.isRequired,
  translateStreetName: PropTypes.func
}

export default compose(
  connect(
    (state, ownProps) => ({
      isLoading: getMapIsLoading(state, ownProps),
      mapDNS: getMapDNSURL(state, ownProps),
      translateStreetName: getLocalizedStreetName(state, ownProps)
    }),
    mapDispatchToProps([{ loadMap,
      mapIsLoaded }])
  ),
  injectIntl
)(MapComponent)
