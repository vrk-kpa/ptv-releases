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
import React, { Component, PropTypes } from 'react'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'

import ImmutablePropTypes from 'react-immutable-proptypes'
import * as PTVValidatorTypes from '../../../Components/PTVValidators'
import { PTVIcon, PTVLabel, PTVTextInputNotEmpty, PTVTextAreaNotEmpty, PTVRadioGroup, PTVPreloader, PTVAddItem, PTVTooltip } from '../../../Components'
import { ButtonAdd } from '../Buttons'
import PostalCodeContainer from './PostalCodeContainer'
import { callApiDirect } from '../../../Middleware/Api'
import AddressMap from './addressMap'

// selectors
import * as CommonSelectors from '../Selectors'

// schemas
import { CommonSchemas } from '../../Common/Schemas'

// actions
import * as commonActions from '../Actions'
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps'

const actions = [
  commonActions,
  {updateCoordinates: ({
    coordinatesHidden, addressId, language
  }) => (
    props
  ) => {
    const state = props.getState()
    const internalProps = { id: addressId, language }
    const isFilled = CommonSelectors.getIsAddressFilled(state, internalProps)
    const addressInfo = CommonSelectors.getAddressInfoForCoordinates(state, internalProps)
    if (!coordinatesHidden && isFilled) {
      return commonActions.apiCall(
        ['addresses', addressId],
        { endpoint: 'common/GetCoordinatesForAddress', data: { language, ...addressInfo } },
        [],
        CommonSchemas.ADDRESS,
        addressId,
        true,
        true
      )(props)
    }
  }
}
]

// enums
// import { streetAddressTypes } from '../Enums';

const messages = defineMessages({
  streetTitle : {
    id: 'Containers.Channels.Address.Street.Title',
    defaultMessage: 'Katuosoite'
  },
  streetNumberTitle : {
    id: 'Containers.Channels.Address.StreetNumber.Title',
    defaultMessage: 'Katunumero'
  },
  streetPlaceholder : {
    id: 'Containers.Channels.Address.Street.Placeholder',
    defaultMessage: 'esim. Mannerheimintie'
  },
  streetNumberPlaceholder : {
    id: 'Containers.Channels.Address.StreetNumber.Placeholder',
    defaultMessage: 'esim. 12 A 23'
  },
  poboxTitle : {
    id: 'Containers.Channels.Address.POBox.Title',
    defaultMessage: 'Postilokero-osoite'
  },
  streetPoboxTitle : {
    id: 'Containers.Channels.Address.StreetPOBox.Title',
    defaultMessage: 'Katuosoite / postilokero'
  },
  poboxPlaceholder : {
    id: 'Containers.Channels.Address.POBox.Placeholder',
    defaultMessage: 'esim. PL-205'
  },
  streetAddressType: {
    id: 'Containers.Channels.Address.Type.Street',
    defaultMessage: 'Katuosoite'
  },
  poboxAddressType: {
    id: 'Containers.Channels.Address.Type.POBox',
    defaultMessage: 'Postilokero-osoite'
  },
  additionalInformation: {
    id: 'Containers.Channels.Address.AdditionalInformation.Title',
    defaultMessage: 'Osoitteen lisätiedot'
  },
  additionalInformationPlaceholder: {
    id: 'Containers.Channels.Address.AdditionalInformation.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Title',
    defaultMessage: 'Toimitusosoite'
  },
  descriptionTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.Title',
    defaultMessage: 'Sanallinen kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.Tooltip',
    defaultMessage: 'Sanallinen kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.DeliveryAddress.Description.PlaceHolder',
    defaultMessage: 'Kirjoita sanallinen kuvaus'
  },
  mapCoordinatesTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.Title',
    defaultMessage: 'Osoitteen perusteella haetut sijaintikoordinaatit: {latitude}, {longitude}'
  },
  mapCoordinatesNotReceivedTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.NotReceived.Title',
    defaultMessage: 'Antamaasi osoitetta ei löytynyt.'
  },
  mapCoordinatesOpenMapTitle: {
    id: 'AddressContainer.OpenMap.Title',
    defaultMessage: 'Näytä kartaa'
  },
  mapCoordinatesOpenMapInfo: {
    id: 'AddressContainer.OpenMap.Info',
    defaultMessage: 'Jos käyntiosoite ei anna kartalla tarkkaa sisäänkäynnin sijaintia, voitluoda sen kartalla'
  },
  mapCoordinatesOpenMapAddressNotValid: {
    id: 'AddressContainer.OpenMap.AddressNotValid',
    defaultMessage: 'Täytä kentät: kadunimi, osoitenumero ja postinumero'
  }
})

