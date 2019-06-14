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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import {
  injectIntl,
  intlShape,
  defineMessages
} from 'util/react-intl'
import { ShowDate } from 'appComponents/Cells/ModifiedAtCell/ModifiedAtCell'
import styles from './styles.scss'
import { historyActionTypesEnum } from 'enums'
import { compose } from 'redux'
import _ from 'lodash'
import { connect } from 'react-redux'
import { getTargetLanguageCodes, getSourceLanguageCode } from './selectors'

const messages = defineMessages({
  published: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Published',
    defaultMessage: 'Published'
  },
  archived: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Archived',
    defaultMessage: 'Archived'
  },
  draft: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Draft',
    defaultMessage: 'Draft'
  },
  delete: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.DeleteAction',
    defaultMessage: 'Archive'
  },
  restore: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.RestoreAction',
    defaultMessage: 'Restore'
  },
  withdraw: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.WithdrawAction',
    defaultMessage: 'Withdraw'
  },
  ordered: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationOrderedAction',
    defaultMessage: 'Translation ordered'
  },
  received: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationReceivedAction',
    defaultMessage: 'Translation arrived'
  },
  copyPrefix: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.CopyAction.Prefix',
    defaultMessage: 'Kopioitu luonnokseksi',
    description: {
      en: 'Copied as a draft',
      sv: 'Kopierat som utkast'
    }
  },
  copyInfix: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.CopyAction.Infix',
    defaultMessage: 'organisaatiolta',
    description: {
      en: 'from organization',
      sv: 'från organisationen'
    }
  },
  massPublish: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.MassPublishAction',
    defaultMessage: 'Published in schedule'
  },
  schedulePublish: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.SchedulePublishAction',
    defaultMessage: 'Scheduled to be published'
  },
  scheduleArchive: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.ScheduleArchiveAction',
    defaultMessage: 'Scheduled to be archived'
  },
  reordered: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationReorderedAction',
    defaultMessage: 'Päivitystilaus lähetetty',
    description: {
      en: 'Translation re-ordered'
    }
  }
})

const OperationTypeCell = props => {
  const {
    intl: { formatMessage, locale },
    versionMajor,
    versionMinor,
    actionDate,
    organization,
    sourceLanguageCode,
    targetLanguageCodes
  } = props

  const publishingStatus = _.camelCase(props.publishingStatus)
  const historyAction = _.camelCase(props.historyAction)

  const getHistoryActionMessage = () => {
    return {
      [historyActionTypesEnum.DELETE]: formatMessage(messages.delete),
      [historyActionTypesEnum.RESTORE]: formatMessage(messages.restore),
      [historyActionTypesEnum.MASSRESTORE]: formatMessage(messages.restore),
      [historyActionTypesEnum.WITHDRAW]: formatMessage(messages.withdraw),
      [historyActionTypesEnum.TRANSLATIONORDERED]: formatMessage(messages.ordered),
      [historyActionTypesEnum.TRANSLATIONREORDERED]: formatMessage(messages.reordered),
      [historyActionTypesEnum.TRANSLATIONRECEIVED]: formatMessage(messages.received),
      [historyActionTypesEnum.MASSPUBLISH]: formatMessage(messages.massPublish),
      [historyActionTypesEnum.SCHEDULEDPUBLISH]: formatMessage(messages.schedulePublish),
      [historyActionTypesEnum.SCHEDULEDARCHIVE]: formatMessage(messages.scheduleArchive)
    }[historyAction] || null
  }

  const getPublishingStatusMessage = () => {
    if ([
      historyActionTypesEnum.MASSPUBLISH,
      historyActionTypesEnum.SCHEDULEDPUBLISH,
      historyActionTypesEnum.SCHEDULEDARCHIVE,
      historyActionTypesEnum.COPY,
      historyActionTypesEnum.TRANSLATIONORDERED,
      historyActionTypesEnum.TRANSLATIONREORDERED,
      historyActionTypesEnum.TRANSLATIONRECEIVED
    ].some(x => x === historyAction)) {
      return null
    }

    return {
      'published': formatMessage(messages.published),
      'archived': formatMessage(messages.archived),
      'draft': formatMessage(messages.draft)
    }[publishingStatus] || null
  }

  const getTranslationLanguages = () => {
    if ([
      historyActionTypesEnum.SCHEDULEDPUBLISH,
      historyActionTypesEnum.SCHEDULEDARCHIVE
    ].some(x => x === historyAction)) {
      if (targetLanguageCodes && targetLanguageCodes.length > 0) {
        return `[${targetLanguageCodes.join(', ')}]`
      }
    }

    if ([
      historyActionTypesEnum.TRANSLATIONORDERED,
      historyActionTypesEnum.TRANSLATIONREORDERED,
      historyActionTypesEnum.TRANSLATIONRECEIVED
    ].some(x => x === historyAction)) {
      if (!sourceLanguageCode || !targetLanguageCodes || targetLanguageCodes.length === 0) {
        return null
      }

      return `[${sourceLanguageCode} > ${targetLanguageCodes.join(', ')}]`
    }
  }

  const getVersion = () => {
    if ([
      historyActionTypesEnum.SCHEDULEDPUBLISH,
      historyActionTypesEnum.SCHEDULEDARCHIVE
    ].some(x => x === historyAction)) {
      return null
    }

    return `${(versionMajor || versionMinor) && (versionMajor + '.')}${versionMinor}`
  }

  const getCopyMessage = () => {
    const organizationNames = organization && organization.name
    const defaultLanguageCode = Object.keys(organizationNames)[0]
    const organizationName = organizationNames && (organizationNames[locale] || organizationNames[defaultLanguageCode])
    // eslint-disable-next-line max-len
    return `${formatMessage(messages.copyPrefix)} ${getVersion()} ${formatMessage(messages.copyInfix)} ${organizationName}`
  }

  const getMessage = () => {
    if (historyAction === historyActionTypesEnum.COPY) {
      return getCopyMessage()
    }

    return (
      <Fragment>
        {[
          getHistoryActionMessage(),
          getPublishingStatusMessage(),
          getVersion(),
          getTranslationLanguages()
        ].join(' ')}
        <ShowDate value={actionDate} />
      </Fragment>
    )
  }

  return (
    <div className={styles.operationTypeCell}>
      {getMessage()}
    </div>
  )
}

OperationTypeCell.propTypes = {
  intl: intlShape,
  publishingStatus: PropTypes.string,
  historyAction: PropTypes.string,
  versionMajor: PropTypes.number,
  versionMinor: PropTypes.number,
  actionDate: PropTypes.string,
  organization: PropTypes.object,
  locale: PropTypes.string,
  targetLanguageCodes: PropTypes.array,
  sourceLanguageCode: PropTypes.string
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    return {
      sourceLanguageCode: getSourceLanguageCode(state, { id: ownProps.sourceLanguageId }),
      targetLanguageCodes: getTargetLanguageCodes(ownProps.targetLanguageIds)(state, ownProps)
    }
  })
)(OperationTypeCell)
