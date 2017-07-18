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
import React, { PropTypes } from 'react'
import { getPublishingStatuses, getTranslationLanguageCode, getTranslationLanguageName } from 'Containers/Common/Selectors'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { PTVIcon } from '../../Components'
import { LocalizedText } from 'Containers/Common/localizedData';
import cx from 'classnames'

const Language = ({
  languageCode,
  onClick,
  onMouseEnter,
  languageId,
  publishingStatus,
  componentClass,
  selectedLanguage,
  includeNames
}) => {
  publishingStatus = publishingStatus.toLowerCase()
  const handleOnCLick = (event) => {
    onClick && onClick({ languageCode, languageId })
  }
  const handleOnMouseEnter = (event) => {
    onMouseEnter && onMouseEnter({ languageCode, languageId })
  }
  const icon = publishingStatus === 'draft' || publishingStatus === 'deleted'
      ? 'icon-not-available'
      : 'icon-available'
  const isCurrent = selectedLanguage === languageId
  const languageBarItemClass = cx(
    'language-bar-item', {
      'with-names': includeNames,
      'selected': isCurrent
    },
    componentClass
  )

  return (
    <div className={languageBarItemClass}>
      <div
        className='flex-container'
        onMouseEnter={!isCurrent ? handleOnMouseEnter : null}
        onClick={!isCurrent ? handleOnCLick : null}
      >
        <PTVIcon name={icon} className={publishingStatus} width={16} height={16} />
        <span className='language-bar-text'>
          <div className='language-code'>{languageCode.toUpperCase()}</div>
          { includeNames &&
            <LocalizedText
              id={languageId}
              name={languageCode}
              className='language-name'
            />
          }
        </span>
      </div>
    </div>
  )
}

Language.propTypes = {
  publishingStatus: PropTypes.string,
  languageCode: PropTypes.string,
  languageId: PropTypes.string,
  onClick: PropTypes.func,
  onMouseEnter: PropTypes.func,
  componentClass: PropTypes.string,
  selectedLanguage: PropTypes.string,
  includeNames: PropTypes.bool
}

export default compose(
  connect(
    (state, { statusId, languageId }) => ({
      publishingStatus: getPublishingStatuses(state).getIn([statusId, 'code']),
      languageCode: getTranslationLanguageCode(state, { id: languageId }),
      languageId
    })
  )
)(Language)
