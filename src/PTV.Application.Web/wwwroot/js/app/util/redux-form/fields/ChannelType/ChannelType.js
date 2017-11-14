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
import { RenderSelect } from 'util/redux-form/renders'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'react-intl'
import { Popup } from 'appComponents'
import { Field } from 'redux-form/immutable'
import { PTVIcon } from 'Components'

const messages = defineMessages({
  title: {
    id: 'Util.ReduxForm.Fields.ChannelType.Title',
    defaultMessage: 'Kanavatyyppi'
  },
  tooltip: {
    id: 'Util.ReduxForm.Fields.ChannelType.Tooltip',
    defaultMessage: 'Kanavatyyppi'
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

const ChannelType = ({
  intl: { formatMessage },
  tooltip,
  ...rest
}) => {
  const labelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    />
  )

  return (
    <Field
      name='channelType'
      component={RenderSelect}
      size='full'
      label={formatMessage(messages.title)}
      labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
      {...rest}
      optionRenderer={({ label }) => <strong>{label}</strong>}
      options={[
        { value: 'eChannel', label: formatMessage(messages.electronicChannelTitle) },
        { value: 'webPage', label: formatMessage(messages.webChannelTitle) },
        { value: 'printableForm', label: formatMessage(messages.printableFormChannelTitle) },
        { value: 'phone', label: formatMessage(messages.phoneChannelTitle) },
        { value: 'serviceLocation', label: formatMessage(messages.serviceLocationChannelTitle) }
      ]}
    />
  )
}
ChannelType.propTypes = {
  intl: PropTypes.any.isRequired,
  tooltip: PropTypes.string
}

export default compose(
  injectIntl
)(ChannelType)
