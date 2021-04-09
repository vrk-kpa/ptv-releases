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
import LanguageBarCell from 'appComponents/Cells/LanguageBarCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import NameCell from 'appComponents/Cells/NameCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import ValueCell from 'appComponents/Cells/ValueCell'
import cx from 'classnames'
import styles from '../styles.scss'
import { PTVIcon } from 'Components'
import ConnectionTags from 'appComponents/ConnectionsStep/ConnectionTags'
import { injectIntl, intlShape } from 'util/react-intl'
import { messages } from 'appComponents/ConnectionsStep/messages'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import {
  entityConcreteTypesEnum,
  entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes
} from 'enums'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { getHasAnyConnectedServiceValue } from 'appComponents/ConnectionsStep/selectors'
import { getFormSyncErrorWithPath } from 'selectors/base'
import { Security } from 'appComponents/Security'
import { fieldPropTypes, isDirty } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { EditIcon } from 'appComponents/Icons'
import { mergeInUIState } from 'reducers/ui'
import { getServiceTypeId } from '../selectors'

const RenderServiceTableRow = ({
  index,
  groupIndex,
  input,
  onClick,
  onRemove,
  isAsti,
  isRemovable,
  isReadOnly,
  field,
  entityType,
  hasChildError,
  hasAnyChildValue,
  isDirty,
  mergeInUIState,
  modalMode,
  className,
  serviceTypeId,
  intl: { formatMessage }
}) => {
  const nameClass = cx(
    styles.tableCell,
    styles.inline,
    styles.name
  )
  const detailButtonClass = cx(
    styles.tableCell,
    styles.inline,
    styles.withDetailButton,
    styles.action
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
    checkOrganization: securityOrganizationCheckTypes.byOrganization,
    permisionTypes: permisionTypes.update
  }

  const onEditClick = ({ field, childIndex, groupIndex, isAsti, security }) => {
    mergeInUIState({
      key: isAsti ? 'editAstiSmallConnectionDialog' : 'editSmallConnectionDialog',
      value: {
        isOpen: true,
        field,
        childIndex,
        groupIndex,
        isAsti,
        security
      }
    })
  }
  const wrapClass = cx(
    className,
    {
      [styles.modal]: modalMode
    }
  )

  const renderServiceCollectionRow = () => {
    return (
      <div className={wrapClass}>
        <div className={styles.tableRow}>
          <div className='row align-items-center'>
            {!modalMode &&
              <div className='col-lg-4'>
                <div className={tableCellFirstClass}>
                  <LanguageBarCell showMissing languagesAvailabilities={input.value.get('languagesAvailabilities').toJS()} />
                </div>
              </div>
            }
            <div className={modalMode ? 'col-lg-14' : 'col-lg-9'}>
              <div className={nameClass}>
                {!modalMode && <PTVIcon name='icon-eye' onClick={() => onClick(entityId)} />}
                <div>
                  <NameCell name={input.value.get('name').toJS()} />
                  <OrganizationCell organizationId={organizationId} compact />
                  {!isAsti && <ConnectionTags entityId={entityId} />}
                </div>
              </div>
            </div>
            <div className='col-lg-5'>
              <div className={styles.tableCell}>
                <ServiceTypeCell serviceTypeId={serviceTypeId} />
              </div>
            </div>
            {!modalMode &&
              <React.Fragment>
                <div className='col-lg-4'>
                  <div className={styles.tableCell}>
                    <ModifiedAtCell modifiedAt={input.value.get('modified')} inline />
                    <ModifiedByCell
                      modifiedBy={isAsti
                        ? formatMessage(messages.modifiedByASTI)
                        : input.value.get('modifiedBy')} compact />
                  </div>
                </div>
                <div className='col-lg-2'>
                  <div className={detailButtonClass}>
                    <Security
                      id={security.entityId}
                      domain={security.domain}
                      checkOrganization={security.ownOrganization}
                      permisionType={security.permisionTypes}
                      organization={security.organizationId}
                    >
                      {!isAsti && isRemovable && <PTVIcon name='icon-trash' onClick={onRemove} /> }
                    </Security>
                  </div>
                </div>
              </React.Fragment>
            }
          </div>
        </div>
        {modalMode &&
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

  if (entityType === entityTypesEnum.SERVICECOLLECTIONS) {
    return renderServiceCollectionRow()
  }

  return (
    <div className={wrapClass}>
      <div className={styles.tableRow}>
        <div className='row align-items-center'>
          {!modalMode &&
            <div className='col-lg-4'>
              <div className={tableCellFirstClass}>
                <LanguageBarCell showMissing languagesAvailabilities={input.value.get('languagesAvailabilities').toJS()} />
              </div>
            </div>
          }
          <div className={modalMode ? 'col-lg-12' : 'col-lg-8'}>
            <div className={nameClass}>
              {!modalMode && <PTVIcon name='icon-eye' onClick={() => onClick(entityId)} />}
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
          {!modalMode &&
            <React.Fragment>
              <div className='col-lg-4'>
                <div className={styles.tableCell}>
                  <ModifiedAtCell modifiedAt={input.value.get('modified')} inline />
                  <ModifiedByCell
                    modifiedBy={isAsti
                      ? formatMessage(messages.modifiedByASTI)
                      : input.value.get('modifiedBy')} compact />
                </div>
              </div>
              <div className='col-lg-4'>
                <div className={detailButtonClass}>
                  {entityType !== entityTypesEnum.SERVICECOLLECTIONS && !hasAnyChildValue && (
                    <ValueCell
                      componentClass={styles.hasValueLabel}
                      value={formatMessage(messages.connectionsNoDetailInformation)}
                    />
                  )}
                  {entityType !== entityTypesEnum.SERVICECOLLECTIONS &&
                    <EditIcon
                      onClick={() => onEditClick({ field, childIndex: index, isAsti, groupIndex, security })}
                      disabled={isDirty}
                    />
                  }
                  <Security
                    id={security.entityId}
                    domain={security.domain}
                    checkOrganization={security.ownOrganization}
                    permisionType={security.permisionTypes}
                    organization={security.organizationId}
                  >
                    {!isAsti && isRemovable && <PTVIcon name='icon-trash' onClick={onRemove} /> }
                  </Security>
                </div>
              </div>
            </React.Fragment>
          }
        </div>
      </div>
      {modalMode &&
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
  index: PropTypes.number,
  groupIndex: PropTypes.number,
  input: fieldPropTypes.input,
  onClick: PropTypes.func,
  onRemove: PropTypes.func,
  isAsti: PropTypes.bool,
  isRemovable: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  field: PropTypes.string,
  entityType: PropTypes.string,
  intl: intlShape,
  hasChildError: PropTypes.bool,
  hasAnyChildValue: PropTypes.bool,
  isDirty: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  modalMode: PropTypes.bool,
  className: PropTypes.string,
  serviceTypeId: PropTypes.string
}

const errorSelector = getFormSyncErrorWithPath(({ index }) => {
  return `selectedConnections[${index}]`
})

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => {
    const hasChildError = ownProps.formName && errorSelector(state, ownProps) || false
    const hasAnyChildValue = getHasAnyConnectedServiceValue(state, { childIndex: ownProps.index, isAsti: ownProps.isAsti }) || false
    const subEntityType = ownProps.input && ownProps.input.value && ownProps.input.value.get('subEntityType')
    return {
      entityType: getSelectedEntityType(state),
      hasChildError: hasChildError,
      hasAnyChildValue: hasAnyChildValue,
      isDirty: isDirty(ownProps.formName)(state),
      serviceTypeId: getServiceTypeId(state, { subEntityType })
    }
  }, {
    mergeInUIState
  })
)(RenderServiceTableRow)
