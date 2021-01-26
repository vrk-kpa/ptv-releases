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
import { connect } from 'react-redux'
import { Button } from 'sema-ui-components'
import { compose } from 'redux'
import { arrayPush, arrayUnshift, arrayInsert } from 'redux-form/immutable'
import { fromJS } from 'immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { entityTypesEnum } from 'enums'
import { getIsServiceAttachedAsASTI, getIsChannelAttachedAsASTI } from '../selectors'
import styles from './styles.scss'
import {
  getInsertIndex
} from 'Routes/Connections/selectors'
import { messages } from './messages'
import { getIsSelected, getSelectedConnections } from './selectors'

const AddConnectionButton = ({
  rowData,
  insertItem,
  isSelected,
  isAttachedAsASTI,
  storeKey,
  intl
}) => {
  const handleOnAddConnection = () => {
    rowData.modified = new Date()
    rowData.modifiedBy = ''
    insertItem(rowData.channelTypeId, fromJS(rowData), storeKey)
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
  intl: intlShape,
  storeKey: PropTypes.string
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
      isSelected: getIsSelected(state, { id }),
      isAttachedAsASTI
    }
  }, {
    insertItem: (typeId, row, storeKey) => ({ dispatch, getState }) => {
      const storeKeyValue = storeKey || 'selectedConnections'
      const state = getState()
      const connections = getSelectedConnections(state)
      const insertIndex = getInsertIndex(state, { connections, typeId })
      if (insertIndex > -1) {
        dispatch(arrayInsert('connections', storeKeyValue, insertIndex, row))
      } else {
        if (typeId) {
          dispatch(arrayPush('connections', storeKeyValue, row))
        } else {
          dispatch(arrayUnshift('connections', storeKeyValue, row))
        }
      }
    }
  })
)(AddConnectionButton)
