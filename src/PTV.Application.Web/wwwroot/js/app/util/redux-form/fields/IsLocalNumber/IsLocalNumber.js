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
import { RenderCheckBox, RenderCheckBoxDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withPath from 'util/redux-form/HOC/withPath'
import { connect } from 'react-redux'
import { polyfillFieldValue } from 'actions/phones'

const messages = defineMessages({
  label: {
    id: 'ReduxForm.Renders.RenderIsLocalNumber.title',
    defaultMessage: 'Palvelunumero, johon ei voi soittaa ulkomailta.'
  }
})

const IsLocalNumber = ({
  intl: { formatMessage },
  validate,
  label,
  simple,
  onChange,
  ...rest
}) => {

  const handleChange = (evt, value) => {
    onChange(evt, value)
    if (!simple) {
      polyfillFieldValue()
    }
  }

  return (
    <Field
      name='isLocalNumber'
      component={RenderCheckBox}
      label={label || formatMessage(messages.label)}
      {...rest}
      onChange={handleChange}
    />
  )
}
IsLocalNumber.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  label: PropTypes.string,
  polyfillFieldValue: PropTypes.func,
  simple: PropTypes.bool,
  onChange: PropTypes.func
}
export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderCheckBoxDisplay }),
  asDisableable,
  withPath,
  connect((state, ownProps) => {
    if (ownProps.simple) {
      return {}
    }
    
    return {
      polyfillFieldValue: polyfillFieldValue({
        state,
        dispatch: ownProps.dispatch,
        formName: ownProps.formName,
        path: ownProps.path
      })
    }
  })
)(IsLocalNumber)
