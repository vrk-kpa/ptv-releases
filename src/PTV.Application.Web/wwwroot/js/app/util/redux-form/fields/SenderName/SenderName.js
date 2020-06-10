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
import { RenderTextField } from 'util/redux-form/renders'
import { isRequired } from 'util/redux-form/validators'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import withValidation from 'util/redux-form/HOC/withValidation'
import asDisableable from 'util/redux-form/HOC/asDisableable'

const messages = defineMessages({
  label: {
    id: 'Components.TranslationOrderDialog.TranslationOrderForm.Subscriber.Name.Title',
    defaultMessage: 'Tilaajan nimi'
  }
})

const SenderName = ({
  intl: { formatMessage },
  title,
  tooltip,
  placeholder,
  ...rest
}) => (
  <Field
    name='senderName'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={placeholder || ''}
    tooltip={tooltip || ''}
    maxLength={100}
    {...rest}
  />
)
SenderName.propTypes = {
  intl: intlShape,
  title: PropTypes.string,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string
}

export default compose(
  injectIntl,
  asDisableable,
  withValidation({
    label: messages.label,
    validate: isRequired
  }),
)(SenderName)
