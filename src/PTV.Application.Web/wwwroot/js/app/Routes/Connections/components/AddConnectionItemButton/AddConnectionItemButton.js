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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { arrayPush, arrayInsert, getFormValues, initialize, change } from 'redux-form/immutable'
import { fromJS, List } from 'immutable'
import { createSelector } from 'reselect'
import {
  getConnectionsMainEntity,
  getConnectionsEntity,
  getConnectionsActiveEntities,
  getConnectionsAddToAllEntities
} from 'selectors/selections'
import {
  setConnectionsMainEntity
} from 'reducers/selections'
import { getParameterFromProps } from 'selectors/base'
import AddButtonActive from './AddButtonActive'
import AddButtonDisabled from './AddButtonDisabled'
import { withFormStates } from 'util/redux-form/HOC'
import {
  getResults,
  getResultsIds,
  getPriorityResult,
  getTypePriority,
  getMainEntityConnections,
  getInsertIndex
} from 'Routes/Connections/selectors'

const getWorkbenchConnections = createSelector(
  getFormValues('connectionsWorkbench'),
  formValues => (formValues && formValues.get('connections')) || List()
)

const getIsActiveAddButton = createSelector(
  [
    getWorkbenchConnections,
    getConnectionsMainEntity,
    getConnectionsEntity,
    getParameterFromProps('unificRootId'),
    getConnectionsAddToAllEntities,
    getConnectionsActiveEntities
  ],
  (connections, main, selected, id, shouldAddToAll, activeIndexes) => {
    if (!shouldAddToAll && main !== selected) {
      connections = connections.filter((_, index) => activeIndexes.includes(index))
    }
    if (!main || main === selected) {
      return !connections.some(x => x.getIn(['mainEntity', 'unificRootId']) === id)
    } else {
      return connections
        .filter(x => {
          const childs = x.get('childs').concat(x.get('astiChilds'))
          return !childs
            .some(z => z.get('unificRootId') === id)
        }).size > 0
    }
  }
)

const getIsActiveAllButton = createSelector(
  [
    getWorkbenchConnections,
    getConnectionsMainEntity,
    getConnectionsEntity,
    getResultsIds,
    getConnectionsAddToAllEntities,
    getConnectionsActiveEntities
  ],
  (connections, main, selected, ids, shouldAddToAll, activeIndexes) => {
    if (!shouldAddToAll && main !== selected) {
      connections = connections.filter((_, index) => activeIndexes.includes(index))
    }
    if (!main || main === selected) {
      const mainIds = connections.map(connection => connection.getIn(['mainEntity', 'unificRootId'])).toSet()
      return ids.some(id => !mainIds.contains(id))
    } else {
      return connections
        .filter(x => {
          const childIds = (x.get('childs').concat(x.get('astiChilds'))).map(child => child.get('unificRootId')).toSet()
          return ids
            .some(id => !childIds.contains(id))
        }).size > 0
    }
  }
)

