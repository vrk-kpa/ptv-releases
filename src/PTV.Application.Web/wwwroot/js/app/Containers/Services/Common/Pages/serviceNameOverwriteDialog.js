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
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  getServiceName,
  getGeneralServiceName
 } from '../../Service/Selectors'

import { getServiceId } from '../Selectors'

// components
import { Modal, ModalActions, ModalContent, Button } from 'sema-ui-components'

// actions
import { overwriteServiceName } from '../Actions'

const messages = defineMessages({
  dialogText: {
    id: 'Containers.Services.NameOverwriteDialog',
    defaultMessage: 'Do you want overwrite service name by general description name?'
  },
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Services.NameOverwriteDialog.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

const ServiceNameOverwriteDialog = ({
  intl: { formatMessage },
  showDialog,
  entityId,
  keyToState,
  overwriteServiceName,
  generalName
}) => {
  return (
    <div>
      <Modal isOpen={showDialog} contentLabel=''
        onRequestClose={() => overwriteServiceName()}>
        <ModalContent>{formatMessage(messages.dialogText)}</ModalContent>
        <ModalActions>
          <div className='btn-group end'>
            <Button small onClick={() => overwriteServiceName(generalName)} >{formatMessage(messages.buttonOk)}</Button>
            <Button small secondary onClick={() => overwriteServiceName()}>{formatMessage(messages.buttonCancel)}</Button>
          </div>
        </ModalActions>
      </Modal>
    </div>
  )
}

ServiceNameOverwriteDialog.propTypes = {
  intl: intlShape.isRequired,
  showDialog: PropTypes.bool.isRequired,
  entityId: PropTypes.string,
  keyToState: PropTypes.string,
  generalName: PropTypes.string,
  overwriteServiceName: PropTypes.func
}

function mapStateToProps (state, ownProps) {
  const serviceName = getServiceName(state, ownProps)
  const generalName = getGeneralServiceName(state, ownProps)
  const entityId = getServiceId(state, ownProps)
  const showDialog = serviceName !== '' && generalName !== '' && serviceName !== generalName
  return {
    showDialog,
    entityId,
    generalName
  }
}

export default compose(
  injectIntl,
  connect(mapStateToProps, { overwriteServiceName })
)(ServiceNameOverwriteDialog)
