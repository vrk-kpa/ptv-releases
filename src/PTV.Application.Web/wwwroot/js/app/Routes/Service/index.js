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
import ServiceForm from 'Routes/Service/components/ServiceForm'
import { entityConcreteTypesEnum, formTypesEnum } from 'enums'
import { withRouteInit } from 'util/helpers/baseRouteInit'
import { mergeInUIState } from 'reducers/ui'
import { deleteApiCall } from 'actions'
import { compose } from 'redux'
import { property } from 'lodash'
import EntitySelectors from 'selectors/entities/entities'
import { enableForm, setIsAddingNewLanguage } from 'reducers/formStates'
import { qualityEntityCheckIfNotRun } from 'actions/qualityAgent'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'

const init = (store, nextState) => {
  const { dispatch } = store
  dispatch(deleteApiCall(['service', entityConcreteTypesEnum.GENERALDESCRIPTION, 'search']))
  dispatch(mergeInUIState({
    key: 'generalDescriptionSelectContainer',
    value: {
      isCollapsed: true
    }
  }))
  const generalDescriptionId = property('location.state.gd')(nextState)
  if (generalDescriptionId) {
    const isLoaded = EntitySelectors.generalDescriptions
      .getIsEntityLoaded(store.getState(), { id: generalDescriptionId })
    isLoaded && [
      setIsAddingNewLanguage({ form: formTypesEnum.SERVICEFORM, value: false }),
      enableForm(formTypesEnum.SERVICEFORM)
    ].forEach(store.dispatch)
  }
}

const successCallback = (_, messages, { data }, { dispatch, getState }) => {
  const isInReview = getShowReviewBar(getState())
  if (isInReview && data) {
    dispatch(qualityEntityCheckIfNotRun({ id: data.id, unificRootId: data.unificRootId }, 'service'))
  }
}

export default compose(
  withRouteInit({
    init,
    successCallback,
    entityConcreteType: entityConcreteTypesEnum.SERVICE
  })
)(ServiceForm)
