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
import { connect } from 'react-redux'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getFormValue } from 'selectors/base'
import { getContentLanguageId } from 'selectors/selections'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import {
  getPublishingStatusDraftId,
  getPublishingStatusDeletedId,
  getPublishingStatusOldPublishedId
} from 'selectors/common'
import { capitalizeFirstLetter } from 'util/helpers'
import { getEntity } from 'selectors/entities/entities'
import { injectIntl, intlShape } from 'util/react-intl'
import { Map } from 'immutable'
import moment from 'moment'
import messages from 'util/redux-form/HOC/withEntityNotification/messages'
import Tag from 'appComponents/Tag'
import styles from './styles.scss'

const LanguageLabel = ({
  language,
  status,
  validFrom,
  intl: { formatMessage },
  ...rest
}) => (
  <div>
    <span>{capitalizeFirstLetter(language) + ' / ' + status}</span>
    {validFrom && <Tag
      message={`${formatMessage(messages.timedPublishTitle)} ${moment(validFrom).format('DD.MM.YYYY')}`}
      isRemovable={false}
      bgColor='#34B6E4'
      textColor='#FFFFFF'
      className={styles.tag}
    />}
  </div>
)

LanguageLabel.propTypes = {
  language: PropTypes.string.isRequired,
  status: PropTypes.string.isRequired,
  validFrom: PropTypes.number,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => {
      const entity = getEntity(state)
      const entityPS = entity.get('publishingStatus')
      const formLanguages = getFormValue('languagesAvailabilities')(state, { formName })
      const contentLanguageId = getContentLanguageId(state, { formName })
      const selectedLanguage = formLanguages.filter(x => x.get('languageId') === contentLanguageId).first() ||
        formLanguages.first() || Map()
      const isArchived = entityPS === getPublishingStatusOldPublishedId(state) ||
        entityPS === getPublishingStatusDeletedId(state)
      const statusId = isArchived && getPublishingStatusDeletedId(state) ||
        (selectedLanguage.get('statusId') || getPublishingStatusDraftId(state))
      return {
        languageId: selectedLanguage.get('languageId'),
        validFrom: selectedLanguage.get('validFrom'),
        statusId
      }
    }
  ),
  localizeProps({
    nameAttribute: 'language',
    idAttribute: 'languageId',
    languageTranslationType: languageTranslationTypes.locale
  }),
  localizeProps({
    nameAttribute: 'status',
    idAttribute: 'statusId',
    languageTranslationType: languageTranslationTypes.locale
  })
)(LanguageLabel)
