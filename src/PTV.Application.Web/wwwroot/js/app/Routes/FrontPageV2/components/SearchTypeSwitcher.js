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
import React, { PropTypes } from 'react'
import { Select } from 'sema-ui-components'
import { connect } from 'react-redux'
import { getSearchDomain } from 'Containers/Common/Selectors'
import { setSearchDomain } from 'Containers/Common/Actions/PageContainerActions'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'react-intl'
import InfoBox from 'appComponents/InfoBox'

const messages = defineMessages({
  title: {
    id: 'FrontPage.SearchContentSwitcher.Title',
    defaultMessage: 'Sisältotyyppi'
  },
  tooltip: {
    id: 'FrontPage.SearchContentSwitcher.Tooltip',
    defaultMessage: 'Sisältotyyppi'
  },
  servicesDescription: {
    id: 'FrontPage.SearchContentSwitcher.Services.description',
    defaultMessage: 'Oman organisaatiosi palvelut'
  },
  generalDescriptionsDescription: {
    id: 'FrontPage.SearchContentSwitcher.SeneralDescriptions.description',
    defaultMessage: 'Valmis peruskuvaus palvelusta tai kanavasta'
  },
  organizationsDescription: {
    id: 'FrontPage.SearchContentSwitcher.Organizations.description',
    defaultMessage: 'Organisaatioiden esittelysisallot'
  },
  servicesTitle: {
    id: 'FrontPage.SearchContentSwitcher.Services.Title',
    defaultMessage: 'Palvelukuvaus'
  },
  channelsTitle: {
    id: 'FrontPage.SearchContentSwitcher.Channels.Title',
    defaultMessage: 'Asiointikanava'
  },
  generalDescriptionsTitle: {
    id: 'FrontPage.SearchContentSwitcher.GeneralDescriptions.Title',
    defaultMessage: 'Pohjakuvaus'
  },
  organizationsTitle: {
    id: 'FrontPage.SearchContentSwitcher.Organizations.Title',
    defaultMessage: 'Organisaatiokuvaus'
  },
  electronicChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.ElectronicChannel.Title',
    defaultMessage: 'Verkkoasiointi'
  },
  webChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.WebChannel.Title',
    defaultMessage: 'Verkkosivu'
  },
  printableFormChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.PrintableFormChannel.Title',
    defaultMessage: 'Tulostettava lomake'
  },
  phoneChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.PhoneChannel.Title',
    defaultMessage: 'Puhelinasiointi'
  },
  serviceLocationChannelTitle: {
    id: 'FrontPage.SearchContentSwitcher.ServiceLocationChannel.Title',
    defaultMessage: 'Palvelupiste'
  }
})

const SearchDomainSwitcher = ({
  searchDomain,
  setSearchDomain,
  intl: { formatMessage },
  ...rest
}) => {
  const Services = () => (
    <div>
      <b>{formatMessage(messages.servicesTitle)}</b>
      <br />
      <p>{formatMessage(messages.servicesDescription)}</p>
    </div>
  )
  const EChannel = () => <div><b>{formatMessage(messages.electronicChannelTitle)}</b></div>
  const WebPage = () => <div><b>{formatMessage(messages.webChannelTitle)}</b></div>
  const PrintableForm = () => <div><b>{formatMessage(messages.printableFormChannelTitle)}</b></div>
  const Phone = () => <div><b>{formatMessage(messages.phoneChannelTitle)}</b></div>
  const ServiceLocation = () => <div><b>{formatMessage(messages.serviceLocationChannelTitle)}</b></div>
  const GeneralDescriptions = () => (
    <div>
      <b>{formatMessage(messages.generalDescriptionsTitle)}</b>
      <br />
      <p>{formatMessage(messages.generalDescriptionsDescription)}</p>
    </div>
  )
  const Organizations = () => (
    <div>
      <b>{formatMessage(messages.organizationsTitle)}</b>
      <br />
      <p>{formatMessage(messages.organizationsDescription)}</p>
    </div>
  )
  return (
    <div>
      <div className='form-field tooltip-label'>
        <strong>
          {formatMessage(messages.title)}
          <InfoBox tip={formatMessage(messages.tooltip)} />
        </strong>
      </div>
      <Select
        size='full'
        {...rest}
        optionRenderer={({ value, label }) => {
          switch (value) {
            case 'services': return <Services />
            case 'eChannel': return <EChannel />
            case 'webPage': return <WebPage />
            case 'printableForm': return <PrintableForm />
            case 'phone': return <Phone />
            case 'serviceLocation': return <ServiceLocation />
            case 'organizations': return <Organizations />
            case 'generalDescriptions': return <GeneralDescriptions />
            default: return <div>{label}</div>
          }
        }}
        options={[
          { value: 'services', label: formatMessage(messages.servicesTitle) },
          { value: 'eChannel', label: formatMessage(messages.electronicChannelTitle) },
          { value: 'webPage', label: formatMessage(messages.webChannelTitle) },
          { value: 'printableForm', label: formatMessage(messages.printableFormChannelTitle) },
          { value: 'phone', label: formatMessage(messages.phoneChannelTitle) },
          { value: 'serviceLocation', label: formatMessage(messages.serviceLocationChannelTitle) },
          { value: 'organizations', label: formatMessage(messages.organizationsTitle) },
          { value: 'generalDescriptions', label: formatMessage(messages.generalDescriptionsTitle) }
        ]}
        onChange={({ value }) => {
          setSearchDomain(value)
        }}
        value={searchDomain}
      />
    </div>
  )
}
SearchDomainSwitcher.propTypes = {
  searchDomain: PropTypes.string.isRequired,
  setSearchDomain: PropTypes.func.isRequired,
  intl: PropTypes.any.isRequired
}

export default compose(
  injectIntl,
  connect(
    state => ({
      searchDomain: getSearchDomain(state)
    }),
    {
      setSearchDomain
    }
  )
)(SearchDomainSwitcher)