const AddConnectionItemButton = ({
  item,
  all,
  addItem,
  insertItem,
  change,
  formName,
  isActive,
  setMainEntity,
  activeMainEntitiesIndexes,
  dispatch,
  makeCurrentFormStateInitial,
  shouldAddToAll
}) => {
  const addMainEntity = (workBench, newItem) => {
    if (!workBench.some(x => x.getIn(['mainEntity', 'unificRootId']) === newItem.unificRootId)) {
      addItem(formName, 'connections', createAstiChilds(newItem))
    }
    makeCurrentFormStateInitial()
  }
  const addChildEntity = (workBench, newItem) => {
    dispatch(({ getState }) => {
      const state = getState()
      workBench
        .map((connection, index) => {
          if (!shouldAddToAll && !activeMainEntitiesIndexes.includes(index)) return
          if (!connection.get('childs').some(x => x.get('unificRootId') === newItem.unificRootId) &&
              !connection.get('astiChilds').some(x => x.get('unificRootId') === newItem.unificRootId)) {
            const insertIndex = getInsertIndex(state, { connections:connection.get('childs'), typeId:newItem.channelTypeId })
            if (insertIndex > -1) {
              insertItem(formName, `connections[${index}].childs`, insertIndex, fromJS(newItem))
            } else {
              addItem(formName, `connections[${index}].childs`, fromJS(newItem))
            }
          }
        })
    })
  }
  const createAstiChilds = (item) => {
    let childs
    dispatch(({ getState }) => {
      const state = getState()
      childs = getMainEntityConnections(state, { id:item.id })
    })
    const groupedChilds = childs.groupBy(child =>
      child.getIn(['astiDetails', 'isASTIConnection'])
        ? 'asti'
        : 'nonAsti'
    )
    return fromJS({
      mainEntity: item,
      childs: groupedChilds.get('nonAsti') || [],
      astiChilds: groupedChilds.get('asti') || []
    })
  }
  const isPrioritized = (childs, priority) => {
    let isSorted = false
    if (priority) {
      const priorityLine = childs.map(ch => priority.get(ch.get('channelTypeId'))).toArray()
      let lastPriority = 0
      for (let element of priorityLine) {
        isSorted = lastPriority <= element
        lastPriority = element
        if (!isSorted) break
      }
    }
    return isSorted
  }
  const priotityMerge = (workConnections, newConnections, priority) => {
    let priorityResult = List()
    priority.map((pv, pk) => {
      priorityResult = priorityResult.concat(workConnections.filter(wc => wc.get('channelTypeId') === pk))
      priorityResult = priorityResult.concat(newConnections.filter(wc => wc.get('channelTypeId') === pk))
    })
    return priorityResult
  }

  const childMerge = (workBench, newItems, priority) => {
    workBench.map((connection, index) => {
      if (!shouldAddToAll && !activeMainEntitiesIndexes.includes(index)) return
      let workIds = connection.get('childs').map(v => v.get('unificRootId'))
      const astiIds = connection.get('astiChilds').map(v => v.get('unificRootId'))
      workIds = workIds.concat(astiIds)
      const toMerge = newItems.filter(newItem => !workIds.contains(newItem.get('unificRootId')))
      const result = priority
      ? isPrioritized(connection.get('childs'), priority)
       ? priotityMerge(connection.get('childs'), toMerge, priority)
       : connection.get('childs').concat(toMerge)
      : connection.get('childs').concat(toMerge)
      change(formName, `connections[${index}].childs`, fromJS(result))
    })
  }

  const parentMerge = (workBench, newItems) => {
    const workIds = workBench.map(v => v.getIn(['mainEntity', 'unificRootId']))
    const toMerge = newItems.filter(newItem => !workIds.contains(newItem.get('unificRootId')))
    const astiResult = toMerge.map(v => createAstiChilds(v.toJS()))
    const result = workBench.concat(astiResult)
    change(formName, 'connections', result)
  }

  const onClick = () => {
    let workBench, isMainItem, mainEntity, selectedEntity
    dispatch(({ getState }) => {
      const state = getState()
      mainEntity = getConnectionsMainEntity(state)
      selectedEntity = getConnectionsEntity(state)
      workBench = getWorkbenchConnections(state)
      isMainItem = mainEntity === selectedEntity
    })

    if (workBench.size === 0) {
      setMainEntity(selectedEntity)
      change(formName, 'mainEntityType', selectedEntity)
      isMainItem = true
    }

    if (all) {
      let allItems, priorityTable
      dispatch(({ getState }) => {
        const state = getState()
        const mergeChildChannels = selectedEntity === 'channels' && !isMainItem
        allItems = mergeChildChannels ? getPriorityResult(state) : getResults(state)
        priorityTable = mergeChildChannels ? getTypePriority(state) : null
      })
      isMainItem
        ? parentMerge(workBench, allItems)
        : childMerge(workBench, allItems, priorityTable)
    } else if (isMainItem) {
      addMainEntity(workBench, item)
    } else {
      addChildEntity(workBench, item)
    }
  }
  return isActive
    ? <AddButtonActive onClick={onClick} />
    : <AddButtonDisabled />
}

AddConnectionItemButton.propTypes = {
  item: PropTypes.object,
  isActive: PropTypes.bool,
  all: PropTypes.bool,
  addItem: PropTypes.func,
  insertItem: PropTypes.func,
  change: PropTypes.func,
  formName: PropTypes.string,
  dispatch: PropTypes.func.isRequired,
  setMainEntity: PropTypes.func.isRequired,
  activeMainEntitiesIndexes: PropTypes.object,
  makeCurrentFormStateInitial: PropTypes.func.isRequired,
  shouldAddToAll: PropTypes.bool.isRequired
}

export default compose(
  withFormStates,
  connect((state, { item, all }) => {
    const isActive = all ? getIsActiveAllButton(state) : (item && getIsActiveAddButton(state, { unificRootId:item.unificRootId }))
    return {
      formName: 'connectionsWorkbench',
      activeMainEntitiesIndexes: getConnectionsActiveEntities(state),
      shouldAddToAll: getConnectionsAddToAllEntities(state),
      isActive
    }
  }, {
    addItem: arrayPush,
    insertItem: arrayInsert,
    change,
    setMainEntity: setConnectionsMainEntity,
    makeCurrentFormStateInitial: () => ({ dispatch, getState }) => {
      const state = getState()
      const getCurrentFormValues = getFormValues('connectionsWorkbench')
      const formValues = getCurrentFormValues(state)
      dispatch(initialize('connectionsWorkbench', formValues, true))
    }
  })
)(AddConnectionItemButton)
