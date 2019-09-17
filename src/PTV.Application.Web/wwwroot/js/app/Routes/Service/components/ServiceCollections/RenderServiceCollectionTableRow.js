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
import OrganizationCell from 'appComponents/Cells/OrganizationCell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import ModifiedByCell from 'appComponents/Cells/ModifiedByCell'
import cx from 'classnames'
import styles from './styles.scss'
import { PTVIcon } from 'Components'
import { injectIntl, intlShape } from 'util/react-intl'
import { connectionMessages } from 'appComponents/ConnectionsStep/messages'
import { getSelectedEntityType } from 'selectors/entities/entities'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { formTypesEnum } from 'enums'
import { fieldArrayPropTypes, fieldPropTypes } from 'redux-form/immutable'

const RenderServiceCollectionTableRow = ({
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
  showIconPreview,
  intl: { formatMessage }
}) => {
  const tableCellInlineClass = cx(
    styles.tableCell,
    styles.inline
  )
  const tableCellFirstClass = cx(
    styles.tableCell,
    styles.tableCellFirst
  )
  const entityId = input.value.get('id')
  const organizationId = input.value.get('organizationId')
  return (
    <div>
      <div className={styles.tableRow}>
        <div className='row align-items-center'>
          <div className='col-lg-4'>
            <div className={tableCellFirstClass}>
              <LanguageBarCell
                showMissing
                languagesAvailabilities={input.value.get('languagesAvailabilities').toJS()} />
            </div>
          </div>
          <div className='col-lg-13'>
            <div className={tableCellInlineClass}>
              {showIconPreview && <PTVIcon name='icon-eye' onClick={() => onClick(entityId)} />}
              <div>
                <NameCell name={input.value.get('name').toJS()} />
                <OrganizationCell organizationId={organizationId} compact />
              </div>
            </div>
          </div>
          <div className='col-lg-7'>
            <div className={styles.tableCell}>
              <ModifiedAtCell modifiedAt={input.value.get('modified')} inline />
              <ModifiedByCell
                modifiedBy={isAsti
                  ? formatMessage(connectionMessages.modifiedByASTI)
                  : input.value.get('modifiedBy')} compact />
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
RenderServiceCollectionTableRow.propTypes = {
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
  showIconPreview: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => ({
      entityType: getSelectedEntityType(state),
      showIconPreview: formName !== formTypesEnum.PREVIEW
    })
  )
)(RenderServiceCollectionTableRow)
