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
import { Accordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { taskTypes, taskTypesEnum } from 'enums'
import styles from './styles.scss'
import {
  getTasksTypeCount,
  getNumbersIsFetching,
  getIsPostponeButtonVisible,
  getPostponeIsFetching
} from '../../selectors'
import TasksForm from '../TasksForm'
import { Spinner, Button } from 'sema-ui-components'
import { TasksSchemas } from 'schemas/tasks'
import { EntitySchemas } from 'schemas'
import { postponeTasksEntities } from '../../actions'

const messages = defineMessages({
  outdatedDraftServices: {
    id: 'Routes.Tasks.OutdatedDraftServices.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat luonnostilaiset palvelut ({count})'
  },
  outdatedPublishedServices: {
    id: 'Routes.Tasks.OutdatedPublishedServices.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat julkaistut palvelut ({count})'
  },
  servicesWithoutChannels: {
    id: 'Routes.Tasks.ServicesWithoutChannels.Main.Title',
    defaultMessage: 'Organisaatiosi palvelut, joihin ei ole liitetty yhtään asiointikanavaa ({count})'
  },
  outdatedDraftChannels: {
    id: 'Routes.Tasks.OutdatedDraftChannels.Main.Title',
    defaultMessage: 'Organisaatiosi vanhentuvat luonnostilaiset kanavat ({count})'
  },
  outdatedPublishedChannels: {
    id: 'Routes.Tasks.OutdatedPublishedChannels.Main.Title',
    defaultMessage: 'Organisaatiosi vanhetuvat julkaistut kanavat ({count})'
  },
  channelsWithoutServices: {
    id: 'Routes.Tasks.ChannelsNotConnectedToAnyService.Main.Title',
    defaultMessage: 'Organisaatiosi asiointikanavat, joihin ei ole liitetty yhtään palvelua ({count})'
  },
  postponeButtonTitle: {
    id: 'Routes.Tasks.Accordion.PostponeButton.Title',
    defaultMessage: 'Lykkää tehtävät'
  }
})

const getSchema = (taskType) => {
  let schema = EntitySchemas.SERVICE

  switch (taskType) {
    case taskTypesEnum.OUTDATEDDRAFTSERVICES:
    case taskTypesEnum.OUTDATEDPUBLISHEDSERVICES:
    case taskTypesEnum.CHANNELSWITHOUTSERVICES:
      schema = EntitySchemas.SERVICE
      break
    case taskTypesEnum.OUTDATEDDRAFTCHANNELS:
    case taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS:
    case taskTypesEnum.SERVICESWITHOUTCHANNELS:
      schema = EntitySchemas.CHANNEL
      break
  }

  return schema
}

const LabelComponent = compose(
  injectIntl,
  connect((state, ownProps) => ({
    isPostponing: getPostponeIsFetching(state, { taskType: ownProps.taskType })
  }), { postponeTasksEntities }))(
  ({ label, date, count, isPostponing, taskType, postponeTasksEntities, isButtonVisible, intl: { formatMessage } }) => {
    const handleClick = (e) => {
      postponeTasksEntities(taskType,
        TasksSchemas.GET_ENTITIES(getSchema(taskType)))
    }

    return (
      <div className={styles.tasksTitle}>
        <div>
          {formatMessage(label, { count })}
        </div>
        {isButtonVisible && <div>
          <Button small
            secondary
            disabled={isPostponing}
            onClick={handleClick}>
            {formatMessage(messages.postponeButtonTitle)}
          </Button>
          {isPostponing && <Spinner />}
        </div>}
      </div>
    )
  })

const TasksBasic = ({
  intl: { formatMessage },
  outdatedDraftServices,
  outdatedPublishedServices,
  outdatedDraftChannels,
  outdatedPublishedChannels,
  servicesWithoutChannels,
  channelsWithoutServices,
  isPostponeButtonVisibleDS,
  isPostponeButtonVisiblePS,
  isPostponeButtonVisibleDCH,
  isPostponeButtonVisiblePCH,
  isPostponeButtonVisibleSWCH,
  isPostponeButtonVisibleCHWS
}) => {
  return (
    <div className={styles.tasks}>
      <Accordion light activeIndex={-1}>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.outdatedDraftServices}
            count={outdatedDraftServices}
            isButtonVisible={isPostponeButtonVisibleDS}
            taskType={taskTypesEnum.OUTDATEDDRAFTSERVICES}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.OUTDATEDDRAFTSERVICES}`}
            taskType={taskTypesEnum.OUTDATEDDRAFTSERVICES}
            taskCount={outdatedDraftServices} />
        </Accordion.Content>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.outdatedPublishedServices}
            count={outdatedPublishedServices}
            isButtonVisible={isPostponeButtonVisiblePS}
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDSERVICES}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.OUTDATEDPUBLISHEDSERVICES}`}
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDSERVICES}
            taskCount={outdatedPublishedServices} />
        </Accordion.Content>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.outdatedDraftChannels}
            count={outdatedDraftChannels}
            isButtonVisible={isPostponeButtonVisibleDCH}
            taskType={taskTypesEnum.OUTDATEDDRAFTCHANNELS}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.OUTDATEDDRAFTCHANNELS}`}
            taskType={taskTypesEnum.OUTDATEDDRAFTCHANNELS}
            taskCount={outdatedDraftChannels} />
        </Accordion.Content>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.outdatedPublishedChannels}
            count={outdatedPublishedChannels}
            isButtonVisible={isPostponeButtonVisiblePCH}
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS}`}
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS}
            taskCount={outdatedPublishedChannels} />
        </Accordion.Content>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.servicesWithoutChannels}
            count={servicesWithoutChannels}
            isButtonVisible={isPostponeButtonVisibleSWCH}
            taskType={taskTypesEnum.SERVICESWITHOUTCHANNELS}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.SERVICESWITHOUTCHANNELS}`}
            taskType={taskTypesEnum.SERVICESWITHOUTCHANNELS}
            taskCount={servicesWithoutChannels} />
        </Accordion.Content>
        <Accordion.Title validate={false}
          className={styles.tasksAccTitle}
          arrowSize={40}
          title={<LabelComponent
            label={messages.channelsWithoutServices}
            count={channelsWithoutServices}
            isButtonVisible={isPostponeButtonVisibleCHWS}
            taskType={taskTypesEnum.CHANNELSWITHOUTSERVICES}
            date='21.08.2018' />}
        />
        <Accordion.Content className={styles.tasksAccContent}>
          <TasksForm form={`taskEntitiesForm${taskTypesEnum.CHANNELSWITHOUTSERVICES}`}
            taskType={taskTypesEnum.CHANNELSWITHOUTSERVICES}
            taskCount={channelsWithoutServices} />
        </Accordion.Content>
      </Accordion>
    </div>
  )
}
TasksBasic.propTypes = {
  intl: intlShape.isRequired,
  outdatedDraftServices: PropTypes.number.isRequired,
  outdatedPublishedServices: PropTypes.number.isRequired,
  outdatedDraftChannels: PropTypes.number.isRequired,
  outdatedPublishedChannels: PropTypes.number.isRequired,
  servicesWithoutChannels: PropTypes.number.isRequired,
  channelsWithoutServices: PropTypes.number.isRequired,
  isPostponeButtonVisibleDS: PropTypes.bool.isRequired,
  isPostponeButtonVisiblePS: PropTypes.bool.isRequired,
  isPostponeButtonVisibleDCH: PropTypes.bool.isRequired,
  isPostponeButtonVisiblePCH: PropTypes.bool.isRequired,
  isPostponeButtonVisibleSWCH: PropTypes.bool.isRequired,
  isPostponeButtonVisibleCHWS: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      return {
        isLoading: getNumbersIsFetching(state),
        outdatedDraftServices: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDDRAFTSERVICES] }),
        outdatedPublishedServices: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDSERVICES] }),
        outdatedDraftChannels: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDDRAFTCHANNELS] }),
        outdatedPublishedChannels: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS] }),
        servicesWithoutChannels: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.SERVICESWITHOUTCHANNELS] }),
        channelsWithoutServices: getTasksTypeCount(state,
          { taskType: taskTypes[taskTypesEnum.CHANNELSWITHOUTSERVICES] }),
        isPostponeButtonVisibleDS: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDDRAFTSERVICES] }),
        isPostponeButtonVisiblePS: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDSERVICES] }),
        isPostponeButtonVisibleDCH: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDDRAFTCHANNELS] }),
        isPostponeButtonVisiblePCH: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS] }),
        isPostponeButtonVisibleSWCH: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.SERVICESWITHOUTCHANNELS] }),
        isPostponeButtonVisibleCHWS: getIsPostponeButtonVisible(state,
          { taskType: taskTypes[taskTypesEnum.CHANNELSWITHOUTSERVICES] })
      }
    }
  ),
  withBubbling
)(TasksBasic)