const streetAddressTypes = [
  {
    id: 'Street',
    message: messages.streetAddressType
  },
  {
    id: 'PostBox',
    message: messages.poboxAddressType
  }]

const validators = [PTVValidatorTypes.IS_REQUIRED]

const Coordinates = connect(mapStateToCoordinatesProps, mapDispatchToProps(actions))(injectIntl(({
  coordinateState,
  longitude,
  latitude,
  coordinatesIsLoading,
  canBeShown,
  intl,
  positionId,
  readOnly,
  language,
  addressId,
  translationMode,
  addressInfo,
  isFilled,
  isValid,
  coordinatesHidden,
  actions: { onLocalizedEntityInputChange, updateCoordinates }
}) => {
  const renderMap = () => {
    return <AddressMap id={addressId}
      language={language}
      coordinatesFetching={coordinatesIsLoading}
      positionId={positionId}
      translationMode={translationMode}
      readOnly={readOnly && translationMode === 'none'} />
  }

  const handleAddMapClick = () => {
    if (isFilled) {
      onLocalizedEntityInputChange('addresses', addressId, 'addressIsValid', true, language)
      updateCoordinates({ coordinatesHidden, addressId, language })
    } else {
      onLocalizedEntityInputChange('addresses', addressId, 'addressIsValid', false, language)
    }
  }

  return (
    <div>
    {coordinatesIsLoading
      ? <div className='row'>
        <PTVPreloader className='col-md-12 left' small />
      </div>
      : canBeShown
        ? <div className='row'>
          <PTVLabel labelClass='col-md-12 disabled'>
            {intl.formatMessage(messages.mapCoordinatesTitle, { latitude, longitude })}
          </PTVLabel>
        </div>
        : coordinateState === 'notreceived'
          ? <div className='row'>
            <PTVLabel labelClass='col-md-12 has-error'>
              {intl.formatMessage(messages.mapCoordinatesNotReceivedTitle)}
            </PTVLabel>
          </div>
          : <div className="form-group">
            <ButtonAdd
              onClick={handleAddMapClick}
              className='button-link'
            >
                { intl.formatMessage(messages.mapCoordinatesOpenMapTitle) }

              <PTVIcon
                name='icon-angle-down'
                componentClass='top-align'
              />
            </ButtonAdd>
            <PTVLabel>
              <PTVTooltip tooltip={intl.formatMessage(messages.mapCoordinatesOpenMapInfo)} />
            </PTVLabel>
            {isValid ? null : <div className="error">{intl.formatMessage(messages.mapCoordinatesOpenMapAddressNotValid)}</div>}
          </div>}
          {canBeShown ? <div className='row'>
            <div className='col-md-12'>
            <PTVAddItem
              readOnly={readOnly && translationMode === 'none'}
              renderItemContent={renderMap}
              messages={{ 'label': intl.formatMessage(messages.mapCoordinatesOpenMapTitle) }}
              collapsible={translationMode === 'none'}
              multiple={false} />
          </div>
        </div> : null}
          </div>
  )
}))

function mapStateToCoordinatesProps (state, ownProps) {
  const props = { id: ownProps.addressId, language: ownProps.language, keyToState: ownProps.keyToState }
  const coordinateState = CommonSelectors.getAddressStreetCoordinatesState(state, props)
  const coordinatesIsLoading = coordinateState.indexOf('loading') !== -1 || CommonSelectors.getAddressIsFetching(state, props)
  const longitude = CommonSelectors.getAddressStreetLongitude(state, props)
  const latitude = CommonSelectors.getAddressStreetLatitude(state, props)
  const isFilled = CommonSelectors.getIsAddressFilled(state, props)
  const canBeShown = coordinateState === 'ok' && isFilled && latitude != null && longitude != null
  return {
    addressInfo: CommonSelectors.getAddressInfoForCoordinates(state, props),
    coordinateState,
    longitude,
    latitude,
    coordinatesIsLoading,
    canBeShown,
    isFilled,
    isValid: CommonSelectors.getIsAddressValid(state, props)
  }
}

class StreetAddressContainer extends Component {

  constructor (props) {
    super(props)
  }

