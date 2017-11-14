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
import { Select, Label } from 'sema-ui-components'
import { getEntityAvailableLanguages } from 'selectors/entities/entities'
import { List } from 'immutable'
import { Field } from 'redux-form/immutable'
import { getContentLanguageCode } from 'selectors/selections'
import { setConnectionLangaugeCode } from 'reducers/selections'
import { localizeList } from 'appComponents/Localize'
import { EntitySelectors } from 'selectors'
import { injectIntl, intlShape, defineMessages } from 'react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Common.PageContainer.LanguageSelect.Title',
    defaultMessage: 'Valitse kieliversio'
  },
  emptyMessage: {
    id: 'appComponents.ConnectionStep.LanguageSwitcher.EmptyTitle',
    defaultMessage: 'No language available'
  }
})

let RenderDisplayLanguageSwitcher = ({
  value,
  intl: { formatMessage },
  options,
  label,
  ...rest
}) => (
  <div>
    <Label labelText={label} />
    <div>{value && options && options.some(x => x.value === value)
        ? rest.formatDisplay
          ? rest.formatDisplay(options.filter(x => x.value === value)[0])
          : options.filter(x => x.value === value)[0].label
        : <Label infoLabel labelText={formatMessage(messages.emptyMessage)} />}
    </div>
  </div>
)
RenderDisplayLanguageSwitcher = compose(
  injectIntl
)(RenderDisplayLanguageSwitcher)
RenderDisplayLanguageSwitcher.propTypes = {
  input: PropTypes.object,
  options: PropTypes.array,
  value: PropTypes.string,
  label: PropTypes.string,
  onChange: PropTypes.func,
  intl: intlShape
}

let RenderLanguageSwitcher = ({
  options,
  value,
  onChange,
  intl: { formatMessage },
  isReadOnly,
  ...rest
}) => {
  const handleOnChange = language => {
    onChange(language.value)
  }
  const Component = isReadOnly
    ? RenderDisplayLanguageSwitcher
    : Select
  return (
    <Component
      labelPosition='top'
      label={formatMessage(messages.title)}
      options={options}
      onChange={handleOnChange}
      value={value}
      {...rest}
    />
  )
}
RenderLanguageSwitcher = compose(
  injectIntl,
  connect((state, { input, formName }) => {
    const connectionLanguageAvailabilities = ((input && input.value) || List())
      .map(languageAvailability => languageAvailability.get('languageId'))
      .toSet()
    const entityLanguageAvailabilities = getEntityAvailableLanguages(state)
      .map(languageAvailability => languageAvailability.get('languageId'))
      .toSet()
    const options = connectionLanguageAvailabilities
      .intersect(entityLanguageAvailabilities)
      .map(id => {
        const languageCode = EntitySelectors.languages
          .getEntity(state, { id })
          .get('code')
        return {
          id,
          value: languageCode
        }
      })
      .toJS()
    return {
      value: getContentLanguageCode(state, { formName }),
      options
    }
  }, {
    onChange: setConnectionLangaugeCode
  }),
  localizeList({
    input: 'options',
    idAttribute: 'id',
    nameAttribute: 'label'
  })
)(RenderLanguageSwitcher)
RenderLanguageSwitcher.propTypes = {
  options: PropTypes.array,
  value: PropTypes.string,
  isReadOnly: PropTypes.bool,
  onChange: PropTypes.func,
  intl: intlShape
}

const LanguageSwitcher = ({
  name,
  ...rest
}) => (
  <Field
    name={name}
    component={RenderLanguageSwitcher}
    {...rest}
  />
)
LanguageSwitcher.propTypes = {
  name: PropTypes.string
}

export default LanguageSwitcher
