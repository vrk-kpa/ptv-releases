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
import {
  setReviewCurrentStep,
  setContentLanguage,
  clearContentLanguage,
  setSelectedEntity
} from 'reducers/selections'
import { getReviewCurrentStep } from 'selectors/selections'
import { mergeInUIState, setInUIState } from 'reducers/ui'
import { formTypesEnum, getEntityInfo, massToolTypes } from 'enums'
import { formValueSelector, change, initialize, submit } from 'redux-form/immutable'
import {
  getPublishingStatusPublishedId,
  getPublishingStatusModifiedId
} from 'selectors/common'
import { getPreviousPublishedLanguagesForApprovedEntities } from 'selectors/massTool'
import { getMassToolType } from 'Routes/Search/components/MassTool/selectors'
import { Map, fromJS } from 'immutable'
import { merger } from 'Middleware/index'
import { push } from 'connected-react-router'
import {
  getShowReviewBar,
  getLanguageStatus,
  getInitialValuesForLatestPublishedSearch,  
  getInitialValuesForCopiedSearch,
  getStepIndexMaping,
  getCanBeItemRestore
} from './selectors'
import { getFormName, getSelectedEntityId } from 'selectors/entities/entities'
import { getIsReadOnly } from 'selectors/formStates'
import { unLockEntity } from 'actions'

const navigateToEntity = (newStep, id, currentItem, dispatch, mappedStepIndex) => {
  if (id !== currentItem.get('id')) {
    dispatch(change(formTypesEnum.MASSTOOLFORM, `review[${mappedStepIndex}].id`, id))
  }
  dispatch(setReviewCurrentStep(newStep))
  // reset bottomReached
  dispatch(mergeInUIState({
    key: 'pageBody',
    value: {
      bottomReached: false,
      scrollProgress: 0,
      scrollTop: 0
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
}

export const moveToNextStep = props => ({ getState, dispatch }) => {
  const currentStep = getReviewCurrentStep(getState())
  dispatch(moveToStep(currentStep + 1))
}

export const switchToEditableVersion = ({ id, editableVersion }) => ({ getState, dispatch }) => {
  const reviewList = formValueSelector(formTypesEnum.MASSTOOLFORM)(getState(), `review`)
  const currentStep = getReviewCurrentStep(getState())

  const newList = reviewList.map(x => {
    if (x.get('id') === id) {
      return x.set('id', editableVersion)
    }
    return x
  })

  dispatch(change(formTypesEnum.MASSTOOLFORM, 'review', newList))
  dispatch(moveToStep(currentStep))
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
  const previousPublishedLanguages = getPreviousPublishedLanguagesForApprovedEntities(
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