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
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import ConnectionEmailPreview from './ConnectionEmailPreview'
import ConnectionNumberPreview from './ConnectionNumberPreview'
import ConnectionAddressPreview from './ConnectionAddressPreview'
import ConnectionWebPagePreview from './ConnectionWebPagePreview'
import {
  getEmailsCollection,
  getFaxNumbersCollection,
  getPhoneNumbersCollection,
  getPostalAddressesCollection,
  getWebPagesCollection
} from './selectors'
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import styles from './styles.scss'

const messages = defineMessages({
  contactDetailsTitle: {
    id: 'AppComponents.ConnectionDescriptionCell.ContactDetails.Title',
    defaultMessage: 'Liitokseen liittyvät yhteystiedot'
  },
  emailsTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  faxNumbersTitle: {
    id: 'Containers.Channels.Common.FaxNumber.Title',
    defaultMessage: 'Faksinumero'
  },
  phoneNumbersTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  postalAddressTitle: {
    id: 'Containers.Cahnnels.PostalAddress.Title',
    defaultMessage: 'Postiosoite (eri kuin käyntiosoite)'
  },
  urlAddressTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Title',
    defaultMessage: 'Verkko-osoite'
  }
})

const ConnectionContactDetails = ({
  emails,
  faxNumbers,
  phoneNumbers,
  webPages,
  postalAddresses,
  intl: { formatMessage }
}) => {
  const detailExist = emails.size || faxNumbers.size ||
    phoneNumbers.size || webPages.size || postalAddresses.size || false

  return (
    <div className={styles.previewBlock}>
      <Label className={styles.previewHeading} labelText={formatMessage(messages.contactDetailsTitle)} />
      <div className={styles.previewItem}>
        {detailExist && (
          <React.Fragment>
            {emails.size > 0 &&
              <div className={styles.previewBlock}>
                <Label labelText={formatMessage(messages.emailsTitle)} />
                <div>
                  {emails.map((email, index) => {
                    return <ConnectionEmailPreview key={email.get('id') || index} email={email} />
                  })}
                </div>
              </div>
            }
            {faxNumbers.size > 0 &&
              <div className={styles.previewBlock}>
                <Label labelText={formatMessage(messages.faxNumbersTitle)} />
                <div>
                  {faxNumbers.map((faxNumber, index) => {
                    return <ConnectionNumberPreview key={faxNumber.get('id') || index} number={faxNumber} />
                  })}
                </div>
              </div>
            }
            {phoneNumbers.size > 0 &&
              <div className={styles.previewBlock}>
                <Label labelText={formatMessage(messages.phoneNumbersTitle)} />
                <div>
                  {phoneNumbers.map((phoneNumber, index) => {
                    return <ConnectionNumberPreview key={phoneNumber.get('id') || index} number={phoneNumber} />
                  })}
                </div>
              </div>
            }
            {webPages.size > 0 &&
              <div className={styles.previewBlock}>
                <Label labelText={formatMessage(messages.urlAddressTitle)} />
                <div>
                  {webPages.map((webPage, index) => {
                    return <ConnectionWebPagePreview key={webPage.get('id') || index} webPage={webPage} />
                  })}
                </div>
              </div>
            }
            {postalAddresses.size > 0 &&
              <div className={styles.previewBlock}>
                <Label labelText={formatMessage(messages.postalAddressTitle)} />
                <div>
                  {postalAddresses.map((address, index) => {
                    return <ConnectionAddressPreview key={address.get('id') || index} address={address} />
                  })}
                </div>
              </div>
            }
          </React.Fragment>) || <NoDataLabel />
        }
      </div>
    </div>
  )
}

ConnectionContactDetails.propTypes = {
  emails: PropTypes.any,
  faxNumbers: PropTypes.any,
  phoneNumbers: PropTypes.any,
  webPages: PropTypes.any,
  postalAddresses: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  withLanguageKey,
  connect((state, { formName, field, languageKey }) => ({
    emails: getEmailsCollection(state, { formName, field, languageKey }),
    faxNumbers: getFaxNumbersCollection(state, { formName, field, languageKey }),
    phoneNumbers: getPhoneNumbersCollection(state, { formName, field, languageKey }),
    postalAddresses: getPostalAddressesCollection(state, { formName, field, languageKey }),
    webPages: getWebPagesCollection(state, { formName, field, languageKey })
  }))
)(ConnectionContactDetails)
