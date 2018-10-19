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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { OrganizationGroupLevel, Organization } from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getOrganizationGroupLevel, canUserChangeMainOrg } from './selectors'
import { NotOwnOrgSecurityCreate } from 'appComponents/Security'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { getRole } from 'appComponents/Security/selectors'
import { userRoleTypesEnum } from 'enums'
import { Label } from 'sema-ui-components'

export const messages = defineMessages({
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
  intl: { formatMessage }
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  const indentCompareModeClass = isCompareMode ? 'col-lg-24 mb-4 mb-lg-0' : 'col-lg-12'
  const isVisibleSubOrganizationInfoTitle = userRole === userRoleTypesEnum.PETE || false
  return (
    <div>
      { isVisibleSubOrganizationInfoTitle
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
      <div className='row'>
        <NotOwnOrgSecurityCreate domain='organizationStructure'>
          <div className={indentCompareModeClass}>
            <OrganizationGroupLevel inline className='radioGroup' required />
          </div>
        </NotOwnOrgSecurityCreate >
        { groupLevel &&
          <div className={basicCompareModeClass}>
            { parentOrgChangable && userRole !== userRoleTypesEnum.SHIRLEY
              ? <Organization
                label={formatMessage(messages.organizationMainOrganizationTitle)}
                required
                />
              : <Organization
                label={formatMessage(messages.organizationMainOrganizationTitle)}
                isReadOnly
              />}
          </div>
        }
      </div>
    </div>
  )
}
OrganizationWithGroupLevel.propTypes = {
  groupLevel: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  parentOrgChangable: PropTypes.bool,
  intl: intlShape,
  userRole: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    return ({
      groupLevel: getOrganizationGroupLevel(state, ownProps),
      userRole: getRole(state, ownProps),
      parentOrgChangable: canUserChangeMainOrg(state, ownProps)
    })
  }),
  withFormStates
)(OrganizationWithGroupLevel)