  renderTypeSelection = () => {
    if (!this.props.showTypeSelection) {
      return null
    }

    return (
      <div className='row'>
        <PTVRadioGroup
          name='StreetAddressType'
          value={this.props.streetType}
          onChange={this.onInputChange('streetType')}
          items={streetAddressTypes}
          className='col-lg-5'
          useFormatMessageData
          readOnly={this.props.readOnly || this.props.translationMode == 'view' || this.props.translationMode == 'edit'}
                    />
      </div>
    )
  }

  componentDidMount = () => {
    this.loadCoordinates()
  }

  componentDidUpdate = (prevProps) => {
    this.loadCoordinates()
  }

  loadCoordinates = () => {
    if (this.props.coordinatesHidden || !this.props.readOnly) return
    // max attempts to get coordinates: 5 (move to CONST config once available)
    if (this.props.coordinateState.indexOf('loading') !== -1 && this.props.attemptCount < 5 && !this.props.addressIsFetching) {
      this.props.actions.apiCall(
        ['addresses', this.props.addressId],
        { endpoint: 'common/GetAddress', data: { Id: this.props.addressId, language: this.props.language, attemptCount: this.props.attemptCount + 1 } },
        [],
        CommonSchemas.ADDRESS,
        null,
        null,
        true
      )
    }
    if (this.props.coordinateState.indexOf('loading') === -1) {
      this.props.actions.clearApiCall(['addresses', this.props.addressId], { model: { attemptCount: 0 } })
    }
  }

  getAddressTextFieldProps = (type, formatMessage, componentClass, readOnly) => {
    let props = {
      componentClass,
      label: formatMessage(messages.streetTitle),
      validatedField: messages.streetTitle,
      placeholder: formatMessage(messages.streetPlaceholder),
      tooltip: formatMessage(this.props.customMessages.tooltip),
      name: 'streetTitle',
      blurCallback: this.onInputChange('street'),
      maxLength: 100,
      value: this.props.street,
      readOnly: this.props.readOnly && this.props.translationMode == 'none',
      disabled: this.props.translationMode == 'view'

    }
    switch (type) {
      case 'PostBox':
        props.label = formatMessage(messages.poboxTitle),
        props.validatedField = messages.poboxTitle,
                props.placeholder = formatMessage(messages.poboxPlaceholder),
                props.name = 'poboxTitle',
                props.blurCallback = this.onInputChange('poBox'),
                props.maxLength = 30,
                props.value = this.props.poBox
        return props
      case 'StreetNumber':
        props.label = formatMessage(messages.streetNumberTitle),
                props.placeholder = formatMessage(messages.streetNumberPlaceholder),
                props.name = 'streetNumber',
                props.blurCallback = this.onInputChange('streetNumber'),
                props.maxLength = 30,
                props.tooltip = null,
                props.value = this.props.streetNumber
        props.readOnly = readOnly || this.props.translationMode == 'view' || this.props.translationMode == 'edit'
        return props
      case 'Both':
            // now it is similar as street
        props.label = formatMessage(messages.streetPoboxTitle)
        props.validatedField = messages.streetPoboxTitle
        props.value = [this.props.street, this.props.poBox].filter(x => x).join(' / ')
        return props
    }
        // default is street
    return props
  }

  renderAddressTextFields = () => {
    const streetClass = this.props.splitContainer ? 'col-md-9' : 'col-md-4'
    const streetNumberClass = this.props.splitContainer ? 'col-md-3' : 'col-md-2'
    const props = this.getAddressTextFieldProps(this.props.streetType, this.props.intl.formatMessage, streetClass, this.props.readOnly)
    const propsStreetNumber = this.getAddressTextFieldProps('StreetNumber', this.props.intl.formatMessage, streetNumberClass, this.props.readOnly)
    props.validators = this.props.isAddressManadatory ? validators : []
    return (
      <div>
        {this.renderAddressTextField(props)}
        {this.props.streetType != 'PostBox' ? this.renderAddressTextField(propsStreetNumber) : null}
      </div>
    )
  }

  renderAddressTextField = (props) => {
    return (
      <PTVTextInputNotEmpty
        {...props}
      />
    )
  }

  onInputChangeAfter = () => {
    this.props.actions.updateCoordinates(this.props)
  }

  onInputChange = input => (value, object) => {
    if (this.props.isNew) {
      this.props.onAddAddress([{
        id: this.props.addressId,
        [input]: value
      }])
    } else {
      this.props.actions.onLocalizedEntityInputChange('addresses', this.props.addressId, input, value, this.props.language)
    }

    this.onInputChangeAfter()
  }

