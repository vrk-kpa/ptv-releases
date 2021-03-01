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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import styles from './styles.scss'
import cx from 'classnames'
import { getPublishingStatuses } from './selectors'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import Popup from 'appComponents/Popup'
import moment from 'moment'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  publishDateInfo: {
    id: 'LanguageStatusSquare.publishDateInfo',
    defaultMessage: 'Ajastettu julkaistavaksi'
  }
})

const LanguageText = compose(
  localizeProps({
    languageTranslationType: languageTranslationTypes.locale
  })
)(({ name }) => <span className={styles.capitalize}>{name}</span>)

const LanguageStatusSquare = ({
  code,
  status,
  statusId,
  languageId,
  onClick,
  validFrom,
  intl: { formatMessage },
  disabled,
  ...rest
}) => {
  const isDeletedStatus = status.toLowerCase() === 'deleted' || status.toLowerCase() === 'oldpublished'
  const isPublishedStatus = status.toLowerCase() === 'published'
  const languageStatusClass = cx(
    styles.languageStatus,
    styles.languageCode, {
      [styles.archived]: isDeletedStatus,
      [styles.published]: isPublishedStatus,
      [styles.draft]: !isDeletedStatus && !isPublishedStatus,
      [styles.timedPublish]: validFrom,
      [styles.disabled]: disabled
    }
  )
  return (
    disabled
      ? (
        <div className={languageStatusClass}>
          {code.toUpperCase()}
        </div>
      ) : (
        <Popup
          trigger={
            <div
              onClick={() => onClick && onClick({ languageId, code })}
              className={languageStatusClass}
            >
              {code.toUpperCase()}
            </div>
          }
          position='top center'
          on='hover'
          dark
          iconClose={false}
          hideOnScroll
        >
          <span className={styles.tooltipContent}>
            <span>
              <LanguageText
                id={languageId}
                name={code}
              />
              {status && ' / '}
              <LanguageText
                id={statusId}
                name={status}
              />
            </span>
            {validFrom && (
              <Fragment>
                <span className={styles.publishOn}>{formatMessage(messages.publishDateInfo)}</span>
                <span>{moment(validFrom).format('DD.MM.YYYY')}</span>
              </Fragment>
            )}
          </span>
        </Popup>
      )
  )
}

LanguageStatusSquare.propTypes = {
  code: PropTypes.string,
  status: PropTypes.string,
  statusId: PropTypes.string,
  languageId: PropTypes.string,
  onClick: PropTypes.func,
  validFrom: PropTypes.number,
  disabled: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => ({
      ...getPublishingStatuses(state, ownProps)
    })
  )
)(LanguageStatusSquare)
