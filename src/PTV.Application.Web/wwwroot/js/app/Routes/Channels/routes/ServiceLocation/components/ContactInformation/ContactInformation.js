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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import asContainer from 'util/redux-form/HOC/asContainer'
import { connect } from 'react-redux'
import {
  PhoneNumberCollection,
  EmailCollection
} from 'util/redux-form/sections'
import FaxNumberCollection from 'Routes/Channels/routes/ServiceLocation/components/FaxNumberCollection'
import { AttachmentCollectionSL } from 'appComponents/AttachmentCollection'
import PostalAddressCollection from 'appComponents/PostalAddressCollection/PostalAddressCollection'
import { getDefaultPhoneNumber } from '../../../../selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import commonMessages from '../../../../messages'
import { Map } from 'immutable'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.Common.ShowContacts',
    defaultMessage: 'Lisää yhteystiedot'
  },
  webPageTitle: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Title',
    defaultMessage: 'Verkkosivun nimi'
  },
  webPageTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  webPagePlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  urlLabel: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Title',
    defaultMessage: 'Verkko-osoite'
  },
  urlTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy.'
  },
  urlPlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  emailTooltip: {
    id: 'ServiceLocation.Email.Tooltip',
    defaultMessage: 'Service location email tooltip placeholder'
  }
})

const ContactInformation = ({ defaultPhoneNumberItem, intl: { formatMessage } }) => {
  const defaultAddress = Map({ streetType: 'Street' })
  return (
    <div>
      <EmailCollection
        label={formatMessage(commonMessages.emailTitle)}
        placeholder={formatMessage(commonMessages.emailPlaceholder)}
        tooltip={null}
        emailProps={{
          tooltip: formatMessage(messages.emailTooltip)
        }}
        nested
      />
      <FaxNumberCollection nested />
      <PhoneNumberCollection defaultItem={defaultPhoneNumberItem} nested />
      <AttachmentCollectionSL
        nested
        nameProps={
          {
            label: formatMessage(messages.webPageTitle),
            tooltip: formatMessage(messages.webPageTooltip),
            placeholder: formatMessage(messages.webPagePlaceholder)
          }
        }
        urlCheckerProps={
          {
            label: formatMessage(messages.urlLabel),
            tooltip: formatMessage(messages.urlTooltip),
            placeholder: formatMessage(messages.urlPlaceholder)
          }
        }
        collectionPrefix={'webPages'}
      />
      <PostalAddressCollection required nested defaultItem={defaultAddress} />
    </div>
  )
}

ContactInformation.propTypes = {
  intl: intlShape.isRequired,
  defaultPhoneNumberItem: ImmutablePropTypes.map
}

export default compose(
  injectIntl,
  asContainer({
    title: messages.title,
    withCollection: true,
    dataPaths: [
      'emails',
      'faxNumbers',
      'phoneNumbers',
      'webPages',
      'postalAddresses'
    ]
  }),
  connect(state => ({
    defaultPhoneNumberItem: getDefaultPhoneNumber(state)
  }))
)(ContactInformation)
