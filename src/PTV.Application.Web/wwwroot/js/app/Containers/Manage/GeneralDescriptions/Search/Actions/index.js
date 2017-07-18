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
import { List } from 'immutable'

import { CALL_API, Schemas } from '../../../../../Middleware/Api'
import { browserHistory } from 'react-router'

// schemas
import { GeneralDescriptionSchemas } from '../../GeneralDescriptions/Schemas'
import { CommonSchemas } from '../../../../Common/Schemas'

// Actions
import * as CommonActions from '../../../../../Containers/Common/Actions'

// selectors
import * as GeneralDescriptionSearchSelectors from '../Selectors'
import * as CommonGeneralDescriptionSelectors from '../../Common/Selectors'
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'
import * as CommonGeneralDescriptionSearchSelectors from '../../Search/Selectors'
import * as CommonSelectors from '../../../../Common/Selectors'

const keyToState = 'generalDescription'
const keyToEntities = 'generalDescriptions'

export const GENERAL_DESCRIPTION_SEARCH_INPUT_CHANGE = 'GENERAL_DESCRIPTION_SEARCH_INPUT_CHANGE'

export function onGeneralDescriptionSearchInputChange (input, value, isSet) {
  return ({ getState }) => CommonActions.onEntityInputChange('searches', keyToState, input, value, isSet)
}

export function setGenerealDescriptionToService () {
  return ({ getState }) => {
    const state = getState()
    const languageId = CommonSelectors.getLanguageTo(state, { keyToState:'service' })
    const languageCode = CommonSelectors.getTranslationLanguageCode(state, { id: languageId })
    return CommonActions.onLocalizedEntityObjectChange(
			'services',
			CommonServiceSelectors.getServiceId(state),
      {
        generalDescription: CommonGeneralDescriptionSearchSelectors.getGeneralDescriptionSelectedId(state),
        serviceName: CommonGeneralDescriptionSearchSelectors.getSelectedGeneralDescriptionServiceNameLocale(state, { language: languageCode })
      },
			languageCode, true
		)
  }
}

export function onGeneralDescriptionSearchListChange (input, value, isAdd) {
  return ({ getState }) => ({
    type: GENERAL_DESCRIPTION_SEARCH_INPUT_CHANGE,
    payload: { property: ['searches', keyToState, input], item: value, isAdd }
  })
}

export function searchGeneralDescription (isShowMore = false) {
  return (props) => {
    const state = props.getState()
    const model = isShowMore ? { ...GeneralDescriptionSearchSelectors.getOldModelToSearch(state, { keyToState }), prevEntities: CommonSelectors.getSearchedEntities(state, { keyToState, keyToEntities }), [keyToEntities]: List() }
		 	: { ...GeneralDescriptionSearchSelectors.getModelToSearch(state, { keyToState }), prevEntities: List() }
    return CommonActions.apiCall([keyToState, 'searchResults'], { endpoint: 'generaldescription/SearchGeneralDescriptions', data: model },
		[], [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY], undefined, undefined, true)(props)
  }
}

export function loadGeneralDescriptions () {
  return CommonActions.apiCall([keyToState, 'search'], { endpoint: 'generaldescription/GetGeneralDescriptionSearch', data: null },
		[CommonSchemas.TARGET_GROUP_ARRAY, CommonSchemas.SERVICE_CLASS_ARRAY], CommonSchemas.SEARCH, keyToState, true)
}

export function getGeneralDescriptions (id, rootId) {
  const payload = CommonActions.onEntityInputChange('searches', 'generalDescription', 'generalDescriptionItemSelectedId', id).payload
  return CommonActions.apiCall([keyToState, 'getGeneralDescription'], { ...payload, ...{ endpoint: 'generaldescription/GetGeneralDescription', data: rootId } },
		[CommonSchemas.LAW_ARRAY], GeneralDescriptionSchemas.GENERAL_DESCRIPTION, keyToState, true)
}
