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
import withPath from 'util/redux-form/HOC/withPath'
import {
  Accessibility,
  AdditionalInformation,
  AddressNumber,
  Municipality,
  PostalCode,
  PostOffice,
  StreetAddressName
} from 'util/redux-form/fields'

import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { addressUseCasesEnum, entityConcreteTypesEnum } from 'enums'
import { getAddressPostalCode,
  getAddressStreet,
  getAddressStreetNumber,
  getAddressId,
  getMunicipalityCode
} from './selectors'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getSelectedEntityConcreteType,
  getSelectedEntityId
} from 'selectors/entities/entities'
import { apiCall3 } from 'actions'
import { change } from 'redux-form/immutable'
import Coordinates from '../Coordinates'
import { fromJS, Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl } from 'util/react-intl'

class StreetAddress extends PureComponent {
  static defaultProps = {
    name: 'streetAddress'
  }
  get shouldShowAccessibility () {
    return this.props.selectedEntityConcreteType === 'serviceLocationChannel' &&
           this.props.selectedEntityId
  }
  render () {
    const {
      isCompareMode,
      addressUseCase,
      postalCode,
      street,
      addressId,
      dispatch,
      formName,
      path,
      language,
      streetNumber,
      additionalInformationProps,
      entityConcreteType,
      mapDisabled,
      municipalities,
      municipalityCode,
      required,
      index,
      layoutClass
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

    const getAddressSuccess = (data) => {
      dispatch(change(formName, path + '.coordinates', fromJS(data.response.result.coordinates)))
    }

    const handleOnBlur = (props) => {
      if (!mapDisabled && street && streetNumber && municipalityCode) {
        dispatch(
          apiCall3({
            keys: ['application', 'coordinates', addressId],
            payload: {
              endpoint: 'common/GetCoordinatesForAddress',
              data: { id: addressId, streetName: street, streetNumber, municipalityCode, language }
            },
            successNextAction: getAddressSuccess
          })
        )
      }
    }

    const handleOnChange = (newValue) => {
      const mCode = newValue && (municipalities.get(newValue.municipalityId) || Map()).get('code') || null
      if (!mapDisabled && street && streetNumber && mCode) {
        dispatch(
          apiCall3({
            keys: ['application', 'coordinates', addressId],
            payload: {
              endpoint: 'common/GetCoordinatesForAddress',
              data: { id: addressId, streetName: street, streetNumber, municipalityCode: mCode, language }
            },
            successNextAction: getAddressSuccess
          })
        )
      }
    }

    return (
      <div>
        <div className='form-row'>
          <div className='row'>
            <div className={streetAddressLayoutClass}>
              <StreetAddressName addressUseCase={addressUseCase} onBlur={handleOnBlur} required={required} />
            </div>
            <div className={addressNumberLayoutClass}>
              <AddressNumber onBlur={handleOnBlur} />
            </div>
          </div>
        </div>
        <div className='form-row'>
          <div className='row'>
            <div className={postalCodeOfficeLayoutClass}>
              <div className='row'>
                <div className={postalCodeLayoutClass}>
                  <PostalCode onChange={handleOnChange} required={required} />
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
              />
            </div>
          </div>
        </div>
        {!mapDisabled && <div className='form-row'>
          <Coordinates addressUseCase={addressUseCase} index={index} />
        </div>}
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
  dispatch: PropTypes.func.isRequired,
  street: PropTypes.string,
  municipalityCode: PropTypes.string,
  mapDisabled: PropTypes.bool,
  municipalities: ImmutablePropTypes.map.isRequired,
  formName: PropTypes.string.isRequired,
  entityConcreteType: PropTypes.string,
  compare: PropTypes.bool,
  path: PropTypes.string.isRequired,
  addressId: PropTypes.string,
  addressUseCase: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  additionalInformationProps: PropTypes.object.isRequired,
  required: PropTypes.bool,
  selectedEntityConcreteType: PropTypes.string,
  selectedEntityId: PropTypes.string,
  index: PropTypes.number,
  layoutClass: PropTypes.string
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
    street: getAddressStreet(state, ownProps),
    addressId: getAddressId(state, ownProps),
    language: getContentLanguageCode(state, ownProps) || 'fi',
    municipalities: EntitySelectors.municipalities.getEntities(state),
    municipalityCode: getMunicipalityCode(state, ownProps),
    selectedEntityConcreteType: getSelectedEntityConcreteType(state),
    selectedEntityId: getSelectedEntityId(state)
  }))
)(StreetAddress)
