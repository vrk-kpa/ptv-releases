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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { OrganizationGroupLevel, Organization } from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asGroup from 'util/redux-form/HOC/asGroup'
import { getOrganizationGroupLevel, canUserChangeMainOrg } from './selectors'
import { getIsSoteOrganizationType } from 'util/redux-form/fields/OrganizationType/selectors'
import { NotOwnOrgSecurityCreate } from 'appComponents/Security'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { getRole } from 'appComponents/Security/selectors'
import { userRoleTypesEnum } from 'enums'
import { Label } from 'sema-ui-components'
import SoteNote from 'appComponents/SoteNote'

export const messages = defineMessages({
  groupLabel: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Level.Title',
    defaultMessage: 'Organisaatiotaso *'
  },
  organizationMainOrganizationTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Select.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationUserCanCreateSubOrganizationTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step1.SubOrganization.infomration.Title',
    defaultMessage: 'Organisaation pääkäyttäjänä voit luoda organisaatiollesi alaorganisaatioita.'
  }
})

const OrganizationWithGroupLevel = ({
  groupLevel,
  isCompareMode,
  userRole,
  parentOrgChangable,
  intl: { formatMessage },
  isSoteOrganization,
  isReadOnly
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  const isVisibleSubOrganizationInfoTitle = userRole === userRoleTypesEnum.PETE || false
  return (
    <div className='form-row-group'>
      <SoteNote showNote={isSoteOrganization} />
      {isVisibleSubOrganizationInfoTitle
        ? <div className='row'>
          <div className='col-lg-24'>
            <Label
              labelText={formatMessage(messages.organizationUserCanCreateSubOrganizationTitle)}
              infoLabel
            />
          </div>
        </div>
        : null
      }
      <NotOwnOrgSecurityCreate domain='organizationStructure'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <OrganizationGroupLevel className='radioGroup' required disabled={isSoteOrganization} />
          </div>
        </div>
      </NotOwnOrgSecurityCreate >
      {groupLevel &&
        <div className='row'>
          <div className={basicCompareModeClass}>
            {parentOrgChangable && userRole !== userRoleTypesEnum.SHIRLEY
              ? <Organization
                label={formatMessage(messages.organizationMainOrganizationTitle)}
                required
                disabled={isSoteOrganization}
              />
              : <Organization
                label={formatMessage(messages.organizationMainOrganizationTitle)}
                isReadOnly
                disabled={isSoteOrganization}
              />}
          </div>
        </div>
      }
    </div>
  )
}
OrganizationWithGroupLevel.propTypes = {
  groupLevel: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  parentOrgChangable: PropTypes.bool,
  intl: intlShape,
  userRole: PropTypes.string,
  isSoteOrganization: PropTypes.bool,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return ({
      groupLevel: getOrganizationGroupLevel(state, ownProps),
      userRole: getRole(state, ownProps),
      parentOrgChangable: canUserChangeMainOrg(state, ownProps),
      isSoteOrganization: getIsSoteOrganizationType(state)
    })
  }),
  withFormStates,
  asGroup({
    title: messages.groupLabel,
    required: true
  })
)(OrganizationWithGroupLevel)
