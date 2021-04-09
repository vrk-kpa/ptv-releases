/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { compose } from 'redux'
import { connect } from 'react-redux'
import withPath from 'util/redux-form/HOC/withPath'
import {
  Accessibility,
  AdditionalInformation,
  AddressNumber,
  Municipality,
  PostalCode,
  PostOffice,
  Street
} from 'util/redux-form/fields'
import { SearchResultPreview } from 'appComponents/ChannelAddressSearch'
import {
  isAddressSearchResultExists
} from 'appComponents/ChannelAddressSearch/selectors'
import WarningMessage from 'appComponents/WarningMessage'

import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { addressUseCasesEnum, entityConcreteTypesEnum } from 'enums'
import { getAddressPostalCode,
  getAddressStreetNumber,
  getAddressId,
  getMunicipalityCode,
  getMunicipality,
  getStreetName,
  getStreetId,
  getAddressStreetNumberRange,
  getCoordinatesNotFoundForAddress,
  getIsFetchingCoordinates
} from './selectors'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getSelectedEntityConcreteType,
  getSelectedEntityId
} from 'selectors/entities/entities'
import Coordinates from '../Coordinates'
import { Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl, defineMessages } from 'util/react-intl'
import { getCoordinates } from './actions'
import { getAddressMainCoordinate } from '../Coordinates/selectors'
import { checkAddressTitleValidity } from 'appComponents/AddressTitle/actions'
import MissingCoordinatesLabel from 'appComponents/MissingCoordinatesLabel'

const messages = defineMessages({
  warningTitle: {
    id: 'Util.ReduxForm.Sections.StreetAddress.Duplicate.WarningTitle',
    defaultMessage: 'Tarkista, onko palvelupaikka jo olemassa.'
  }
})
class StreetAddress extends PureComponent {
  static defaultProps = {
    name: 'streetAddress'
  }
  get shouldShowAccessibility () {
    return this.props.selectedEntityConcreteType === 'serviceLocationChannel' &&
           this.props.selectedEntityId
  }

  useSearchDuplicateAddress = () => {
    return this.props.entityConcreteType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL &&
      this.props.addressUseCase === addressUseCasesEnum.VISITING &&
      this.props.index === 0 && !this.props.selectedEntityId
  }

  showDuplicateSearch = () => this.useSearchDuplicateAddress() && this.props.searchResultExists

  coordinatesSearchAllowed = props => props.coordinatesLoaded || (!props.mapDisabled && props.streetName && props.mCode)

  trySearchCoordinates = (oldArgs, newArgs) => {
    const mergedArgs = { ...oldArgs, ...newArgs, disableLoad: !this.useSearchDuplicateAddress() }

    checkAddressTitleValidity(mergedArgs)

    if (this.coordinatesSearchAllowed(mergedArgs)) {
      mergedArgs.dispatch(getCoordinates(mergedArgs))
    }
  }

  handleOnRangeChanged = ({ postalCodeId, streetNumber, streetNumberRange }) => {
    const mCode = this.props.municipalityCode
    const pCode = postalCodeId || this.props.postalCode
    this.trySearchCoordinates(this.props, { mCode, pCode, streetNumber, streetNumberRange })
  }

  handleOnChange = (newValue) => {
    const mCode = newValue && (this.props.municipalities.get(newValue.municipalityId) || Map()).get('code') || null
    const pCode = newValue && newValue.value

    this.trySearchCoordinates(this.props, { mCode, pCode })
  }

  handleOnStreetBlur = (newNameMap, streetProps) => {
    const mCode = this.props.municipalityCode
    const pCode = this.props.postalCode
    this.trySearchCoordinates(this.props, { streetName: newNameMap, mCode, pCode })
  }

  componentDidMount () {
    if (!this.props.coordinatesLoaded) {
      this.trySearchCoordinates(this.props, { mCode: this.props.municipalityCode, pCode: this.props.postalCode })
    }
  }

  collectionPrefix = (useCase) => {
    switch (useCase) {
      case addressUseCasesEnum.VISITING:
        return 'visitingAddresses'
      case addressUseCasesEnum.POSTAL:
        return 'postalAddresses'
      case addressUseCasesEnum.DELIVERY:
        return 'deliveryAddresses'
      default:
        return ''
    }
  }

