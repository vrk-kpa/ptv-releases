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
import { injectIntl, intlShape } from 'util/react-intl'
import Organization from 'util/redux-form/fields/Organization'
import {
  RenderAsyncMultiSelect,
  RenderMultiSelectDisplay
} from 'util/redux-form/renders'
import {
  getFormServiceProducersSelfProducers,
  getFormServiceProducersSelfProducedIndex,
  getSelectedOrganizationOnForm
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import ImmutablePropTypes from 'react-immutable-proptypes'
import CommonMessages from 'util/redux-form/messages'
import { normalize } from 'normalizr'
import { camelizeKeys } from 'humps'
import { EntitySchemas } from 'schemas'

const getDisplayRenderFromProps = props => RenderMultiSelectDisplay

const Organizations = ({
  intl: { formatMessage },
  validate,
  dispatch,
  selfProducers,
  selfProducersIndex,
  mainOrganization,
  formName,
  ...rest
}) => {
  const formatResponse = data =>
    data
      .filter(org => org.id !== mainOrganization)
      .map((org) => ({
        label: org.name,
        value: org.id,
        publishingStatus: org.publishingStatus,
        entity: org
      }))

  const handleOnChange = (_, organizations, previousOrganizations) => {
    if (previousOrganizations && organizations && previousOrganizations.size > organizations.size) {
      const removedOrganization = previousOrganizations.subtract(organizations).first()
      if (selfProducers.includes(removedOrganization)) {
        dispatch(change(formName, 'organizationProducerWarnning', true))
      }
    } else if (!organizations && selfProducers && selfProducers.size > 0) {
      dispatch(change(formName, 'organizationProducerWarnning', true))
    }
    if (!mainOrganization && selfProducers.intersect(organizations).size === 0) {
      dispatch(
        change(
          formName,
          `serviceProducers[${selfProducersIndex}].provisionType`,
          null
        )
      )
    }
  }

  const handleOnChangeObject = (object) => {
    const newSelected = object && object.filter(x => x.entity)
    newSelected && newSelected.length &&
      dispatch({
        type: 'ORGANIZATIONS',
        response: normalize(camelizeKeys(newSelected[0].entity), EntitySchemas.ORGANIZATION)
      })
  }

  return (<Organization
    name='organizations'
    label={formatMessage(CommonMessages.responsibleOrganizations)}
    component={RenderAsyncMultiSelect}
    onChangeObject={handleOnChangeObject}
    onChange={handleOnChange}
    formatResponse={formatResponse}
    skipValidation
    showAll
    getDisplayRenderFromProps={getDisplayRenderFromProps}
    {...rest}
  />)
}

Organizations.propTypes = {
  validate: PropTypes.func,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool,
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  selfProducers: ImmutablePropTypes.orderedSet.isRequired,
  selfProducersIndex: PropTypes.any,
  mainOrganization: PropTypes.any
}

export default compose(
  injectIntl,
  // asDisableable,
  // asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  injectFormName,
  connect((state, ownProps) => ({
    selfProducers: getFormServiceProducersSelfProducers(state, ownProps),
    selfProducersIndex: getFormServiceProducersSelfProducedIndex(state, ownProps),
    mainOrganization: getSelectedOrganizationOnForm(state, ownProps)
  })),
)(Organizations)
