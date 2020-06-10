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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { isDirty } from 'redux-form/immutable'
import { reset } from 'redux-form'
import { formTypesEnum } from 'enums'
import NavigationDialog from './NavigationDialog'
import { getIsReadOnly } from 'selectors/formStates'
import { getFormName } from 'selectors/entities/entities'
import { getConnectionsMainEntity } from 'selectors/selections'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getIsMassSelectionInProgress } from 'Routes/App/selectors'
import { resetForms } from 'actions'
import { cancelReview } from 'util/redux-form/HOC/withMassToolForm/actions'
import { resetConnectionsWorkbench } from 'Routes/Connections/components/WorkbenchActions/actions'
import { goBack, goBackWithUnlockEntity } from 'appComponents/GoBackLink/actions'
import { defineMessages, injectIntl } from 'util/react-intl'

const messages = defineMessages({
  dialogTitle: {
    id: 'NavigationDialog.Title',
    defaultMessage: 'This action will discard current changes.'
  },
  reviewDialogDescription: {
    id: 'NavigationDialog.Review.Text',
    defaultMessage: 'Are you sure you want to cancel review process?',
    description: 'MassTool.CancelReviewDialog.Text'
  },
  connectionsDialogDescription: {
    id: 'NavigationDialog.Connections.Text',
    defaultMessage: 'Menetät kaikki tallentamattomat tiedot. Työpöytä ja hakutulokset tyhjenevät.',
    description: 'AppComponents.UnsavedConnectionsDialog.Text'
  },
  formDialogDescription: {
    id: 'NavigationDialog.Form.Text',
    defaultMessage: 'Are you sure you want to leave the form?',
    description: 'Containers.Channel.ViewChannel.GoBack.Text'
  },
  massSelectionDialogDescription: {
    id: 'NavigationDialog.MassSelection.Text',
    defaultMessage: 'Are you sure you want to discard selected entities?'
  },
  dialogConfirmTitle: {
    id: 'NavigationDialog.Confirm.Title',
    defaultMessage: 'Kyllä',
    description: 'Components.Buttons.Accept'
  },
  dialogCancelTitle: {
    id: 'NavigationDialog.Cancel.Title',
    defaultMessage: 'Peruuta',
    description: 'Components.Buttons.Cancel'
  }
})

const resetMassToolForm = () => ({ dispatch }) => {
  dispatch(reset(formTypesEnum.MASSTOOLSELECTIONFORM))
}

const getDialogMessages = ({
  isInReview,
  isConnectionsWorkbenchTouched,
  formName,
  isMassSelectionInProgress,
  dirtyFormsMap
}) => {
  const dialogMessages = {
    dialogTitle: messages.dialogTitle,
    dialogConfirmTitle: messages.dialogConfirmTitle,
    dialogCancelTitle: messages.dialogCancelTitle
  }
  let dialogDescription = null
  if (isInReview) {
    dialogDescription = messages.reviewDialogDescription
  } else if (isConnectionsWorkbenchTouched) {
    dialogDescription = messages.connectionsDialogDescription
  } else if (formName || dirtyFormsMap.size > 0) {
    dialogDescription = messages.formDialogDescription
  } else if (isMassSelectionInProgress) {
    dialogDescription = messages.massSelectionDialogDescription
  }
  return {
    ...dialogMessages,
    dialogDescription
  }
}

const withNavigationDialog = WrappedComponent => {
  const InnerComponent = ({
    isInReview,
    cancelReview,
    customConfirmAction,
    isConnectionsWorkbenchTouched,
    resetConnectionsWorkbench,
    formName,
    isReadOnly,
    goBack,
    goBackWithUnlockEntity,
    isMassSelectionInProgress,
    resetMassToolForm,
    resetForms,
    dirtyFormsMap,
    ...rest
  }) => {
    const dialogMessages = getDialogMessages({
      isInReview,
      isConnectionsWorkbenchTouched,
      formName,
      isMassSelectionInProgress,
      dirtyFormsMap
    })
    let confirmAction = null
    if (isInReview) {
      confirmAction = cancelReview
    } else if (isConnectionsWorkbenchTouched) {
      confirmAction = resetConnectionsWorkbench
    } else if (formName) {
      confirmAction = isReadOnly ? goBack : goBackWithUnlockEntity
    } else if (isMassSelectionInProgress) {
      confirmAction = resetMassToolForm
    } else if (dirtyFormsMap.size > 0) {
      confirmAction = () => resetForms(dirtyFormsMap)
    }
    const enhancedConfirmAction = () => {
      customConfirmAction && typeof customConfirmAction === 'function' && customConfirmAction()
      confirmAction()
    }
    return (
      <Fragment>
        <WrappedComponent {...rest} />
        <NavigationDialog
          {...dialogMessages}
          confirmAction={enhancedConfirmAction}
          history={rest.history}
        />
      </Fragment>
    )
  }

  InnerComponent.propTypes = {
    isInReview: PropTypes.bool,
    isConnectionsWorkbenchTouched: PropTypes.bool,
    isReadOnly: PropTypes.bool,
    formName: PropTypes.string,
    cancelReview: PropTypes.func,
    customConfirmAction: PropTypes.func,
    defaultAction: PropTypes.func,
    resetConnectionsWorkbench: PropTypes.func,
    goBack: PropTypes.func,
    goBackWithUnlockEntity: PropTypes.func,
    isMassSelectionInProgress: PropTypes.bool,
    resetMassToolForm: PropTypes.func,
    resetForms: PropTypes.func,
    dirtyFormsMap: ImmutablePropTypes.map
  }

  return compose(
    injectIntl,
    connect(state => {
      const mainConnectionEntity = getConnectionsMainEntity(state)
      const isConnectionsWorkbenchDirty = isDirty('connectionsWorkbench')(state)
      const formName = getFormName(state)
      return {
        isInReview: getShowReviewBar(state),
        isConnectionsWorkbenchTouched: mainConnectionEntity && isConnectionsWorkbenchDirty,
        formName,
        isReadOnly: getIsReadOnly(state, { formName }),
        isMassSelectionInProgress: getIsMassSelectionInProgress(state)
      }
    }, {
      cancelReview,
      resetConnectionsWorkbench,
      goBack,
      goBackWithUnlockEntity,
      resetMassToolForm,
      resetForms
    })
  )(InnerComponent)
}

export default withNavigationDialog
