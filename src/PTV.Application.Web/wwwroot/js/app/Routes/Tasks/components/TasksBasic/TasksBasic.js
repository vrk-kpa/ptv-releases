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
import React, { PureComponent, createElement } from 'react'
import PropTypes from 'prop-types'
import { Accordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import { taskTypes, taskTypesEnum } from 'enums'
import styles from './styles.scss'
import {
  getTasksTypeCount,
  getNumbersIsFetching,
  getIsPostponeButtonVisible,
  getPostponeIsFetching
} from '../../selectors'
import { TasksSchemas } from 'schemas/tasks'
import { EntitySchemas } from 'schemas'
import { postponeTasksEntities } from '../../actions'
import AccordionTitle from '../AccordionTitle'
import AccordionHeader from '../AccordionHeader'
import TranslationArrived from 'Routes/Tasks/components/TranslationArrived'
import ChannelsWithoutServices from 'Routes/Tasks/components/ChannelsWithoutServices'
import ServicesWithoutChannels from 'Routes/Tasks/components/ServicesWithoutChannels'
import OutdatedDraftChannels from 'Routes/Tasks/components/OutdatedDraftChannels'
import OutdatedDraftServices from 'Routes/Tasks/components/OutdatedDraftServices'
import OutdatedPublishedChannels from 'Routes/Tasks/components/OutdatedPublishedChannels'
import OutdatedPublishedServices from 'Routes/Tasks/components/OutdatedPublishedServices'
import MissingLanguageOrganizations from 'Routes/Tasks/components/MissingLanguageOrganizations'
import withState from 'util/withState'

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
  },
  translationArrivedTitle: {
    id: 'Routes.Tasks.Notification.TranslationArrived.Title',
    defaultMessage: 'Saapuneet käännöstilaukset ({count})'
  },
  missingLangugageOrganizationsTitle: {
    id: 'Routes.Tasks.MissingLangugageOrganizations.Title',
    defaultMessage: 'Organisaatiokuvauksen kieliversioita puuttuu ({count})'
  }
})

