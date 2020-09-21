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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Select, Label } from 'sema-ui-components'
import { change } from 'redux-form/immutable'
import { createSelector } from 'reselect'
import { compose } from 'redux'
import { List, Map } from 'immutable'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  setNewlyAddedLanguage,
  enableForm,
  setIsAddingNewLanguage
} from 'reducers/formStates'
import { setContentLanguage } from 'reducers/selections'
import { getNewlyAddedLanguage } from 'selectors/formStates'

import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { EnumsSelectors } from 'selectors'
import { getFormValue } from 'selectors/base'
import styles from './styles.scss'

const messages = defineMessages({
  title: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.NewLanguageSelection.Title',
    defaultMessage: 'Valitse kieliversio'
  },
  placeholder: {
    id: 'Component.Select.Placeholder',
    defaultMessage: '- valitse -'
  }
})

const getLanguages = createSelector(
  getFormValue('languagesAvailabilities'),
  EnumsSelectors.translationLanguages.getEntities,
  (languagesAvailabilities, languages) => {
    const result = languages.filter(language => {
      return !(languagesAvailabilities || List()).some(languageAvailability => {
        return languageAvailability.get('languageId') === language.get('id')
      })
    }).toList() || List()
    return result.map(language => ({
      label: language.get('code'),
      code: language.get('code'),
      value: language.get('id')
    }))
  }
)

const handleNewLanguage = (code, handleNewLanguageResponse) => store => {
  handleNewLanguageResponse(code, store)
}

const NewLanguageSelection = ({
  languages,
  formLanguages,
  change,
  setNewlyAddedLanguage,
  setContentLanguage,
  setIsAddingNewLanguage,
  newlyAddedLanguage,
  enableForm,
  formName,
  handleNewLanguageResponse,
  handleNewLanguage,
  intl: { formatMessage }
}) => {
  const handleOnSelectNewLanguage = ({ value: id, code }) => {
    handleNewLanguageResponse && handleNewLanguage(code, handleNewLanguageResponse)
    setNewlyAddedLanguage({
      form: formName,
      value: { id, code }
    })
    enableForm(formName)
    setContentLanguage({ id, code })
    setIsAddingNewLanguage({
      form: formName,
      value: false
    })
    if (formLanguages) {
      change(formName, 'languagesAvailabilities', formLanguages.pop().push(Map({ languageId : id, code })))
    }
  }
  return (
    <div className='d-flex align-items-center'>
      <Label required labelText={formatMessage(messages.title)} className={styles.addNewLanguageLabel} />
      <Select size='w160'
        options={languages}
        value={newlyAddedLanguage.value}
        onChange={handleOnSelectNewLanguage}
        label={null}
        placeholder={formatMessage(messages.placeholder)}
      />
    </div>
  )
}
NewLanguageSelection.propTypes = {
  languages: PropTypes.array,
  setNewlyAddedLanguage: PropTypes.func,
  setContentLanguage: PropTypes.func,
  setIsAddingNewLanguage: PropTypes.func,
  change: PropTypes.func.isRequired,
  newlyAddedLanguage: PropTypes.object,
  handleNewLanguageResponse: PropTypes.func,
  handleNewLanguage: PropTypes.func,
  enableForm: PropTypes.func,
  formName: PropTypes.string,
  formLanguages: ImmutablePropTypes.list,
  intl: intlShape
}

export default compose(
  injectFormName,
  injectIntl,
  connect(
    (state, { formName }) => ({
      languages: getLanguages(state, { formName }).toJS(),
      formLanguages: getFormValue('languagesAvailabilities')(state, { formName }) || List(),
      newlyAddedLanguage: getNewlyAddedLanguage(formName)(state) || {}
    }), {
      setNewlyAddedLanguage,
      setContentLanguage,
      setIsAddingNewLanguage,
      enableForm,
      change,
      handleNewLanguage
    }
  ),
  localizeList({
    input:'languages',
    idAttribute: 'value',
    nameAttribute: 'label',
    languageTranslationType: languageTranslationTypes.locale
  }),
)(NewLanguageSelection)
