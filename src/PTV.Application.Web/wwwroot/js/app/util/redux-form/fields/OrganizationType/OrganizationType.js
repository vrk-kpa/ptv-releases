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
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { getOrganizationTypesObjectArray } from './selectors'
import { translateValidation } from 'util/redux-form/validators'
import { localizeList } from 'appComponents/Localize'
import { asDisableable, asComparable } from 'util/redux-form/HOC'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Type.Title',
    defaultMessage: 'Organisaatiotyyppi *'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.Type.Tooltip',
    defaultMessage: 'OrganizationType tooltip'
  }
})

const OrganizationType = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => {
  const renderTypeOption = option => {
    if (!option.disabled) {
      return (
        <div className='leaf'>
          <span>{option.label}</span>
        </div>
      )
    } else {
      return (
        <div className='root'>
          <span>{option.label}</span>
        </div>
      )
    }
  }
  return (
    <Field
      name='organizationType'
      component={RenderSelect}
      label={formatMessage(messages.label)}
      // validate={translateValidation(validate, formatMessage, messages.label)}
      optionRenderer={renderTypeOption}
      {...rest}
    />
  )
}
OrganizationType.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  asDisableable,
  connect(
    state => ({
      options: getOrganizationTypesObjectArray(state)
        .map(({
          name,
          id: value,
          code: label,
          isDisabled: disabled
        }) => ({
          name,
          value,
          label,
          disabled
        }))
    })
  ),
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  injectSelectPlaceholder()
)(OrganizationType)
