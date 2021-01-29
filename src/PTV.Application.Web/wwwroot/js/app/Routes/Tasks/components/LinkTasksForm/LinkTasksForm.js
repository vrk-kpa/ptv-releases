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
import EntityForm from 'Routes/Tasks/components/EntityForm'
import { getTNColumnsDefinition } from 'Routes/Tasks/components/columnsDefinition'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getBrokenLinkEntitiesJS,
  getTasksIsFetching
} from 'Routes/Tasks/selectors'
import { taskTypes } from 'enums'
import { TasksSchemas } from 'schemas/tasks'
import { createLoadTaskEntities } from 'Routes/Tasks/actions'
import { EntitySchemas } from 'schemas'

const LinkTasksForm = props => {
  const {
    taskType,
    columnWidths,
    isLoading,
    searchResults,
    activeType,
    customRowComponent
  } = props

  const getColumnsDefinition = getTNColumnsDefinition(taskType, false)
  const loadTaskEntities = createLoadTaskEntities(taskType, TasksSchemas.GET_ENTITIES(EntitySchemas.BROKEN_LINK))
  const isActive = Array.isArray(activeType)
    ? activeType.includes(taskType)
    : activeType === taskType
  return (<EntityForm form={`taskEntitiesForm${taskType}`}
    getColumnsDefinition={getColumnsDefinition}
    columnWidths={columnWidths}
    isLoading={isLoading}
    isActive={isActive}
    searchResults={searchResults}
    loadEntities={loadTaskEntities}
    taskType={taskType}
    customRowComponent={customRowComponent}
    useCustomTable />
  )
}

LinkTasksForm.propTypes = {
  taskType: PropTypes.string,
  columnWidths: PropTypes.array,
  isLoading: PropTypes.bool,
  searchResults: PropTypes.array,
  customRowComponent: PropTypes.any,
  activeType: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.array
  ])
}

export default compose(
  connect(
    (state, ownProps) => {
      const taskType = ownProps.taskType
      return {
        isLoading: getTasksIsFetching(state, { taskType: taskTypes[taskType] }),
        searchResults: getBrokenLinkEntitiesJS(state, { taskType: taskTypes[taskType] })
      }
    }
  ),
)(LinkTasksForm)
