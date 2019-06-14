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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import cx from 'classnames'
import { connect } from 'react-redux'
import TasksBasic from '../TasksBasic'
import styles from './styles.scss'
import { Tabs, Tab, Label } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'
import { default as LabelWithCounter } from 'appComponents/Label'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import NotificationRoute from 'Routes/Tasks/components/NotificationRoute/NotificationRoute'
import { getNotificationsCount, getAreNotificationsEmpty } from 'Routes/Tasks/selectors/notifications'
import { getTasksSummaryCount, getAreTasksEmpty } from 'Routes/Tasks/selectors'
import withState from 'util/withState'

const messages = defineMessages({
  mainTitle: {
    id: 'Routes.Tasks.Main.Header.Title',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  mainTooltip: {
    id: 'Routes.Tasks.Main.Header.Tooltip',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  emptyTasksTitle: {
    id: 'Routes.Tasks.Main.EmptyTasks.Title',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  emptyNotificationsTitle: {
    id: 'Routes.Tasks.Main.EmptyNotifications.Title',
    defaultMessage: 'Nämä tehtävät nousevat automaattisesti PTV-järjestelmästä.'
  },
  tasksTabName: {
    id: 'Routes.Tasks.Main.Tab.Name',
    defaultMessage: 'Tehtävät'
  },
  notificationsTabName: {
    id: 'Routes.Tasks.Main.Notifications.Tab.Name',
    defaultMessage: 'Ilmoitukset'
  }
})

class TasksRoute extends Component {
  handleTabOnChange = index => this.props.updateUI('activeTabIndex', index)
  render () {
    const {
      intl: { formatMessage },
      areTasksEmpty,
      areNotificationsEmpty,
      activeTabIndex,
      notificationsCount,
      tasksCount
    } = this.props
    const formClass = cx(
      styles.form,
      styles.tasks
    )
    return (
      <div className={formClass}>
        <div className={styles.tasksLabel}>
          <Label labelText={formatMessage(messages.mainTitle)} labelPosition='top'>
            <Tooltip tooltip={formatMessage(messages.mainTooltip)} />
          </Label>
        </div>
        <div>
          <Tabs index={activeTabIndex} onChange={this.handleTabOnChange} className={styles.taskTabs}>
            <Tab
              label={(
                <LabelWithCounter
                  label={formatMessage(messages.tasksTabName)}
                  count={tasksCount}
                />
              )}
            >
              {areTasksEmpty
                ? (
                  <Label
                    labelText={formatMessage(messages.emptyTasksTitle)}
                    labelPosition='top'
                  />
                )
                : <TasksBasic />}
            </Tab>
            <Tab
              label={(
                <LabelWithCounter
                  label={formatMessage(messages.notificationsTabName)}
                  count={notificationsCount}
                />
              )}
            >
              {areNotificationsEmpty
                ? (
                  <Label
                    labelText={formatMessage(messages.emptyNotificationsTitle)}
                    labelPosition='top'
                  />
                )
                : <NotificationRoute />}
            </Tab>
          </Tabs>
        </div>
      </div>
    )
  }
}
TasksRoute.propTypes = {
  intl: intlShape,
  areNotificationsEmpty: PropTypes.bool.isRequired,
  activeTabIndex: PropTypes.number,
  notificationsCount: PropTypes.number,
  tasksCount: PropTypes.number,
  updateUI: PropTypes.func,
  areTasksEmpty: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    areTasksEmpty: getAreTasksEmpty(state, ownProps),
    areNotificationsEmpty: getAreNotificationsEmpty(state, ownProps),
    notificationsCount: getNotificationsCount(state, ownProps),
    tasksCount: getTasksSummaryCount(state, ownProps)
  })),
  withPreviewDialog,
  withState({
    redux: true,
    initialState: {
      activeTabIndex: 0
    }
  })
)(TasksRoute)
