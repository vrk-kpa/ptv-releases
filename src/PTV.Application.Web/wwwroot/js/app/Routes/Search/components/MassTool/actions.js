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
import { getEntitiesWithGivenType } from 'selectors/entities/entities'
import {
  getEntityInfo,
  formTypesEnum,
  massToolTypes,
  timingTypes
} from 'enums'

export const confirmMassPublishSelection = ({ formName, selected }) => ({ dispatch, getState }) => {
  const reviewEntities = getEntitiesForReview(getState(), { selected, formName })
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'review', reviewEntities))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.PUBLISH))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'timingType', timingTypes.NOW))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'publishAt', null))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'archiveAt', null))
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

export const confirmMassArchiveSelection = ({ selected }) => ({ dispatch }) => {
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'selected', selected))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'type', massToolTypes.ARCHIVE))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'timingType', timingTypes.NOW))
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'archiveAt', null))
  showMassDialog(dispatch)
}

export const resetDisabled = (massToolType) => ({ dispatch, getState }) => {
  const state = getState()
  const selected = getSelectedEntitiesForMassOperation(state).filter(x => x)
  const filtered = selected.filter((item, key) => {
    const entityType = item.get('entityType')
    const subEntityType = item.get('subEntityType')
    const type = getEntityInfo(entityType, subEntityType).type
    const entities = getEntitiesWithGivenType(state, { type })
    const entity = entities.get(item.get('id'))
    const organizationId = entity.get('organizationId')
    const unificRootId = entity.get('unificRootId')
    const languagesAvailabilities = entity.get('languagesAvailabilities')
    const publishingStatusId = entity.get('publishingStatusId')
    const canBeSelected = !getDisabledForRow(state, {
      languagesAvailabilities: languagesAvailabilities.toJS(),
      massToolType,
      id: key,
      publishingStatusId,
      entityType,
      subEntityType,
      organizationId,
      unificRootId
    })
    return canBeSelected
  })
  dispatch(
    change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', filtered)
  )
}

export const clearSelection = () => ({ dispatch, getState }) => {
  dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', Map()))
  dispatch(mergeInUIState({
    key: 'MassToolBatchSelect',
    value: {
      checked: false
    }
  }))
}

export const batchSelection = (checked) => ({ dispatch, getState }) => {
  const state = getState()
  const selected = getSelectedEntitiesForMassOperation(state)
  const searchResultItems = getDomainSearchIds(state)
  const batchSelected = searchResultItems.reduce((acc, curr) => {
    const entityId = curr.get('id')
    const type = curr.get('schema')
    const entities = getEntitiesWithGivenType(state, { type })
    const entity = entities.get(entityId)
    const entityType = entity.get('entityType')
    const subEntityType = entity.get('subEntityType')
    const organizationId = entity.get('organizationId')
    const unificRootId = entity.get('unificRootId')
    const languagesAvailabilities = entity.get('languagesAvailabilities')
    const publishingStatusId = entity.get('publishingStatusId')
    const massToolType = getMassToolType(state, { formName: formTypesEnum.MASSTOOLSELECTIONFORM })
    const canBeSelected = !getDisabledForRow(state, {
      languagesAvailabilities: languagesAvailabilities.toJS(),
      massToolType,
      id: entityId,
      publishingStatusId,
      entityType,
      subEntityType,
      organizationId,
      unificRootId
    })
    return acc.mergeWith(merger, canBeSelected && fromJS({
      [massToolType === massToolTypes.ARCHIVE ? entityId : unificRootId]: {
        id: entityId,
        unificRootId,
        entityType,
        subEntityType
      }
    }) || null)
  }, Map()).filter(x => x)
  dispatch(
    change(
      formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', checked
        ? selected.filter((v, k) => !batchSelected.has(k))
        : selected.merge(batchSelected)
    )
  )
}
