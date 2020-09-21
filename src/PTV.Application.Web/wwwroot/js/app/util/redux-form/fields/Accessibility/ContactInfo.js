/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import React, { Fragment } from 'react'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import PropTypes from 'prop-types'
import { Label } from 'sema-ui-components'
import { getContactEmail, getContactPhone, getContactUrl } from './selectors'
import { compose } from 'redux'
import { connect } from 'react-redux'

const messages = defineMessages({
  title: {
    id: 'Accessibility.ContactInfo.Title',
    defaultMessage: 'Esteettömyystiedoista vastuussa olevan henkilön yhteystiedot',
    description: {
      en: 'Contact details of the person responsible for accessibility information'
    }
  }
})

const ContactInfo = ({
  contactEmail,
  contactPhone,
  contactUrl,
  intl: { formatMessage }
}) => {
  const isEmpty = !contactEmail && !contactPhone && !contactUrl
  if (isEmpty) return null
  return (
    <Fragment>
      <Label>
        {formatMessage(messages.title)}
      </Label>
      {contactEmail && <div>{contactEmail}</div>}
      {contactPhone && <div>{contactPhone}</div>}
      {contactUrl && <div>{contactUrl}</div>}
    </Fragment>
  )
}
ContactInfo.propTypes = {
  contactEmail: PropTypes.string,
  contactPhone: PropTypes.string,
  contactUrl: PropTypes.string,
  intl: intlShape
}

export default compose(
  connect(state => ({
    contactEmail: getContactEmail(state),
    contactPhone: getContactPhone(state),
    contactUrl: getContactUrl(state)
  })),
  injectIntl
)(ContactInfo)
