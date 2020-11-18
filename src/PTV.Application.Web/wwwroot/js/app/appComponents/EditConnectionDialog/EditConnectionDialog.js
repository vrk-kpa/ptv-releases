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
import {
  Modal,
  ModalActions,
  Button
} from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import {
  getReduxFormKey,
  getFieldName,
  getConnectedEntityFieldValue,
  getMainEntityFieldValue
} from './selectors'
import { setReadOnly } from 'reducers/formStates'
import { clearContentLanguage } from 'reducers/selections'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { entityTypesEnum } from 'enums'
import MainEntity from 'Routes/Connections/components/MainEntity'
import EntityRow from 'appComponents/EntityRow'
import Child from 'Routes/Connections/components/Child'
import RenderServiceTableRow from 'appComponents/ConnectionsStep/ServiceConnections/RenderServiceTableRow'
import RenderChannelTableRow from 'appComponents/ConnectionsStep/ChannelConnections/RenderChannelTableRow'
import {
  isDirty,
  change,
  reset
} from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'
import CancelConnectionPopup from './CancelConnectionPopup.js'
import SaveEditButton from './SaveEditButton'

const messages = defineMessages({
  cancelUpdateButton: {
    id: 'Components.Buttons.CancelUpdateButton',
    defaultMessage: 'Keskeytä'
  },
  close: {
    id: 'Components.Buttons.Close',
    defaultMessage: 'Sulje'
  }
})

const EditConnectionDialog = ({
  isOpen,
  parentIndex,
  childIndex,
  groupIndex,
  mergeInUIState,
  field,
  inputValue,
  isDirty,
  change,
  reset,
  formName,
  intl: { formatMessage },
  isAsti,
  mainEntityInputValue,
  isEdit,
  selectedEntityType,
  reduxKey,
  setReadOnly,
  security,
  clearContentLanguage,
  ...rest
}) => {
  const openConfirmDialog = (callback) => {
    mergeInUIState({
      key: 'CancelConnectionPopup',
      value: {
        isOpen: true,
        onCancelSuccess: callback
      }
    })
  }
  const close = () => {
    setReadOnly({
      form: formName,
      value: true
    })
    mergeInUIState({
      key: reduxKey,
      value: {
        isOpen: false,
        parentIndex: null,
        childIndex: null,
        groupIndex: null,
        isAsti: false,
        isEdit: false
      }
    })
    clearContentLanguage({ languageKey: 'connectionDialogPreview' })
  }
  const handleOnClose = () => {
    if (isDirty) {
      openConfirmDialog(() => {
        reset(formName)  
        close()              
      })
    } else {
      reset(formName)
      close()
    }
  }
  const handleOnCancel = () => {
    if (isDirty) {
      openConfirmDialog(() => {
        reset(formName)
        mergeInUIState({
          key: reduxKey,
          value: {
            isEdit: false
          }
        })
      })
    } else {
      reset(formName)
      mergeInUIState({
        key: reduxKey,
        value: {
          isEdit: false
        }
      })
    }
  }

  const customModalStyles = {
    content: {
      width: '100%',
      maxWidth: '1000px'
    }
  }
  return (
    <Modal
      className={styles.editConnectionDialog}
      isOpen={isOpen}
      onRequestClose={handleOnClose}
      contentLabel='Connection edit dialog'
      contentScroll
      style={customModalStyles}>
      <div>
        <div className={styles.mainEntityDialogWrap}>
          {!selectedEntityType &&
          <MainEntity
            connectionIndex={parentIndex}
            input={{ value: mainEntityInputValue }}
            modalMode
          /> ||
          <EntityRow
            input={{ value: mainEntityInputValue }}
            entityType={selectedEntityType}
          />}
          <SaveEditButton
            isEdit={isEdit}
            isAsti={isAsti}
            security={security}
            reduxKey={reduxKey}
            className={styles.top}
          />
        </div>
        {!selectedEntityType
          ? <Child
            childIndex={childIndex}
            connectionIndex={parentIndex}
            field={field}
            isOpen
            input={{ value: inputValue }}
            modalMode
            isAsti={isAsti}
            className={styles.connectionWrap}
            isReadOnly={!isEdit}
          />
          : selectedEntityType === entityTypesEnum.CHANNELS
            ? <RenderServiceTableRow
              field={field}
              index={childIndex}
              isAsti={isAsti}
              input={{ value: inputValue, name: field }}
              modalMode
              isOpen
              className={styles.connectionWrap}
              isReadOnly={!isEdit}
            />
            : <RenderChannelTableRow
              field={field}
              index={childIndex}
              isAsti={isAsti}
              input={{ value: inputValue, name: field }}
              modalMode
              isOpen
              className={styles.connectionWrap}
              isReadOnly={!isEdit}
            />
        }
        <ModalActions>
          <div className={styles.buttonGroup}>
            <SaveEditButton isEdit={isEdit} isAsti={isAsti} security={security} reduxKey={reduxKey} />
            {!isEdit && <Button onClick={handleOnClose} small secondary children={formatMessage(messages.close)} />}
            {isEdit && (
              <CancelConnectionPopup
                trigger={
                  <Button
                    onClick={handleOnCancel}
                    link
                    children={formatMessage(messages.cancelUpdateButton)}
                  />
                }
              />
            )}
          </div>
        </ModalActions>
      </div>
    </Modal>
  )
}
EditConnectionDialog.propTypes = {
  parentIndex: PropTypes.number,
  childIndex: PropTypes.number,
  groupIndex: PropTypes.number,
  isOpen: PropTypes.bool,
  isEdit: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  isDirty: PropTypes.bool.isRequired,
  field: PropTypes.string,
  inputValue: PropTypes.object,
  change: PropTypes.func.isRequired,
  reset: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  isAsti: PropTypes.bool,
  mainEntityInputValue: PropTypes.object,
  selectedEntityType: PropTypes.string,
  reduxKey: PropTypes.string,
  setReadOnly: PropTypes.func,
  security: PropTypes.object,
  clearContentLanguage: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { isAsti, parentIndex, childIndex, groupIndex, formName }) => {
    const childType = isAsti ? 'astiChilds' : 'childs'
    const reduxFormKey = getReduxFormKey(state, { isAsti })
    return {
      field: getFieldName(state, { childType, parentIndex, childIndex, groupIndex, reduxFormKey }),
      inputValue: getConnectedEntityFieldValue(state, {
        reduxFormKey,
        parentIndex,
        childIndex,
        groupIndex,
        childType,
        formName
      }),
      isDirty: isDirty(formName)(state),
      mainEntityInputValue: getMainEntityFieldValue(state, { reduxFormKey, parentIndex, childIndex, groupIndex, formName }),
      selectedEntityType: getSelectedEntityType(state)
    }
  }, {
    mergeInUIState,
    change,
    reset,
    setReadOnly,
    clearContentLanguage
  })
)(EditConnectionDialog)
