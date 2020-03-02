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
import { change } from 'redux-form/immutable'
import {
  getEntitiesForReview,
  getSelectedEntitiesForMassOperation,
  getDisabledForRow,
  getMassToolType
} from './selectors'
import { moveToStep } from 'util/redux-form/HOC/withMassToolForm/actions'
import { mergeInUIState } from 'reducers/ui'
import { merger } from 'Middleware/index'
import {
  fromJS,
  Map
} from 'immutable'
import { getDomainSearchIds } from 'Routes/Search/selectors'
import { getTasksEntitiesIds } from 'Routes/Tasks/selectors'
import { getEntitiesWithGivenType, getEntityAvailableLanguages } from 'selectors/entities/entities'
import {
  getEntityInfo,
  formTypesEnum,
  massToolTypes,
  timingTypes,
  taskTypesDefinition
} from 'enums'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'

export const confirmMassPublishSelection = ({ formName, selected, returnPath }) => ({ dispatch, getState }) => {
  const reviewEntities = getEntitiesForReview(getState(), { selected, formName })
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'review', reviewEntities))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.PUBLISH))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'timingType', timingTypes.NOW))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'publishAt', null))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'archiveAt', null))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'returnPath', returnPath))
  dispatch(moveToStep(0))
}

const showMassDialog = (dispatch) => {
  dispatch(mergeInUIState({
    key: 'MassToolDialog',
    value: {
      isOpen: true
    }
  }))
}

export const confirmMassCopySelection = ({ selected, organization }) => ({ dispatch }) => {
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'selected', selected))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.COPY))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'organization', organization))
  showMassDialog(dispatch)
}

const prepareRequestData = selected => {
  return selected.filter(x => x).reduce((requestData, item, key) => {
    const meta = getEntityInfo(item.get('entityType'), item.get('subEntityType'))
    return requestData.mergeWith(merger, fromJS({
      [meta.type]: {
        [key]: {
          id: item.get('id'),
          unificRootId: item.get('unificRootId')
        }
      }
    }))
  }, Map()
  ).map(
    x => x.toList()
  )
}

export const confirmMassRestoreSelection = ({ selected }) => ({ dispatch }) => {
  const data = prepareRequestData(selected)
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'selected', selected))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.RESTORE))
  dispatch(
    apiCall3({
      keys: [formTypesEnum.MASSTOOLFORM, 'lastModifiedInfo'],
      payload: {
        endpoint: 'common/GetLastModifiedInfo',
        data: data && data.toJS()
      },
      schemas: EntitySchemas.LAST_MODIFIED_INFO_ARRAY,
      successNextAction: showMassDialog(dispatch)
    })
  )
}

export const confirmMassArchiveSelection = ({ selected }) => ({ dispatch }) => {
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'selected', selected))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.ARCHIVE))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'timingType', timingTypes.NOW))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'archiveAt', null))
  showMassDialog(dispatch)
}

export const resetDisabled = (massToolType) => ({ dispatch, getState }) => {
  const formName = formTypesEnum.MASSTOOLSELECTIONFORM
  const state = getState()
  const selected = getSelectedEntitiesForMassOperation(state, { formName }).filter(x => x)
  const filtered = selected.filter((item, key) => {
    const entityType = item.get('entityType')
    const subEntityType = item.get('subEntityType')
    const type = getEntityInfo(entityType, subEntityType).type
    const entities = getEntitiesWithGivenType(state, { type })
    const entity = entities.get(item.get('id'))
    const organizationId = entity.get('organizationId')
    const unificRootId = entity.get('unificRootId')
    const languagesAvailabilities = getEntityAvailableLanguages(state, { id: item.get('id') })
    const publishingStatusId = entity.get('publishingStatusId')
    const hasTranslationOrder = entity.get('hasTranslationOrder')
    const canBeSelected = !getDisabledForRow(state, {
      languagesAvailabilities: languagesAvailabilities.toJS(),
      massToolType,
      id: key,
      publishingStatusId,
      entityType,
      subEntityType,
      organizationId,
      unificRootId,
      hasTranslationOrder
    })
    return canBeSelected
  })
  dispatch(
    change(formName, 'selected', filtered)
  )
}

export const clearSelection = () => ({ dispatch, getState }) => {
  dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', Map()))
}

export const batchSelection = (checked, taskType) => ({ dispatch, getState }) => {
  const state = getState()
  const formName = formTypesEnum.MASSTOOLSELECTIONFORM
  const selected = getSelectedEntitiesForMassOperation(state, { formName })
  const searchResultItems = taskTypesDefinition.includes(taskType)
    ? getTasksEntitiesIds(state, { taskType })
    : getDomainSearchIds(state)
  const batchSelected = searchResultItems.reduce((acc, curr) => {
    const entityId = curr.get('id')
    const type = curr.get('schema')
    const entities = getEntitiesWithGivenType(state, { type })
    const entity = entities.get(entityId)
    const entityType = entity.get('entityType')
    const subEntityType = entity.get('subEntityType')
    const organizationId = entity.get('organizationId')
    const unificRootId = entity.get('unificRootId')
    const languagesAvailabilities = getEntityAvailableLanguages(state, { id: entityId })
    const publishingStatusId = entity.get('publishingStatusId')
    const hasTranslationOrder = entity.get('hasTranslationOrder')
    const hasAstiConnection = entity.get('hasAstiConnection')
    const massToolType = getMassToolType(state, { formName })
    const approved = true // select all values in summary masstool dialog
    const canBeSelected = !getDisabledForRow(state, {
      languagesAvailabilities: languagesAvailabilities.toJS(),
      massToolType,
      id: entityId,
      publishingStatusId,
      entityType,
      subEntityType,
      organizationId,
      unificRootId,
      hasTranslationOrder,
      hasAstiConnection
    })
    return acc.mergeWith(merger, canBeSelected && fromJS({
      [massToolType === massToolTypes.ARCHIVE ? entityId : unificRootId]: {
        id: entityId,
        unificRootId,
        entityType,
        subEntityType,
        approved
      }
    }) || null)
  }, Map()).filter(x => x)
  dispatch(
    change(
      formName, 'selected', checked
        ? selected.filter((v, k) => !batchSelected.has(k))
        : selected.merge(batchSelected)
    )
  )
}
