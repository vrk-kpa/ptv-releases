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
import { compose } from 'redux'
import { defineMessages, injectIntl } from 'react-intl'
import { ModalContent, Button } from 'sema-ui-components'
import ModalDialog from '../../appComponents/ModalDialog'
import { mergeInUIState } from 'reducers/ui'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import styles from './styles.scss'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { injectFormName } from 'util/redux-form/HOC'
import { getSelectedEntityId } from 'selectors/entities/entities'
import { connect } from 'react-redux'
import { getContentLanguageId } from 'selectors/selections'
import { formApiPaths } from 'enums'
import TranslationOrderContent from './TranslationOrderContent'

const messages = defineMessages({
  dialogTitle: {
    id: 'Components.TranslationOrderDialog.Title',
    defaultMessage: 'Käännöstilaus'
  },
  linkTitle: {
    id: 'Components.TranslationOrder.Link.Title',
    defaultMessage: 'Käännöstilaukset'
  }
})

const TranslationOrderDialog = ({
  intl: { formatMessage },
  formName,
  entityId,
  sourceLanguage,
  dispatch
}) => {
  const validateSuccess = () => {
    dispatch(mergeInUIState({
      key: `TranslationOrderDialog`,
      value: {
        isOpen: true
      }
    }))
  }

  const onRequestClose = () => {
    dispatch(mergeInUIState({
      key: `TranslationOrderDialog`,
      value: {
        isOpen: false
      }
    }))
    dispatch(mergeInUIState({
      key: `translationOrderDialogActiveIndex`,
      value: {
        activeIndex: 0
      }
    }))
    dispatch(mergeInUIState({
      key: `translationOrderDialogReorderLanguage`,
      value: {
        reorderLang: null
      }
    }))
  }

  const handleTranslationDialog = () => {
    dispatch(
      apiCall3({
        keys: ['translationOrderForm', 'translationOrderDialog'],
        payload: {
          endpoint: formApiPaths[formName] + '/GetTranslationData',
          data: { entityId: entityId, sourceLanguage: sourceLanguage }
        },
        schemas: EntitySchemas.TRANSLATION,
        successNextAction: validateSuccess,
        formName
      })
    )
  }

  return (
    <div>
      <div>
        <Button
          link
          onClick={handleTranslationDialog}
          children={formatMessage(messages.linkTitle)}
        />
      </div>
      <div>
        <ModalDialog
          name={`TranslationOrderDialog`}
          title={formatMessage(messages.dialogTitle)}
          className={styles.orderForm}
          style={{ content: { minHeight: '300px' } }}
          onRequestClose={onRequestClose}
        >
          <ModalContent>
            <TranslationOrderContent />
          </ModalContent>
        </ModalDialog>

      </div>
    </div>
  )
}

TranslationOrderDialog.propTypes = {
  intl: PropTypes.object.isRequired,
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  entityId: PropTypes.string.isRequired,
  sourceLanguage: PropTypes.string.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, { formName }) => ({
    entityId: getSelectedEntityId(state),
    sourceLanguage: getContentLanguageId(state),
    formName: formName
  }))
)(TranslationOrderDialog)

