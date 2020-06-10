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
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { localizeList } from 'appComponents/Localize'
import { RenderCheckBoxList } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { getRequiredLanguages, getRequiredLanguage } from '../TranslationOrderTable/selectors'

const messages = defineMessages({
  title: {
    id: 'Components.TranslationOrderDialog.RequiredLanguage.Title',
    defaultMessage: 'Kohdekieli'
  },
  additionalReorderText: {
    id: 'Components.TranslationOrderDialog.RequiredLanguage.AdditionalReorder.Text',
    defaultMessage: '(Re-order)'
  },
  additionalReorderTooltip: {
    id: 'Components.TranslationOrderDialog.RequiredLanguage.AdditionalReorder.Tooltip',
    defaultMessage: 'Re-order'
  }
})

const TranslationRequiredLanguage = ({
  intl: { formatMessage },
  languagesOptions,
  ...rest
}) => (
  <Field
    name='requiredLanguages'
    label={formatMessage(messages.title)}
    component={RenderCheckBoxList}
    options={languagesOptions}
    additionalText={formatMessage(messages.additionalReorderText)}
    additionalTooltip={formatMessage(messages.additionalReorderTooltip)}
    {...rest}
  />
)

TranslationRequiredLanguage.propTypes = {
  intl: intlShape.isRequired,
  languagesOptions: PropTypes.array
}

export default compose(
  injectIntl,
  asDisableable,
  connect(
    (state, { selectedLang }) => ({
      languagesOptions: selectedLang ? getRequiredLanguage(state, { selectedLang }) : getRequiredLanguages(state)
    })
  ),
  localizeList({
    input:'languagesOptions',
    idAttribute: 'id',
    nameAttribute: 'label'
  }),
)(TranslationRequiredLanguage)
