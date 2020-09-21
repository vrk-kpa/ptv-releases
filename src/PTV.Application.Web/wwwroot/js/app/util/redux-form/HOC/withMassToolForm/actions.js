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
import {
  setReviewCurrentStep,
  setContentLanguage,
  clearContentLanguage,
  setSelectedEntity
} from 'reducers/selections'
import { getReviewCurrentStep } from 'selectors/selections'
import { mergeInUIState, setInUIState } from 'reducers/ui'
import { formTypesEnum, getEntityInfo, massToolTypes, taskTypesEnum } from 'enums'
import { formValueSelector, change, initialize, submit, getFormValues } from 'redux-form/immutable'
import {
  getPublishingStatusPublishedId,
  getPublishingStatusModifiedId
} from 'selectors/common'
import { getPreviousLanguagesForApprovedEntities } from 'selectors/massTool'
import { getMassToolType } from 'Routes/Search/components/MassTool/selectors'
import { Map, fromJS } from 'immutable'
import { merger } from 'Middleware/index'
import { push } from 'connected-react-router'
import {
  getShowReviewBar,
  getLanguageStatus,
  getInitialValuesForLatestPublishedSearch,
  getInitialValuesForCopiedSearch,
  getInitialValuesForRestoredSearch,
  getStepIndexMaping,
  getCanBeItemRestore,
  getEntityLanguages
} from './selectors'
import { getFormName, getSelectedEntityId, getEntity } from 'selectors/entities/entities'
import { getIsReadOnly } from 'selectors/formStates'
import { unLockEntity } from 'actions'
import { hideMessageByCode } from 'reducers/notifications'
import { EntitySelectors } from 'selectors'
import { TasksSchemas } from 'schemas/tasks'
import { createLoadTaskEntities } from 'Routes/Tasks/actions'
import { EntitySchemas } from 'schemas'

const navigateToEntity = (newStep, id, currentItem, dispatch, mappedStepIndex) => {
  if (id !== currentItem.get('id')) {
    dispatch(change(formTypesEnum.MASSTOOLFORM, `review[${mappedStepIndex}].id`, id))
  }
  dispatch(setReviewCurrentStep(newStep))
  // reset bottomReached
  dispatch(mergeInUIState({
    key: 'entityReview',
    value: {
      reviewed: false
    }
  }))
  dispatch(push(
    currentItem.getIn(['meta', 'path']) + '/' + id, {
      includeValidation: true
    }
  ))
}

export const moveToStep = (newStep = 0) => ({ getState, dispatch }) => {
  const mappedStepIndex = getStepIndexMaping(getState(), { formName: formTypesEnum.MASSTOOLFORM }).get(newStep)
  const currentItem = formValueSelector(formTypesEnum.MASSTOOLFORM)(getState(), `review[${mappedStepIndex}]`)
  if (currentItem) {
    dispatch(setContentLanguage({ id: currentItem.get('languageId'), code: currentItem.get('language') }))
    navigateToEntity(newStep, currentItem.get('id'), currentItem, dispatch, mappedStepIndex)
  }
  window.scrollTo(0, 0)
}

export const moveToNextStep = props => ({ getState, dispatch }) => {
  const currentStep = getReviewCurrentStep(getState())
  dispatch(moveToStep(currentStep + 1))
}

export const switchToEditableVersion = ({ id, editableVersion }) => ({ getState, dispatch }) => {
  const reviewList = formValueSelector(formTypesEnum.MASSTOOLFORM)(getState(), `review`)
  const editableVersionLanguages = getEntityLanguages(getState(), { id: editableVersion })
  const publishedVersionLanguages = getEntityLanguages(getState(), { id })

  // replacing language versions from editable version
  // 1. get the index where to put the new language versions
  const insertIndex = reviewList.findIndex(x => x.get('id') === id)
  // 2. remove the old language versions
  const filteredList = reviewList.filter(x => x.get('id') !== id)
  // 3. get the beginning part of the review list
  const beginningPart = filteredList.slice(0, insertIndex)
  // 4. update the ending part of the review list with index and step shifted accordingly
  const shiftIndex = editableVersionLanguages.size - publishedVersionLanguages.size
  const endingPart = filteredList.slice(insertIndex).map(x => x
    .set('index', x.get('index') + shiftIndex)
    .set('step', x.get('step') + shiftIndex))
  // 5. prepare the middle part of the review list
  const editableEntity = getEntity(getState(), { id })
  const meta = getEntityInfo(editableEntity.get('entityType'), editableEntity.get('subEntityType'))
  let editableVersionIndex = 0
  const middlePart = editableVersionLanguages.map((value, key) => {
    const newItem = Map({
      id: editableVersion,
      index: insertIndex + editableVersionIndex,
      unificRootId: editableEntity.get('unificRootId'),
      language: key,
      languageId: value.get('languageId'),
      meta: Map(meta),
      useForReview: true,
      step: insertIndex + editableVersionIndex
    })
    editableVersionIndex = editableVersionIndex + 1
    return newItem
  })
  // 6. create the new list from all parts
  const newList = beginningPart.concat(middlePart).concat(endingPart)
  dispatch(change(formTypesEnum.MASSTOOLFORM, 'review', newList))
  // 7. move to the first language of editable version
  dispatch(moveToStep(insertIndex))
}

