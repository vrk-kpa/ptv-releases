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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { localizeList } from 'appComponents/Localize'
import { RenderMultiSelect, RenderMultiSelectDisplay } from 'util/redux-form/renders'
import { getTranslatableLanguages } from 'Routes/FrontPage/routes/Search/selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
// import { translateValidation } from 'util/redux-form/validators'
import { asComparable, asDisableable } from 'util/redux-form/HOC'

const messages = defineMessages({
  title: {
    id: 'FrontPage.SelectLanguage.Title',
    defaultMessage: 'Kielivalinta'
  },
  tooltip: {
    id: 'FrontPage.SelectLanguage.Tooltip',
    defaultMessage: 'Haku kohdentaa vain yhtä kieltä kohden. Jos haet palvelua ruotsin tai englannin kielisen nimen mukaan, valitse haluamasi kieli pudotusvalikosta.'
  }
})

const Language = ({
  intl: { formatMessage },
  options,
  // validate,
  ...rest
}) => (
  <Field
    name='languages'
    label={formatMessage(messages.title)}
    tooltip={formatMessage(messages.tooltip)}
    component={RenderMultiSelect}
    options={options}
    // validate={translateValidation(validate, formatMessage, messages.title)}
    {...rest}
  />
)
Language.propTypes = {
  // validate: PropTypes.func.isRequired,
  options: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

export default compose(
  injectIntl,
  asDisableable,
  connect(
    state => ({
      options: getTranslatableLanguages(state)
    })
  ),
  localizeList({
    input: 'options',
    idAttribute: 'id',
    nameAttribute: 'label'
  }),
  asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  injectSelectPlaceholder()
)(Language)
