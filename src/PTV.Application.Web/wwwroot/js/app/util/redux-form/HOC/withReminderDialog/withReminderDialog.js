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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import {
  Modal,
  ModalActions,
  ModalTitle,
  Button
} from 'sema-ui-components'
import withState from 'util/withState'
import styles from './styles.scss'

const messages = defineMessages({
  dialogTitle: {
    id: 'ReminderDialog.Title',
    defaultMessage: 'Remember to save.'
  },
  dialogConfirmTitle: {
    id: 'ReminderDialog.Confirm.Title',
    defaultMessage: 'Ok'
  }
})

const withReminderDialog = WrappedComponent => {
  const InnerComponent = ({
    isOpen,
    updateUI,
    ...rest
  }) => {
    const hideDialog = () => {
      updateUI({
        'isOpen': false
      })
    }
    return (
      <Fragment>
        <Modal
          isOpen={isOpen}
          onRequestClose={hideDialog}
          contentLabel='Reminder_Dialog'
        >
          <ModalTitle title={rest.intl.formatMessage(messages.dialogTitle)} />
          <ModalActions>
            <div className={styles.buttonGroup}>
              <Button small onClick={hideDialog}>
                {rest.intl.formatMessage(messages.dialogConfirmTitle)}</Button>
            </div>
          </ModalActions>
        </Modal>
        <WrappedComponent {...rest} />
      </Fragment>
    )
  }

  InnerComponent.propTypes = {
    intl: intlShape.isRequired,
    isOpen: PropTypes.bool,
    updateUI: PropTypes.func
  }

  return compose(
    injectIntl,
    withState({
      key: 'reminderDialog',
      initialState: {
        isOpen: false
      },
      redux: true
    })
  )(InnerComponent)
}

export default withReminderDialog