export const fixReviewAfterSave = ({ id, languagesAvailabilities }) => ({ dispatch, getState }) => {
  const state = getState()
  const isInReview = getShowReviewBar(state)
  if (isInReview) {
    const currentReviewStep = getReviewCurrentStep(state)
    const reviewList = formValueSelector(formTypesEnum.MASSTOOLFORM)(getState(), `review`)
    const currentId = reviewList.getIn([currentReviewStep, 'id'])
    const errors = languagesAvailabilities.reduce((langs, lang) => {
      langs[lang.languageId] = lang.canBePublished
      return langs
    }, {})
    const newList = reviewList.map(x => {
      if (x.get('id') === currentId) {
        return x.merge({ id, approved: errors[x.get('languageId')] && x.get('approved') })
      }
      return x
    })
    dispatch(
      change(
        formTypesEnum.MASSTOOLFORM,
        `review`,
        newList
      )
    )
  }
}

export const hideReviewBar = setReviewCurrentStep

export const cancelReview = () => ({ dispatch, getState }) => {
  const state = getState()
  const massToolType = getMassToolType(state, { formName: formTypesEnum.MASSTOOLSELECTIONFORM })
  if (massToolType === massToolTypes.PUBLISH) {
    const returnPath = formValueSelector(formTypesEnum.MASSTOOLFORM)(state, 'returnPath')
    const formName = getFormName(state)
    const isReadOnly = getIsReadOnly(state, { formName })
    if (!isReadOnly) {
      const entityId = getSelectedEntityId(state)
      dispatch(unLockEntity(entityId, formName))
    }
    dispatch(hideReviewBar())
    dispatch(clearContentLanguage())
    dispatch(setSelectedEntity({ id: null }))
    dispatch(push(returnPath || '/'))
  }
}

export const refreshArchivedTasks = () => ({ dispatch, getState }) => {
  const state = getState()
  const massToolType = getMassToolType(state, { formName: formTypesEnum.MASSTOOLSELECTIONFORM })
  if (massToolType === massToolTypes.RESTORE) {
    const inTaskPage = formValueSelector(formTypesEnum.MASSTOOLFORM)(state, 'inTaskPage')
    if (inTaskPage) {
      dispatch(createLoadTaskEntities(
        taskTypesEnum.CONTENTARCHIVED,
        TasksSchemas.GET_ENTITIES(EntitySchemas.SEARCH)
      )(getState(), dispatch, { form: 'taskEntitiesFormcontentArchived' }))
      dispatch(createLoadTaskEntities(
        taskTypesEnum.OUTDATEDDRAFTSERVICES,
        TasksSchemas.GET_ENTITIES(EntitySchemas.SEARCH)
      )(getState(), dispatch, { form: 'taskEntitiesFormoutdatedDraftServices' }))
      dispatch(createLoadTaskEntities(
        taskTypesEnum.OUTDATEDDRAFTCHANNELS,
        TasksSchemas.GET_ENTITIES(EntitySchemas.SEARCH)
      )(getState(), dispatch, { form: 'taskEntitiesFormoutdatedDraftChannels' }))
    }
  }
}
const getPreviousStatus = (state, item) =>
  getLanguageStatus(state, { id: item.get('id'), type: item.getIn(['meta', 'type']), language: item.get('language') })

const getNewStatus = (state, item, previousPublishedLanguages, publishedId, modifiedId) => {
  const previousStatus = !item.get('approved') && getPreviousStatus(state, item)
  if (item.get('approved') ||
    // languages not included in review by filtered language, but was published in current published version
    (!item.get('useForReview') && previousPublishedLanguages.hasIn([item.get('unificRootId'), item.get('languageId')]))
  ) {
    return publishedId
  }
  // if not approved and previous status is published then changed to modified
  return previousStatus === publishedId && modifiedId || previousStatus
}

