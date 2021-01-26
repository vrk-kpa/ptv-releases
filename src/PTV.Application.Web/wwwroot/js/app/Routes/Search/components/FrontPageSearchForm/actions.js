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
import { API_CALL_CLEAN } from 'actions'
import { formTypesEnum, massToolTypes, massToolSectionTypesEnum } from 'enums'
import {
  change,
  getFormValues,
  initialize,
  submit
} from 'redux-form/immutable'
import { getFrontPageInitialValues } from '../../selectors'
import { getPublishingStatusDeletedId } from 'selectors/common'
import { getSelectedLanguage } from 'Intl/Selectors'
import { getUiSortingData } from 'selectors/base'
import { fetchEntities } from '../../actions'
import { clearSelection } from '../MassTool/actions'
import { getSelectedEntitiesForMassOperation, getSelectedCount } from '../MassTool/selectors'

export const clearForm = formValues => ({ dispatch, getState }) => {
  const state = getState()
  const initialValues = getFrontPageInitialValues(state)
  dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, initialValues))
  dispatch({
    type: API_CALL_CLEAN,
    keys: ['frontPageSearch', 'entities']
  })
}

export const resubmit = formValues => ({ dispatch, getState }) => {
  if (formValues) {
    dispatch(initialize(formTypesEnum.FRONTPAGESEARCH, formValues))
    dispatch(submit(formTypesEnum.FRONTPAGESEARCH))
  }
}

export const searchSubmit = (_, dispatch, { updateUI }) => {
  dispatch(({ dispatch, getState }) => {
    const state = getState()
    let formValues = getFormValues(formTypesEnum.FRONTPAGESEARCH)(state)
    const sortingData = getUiSortingData(getState(), { contentType: 'entities' })
    const publishingStatusDelete = getPublishingStatusDeletedId(state)
    updateUI('frontPageFormState', formValues)
    const selectedPublishingStatuses = formValues.get('selectedPublishingStatuses')
      .filter(value => value)
      .keySeq()
    const language = getSelectedLanguage(state)
    formValues = formValues.set('selectedPublishingStatuses', selectedPublishingStatuses)
    formValues = formValues.set('language', language)
    dispatch(fetchEntities({
      ...formValues.toJS(),
      sortData: sortingData.size > 0 ? [sortingData] : []
    }))
    const massToolCartItems = getSelectedEntitiesForMassOperation(state, {
      formName: formTypesEnum.MASSTOOLSELECTIONFORM
    })
    const cartContentItems = getSelectedCount(state, {
      selected: massToolCartItems,
      formName: formTypesEnum.MASSTOOLSELECTIONFORM
    })

    // Do not switch mass tool if some content has been already selected.
    if (cartContentItems > 0) {
      return
    }

    if (selectedPublishingStatuses.size === 1 && selectedPublishingStatuses.first() === publishingStatusDelete) {
      const massValues = getFormValues(formTypesEnum.MASSTOOLSELECTIONFORM)(state)
      if (massValues && massValues.get('type') !== massToolTypes.RESTORE) {
        dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'type', massToolTypes.RESTORE))
        dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'section', massToolSectionTypesEnum.SEARCH))
        dispatch(clearSelection())
      }
    } else {
      dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'type', massToolTypes.PUBLISH))
      dispatch(change(formTypesEnum.MASSTOOLSELECTIONFORM, 'section', massToolSectionTypesEnum.SEARCH))
    }
  })
}
