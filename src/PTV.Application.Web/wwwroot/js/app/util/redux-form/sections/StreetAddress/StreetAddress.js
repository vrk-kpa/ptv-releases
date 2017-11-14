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
  AdditionalInformation,
  AddressNumber,
  Municipality,
  PostalCode,
  PostOffice,
  StreetAddressName
} from 'util/redux-form/fields'
import { withFormStates, injectFormName } from 'util/redux-form/HOC'
import { addressUseCasesEnum } from 'enums'
import { getAddressPostalCode,
  getAddressStreet,
  getAddressStreetNumber,
  getAddressId,
  getMunicipalityCode
} from './selectors'
import { getContentLanguageCode } from 'selectors/selections'
import { apiCall3 } from 'actions'
import { change } from 'redux-form/immutable'
import Coordinates from '../Coordinates'
import { fromJS, Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { injectIntl } from 'react-intl'

class StreetAddress extends PureComponent {
  static defaultProps = {
    name: 'streetAddress'
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
      mapDisabled,
      municipalities,
      municipalityCode,
      required,
      index
    } = this.props
    const streetAddressLayoutClass = isCompareMode ? 'col-lg-24  mb-4' : 'col-lg-8 mb-2 mb-lg-0'
    const addressNumberLayoutClass = isCompareMode ? 'col-lg-24' : 'col-lg-4'
    const postalCodeOfficeLayoutClass = isCompareMode ? 'col-lg-24' : 'col-lg-16 mb-2 mb-lg-0'
    const postalCodeLayoutClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
    const postalOfficeLayoutClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
    const municipalityLayoutClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-8 mb-2 mb-lg-0'
    const municipalityInnerLayoutClass = isCompareMode ? 'col-lg-24 mb-2' : 'col-lg-24 mb-2 mb-lg-0'
    const additionalInformationLayoutClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
    const coordinatesLayoutClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'

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
                  <PostalCode onChangeObject={handleOnChange} required={required} />
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
  path: PropTypes.string.isRequired,
  addressId: PropTypes.string,
  addressUseCase: PropTypes.string.isRequired,
  isCompareMode: PropTypes.bool.isRequired,
  additionalInformationProps: PropTypes.object.isRequired,
  required: PropTypes.bool
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
    language: getContentLanguageCode(state, ownProps),
    municipalities: EntitySelectors.municipalities.getEntities(state),
    municipalityCode: getMunicipalityCode(state, ownProps)
  }))
)(StreetAddress)
