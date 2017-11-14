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
import { RenderTextEditor, RenderTextEditorDisplay } from 'util/redux-form/renders'
import { asLocalizable, withValidation, asDisableable, asComparable } from 'util/redux-form/HOC'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { isDraftEditorSizeExceeded } from 'util/redux-form/validators'

const messages = defineMessages({
  conditionOfServiceUsageTitle: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Title',
    defaultMessage: 'Palvelun käytön edellytykset'
  },
  conditionOfServiceUsageServicePlaceholder: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Service.Placeholder',
    defaultMessage: 'Kuvaa lyhyesti, jos palvelun käyttöön liittyy ehtoja tai edellytyksiä, kirjoita ne tähän kenttään.'
  },
  conditionOfServiceUsageTooltip: {
    id: 'Containers.Services.AddService.Step1.ConditionOfServiceUsage.Tooltip',
    defaultMessage: 'Mikäli käyttäjä saa palvelua vain tietyin edellytyksin, kuvaa sanallisesti nämä edellytykset, ehdot ja kriteerit. Esim. "Palvelun piiriin kuuluvat vain työttömyysuhan alla olevat henkilöt". Myös mikäli palvelun onnistunut käyttö edellyttää esim. tiettyjen tietojen tai dokumenttien hankkimista, kuvaa se tähän: Esim. "Muutosverokorttia varten tarvitset tiedot vuoden alusta kertyneistä tuloista ja maksamistasi veroista, sekä arvion koko vuoden tuloista."'
  }
})

const ConditionOfServiceUsage = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='conditionOfServiceUsage'
    component={RenderTextEditor}
    label={formatMessage(messages.conditionOfServiceUsageTitle)}
    placeholder={formatMessage(messages.conditionOfServiceUsageServicePlaceholder)}
    tooltip={formatMessage(messages.conditionOfServiceUsageTooltip)}
    {...rest}
  />
)
ConditionOfServiceUsage.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  asDisableable,
  asLocalizable,
  withValidation({
    label: messages.conditionOfServiceUsageTitle,
    validate: isDraftEditorSizeExceeded
  }),
)(ConditionOfServiceUsage)
