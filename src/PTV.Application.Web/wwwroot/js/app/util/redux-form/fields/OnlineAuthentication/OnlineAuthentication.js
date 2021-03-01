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
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { isRequired, translateValidation } from 'util/redux-form/validators'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationList.Title',
    defaultMessage: 'Vaatiiko verkkoasiointi tunnistautumisen'
  },
  authenticationListInfo: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationList.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta kyllä tai ei sen mukaan, vaatiiko verkkoasiointi tunnistautumisen vai ei. Tunnistautumisella tarkoitetaan mitä tahansa tapaa, jolla käyttäjä joutuu kirjautumaan tai tunnistautumaan verkkoasioinnissa.'
  },
  yesTitle: {
    id: 'Common.Components.Yes.Title',
    defaultMessage: 'Kyllä'
  },
  noTitle: {
    id: 'Common.Components.No.Title',
    defaultMessage: 'Ei'
  }
})

const OnlineAuthentication = ({
  intl: { formatMessage },
  placeholder,
  validate = isRequired,
  ...rest
}) => (
  <Field
    name='onlineAuthentication'
    component={RenderRadioButtonGroup}
    placeholder={placeholder}
    label={formatMessage(messages.label)}
    tooltip={formatMessage(messages.authenticationListInfo)}
    //validate={translateValidation(validate, formatMessage, messages.label)}
    {...rest}
    options={[
      { label: formatMessage(messages.noTitle), value: false },
      { label: formatMessage(messages.yesTitle), value: true }
    ]}
  />
)
OnlineAuthentication.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable,
  injectSelectPlaceholder()
)(OnlineAuthentication)
