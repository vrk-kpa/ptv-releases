import React from 'react'
import PropTypes from 'prop-types'
import {
  RenderTextField,
  RenderTextFieldDisplay
} from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label : {
    id: 'Containers.Channels.Address.POBox.Title',
    defaultMessage: 'Postilokero-osoite'
  },
  placeholder : {
    id: 'Containers.Channels.Address.POBox.Placeholder',
    defaultMessage: 'esim. PL-205'
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

const POBox = ({
  intl: { formatMessage },
  addressUseCase,
  ...rest
}) => (
  <Field
    name='poBox'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages[`${addressUseCase}Tooltip`])}
    maxLength={30}
    {...rest}
  />
)
POBox.propTypes = {
  intl: intlShape,
  addressUseCase: PropTypes.string
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable
)(POBox)
