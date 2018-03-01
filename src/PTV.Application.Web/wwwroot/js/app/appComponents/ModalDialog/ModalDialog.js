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
import { Modal, ModalTitle, ModalActions, ModalContent, Button } from 'sema-ui-components'
import { compose } from 'redux'
import { defineMessages, injectIntl } from 'react-intl'
import withState from 'util/withState'

const messages = defineMessages({
  closeDialogButtonTitle: {
    id: 'Components.ModalDialog.Buttons.Close.Title',
    defaultMessage: 'Peruuta'
  }
})

const ModalDialog = ({
  name,
  isOpen,
  updateUI,
  title,
  description,
  actionClose,
  children,
  intl: { formatMessage },
  contentLabel,
  onRequestClose,
  ...rest
}) => {
  const handleOnClose = () => {
    updateUI('isOpen', !isOpen)
    onRequestClose && onRequestClose()
  }

  return (
    <Modal isOpen={isOpen}
      onRequestClose={handleOnClose}
      contentLabel={contentLabel} {...rest}>
      {title && <ModalTitle title={title} />}
      {description && <ModalContent>{description}</ModalContent>}
      {children}
      {actionClose && <ModalActions>
        <Button onClick={handleOnClose}>{formatMessage(messages.closeDialogButtonTitle)}</Button>
      </ModalActions>}
    </Modal>
  )
}

ModalDialog.propTypes = {
  id: PropTypes.string,
  updateUI: PropTypes.func.isRequired,
  title: PropTypes.string,
  description: PropTypes.string,
  name: PropTypes.string.isRequired,
  actionClose: PropTypes.bool,
  isOpen: PropTypes.bool,
  children: PropTypes.object,
  intl: PropTypes.object.isRequired,
  contentLabel: PropTypes.string.isRequired
}

ModalDialog.defaultProps = {
  actionClose: false,
  isOpen: false,
  contentLabel: ''
}

export default compose(
  withState({
    key: props => props.name,
    initialState: {
      isOpen: false
    },
    redux: true
  }),
  injectIntl
)(ModalDialog)
