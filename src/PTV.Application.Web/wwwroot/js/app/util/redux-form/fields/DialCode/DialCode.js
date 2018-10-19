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
import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { apiCall3 } from 'actions'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { localizeItem } from 'appComponents/Localize'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withLazyLoading from 'util/redux-form/HOC/withLazyLoading'
import {
  getDefaultDialCode,
  getDialCodeById,
  getIsDialCodeInStore,
  getDialCodeOptionsJS,
  getPhoneNumber
} from './selectors'
import { CodeListSchemas } from 'schemas'
import { normalize } from 'normalizr'

const messages = defineMessages({
  label:{
    id: 'Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Title',
    defaultMessage: 'Maan suuntanumero'
  },
  placeholder:{
    id: 'Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Placeholder',
    defaultMessage: 'esim. +358'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhonePrefix.Tooltip',
    defaultMessage: 'esim. +358'
  }
})
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const formatResponse = data =>
  data.map((dCode) => ({
    label: dCode.countryName,
    value: dCode.id,
    countryName: dCode.countryName,
    code: dCode.code,
    entity: dCode
  }))

const OptionValue = compose(
  localizeItem({
    input: 'item',
    output: 'item',
    idAttribute: 'value',
    nameAttribute: 'label',
    getTranslationTexts: item => item.entity && item.entity.translation.texts || null
  })
)(({ item }) => <span>({item.code}) {item.label}</span>)

const DialCodeDisplay = connect(
  (state, { id }) => ({
    item: getDialCodeById(state, { id })
  })
)(OptionValue)

const formatDisplay = (dialCode) => dialCode.code && dialCode.countryName
  ? <OptionValue item={dialCode} />
  : <DialCodeDisplay id={dialCode.value} />

const DialCodeRender = compose(
  connect(
    (state, { input, meta, disabled }) => {
      const newInput = input.value && !disabled
        ? input
        : { ...input, value: getDefaultDialCode(state) }
      return {
        input: newInput,
        clearable: !!input.value
      }
    }
  ),
  withLazyLoading({
    loadItemByIdAction: dialCodeId => (
      apiCall3({
        keys: ['enums', 'GetDialCodeById'],
        payload: {
          endpoint: 'common/GetDialCodeById',
          data: { id: dialCodeId }
        },
        schemas: CodeListSchemas.DIAL_CODE
      })
    ),
    getIsItemInStoreAction: getIsDialCodeInStore
  })
)(RenderAsyncSelect)

const DialCodeDisplayRender = compose(
  withPath,
  injectFormName,
  connect(
    (state, { input, meta, disabled, formName, path }) => {
      const pNumber = getPhoneNumber(formName, path)(state)
      const newInput = pNumber ? (input.value && !disabled
        ? input
        : { ...input, value: getDefaultDialCode(state) }) : { ...input, value: null }
      return {
        input: newInput,
        clearable: !!input.value
      }
    }
  )
)(RenderAsyncSelectDisplay)

const filterOption = () => true

const DialCode = ({
  intl: { formatMessage },
  validate,
  dispatch,
  ...rest
}) => {
  const onChangeObject = object => object && dispatch({
    type: 'DIAL_CODES',
    response: normalize(object.entity, CodeListSchemas.DIAL_CODE)
  })
  return (
    <Field
      name='dialCode'
      component={DialCodeRender}
      formatResponse={formatResponse}
      formatDisplay={formatDisplay}
      searchable
      filterOption={filterOption}
      label={formatMessage(messages.label)}
      tooltip={formatMessage(messages.tooltip)}
      placeholder={formatMessage(messages.placeholder)}
      optionRenderer={formatDisplay}
      valueRenderer={formatDisplay}
      onChangeObject={onChangeObject}
      inputProps={{ 'maxLength':'8' }}
      url='common/GetDialCodes'
      {...rest}
    />
  )
}
DialCode.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  dispatch: PropTypes.func
}

export default compose(
  injectIntl,
  connect(
    state => ({
      options: getDialCodeOptionsJS(state)
    })
  ),
  asComparable({ DisplayRender: DialCodeDisplayRender }),
  asDisableable
)(DialCode)

