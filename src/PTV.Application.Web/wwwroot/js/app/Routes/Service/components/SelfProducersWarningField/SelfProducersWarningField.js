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
import { injectIntl, defineMessages } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import { Field } from 'redux-form/immutable'

const messages = defineMessages({
  organizationProducerWarning:{
    id: 'Service.Organization.ProducerWarning',
    defaultMessage: 'Olet tehnyt muutoksia vastuuorganisaatiolistaan, jolla voi olla vaikutuksia myös uottajatietoihin. Ole hyvä ja tarkista ne alempana lomakkeella.'
  }
})

const WarnnnigComponent = compose(injectIntl)((
  { intl: { formatMessage },
  input: { value } }
) => {
  return (value ? <Label
    labelText={formatMessage(messages.organizationProducerWarning)}
    labelPosition='top'
  /> : null)
})

const SelfProducersWarningField = (props) => {
  return (
    <div>
      <Field
        name='organizationProducerWarnning'
        component={WarnnnigComponent}
        {...props}
      />

    </div>
  )
}

export default compose(injectIntl)(SelfProducersWarningField)
