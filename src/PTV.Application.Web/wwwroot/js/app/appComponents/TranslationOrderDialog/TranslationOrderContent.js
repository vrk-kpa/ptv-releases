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
import TranslastionOrderForm from './components/TranslastionOrderForm'
import TranslationOrderTable from './components/TranslationOrderTable'
import { defineMessages, injectIntl } from 'react-intl'
import { ReduxAccordion } from 'appComponents/Accordion'
import withNotification from 'util/redux-form/HOC/withNotification'
import { formTypesEnum } from 'enums'

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

const TranslationOrderDialogContent = ({
  intl: { formatMessage }
}) => {
  return (
    <ReduxAccordion light reduxKey='translationOrderDialogActiveIndex'>
      <ReduxAccordion.Title
        title={formatMessage(messages.orderFormTitle)}
        tooltip={formatMessage(messages.orderFormTooltip)}
        hideTooltipOnScroll
        validate={false} />
      <ReduxAccordion.Content>
        <TranslastionOrderForm />
      </ReduxAccordion.Content>
      <ReduxAccordion.Title
        title={formatMessage(messages.orderListTitle)}
        tooltip={formatMessage(messages.orderListTooltip)}
        hideTooltipOnScroll
        validate={false} />
      <ReduxAccordion.Content>
        <TranslationOrderTable />
      </ReduxAccordion.Content>
    </ReduxAccordion>
  )
}

TranslationOrderDialogContent.propTypes = {
  intl: PropTypes.object.isRequired
}

export default compose(
  injectIntl,
  withNotification({
    formName: formTypesEnum.TRANSLATIONORDERFORM
  })
)(TranslationOrderDialogContent)

