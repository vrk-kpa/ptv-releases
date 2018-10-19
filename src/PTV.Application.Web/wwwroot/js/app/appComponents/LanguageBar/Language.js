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
import { getTranslationLanguageCode } from 'selectors/common'
import { EntitySelectors } from 'selectors'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { localizeProps } from 'appComponents/Localize'
import Popup from 'appComponents/Popup'
import cx from 'classnames'
import styles from './styles.scss'
import { getArchivedStatusesId } from './selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  languageVersionNotAvailablePart1: {
    id: 'AppComponents.LanguageBar.LanguageVersionNotAvailablePart1.Tooltip',
    defaultMessage: 'Kieliversiota'
  },
  languageVersionNotAvailablePart2: {
    id: 'AppComponents.LanguageBar.LanguageVersionNotAvailablePart2.Tooltip',
    defaultMessage: 'ei löydy'
  }
})

const LanguageText = compose(
  localizeProps()
)(({ name, className }) => (
  <div className={className}>{name}</div>
))

const Language = ({
  languageCode,
  onClick,
  onMouseEnter,
  languageId,
  publishingStatus,
  componentClass,
  selectedLanguage,
  isArchived,
  notAvailable,
  intl: { formatMessage }
}) => {
  publishingStatus = (publishingStatus && publishingStatus.toLowerCase()) || null
  const handleOnCLick = (event) => {
    onClick && onClick({ languageCode, languageId })
  }
  const handleOnMouseEnter = (event) => {
    onMouseEnter && onMouseEnter({ languageCode, languageId })
  }
  const isCurrent = selectedLanguage === languageId
  const languageBarItemClass = cx(
    styles.languageBarItem, {
      [styles.selected]: isCurrent
    },
    componentClass
  )
  const languageCodeClass = cx(
    styles.languageCode, {
      [styles.archived]: isArchived || publishingStatus === 'deleted',
      [styles.published]: !isArchived && publishingStatus === 'published',
      [styles.draft]: !isArchived && publishingStatus !== 'deleted' && publishingStatus !== 'published'
    }
  )
  if (notAvailable) {
    return (
      <div className={styles.languageBarItem}>
        <Popup
          trigger={<div className={styles.notificationIcon}>!</div>}
          position='top center'
          on='hover'
          dark
          iconClose={false}
        >
          <div className={styles.notAvailableTooltip}>
            <span>{formatMessage(messages.languageVersionNotAvailablePart1)}</span>
            <LanguageText
              id={languageId}
              name={languageCode}
              className={styles.tooltipLanguageName}
            />
            <div className={styles.tooltipLanguageCode}>({languageCode})</div>
            <span>{formatMessage(messages.languageVersionNotAvailablePart2)}</span>
          </div>
        </Popup>
      </div>
    )
  }
  return (
    <div
      className={languageBarItemClass}
      onMouseEnter={!isCurrent ? handleOnMouseEnter : null}
      onClick={!isCurrent ? handleOnCLick : null}
    >
      <div className={languageCodeClass}>{languageCode.toUpperCase()}</div>
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
  isArchived: PropTypes.bool,
  notAvailable: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(
    (state, { statusId, languageId, publishingStatusId }) => ({
      publishingStatus: EntitySelectors.publishingStatuses.getEntities(state).getIn([statusId, 'code']),
      languageCode: getTranslationLanguageCode(state, { id: languageId }),
      languageId,
      isArchived: getArchivedStatusesId(state).includes(publishingStatusId)
    })
  )
)(Language)
