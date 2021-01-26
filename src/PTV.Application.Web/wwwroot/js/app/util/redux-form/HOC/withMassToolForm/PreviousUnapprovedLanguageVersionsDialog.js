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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import ModalDialog from 'appComponents/ModalDialog'
import Paragraph from 'appComponents/Paragraph'
import { ContinueButton, CancelButton } from 'appComponents/Buttons'
import { ModalActions, ModalContent } from 'sema-ui-components'
import { moveToStep } from './actions'
import { mergeInUIState } from 'reducers/ui'
import {
  getReviewCurrentStep,
  getContentLanguageCode
} from 'selectors/selections'
import { getCurrentItem } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getMissingPreviousLanguagesForApproveTranslated } from 'selectors/massTool'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { formTypesEnum } from 'enums'

const messages = defineMessages({
  dialogTitle: {
    id: 'PreviousUnapprovedLanguageVersionsDialog.Title',
    defaultMessage: 'You have skipped other previously published language versions for this content'
  },
  dialogDescription1: {
    id: 'PreviousUnapprovedLanguageVersionsDialog.Text1',
    defaultMessage: 'You haven\'t accepted following previously published language versions ({languages}) for this content'
  },
  dialogDescription2: {
    id: 'PreviousUnapprovedLanguageVersionsDialog.Text2',
    defaultMessage: 'Keep in mind that you won\'t be able to publish the entity unless you approve all previously published language versions. You can return to them using previous button from the review bar or later from the mass publish summary page.' // eslint-disable-line
  },
  actionConfirm: {
    id: 'PreviousUnapprovedLanguageVersionsDialog.ContinueButton',
    defaultMessage: 'Jatka hyväksymättä'
  }
})

const closeDialog = (mergeInUIState, dialogKey) => {
  mergeInUIState({
    key: dialogKey,
    value: {
      isOpen: false
    }
  })
}

const PreviousUnapprovedLanguageVersionsDialog = ({
  title,
  description,
  moveToStep,
  mergeInUIState,
  currentStep,
  approveMissingPreviousLanguages,
  intl: { formatMessage }
}) => {
  const dialogKey = 'previousUnapprovedLanguageVersionsDialog'
  const handleConfirmAction = () => {
    moveToStep(currentStep + 1)
    closeDialog(mergeInUIState, dialogKey)
  }
  const handleCancelAction = () => {
    closeDialog(mergeInUIState, dialogKey)
  }
  return (
    <ModalDialog
      name={dialogKey}
      title={formatMessage(messages.dialogTitle)}
      contentLabel='Previous unapproved language versions dialog'
      style={{ content: { maxWidth: '30rem', minWidth: 'auto' } }}
    >
      <ModalContent>
        <Paragraph>{formatMessage(messages.dialogDescription1, {
          languages: approveMissingPreviousLanguages.map(l => l.name).join(', ')
        })}</Paragraph>
        <Paragraph>{formatMessage(messages.dialogDescription2)}</Paragraph>
      </ModalContent>
      <ModalActions>
        <ContinueButton
          onClick={handleConfirmAction}
          children={formatMessage(messages.actionConfirm)}
        />
        <CancelButton onClick={handleCancelAction} />
      </ModalActions>
    </ModalDialog>
  )
}

PreviousUnapprovedLanguageVersionsDialog.propTypes = {
  title: PropTypes.node,
  description: PropTypes.node,
  moveToStep: PropTypes.func,
  currentStep: PropTypes.number,
  mergeInUIState: PropTypes.func,
  approveMissingPreviousLanguages: ImmutablePropTypes.set,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(state => {
    const language = getContentLanguageCode(state)
    const currentItem = getCurrentItem(state, { language })
    return {
      currentStep: getReviewCurrentStep(state),
      approveMissingPreviousLanguages: getMissingPreviousLanguagesForApproveTranslated(
        state, {
          id: currentItem.get('unificRootId'),
          formName: formTypesEnum.MASSTOOLFORM
        }
      )
    }
  }, {
    moveToStep,
    mergeInUIState
  })
)(PreviousUnapprovedLanguageVersionsDialog)
