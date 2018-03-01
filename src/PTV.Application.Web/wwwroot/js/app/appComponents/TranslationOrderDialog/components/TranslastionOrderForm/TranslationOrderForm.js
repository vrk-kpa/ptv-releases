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
import { connect } from 'react-redux'
import { reduxForm } from 'redux-form/immutable'
import { defineMessages, injectIntl } from 'react-intl'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { EntitySchemas } from 'schemas'
import { formTypesEnum, formApiPaths } from 'enums'
import styles from './styles.scss'
import TranslationSourceLanguage from '../TranslationSourceLanguage'
import TranslationRequiredLanguage from '../TranslationRequiredLanguage'
import { SenderName, SenderEmail } from 'util/redux-form/fields'
import { TranslationAdditionalInformation } from 'appComponents/TranslationOrderDialog/components'
import { Button, Label, Spinner } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { injectFormName } from 'util/redux-form/HOC'
import { translationOrderFormTransformer } from '../../Transformers'
import { getTranslationOrder, isRequiredLanguageSelected } from '../../selectors'
import withState from 'util/withState'

const translationOrderFormMessages = defineMessages({
  additionalInformationLabel: {
    id: 'Components.TranslationOrderDialog.TranslationOrderForm.AdditinalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  additionalInformationPlaceholder: {
    id: 'Components.TranslationOrderDialog.TranslationOrderForm.AdditinalInformation.Placeholder',
    defaultMessage: 'Voit tarvittaessa ohjeistaa kääntäjiä'
  },
  sendButton: {
    id: 'CurrentIssues.Buttons.SendButton',
    defaultMessage: 'Tilaa'
  },
  clearButton: {
    id: 'CurrentIssues.Buttons.Cancel',
    defaultMessage: 'Peruuta'
  },
  translationNote: {
    id: 'appComponents.TranslationOrderDialog.Components.TranslationOrderForm.TranslationNote.Title',
    defaultMessage: 'Käännöksen tekee Väestörekisterikeskuksen käyttämä käännöstoimisto Lingsoft'
  },
  translationCompanyEmail: {
    id: 'appComponents.TranslationOrderDialog.Components.TranslationOrderForm.TranslationCompanyEmail.Title',
    defaultMessage: 'ptv@lingsoft.fi'
  }
})

const TranslationOrderForm = ({
  handleSubmit,
  form,
  intl: { formatMessage },
  dispatch,
  formName,
  navigateToOrderList,
  submitting,
  canBeSend,
  reorderLang
}) => {
  const handleOnClear = () => {
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

  return (
    <form onSubmit={handleSubmit}>
      <div>
        <div className={styles.translationRow}>
          <div className='row'>

            <div className='col-lg-10 mb-3 mb-md-0'>
              <div className='row'>

                <div className='col-md-10 mb-3 mb-lg-0'>
                  <TranslationSourceLanguage required />
                </div>
                <div className='col-md-10'>
                  <TranslationRequiredLanguage
                    selectedLang={reorderLang}
                    required />
                </div>

              </div>
            </div>

            <div className='col-lg-14'>
              <div className='row'>
                <div className='col-lg-20'>
                  <TranslationAdditionalInformation
                    label={formatMessage(translationOrderFormMessages.additionalInformationLabel)}
                    placeholder={formatMessage(translationOrderFormMessages.additionalInformationPlaceholder)}
                    multiline
                    rows={4}
                    counter
                    maxLength={2500} />
                </div>
              </div>
            </div>

          </div>
        </div>

        <div className={styles.translationRow}>
          <div className='row'>

            <div className='col-lg-14 push-lg-10 mb-3 mb-md-0'>
              <div className='row'>
                <div className='col-md-10 mb-3 mb-lg-0'>
                  <SenderName required />
                </div>
                <div className='col-md-10'>
                  <SenderEmail required />
                </div>
              </div>
            </div>

            <div className='col-lg-10 pull-lg-14'>
              <div className={styles.buttonGroup}>
                <Button
                  small
                  type='submit'
                  disabled={submitting || !(canBeSend || reorderLang)}
                  children={submitting
                    ? <Spinner />
                    : formatMessage(translationOrderFormMessages.sendButton)}
                />
                <Button
                  link
                  onClick={handleOnClear}
                  children={formatMessage(translationOrderFormMessages.clearButton)}
                />
              </div>
            </div>

          </div>
        </div>

        <div className={styles.translationNote}>
          <Label infoLabel>
            <span>{formatMessage(translationOrderFormMessages.translationNote)}</span>
            <a href='mailTo:ptv@lingsoft.fi'>{formatMessage(translationOrderFormMessages.translationCompanyEmail)}</a>
          </Label>
        </div>

      </div>
    </form>
  )
}
TranslationOrderForm.propTypes = {
  handleSubmit: PropTypes.func.isRequired,
  form: PropTypes.string.isRequired,
  intl: PropTypes.object.isRequired,
  dispatch: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
  navigateToOrderList: PropTypes.func.isRequired,
  submitting: PropTypes.bool.isRequired,
  canBeSend: PropTypes.bool.isRequired,
  reorderLang: PropTypes.any
}

const onSubmit = async (...args) => {
  const { formName } = args[2] // ...args === (formValues, dispatch, ownProps)
  const submit = handleOnSubmit({
    url: formApiPaths[formName] + '/SendEntityToTranslation',
    schema: EntitySchemas.TRANSLATION_ORDER_RESULT,
    transformers: [
      translationOrderFormTransformer
    ]
  })
  return await submit(...args)
}

const onSubmitSuccess = (result, dispatch, props) => {
  handleOnSubmitSuccess(result, dispatch, formTypesEnum.TRANSLATIONORDERFORM, getTranslationOrder)
  props.openResultAccordion()
  props.clearReorder()
}

const onSubmitFail = (...args) => console.log(args)

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: 'translationOrderDialogReorderLanguage',
    initialState: {
      reorderLang: null
    }
  }),
  connect((state, { formName }) => ({
    initialValues: getTranslationOrder(state),
    formName: formName,
    canBeSend: isRequiredLanguageSelected(state)
  }), {
    openResultAccordion: () => mergeInUIState({
      key: 'translationOrderDialogActiveIndex',
      value: {
        activeIndex: 1
      }
    }),
    clearReorder: () => mergeInUIState({
      key: 'translationOrderDialogReorderLanguage',
      value: {
        reorderLang: null
      }
    })
  }),
  reduxForm({
    form: formTypesEnum.TRANSLATIONORDERFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess
  })
)(TranslationOrderForm)
