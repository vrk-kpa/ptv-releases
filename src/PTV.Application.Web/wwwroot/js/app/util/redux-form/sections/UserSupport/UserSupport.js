import React from 'react'
import { asContainer } from 'util/redux-form/HOC'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  PhoneNumberCollection,
  EmailCollection
} from 'util/redux-form/sections'
import { messages } from './messages'
import { injectIntl, FormattedMessage } from 'react-intl'
import { getDefaultPhoneNumber } from '../../../../Routes/Channels/selectors'

const UserSupport = ({
  intl: { formatMessage },
  defaultPhoneNumberItem,
  ...rest
}) => (
  <div>
    <PhoneNumberCollection
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
