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
import { RenderTextField } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import isRequired from 'util/redux-form/validators/isRequired'
import isEmail from 'util/redux-form/validators/isEmail'
import withValidation from 'util/redux-form/HOC/withValidation'

const messages = defineMessages({
  label: {
    id: 'Components.TranslationOrderDialog.TranslationOrderForm.Subscriber.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  placeholder: {
    id: 'Components.TranslationOrderDialog.TranslationOrderForm.Subscriber.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})

const SenderEmail = ({
  intl: { formatMessage },
  emailProps = {},
  ...rest
}) => (
  <Field
    name='senderEmail'
    component={RenderTextField}
    placeholder={formatMessage(messages.placeholder)}
    label={formatMessage(messages.label)}
    maxLength={100}
    {...emailProps}
    {...rest}
    />
)
SenderEmail.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  emailProps: PropTypes.object
}

export default compose(
  injectIntl,
  withValidation({
    label: messages.label,
    validate: [isEmail, isRequired]
  })
)(SenderEmail)
