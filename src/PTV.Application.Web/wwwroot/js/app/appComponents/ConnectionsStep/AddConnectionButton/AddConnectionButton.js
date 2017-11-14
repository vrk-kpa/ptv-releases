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
import { connect } from 'react-redux'
import { Button } from 'sema-ui-components'
import { compose } from 'redux'
import { arrayPush, getFormValues } from 'redux-form/immutable'
import { createSelector } from 'reselect'
import { List, fromJS } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'

const messages = defineMessages({
  connectedLabel: {
    id: 'appComponents.ConnectionStep.AddConnectionButton.ConnectedLabel',
    defaultMessage: 'Liitetty'
  },
  connectToLabel: {
    id: 'appComponents.ConnectionStep.AddConnectionButton.ConnectToLabel',
    defaultMessage: 'Liitä'
  }
})

const getSelectedConnections = createSelector(
  getFormValues('connections'),
  formValues => (formValues && formValues.get('selectedConnections')) || List()
)
const getSelectedConnectionsIds = createSelector(
  getSelectedConnections,
  selectedConnections => (
    selectedConnections &&
    selectedConnections.map(connection => connection.get('id'))
  ) || List()
)
const getIsSelected = id => createSelector(
  getSelectedConnectionsIds,
  selectedIds => (
    selectedIds &&
    selectedIds.some(selectedId => selectedId === id)
  ) || false
)

const AddConnectionButton = ({
  rowData,
  arrayPush,
  isSelected,
  intl: { formatMessage }
}) => {
  const handleOnAddConnection = () => {
    rowData.modified = new Date()
    rowData.modifiedBy = ''
    arrayPush(
      'connections',
      'selectedConnections',
      fromJS(rowData)
    )
  }
  return (
    <Button
      onClick={handleOnAddConnection}
      small
      secondary
      disabled={isSelected}
    >
      {isSelected
        ? formatMessage(messages.connectedLabel)
        : formatMessage(messages.connectToLabel)
      }
    </Button>
  )
}
AddConnectionButton.propTypes = {
  rowData: PropTypes.object,
  arrayPush: PropTypes.func,
  isSelected: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, { rowData: { id } }) => ({
    isSelected: getIsSelected(id)(state)
  }), {
    arrayPush
  })
)(AddConnectionButton)
