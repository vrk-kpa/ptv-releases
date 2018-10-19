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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { apiCall3 } from 'actions'
import { CodeListSchemas } from 'schemas'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Field, change } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLazyLoading from 'util/redux-form/HOC/withLazyLoading'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import { normalize } from 'normalizr'
import { getActiveFormField } from 'selectors/base'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import {
  getPostalCodeOptionsJS,
  getIsPostalCodeInStore,
  getPostalCodeById
} from './selectors'
import { localizeItem } from 'appComponents/Localize'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.PostalCode.Title',
    defaultMessage: 'Postinumero'
  }
})

const PostalCodeRender = compose(
  withLazyLoading({
    loadItemByIdAction: postalCodeId => (
      apiCall3({
        keys: ['enums', 'GetPostalCodeById'],
        payload: {
          endpoint: 'common/GetPostalCodeById',
          data: { Id: postalCodeId }
        },
        schemas: CodeListSchemas.POSTAL_CODE
      })
    ),
    getIsItemInStoreAction: getIsPostalCodeInStore
  })
)(RenderAsyncSelect)

const formatResponse = data =>
  data.map((pCode) => ({
    label: pCode.postOffice.toUpperCase(),
    code: pCode.code,
    value: pCode.id,
    municipalityId: pCode.municipalityId,
    entity: pCode
  }))

const PostalCodeDisplay = connect(
  (state, { id }) => ({
    code: getPostalCodeById(state, { id })
  })
)(({ code }) => <div>{code}</div>)

const OptionValue = compose(
  localizeItem({
    input: 'item',
    output: 'item',
    idAttribute: 'value',
    nameAttribute: 'label',
    getTranslationTexts: item => item.entity.translation.texts
  })
)(({ item }) => <span>{item.code} {item.label.toUpperCase()}</span>)

const formatDisplay = ({ code, value }) => code
  ? <div>{code}</div>
  : <PostalCodeDisplay id={value} />

const formatOption = (postalCode) => <OptionValue item={postalCode} />

class PostalCode extends Component {
  getFieldPath = fieldName => {
    const { activeFieldPath } = this.props
    const pathParts = activeFieldPath.split('.')
    pathParts.length > 1 && pathParts.splice(pathParts.length - 1, 1, fieldName)
    return pathParts.join('.')
  }
  handleOnChangeObject = object => {
    const {
      formName,
      dispatch,
      onChange
    } = this.props
    onChange && onChange(object && object.entity)
    object && dispatch({
      type: 'POSTAL_CODES',
      response: normalize(object.entity, CodeListSchemas.POSTAL_CODE)
    })
    dispatch(
      change(
        formName,
        this.getFieldPath('municipality'),
        object && object.entity.municipalityId
      )
    )
  }
  filterOption = () => true
  inputProps = { maxLength: 10 }
  render () {
    const {
      intl: { formatMessage },
      ...rest
    } = this.props
    return (
      <Field
        component={PostalCodeRender}
        name='postalCode'
        filterOption={this.filterOption}
        onChangeObject={this.handleOnChangeObject}
        label={formatMessage(messages.label)}
        url='common/GetPostalCodes'
        formatResponse={formatResponse}
        valueRenderer={formatDisplay}
        formatDisplay={formatDisplay}
        optionRenderer={formatOption}
        minLength={1}
        inputProps={this.inputProps}
        {...rest}
      />
    )
  }
}
PostalCode.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  dispatch: PropTypes.func,
  onChange: PropTypes.func,
  activeFieldPath: PropTypes.string.isRequired,
  formName: PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => ({
      activeFieldPath: getActiveFormField(formName)(state),
      options: getPostalCodeOptionsJS(state),
      triggerOnChange: true
    })
  ),
  asComparable({ DisplayRender: RenderAsyncSelectDisplay }),
  asDisableable,
  injectSelectPlaceholder(),
  withAccessibilityPrompt
)(PostalCode)
