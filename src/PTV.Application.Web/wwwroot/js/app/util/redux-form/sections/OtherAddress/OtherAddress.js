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
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  Accessibility,
  AdditionalInformation,
  PostalCode,
  CoordinatesInput
} from 'util/redux-form/fields'

import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getAddressPostalCode,
  getAddressStreet,
  getAddressStreetNumber,
  getAddressId,
  getMunicipalityCode,
  getHasAccessibilityId,
  getCoordinates
} from './selectors'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getSelectedEntityConcreteType,
  getSelectedEntityId
} from 'selectors/entities/entities'
import { EntitySelectors } from 'selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { addressUseCasesEnum, entityConcreteTypesEnum } from 'enums'
import { checkAddressTitleValidity } from 'appComponents/AddressTitle/actions'

const messages = defineMessages({
  additionalInformationPlaceholder: {
    id: 'Util.ReduxForm.Sections.OtherAddress.AdditionalInformation.Placeholder',
    defaultMessage: 'Type here the text to specify the location'
  }
})
class OtherAddress extends PureComponent {
  static otherProps = {
    name: 'streetAddress'
  }
  get shouldShowAccessibility () {
    return this.props.selectedEntityConcreteType === 'serviceLocationChannel' &&
           this.props.selectedEntityId
  }
  isAdditionalInformationRequired = () => {
    return (
      this.props.selectedEntityConcreteType !== entityConcreteTypesEnum.SERVICELOCATIONCHANNEL ||
      (this.props.addressUseCase === addressUseCasesEnum.VISITING && !this.props.hasAccessibilityId)
    )
  }

  handleCoordinatesChange = (newCoordinates) => {
    checkAddressTitleValidity({
      ...this.props,
      coordinates: newCoordinates,
      postalCodeId: this.props.postalCode
    })
  }

  handlePostalCodeChange = (newPostalCode) => {
    const jsCoordinates = this.props && this.props.coordinates && this.props.coordinates.toJS()
    checkAddressTitleValidity({
      ...this.props,
      postalCodeId: newPostalCode && newPostalCode.value,
      coordinates: jsCoordinates
    })
  }

  render () {
    const {
      isCompareMode,
      additionalInformationProps,
      required,
      addressUseCase,
      hasAccessibilityId,
      index,
      intl: { formatMessage }
    } = this.props
    const streetAddressLayoutClass = isCompareMode ? 'col-lg-24  mb-4' : 'col-lg-8 mb-2 mb-lg-0'
    const postalCodeLayoutClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
    const additionalInformationLayoutClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'

    return (
      <div>
        <div className='form-row'>
          <div className='row'>
            <div className={streetAddressLayoutClass}>
              <CoordinatesInput
                required={required}
                onCoordinatesChange={this.handleCoordinatesChange} />
            </div>
            <div className={postalCodeLayoutClass}>
              <PostalCode
                onPostalCodeChanged={this.handlePostalCodeChange} />
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={additionalInformationLayoutClass}>
              <AdditionalInformation
                {...additionalInformationProps}
                placeholder={formatMessage(messages.additionalInformationPlaceholder)}
                maxLength={150}
                required={
                  this.isAdditionalInformationRequired()
                    ? required
                    : undefined
                }
              />
            </div>
          </div>
        </div>
        {
          this.props.selectedEntityConcreteType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL &&
          addressUseCase === addressUseCasesEnum.VISITING &&
          hasAccessibilityId &&
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

OtherAddress.propTypes = {
  postalCode: PropTypes.string,
  streetNumber: PropTypes.string,
  language: PropTypes.string.isRequired,
  dispatch: PropTypes.func.isRequired,
  street: PropTypes.string,
  municipalityCode: PropTypes.string,
  mapDisabled: PropTypes.bool,
  municipalities: ImmutablePropTypes.map.isRequired,
  formName: PropTypes.string.isRequired,
  path: PropTypes.string.isRequired,
  addressId: PropTypes.string,
  addressUseCase: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  additionalInformationProps: PropTypes.object.isRequired,
  required: PropTypes.bool,
  selectedEntityConcreteType: PropTypes.string,
  selectedEntityId: PropTypes.string,
  index: PropTypes.number,
  intl: intlShape,
  hasAccessibilityId: PropTypes.bool,
  compare: PropTypes.bool,
  coordinates: PropTypes.object
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    postalCode: getAddressPostalCode(state, ownProps),
    streetNumber: getAddressStreetNumber(state, ownProps),
    street: getAddressStreet(state, ownProps),
    addressId: getAddressId(state, ownProps),
    language: getContentLanguageCode(state, ownProps) || 'fi',
    municipalities: EntitySelectors.municipalities.getEntities(state),
    municipalityCode: getMunicipalityCode(state, ownProps),
    selectedEntityConcreteType: getSelectedEntityConcreteType(state),
    selectedEntityId: getSelectedEntityId(state),
    hasAccessibilityId: getHasAccessibilityId(state, ownProps),
    coordinates: getCoordinates(state, ownProps)
  }))
)(OtherAddress)