export const callMassPublish = (values, invoke) => ({ getState, dispatch }) => {
  const state = getState()
  const psId = getPublishingStatusPublishedId(state)
  const modifiedId = getPublishingStatusModifiedId(state)
  const previousPublishedLanguages = getPreviousLanguagesForApprovedEntities(
    state, {
      formName: formTypesEnum.MASSTOOLFORM
    }
  )
  const validFrom = values.get('publishAt')
  const validTo = values.get('archiveAt')
  const data = values.get('review').reduce((requestData, item) => {
    return requestData.mergeWith(merger, fromJS({
      [item.getIn(['meta', 'type'])]: {
        [item.get('id')]: {
          id: item.get('id'),
          languagesAvailabilities: [{
            languageId: item.get('languageId'),
            statusId: getNewStatus(state, item, previousPublishedLanguages, psId, modifiedId)
          }]
        }
      }
    }))
  },
  Map()
  ).map(
    x => x.filter(res => res.get('languagesAvailabilities').some(la => la.get('statusId') === psId)).toList()
  ).update(
    'validFrom', (value = validFrom) => value
  ).update(
    'validTo', (value = validTo) => value
  )
  invoke('MassPublish', data)
}

const prepareRequestData = selected => {
  return selected.filter(x => x && x.get('approved')).reduce((requestData, item, key) => {
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

export const callMassCopy = (values, invoke) => () => {
  const organization = values.get('organization')
  const data = prepareRequestData(values.get('selected'))
  const copyData = data.set('organization', organization)
  invoke('MassCopy', copyData)
}

export const callMassArchive = (values, invoke) => () => {
  const validTo = values.get('archiveAt')
  const data = prepareRequestData(values.get('selected'))
  const archiveData = data.set('validTo', validTo)
  invoke('MassArchive', archiveData)
}

export const callMassRestore = (values, invoke) => ({ getState, dispatch }) => {
  const state = getState()
  const allSelectedWithoutModifiedItems = values.get('selected').filter(x => x &&
    getCanBeItemRestore(state, { id: x.get('unificRootId') }))
  const data = prepareRequestData(allSelectedWithoutModifiedItems)
  invoke('MassRestore', data)
}

export const searchLatestPublished = () => ({ dispatch, getState }) => {
  const state = getState()
  const initialValues = getInitialValuesForLatestPublishedSearch(state)
  dispatch(setInUIState({
    key: 'uiData',
    path: 'sorting',
    value: Map()
  }))
  dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, initialValues))
  dispatch(submit(formTypesEnum.FRONTPAGESEARCH))
}

export const searchCoppied = () => ({ dispatch, getState }) => {
  const state = getState()
  const initialValues = getInitialValuesForCopiedSearch(state)
  dispatch(setInUIState({
    key: 'uiData',
    path: 'sorting',
    value: Map()
  }))
  dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, initialValues))
  dispatch(submit(formTypesEnum.FRONTPAGESEARCH))
}

export const searchRestored = () => ({ dispatch, getState }) => {
  const state = getState()
  let initialValues = getInitialValuesForRestoredSearch(state)
  let formValues = getFormValues(formTypesEnum.MASSTOOLFORM)(state)
  const prevEntities = EntitySelectors.previousInfos.getEntities(state)
  let selectedEntities = formValues.get('selected').filter(x => x)
  selectedEntities = selectedEntities.filter(x => {
    const previousInfo = prevEntities.get(x.get('unificRootId')) || Map()
    return !previousInfo.get('lastModifiedId')
  })
  initialValues = initialValues.set('entityIds', selectedEntities.filter(x => x).map(x => x.get('unificRootId')).toArray())

  dispatch(setInUIState({
    key: 'uiData',
    path: 'sorting',
    value: Map()
  }))
  dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, initialValues))
  dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', selectedEntities))
  dispatch(submit(formTypesEnum.FRONTPAGESEARCH))
  dispatch(hideMessageByCode({ key: 'massTool', code : 'MassTool.Restore.Success' }))
}

export const searchTaskRestored = () => ({ dispatch, getState }) => {
  const state = getState()
  let formValues = getFormValues(formTypesEnum.MASSTOOLFORM)(state)
  const prevEntities = EntitySelectors.previousInfos.getEntities(state)
  let selectedEntities = formValues.get('selected').filter(x => x)
  selectedEntities = selectedEntities.filter(x => {
    const previousInfo = prevEntities.get(x.get('unificRootId')) || Map()
    return !previousInfo.get('lastModifiedId')
  })
  dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'selected', selectedEntities))
  dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'type', massToolTypes.PUBLISH))
  dispatch(mergeInUIState({
    key: 'taskType',
    value: {
      massToolTabAction: massToolTypes.PUBLISH
    }
  }))
  dispatch(hideMessageByCode({ key: 'massTool', code : 'MassTool.Restore.Success' }))
}
