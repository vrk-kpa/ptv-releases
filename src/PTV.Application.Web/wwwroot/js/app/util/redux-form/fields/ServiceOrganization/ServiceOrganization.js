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
import { change } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import {
  getFormOrganizations,
  getFormServiceProducersSelfProducers,
  getFormServiceProducersSelfProducedIndex } from './selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import Organization from '../Organization'
import { loadDefaultAreaInformationForOrganization } from 'actions'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

export const messages = defineMessages({
  organizationTranslationWarning: {
    id: 'Util.ReduxForm.Fields.ServiceOrganization.MissingTranslationWarning',
    defaultMessage: 'Organisation language version missing! Organisation must be described in the same language as the service.'
  }
})

const ServiceOrganization = ({
  organizations,
  dispatch,
  selfProducers,
  selfProducerIndex,
  formName,
  intl: { formatMessage },
  ...rest
}) => {
  const handleOnChange = (_, organizationId, previousOrganizationId) => {
    if (organizations.includes(organizationId)) {
      dispatch(
        change(
          formName,
          'responsibleOrganizations',
          organizations.filter(o => o !== organizationId)
        )
      )
    }
    if (selfProducers.includes(previousOrganizationId)) {
      dispatch(
        change(
          formName,
          'organizationProducerWarnning',
          true
        )
      )
    }
    if ((selfProducers.includes(previousOrganizationId) && selfProducers.size === 1) ||
        selfProducers.size === 0) {
      dispatch(
        change(
          formName,
          `serviceProducers[${selfProducerIndex}].provisionType`,
          null
        )
      )
    }
    dispatch(loadDefaultAreaInformationForOrganization(organizationId, formName, true))
  }
  return (
    <Organization
      onChange={handleOnChange}
      missingTranslationWarning={messages.organizationTranslationWarning}
      {...rest}
    />
  )
}

ServiceOrganization.propTypes = {
  validate: PropTypes.func,
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  organizations: ImmutablePropTypes.orderedSet.isRequired,
  selfProducers: ImmutablePropTypes.orderedSet.isRequired,
  intl: intlShape,
  selfProducerIndex: PropTypes.any
}

export default compose(
  injectIntl,
  injectFormName,
  asDisableable,
  connect(
    (state, ownProps) => ({
      organizations: getFormOrganizations(state, ownProps),
      selfProducers: getFormServiceProducersSelfProducers(state, ownProps),
      selfProducerIndex: getFormServiceProducersSelfProducedIndex(state, ownProps)
    })
  )
)(ServiceOrganization)
