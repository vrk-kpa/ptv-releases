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
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withCopyToClipboard from 'util/redux-form/HOC/withCopyToClipboard'
import { localizeProps } from 'appComponents/Localize'
import { Map } from 'immutable'
import {
  getEntity,
  getSelectedEntityId,
  getEntityUnificRoot
} from 'selectors/entities/entities'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import styles from './styles.scss'
import { EntitySelectors } from 'selectors'
import { idMessages } from './messages'
import { Oid } from 'util/redux-form/fields'
import { formTypesEnum } from 'enums'
import { getIsSoteOid } from './selectors'

export const messages = defineMessages({
  entityIdTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.EntityIdTitle',
    defaultMessage: 'Tunniste'
  },
  entityUnificRootIdTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.EntityUnificRootIdTitle',
    defaultMessage: 'Versiotunniste'
  },
  notAvailable: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.NotAvailableTitle',
    defaultMessage: 'Not available'
  },
  oidLabelORG: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.OrganizationId.Title',
    defaultMessage: 'Organisaatiotunniste'
  },
  oidTooltipORG: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.OrganizationId.Tooltip',
    defaultMessage: 'Jos organisaatiollasi tai toimialallasi on käytössä organisaatio-OID-luokitus (tällainen on olemassa esimerkiksi sote-sektorilla), kirjoita tähän tämän luokituksen mukainen organisaatiotunniste eli OID.' // eslint-disable-line
  },
  oidPlaceholderORG: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.OrganizationId.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  oidLabelSL: {
    id: 'Util.ReduxForm.Fields.Oid.Title',
    defaultMessage: 'SOTE-palvelupaikkatunniste'
  },
  oidPlaceholderSL: {
    id: 'Util.ReduxForm.Fields.Oid.Placeholder',
    defaultMessage: 'Kirjoita tai liitä tunniste'
  },
  ptvIdFeedback: {
    id: 'EntityAdditionalInformation.PtvIdFeedback.Title',
    defaultMessage: 'PTV-tunniste kopioitu leikepöydälle.'
  },
  versionIdFeedback: {
    id: 'EntityAdditionalInformation.VersionIdFeedback.Title',
    defaultMessage: 'Versiotunniste kopioitu leikepöydälle.'
  }
})

const VersionLabelRowData = compose(
  withCopyToClipboard
)(({
  value
}) => {
  return <span>{value}</span>
})

const VersionLabelRow = ({
  value,
  label,
  emptyLabel = '',
  feedbackMessage
}) => {
  return <div className='row'>
    <div className='col-md-12 col-lg-6'>
      <em>{label}</em>
    </div>
    <div className='col-md-12'>
      {value && <VersionLabelRowData value={value} feedbackMessage={feedbackMessage} /> || emptyLabel}
    </div>
  </div>
}
VersionLabelRow.propTypes = {
  value: PropTypes.string,
  label: PropTypes.string,
  emptyLabel: PropTypes.any,
  feedbackMessage: PropTypes.object
}

const VersionLabel = ({
  intl: { formatMessage },
  entityID,
  unificRootId,
  version,
  pStatus,
  formName,
  isServiceLocation,
  isOrganization,
  isSoteOid,
  isReadOnly
}) => {
  const oidClass = cx(
    styles.oid,
    {
      [styles.isReadOnly]: isReadOnly
    }
  )
  const messageSuffix = isServiceLocation && 'SL' || 'ORG'
  const oidLabel = formatMessage(messages[`oidLabel${messageSuffix}`])
  const oidPlaceholder = formatMessage(messages[`oidPlaceholder${messageSuffix}`])
  const oidTooltip = isOrganization && formatMessage(messages[`oidTooltip${messageSuffix}`])
  const showOid = isServiceLocation || isOrganization
  const isOidReadonly = isSoteOid || isReadOnly
  return (
    <div className={styles.versionLabel}>
      <h4>
        <Label labelText={formatMessage(idMessages[formName].headline.title)} />
        <Tooltip tooltip={formatMessage(idMessages[formName].headline.tooltip)} indent='i0' />
      </h4>
      <VersionLabelRow
        label={formatMessage(messages.entityUnificRootIdTitle)}
        value={unificRootId}
        emptyLabel={formatMessage(messages.notAvailable)}
        feedbackMessage={messages.ptvIdFeedback}
      />
      <VersionLabelRow
        label={formatMessage(messages.entityIdTitle)}
        value={entityID}
        emptyLabel={formatMessage(messages.notAvailable)}
        feedbackMessage={messages.versionIdFeedback}
      />
      <div className={oidClass}>
        {showOid && <div className={isOidReadonly ? styles.read : undefined}>
          <Oid
            fieldClass='row'
            labelClass='col-md-12 col-lg-6'
            inputClass='col-md-8 col-lg-6'
            useBSClasses
            isReadOnly={isOidReadonly}
            label={oidLabel}
            placeholder={oidPlaceholder}
            tooltip={oidTooltip}
          />
        </div>}
      </div>
      <span>{pStatus[0].toUpperCase() + ' ' + version}</span>
    </div>
  )
}

VersionLabel.propTypes = {
  version: PropTypes.string.isRequired,
  entityID: PropTypes.string.isRequired,
  unificRootId: PropTypes.string.isRequired,
  pStatus: PropTypes.string.isRequired,
  formName: PropTypes.string,
  intl: intlShape,
  isServiceLocation: PropTypes.bool,
  isOrganization: PropTypes.bool,
  isSoteOid: PropTypes.bool,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, { formName }) => {
    const entity = getEntity(state) || Map()
    const ps = entity.get('publishingStatus')
    const isServiceLocation = formName === formTypesEnum.SERVICELOCATIONFORM
    const isOrganization = formName === formTypesEnum.ORGANIZATIONFORM
    const code = ps && (EntitySelectors.publishingStatuses.getEntities(state) || Map())
      .get(ps).get('code')
    const pStatus = (code && (code === 'Deleted' ? 'Archived' : code) || 'None')[0].toUpperCase()
    return {
      pStatus,
      version: (entity.get('version') || Map()).get('value') || '0.0',
      entityID: getSelectedEntityId(state),
      unificRootId: getEntityUnificRoot(state),
      isServiceLocation,
      isOrganization,
      formName,
      isSoteOid: isOrganization && getIsSoteOid(state, { formName })
    }
  }),
  localizeProps({
    nameAttribute: 'serviceType',
    idAttribute: 'type'
  })
)(VersionLabel)
