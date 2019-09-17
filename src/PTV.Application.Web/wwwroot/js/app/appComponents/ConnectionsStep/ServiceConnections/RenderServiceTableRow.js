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
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import NameCell from 'appComponents/Cells/NameCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import TableRowDetailButton from 'appComponents/TableRowDetailButton'
import cx from 'classnames'
import styles from '../styles.scss'
import { PTVIcon } from 'Components'
import ConnectionTags from 'appComponents/ConnectionsStep/ConnectionTags'
import { injectIntl, intlShape } from 'util/react-intl'
import { connectionMessages } from 'appComponents/ConnectionsStep/messages'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import {
  entityConcreteTypesEnum,
  entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes } from 'enums'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { getFormSyncErrorWithPath } from 'selectors/base'
import { Security } from 'appComponents/Security'
import { fieldArrayPropTypes, fieldPropTypes, getFormSyncErrors } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'

const RenderServiceTableRow = ({
  fields,
  index,
  input,
  onClick,
  onRemove,
  onOpen,
  isOpen,
  isAsti,
  isRemovable,
  isReadOnly,
  field,
  entityType,
  hasChildError,
  hasformError,
  intl: { formatMessage }
}) => {
  const tableCellInlineClass = cx(
    styles.tableCell,
    styles.inline
  )
  const detailButtonClass = cx(
    styles.tableCell,
    styles.inline,
    styles.withDetailButton
  )
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  const entityId = input.value.get('id')
  const organizationId = input.value.get('organizationId')
  const security = {
    entityId,
    domain: entityTypesEnum.SERVICES,
    organizationId,
    checkOrganization:securityOrganizationCheckTypes.byOrganization,
    permisionTypes: permisionTypes.update
  }
  return (
    <div>
      <div className={styles.tableRow}>
        <div className='row align-items-center'>
          <div className='col-lg-3'>
            <div className={tableCellFirstClass}>
              <LanguageBarCell showMissing languagesAvailabilities={input.value.get('languagesAvailabilities').toJS()} />
            </div>
          </div>
          <div className='col-lg-7'>
            <div className={tableCellInlineClass}>
              <PTVIcon name='icon-eye' onClick={() => onClick(entityId)} />
              <div>
                <NameCell name={input.value.get('name').toJS()} />
                <OrganizationCell organizationId={organizationId} compact />
                {!isAsti && <ConnectionTags entityId={entityId} />}
              </div>
            </div>
          </div>
          <div className='col-lg-4'>
            <div className={styles.tableCell}>
              <ServiceTypeCell serviceTypeId={input.value.get('serviceType')} />
            </div>
          </div>
          <div className='col-lg-4'>
            <div className={styles.tableCell}>
              <ModifiedAtCell modifiedAt={input.value.get('modified')} inline />
              <ModifiedByCell
                modifiedBy={isAsti
                  ? formatMessage(connectionMessages.modifiedByASTI)
                  : input.value.get('modifiedBy')} compact />
            </div>
          </div>
          <div className='col-lg-6'>
            <div className={detailButtonClass}>
              {entityType !== entityTypesEnum.SERVICECOLLECTIONS &&
                <TableRowDetailButton
                  isOpen={isOpen}
                  onClick={onOpen}
                  shouldShowNotificationIcon={hasChildError}
                  disabled={!isOpen && hasformError || hasChildError}
                />
              }
              <Security
                id={security.entityId}
                domain={security.domain}
                checkOrganization={security.checkOrganization}
                permisionType={security.permisionTypes}
                organization={security.organizationId}
              >
                {!isAsti && isRemovable && <PTVIcon name='icon-trash' onClick={onRemove} /> }
              </Security>
            </div>
          </div>
        </div>
      </div>
      {isOpen &&
        <div>
          <AdditionalInformation
            field={field}
            index={index}
            isAsti={isAsti}
            isReadOnly={isReadOnly}
            entityConcreteType={entityConcreteTypesEnum.SERVICE}
            security={security}
            hasError={hasChildError}
          />
        </div>
      }
    </div>
  )
}
RenderServiceTableRow.propTypes = {
  fields: fieldArrayPropTypes.fields,
  index: PropTypes.number,
  input: fieldPropTypes.input,
  onClick: PropTypes.func,
  onRemove: PropTypes.func,
  onOpen: PropTypes.func,
  isOpen: PropTypes.bool,
  isAsti: PropTypes.bool,
  isRemovable: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  field: PropTypes.string,
  entityType: PropTypes.string,
  intl: intlShape,
  hasChildError: PropTypes.bool,
  hasformError: PropTypes.bool
}

const errorSelector = getFormSyncErrorWithPath(({ index }) => {
  return `selectedConnections[${index}]`
})

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const hasChildError = ownProps.formName && errorSelector(state, ownProps) || false
    const formSyncErrors = getFormSyncErrors(ownProps.formName)(state) || null
    return {
      entityType: getSelectedEntityType(state),
      hasformError: formSyncErrors && formSyncErrors.hasOwnProperty('selectedConnections'),
      hasChildError: hasChildError
    }
  }
  )
)(RenderServiceTableRow)