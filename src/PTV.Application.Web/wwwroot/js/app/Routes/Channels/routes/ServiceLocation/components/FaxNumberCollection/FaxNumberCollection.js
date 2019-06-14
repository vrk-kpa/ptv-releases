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
