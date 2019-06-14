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
import { getGeneralDescriptionTypesJS,
  getGeneralDescriptionTypesAllJS,
  getGeneralDescriptionTypesJSReadOnlyValues } from './selectors'
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
    id: 'Util.ReduxForm.Fields.GeneralDescriptionType.Title',
    defaultMessage: 'Käyttöaluetyyppi'
  },
  tooltip: {
    id: 'Util.ReduxForm.Fields.GeneralDescriptionType.Tooltip',
    defaultMessage: 'Käyttöaluetyyppi'
  }
})

const GeneralDescriptionType = ({
  generalDescriptionTypes,
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
        name='generalDescriptionType'
        label={formatMessage(messages.title)}
        tooltip={formatMessage(messages.tooltip)}
        component={Component}
        options={generalDescriptionTypes}
        {...rest}
      />
    )) || (
      <Component
        label={formatMessage(messages.title)}
        tooltip={formatMessage(messages.tooltip)}
        options={generalDescriptionTypes}
        {...rest}
        disabled
      />
    )
  )
}
GeneralDescriptionType.propTypes = {
  intl: intlShape.isRequired,
  radio: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  nonField: PropTypes.bool.isRequired,
  validate: PropTypes.func,
  generalDescriptionTypes: PropTypes.array
}

GeneralDescriptionType.defaultProps = {
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
  connect((state, { isReadOnly, readOnlyValues }) => ({
    generalDescriptionTypes: isReadOnly && getGeneralDescriptionTypesAllJS(state) ||
      readOnlyValues && getGeneralDescriptionTypesJSReadOnlyValues(state) ||
      getGeneralDescriptionTypesJS(state)
  })),
  localizeList({
    input: 'generalDescriptionTypes',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  injectSelectPlaceholder()
)(GeneralDescriptionType)
