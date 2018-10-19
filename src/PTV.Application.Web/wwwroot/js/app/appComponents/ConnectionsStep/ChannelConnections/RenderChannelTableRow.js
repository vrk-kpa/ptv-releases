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
import NameCell from 'appComponents/Cells/NameCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import TableRowDetailButton from 'appComponents/TableRowDetailButton'
import cx from 'classnames'
import styles from 'appComponents/ConnectionsStep/styles.scss'
import { PTVIcon } from 'Components'
import ConnectionTags from 'appComponents/ConnectionsStep/ConnectionTags'
import { injectIntl, intlShape } from 'util/react-intl'
import { connectionMessages } from 'appComponents/ConnectionsStep/messages'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { entityTypesEnum,
  securityOrganizationCheckTypes,
  permisionTypes } from 'enums'
import { Security } from 'appComponents/Security'
import { isChannelConnectionCommon } from 'appComponents/ConnectionsStep/selectors'
import { fieldPropTypes } from 'redux-form/immutable'

const RenderChannelTableRow = ({
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
  isCommon,
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
  const entityId = input.value.get('id')
  const channelCode = input.value.get('channelType')
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  const organizationId = input.value.get('organizationId')
  const security = {
    entityId,
    domain: entityTypesEnum.CHANNELS,
    organizationId,
    checkOrganization: isCommon ? securityOrganizationCheckTypes.ownOrganization : securityOrganizationCheckTypes.byOrganization,
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
              <PTVIcon name='icon-eye' onClick={() => onClick(entityId, channelCode)} />
              <div>
                <NameCell name={input.value.get('name').toJS()} />
                <OrganizationCell organizationId={organizationId} compact />
                {!isAsti && <ConnectionTags entityId={entityId} />}
              </div>
            </div>
          </div>
          <div className='col-lg-4'>
            <div className={styles.tableCell}>
              <ChannelTypeCell channelTypeId={input.value.get('channelTypeId')} />
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
              {entityType !== entityTypesEnum.GENERALDESCRIPTIONS &&
                <TableRowDetailButton
                  isOpen={isOpen}
                  onClick={onOpen}
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
        <AdditionalInformation
          index={index}
          field={field}
          isAsti={isAsti}
          isReadOnly={isReadOnly}
          entityConcreteType={channelCode}
          security={security}
        />
      }
    </div>
  )
}
RenderChannelTableRow.propTypes = {
  index: PropTypes.number,
  input: fieldPropTypes.input,
  onClick: PropTypes.func,
  onRemove: PropTypes.func,
  onOpen: PropTypes.func,
  isOpen: PropTypes.bool,
  isCommon: PropTypes.bool,
  isAsti: PropTypes.bool,
  isRemovable: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  field: PropTypes.string,
  entityType: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => ({
      entityType: getSelectedEntityType(state),
      isCommon: isChannelConnectionCommon(state, ownProps)
    })
  )
)(RenderChannelTableRow)
