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
import PropTypes from 'prop-types'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { isEmail } from 'util/redux-form/validators'
import { asDisableable, asComparable, withValidation } from 'util/redux-form/HOC'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})

const Email = ({
  intl: { formatMessage },
  emailProps = {},
  ...rest
}) => (
  <div className='form-row'>
    <Field
      name='email'
      component={RenderTextField}
      tooltip={formatMessage(messages.tooltip)}
      placeholder={formatMessage(messages.placeholder)}
      label={formatMessage(messages.label)}
      size='w320'
      maxLength={100}
      {...emailProps}
      {...rest}
    />
  </div>
)
Email.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  emailProps: PropTypes.object
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  withValidation({
    label: messages.label,
    validate: isEmail
  })
)(Email)
