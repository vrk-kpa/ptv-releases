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
import { Map } from 'immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { getContentLanguageCode } from 'selectors/selections'
import ConnectionEmailPreview from './ConnectionEmailPreview'
import ConnectionNumberPreview from './ConnectionNumberPreview'
import ConnectionAddressPreview from './ConnectionAddressPreview'
import ConnectionWebPagePreview from './ConnectionWebPagePreview'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import {
  getFilteredList,
  languageMap
} from 'util/redux-form/submitFilters/Mappers'

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
    languageCode,
    intl: { formatMessage }
}) => {
  let emailCollection = emails.size && (emails.get(languageCode) || emails.first()) || Map()
  let phoneNumbersCollection = phoneNumbers.size && (phoneNumbers.get(languageCode) || phoneNumbers.first()) || Map()
  let faxNumbersCollection = faxNumbers.size && (faxNumbers.get(languageCode) || faxNumbers.first()) || Map()
  let webPageCollection = webPages.size && (webPages.get(languageCode) || webPages.first()) || Map()
  
  emailCollection = emailCollection.filter(email => email.get('email'))
  phoneNumbersCollection = phoneNumbersCollection.filter(number => number.get('phoneNumber'))
  faxNumbersCollection = faxNumbersCollection.filter(number => number.get('phoneNumber'))
  webPageCollection = webPageCollection.filter(web => web.get('name') || web.get('description') || web.get('urlAddress'))

  const detailExist = emailCollection.size || faxNumbersCollection.size || phoneNumbersCollection.size || webPageCollection.size || postalAddresses.size || false
  return (
    detailExist && <div>
      <strong>{formatMessage(messages.contactDetailsTitle)}:</strong>
      {emailCollection.size > 0 &&
        <div className={styles.previewBlock}>
          <Label labelText={formatMessage(messages.emailsTitle)} />
          <div>
            {emailCollection.map(email => {
              return <ConnectionEmailPreview email={email} />
            })}
          </div>
        </div>
      }
      {faxNumbersCollection.size > 0 &&
        <div className={styles.previewBlock}>
          <Label labelText={formatMessage(messages.faxNumbersTitle)} />
          <div>
            {faxNumbersCollection.map(faxNumber => {
              return <ConnectionNumberPreview number={faxNumber} />
            })}
          </div>
        </div>
      }
      {phoneNumbersCollection.size > 0 &&
        <div className={styles.previewBlock}>
          <Label labelText={formatMessage(messages.phoneNumbersTitle)} />
          <div>
            {phoneNumbersCollection.map(phoneNumber => {
              return <ConnectionNumberPreview number={phoneNumber} />
            })}
          </div>
        </div>
      }
      {webPageCollection.size > 0 &&
        <div className={styles.previewBlock}>
          <Label labelText={formatMessage(messages.urlAddressTitle)} />
          <div>
            {webPageCollection.map(webPage => {
              return <ConnectionWebPagePreview webPage={webPage} />
            })}
          </div>
        </div>
      }
      {postalAddresses.size > 0 &&
        <div className={styles.previewBlock}>
          <Label labelText={formatMessage(messages.postalAddressTitle)} />
          <div>
            {postalAddresses.map(address => {
              return <ConnectionAddressPreview address={address} />
            })}
          </div>
        </div>
      }
    </div>
  )
}

ConnectionContactDetails.propTypes = {
  emails: PropTypes.any,
  faxNumbers: PropTypes.any,
  phoneNumbers: PropTypes.any,
  webPages: PropTypes.any,
  postalAddresses: PropTypes.any,
  languageCode: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const contactDetails = ownProps.contactDetails.map((contactDetail, key) => {
      if (key === 'postalAddresses') return getFilteredList(contactDetail)
      return languageMap(getFilteredList)(contactDetail)
    })
    return {
      emails: contactDetails.get('emails') || Map(),
      faxNumbers: contactDetails.get('faxNumbers') || Map(),
      phoneNumbers: contactDetails.get('phoneNumbers') || Map(),
      webPages: contactDetails.get('webPages') || Map(),
      postalAddresses: contactDetails.get('postalAddresses') || Map(),
      languageCode: getContentLanguageCode(state, ownProps)
    }
  })
)(ConnectionContactDetails)
