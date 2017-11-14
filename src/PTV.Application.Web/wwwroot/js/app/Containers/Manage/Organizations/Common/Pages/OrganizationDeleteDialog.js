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
  getDeleteAnyConnected,
  getDeleteSubOrganizationsConnected,
  getIsFetchingOfAnyStep } from '../../../../Common/Selectors'

import { getOrganizationId } from '../../Common/Selectors'

// components
import { Modal, ModalActions, ModalContent, Button } from 'sema-ui-components'

// actions
import { forceDeleteOrganization } from '../../Organization/Actions'
import { deleteApiCall } from '../../../../Common/Actions'

const messages = defineMessages({
  serviceAndChannelText: {
    id: 'Containers.Organizations.DeleteDialog.ServiceAndChannels',
    defaultMessage: 'You have services and channels connected to this organization. Do you want to archive all services and channels?'
  },
  subOrganizationText: {
    id: 'Containers.Organizations.DeleteDialog.SubOrganization',
    defaultMessage: 'You have sub-organization(s), services and channels connected to this organization. Do you want to archive sub-organization and services and channels?'
  },
  buttonOk: {
    id: 'Containers.Organizations.DeleteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Containers.Organizations.DeleteDialog.Cancel',
    defaultMessage: 'Jatka muokkausta'
  }
})

const OrganizationDeleteDialog = ({
  intl: { formatMessage },
  showDialog,
  anyConnected,
  subConnected,
  entityId,
  keyToState,
  forceDeleteOrganization,
  deleteApiCall
}) => {
  return (
    <div>
      <Modal isOpen={showDialog} contentLabel=''
        onRequestClose={() => deleteApiCall([keyToState, 'delete'])}>
        <ModalContent>{formatMessage(subConnected ? messages.subOrganizationText : messages.serviceAndChannelText)}</ModalContent>
        <ModalActions>
          <div className='btn-group end'>
            <Button small onClick={() => { forceDeleteOrganization(entityId, keyToState); deleteApiCall([keyToState, 'delete']) }}>{formatMessage(messages.buttonOk)}</Button>
            <Button small secondary onClick={() => deleteApiCall([keyToState, 'delete'])}>{formatMessage(messages.buttonCancel)}</Button>
          </div>
        </ModalActions>
      </Modal>
    </div>
  )
}

OrganizationDeleteDialog.propTypes = {
  intl: intlShape.isRequired,
  showDialog: PropTypes.bool.isRequired,
  anyConnected: PropTypes.bool.isRequired,
  subConnected: PropTypes.bool.isRequired
}

function mapStateToProps (state, ownProps) {
  const isFetching = getIsFetchingOfAnyStep(state, ownProps)
  const anyConnected = getDeleteAnyConnected(state, ownProps)
  const subConnected = getDeleteSubOrganizationsConnected(state, ownProps)
  const entityId = getOrganizationId(state, ownProps)
  const showDialog = !isFetching && anyConnected
  return {
    showDialog,
    anyConnected,
    subConnected,
    entityId
  }
}

export default compose(
  injectIntl,
  connect(mapStateToProps, { forceDeleteOrganization, deleteApiCall })
)(OrganizationDeleteDialog)
