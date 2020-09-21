
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
import { compose } from 'redux'
import { connect } from 'react-redux'
import AdminTask from '../AdminTask'
import { getActiveDetailEntityIds, getAdminTasksEntityByCode } from 'Routes/Admin/selectors'
import { adminTaskTypesEnum } from 'enums'
import ForceJobDialog from 'Routes/Admin/components/dialogs/ForceJobDialog'
import JobLogDialog from 'Routes/Admin/components/dialogs/JobLogDialog'
import PropTypes from 'prop-types'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Label, Button } from 'sema-ui-components'
import { mergeInUIState } from 'reducers/ui'
import { getJob } from 'Routes/Admin/actions'
import styles from './styles.scss'
import Spacer from 'appComponents/Spacer'
import Paragraph from 'appComponents/Paragraph'
import moment from 'moment'
// import withState from 'util/withState'
// import { List } from 'immutable'

const messages = defineMessages({
  scheduledTasksRowDescriptionTitle: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.Title',
    defaultMessage: 'Job description'
  },
  scheduledTasksRowDescriptionState: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.JobState.Title',
    defaultMessage: 'State of the job'
  },
  scheduledTasksRowDescriptionExecution: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.JobFailedExecution.Title',
    defaultMessage: 'Count of failed executions'
  },
  scheduledTasksRowDescriptionRetries: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.JobRetries.Title',
    defaultMessage: 'Number of retries when failed'
  },
  scheduledTasksRowDescriptionExecutionTime: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.JobExecutionTime.Title',
    defaultMessage: 'Execution time'
  },
  scheduledTasksRowDescriptionRunning: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.JobRunning.Title',
    defaultMessage: 'Job is runnig'
  },
  scheduledTasksRowDescriptionNo: {
    id: 'Common.Components.No.Title',
    defaultMessage: 'Ei'
  },
  scheduledTasksRowDescriptionYes: {
    id: 'Common.Components.Yes.Title',
    defaultMessage: 'Kyllä'
  },
  scheduledTasksRowLogLink: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.LogLink.Title',
    defaultMessage: 'Actual Log'
  },
  scheduledTasksRowArchiveLogsLink: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.ArchiveLogsLink.Title',
    defaultMessage: 'Archive Logs'
  },
  scheduledTasksRowDescriptionLogs: {
    id: 'AdminTasks.ScheduledTasks.RowDescription.Logs.Title',
    defaultMessage: 'Job logs'
  }
})

const RowDetailLabel = ({
  value,
  label,
  children
}) => {
  return (
    <div className='row'>
      <div className='col-lg-10'>
        <em>{label}</em>
      </div>
      {value !== undefined && <div className='col-lg-7'>
        <span>{value}</span>
      </div>}
      <div className='col-lg-7'>
        {children}
      </div>
    </div>
  )
}
RowDetailLabel.propTypes = {
  value: PropTypes.any,
  label: PropTypes.string
}
const RowDetail = compose(
  injectIntl,
  connect((state, { code, ...rest }) => {
    const activeIds = getActiveDetailEntityIds(state, { taskType: adminTaskTypesEnum.SCHEDULEDTASKS })
    const jobData = getAdminTasksEntityByCode(state, { code, taskType: adminTaskTypesEnum.SCHEDULEDTASKS })
    const nameId = jobData.getIn(['name'])
    const description = jobData.get('description')
    const lastExecutionTime = jobData.get('lastExecutionTime')
    return {
      isVisible: activeIds.includes(code),
      state: jobData.get('state'),
      execution: jobData.get('countOfFailedExecutions'),
      retries: jobData.get('retriesOnFails'),
      isRunning: jobData.get('isRunning'),
      estimationTime: moment.utc(lastExecutionTime).isValid() && moment.utc(lastExecutionTime).format('HH:mm:ss.SSS'),
      code,
      jobName: nameId && rest.intl.formatMessage(nameId.toJS()) || '',
      jobDescription: description && description.toJS() || null
    }
  }, {
      mergeInUIState,
      getJob
    })
)(({
  isVisible,
  intl: { formatMessage },
  state,
  execution,
  estimationTime,
  retries,
  isRunning,
  code,
  jobName,
  jobDescription,
  mergeInUIState,
  getJob
}) => {
  const handleOnLogJobClick = (archive) => {
    getJob(code)
    mergeInUIState({
      key: 'logJobDialog',
      value: {
        isOpen: true,
        id: code,
        jobName,
        archive: archive
      }
    })
  }
  return isVisible && (
    <div className={styles.rowDetail}>
      <Spacer marginSize='m0' />
      <Label labelText={formatMessage(messages.scheduledTasksRowDescriptionTitle)} />
      <Paragraph>{formatMessage(jobDescription)}</Paragraph>
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionRunning)} value={isRunning ? formatMessage(messages.scheduledTasksRowDescriptionYes) : formatMessage(messages.scheduledTasksRowDescriptionNo)} />
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionState)} value={state} />
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionExecution)} value={execution} />
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionRetries)} value={retries} />
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionExecutionTime)} value={estimationTime} />
      <RowDetailLabel label={formatMessage(messages.scheduledTasksRowDescriptionLogs)} >
        <Button className={styles.jobRowLink}
          link
          onClick={() => handleOnLogJobClick(false)} >
          {formatMessage(messages.scheduledTasksRowLogLink)}
        </Button>
        <Button className={styles.jobRowLink}
          link
          onClick={() => handleOnLogJobClick(true)} >
          {formatMessage(messages.scheduledTasksRowArchiveLogsLink)}
        </Button>
      </RowDetailLabel>
    </div>
  )
})

const TaskSchedulerActions = props => {
  return (
    <div>
      <AdminTask {...props} customRowComponent={RowDetail} useCustomTable useUISorting />
      <ForceJobDialog />
      <JobLogDialog />
    </div>
  )
}

export default compose(
  // withState({
  //   redux: true,
  //   keepImmutable: true,
  //   key: adminTaskTypesEnum.SCHEDULEDTASKS,
  //   initialState: {
  //     activeDetailEntityIds: List()
  //   }
  // })
)(TaskSchedulerActions)