  render () {
    const {
      isCompareMode,
      addressUseCase,
      postalCode,
      streetName,
      streetNumber,
      additionalInformationProps,
      entityConcreteType,
      mapDisabled,
      index,
      layoutClass,
      isSoteAddress,
      coordinatesNotFoundForAddress,
      isFetchingCoordinates,
    } = this.props

    const streetAddressLayoutClass = isCompareMode
      ? 'col-lg-24 mb-4'
      : {
        'default': 'col-lg-8 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const addressNumberLayoutClass = isCompareMode
      ? 'col-lg-24'
      : {
        'default': 'col-lg-4',
        'bigConnection': 'col-lg-6'
      }[layoutClass]
    const postalCodeOfficeLayoutClass = isCompareMode
      ? 'col-lg-24'
      : {
        'default': 'col-lg-16 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-24 mb-2 mb-lg-0'
      }[layoutClass]
    const postalCodeLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-12 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const postalOfficeLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-12 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const municipalityLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-8 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-12 mb-2 mb-lg-0'
      }[layoutClass]
    const municipalityInnerLayoutClass = isCompareMode
      ? 'col-lg-24 mb-2'
      : {
        'default': 'col-lg-24 mb-2 mb-lg-0',
        'bigConnection': 'col-lg-24 mb-2 mb-lg-0'
      }[layoutClass]
    const additionalInformationLayoutClass = isCompareMode
      ? 'col-lg-24'
      : {
        'default': 'col-lg-12',
        'bigConnection': 'col-lg-18'
      }[layoutClass]

    return (
      <div>
        <div className='form-row'>
          {this.showDuplicateSearch() &&
          <WarningMessage
            warningText={messages.warningTitle}
            {...this.props}
          />}
          <div className='row'>
            <div className={streetAddressLayoutClass}>
              <Street
                addressUseCase={addressUseCase}
                onStreetBlur={this.handleOnStreetBlur}
                required
                isFirst={index === 0}
                disabled={isSoteAddress}
              />
            </div>
            <div className={addressNumberLayoutClass}>
              <AddressNumber
                onRangeChanged={this.handleOnRangeChanged}
                disabled={isSoteAddress}
                required />
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={postalCodeOfficeLayoutClass}>
              <div className='row'>
                <div className={postalCodeLayoutClass}>
                  <PostalCode
                    onPostalCodeChanged={this.handleOnChange}
                    streetIdField='street'
                    required
                    disabled={isSoteAddress} />
                </div>
                <div className={postalOfficeLayoutClass}>
                  <PostOffice />
                </div>
              </div>
            </div>
            {
              addressUseCase === addressUseCasesEnum.VISITING &&
              postalCode &&
                <div className={municipalityLayoutClass}>
                  <div className='row'>
                    <div className={municipalityInnerLayoutClass}>
                      <Municipality />
                    </div>
                  </div>
                </div>
            }
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={additionalInformationLayoutClass}>
              <AdditionalInformation
                {...additionalInformationProps}
                maxLength={150}
                disabled={isSoteAddress}
                useQualityAgent
                collectionPrefix={this.collectionPrefix(addressUseCase)}
                compare={this.props.compare}
              />
            </div>
          </div>
        </div>
        {this.showDuplicateSearch() &&
          <SearchResultPreview withinGroup values={Map({ streetName, streetNumber, postalCode })} />
        }
        {!mapDisabled && <div className='form-row'>
          <Coordinates addressUseCase={addressUseCase} index={index} />
        </div>}
        {
          entityConcreteType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL &&
          addressUseCase === addressUseCasesEnum.VISITING &&          
          !isFetchingCoordinates && coordinatesNotFoundForAddress && (
            <MissingCoordinatesLabel />
          )
        } 
        {
          entityConcreteType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL &&
          addressUseCase === addressUseCasesEnum.VISITING &&
          index === 0 &&
          <Accessibility
            index={index}
            addressUseCase={addressUseCase}
            compare={!!this.props.compare}
            withinGroup
          />
        }
      </div>
    )
  }
}

StreetAddress.propTypes = {
  postalCode: PropTypes.string,
  streetNumber: PropTypes.string,
  language: PropTypes.string.isRequired,
  streetName: PropTypes.object,
  municipalityCode: PropTypes.string,
  mapDisabled: PropTypes.bool,
  municipalities: ImmutablePropTypes.map.isRequired,
  entityConcreteType: PropTypes.string,
  compare: PropTypes.bool,
  addressUseCase: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  additionalInformationProps: PropTypes.object.isRequired,
  selectedEntityConcreteType: PropTypes.string,
  selectedEntityId: PropTypes.string,
  index: PropTypes.number,
  layoutClass: PropTypes.string,
  searchResultExists: PropTypes.bool,
  coordinatesLoaded: ImmutablePropTypes.map,
  isSoteAddress: PropTypes.bool,
  coordinatesNotFoundForAddress: PropTypes.bool,
  isFetchingCoordinates: PropTypes.bool,
}

StreetAddress.defaultProps = {
  layoutClass: 'default'
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  withPath,
  connect((state, ownProps) => ({
    entityConcreteType: getSelectedEntityConcreteType(state),
    postalCode: getAddressPostalCode(state, ownProps),
    streetNumber: getAddressStreetNumber(state, ownProps),
    streetName: getStreetName(state, ownProps),
    streetId: getStreetId(state, ownProps),
    addressId: getAddressId(state, ownProps),
    language: getContentLanguageCode(state, ownProps) || 'fi',
    municipalities: EntitySelectors.municipalities.getEntities(state),
    municipalityCode: getMunicipalityCode(state, ownProps),
    municipalityId: getMunicipality(state, ownProps),
    selectedEntityConcreteType: getSelectedEntityConcreteType(state),
    selectedEntityId: getSelectedEntityId(state),
    searchResultExists: isAddressSearchResultExists(state, ownProps),
    coordinatesLoaded: getAddressMainCoordinate(state, ownProps),
    streetNumberRange: getAddressStreetNumberRange(state, ownProps),
    coordinatesNotFoundForAddress: getCoordinatesNotFoundForAddress(state, ownProps),
    isFetchingCoordinates: getIsFetchingCoordinates(state, ownProps),
  }))
)(StreetAddress)
