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
import PhoneNumbers from 'util/redux-form/sections/PhoneNumbers'
import PhoneNumbersWithType from 'util/redux-form/sections/PhoneNumbersWithType'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import asGroup from 'util/redux-form/HOC/asGroup'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import { compose } from 'redux'
import { withProps } from 'recompose'
import { injectIntl, FormattedMessage, defineMessages } from 'util/react-intl'
import CommonMessages from 'util/redux-form/messages'
import PhoneNumberTitle from './PhoneNumberTitle'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.PhoneNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi puhelinumero'
  },
  phoneNumberTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Kirjoita puhelinnumero kansainvälisessä muodossa. Kirjoita ensin maan suuntanumero ja anna kansallinen numero ilman alkunollaa. Jos numeroon ei voi soittaa ulkomailta, voit antaa numeron ilman ulkomaansuuntanumeroa. Voit antaa useita puhelinnumeroita "uusi puhelinnumero" -painikkeella.' // eslint-disable-line
  },
  phoneChannelFormPhoneNumberDisclaimer: {
    id : 'Util.ReduxForm.Sections.Phone.PhoneNumberCollection.DisclaimerMessage.Title',
    defaultMessage: 'Olet lisäämässä uuden puhelinnumeron. Voit lisätä uuden numeron vain, jos siinä annetaan palvelua puhelinkanavassa kuvattuun asiaan liittyen. Älä niputa samaan puhelinkanavaan kaikkia organisaation puhelinnumeroita, vaan tee jokaisesta asiasta oma puhelinkanava.', // eslint-disable-line
    description: {
      en: 'You are adding a new phone number. You can only add a new phone number, if the number provides service related to the issue described in the telephone channel. Do not group all your organisation\'s telephone numbers in the same telephone channel. Instead, make a separate telephone channel for every topic area.', // eslint-disable-line
      sv: 'Du håller på att lägga till ett nytt telefonnummer. Du kan lägga till ett nytt nummer endast om det tillhör funktionen i telefonkanalens beskrivning. Samla inte organisationens alla nummer under samma telefonkanal. Varje skild funktion ska ha sin egen telefonkanal.' // eslint-disable-line
    }
  },
  phoneChannelFormPhoneNumberTooltip: {
    id : 'Util.ReduxForm.Sections.Phone.PhoneNumberCollection.Tooltip',
    defaultMessage: 'Lisää puhelinkanavan puhelinnumero. Voit lisätä yhden tai useamman puhelinnumeron. HUOM. Voit lisätä useita numeroita ainoastaan siinä tapauksessa, että numeroissa annetaan palvelua samaan, puhelinkanavassa kuvattuun asiakokonaisuuteen tai palveluun liittyen. Yksittäiseen puhelinkanavaan ei tule niputtaa organisaation eri asioihin liittyviä palvelunumeroita, vaan kustakin asiasta tulee tehdä oma puhelinkanava.', // eslint-disable-line
    description: {
      en: 'Add the telephone channel\'s telephone number. You can add one or more telephone numbers. Please note: You can only add several numbers, if the numbers provide service related to the one and same topic area or service described in the telephone channel. Service telephone numbers related to various topic areas handled by your organisation should not be grouped under a single telephone channel. Instead, each topic area should have its own telephone channel.', // eslint-disable-line
      sv: 'Lägg till telefonkanalens telefonnummer. Du kan lägga till flera telefonnummer. OBS. Du kan lägga till nummer endast om numren tillhör samma funktion eller tjänst enligt telefonkanalens beskrivning. Samla inte alla servicenummer till organisationens olika funktioner under en telefonkanal. Varje skild funktion ska ha sin egen telefonkanal.' // eslint-disable-line
    }
  }
})

const PhoneNumberCollection = compose(
  injectIntl,
  asContainer({
    title: CommonMessages.phoneNumbers,
    dataPaths: 'phoneNumbers'
  }),
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumbers',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: PhoneNumberTitle
  })
)(PhoneNumbers)

export const PhoneNumberCollectionPhoneChannel = compose(
  injectIntl,
  withErrorDisplay('phoneNumbers'),
  asGroup({
    title: CommonMessages.phoneNumbers,
    tooltip: <FormattedMessage {...messages.phoneChannelFormPhoneNumberTooltip} />
  }),
  asLocalizableSection('phoneNumbers'),
  withProps(props => ({
    withType: true,
    disclaimerMessage: <FormattedMessage {...messages.phoneChannelFormPhoneNumberDisclaimer} />
  })),
  asCollection({
    name: 'phoneNumber',
    addBtnTitle: (
      <FormattedMessage {...messages.addBtnTitle} />
    ),
    pluralName: 'formFiles',
    simple: true,
    stacked: true,
    dragAndDrop: true,
    Title: PhoneNumberTitle
  })
)(PhoneNumbersWithType)

export const PhoneNumberCollectionOrganization = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    dataPaths: 'phoneNumbers'
  }),
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumber',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: PhoneNumberTitle
  })
)(({ intl: { formatMessage }, ...rest }) => (
  <PhoneNumbers
    phoneNumberProps={{
      tooltip: formatMessage(messages.phoneNumberTooltip)
    }}
    {...rest}
  />
))

export default PhoneNumberCollection
