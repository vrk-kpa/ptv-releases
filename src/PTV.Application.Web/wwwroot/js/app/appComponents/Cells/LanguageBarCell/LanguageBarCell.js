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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getAvailableLanguagesToCompare } from './selectors'
import enumSelector from 'selectors/enums'
import { entityUrlSetup } from 'enums/form'

// Components
import LanguageBar from 'appComponents/LanguageBar'
import { setContentLanguage, setSelectedEntity } from 'reducers/selections'
import { camelCase } from 'lodash'
import { fromJS } from 'immutable'
import { withRouter } from 'react-router'
import styles from './styles.scss'

const LanguageBarCell = ({
  languagesAvailabilities,
  languageIds,
  main,
  sub,
  id,
  setSelectedEntity,
  setContentLanguage,
  clickable,
  history,
  availableLanguagesToCompare,
  showMissing, // exclamation mark icon for languages which do not exist considering selected entity languages
  className,
  ...rest
}) => {
  const handleOnLanguageClick = ({ languageId, languageCode }) => {
    setContentLanguage({
      id: languageId,
      code: languageCode
    })
    setSelectedEntity({
      id,
      type: entityUrlSetup[main][sub].type
    })
    history.push(entityUrlSetup[main][sub].path + '/' + id)
  }
  const getSortedLanguageAvailabilities = (langs) => {
    if (!langs) return []
    return langs.sort((languageA, languageB) =>
      languageIds.indexOf(languageA.languageId) - languageIds.indexOf(languageB.languageId)
    )
  }
  const getLanguageAvailabilitiesIntersection = () => {
    const availableLanguagesSet = fromJS(languagesAvailabilities).toSet()
    const availableLanguagesToCompareSet = availableLanguagesToCompare.toSet()
    const availableLanguagesUnion = availableLanguagesSet.union(availableLanguagesToCompareSet).toJS()
    const languageSetup = availableLanguagesUnion.reduce((obj, item) => {
      const languageId = item.languageId
      obj[languageId] = {
        languageId: languageId,
        notAvailable: true
      }
      return obj
    }, {})
    languagesAvailabilities.forEach(lang => {
      const languageId = lang.languageId
      if (languageSetup.hasOwnProperty(languageId)) {
        languageSetup[languageId] = lang
      }
    })
    return getSortedLanguageAvailabilities(Object.values(languageSetup))
  }
  return (
    <div className={className}>
      <LanguageBar
        items={showMissing ? getLanguageAvailabilitiesIntersection() : languagesAvailabilities}
        {...rest}
        onClick={clickable ? handleOnLanguageClick : () => {}}
        className={styles.languages}
      />
    </div>
  )
}

LanguageBarCell.propTypes = {
  languagesAvailabilities: PropTypes.array,
  languageIds: ImmutablePropTypes.list,
  main: PropTypes.string,
  sub: PropTypes.string,
  id: PropTypes.string,
  setSelectedEntity: PropTypes.func.isRequired,
  setContentLanguage: PropTypes.func.isRequired,
  clickable: PropTypes.bool,
  availableLanguagesToCompare: ImmutablePropTypes.list.isRequired,
  showMissing: PropTypes.bool,
  history: PropTypes.object.isRequired,
  className: PropTypes.string
}

LanguageBarCell.defaultProps = {
  clickable: false
}

export default compose(
  withRouter,
  connect(
    (state, ownProps) => ({
      main: camelCase(ownProps.entityType),
      sub: camelCase(ownProps.subEntityType),
      languageIds: enumSelector.translationLanguages.getEnums(state),
      availableLanguagesToCompare: getAvailableLanguagesToCompare(state, ownProps)
    }), {
      setSelectedEntity,
      setContentLanguage
    }
  ))(LanguageBarCell)
