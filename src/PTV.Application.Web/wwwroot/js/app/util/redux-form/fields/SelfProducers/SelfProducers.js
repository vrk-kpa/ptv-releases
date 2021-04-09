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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { localizeList } from 'appComponents/Localize'
import {
  RenderMultiSelect,
  RenderMultiSelectDisplay
} from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import injectFormName from 'util/redux-form/HOC/injectFormName'

import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { getOrganizationsForFormJS,
  getFormSelfProducersOrganizationIntersect } from './selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { getOrganizationStatus } from '../Organization/selectors'

export const serviceOrganizationMessages = defineMessages({
  label: {
    id : 'Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Title',
    defaultMessage: 'Tuottaja'
  },
  placeholder: {
    id: 'Components.AutoCombobox.Placeholder',
    defaultMessage: '- valitse -'
  },
  deletedLabel: {
    id: 'FrontPage.PublishingStatus.Deleted.Title',
    defaultMessage: 'Arkistoitu'
  },
  draftLabel: {
    id: 'FrontPage.PublishingStatus.Draft.Title',
    defaultMessage: 'Luonnos'
  }
})

const OrganizationWithStatusLabel = compose(
  injectIntl,
  connect(
    (state, { organization: { publishingStatus } }) => ({
      code: getOrganizationStatus(state, { id: publishingStatus })
    })
  )
)(({ code, organization, intl: { formatMessage }, ...rest }) => {
  switch (code) {
    case 'Draft':
    case 'Modified':
      return <span>{organization.label} ({formatMessage(serviceOrganizationMessages.draftLabel)})</span>
    case 'Deleted':
      return <span>{organization.label} ({formatMessage(serviceOrganizationMessages.deletedLabel)})</span>
    default:
      return organization.label || null
  }
})

const valueRenderer = org => org && <OrganizationWithStatusLabel organization={org} /> || null

const SelfProducers = ({
  intl: { formatMessage },
  options,
  intersectValues,
  ...rest
}) => {
  const getCustomValue = () => intersectValues
  return (
    <Field
      name='selfProducers'
      label={formatMessage(serviceOrganizationMessages.label)}
      placeholder={formatMessage(serviceOrganizationMessages.placeholder)}
      component={RenderMultiSelect}
      options={options}
      getCustomValue={getCustomValue}
      valueRenderer={valueRenderer}
      {...rest}
    />
  )
}
SelfProducers.propTypes = {
  intersectValues: ImmutablePropTypes.orderedSet.isRequired,
  options: PropTypes.array.isRequired,
  intl: intlShape.isRequired
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  asDisableable,
  injectFormName,
  connect(
    (state, ownProps) => ({
      options: getOrganizationsForFormJS(state, ownProps),
      intersectValues: getFormSelfProducersOrganizationIntersect(state, ownProps)
    })
  ),
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  injectSelectPlaceholder()
)(SelfProducers)
