/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { Field } from 'redux-form/immutable'
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationSign.Title',
    defaultMessage: 'Vaatiiko verkkoasiointi sähköisen allekirjoituksen'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.AuthenticationSign.Tooltip',
    defaultMessage: 'Osa verkkoasioinneista pitää allekirjoittaa sähköisesti ennen lähettämistä. Valitse kyllä tai ei sen mukaan, vaatiiko verkkoasiointi sähköisen allekirjoituksen. Jos verkkoasiointi vaatii sähköisen allekirjoituksen, valitse alasvetovalikosta vaadittavien sähköisten allekirjoitusten lukumäärä. Useampi allekirjoitus vaaditaan joissakin asioinneissa, joissa useamman henkilön tulee vakuuttaa tiedot oikeiksi.'
  },
  yes: {
    id: 'Util.ReduxForm.Fields.AuthenticationSign.Yes.Title',
    defaultMessage: 'Kyllä'
  },
  no: {
    id: 'Util.ReduxForm.Fields.AuthenticationSign.No.Title',
    defaultMessage: 'Ei'
  }
})

const AuthenticationSign = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => (
  <Field
    name='isOnlineSign'
    component={RenderRadioButtonGroup}
    options={[
      { value: false, label: formatMessage(messages.no) },
      { value: true, label: formatMessage(messages.yes) }
    ]}
    label={formatMessage(messages.label)}
    tooltip={formatMessage(messages.tooltip)}
    {...rest}
  />
)
AuthenticationSign.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable
)(AuthenticationSign)
