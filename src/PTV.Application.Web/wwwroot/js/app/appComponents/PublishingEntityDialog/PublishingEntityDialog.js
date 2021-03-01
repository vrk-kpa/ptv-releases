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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { ModalContent } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { updateUI } from 'util/redux-ui/action-reducer'
import ModalDialog from 'appComponents/ModalDialog'
import PublishingEntityForm from 'appComponents/PublishingEntityForm'

const messages = defineMessages({
  dialogTitle: {
    id: 'Components.PublishingEntityDialog.Title',
    defaultMessage: 'Kieliversioiden näkyvyys'
  },
  dialogDescription: {
    id: 'Components.PublishingEntityDialog.Description',
    defaultMessage: 'Valitse, mitkä kieliversiot haluat julkaista. ' +
      'Jos valitset, että kieliversio näkyy vain PTV:ssä, ja klikkaat Julkaise-nappia, ' +
      'tällöin myös palvelun nykyinen kieliversio piilotetaan loppukäyttäjiltä.'
  }
})

const PublishingEntityDialog = (
  { intl: { formatMessage },
  type,
  name,
  updateUI,
  entityId,
  ...rest
}) => {
  const handleCloseDialog = () => updateUI([name], 'isOpen', false)
  return (
    <ModalDialog
      name={name}
      title={formatMessage(messages.dialogTitle)}
      description={formatMessage(messages.dialogDescription)}
      contentLabel='Publishing entity'
      {...rest}
    >
      <ModalContent>
        <PublishingEntityForm onCancel={handleCloseDialog} type={type} id={entityId} />
      </ModalContent>
    </ModalDialog>
  )
}

PublishingEntityDialog.propTypes = {
  type: PropTypes.string.isRequired,
  entityId: PropTypes.string,
  name: PropTypes.string.isRequired,
  intl: intlShape.isRequired,
  actions: PropTypes.object
  // updateUI: PropTypes.func.isRequired
}

export default compose(
  injectIntl,
  connect(null, { updateUI })
)(PublishingEntityDialog)
