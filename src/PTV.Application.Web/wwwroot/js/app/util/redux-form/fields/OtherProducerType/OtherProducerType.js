/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the 'Software'), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  RenderSelect,
  RenderRadioButtonGroup
} from 'util/redux-form/renders'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { otherProducerTypesEnum } from 'enums'

export const messages = defineMessages({
  label: {
    id : 'Util.ReduxForm.Fields.ServiceProducer.OtherOrganizationType.Label',
    defaultMessage: 'Tuottajatyyppi'
  },
  tooltip: {
    id : 'Util.ReduxForm.Fields.ServiceProducer.OtherOrganizationType.Tooltip',
    defaultMessage: 'Tuottajatyyppi'
  },
  organizationSelected: {
    id : 'Util.ReduxForm.Fields.ServiceProducer.OtherOrganizationRadio.Label',
    defaultMessage: 'PTV:ssä oleva tuottaja'
  },
  otherInfoSelected: {
    id : 'Util.ReduxForm.Fields.ServiceProducer.OtherInfoProvidedRadio.Label',
    defaultMessage: 'Muu tuottaja'
  }
})

const getOptions = (formatMessage) =>
  [{ value: otherProducerTypesEnum.ORGANIZATION, label: formatMessage(messages.organizationSelected) },
    { value: otherProducerTypesEnum.ADDITIONALINFORMATION, label: formatMessage(messages.otherInfoSelected) }]

const OtherProducerType = ({
  intl: { formatMessage },
  radio,
  ...rest
}) => {
  const Component = radio && RenderRadioButtonGroup || RenderSelect
  return (
    (
      <Field
        name='otherProducerType'
        label={formatMessage(messages.label)}
        tooltip={formatMessage(messages.tooltip)}
        required
        inline
        component={Component}
        options={getOptions(formatMessage)}
        defaultValue={otherProducerTypesEnum.ORGANIZATION}
        {...rest}
      />
    )
  )
}
OtherProducerType.propTypes = {
  intl: intlShape.isRequired,
  radio: PropTypes.bool
}

OtherProducerType.defaultProps = {
  radio: true
}

export default compose(
  injectIntl,
  asDisableable,
  injectSelectPlaceholder()
)(OtherProducerType)
