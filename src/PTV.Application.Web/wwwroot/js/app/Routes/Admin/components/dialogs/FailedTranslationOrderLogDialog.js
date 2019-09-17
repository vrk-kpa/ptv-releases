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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button, ModalContent } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { getUiFailedTranslationOrderLogData } from 'Routes/Admin/selectors'
import { buttonMessages } from 'Routes/messages'
import DefinitionList, { DefinitionTerm, DefinitionData } from 'appComponents/DefinitionList'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import ComposedCell from 'appComponents/Cells/ComposedCell'
import Cell from 'appComponents/Cells/Cell'
import ModifiedAtCell from 'appComponents/Cells/ModifiedAtCell'
import OrderStateCell from 'appComponents/TranslationOrderDialog/components/OrderStateCell'
import styles from './styles.scss'

const messages = defineMessages({
  title: {
    id: 'FailedTranslationOrderLogDialog.Title',
    defaultMessage: 'Failed translation orders log'
  }
})

const LogItem = compose(
  injectIntl
)(({
  intl: { formatMessage },
  senderEmail,
  sentAt,
  translationStateTypeId
}) => {
  return (
    <DefinitionList alterBg>
      <DefinitionTerm text={formatMessage(CellHeaders.orderDateWithOrderer)} />
      <DefinitionData>
        <ComposedCell
          inTable
          main={<ModifiedAtCell inline modifiedAt={sentAt} />}
          sub={<Cell>{senderEmail}</Cell>}
        />
      </DefinitionData>
      <DefinitionTerm text={formatMessage(CellHeaders.errorCode)} />
      <DefinitionData>
        <OrderStateCell id={translationStateTypeId} />
      </DefinitionData>
    </DefinitionList>
  )
})

const closeDialog = (mergeInUIState, dialogKey) => {
  mergeInUIState({
    key: dialogKey,
    value: {
      isOpen: false
    }
  })
}

const FailedTranslationOrderLogDialog = ({
  title,
  mergeInUIState,
  intl: { formatMessage },
  data,
  ...rest
}) => {
  const dialogKey = 'failedTranslationOrderLogDialog'
  const handleCancelAction = () => {
    closeDialog(mergeInUIState, dialogKey)
  }
  return (
    <ModalDialog
      name={dialogKey}
      title={formatMessage(messages.title)}
      contentLabel='Failed Translation Order Log Dialog'
      style={{ content: { maxWidth: '60rem', minWidth: 'auto' } }}
    >
      <ModalContent>
        <div className={styles.log}>
          {data.map(d => (
            <LogItem
              key={d.get('id')}
              senderEmail={d.get('senderEmail')}
              sentAt={d.get('sentAt')}
              translationStateTypeId={d.get('translationStateTypeId')}
            />
          ))}
        </div>
      </ModalContent>
      <ModalActions>
        <Button
          small
          onClick={() => handleCancelAction()}
          children={formatMessage(buttonMessages.close)}
        />
      </ModalActions>
    </ModalDialog>
  )
}

FailedTranslationOrderLogDialog.propTypes = {
  title: PropTypes.node,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  data: ImmutablePropTypes.list
}

export default compose(
  injectIntl,
  connect(state => ({
    data: getUiFailedTranslationOrderLogData(state)
  }), {
    mergeInUIState
  })
)(FailedTranslationOrderLogDialog)
