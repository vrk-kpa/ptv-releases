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
import { Email } from 'util/redux-form/fields'
import asContainer from 'util/redux-form/HOC/asContainer'
import asCollection from 'util/redux-form/HOC/asCollection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import { compose } from 'redux'
import { injectIntl, FormattedMessage, defineMessages } from 'util/react-intl'
import EmailTitle from './EmailTitle'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.EmailCollection.AddButton.Title',
    defaultMessage: '+ Uusi sähköpostiosoite'
  }
})

const EmailCollection = compose(
  injectIntl,
  asContainer({
    title: messages.title,
    withCollection: true,
    dataPaths: 'emails'
  }),
  asLocalizableSection('emails'),
  asCollection({
    name: 'emails',
    addBtnTitle: <FormattedMessage {...messages.addBtnTitle} />,
    stacked: true,
    dragAndDrop: true,
    Title: EmailTitle
  }),
  asSection()
)(props => <div className='collection-form-row'><Email {...props} /></div>)

export default EmailCollection
