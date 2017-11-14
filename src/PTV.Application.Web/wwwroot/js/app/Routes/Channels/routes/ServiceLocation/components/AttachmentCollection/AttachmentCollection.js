import React from 'react'
import {
  asContainer,
  asCollection,
  asLocalizableSection,
  asSection
} from 'util/redux-form/HOC'
import { compose } from 'redux'
import { SimpleAttachment } from 'util/redux-form/sections'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step2.WebPage.Section.Title',
    defaultMessage: 'Lisää verkkosivu'
  },
  addBtnTitle: {
    id : 'Routes.Channels.ServiceLocation.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi verkkosivu'
  },
  webPageTitle: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Title',
    defaultMessage: 'Verkkosivun nimi'
  },
  webPageTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  webPagePlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  urlLabel: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Title',
    defaultMessage: 'Verkko-osoite'
  },
  urlTooltip: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy.'
  },
  urlPlaceholder: {
    id: 'Containers.Channels.AddServiceLocationChannel.Step2.WebPage.Url.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  }
})

const AttachmentCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    simple: true
  }),
  asLocalizableSection('webPages'),
  asCollection({
    name: 'webPage',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  }),
  asSection()
)(({ intl: { formatMessage }, ...rest }) =>
  <SimpleAttachment
    nameMessages={
    {
      label: formatMessage(messages.webPageTitle),
      tooltip: formatMessage(messages.webPageTooltip),
      placeholder: formatMessage(messages.webPagePlaceholder)
    }
    }
    urlCheckerMessages={
    {
      label: formatMessage(messages.urlLabel),
      tooltip: formatMessage(messages.urlTooltip),
      placeholder: formatMessage(messages.urlPlaceholder)
    }
    }
    {...rest}
  />
)

export default AttachmentCollection
