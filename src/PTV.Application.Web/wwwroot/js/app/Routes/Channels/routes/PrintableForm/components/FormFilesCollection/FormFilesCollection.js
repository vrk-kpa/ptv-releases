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
import { compose } from 'redux'
import { FormFiles } from 'util/redux-form/sections'
import {
  asGroup,
  asCollection,
  asLocalizableSection
} from 'util/redux-form/HOC'
import withErrorDisplay from 'util/redux-form/HOC/withErrorDisplay'
import { injectIntl, defineMessages, FormattedMessage } from 'react-intl'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  addBtnTitle: {
    id : 'Routes.Channels.PrintableForm.FormFilesCollection.AddButton.Title',
    defaultMessage: '+ Uusi lomaketiedosto'
  }
})

const FormFilesCollection = compose(
  withErrorDisplay('formFiles'),
  asGroup({ title: CommonMessages.formFiles }),
  asLocalizableSection('formFiles'),
  asCollection({
    name: 'formFile',
    pluralName: 'formFiles',
    simple: true,
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />
  })
)(props => <FormFiles {...props} />)

export default compose(
  injectIntl
)(FormFilesCollection)