  render () {
    const { readOnly, language, translationMode, splitContainer, addressId, actions,
    showTypeSelection, isAdditionalInformationVisible, isAdditionalInformationTextAreaVisible, customMessages, additionalInformation, streetType, street, isNew,
    poBox, streetNumber, coordinateState, coordinatesHidden, intl, onAddAddress, isAddressManadatory, latitude, longitude, isMunicipalityVisible,
    addressIsFetching, attemptCount } = this.props

    return (
      <div className='street-address item-row'>
        { !(readOnly || translationMode == 'view' || translationMode == 'edit') ? this.renderTypeSelection(showTypeSelection, streetType) : null }
        <div className='row'>
          { this.renderAddressTextFields() }
        </div>
        <PostalCodeContainer
          readOnly={readOnly}
          language={language}
          translationMode={translationMode}
          addressId={addressId}
          isNew={isNew}
          afterChange={this.onInputChangeAfter}
          onAddAddress={onAddAddress}
          isAddressManadatory={isAddressManadatory}
          isMunicipalityVisible={isMunicipalityVisible}
              />

        {isAdditionalInformationVisible
              ? <div className='row'>
                <PTVTextInputNotEmpty
                  componentClass={splitContainer ? 'col-md-12' : 'col-md-6'}
                  label={intl.formatMessage(messages.additionalInformation)}
                  placeholder={intl.formatMessage(messages.additionalInformationPlaceholder)}
                  name='additionalInformation'
                  blurCallback={this.onInputChange('additionalInformation')}
                  value={additionalInformation}
                  maxLength={150}
                  readOnly={readOnly && translationMode == 'none'}
                  disabled={translationMode == 'view'}
                  />
              </div>
              : null }
        {isAdditionalInformationTextAreaVisible
              ? <div className='row'>
                <PTVTextAreaNotEmpty
                  componentClass={splitContainer ? 'col-md-6' : 'col-md-12'}
                  label={intl.formatMessage(customMessages.descriptionTitle)}
                  tooltip={intl.formatMessage(customMessages.descriptionTootltip)}
                  placeholder={intl.formatMessage(customMessages.descriptionPlaceholder)}
                  name='additionalInformation'
                  blurCallback={this.onInputChange('additionalInformation')}
                  value={additionalInformation}
                  maxLength={150}
                  readOnly={readOnly && translationMode == 'none'}
                  disabled={translationMode == 'view'}
                  />
              </div>
              : null }

        { coordinatesHidden ? null : <Coordinates {...this.props} /> }
      </div>
    )
  }
}

StreetAddressContainer.propTypes = {
  intl: intlShape.isRequired,
  isAdditionalInformationVisible: PropTypes.bool,
  isMunicipalityVisible: PropTypes.bool,
  showTypeSelection: PropTypes.bool,
  dataKey: PropTypes.string
}

StreetAddressContainer.defaultProps = {
  canBeRemoved: false,
  shouldValidate: true,
  isTypeSelectionEnabled: false,
  isAdditionalInformationVisible: false,
  isMunicipalityVisible: false
}

function mapStateToProps (state, ownProps) {
  const props = { id: ownProps.addressId, language: ownProps.language, keyToState: ownProps.keyToState }
  const street = CommonSelectors.getAddressStreet(state, props)
  const poBox = CommonSelectors.getAddressPOBox(state, props)
  const streetNumber = CommonSelectors.getAddressStreetNumber(state, props)
  const coordinateState = CommonSelectors.getAddressStreetCoordinatesState(state, props)
  const longitude = CommonSelectors.getAddressStreetLongitude(state, props)
  const latitude = CommonSelectors.getAddressStreetLatitude(state, props)
  const additionalInformation = CommonSelectors.getAddressAditionalInformation(state, props)
  const addressIsFetching = CommonSelectors.getAddressIsFetching(state, props)
  const attemptCount = CommonSelectors.getAttemptCount(state, props)
  const isAddressManadatory = ownProps.shouldValidate || (!!((street || poBox || streetNumber || (!ownProps.isAdditionalInformationTextAreaVisible && additionalInformation))))
  return {
    streetType: ownProps.type || CommonSelectors.getAddressStreetType(state, props),
    street,
    poBox,
    streetNumber,
    coordinateState,
    longitude,
    latitude,
    isAddressManadatory,
    additionalInformation,
    addressIsFetching,
    attemptCount
  }
}

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(StreetAddressContainer))
