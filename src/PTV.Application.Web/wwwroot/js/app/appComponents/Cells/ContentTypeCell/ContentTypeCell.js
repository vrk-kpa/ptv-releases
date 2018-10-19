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
import cx from 'classnames'
import styles from './styles.scss'
import { defineMessages } from 'util/react-intl'
import { camelCase } from 'lodash'

const messages = defineMessages({
  serviceTitle: {
    id: 'FrontPage.SearchContentSwitcher.Services.Title',
    defaultMessage: 'Palvelukuvaus'
  },
  channelTitle: {
    id: 'FrontPage.SearchContentSwitcher.Channels.Title',
    defaultMessage: 'Asiointikanava'
  },
  generalDescriptionTitle: {
    id: 'FrontPage.SearchContentSwitcher.GeneralDescriptions.Title',
    defaultMessage: 'Pohjakuvaus'
  },
  organizationTitle: {
    id: 'FrontPage.SearchContentSwitcher.Organizations.Title',
    defaultMessage: 'Organisaatiokuvaus'
  },
  eChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.ElectronicChannel.Title',
    defaultMessage: 'Verkkoasiointi'
  },
  webPageTitle: {
    id: 'FrontPage.SearchContentSwitcher.WebChannel.Title',
    defaultMessage: 'Verkkosivu'
  },
  printableFormTitle: {
    id: 'FrontPage.SearchContentSwitcher.PrintableFormChannel.Title',
    defaultMessage: 'Tulostettava lomake'
  },
  phoneTitle: {
    id: 'FrontPage.SearchContentSwitcher.PhoneChannel.Title',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocationTitle: {
    id: 'FrontPage.SearchContentSwitcher.ServiceLocationChannel.Title',
    defaultMessage: 'Palvelupiste'
  },
  serviceCollectionTitle: {
    id: 'FrontPage.SearchContentSwitcher.ServiceCollection.Title',
    defaultMessage: 'Palvelukokonaisuus'
  }
})
const ContentTypeCell = ({
  entityType,
  subEntityType,
  componentClass,
  inline,
  formatMessage
}) => {
  const contentTypeClass = cx(
    componentClass,
    'cell',
    {
      [styles.inline]: inline
    }
  )
  return <div className={contentTypeClass}>
    <div className={styles.main}>{formatMessage(messages[`${camelCase(entityType)}Title`])}</div>
    <div className={styles.sub}>{formatMessage(messages[`${camelCase(subEntityType)}Title`])}</div>
  </div>
}
ContentTypeCell.propTypes = {
  entityType: PropTypes.string,
  subEntityType: PropTypes.string,
  componentClass: PropTypes.string,
  inline: PropTypes.bool,
  formatMessage: PropTypes.func
}
export default ContentTypeCell
