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
import React from 'react'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'

const messages = defineMessages({
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  label: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Title',
    defaultMessage: 'Lomaketunnus'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FormIdentifier.Tooltip',
    defaultMessage: 'Lomaketunnus'
  }
})

const ChannelFormIdentifier = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='formIdentifier'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    maxLength={100}
    {...rest}
  />
)
ChannelFormIdentifier.propTypes = {
  intl:  intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asLocalizable,
  asDisableable
)(ChannelFormIdentifier)
