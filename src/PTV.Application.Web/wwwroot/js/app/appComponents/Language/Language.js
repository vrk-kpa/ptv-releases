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
import { compose } from 'redux'
import LanguageStatusSquare from 'appComponents/LanguageStatusSquare'
import { setContentLanguage } from 'reducers/selections'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import { EnumsSelectors } from 'selectors'
import styles from './styles.scss'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Map } from 'immutable'
import ImmutablePropTypes from 'react-immutable-proptypes'

const messages = defineMessages({
  noLanguageName: {
    id: 'Util.ReduxForm.Renders.RenderLanguageAvailabilitiesTable.LanguageCell.NoLanguageName',
    defaultMessage: 'Ei valittu'
  }
})

const Language = ({
  intl: { formatMessage },
  languages,
  publishingStatuses,
  languageId,
  languageCode,
  setContentLanguage,
  statusId,
  isArchived,
  version,
  validFrom
}) => {
  const handleOnClick = () => setContentLanguage({
    id: languageId,
    code: languageCode
  })

  const currentLanguage = languages.get(languageId) || Map({ name: formatMessage(messages.noLanguageName) })
  const languageStatus = publishingStatuses.get(statusId) ||
    (publishingStatuses.filter(x => x.get('code').toLowerCase() === 'draft').first() || Map())
  return (
    <div
      onClick={handleOnClick}
      className={styles.language}
    >
      <LanguageStatusSquare
        languageId={languageId}
        statusId={statusId}
        isArchived={isArchived}
        validFrom={validFrom}
      />
      <div className={styles.languageInfo}>
        <div className={styles.languageName}>{currentLanguage && currentLanguage.get('name') || ''}</div>
        <div className={styles.languageVersion}>{
          (languageStatus && languageStatus.get('name') || '')/* + ' ' + (version || '0.0')*/}</div>
      </div>
    </div>
  )
}
Language.propTypes = {
  intl: intlShape,
  languages: ImmutablePropTypes.map.isRequired,
  isArchived: PropTypes.bool.isRequired,
  publishingStatuses: ImmutablePropTypes.map.isRequired,
  languageId: PropTypes.string.isRequired,
  languageCode: PropTypes.string,
  setContentLanguage: PropTypes.func.isRequired,
  statusId: PropTypes.string.isRequired,
  version: PropTypes.string.isRequired,
  validFrom: PropTypes.number
}

export default compose(
  injectIntl,
  connect(state => ({
    languages: EnumsSelectors.translationLanguages.getEntitiesMap(state),
    publishingStatuses: EnumsSelectors.publishingStatuses.getEntitiesMap(state)
  }), {
    setContentLanguage
  }),
  localizeList({
    inputMapping: [
      { input: 'languages', output: 'languages' },
      { input: 'publishingStatuses', output: 'publishingStatuses' }
    ],
    languageTranslationType: languageTranslationTypes.locale
  })
)(Language)
