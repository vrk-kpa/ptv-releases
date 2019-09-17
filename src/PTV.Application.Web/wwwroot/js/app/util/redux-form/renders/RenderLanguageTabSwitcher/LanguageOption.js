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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { injectIntl, intlShape } from 'util/react-intl'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import {
  getCurrentEntityApprovedLanguages,
  getLanguageEntity
} from './selectors'
import {
  getIsInTranslation,
  getIsTranslationDelivered
} from 'selectors/entities/entities'
import { commonAppMessages } from 'util/redux-form/messages'
import {
  localizeItem,
  languageTranslationTypes
} from 'appComponents/Localize'
import LanguageStatusSquare from 'appComponents/LanguageStatusSquare'
import Popup from 'appComponents/Popup'
import styles from './styles.scss'
import cx from 'classnames'

const LanguageOption = ({
  active,
  approvedLanguages,
  className,
  code,
  disabled,
  inTab,
  intl: { formatMessage },
  isAddingNewLanguage,
  isInReview,
  name,
  option,
  translationArrived,
  translationOrdered,
  ...rest
}) => {
  const isApprovedLanguage = isInReview && approvedLanguages.includes(code)
  const languageOptionClass = cx(
    styles.languageOption,
    {
      [styles.approved]: isApprovedLanguage
    },
    className
  )
  const isNewOption = inTab ? active && isAddingNewLanguage : isAddingNewLanguage
  return (
    <div className={languageOptionClass}>
      <LanguageStatusSquare disabled={disabled} {...rest} />
      <span className={styles.languageOptionText}>
        {isNewOption && formatMessage(name) || option.get('name') || code}
      </span>
      {(translationOrdered || translationArrived) && !disabled && (
        <Popup
          trigger={<span className={styles.notificationIcon}>!</span>}
          position='top center'
          on='hover'
          dark
          iconClose={false}
        >
          {translationArrived
            ? formatMessage(commonAppMessages.translationArrived)
            : formatMessage(commonAppMessages.translationOrdered) }
        </Popup>
      )}
      {(translationOrdered || translationArrived) && disabled && (
        <span className={styles.notificationIcon}>!</span>
      )}
    </div>
  )
}

LanguageOption.propTypes = {
  active: PropTypes.bool,
  approvedLanguages: ImmutablePropTypes.list,
  className: PropTypes.string,
  code: PropTypes.string.isRequired,
  disabled: PropTypes.bool,
  inTab: PropTypes.bool,
  intl: intlShape,
  isAddingNewLanguage: PropTypes.bool,
  isInReview: PropTypes.bool,
  name: PropTypes.any,
  option: ImmutablePropTypes.map.isRequired,
  translationArrived: PropTypes.bool,
  translationOrdered: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { languageId, formName, code }) => ({
    isAddingNewLanguage: getIsAddingNewLanguage(formName)(state),
    language: getLanguageEntity(state, { languageId }),
    translationOrdered: getIsInTranslation(state, { languageCode: code }),
    translationArrived: getIsTranslationDelivered(state, { languageCode: code }),
    approvedLanguages: getCurrentEntityApprovedLanguages(state),
    isInReview: getShowReviewBar(state)
  })),
  localizeItem({
    input: 'language',
    output: 'option',
    languageTranslationType: languageTranslationTypes.locale
  })
)(LanguageOption)
