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
import { arrayPush, arrayUnshift, getFormValues, arrayInsert } from 'redux-form/immutable'
import { createSelector } from 'reselect'
import { List, fromJS } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { entityTypesEnum } from 'enums'
import { getIsServiceAttachedAsASTI, getIsChannelAttachedAsASTI } from '../selectors'
import styles from './styles.scss'
import {
  getInsertIndex
} from 'Routes/Connections/selectors'

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

// regular connections
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
  insertItem,
  isSelected,
  isAttachedAsASTI,
  intl
}) => {
  const handleOnAddConnection = () => {
    rowData.modified = new Date()
    rowData.modifiedBy = ''
    insertItem(rowData.channelTypeId, fromJS(rowData))
  }
  return (
    <Button
      onClick={handleOnAddConnection}
      small
      secondary
      disabled={isAttachedAsASTI || isSelected}
      className={styles.addConnectionButton}
    >
      {isSelected || isAttachedAsASTI
        ? intl.formatMessage(messages.connectedLabel)
        : intl.formatMessage(messages.connectToLabel)
      }
    </Button>
  )
}
AddConnectionButton.propTypes = {
  rowData: PropTypes.object,
  insertItem: PropTypes.func,
  isSelected: PropTypes.bool,
  isAttachedAsASTI: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, { rowData: { id }, ...rest }) => {
    const entityType = getSelectedEntityType(state)
    const isAttachedAsASTI = {
      [entityTypesEnum.CHANNELS]: getIsServiceAttachedAsASTI(id)(state),
      [entityTypesEnum.SERVICES]: getIsChannelAttachedAsASTI(id)(state),
      [entityTypesEnum.GENERALDESCRIPTIONS]: false
    }[entityType]
    return {
      isSelected: getIsSelected(id)(state),
      isAttachedAsASTI
    }
  }, {
    insertItem: (typeId, row) => ({ dispatch, getState }) => {
      const state = getState()
      const connections = getSelectedConnections(state)
      const insertIndex = getInsertIndex(state, { connections, typeId })
      if (insertIndex > -1) {
        dispatch(arrayInsert('connections', 'selectedConnections', insertIndex, row))
      } else {
        if (typeId) {
          dispatch(arrayPush('connections', 'selectedConnections', row))
        } else {
          dispatch(arrayUnshift('connections', 'selectedConnections', row))
        }
      }
    }
  })
)(AddConnectionButton)