const infoIconMessages = defineMessages({
  outdatedDraftServices: {
    id: 'Routes.Tasks.OutdatedDraftServices.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedPublishedServices: {
    id: 'Routes.Tasks.OutdatedPublishedServices.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  servicesWithoutChannels: {
    id: 'Routes.Tasks.ServicesWithoutChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedDraftChannels: {
    id: 'Routes.Tasks.OutdatedDraftChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  outdatedPublishedChannels: {
    id: 'Routes.Tasks.OutdatedPublishedChannels.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  channelsWithoutServices: {
    id: 'Routes.Tasks.ChannelsNotConnectedToAnyService.Main.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  translationArrived: {
    id: 'Routes.Tasks.Notification.TranslationArrived.InfoIcon',
    defaultMessage: 'Info icon text'
  },
  missingLangugageOrganizations: {
    id: 'Routes.Tasks.MissingLangugageOrganizations.InfoIcon',
    defaultMessage: 'Info icon text'
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
    case taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS:
      schema = EntitySchemas.ORGANIZATION
  }

  return schema
}

const TaskAccordionTitle = compose(
  injectIntl,
  connect((state, ownProps) => ({
    isPostponing: getPostponeIsFetching(state, { taskType: ownProps.taskType }),
    count: getTasksTypeCount(state,
      { taskType: taskTypes[ownProps.taskType] }),
    isButtonVisible: getIsPostponeButtonVisible(state,
      { taskType: taskTypes[ownProps.taskType] })
  }), { postponeTasksEntities }))(
  ({ label, count, tooltip, isPostponing, postponeTasksEntities, taskType, isButtonVisible }) => {
    const handleClick = (e) => {
      postponeTasksEntities(taskType,
        TasksSchemas.GET_ENTITIES(getSchema(taskType)))
    }
    return (
      <AccordionTitle
        label={label}
        count={count}
        tooltip={tooltip}
        isButtonVisible={isButtonVisible}
        isButtonLoading={isPostponing}
        onButtonClick={handleClick}
        buttonTitle={messages.postponeButtonTitle}
      />
    )
  })

const TaskAccordionHeader =
  ({ onTabClick, taskType, activeType, label, tooltip, contentComponent, columnWidths }) => {
    return (<AccordionHeader
      titleComponent={(<TaskAccordionTitle
        taskType={taskType}
        label={label}
        tooltip={tooltip}
      />)}
      contentComponent={createElement(contentComponent, { activeType, taskType, columnWidths })}
      active={activeType === taskType}
      validate={false}
      arrowSize={40}
      type={taskType}
      onClick={onTabClick}
      stretchTitle
      contentClassName={styles.tasksAccContent}
      titleClassName={styles.tasksAccTitle}
    />
    )
  }

TaskAccordionHeader.propTypes = {
  onTabClick: PropTypes.func.isRequired,
  taskType: PropTypes.string.isRequired,
  label: PropTypes.string.isRequired,
  activeType: PropTypes.string.isRequired,
  tooltip: PropTypes.string.isRequired,
  contentComponent: PropTypes.object,
  columnWidths: PropTypes.array
}

class TasksBasic extends PureComponent {
  handleTabOnChange = (_, { type }) => {
    const isAlreadOpened = this.props.activeType === type
    this.props.updateUI({
      activeType: isAlreadOpened
        ? null
        : type
    })
  }

  render () {
    const {
      activeType
    } = this.props

    return (
      <div className={styles.tasks}>
        <Accordion light activeIndex={-1}>
          <TaskAccordionHeader
            taskType={taskTypesEnum.OUTDATEDDRAFTSERVICES}
            onTabClick={this.handleTabOnChange}
            label={messages.outdatedDraftServices}
            tooltip={infoIconMessages.outdatedDraftServices}
            activeType={activeType}
            contentComponent={OutdatedDraftServices}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDSERVICES}
            onTabClick={this.handleTabOnChange}
            label={messages.outdatedPublishedServices}
            tooltip={infoIconMessages.outdatedPublishedServices}
            activeType={activeType}
            contentComponent={OutdatedPublishedServices}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.OUTDATEDDRAFTCHANNELS}
            onTabClick={this.handleTabOnChange}
            label={messages.outdatedDraftChannels}
            tooltip={infoIconMessages.outdatedDraftChannels}
            activeType={activeType}
            contentComponent={OutdatedDraftChannels}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.OUTDATEDPUBLISHEDCHANNELS}
            onTabClick={this.handleTabOnChange}
            label={messages.outdatedPublishedChannels}
            tooltip={infoIconMessages.outdatedPublishedChannels}
            activeType={activeType}
            contentComponent={OutdatedPublishedChannels}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.SERVICESWITHOUTCHANNELS}
            onTabClick={this.handleTabOnChange}
            label={messages.servicesWithoutChannels}
            tooltip={infoIconMessages.servicesWithoutChannels}
            activeType={activeType}
            contentComponent={ServicesWithoutChannels}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.CHANNELSWITHOUTSERVICES}
            onTabClick={this.handleTabOnChange}
            label={messages.channelsWithoutServices}
            tooltip={infoIconMessages.channelsWithoutServices}
            activeType={activeType}
            contentComponent={ChannelsWithoutServices}
          />

          <TaskAccordionHeader
            taskType={taskTypesEnum.TRANSLATIONARRIVED}
            onTabClick={this.handleTabOnChange}
            label={messages.translationArrivedTitle}
            tooltip={infoIconMessages.translationArrived}
            activeType={activeType}
            contentComponent={TranslationArrived}
          />
          <TaskAccordionHeader
            taskType={taskTypesEnum.MISSINGLANGUAGEORGANIZATIONS}
            onTabClick={this.handleTabOnChange}
            label={messages.missingLangugageOrganizationsTitle}
            tooltip={infoIconMessages.missingLangugageOrganizations}
            activeType={activeType}
            contentComponent={MissingLanguageOrganizations}
            columnWidths={['20%', '30%', '25%', '25%']}
          />
        </Accordion>
      </div>
    )
  }
}

TasksBasic.propTypes = {
  updateUI: PropTypes.func.isRequired,
  activeType: PropTypes.string
}

export default compose(
  injectIntl,
  connect(
    (state, ownProps) => {
      return {
        isLoading: getNumbersIsFetching(state)
      }
    }
  ),
  withState({
    redux: true,
    initialState: {
      activeType: null
    }
  }),
  withBubbling()
)(TasksBasic)
