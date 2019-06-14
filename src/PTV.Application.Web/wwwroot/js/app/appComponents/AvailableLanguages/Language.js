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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getContentLanguageCode } from 'selectors/selections'
import LanguageStatusSquare from 'appComponents/LanguageStatusSquare'
import styles from './styles.scss'
import cx from 'classnames'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getArchivedStatusesId } from 'appComponents/LanguageBar/selectors'

const Language = ({
  languageId,
  languageCode,
  previewDialogLanguageCode,
  statusId,
  languageName,
  onClick,
  defaultLanguage,
  languageKey,
  validFrom,
  isArchived
}) => {
  const handleOnClick = () => {
    onClick && onClick({ code: languageCode, id: languageId, languageKey })
  }
  const active = languageCode === previewDialogLanguageCode || !previewDialogLanguageCode && defaultLanguage
  const languageClass = cx(
    styles.language,
    {
      [styles.active]: active
    }
  )
  return (
    <div className={languageClass} onClick={!active ? handleOnClick : undefined}>
      <LanguageStatusSquare
        languageId={languageId}
        statusId={statusId}
        validFrom={validFrom}
        isArchived={isArchived}
      />
      <div className={styles.languageName}>{languageName}</div>
    </div>
  )
}

Language.propTypes = {
  onClick: PropTypes.func,
  languageId: PropTypes.string,
  languageCode: PropTypes.string,
  previewDialogLanguageCode: PropTypes.string,
  statusId: PropTypes.string,
  languageName: PropTypes.string,
  languageKey: PropTypes.string,
  defaultLanguage: PropTypes.bool,
  validFrom: PropTypes.number,
  isArchived: PropTypes.bool
}

export default compose(
  withLanguageKey,
  injectFormName,
  connect((state, { languageKey, formName, publishingStatusId }) => ({
    previewDialogLanguageCode: getContentLanguageCode(state, { languageKey, formName }),
    isArchived: getArchivedStatusesId(state).includes(publishingStatusId)
  }))
)(Language)
