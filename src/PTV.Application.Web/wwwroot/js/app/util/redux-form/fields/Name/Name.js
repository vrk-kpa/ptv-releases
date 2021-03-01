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
import { Field, change, formValues } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import CommonMessages from 'util/redux-form/messages'
import {
  RenderTextField,
  RenderTextFieldDisplay,
  RenderTextEditor,
  RenderTextEditorDisplay
} from 'util/redux-form/renders'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import withValidation from 'util/redux-form/HOC/withValidation'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'
import { withProps } from 'recompose'
import { isRequired, isDraftEditorSizeExceeded, isRequiredDraftJs } from 'util/redux-form/validators'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'
import { getTextValue, getEditorStateValue } from 'util/redux-form/HOC/withQualityAgent/defaultFunctions'
import withPath from 'util/redux-form/HOC/withPath'
import shortId from 'shortid'
import { getLanguageCode } from 'selectors/selections'
import { connect } from 'react-redux'

const messages = defineMessages({
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  }
})

const Name = ({
  intl: { formatMessage },
  title,
  tooltip,
  placeholder,
  component,
  ...rest
}) => {
  return (
    <Field
      name='name'
      component={component}
      label={title || formatMessage(CommonMessages.name)}
      placeholder={placeholder || formatMessage(messages.placeholder)}
      tooltip={tooltip || ''}
      counter
      {...rest}
    />
  )
}
Name.propTypes = {
  intl:  intlShape,
  title: PropTypes.string,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string,
  component: PropTypes.any
}

export const NameEditor = compose(
  injectIntl,
  withProps(props => ({
    limit: 100,
    charLimit: 100,
    styleAs: 'textinput',
    hideBlockStyleButtons: true,
    component: RenderTextEditor,
    editorHeight:'eh20'
  })),
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  withTranslationLock(),
  asDisableable,
  asLocalizable,
  withValidation({
    label: CommonMessages.name,
    validate: [isRequiredDraftJs, isDraftEditorSizeExceeded]
  }),
  withQualityAgent({
    key: ({ language = 'fi' }, { entityPrefix = '' }, { type }) => `${entityPrefix}Names.${type}.${language}`,
    // property: (_, { entityPrefix }) => `${entityPrefix}Names`,
    options: {
      type: 'Name'
    } // ,
    // getValue: getEditorStateValue
  })
)(Name)

export const NameInput = compose(
  injectIntl,
  connect((state, ownProps) => ({
    contentLanguage: getLanguageCode(state, ownProps)
  })),
  withProps(props => ({
    maxLength: props.maxLength ? props.maxLength : 100,
    autocomplete: 'ptv-name',
    component: RenderTextField
  })),
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  withTranslationLock(),
  asDisableable,
  asLocalizable,
  withValidation({
    label: CommonMessages.name,
    validate: isRequired
  }),
  withPath,
  formValues('order'),
  withQualityAgent({
    key: ({ language = 'fi', contentLanguage, order, collectionPrefix }, { entityPrefix = '' }, { type }) => collectionPrefix ? `${collectionPrefix}.${contentLanguage}.${order}` : `${entityPrefix}Names.${type}.${language}`,
    // property: ({ collectionPrefix }, { entityPrefix }) => !collectionPrefix && `${entityPrefix}Names` || null,
    options: {
      type: 'name'
    },
    // getValue: getTextValue,
    customAction: ({ formName, order, path, collectionPrefix }) => (store) => {
      !(typeof order !== 'undefined') && collectionPrefix && store.dispatch(change(formName, `${path}.order`, shortId.generate()))
    }
  })
)(Name)
