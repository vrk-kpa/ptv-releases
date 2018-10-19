import React from 'react'
import { FaxNumber } from 'util/redux-form/sections'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import { compose } from 'redux'
import { defineMessages, injectIntl, FormattedMessage } from 'util/react-intl'
import FaxNumberTitle from './FaxNumberTitle'

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
    dataPaths: 'faxNumbers'
  }),
  asLocalizableSection('faxNumbers'),
  asCollection({
    name: 'fax',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    simple: true,
    stacked: true,
    dragAndDrop: true,
    Title: FaxNumberTitle
  }),
  asSection()
)(props => <FaxNumber {...props} />)

export default FaxNumberCollection
