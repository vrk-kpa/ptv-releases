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
import {
  getSelectedEntityId,
  getIsAnyInTranslation
} from 'selectors/entities/entities'
import { getContentLanguageCode } from 'selectors/selections'
import { getCurrentItem } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { switchToEditableVersion } from 'util/redux-form/HOC/withMassToolForm/actions'
import { injectIntl, intlShape } from 'util/react-intl'
import NotificationDialog from './NotificationDialog'
import { Button } from 'sema-ui-components'
import messages from '../messages'
import cx from 'classnames'
import styles from '../styles.scss'
import { getMissingLanguagesForApproveTranslated } from 'selectors/massTool'
import { formTypesEnum } from 'enums'
import QualityAgentWarningDialog from './QualityAgentWarningDialog'

const PublishUnavailableDialog = compose(
  injectIntl,
  connect(state => {
    const language = getContentLanguageCode(state)
    const currentItem = getCurrentItem(state, { language })
    return {
      id: getSelectedEntityId(state),
      hasError: currentItem.get('hasError'),
      isInTranslation: getIsAnyInTranslation(state),
      isEditable: currentItem.get('isEditable'),
      editableVersion: currentItem.get('editableVersion'),
      approveMissingLanguages: getMissingLanguagesForApproveTranslated(
        state, {
          id: currentItem.get('unificRootId'),
          formName: formTypesEnum.MASSTOOLFORM
        })
    }
  }, {
    switchToEditableVersion
  })
)(({
  intl: { formatMessage },
  id,
  hasError,
  isInTranslation,
  isEditable,
  editableVersion,
  switchToEditableVersion,
  approveMissingLanguages
}) => {
  const descriptionMessages = [
    hasError ? formatMessage(messages.mandatoryInfoMissing) : null,
    isInTranslation ? formatMessage(messages.inTranslation) : null,
    approveMissingLanguages.length
      ? formatMessage(messages.languageVersionApproveMissing, {
        languages: approveMissingLanguages.map(l => l.name).join(', ')
      })
      : null
  ].filter(x => x)
  const handleEditableVersionClick = () => {
    switchToEditableVersion({ id, editableVersion })
  }
  const dialogClass = cx(
    styles.withBullets,
    styles.withoutDescription
  )
  return (
    (descriptionMessages.length || !isEditable) &&
      <NotificationDialog
        type='fail'
        asAlert
        title={formatMessage(messages.massPublishErrorTitle)}
        className={dialogClass}
        closeLabel={formatMessage(messages.closeLabel)}
      >
        <ul>
          {!isEditable && editableVersion && (
            <li>
              <Button link onClick={handleEditableVersionClick}>
                {formatMessage(messages.editableVersionLink)}
              </Button>
            </li>
          ) || descriptionMessages.length > 0 && (
              descriptionMessages.map((message, index) => <li key={index}>{message}</li>)
            )}
        </ul>
      </NotificationDialog> || <QualityAgentWarningDialog />
  )
})

PublishUnavailableDialog.propTypes = {
  id: PropTypes.string,
  hasError: PropTypes.bool,
  isInTranslation: PropTypes.bool,
  isEditable: PropTypes.bool,
  editableVersion: PropTypes.string,
  switchToEditableVersion: PropTypes.func,
  intl: intlShape
}

export default PublishUnavailableDialog
