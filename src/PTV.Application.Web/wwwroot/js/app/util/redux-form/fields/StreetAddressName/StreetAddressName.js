import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { asComparable, asDisableable, asLocalizable } from 'util/redux-form/HOC'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({

  placeholder : {
    id: 'Containers.Channels.Address.Street.Placeholder',
    defaultMessage: 'esim. Mannerheimintie'
  },
  visitingTooltip : {
    id: 'Containers.Channels.VistingAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  postalTooltip : {
    id: 'Containers.Channels.PostalAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  },
  deliveryTooltip : {
    id: 'Containers.Channels.DeliveryAddress.Street.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  }
})
const StreetAddressName = ({
  intl: { formatMessage },
  addressUseCase,
  ...rest
}) => (
  <Field
    name='street'
    component={RenderTextField}
    label={formatMessage(CommonMessages.street)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages[`${addressUseCase}Tooltip`])}
    maxLength={100}
    {...rest}
  />
)
StreetAddressName.propTypes = {
  intl: intlShape,
  addressUseCase: PropTypes.string
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable
)(StreetAddressName)
