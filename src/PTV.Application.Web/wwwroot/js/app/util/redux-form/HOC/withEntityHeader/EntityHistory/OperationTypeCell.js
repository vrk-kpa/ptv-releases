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
    defaultMessage: 'Versio {version} Arkistoitu muokkaajan toimesta',
    description: {
      en: 'Version {version} archived by the editor'
    }
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
    defaultMessage: 'The editor scheduled the version {version} to be archived'
  },
  reordered: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationReorderedAction',
    defaultMessage: 'Päivitystilaus lähetetty',
    description: {
      en: 'Translation re-ordered'
    }
  },
  oldPublished: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.OldPublishedAction',
    defaultMessage: 'Versio {nextVersion} julkaisun myötä versio {version} arkistoitui',
    description: {
      en: 'Version {version} archived due to publishing of {nextVersion}'
    }
  },
  expired: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.ExpiredAction',
    defaultMessage: 'Versio {version} arkistoitui: sisältöä ei päivitetty {expiration} kuukauden sisällä',
    description: {
      en: 'Version {version} archived: the content was not updated within {expiration} months'
    }
  },
  massArchive: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.MassArchiveAction',
    defaultMessage: 'Archived in a batch'
  },
  archivedViaOrganization: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.ArchivedViaOrganizationAction',
    defaultMessage: 'Sisältö arkistoitunut organisaation arkistoimisen yhteydessä',
    description: {
      en: 'Content archived due to archived parent organisation'
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
    targetLanguageCodes,
    expirationPeriod
  } = props

  const publishingStatus = _.camelCase(props.publishingStatus)
  const historyAction = _.camelCase(props.historyAction)

  const getHistoryActionMessage = () => {
    return {
      [historyActionTypesEnum.RESTORE]: messages.restore,
      [historyActionTypesEnum.MASSRESTORE]: messages.restore,
      [historyActionTypesEnum.WITHDRAW]: messages.withdraw,
      [historyActionTypesEnum.DELETE]: messages.delete,
      [historyActionTypesEnum.SCHEDULEDARCHIVE]: messages.scheduleArchive,
      [historyActionTypesEnum.OLDPUBLISHED]: messages.oldPublished,
      [historyActionTypesEnum.EXPIRED]: messages.expired,
      // Mass archive should use the same message as Delete
      [historyActionTypesEnum.MASSARCHIVE]: messages.delete,
      [historyActionTypesEnum.ARCHIVEDVIAORGANIZATION]: messages.archivedViaOrganization
    }[historyAction] || null
  }

  const getPublishingStatusMessage = () => {
    return {
      'published': formatMessage(messages.published),
      'archived': formatMessage(messages.archived),
      'draft': formatMessage(messages.draft)
    }[publishingStatus] || null
  }

  const getVersion = () => {
    return `${(versionMajor || versionMinor) && (versionMajor + '.')}${versionMinor}`
  }

  const calculateNextVersion = () => {
    const safeVersion = versionMajor || '0'
    const previousVersion = parseInt(safeVersion)
    return `${previousVersion + 1}.0`
  }

  const getCopyMessage = () => {
    const organizationNames = organization && organization.name
    const defaultLanguageCode = Object.keys(organizationNames)[0]
    const organizationName = organizationNames && (organizationNames[locale] || organizationNames[defaultLanguageCode])
    // eslint-disable-next-line max-len
    return `${formatMessage(messages.copyPrefix)} ${getVersion()} ${formatMessage(messages.copyInfix)} ${organizationName}`
  }

  const getScheduledPublishMessage = () => {
    const languages = (targetLanguageCodes && targetLanguageCodes.length > 0)
      ? `[${targetLanguageCodes.join(', ')}]`
      : null

    return (
      <Fragment>
        {`${formatMessage(messages.schedulePublish)} ${languages}`}
        <ShowDate value={actionDate} />
      </Fragment>
    )
  }

  const getTranslationMessage = () => {
    const languages = (!sourceLanguageCode || !targetLanguageCodes || targetLanguageCodes.length === 0)
      ? null
      : `[${sourceLanguageCode} > ${targetLanguageCodes.join(', ')}]`

    return (
      <Fragment>
        {`${getHistoryActionMessage()} ${getVersion()} ${languages}`}
        <ShowDate value={actionDate} />
      </Fragment>
    )
  }

  const getMassPublishMessage = () => {
    return (
      <Fragment>
        {`${formatMessage(messages.massPublish)} ${getVersion()}`}
        <ShowDate value={actionDate} />
      </Fragment>
    )
  }

  const getArchiveMessage = () => {
    const actionMessage = getHistoryActionMessage()
    const version = getVersion()
    const nextVersion = calculateNextVersion()

    return formatMessage(actionMessage, { version, nextVersion, expiration: expirationPeriod })
  }

  const getDefaultMessage = () => {
    const historyActionMessage = getHistoryActionMessage()
    const formattedMessage = historyActionMessage && formatMessage(historyActionMessage)
    return (
      <Fragment>
        {[formattedMessage, getPublishingStatusMessage(), getVersion()].join(' ')}
        <ShowDate value={actionDate} />
      </Fragment>
    )
  }

  const getMessage = action => {
    switch (action) {
      case historyActionTypesEnum.COPY:
        return getCopyMessage()
      case historyActionTypesEnum.DELETE:
      case historyActionTypesEnum.SCHEDULEDARCHIVE:
      case historyActionTypesEnum.OLDPUBLISHED:
      case historyActionTypesEnum.EXPIRED:
      case historyActionTypesEnum.MASSARCHIVE:
      case historyActionTypesEnum.ARCHIVEDVIAORGANIZATION:
        return getArchiveMessage()
      case historyActionTypesEnum.SCHEDULEDPUBLISH:
        return getScheduledPublishMessage()
      case historyActionTypesEnum.TRANSLATIONORDERED:
      case historyActionTypesEnum.TRANSLATIONRECEIVED:
      case historyActionTypesEnum.TRANSLATIONREORDERED:
        return getTranslationMessage()
      case historyActionTypesEnum.MASSPUBLISH:
        return getMassPublishMessage()
      default:
        return getDefaultMessage()
    }
  }

  return (
    <div className={styles.operationTypeCell}>
      {getMessage(historyAction)}
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
  targetLanguageCodes: PropTypes.array,
  sourceLanguageCode: PropTypes.string,
  expirationPeriod: PropTypes.string
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
