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
import {
  LanguageBarCell,
  NameCell,
  TableRowDetailButton,
  ArrowDown,
  ArrowUp
} from 'appComponents'
import {
  OrganizationCell,
  ModifiedAtCell,
  ModifiedByCell,
  ServiceTypeCell
} from 'Routes/FrontPage/routes/Search/components'
import { Button } from 'sema-ui-components'
import cx from 'classnames'
import styles from '../styles.scss'
import { PTVIcon } from 'Components'
import ConnectionTags from 'appComponents/ConnectionsStep/ConnectionTags'
import { injectIntl, intlShape } from 'react-intl'
import { connectionMessages } from 'appComponents/ConnectionsStep/messages'
import AdditionalInformation from 'appComponents/ConnectionsStep/AdditionalInformation'
import { entityConcreteTypesEnum } from 'enums'

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
                <OrganizationCell OrganizationIds={[input.value.get('organizationId')]} compact />
                {!isAsti && <ConnectionTags entityId={entityId} />}
              </div>
            </div>
          </div>
          <div className='col-lg-4'>
            <div className={styles.tableCell}>
              <ServiceTypeCell id={input.value.get('serviceTypeId')} />
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
              <TableRowDetailButton
                isOpen={isOpen}
                onClick={onOpen}
              />
              {!isAsti && isRemovable && <PTVIcon name='icon-trash' onClick={onRemove} /> }
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
          />
        </div>
      }
    </div>
  )
}
RenderServiceTableRow.propTypes = {
  fields: PropTypes.object,
  index: PropTypes.number,
  input: PropTypes.object,
  onClick: PropTypes.func,
  onRemove: PropTypes.func,
  onOpen: PropTypes.func,
  isOpen: PropTypes.bool,
  isAsti: PropTypes.bool,
  isRemovable: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  field: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl
)(RenderServiceTableRow)
