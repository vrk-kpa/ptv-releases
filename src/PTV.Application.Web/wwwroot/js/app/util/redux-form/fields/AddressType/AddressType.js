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
import React from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { Field, change } from 'redux-form/immutable'
import { List } from 'immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getIsAccessibilityRegisterAddresss,
  getInvalidAddress,
  getCoordinates,
  getPostalCode,
  getMunicipalityId,
  getStreetId,
  getStreetNumberRangeId
} from './selectors'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import { addressUseCases, addressUseCasesEnum, addressTypesEnum } from 'enums'
import CommonMessages from 'util/redux-form/messages'
import { getAreAllEnabled, getAreStreetOtherEnabled } from 'selectors/form'
import { checkAddressTitleValidity } from 'appComponents/AddressTitle/actions'

const AddressType = ({
  intl: { formatMessage },
  validate,
  addressUseCase,
  isAccessibilityRegsiterAddress,
  arrAllEnabled,
  disableTypes,
  areStreetOtherEnabled,
  isSoteAddress,
  change,
  path,
  formName,
  dispatch,
  invalidAddress,
  coordinates,
  postalCodeId,
  municipalityId,
  streetId,
  streetNumberRangeId,
  ...rest
}) => {
  let options = [
    {
      label: formatMessage(CommonMessages.streetAddressType),
      value: addressTypesEnum.STREET,
      disabledItem: (!disableTypes || addressUseCase !== addressUseCasesEnum.VISITING || arrAllEnabled)
        ? false : !areStreetOtherEnabled
    },
    {
      label: formatMessage(CommonMessages.otherAddressType),
      value: addressTypesEnum.OTHER,
      disabledItem: (!disableTypes || addressUseCase !== addressUseCasesEnum.VISITING || arrAllEnabled)
        ? false : !areStreetOtherEnabled
    },
    {
      label: formatMessage(CommonMessages.postofficeboxAddressType),
      value: addressTypesEnum.POSTOFFICEBOX
    },
    {
      label: formatMessage(CommonMessages.foreignAddressType),
      value: addressTypesEnum.FOREIGN,
      disabledItem: (!disableTypes || addressUseCase !== addressUseCasesEnum.VISITING || arrAllEnabled)
        ? false : areStreetOtherEnabled
    },
    {
      label: formatMessage(CommonMessages.noaddressAddressType),
      value: addressTypesEnum.NOADDRESS
    }
  ]

  switch (addressUseCase) {
    case addressUseCasesEnum.POSTAL :
      options = options.filter(option => option.value !== addressTypesEnum.NOADDRESS && option.value !== addressTypesEnum.OTHER)
      break
    case addressUseCasesEnum.VISITING :
      options = options.filter(option => option.value !== addressTypesEnum.POSTOFFICEBOX && option.value !== addressTypesEnum.NOADDRESS)
      break
    case addressUseCasesEnum.DELIVERY :
      options = options.filter(option => option.value !== addressTypesEnum.FOREIGN && option.value !== addressTypesEnum.OTHER)
      break
  }
  const handleOnChange = (_, newValue) => {
    if (newValue === addressTypesEnum.OTHER) {
      change(formName, `${path}.coordinates`, List())
    }

    checkAddressTitleValidity({
      invalidAddress,
      addressType: newValue,
      dispatch,
      formName,
      coordinates,
      postalCodeId,
      path,
      pCode: postalCodeId,
      mCode: municipalityId,
      streetId,
      streetNumberRange: streetNumberRangeId
    })
  }
  return (
    <Field
      name='streetType'
      component={RenderRadioButtonGroup}
      options={options}
      defaultValue={!areStreetOtherEnabled && addressTypesEnum.FOREIGN || addressTypesEnum.STREET}
      onChange={handleOnChange}
      {...rest}
      disabled={rest.disabled || isAccessibilityRegsiterAddress || isSoteAddress}
    />
  )
}

AddressType.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  addressUseCase: PropTypes.oneOf(addressUseCases),
  isAccessibilityRegsiterAddress: PropTypes.bool,
  areStreetOtherEnabled: PropTypes.bool,
  disableTypes: PropTypes.bool,
  arrAllEnabled: PropTypes.bool,
  isSoteAddress: PropTypes.bool,
  change: PropTypes.func.isRequired,
  path: PropTypes.string,
  formName: PropTypes.string,
  dispatch: PropTypes.func,
  invalidAddress: PropTypes.bool,
  coordinates: ImmutablePropTypes.list,
  postalCodeId: PropTypes.string,
  municipalityId: PropTypes.string,
  streetId: PropTypes.string,
  streetNumberRangeId: PropTypes.string
}

export default compose(
  injectIntl,
  withPath,
  injectFormName,
  connect((state, { formName, path }) => ({
    arrAllEnabled: getAreAllEnabled(state, { formName }),
    areStreetOtherEnabled: getAreStreetOtherEnabled(state, { formName }),
    isAccessibilityRegsiterAddress: getIsAccessibilityRegisterAddresss(formName, path)(state),
    path,
    formName,
    invalidAddress: getInvalidAddress(formName, path)(state),
    coordinates: getCoordinates(formName, path)(state),
    postalCodeId: getPostalCode(formName, path)(state),
    municipalityId: getMunicipalityId(formName, path)(state),
    streetId: getStreetId(formName, path)(state),
    streetNumberRangeId: getStreetNumberRangeId(formName, path)(state)
  }), { change }),
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable
)(AddressType)
