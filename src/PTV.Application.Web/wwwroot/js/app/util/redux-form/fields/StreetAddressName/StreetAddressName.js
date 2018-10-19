import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import CommonMessages from 'util/redux-form/messages'
import withPath from 'util/redux-form/HOC/withPath'
import { getDefaultValue } from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { withProps } from 'recompose'

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
  defaultValue,
  ...rest
}) => (<Field
  name='street'
  component={RenderTextField}
  label={formatMessage(CommonMessages.street)}
  placeholder={formatMessage(messages.placeholder)}
  tooltip={formatMessage(messages[`${addressUseCase}Tooltip`])}
  maxLength={100}
  defaultValue={defaultValue}
  {...rest}
/>
)
StreetAddressName.propTypes = {
  intl: intlShape,
  defaultValue: PropTypes.string,
  addressUseCase: PropTypes.string
}

export default compose(
  injectIntl,
  withPath,
  injectFormName,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable,
  connect((state, { formName, path, language }) => ({
    defaultValue: getDefaultValue(state, { formName, path, language })
  })),
  withProps(props => ({
    triggerOnBlur: true
  })),
  withAccessibilityPrompt
)(StreetAddressName)
