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
import {
  RenderTextField,
  RenderTextFieldDisplay
} from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  invoicingAddressLabel: {
    id: 'Containers.Manage.Organizations.ElectronicInvoicingAddress.Address.Title',
    defaultMessage: 'Verkkolaskutusosoite (OVT-tunnus)'
  },
  invoicingAddressTooltip: {
    id: 'Containers.Manage.Organizations.ElectronicInvoicingAddress.Address.Tooltip',
    defaultMessage: 'tooltip'
  },
  invoicingAddressPlaceholder: {
    id: 'Containers.Manage.Organizations.ElectronicInvoicingAddress.Address.Placeholder',
    defaultMessage: 'esim. 003716333854'
  }
})

const ElectronicInvoicingAddress = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='electronicInvoicingAddress'
    component={RenderTextField}
    label={formatMessage(messages.invoicingAddressLabel)}
    placeholder={formatMessage(messages.invoicingAddressPlaceholder)}
    tooltip={formatMessage(messages.invoicingAddressTooltip)}
    maxLength={100}
    {...rest}
  />
)
ElectronicInvoicingAddress.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable
)(ElectronicInvoicingAddress)
