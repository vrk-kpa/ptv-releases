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
import { Select } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { setCompareMode } from 'reducers/formStates'
import {
  setComparisionLanguage,
  clearComparisionLanguage
} from 'reducers/selections'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import { createSelector } from 'reselect'
import entitySelector from 'selectors/entities/entities'
import { getSelectedComparisionLanguageId, getContentLanguageCode } from 'selectors/selections'
import { getFormValue } from 'selectors/base'
import { defineMessages, intlShape, injectIntl } from 'util/react-intl'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import cx from 'classnames'
import { Map, List } from 'immutable'

const getLanguageAvailabilities = createSelector(
  getFormValue('languagesAvailabilities'),
  languages => languages || List()
)

const getLanguages = createSelector(
  [getLanguageAvailabilities, entitySelector.languages.getEntities, getContentLanguageCode],
  (languagesAvailability, languages, entityLanguageCode) => {
    const result = languagesAvailability.map(language => {
      const languageId = language.get('languageId')
      return languages.get(languageId) || Map()
    })
    .filter(language => language.get('code') !== entityLanguageCode && language.get('code') != null)
    .map(language => ({
      label: language.get('code'),
      code: language.get('code'),
      value: language.get('id')
    }))
    .toList()
    .toJS()
    return result
  }
)
const getHasNoComparisionLanguage = createSelector(
  getLanguages,
  languagesAvailability => languagesAvailability.length === 0
)

const messages = defineMessages({
  title: {
    id: 'AppComponents.LanguageComparisonSelect.Title',
    defaultMessage: 'Vertailukieli'
  },
  placeholder: {
    id: 'AppComponents.LanguageComparisonSelect.Placeholder',
    defaultMessage: 'Valitse vertailukieli'
  },
  noComparisionLanguage: {
    id: 'AppComponents.LanguageComparisonSelect.NoComparisionLanguage',
    defaultMessage: 'Ei vertailukieltä'
  }
})

const LanguageComparisonSelect = ({
  dispatch,
  formName,
  languages,
  // entityId,
  selectedLanguageId,
  noComparisionLanguage,
  intl: { formatMessage },
  isCompareMode,
  inTranslation,
  hasError,
  ...rest
}) => {
  const handleOnChange = ({ code, value }) => {
    [
      setCompareMode({
        form: formName,
        value: true
      }),
      setComparisionLanguage({
        id: value,
        code: code
      })
    ].forEach(dispatch)
  }
  const handleOnCancel = () => {
    [
      setCompareMode({
        form: formName,
        value: false
      }),
      clearComparisionLanguage()
    ].forEach(dispatch)
  }
  const renderValue = (option) => {
    return <span>
      <span className={styles.selectLabel}>{formatMessage(messages.title)}</span>
      <span className={styles.selectValue}>{option.label}</span>
    </span>
  }
  // if (!entityId) return null
  const wrapClass = cx(
    styles.wrap,
    {
      [styles.compare]: isCompareMode
    }
  )
  return (
    !inTranslation &&
    <div className={wrapClass}>
      {noComparisionLanguage
        ? <div>{formatMessage(messages.noComparisionLanguage)}</div>
        : <Select
          label={null}
          value={selectedLanguageId}
          onChange={handleOnChange}
          options={languages}
          valueRenderer={renderValue}
          disabled={hasError}
          {...rest}
        />
      }
      {selectedLanguageId && isCompareMode && !hasError &&
        <div className={styles.closeIcon} onClick={handleOnCancel}>
          <PTVIcon name='icon-cross' />
        </div>
      }
    </div>
  )
}
LanguageComparisonSelect.propTypes = {
  dispatch: PropTypes.func,
  formName: PropTypes.string,
  languages: PropTypes.array,
  isSomeLanguageAvailable: PropTypes.bool,
  selectedLanguageId: PropTypes.string,
  intl: intlShape,
  isCompareMode: PropTypes.bool,
  noComparisionLanguage: PropTypes.bool,
  inTranslation: PropTypes.bool,
  hasError: PropTypes.bool
}

export default compose(
  injectFormName,
  injectIntl,
  withFormStates,
  connect((state, { formName, getHasErrorSelector }) => {
    const hasError = getHasErrorSelector && getHasErrorSelector(state, { formName })
    return {
      noComparisionLanguage: getHasNoComparisionLanguage(state, { formName }),
      languages: getLanguages(state, { formName }),
      // entityId: getSelectedEntityId(state),
      selectedLanguageId: getSelectedComparisionLanguageId(state),
      hasError
    }
  }),
  localizeList({
    input:'languages',
    idAttribute: 'value',
    nameAttribute: 'label',
    languageTranslationType: languageTranslationTypes.locale
  }),
  injectSelectPlaceholder({ placeholder: messages.placeholder })
)(LanguageComparisonSelect)
