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
import { RenderTextEditor, RenderTextEditorDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import CommonMessages from 'util/redux-form/messages'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'
import { withProps } from 'recompose'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isDraftEditorSizeExceeded } from 'util/redux-form/validators'

const EditorDescription = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='editorDescription'
    component={RenderTextEditor}
    label={formatMessage(CommonMessages.shortDescription)}
    editorHeight='eh80'
    {...rest}
  />
)
EditorDescription.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  withProps(props => ({
    limit: 500,
    charLimit: 500
  })),
  asComparable({ DisplayRender: RenderTextEditorDisplay }),
  withTranslationLock({ fieldType: 'textEditor' }),
  asDisableable,
  asLocalizable,
  withValidation({
    validate: isDraftEditorSizeExceeded
  }),
  withQualityAgent({
    key: ({ language = 'fi', fieldPrefix = '' }, { entityPrefix = '' }, { type }) => `${entityPrefix || fieldPrefix}Descriptions.${type}.${language}`,
    options: ({ type }) => ({
      type: type || 'Summary'
    })
  })
)(EditorDescription)
