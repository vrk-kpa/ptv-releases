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
import {
  OrganizationType,
  MunicipalityCode,
  Organization
} from 'util/redux-form/fields'
import { AreaInformationExpanded } from 'util/redux-form/sections'
import { change } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asGroup from 'util/redux-form/HOC/asGroup'
import { getIsSoteOrganizationType } from 'util/redux-form/fields/OrganizationType/selectors'
import {
  getIsOrganizationTypeMunicipalitySelected,
  getIsOrganizationTypeRegionalSelected,
  getIsOrganizationTypeRegionSelected,
  getIsAreaInformationVisible,
  getIsAreaInformationWarningVisible,
  getRegionOrganizationType,
  getProvinceAreaType
} from './selectors'
import { formTypesEnum, userRoleTypesEnum } from 'enums'
import { getRole } from 'appComponents/Security/selectors'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import organizationMessages from '../messages'
import SoteNote from 'appComponents/SoteNote'

export const messages = defineMessages({
  groupLabel: {
    id: 'OrganizationWithAreaInformation.Group.Title',
    defaultMessage: 'Organisaatio- ja aluetiedot'
  },
  responsibleOrganizationLabel: {
    id: 'ResponsibleOrganization.Title',
    defaultMessage: 'Lähdejärjestelmästä tuleva vastuumaakunta'
  },
  responsibleOrganizationTooltip: {
    id: 'ResponsibleOrganization.Tooltip',
    defaultMessage: 'Responsible organization region tooltip'
  }
})

const OrganizationWithAreaInformation = ({
  isMunicipalitySelected,
  isAreaInformationVisible,
  disableWarning,
  isRegionalOrganizationSelected,
  isRegionSelected,
  regionOrganizationType,
  provinceAreaType,
  isCompareMode,
  dispatch,
  intl: { formatMessage },
  isSoteOrganization,
  userRole,
  isReadOnly
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  const indentCompareModeClass = isCompareMode ? 'col-lg-24 mb-4' : 'col-lg-12 mb-2'

  const handleOrganizationTypeClick = (id, value) => {
    if (regionOrganizationType === value) {
      dispatch(
        change(formTypesEnum.ORGANIZATIONFORM, 'areaInformation.areaType', provinceAreaType)
      )
    }
  }

  return (
    <div className='form-row-group'>
      <SoteNote showNote={isSoteOrganization} />
      <div className='row'>
        <div className={indentCompareModeClass}>
          <OrganizationType
            onChange={handleOrganizationTypeClick}
            disabled={isSoteOrganization}
            required />
        </div>
        <div className={basicCompareModeClass}>
          {isMunicipalitySelected && <MunicipalityCode required />}
        </div>
      </div>
      {isSoteOrganization && (
        <div className='row'>
          <div className={indentCompareModeClass}>
            <Organization
              label={formatMessage(messages.responsibleOrganizationLabel)}
              tooltip={formatMessage(messages.responsibleOrganizationTooltip)}
              name='responsibleOrganization'
              isReadOnly={isReadOnly || userRole !== userRoleTypesEnum.EEVA}
              required />
          </div>
        </div>
      )}
      {isAreaInformationVisible &&
        <AreaInformationExpanded
          hideType={isRegionalOrganizationSelected || isRegionSelected}
          singleSelect
          title={formatMessage(organizationMessages.areaInformationTitle)}
          tooltip={formatMessage(organizationMessages.areaInformationTooltip)}
          disableWarning={disableWarning}
        />
      }
    </div>
  )
}
OrganizationWithAreaInformation.propTypes = {
  isMunicipalitySelected: PropTypes.bool.isRequired,
  isAreaInformationVisible: PropTypes.bool.isRequired,
  isRegionalOrganizationSelected: PropTypes.bool.isRequired,
  isRegionSelected: PropTypes.bool.isRequired,
  regionOrganizationType: PropTypes.string,
  provinceAreaType: PropTypes.string,
  isCompareMode: PropTypes.bool,
  disableWarning: PropTypes.bool,
  dispatch: PropTypes.func.isRequired,
  intl: intlShape,
  isSoteOrganization: PropTypes.bool,
  userRole: PropTypes.string,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isMunicipalitySelected: getIsOrganizationTypeMunicipalitySelected(state, ownProps),
    isAreaInformationVisible: getIsAreaInformationVisible(state, ownProps),
    isRegionalOrganizationSelected: getIsOrganizationTypeRegionalSelected(state, ownProps),
    isRegionSelected: getIsOrganizationTypeRegionSelected(state, ownProps),
    regionOrganizationType: getRegionOrganizationType(state, ownProps),
    provinceAreaType: getProvinceAreaType(state, ownProps),
    disableWarning: !getIsAreaInformationWarningVisible(state, ownProps),
    isSoteOrganization: getIsSoteOrganizationType(state),
    userRole: getRole(state, ownProps)
  }), {
    change
  }),
  withFormStates,
  asGroup({ title: messages.groupLabel })
)(OrganizationWithAreaInformation)
