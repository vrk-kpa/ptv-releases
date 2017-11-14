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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field, change } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { localizeList } from 'appComponents/Localize'
import {
  RenderMultiSelect,
  RenderMultiSelectDisplay
} from 'util/redux-form/renders'
import { getOrganizationsJS, getFormServiceProducersSelfProducers } from './selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { asComparable, asDisableable, injectFormName } from 'util/redux-form/HOC'
import ImmutablePropTypes from 'react-immutable-proptypes'
import CommonMessages from 'util/redux-form/messages'

export const serviceOrganizationMessages = defineMessages({
  placeholder: {
    id: 'Components.AutoCombobox.Placeholder',
    defaultMessage: '- valitse -'
  }
})

const Organizations = ({
  intl: { formatMessage },
  organizations,
  validate,
  dispatch,
  selfProducers,
  formName,
  ...rest
}) => {
  const handleOnChange = (_, organizations, previousOrganizations) => {
    if (previousOrganizations && organizations && previousOrganizations.size > organizations.size) {
      const removedOrganization = previousOrganizations.subtract(organizations).first()
      if (selfProducers.includes(removedOrganization)) {
        dispatch(change(formName, 'organizationProducerWarnning', true))
      }
    } else if (!organizations && selfProducers && selfProducers.size > 0) {
      dispatch(change(formName, 'organizationProducerWarnning', true))
    }
  }
  return (<Field
    name='organizations'
    label={formatMessage(CommonMessages.responsibleOrganizations)}
    placeholder={formatMessage(serviceOrganizationMessages.placeholder)}
    component={RenderMultiSelect}
    options={organizations}
    onChange={handleOnChange}
    //validate={translateValidation(validate, formatMessage, serviceOrganizationMessages.title)}
    {...rest}
  />)
}

Organizations.propTypes = {
  validate: PropTypes.func,
  organizations: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool,
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  selfProducers: ImmutablePropTypes.list.isRequired
}

export default compose(
  injectIntl,
  asDisableable,
  asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  injectFormName,
  connect(
    (state, ownProps) => ({
      organizations: getOrganizationsJS(state, ownProps),
      selfProducers: getFormServiceProducersSelfProducers(state, ownProps)
    })
  ),
  localizeList({
    input: 'organizations',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  }),
  injectSelectPlaceholder()
)(Organizations)
