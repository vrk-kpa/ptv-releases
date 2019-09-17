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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getAdminTasksEntitiesJS,
  getIsAdminTaskLoadingByType,
  getAdminTaskIsEmptyByType,
  getAdminTaskCount,
  getVisibleAdminTaskSchedulerResultByTypeJS
} from 'Routes/Admin/selectors'
import { getAdminTaskColumnDefinition } from 'Routes/Admin/columnDefinition'
import EntityForm from 'Routes/Tasks/components/EntityForm'
import { createLoadAdminTaskEntities } from 'Routes/Admin/actions'
import { adminTaskTypesEnum } from 'enums'

class AdminTask extends Component {
  loadAdminTaskEntities = createLoadAdminTaskEntities(this.props.taskType)
  render () {
    const {
      taskType,
      count,
      activeType,
      isLoading,
      rows,
      columnWidths,
      ...rest
    } = this.props
    return (
      <EntityForm form={`adminTask${taskType}`}
        getColumnsDefinition={getAdminTaskColumnDefinition(taskType)}
        count={count}
        isLoading={isLoading}
        isActive={activeType === taskType}
        searchResults={rows}
        loadEntities={this.loadAdminTaskEntities}
        columnWidths={columnWidths}
        taskType={taskType}
        {...rest}
      />
    )
  }
}

AdminTask.propTypes = {
  rows: PropTypes.oneOfType([
    PropTypes.array,
    ImmutablePropTypes.list
  ]),
  isLoading: PropTypes.bool.isRequired,
  taskType: PropTypes.string.isRequired,
  activeType: PropTypes.string,
  count: PropTypes.number.isRequired,
  columnWidths: PropTypes.array
}

export default compose(
  connect(
    (state, ownProps) => ({
      rows: ownProps.taskType === adminTaskTypesEnum.SCHEDULEDTASKS
        ? getVisibleAdminTaskSchedulerResultByTypeJS(state, ownProps)
        : getAdminTasksEntitiesJS(state, ownProps),
      isLoading: getIsAdminTaskLoadingByType(state, ownProps),
      isEmpty: getAdminTaskIsEmptyByType(state, ownProps),
      count: getAdminTaskCount(state,
        { taskType: ownProps.taskType })
    })),
)(AdminTask)
