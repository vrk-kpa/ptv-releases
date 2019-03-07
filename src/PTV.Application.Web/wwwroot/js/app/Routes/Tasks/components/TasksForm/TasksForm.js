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
import EntityForm from 'Routes/Tasks/components/EntityForm'
import { taskTypes } from 'enums'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getTasksServicesEntities,
  getTasksIsFetching,
  getTasksIsMoreAvailable,
  getTaskTypeLoadMoreIsFetching
} from 'Routes/Tasks/selectors'
import { getTNColumnsDefinition } from 'Routes/Tasks/components/columnsDefinition'
import { createLoadTaskEntities } from 'Routes/Tasks/actions'

const TasksForm = ({
  taskType,
  isLoading,
  searchResults,
  activeType,
  isMoreAvailable,
  isLoadMoreFetching,
  columnWidths
}) => {
  const getColumnsDefinition = getTNColumnsDefinition(taskType)
  const loadTaskEntities = createLoadTaskEntities(taskType)

  return (<EntityForm form={`taskEntitiesForm${taskType}`}
    getColumnsDefinition={getColumnsDefinition}
    columnWidths={columnWidths}
    isLoading={isLoading}
    isActive={activeType === taskType}
    searchResults={searchResults}
    isMoreAvailable={isMoreAvailable}
    isLoadMoreFetching={isLoadMoreFetching}
    loadEntities={loadTaskEntities} />
  )
}

TasksForm.propTypes = {
  taskType: PropTypes.string.isRequired,
  isLoading: PropTypes.bool.isRequired,
  searchResults: PropTypes.array.isRequired,
  isMoreAvailable: PropTypes.bool.isRequired,
  activeType: PropTypes.bool.isRequired,
  isLoadMoreFetching: PropTypes.bool.isRequired,
  columnWidths: PropTypes.array
}

export default compose(
  connect(
    (state, ownProps) => {
      return {
        isLoading: getTasksIsFetching(state, { taskType: taskTypes[ownProps.taskType] }),
        searchResults: getTasksServicesEntities(state, { taskType: taskTypes[ownProps.taskType] }),
        isMoreAvailable: getTasksIsMoreAvailable(state, { taskType: taskTypes[ownProps.taskType] }),
        isLoadMoreFetching: getTaskTypeLoadMoreIsFetching(state,
          { taskType: taskTypes[ownProps.taskType] })
      }
    }
  ),
)(TasksForm)
