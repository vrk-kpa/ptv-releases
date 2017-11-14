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
import { connect } from 'react-redux'
import { Map } from 'immutable'
import { asContainer } from 'util/redux-form/HOC'
import { defineMessages, injectIntl, FormattedMessage } from 'react-intl'
import {
  EmailCollection,
  AttachmentCollection,
  PhoneNumberCollection,
  PostalAddressCollection,
  VisitingAddressCollection
} from 'Routes/Organization/components'
import { getDefaultPhoneNumber } from '../../selectors'

const messages = defineMessages({
  title: {
    id: 'Containers.Manage.Organizations.Manage.Step1.ShowContacts',
    defaultMessage: 'Lisää yhteystiedot'
  }
})
const defaultAddress = Map({ streetType: 'Street' })
const ContactInformationCollections = ({defaultPhoneNumberItem}) => (
  <div>
    <EmailCollection />
    <PhoneNumberCollection defaultItem={defaultPhoneNumberItem} />
    <AttachmentCollection />
    <PostalAddressCollection defaultItem={defaultAddress} required />
    <VisitingAddressCollection defaultItem={defaultAddress} required />
  </div>
)

export default compose(
  injectIntl,
  asContainer({ title: messages.title, withCollection: true }),
  connect(state => ({
    defaultPhoneNumberItem: getDefaultPhoneNumber(state)
  }))
)(ContactInformationCollections)
