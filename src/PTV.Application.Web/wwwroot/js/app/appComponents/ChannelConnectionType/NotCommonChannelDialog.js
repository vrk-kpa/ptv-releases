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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { mergeInUIState } from 'reducers/ui'
import { change } from 'redux-form/immutable'
import { DataTable } from 'util/redux-form/fields'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button, Spinner } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'
import { getColumnDefinitions } from './columnDefinitions'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  getConnectionTypeNotCommonId,
  getSearchForConnectedServicesIsFetching,
  getParsedConnectedServices
} from './selectors'
import { getSelectedEntityId } from 'selectors/entities/entities'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'ConnectionType.NotCommonChannelDialog.Title',
    defaultMessage: 'Hyväksy muutos'
  },
  dialogFetchingTitle: {
    id: 'ConnectionType.NotCommonChannelDialog.FetchingTitle',
    defaultMessage: 'Checking for connected services'
  },
  dialogTooltip: {
    id: 'ConnectionType.NotCommonChannelDialog.Tooltip',
    defaultMessage: 'dialog tooltip placeholder'
  },
  dialogDescription: {
    id: 'ConnectionType.NotCommonChannelDialog.Description',
    defaultMessage: 'Kanava on kytketty alla oleviin palveluihin. Oletko varma muutoksesta?'
  },
  dialogActionConfirm: {
    id: 'ConnectionType.NotCommonChannelDialog.Confirm.Title',
    defaultMessage: 'Hyväksy muutos'
  },
  dialogActionCancel: {
    id: 'ConnectionType.NotCommonChannelDialog.Cancel.Title',
    defaultMessage: 'Peruuta',
    description: 'Components.Buttons.Cancel'
  }
})

const NotCommonChannelDialog = ({
  updateUI,
  isOpen,
  cancelReview,
  mergeInUIState,
  intl: { formatMessage },
  formName,
  notCommonId,
  change,
  isFetching,
  entityId,
  rows
}) => {
  const handleCancelAction = () => {
    mergeInUIState({
      key: 'notCommonChannelDialog',
      value: {
        isOpen: false
      }
    })
  }

  const handleConfirmAction = () => {
    change(formName, 'connectionType', notCommonId)
    mergeInUIState({
      key: 'notCommonChannelDialog',
      value: {
        isOpen: false
      }
    })
  }
  const titleMessage = isFetching && formatMessage(messages.dialogFetchingTitle) || formatMessage(messages.dialogTitle)
  return (
    <ModalDialog name='notCommonChannelDialog'
      title={titleMessage}
      tooltip={!isFetching && <Tooltip tooltip={formatMessage(messages.dialogTooltip)} />}
      className={styles.responsiveModal}
      description={!isFetching ? formatMessage(messages.dialogDescription) : ''}>
      <ModalActions>
        {!isFetching && (
          <Fragment>
            <div className={styles.buttonGroup}>
              <Button small onClick={handleConfirmAction} disabled={isFetching}>
                {formatMessage(messages.dialogActionConfirm)}</Button>
              <Button link onClick={handleCancelAction}>
                {formatMessage(messages.dialogActionCancel)}
              </Button>
            </div>
            <div className={styles.connectedServiceList}>
              <DataTable
                name={'connectedServicesWithDifferentOrganization'}
                rows={rows}
                columnsDefinition={getColumnDefinitions({
                  formatMessage,
                  withNavigation: true
                })}
                scrollable
                columnWidths={['15%', '25%', '25%', '35%']}
                sortOnClick={() => {}}
              />
            </div>
          </Fragment>
        )}
        {isFetching && <Spinner />}
      </ModalActions>
    </ModalDialog>
  )
}

NotCommonChannelDialog.propTypes = {
  updateUI: PropTypes.func,
  isOpen: PropTypes.bool,
  cancelReview: PropTypes.func,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  formName: PropTypes.string,
  notCommonId: PropTypes.string,
  change: PropTypes.func.isRequired,
  isFetching: PropTypes.bool,
  entityId: PropTypes.string,
  rows: PropTypes.array
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName }) => ({
    formName,
    notCommonId: getConnectionTypeNotCommonId(state),
    isFetching: getSearchForConnectedServicesIsFetching(state),
    entityId: getSelectedEntityId(state),
    rows: getParsedConnectedServices(state, { contentType: formName })
  }), {
    change,
    mergeInUIState
  })
)(NotCommonChannelDialog)
