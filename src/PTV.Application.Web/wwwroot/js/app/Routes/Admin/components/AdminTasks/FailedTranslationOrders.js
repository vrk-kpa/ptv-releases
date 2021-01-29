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
import { compose } from 'redux'
import { connect } from 'react-redux'
import AdminTask from '../AdminTask'
// import withState from 'util/withState'
import { injectIntl } from 'util/react-intl'
import { messages } from 'Routes/Admin/messages'
import { commonAppMessages } from 'util/redux-form/messages'
import { adminTaskTypesEnum } from 'enums'
import { mergeInUIState } from 'reducers/ui'
import { getActiveDetailEntityIds } from 'Routes/Admin/selectors'
// import { List } from 'immutable'
import { Button } from 'sema-ui-components'
import DefinitionList, { DefinitionTerm, DefinitionData } from 'appComponents/DefinitionList'
import DeleteTranslationOrderDialog from 'Routes/Admin/components/dialogs/DeleteTranslationOrderDialog'
import FailedTranslationOrderLogDialog from 'Routes/Admin/components/dialogs/FailedTranslationOrderLogDialog'
import withCopyToClipboard from 'util/redux-form/HOC/withCopyToClipboard'
import styles from './styles.scss'

const CopyLink = compose(
  withCopyToClipboard
)(() => null)

const FailedTORowDetail = compose(
  injectIntl,
  connect((state, { id }) => {
    const activeIds = getActiveDetailEntityIds(state, { taskType: adminTaskTypesEnum.FAILEDTRANSLATIONORDERS })
    return {
      isVisible: activeIds.includes(id)
    }
  }, {
    mergeInUIState
  })
)(({ errorDescription, nestedItems, isVisible, intl: { formatMessage }, mergeInUIState }) => {
  const formerFailedExecutionCount = nestedItems && nestedItems.length || 0
  const handleShowLogClick = () => {
    mergeInUIState({
      key: 'failedTranslationOrderLogDialog',
      value: {
        isOpen: true,
        data: nestedItems
      }
    })
  }
  return isVisible && (
    <div className={styles.rowDetail}>
      <DefinitionList>
        <DefinitionTerm text={formatMessage(messages.failedTOErrorDescription)} />
        <DefinitionData>
          <span>{errorDescription || formatMessage(messages.failedTOErrorNotAvailable)}</span>
          {errorDescription && (
            <CopyLink
              value={errorDescription}
              title={formatMessage(commonAppMessages.copyLink)}
              feedbackMessage={messages.FailedTOCopyErrorFeedback}
            />
          )}
        </DefinitionData>
        <DefinitionTerm text={formatMessage(messages.failedTOExecutionCount)} />
        <DefinitionData>
          <span className={styles.failedTOExecutionCount}>{formerFailedExecutionCount}</span>
          {formerFailedExecutionCount > 0 && (
            <Button
              children={formatMessage(messages.failedTOShowLog)}
              onClick={handleShowLogClick}
              small
              secondary
            />
          )}
        </DefinitionData>
      </DefinitionList>
    </div>
  )
})

const FailedTranslationOrders = props => {
  return (
    <div>
      <AdminTask {...props} customRowComponent={FailedTORowDetail} useCustomTable />
      <FailedTranslationOrderLogDialog />
      <DeleteTranslationOrderDialog />
    </div>
  )
}

export default compose(
  // withState({
  //   redux: true,
  //   keepImmutable: true,
  //   key: adminTaskTypesEnum.FAILEDTRANSLATIONORDERS,
  //   initialState: {
  //     activeDetailEntityIds: List()
  //   }
  // })
)(FailedTranslationOrders)
