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
import { ModalActions, Button } from 'sema-ui-components'
import ModalDialog from 'appComponents/ModalDialog'
import { mergeInUIState } from 'reducers/ui'
import styles from '../styles.scss'
import messages from '../messages'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'

const ActionNotAllowedDialog = ({
  formName,
  mergeInUIState,
  title,
  description,
  action,
  intl: { formatMessage }
}) => {
  const closeDialog = () => {
    mergeInUIState({
      key: `${formName}${action}NotAllowedDialog`,
      value: {
        isOpen: false
      }
    })
  }
  return (
    <ModalDialog name={`${formName}${action}NotAllowedDialog`}
      title={title}
      description={description}
      onRequestClose={closeDialog}
    >
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button small onClick={closeDialog}>
            {formatMessage(messages.okButton)}
          </Button>
        </div>
      </ModalActions>
    </ModalDialog>
  )
}

ActionNotAllowedDialog.propTypes = {
  mergeInUIState: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  title: PropTypes.string,
  description: PropTypes.string,
  action: PropTypes.string.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, {
    mergeInUIState
  })
)(ActionNotAllowedDialog)
