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
import { Tabs, Tab } from 'sema-ui-components'
import TranslationOrderMain from '../TranslationOrderMain'
import TranslationOrderTable from '../TranslationOrderTable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { formTypesEnum, formApiPaths, formPaths } from 'enums'
import withState from 'util/withState'
import styles from '../../styles.scss'
import { mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import { translationOrderFormTransformer } from '../../Transformers'
import { getTranslationOrder } from '../../selectors'
import { reduxForm } from 'redux-form/immutable'
import { handleOnSubmit, handleOnSubmitSuccess } from 'util/redux-form/util'
import { EntitySchemas } from 'schemas'
import { withRouter } from 'react-router'
import { getSelectedEntityId } from 'selectors/entities/entities'

const messages = defineMessages({
  orderFormTitle: {
    id: 'Components.TranslationOrderDialog.OrderForm.Title',
    defaultMessage: 'Tee käännöstilaus'
  },
  orderFormTooltip: {
    id: 'Components.TranslationOrderDialog.OrderForm.Tooltip',
    defaultMessage: 'Käännöstilaus on sitova. Turhien kustannusten välttämiseksi, varmista, että lähdekielen tekstit ovat kunnossa, ennen kuin lähetät tekstit uudelleen käännettäväksi.'
  },
  orderListTitle: {
    id: 'Components.TranslationOrderDialog.OrderList.Title',
    defaultMessage: 'Aiemmat käännöstilaukset'
  },
  orderListTooltip: {
    id: 'Components.TranslationOrderDialog.OrderList.Tooltip',
    defaultMessage: 'Tässä osiossa voit katsella aiempia käännöstilauksia. Huomaathan, että käännöksen saapumiseen voi mennä useita viikkoja. Voit tarvittaessa olla sähköpostitse yhteydessä VRK:n sopimuskäännöstoimistoon, jonka tiedot täältä.'
  }
})

const TranslationOrderForm = ({
  activeIndex,
  updateUI,
  handleSubmit,
  intl: { formatMessage }
}) => {
  const handleOnChange = activeIndex => updateUI('activeIndex', activeIndex)
  return (
    <form onSubmit={handleSubmit}>
      <Tabs index={activeIndex} onChange={handleOnChange} simple indent='i30' className={styles.orderTabs}>
        <Tab label={formatMessage(messages.orderFormTitle)}>
          <div className={styles.orderTab}>
            <TranslationOrderMain />
          </div>
        </Tab>
        <Tab label={formatMessage(messages.orderListTitle)}>
          <div className={styles.orderTab}>
            <TranslationOrderTable />
          </div>
        </Tab>
      </Tabs>
    </form>
  )
}

TranslationOrderForm.propTypes = {
  intl: intlShape.isRequired,
  activeIndex: PropTypes.number,
  updateUI: PropTypes.func,
  handleSubmit: PropTypes.func.isRequired
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
  handleOnSubmitSuccess(result, dispatch, props, getTranslationOrder)
  props.openResultAccordion()
  props.clearReorder()
  const { selectedEntityId, formName, history } = props
  const currentId = result.id
  if (selectedEntityId !== currentId) {
    currentId && formName && history && history.replace(`${formPaths[formName]}/${currentId}`)
  }
}

const onSubmitFail = (...args) => console.log(args)

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: 'translationOrderDialogActiveIndex',
    initialState: {
      activeIndex: 0
    }
  }),
  connect(
    state => ({
      initialValues: getTranslationOrder(state),
      selectedEntityId: getSelectedEntityId(state)
    }),
    {
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
    }
  ),
  injectFormName,
  withRouter,
  reduxForm({
    form: formTypesEnum.TRANSLATIONORDERFORM,
    enableReinitialize: true,
    onSubmit,
    onSubmitFail,
    onSubmitSuccess
  })
)(TranslationOrderForm)

