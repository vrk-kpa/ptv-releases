import React from 'react'
import { FaxNumber } from 'util/redux-form/sections'
import {
  asContainer,
  asCollection,
  asLocalizableSection,
  asSection
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.Common.FaxNumber.Title',
    defaultMessage: 'Faksinumero'
  },
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.FaxNumberCollection.AddButton.Title',
    defaultMessage: '+ Uusi faksinumero'
  }
})

const FaxNumberCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    simple: true
  }),
  asLocalizableSection('faxNumbers'),
  asCollection({
    name: 'fax',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  }),
  asSection()
)(props => <FaxNumber {...props} />)

export default FaxNumberCollection
