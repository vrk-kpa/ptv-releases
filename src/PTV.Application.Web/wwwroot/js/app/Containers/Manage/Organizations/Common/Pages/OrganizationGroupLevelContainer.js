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
import React, { PropTypes, Component } from 'react'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import Immutable, { Map } from 'immutable'
import { connect } from 'react-redux'

// Components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
import { PTVRadioGroup } from '../../../../../Components'
import Organizations from '../../../../Common/organizations'
import { NotOwnOrgSecurityCreate, SecurityCreate } from 'appComponents/Security'

///Selectors
import * as CommonOrganizationSelectors from '../../Common/Selectors'

//Actions
import * as commonActions from '../Actions'
import * as organizationActions from '../../Organization/Actions'

const OrganizationGroupLevelContainer = props => {

  const validators = [PTVValidatorTypes.IS_REQUIRED]

  const { messages, readOnly, language, translationMode } = props
  const { organizationId, organizationUnificRootId, parentId, isSubOrganizationSelected } = props
  const { formatMessage } = props.intl

  var organizationGroupLevelItems = [
    {
      id: false,
      name: formatMessage(messages.organizationLevelMainOrganization)
    },
    {
      id: true,
      name: formatMessage(messages.organizationLevelSubOrganization)
    }]

  const onInputChange = (input, isSet = false) => value => {
    props.actions.onLocalizedOrganizationInputChange(input, organizationId, value, isSet, props.language)
  }

  const sharedProps = { readOnly, translationMode }
  return (
    <div className="row form-group">
      <NotOwnOrgSecurityCreate domain='organizationStructure'>
        <PTVRadioGroup
          radioGroupLegend={formatMessage(messages.organizationLevelTitle)}
          value={isSubOrganizationSelected}
          items={organizationGroupLevelItems}
          onChange={onInputChange('isSubOrganizationSelected', false)}
          className='col-md-6'
          name='manageOrganizationLevelGroup'
          readOnly={readOnly || translationMode == 'view' || translationMode == 'edit'}
        />
      </NotOwnOrgSecurityCreate >

      <SecurityCreate domain='organizationStructure' >
        {isSubOrganizationSelected ?
          <Organizations {...sharedProps}
            value={parentId}
            label={formatMessage(messages.organizationMainOrganizationTitle)}
            placeholder={formatMessage(messages.organizationMainOrganizationPlaceholder)}
            name='mainOrganization'
            changeCallback={onInputChange('parentId')}
            componentClass="col-md-6"
            inputProps={{ 'maxLength': '50' }}
            withoutOrganizationId={organizationUnificRootId}
            validators={validators} />
          : null}
      </SecurityCreate>

    </div>
  )
}

OrganizationGroupLevelContainer.propTypes = {
  readOnly: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  return {
    organizationId: CommonOrganizationSelectors.getOrganizationId(state, ownProps),
    organizationUnificRootId: CommonOrganizationSelectors.getUnificRootId(state, ownProps),
    isSubOrganizationSelected: CommonOrganizationSelectors.getIsSubOrganizationSelected(state, ownProps),
    parentId: CommonOrganizationSelectors.getOrganizationParentId(state, ownProps)
  }
}

const actions = [
  commonActions,
  organizationActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationGroupLevelContainer));
