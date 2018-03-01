/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { asLocalizable, withValidation, asDisableable, asComparable } from 'util/redux-form/HOC'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { isRequired } from 'util/redux-form/validators'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.DeliveryAddress.FormReceiver.Title',
    defaultMessage: 'Lomakkeen vastaanottaja'
  },
  tooltip: {
    id: 'Containers.Channels.DeliveryAddress.FormReceiver.Tooltip',
    defaultMessage: 'Lomakkeen vastaanottaja'
  },
  placeholder: {
    id: 'Containers.Channels.DeliveryAddress.FormReceiver.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const FormReceiver = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='formReceiver'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    maxLength={100}
    {...rest}
  />
)
FormReceiver.propTypes = {
  intl:  intlShape,
  validate: PropTypes.func,
  ui: PropTypes.object,
  updateUI: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asLocalizable,
  asDisableable
)(FormReceiver)
