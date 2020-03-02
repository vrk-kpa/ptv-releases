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
import { compose } from 'redux'
import { connect } from 'react-redux'
import ModalDialog from 'appComponents/ModalDialog'
import { ModalActions, Button, ModalContent, Spinner } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { forceJob } from 'Routes/Admin/actions'
import {
  getLogJobId,
  getLogJobName,
  getAdminTasksEntityByCode,
  getLogJobArchive,
  getLoadJobIsFetching } from 'Routes/Admin/selectors'
import { adminTaskTypesEnum } from 'enums'
import withCopyToClipboard from 'util/redux-form/HOC/withCopyToClipboard'
import { buttonMessages } from 'Routes/messages'
import { commonAppMessages } from 'util/redux-form/messages'
import styles from './styles.scss'

const messages = defineMessages({
  title: {
    id: 'JobLogDialog.Title',
    defaultMessage: '{jobName} actual log'
  },
  titleArchive: {
    id: 'JobLogDialog.Archive.Title',
    defaultMessage: '{jobName} archive logs'
  },
  defaultEmptyMessage: {
    id: 'AppComponents.EmptyTableMessage.DefaultEmptyMessage',
    defaultMessage: 'No data'
  },
  copyLinkFeedback:{
    id: 'JobLogDialog.Copy.Feedback.Title',
    defaultMessage: 'The log has been copied to the clipboard.'
  }
})

const closeDialog = (mergeInUIState, dialogKey) => {
  mergeInUIState({
    key: dialogKey,
    value: {
      isOpen: false
    }
  })
}

const CopyLink = compose(
  withCopyToClipboard
)(() => null)

const JobLogDialog = ({
  title,
  description,
  mergeInUIState,
  intl: { formatMessage },
  id,
  forceJob,
  jobName,
  summary,
  isLoading,
  showArchiveLogs,
  ...rest
}) => {
  const dialogKey = 'logJobDialog'
  const handleCancelAction = () => {
    closeDialog(mergeInUIState, dialogKey)
  }
  return (
    <ModalDialog
      name={dialogKey}
      title={showArchiveLogs ? formatMessage(messages.titleArchive, { jobName }) : formatMessage(messages.title, { jobName })}
      contentLabel='Job log dialog'
      style={{ content: { maxWidth: '60rem', minWidth: 'auto' } }}
    >
      <ModalContent>
        {summary ? <CopyLink
          link
          value={summary}
          type='button'
          title={formatMessage(commonAppMessages.copyLink)}
          feedbackMessage={messages.copyLinkFeedback}
          className={styles.copyLink}
        /> : null}
        {isLoading
          ? <Spinner />
          : (
            <pre className={styles.log}>
              <code>{summary || formatMessage(messages.defaultEmptyMessage)}</code>
            </pre>
          )}
      </ModalContent>
      <ModalActions>
        <Button
          small
          onClick={handleCancelAction}
          children={formatMessage(buttonMessages.close)}
        />
      </ModalActions>
    </ModalDialog>
  )
}

JobLogDialog.propTypes = {
  title: PropTypes.node,
  description: PropTypes.node,
  mergeInUIState: PropTypes.func,
  intl: intlShape,
  id: PropTypes.string,
  jobName: PropTypes.string,
  forceJob: PropTypes.func,
  summary: PropTypes.string,
  isLoading: PropTypes.bool,
  showArchiveLogs: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(state => {
    const id = getLogJobId(state)
    const jobDescription = getAdminTasksEntityByCode(state, { code: id, taskType: adminTaskTypesEnum.SCHEDULEDTASKS })
    const showArchiveLogs = getLogJobArchive(state)
    const content = showArchiveLogs ? 'archiveSummary' : 'summary'
    const summary = jobDescription.get(content)
    return {
      id,
      jobName: getLogJobName(state),
      summary: summary && summary.size && JSON.stringify(summary, null, 2).replace(/\\r\\n/g, ' '),
      isLoading: getLoadJobIsFetching(state, { taskType: adminTaskTypesEnum.SCHEDULEDTASKS }),
      showArchiveLogs
    }
  }, {
    mergeInUIState,
    forceJob
  })
)(JobLogDialog)
