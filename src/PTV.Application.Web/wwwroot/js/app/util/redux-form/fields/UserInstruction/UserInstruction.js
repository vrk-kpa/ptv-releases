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
import PropTypes from 'prop-types'
import { RenderTextEditor, RenderTextEditorDisplay } from 'util/redux-form/renders'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import withValidation from 'util/redux-form/HOC/withValidation'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { isDraftEditorSizeExceeded } from 'util/redux-form/validators'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'
import commonMessages from 'util/redux-form/messages'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'

const messages = defineMessages({
  serviceUserInstructionPlaceholder: {
    id: 'Containers.Services.AddService.Step1.ServiceUserInstruction.Placeholder',
    defaultMessage: 'Mikäli palvelun saamiseksi on toimittava tietyllä tavalla, kirjoita ohjeistus tähän kenttään.'
  },
  serviceUserInstructionTooltip: {
    id: 'Containers.Services.AddService.Step1.ServiceUserInstruction.Tooltip',
    defaultMessage: 'Jos palvelun käyttö tai etuuden hakeminen edellyttää tiettyä toimintajärjestystä tai tietynlaista toimintatapaa, kuvaa se tähän mahdollisimman tarkasti. Esimerkiksi jos jokin asiointikanava on ensisijainen, kerro se tässä. "Käytä ilmoittautumiseen ensisijaisesti verkkoasiointia."'
  }
})

const UserInstruction = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='userInstruction'
    component={RenderTextEditor}
    label={formatMessage(commonMessages.userInstruction)}
    placeholder={formatMessage(messages.serviceUserInstructionPlaceholder)}
    tooltip={formatMessage(messages.serviceUserInstructionTooltip)}
    {...rest}
  />
)
UserInstruction.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  withTranslationLock({ fieldType: 'textEditor' }),
  asDisableable,
  asLocalizable,
  withValidation({
    label: messages.serviceUserInstructionTitle,
    validate: isDraftEditorSizeExceeded
  }),
  withQualityAgent({
    key: ({ language = 'fi' }, { entityPrefix = '' }, { type }) => `${entityPrefix}Descriptions.${type}.${language}`,
    options: {
      type: 'UserInstruction'
    }
  })
)(UserInstruction)
