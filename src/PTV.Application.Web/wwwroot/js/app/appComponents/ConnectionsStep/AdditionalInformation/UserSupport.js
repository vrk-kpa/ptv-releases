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
import { FormSection } from 'redux-form/immutable'
import { Map } from 'immutable'
import {
  Attachment,
  PhoneNumbers,
  FaxNumber,
  ConnectionEmail,
  AddressSwitch
} from 'util/redux-form/sections'
import asCollection from 'util/redux-form/HOC/asCollection'
import asContainer from 'util/redux-form/HOC/asContainer'
import asSection from 'util/redux-form/HOC/asSection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import { compose } from 'redux'
import { injectIntl, FormattedMessage, defineMessages } from 'util/react-intl'
import CommonMessages from 'util/redux-form/messages'
import PostalAddressTitle from 'appComponents/PostalAddressCollection/PostalAddressTitle'
import AttachmentTitle from 'appComponents/AttachmentCollection/AttachmentTitle'
import EmailTitle from 'util/redux-form/sections/EmailCollection/EmailTitle'
import FaxNumberTitle from 'Routes/Channels/routes/ServiceLocation/components/FaxNumberCollection/FaxNumberTitle'
import PhoneNumberTitle from 'util/redux-form/sections/PhoneNumberCollection/PhoneNumberTitle'
import { addressUseCasesEnum } from 'enums'
import styles from './styles.scss'

const messages = defineMessages({
  postalAddressTitle: {
    id: 'Containers.Cahnnels.PostalAddress.Title',
    defaultMessage: 'Postiosoite (eri kuin käyntiosoite)'
  },
  postalAddressAddButtonTitle: {
    id : 'Routes.Channels.ServiceLocation.PostalAddressCollection.AddButton.Title',
    defaultMessage: '+ Uusi postiosoite'
  },
  postalAddressAdditionalInformationTitle: {
    id: 'Containers.Channels.Address.AdditionalInformation.Title',
    defaultMessage: 'Osoitteen lisätiedot'
  },
  postalAddressAdditionalInformationPlaceholder: {
    id: 'Containers.Channels.Address.AdditionalInformation.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  faxNumbersTitle: {
    id: 'Containers.Channels.Common.FaxNumber.Title',
    defaultMessage: 'Faksinumero'
  },
  addFaxNumberButton: {
    id : 'Routes.Channels.ServiceLocation.FaxNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi faksinumero'
  },
  addPhoneNumberButton: {
    id : 'Util.ReduxForm.Sections.PhoneNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi puhelinumero'
  },
  webPageTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step2.WebPage.Section.Title',
    defaultMessage: 'Lisää verkkosivu'
  },
  addWebPageButton: {
    id : 'Routes.Channels.ServiceLocation.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi verkkosivu'
  },
  emailTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  addEmailButton: {
    id : 'Util.ReduxForm.Sections.EmailCollection.AddButton.Title',
    defaultMessage: '+ Uusi sähköpostiosoite'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  descriptionTitle: {
    id: 'Container.ChannelAttachmentContainer.DescriptionLabel',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Container.ChannelAttachmentContainer.Description.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const EmailCollection = compose(
  injectIntl,
  asContainer({
    title: messages.emailTitle,
    dataPaths: 'emails'
  }),
  withLanguageKey,
  asLocalizableSection('emails'),
  asCollection({
    name: 'emails',
    addBtnTitle: <FormattedMessage {...messages.addEmailButton} />,
    stacked: true,
    dragAndDrop: true,
    Title: EmailTitle
  }),
  asSection()
)(props => <ConnectionEmail isLocalized={false} {...props} />)

const FaxNumberCollection = compose(
  injectIntl,
  asContainer({
    title: messages.faxNumbersTitle,
    dataPaths: 'faxNumbers'
  }),
  withLanguageKey,
  asLocalizableSection('faxNumbers'),
  asCollection({
    name: 'faxNumbers',
    addBtnTitle: <FormattedMessage {...messages.addFaxNumberButton} />,
    stacked: true,
    dragAndDrop: true,
    Title: FaxNumberTitle
  }),
  asSection()
)(props => <FaxNumber {...props} />)
const PhoneNumberCollection = compose(
  injectIntl,
  asContainer({
    title: CommonMessages.phoneNumbers,
    dataPaths: 'phoneNumbers'
  }),
  withLanguageKey,
  asLocalizableSection('phoneNumbers'),
  asCollection({
    name: 'phoneNumbers',
    addBtnTitle: <FormattedMessage {...messages.addPhoneNumberButton} />,
    stacked: true,
    dragAndDrop: true,
    Title: PhoneNumberTitle
  })
)(props => <PhoneNumbers {...props} />)
const AttachmentCollection = compose(
  injectIntl,
  asContainer({
    title: messages.webPageTitle,
    dataPaths: 'webPages'
  }),
  withLanguageKey,
  asLocalizableSection('webPages'),
  asCollection({
    name: 'webPage',
    addBtnTitle: <FormattedMessage {...messages.addWebPageButton} />,
    stacked: true,
    dragAndDrop: true,
    Title: AttachmentTitle
  })
)(props => <Attachment {...props} />)
const PostalAddressCollection = compose(
  injectIntl,
  asContainer({
    title: messages.postalAddressTitle,
    dataPaths: 'postalAddresses'
  }),
  asCollection({
    name: 'postalAddress',
    pluralName: 'postalAddresses',
    addBtnTitle: <FormattedMessage {...messages.postalAddressAddButtonTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: PostalAddressTitle
  }),
  asSection()
)(
  ({
  intl: { formatMessage },
    ...props
  }) =>
    <AddressSwitch
      mapDisabled
      additionalInformationProps={{
        title: formatMessage(messages.postalAddressAdditionalInformationTitle),
        placeholder: formatMessage(messages.postalAddressAdditionalInformationPlaceholder)
      }}
      addressUseCase={addressUseCasesEnum.POSTAL}
      {...props}
    />
)

class UserSupport extends FormSection {
  render () {
    const { isReadOnly, intl: { formatMessage } } = this.props
    const defaultAddress = Map({ streetType: 'Street' })
    return (
      <div className={styles.userSupport}>
        <EmailCollection
          isReadOnly={isReadOnly}
          splitView
          nested
        />
        <FaxNumberCollection
          splitView
          nested
          isReadOnly={isReadOnly}
        />
        <PhoneNumberCollection
          splitView
          nested
          isReadOnly={isReadOnly}
        />
        <AttachmentCollection
          nested
          isReadOnly={isReadOnly}
          nameProps={{
            placeholder: formatMessage(messages.namePlaceholder)
          }}
          descriptionProps={{
            label: formatMessage(messages.descriptionTitle),
            placeholder: formatMessage(messages.descriptionPlaceholder)
          }}
        />
        <PostalAddressCollection
          isReadOnly={isReadOnly}
          defaultItem={defaultAddress}
          layoutClass='bigConnection'
          nested
        />
      </div>
    )
  }
}

export default compose(
  injectIntl
)(UserSupport)
