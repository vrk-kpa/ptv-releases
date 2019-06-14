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
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { withProps } from 'recompose'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isDraftEditorSizeExceeded } from 'util/redux-form/validators'
import CommonMessages from 'util/redux-form/messages'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'
import { getEditorStateValue } from 'util/redux-form/HOC/withQualityAgent/defaultFunctions'

const messages = defineMessages({
  // label: {
  //   id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Title',
  //   defaultMessage: 'Lyhyt kuvaus'
  // },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä verkkoasiointikanavan keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan.' // eslint-disable-line max-len
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  }
})

const Summary = ({
  intl: { formatMessage },
  ...rest
}) => {
  return (
    <Field
      name='shortDescription'
      component={RenderTextEditor}
      label={formatMessage(CommonMessages.shortDescription)}
      placeholder={formatMessage(messages.placeholder)}
      tooltip={formatMessage(messages.tooltip)}
      hideBlockStyleButtons
      styleAs='textarea'
      {...rest}
    />
  )
}
Summary.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withProps(props => ({
    limit: 150,
    charLimit: 150
  })),
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  withTranslationLock({ fieldType: 'textEditor' }),
  asDisableable,
  asLocalizable,
  withValidation({
    label: CommonMessages.shortDescription,
    validate: isDraftEditorSizeExceeded
  }),
  withQualityAgent({
    key: ({ language = 'fi' }, { entityPrefix = '' }, { type }) => `${entityPrefix}Descriptions.${type}.${language}`,
    property: ({ qualityAgentCompare }, { entityPrefix }) =>
      !qualityAgentCompare && `${entityPrefix}Descriptions` || null,
    options: {
      type: 'Summary'
    },
    getValue: getEditorStateValue
  })
)(Summary)
