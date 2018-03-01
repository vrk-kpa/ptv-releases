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
import { Attachment } from 'util/redux-form/sections'
import { compose } from 'redux'
import {
  asCollection,
  asContainer,
  asLocalizableSection
} from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.Step2.WebPageSection.Title',
    defaultMessage: 'Lisää verkkosivu'
  },
  urlCheckerTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step2.WebPage.Url.Tooltip',
    defaultMessage: 'Anna verkkosivun osoite muodossa www.suomi.fi. Kun klikkaat Lisää uusi -painiketta, osoitteen alkuun lisätään http:// automaattisesti. Voit lisätä useita verkkosivuja yksi kerrallaan Lisää uusi -painikkeella.'
  },
  nameTitle: {
    id: 'Containers.Manage.Organizations.Manage.Step2.WebPage.Name.Title',
    defaultMessage: 'Verkkosivun nimi'
  },
  nameTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step2.WebPage.Name.Tooltip',
    defaultMessage: 'Anna palvelupisteen verkkosivuille havainnollinen nimi.'
  },
  namePlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step2.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  addBtnTitle: {
    id: 'Routes.Organization.AttachmentCollection.AddButton.Title',
    defaultMessage: '+ Uusi verkkosivu'
  }
})

const AttachmentCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    dataPaths: 'webPages'
  }),
  asLocalizableSection('webPages'),
  asCollection({
    name: 'webPage',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(({
  intl: { formatMessage },
  ...rest
}) => <Attachment
  hideDescription
  urlCheckerProps={{
    tooltip: formatMessage(messages.urlCheckerTooltip)
  }}
  nameProps={{
    title: formatMessage(messages.nameTitle),
    placeholder: formatMessage(messages.namePlaceholder),
    tooltip: formatMessage(messages.nameTooltip)
  }}
  {...rest}
  />)

export default AttachmentCollection
