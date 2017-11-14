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
import { PhoneNumbers } from 'util/redux-form/sections'
import { compose } from 'redux'
import React from 'react'
import {
  asCollection,
  asContainer,
  asLocalizableSection
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  phoneNumberTooltip:{
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Kirjoita puhelinnumero kansainvälisessä muodossa. Kirjoita ensin maan suuntanumero ja anna kansallinen numero ilman alkunollaa. Jos numeroon ei voi soittaa ulkomailta, voit antaa numeron ilman ulkomaansuuntanumeroa. Voit antaa useita puhelinnumeroita "uusi puhelinnumero" -painikkeella.'
  },
  addBtnTitle: {
    id: 'Routes.Organization.PhoneNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi puhelinnumero'
  }
})

const PhoneNumberCollection = compose(
  injectIntl,
  asContainer({ title: messages.title, simple: true }),
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumber',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(({
  intl: { formatMessage },
  ...rest
}) => <PhoneNumbers
  phoneNumberProps={{
    tooltip: formatMessage(messages.phoneNumberTooltip)
  }}
  {...rest}
  />)

export default PhoneNumberCollection
