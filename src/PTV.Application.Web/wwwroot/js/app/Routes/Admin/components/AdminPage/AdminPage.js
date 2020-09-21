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
import React, { PureComponent, createElement } from 'react'
import PropTypes from 'prop-types'
import { Accordion } from 'appComponents/Accordion'
import withBubbling from 'util/redux-form/HOC/withBubbling'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl } from 'util/react-intl'
import { adminTaskTypes, adminTaskTypesEnum } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'
import {
  getAdminTaskCount,
  getNumbersIsFetching,
  getIsAdminTaskLoadingByType
} from 'Routes/Admin/selectors'
import { performOnSectionButtonAction, downloadLogs } from 'Routes/Admin/actions'
import { messages } from 'Routes/Admin/messages'
import AccordionTitle from 'Routes/sharedComponents/AccordionTitle'
import AccordionHeader from 'Routes/sharedComponents/AccordionHeader'
import SectionTitle from 'appComponents/SectionTitle'
import { FailedTranslationOrders, TaskSchedulerActions } from '../AdminTasks'
import withState from 'util/withState'
import { Button } from 'sema-ui-components'

const renderDownloadLink = (downloadAction, title) => {
  const handleOnCLick = e => {
    e.stopPropagation()
    downloadAction()
  }
  return (
    <Button className
      small
      secondary
      onClick={handleOnCLick}>
      {title}
    </Button>
  )
}

const TaskAccordionTitle = compose(
  injectIntl,
  connect((state, { taskType }) => ({
    count: getAdminTaskCount(state, { taskType }),
    isFetching: getIsAdminTaskLoadingByType(state, { taskType })
  }), {
    performOnSectionButtonAction,
    downloadLogs
  })
)(({
  label,
  count,
  tooltip,
  taskType,
  performOnSectionButtonAction,
  showSectionButton,
  intl: { formatMessage },
  isFetching
}) => {
  const handleClick = e => {
    e.stopPropagation()
    performOnSectionButtonAction(taskType)
  }
  const isButtonDisabled = count === 0
  return (
    <AccordionTitle
      label={label}
      count={count}
      tooltip={tooltip}
      isButtonVisible={showSectionButton}
      isButtonDisabled={isButtonDisabled || isFetching}
      onButtonClick={handleClick}
      buttonTitle={showSectionButton && messages[`${taskType}SectionButtonTitle`]}
      customComponentRender={taskType === adminTaskTypesEnum.SCHEDULEDTASKS && !isFetching ? () => renderDownloadLink(downloadLogs, formatMessage(messages.scheduledTasksDownloadButtonTitle)) : null}
    />
  )
})

const TaskAccordionHeader = ({
  onTabClick,
  taskType,
  activeType,
  label,
  tooltip,
  contentComponent,
  columnWidths,
  showSectionButton
}) => {
  return (
    <AccordionHeader
      titleComponent={(
        <TaskAccordionTitle
          taskType={taskType}
          label={label}
          tooltip={tooltip}
          showSectionButton={showSectionButton}
        />
      )}
      contentComponent={createElement(contentComponent, { activeType, taskType, columnWidths })}
      active={activeType === taskType}
      validate={false}
      arrowSize={40}
      type={taskType}
      keyProp={taskType}
      onTabClick={onTabClick}
      stretchTitle
      contentClassName={styles.tasksAccContent}
      titleClassName={styles.tasksAccTitle}
    />
  )
}

TaskAccordionHeader.propTypes = {
  onTabClick: PropTypes.func.isRequired,
  taskType: PropTypes.string.isRequired,
  label: PropTypes.object.isRequired,
  activeType: PropTypes.string,
  tooltip: PropTypes.object,
  contentComponent: PropTypes.any,
  columnWidths: PropTypes.array,
  showSectionButton: PropTypes.bool
}

class AdminPage extends PureComponent {
  handleTabOnChange = (_, { type, rest }) => {
    const isAlreadyOpen = this.props.activeType === type
    this.props.updateUI({
      activeType: isAlreadyOpen
        ? null
        : type
    })
  }
  render () {
    const { activeType } = this.props
    return (
      <div className={styles.tasks}>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.adminTranslationOrdersSection} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <TaskAccordionHeader
              taskType={adminTaskTypesEnum.FAILEDTRANSLATIONORDERS}
              onTabClick={this.handleTabOnChange}
              label={messages.failedTranslationOrdersTitle}
              tooltip={messages.failedTranslationOrdersTooltip}
              activeType={activeType}
              contentComponent={FailedTranslationOrders}
              // columnWidths={['15%', '30%', '20%', '15%', '20%']}
              showSectionButton
            />
          </Accordion>
        </div>
        <div className={styles.taskSection}>
          <SectionTitle title={messages.adminScheduledTasksSection} />
          <Accordion light activeIndex={-1} className={styles.stripe}>
            <TaskAccordionHeader
              taskType={adminTaskTypesEnum.SCHEDULEDTASKS}
              onTabClick={this.handleTabOnChange}
              label={messages.scheduledTasksTitle}
              tooltip={messages.scheduledTasksTooltip}
              activeType={activeType}
              contentComponent={TaskSchedulerActions}
              showSectionButton
            />
          </Accordion>
        </div>
      </div>
    )
  }
}

AdminPage.propTypes = {
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
    key: 'adminType',
    initialState: {
      activeType: null
    }
  }),
  withBubbling()
)(AdminPage)
