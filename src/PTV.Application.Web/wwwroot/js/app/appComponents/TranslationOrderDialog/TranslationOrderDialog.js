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
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { ModalContent } from 'sema-ui-components'
import ModalDialog from 'appComponents/ModalDialog'
import { mergeInUIState } from 'reducers/ui'
import styles from './styles.scss'
import TranslationOrderForm from './components/TranslationOrderForm'
import { isSubmitting } from 'redux-form/immutable'
import QualityErrorWarningDialog from './components/QualityErrorWarningDialog'

const messages = defineMessages({
  dialogTitle: {
    id: 'Components.TranslationOrderDialog.Title',
    defaultMessage: 'Käännöstilaus'
  },
  dialogDescription: {
    id: 'Components.TranslationOrderDialog.Description',
    defaultMessage: 'Huom. Käännettävän tekstin pituus on usein pidempi kuin suomenkielinen versio.'
  },
  linkTitle: {
    id: 'Components.TranslationOrder.Link.Title',
    defaultMessage: 'Käännöstilaukset'
  }
})

const TranslationOrderDialog = ({
  intl: { formatMessage },
  dispatch,
  isLoading
}) => {
  const onRequestClose = (close) => {
    dispatch(mergeInUIState({
      key: `TranslationOrderDialog`,
      value: {
        isOpen: close
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

  return (
    <div>
      <ModalDialog
        name={`TranslationOrderDialog`}
        title={formatMessage(messages.dialogTitle)}
        className={styles.orderForm}
        style={{ content: { minHeight: '300px' } }}
        onRequestClose={() => onRequestClose(isLoading)}
      >
        <ModalContent>
          <p className={styles.content}>{formatMessage(messages.dialogDescription)}</p>
          <QualityErrorWarningDialog />
          <TranslationOrderForm />
        </ModalContent>
      </ModalDialog>
    </div>
  )
}

TranslationOrderDialog.propTypes = {
  intl: intlShape.isRequired,
  dispatch: PropTypes.func.isRequired,
  isLoading: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  connect((state) => {
    return {
      isLoading: isSubmitting('translationOrderForm')(state)
    }
  })
)(TranslationOrderDialog)

