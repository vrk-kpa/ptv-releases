/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { getServiceTypesJS } from './selectors'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { localizeList } from 'appComponents/Localize'
import {
  RenderSelect,
  RenderSelectDisplay,
  RenderRadioButtonGroup,
  RenderRadioButtonGroupDisplay
} from 'util/redux-form/renders'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step1.ServiceType.Title',
    defaultMessage: 'Palvelutyyppi'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step1.ServiceType.Tooltip',
    defaultMessage: 'Palvelutyyppi'
  }
})

const ServiceType = ({
  serviceTypes,
  intl: { formatMessage },
  validate,
  radio,
  nonField,
  isReadOnly,
  ...rest
}) => {
  const Component = radio
    ? isReadOnly
      ? RenderRadioButtonGroupDisplay
      : RenderRadioButtonGroup
    : RenderSelect
  return (
    (!nonField && (
      <Field
        name='serviceType'
        label={formatMessage(messages.title)}
        tooltip={formatMessage(messages.tooltip)}
        component={Component}
        options={serviceTypes}
        {...rest}
      />
    )) || (
      <Component
        label={formatMessage(messages.title)}
        tooltip={formatMessage(messages.tooltip)}
        options={serviceTypes}
        {...rest}
        disabled
      />
    )
  )
}
ServiceType.propTypes = {
  intl: intlShape.isRequired,
  radio: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  nonField: PropTypes.bool.isRequired,
  validate: PropTypes.func,
  serviceTypes: PropTypes.array
}

ServiceType.defaultProps = {
  nonField: false
}

export default compose(
  injectIntl,
  asDisableable,
  asComparable({
    getDisplayRenderFromProps: ({ radio }) => radio
      ? RenderRadioButtonGroupDisplay
      : RenderSelectDisplay
  }),
  connect(state => ({
    serviceTypes: getServiceTypesJS(state)
  })),
  localizeList({
    input: 'serviceTypes',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  injectSelectPlaceholder()
)(ServiceType)
