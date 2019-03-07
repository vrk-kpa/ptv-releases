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
import { setReviewCurrentStep, setContentLanguage } from 'reducers/selections'
import { getReviewCurrentStep } from 'selectors/selections'
import { mergeInUIState } from 'reducers/ui'
import { formTypesEnum, getEntityInfo, massToolTypes } from 'enums'
import { formValueSelector, change } from 'redux-form/immutable'
import {
  getPublishingStatusPublishedId,
  getPublishingStatusModifiedId
} from 'selectors/common'
import { getMassToolType } from 'Routes/Search/components/MassTool/selectors'
import { Map, fromJS } from 'immutable'
import { merger } from 'Middleware/index'
import { push } from 'connected-react-router'
import { apiCall3 } from 'actions'
import { EntitySchemas } from 'schemas'
import { getEditableVersionId, getShowReviewBar, getLanguageStatus } from './selectors'
import { property } from 'lodash'

const navigateToEntity = (newStep, id, currentItem, dispatch) => {
  if (id !== currentItem.get('id')) {
    dispatch(change(formTypesEnum.MASSTOOLFORM, `review[${newStep}].id`, id))
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
  const currentItem = formValueSelector(formTypesEnum.MASSTOOLFORM)(getState(), `review[${newStep}]`)
  if (currentItem) {
    dispatch(setContentLanguage({ id: currentItem.get('languageId'), code: currentItem.get('language') }))
    navigateToEntity(newStep, currentItem.get('id'), currentItem, dispatch)
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

    // const selectedLanguage = getContentLanguageId(state)
    // const selectedEntity = getEntity(state)
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
  const store = getState()
  const massToolType = getMassToolType(store, { formName: formTypesEnum.MASSTOOLSELECTIONFORM })
  if (massToolType === massToolTypes.PUBLISH) {
    dispatch(hideReviewBar())
    dispatch(push('/frontpage/search'))
  }
}

const getPreviousStatus = (item, state) =>
  getLanguageStatus(state, { id: item.get('id'), type: item.getIn(['meta', 'type']), language: item.get('language') })

export const callMassPublish = (values, hubConnection) => ({ getState, dispatch }) => {
  const psId = getPublishingStatusPublishedId(getState())
  const modifiedId = getPublishingStatusModifiedId(getState())
  const validFrom = values.get('publishAt')
  const validTo = values.get('archiveAt')
  const data = values.get('review').reduce((requestData, item) => {
    const previousStatus = !item.get('approved') && getPreviousStatus(item, getState())
    return requestData.mergeWith(merger, fromJS({
      [item.getIn(['meta', 'type'])]: {
        [item.get('id')]: {
          id: item.get('id'),
          languagesAvailabilities: [{
            languageId: item.get('languageId'),
            statusId: item.get('approved')
              ? psId
              // if not approved and previous status is published then changed to modified
              : previousStatus === psId && modifiedId || previousStatus
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
  hubConnection.invoke('MassPublish', data).catch(err => console.error(err.toString()))
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

export const callMassCopy = (values, hubConnection) => () => {
  const organization = values.get('organization')
  const data = prepareRequestData(values.get('selected'))
  const copyData = data.set('organization', organization)
  hubConnection.invoke('MassCopy', copyData).catch(err => console.error(err.toString()))
}

export const callMassArchive = (values, hubConnection) => () => {
  const validTo = values.get('archiveAt')
  const data = prepareRequestData(values.get('selected'))
  const archiveData = data.set('validTo', validTo)
  hubConnection.invoke('MassArchive', archiveData).catch(err => console.error(err.toString()))
}
