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
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.InfoTitle',
    defaultMessage: 'Lisätieto'
  },
  placeholder: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.InfoPlaceholder',
    defaultMessage: 'esim. Asiakaspalvelu'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.InfoTooltip',
    defaultMessage: 'esim. Asiakaspalvelu'
  }
})

const getText = (text, formatMessage) =>
  typeof text === 'object'
    ? formatMessage(text)
    : text

const AdditionalInformation = ({
  intl: { formatMessage },
  title,
  placeholder,
  tooltip,
  validate,
  ...rest
}) => (
  <Field
    name='additionalInformation'
    component={RenderTextField}
    multiline
    rows={2}
    label={getText(title, formatMessage) || formatMessage(messages.label)}
    placeholder={getText(placeholder, formatMessage) || formatMessage(messages.placeholder)}
    tooltip={tooltip}
    maxLength={100}
    counter
    {...rest}
  />
)
AdditionalInformation.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  title: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  placeholder: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  tooltip: PropTypes.string
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asLocalizable,
  asDisableable
)(AdditionalInformation)
