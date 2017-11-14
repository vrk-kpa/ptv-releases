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
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import { Field, change } from 'redux-form/immutable'
import { asDisableable, asComparable, injectFormName } from 'util/redux-form/HOC'
import { normalize } from 'normalizr'
import { CodeListSchemas } from 'schemas'
import { getActiveFormField } from 'selectors/base'
import { getPostalCodeOptionsJS, getPostalCodeDefaultOptionsJS } from './selectors'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.PostalCode.Title',
    defaultMessage: 'Postinumero'
  }
})

const PostalCodeRender = compose(
  connect(
    (state, { input: { value } }) => ({
      defaultOptions: value && getPostalCodeDefaultOptionsJS(state, { id: value })
    })
  )
)(RenderAsyncSelect)

const formatResponse = data =>
  data.map((pCode) => ({
    label: `${pCode.code} ${pCode.postOffice.toUpperCase()}`,
    code: pCode.code,
    value: pCode.id,
    municipalityId: pCode.municipalityId,
    entity: pCode
  }))

const formatDisplay = ({ code }) => `${code}`

const PostalCode = ({
  intl: { formatMessage },
  validate,
  dispatch,
  formName,
  activeFieldPath,
  onChangeObject,
  ...rest
}) => {
  const getFieldPath = (fieldName) => {
    const pathParts = activeFieldPath.split('.')
    pathParts.length > 1 && pathParts.splice(pathParts.length - 1, 1, fieldName)
    return pathParts.join('.')
  }

  const handleOnChangeObject = (object) => {
    onChangeObject && onChangeObject(object && object.entity)
    object && dispatch({ type: 'POSTAL_CODES', response: normalize(object.entity, CodeListSchemas.POSTAL_CODE) })
    dispatch(change(formName, getFieldPath('municipality'), object && object.entity.municipalityId))
  }

  return (
    <Field
      component={PostalCodeRender}
      name='postalCode'
      filterOption={() => true}
      onChangeObject={handleOnChangeObject}
      label={formatMessage(messages.label)}
      url='common/GetPostalCodes'
      formatResponse={formatResponse}
      valueRenderer={formatDisplay}
      formatDisplay={formatDisplay}
      minLength={1}
      inputProps={{ maxLength: 10 }}
      {...rest}
    />
  )
}
PostalCode.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  dispatch: PropTypes.func,
  onChangeObject: PropTypes.func,
  onChange: PropTypes.func,
  activeFieldPath: PropTypes.string.isRequired,
  formName: PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    activeFieldPath: getActiveFormField(formName)(state),
    options: getPostalCodeOptionsJS(state)
  })),
  asComparable({ DisplayRender: RenderAsyncSelectDisplay }),
  asDisableable,
  injectSelectPlaceholder()
)(PostalCode)
