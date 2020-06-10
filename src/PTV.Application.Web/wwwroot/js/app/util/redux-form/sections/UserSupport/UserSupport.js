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
import asContainer from 'util/redux-form/HOC/asContainer'
import { connect } from 'react-redux'
import { compose } from 'redux'
import PhoneNumberCollection from 'util/redux-form/sections/PhoneNumberCollection'
import EmailCollection from 'util/redux-form/sections/EmailCollection'
import { messages } from './messages'
import { injectIntl, FormattedMessage } from 'util/react-intl'
import { getDefaultPhoneNumber } from 'Routes/Channels/selectors'

const UserSupport = ({
  intl: { formatMessage },
  defaultPhoneNumberItem,
  ...rest
}) => (
  <div>
    <PhoneNumberCollection
      nested
      dialCodeProps={{
        label: formatMessage(messages.dialCodeLabel),
        tooltip: formatMessage(messages.dialCodeTooltip),
        placeholder: formatMessage(messages.dialCodePlaceholder)
      }}
      phoneNumberProps={{
        label: formatMessage(messages.phoneNumberLabel),
        tooltip: formatMessage(messages.phoneNumberTooltip),
        placeholder: formatMessage(messages.phoneNumberPlaceholder)
      }}
      phoneNumberInfoProps={{
        label: formatMessage(messages.phoneNumberInfoLabel),
        tooltip: formatMessage(messages.phoneNumberInfoTooltip),
        placeholder: formatMessage(messages.phoneNumberInfoPlaceholder)
      }}
      chargeTypeProps={{
        label: formatMessage(messages.chargeTypeLabel),
        tooltip: formatMessage(messages.chargeTypeTooltip)
      }}
      phoneCostDescriptionProps={{
        label: formatMessage(messages.phoneCostDescriptionLabel),
        tooltip: formatMessage(messages.phoneCostDescriptionTooltip),
        placeholder: formatMessage(messages.phoneCostDescriptionPlaceholder)
      }}
      localServiceNumberLabel={formatMessage(messages.localServiceNumberLabel)}
      defaultItem={defaultPhoneNumberItem}
      {...rest}
    />
    <EmailCollection
      nested
      emailProps={{
        label: formatMessage(messages.emailLabel),
        tooltip: formatMessage(messages.emailTooltip),
        placeholder: formatMessage(messages.emailPlaceholder)
      }}
      {...rest}
    />
  </div>
)

export default compose(
  injectIntl,
  asContainer({
    title: messages.userSupportTitle,
    withCollection: true,
    dataPaths: ['phoneNumbers', 'emails']
  }),
  connect(state => ({
    defaultPhoneNumberItem: getDefaultPhoneNumber(state)
  }))
)(UserSupport)
