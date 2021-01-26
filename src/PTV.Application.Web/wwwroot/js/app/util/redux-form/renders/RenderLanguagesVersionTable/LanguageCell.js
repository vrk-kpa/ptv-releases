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
import { getLanguageCode } from 'selectors/common'
import { EnumsSelectors } from 'selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import styles from './styles.scss'

const LanguageCell = ({
  languageCode,
  languageId,
  languages
}) => {
  const languageEntity = languages.get(languageId)
  return (
    <div>
      <span className={styles.languageCode}>{languageCode}</span>
      <span className={styles.languageName}>{languageEntity && languageEntity.get('name')}</span>
    </div>
  )
}
LanguageCell.propTypes = {
  languageCode: PropTypes.string.isRequired,
  languageId: PropTypes.string,
  languages: ImmutablePropTypes.map.isRequired
}

export default compose(
  connect((state, { languageId }) => {
    const languages = EnumsSelectors.translationLanguages.getEntitiesMap(state)
    return {
      languages,
      languageId,
      languageCode: getLanguageCode(state, { languageId })
    }
  }),
  localizeList({
    input: 'languages',
    languageTranslationType: languageTranslationTypes.locale
  }),
)(LanguageCell)
