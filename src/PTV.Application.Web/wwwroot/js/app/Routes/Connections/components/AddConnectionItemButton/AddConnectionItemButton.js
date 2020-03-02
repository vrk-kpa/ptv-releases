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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { arrayPush, arrayInsert, getFormValues, change, arrayRemove, isDirty } from 'redux-form/immutable'
import { fromJS, List } from 'immutable'
import { createSelector } from 'reselect'
import styles from './styles.scss'
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
import withFormStates from 'util/redux-form/HOC/withFormStates'
import {
  getResults,
  getResultsIds,
  getPriorityResult,
  getTypePriority,
  getMainEntityConnections,
  getInsertIndex,
  getIsConnectionsOrganizingActive
} from 'Routes/Connections/selectors'
import {
  makeCurrentFormStateInitial,
  removeSorting
} from 'Routes/Connections/actions'
import { Checkbox } from 'sema-ui-components'

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
  removeItem,
  insertItem,
  change,
  formName,
  isActive,
  setMainEntity,
  activeMainEntitiesIndexes,
  dispatch,
  makeCurrentFormStateInitial,
  shouldAddToAll,
  removeSorting,
  isDirty,
  isOrganizingActive
}) => {
  const addMainEntity = (workBench, newItem) => {
    if (!workBench.some(x => x.getIn(['mainEntity', 'unificRootId']) === newItem.unificRootId)) {
      addItem(formName, 'connections', createAstiChilds(newItem))
    }
    if (!isDirty) {
      makeCurrentFormStateInitial()
    }
  }

  const removeMainEntity = (workBench, item) => {
    workBench.forEach((val, index) => {
      if (val.getIn(['mainEntity', 'unificRootId']) === item.unificRootId) {
        removeItem(formName, 'connections', index)
      }
    })
    if (workBench.size === 1) {
      setMainEntity(null)
      makeCurrentFormStateInitial()
    }
    if (!isDirty) {
      makeCurrentFormStateInitial()
    }
  }

  const removeAllParents = () => {
    change(formName, 'connections', List())
    setMainEntity(null)
    makeCurrentFormStateInitial()
  }

  const addChildEntity = (workBench, newItem) => {
    newItem.isNew = true
    newItem.modified = new Date()
    newItem.modifiedBy = ''
    dispatch(({ getState }) => {
      const state = getState()
      workBench
        .map((connection, index) => {
          if (!shouldAddToAll && !activeMainEntitiesIndexes.includes(index)) return
          if (!connection.get('childs').some(x => x.get('unificRootId') === newItem.unificRootId) &&
              !connection.get('astiChilds').some(x => x.get('unificRootId') === newItem.unificRootId)) {
            const insertIndex = getInsertIndex(state, {
              connections:connection.get('childs'),
              typeId:newItem.channelTypeId
            })
            if (insertIndex > -1) {
              insertItem(formName, `connections[${index}].childs`, insertIndex, fromJS(newItem))
            } else {
              addItem(formName, `connections[${index}].childs`, fromJS(newItem))
            }
          }
        })
    })
  }

  const removeChildEntity = (workBench, item) => {
    workBench.forEach((mainVal, mainIndex) => {
      mainVal.get('childs').forEach((childVal, childIndex) => {
        if (childVal.get('unificRootId') === item.unificRootId) {
          removeItem(formName, `connections[${mainIndex}].childs`, childIndex)
        }
      })
    })
  }

  const removeAllChilds = (workBench, itemsIds) => {
    workBench.forEach((mainVal, mainIndex) => {
      var newChilds = mainVal.get('childs').filter(ch => !itemsIds.contains(ch.get('unificRootId')))
      change(formName, `connections[${mainIndex}].childs`, fromJS(newChilds))
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
    const addDate = new Date()
    workBench.map((connection, index) => {
      if (!shouldAddToAll && !activeMainEntitiesIndexes.includes(index)) return
      let workIds = connection.get('childs').map(v => v.get('unificRootId'))
      const astiIds = connection.get('astiChilds').map(v => v.get('unificRootId'))
      workIds = workIds.concat(astiIds)
      const toMerge = newItems.filter(newItem => !workIds.contains(newItem.get('unificRootId')))
        .map(x => x.set('isNew', true).set('modified', addDate))
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
    if (!isDirty) {
      makeCurrentFormStateInitial()
    }
  }

  const onClick = (add) => {
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
        ? add ? parentMerge(workBench, allItems)
          : removeAllParents()
        : add ? childMerge(workBench, allItems, priorityTable)
          : removeAllChilds(workBench, allItems.map(x => x.get('unificRootId')))
    } else if (isMainItem) {
      add ? addMainEntity(workBench, item) : removeMainEntity(workBench, item)
    } else {
      add ? addChildEntity(workBench, item) : removeChildEntity(workBench, item)
    }
    if (isMainItem) { // remove sorting
      removeSorting({ type: 'main' })
    } else {
      removeSorting({ type: 'sub' })
    }
  }

  return <Checkbox
    className={styles.addButton}
    checked={!isActive}
    onChange={() => onClick(isActive)}
    disabled={isOrganizingActive}
  />
}

AddConnectionItemButton.propTypes = {
  item: PropTypes.object,
  isActive: PropTypes.bool,
  all: PropTypes.bool,
  addItem: PropTypes.func,
  removeItem: PropTypes.func,
  insertItem: PropTypes.func,
  change: PropTypes.func,
  formName: PropTypes.string,
  dispatch: PropTypes.func.isRequired,
  setMainEntity: PropTypes.func.isRequired,
  activeMainEntitiesIndexes: PropTypes.object,
  makeCurrentFormStateInitial: PropTypes.func.isRequired,
  shouldAddToAll: PropTypes.bool.isRequired,
  removeSorting: PropTypes.func.isRequired,
  isDirty: PropTypes.bool.isRequired,
  isOrganizingActive: PropTypes.bool
}

export default compose(
  withFormStates,
  connect((state, { item, all }) => {
    const isActive = all
      ? getIsActiveAllButton(state)
      : (item && getIsActiveAddButton(state, { unificRootId:item.unificRootId }))
    return {
      formName: 'connectionsWorkbench',
      activeMainEntitiesIndexes: getConnectionsActiveEntities(state),
      shouldAddToAll: getConnectionsAddToAllEntities(state),
      isActive,
      isDirty: isDirty('connectionsWorkbench')(state),
      isOrganizingActive: getIsConnectionsOrganizingActive(state)
    }
  }, {
    addItem: arrayPush,
    insertItem: arrayInsert,
    removeItem: arrayRemove,
    change,
    setMainEntity: setConnectionsMainEntity,
    makeCurrentFormStateInitial,
    removeSorting
  })
)(AddConnectionItemButton)
