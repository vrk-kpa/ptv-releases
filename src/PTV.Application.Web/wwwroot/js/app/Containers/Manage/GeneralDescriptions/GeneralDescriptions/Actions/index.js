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

import { CALL_API, Schemas } from '../../../../../Middleware/Api'
import { onTranslatedEntityInputChange, onEntityInputChange, onEntityListChange, onLocalizedEntityAdd }
  from '../../../../Common/Actions'
import { getSaveStep1Model, getGeneralDescriptionInfo } from '../Selectors'
import { getGeneralDescriptionId } from '../../Common/Selectors'
import { GeneralDescriptionSchemas } from '../../GeneralDescriptions/Schemas'
import { CommonServiceSchemas } from '../../../../Services/Common/Schemas'
import { CommonSchemas } from '../../../../Common/Schemas'
import { EnumDefinitionSchema } from '../../../../Common/Schemas/enum'
import * as CommonActions from '../../../../../Containers/Common/Actions'

const keyToState = 'generalDescription'

export function onGeneralDescriptionTranslatedInputChange (input, value, langCode, isSet) {
  return ({ getState }) =>
    onTranslatedEntityInputChange('generalDescriptions',
     getGeneralDescriptionId(getState()),
     input, value, langCode, isSet)
}
export function onGeneralDescriptionInputChange (input, value, isSet) {
  return ({ getState }) =>
    onEntityInputChange('generalDescriptions', getGeneralDescriptionId(getState()), input, value, isSet)
}

export function onGeneralDescriptionLawRemove (id, langCode) {
  return ({ getState }) => {
    return onEntityListChange('generalDescriptions', getGeneralDescriptionId(getState()), 'laws', id)
  }
}

export function onGeneralDescriptionLawEntityRemove (id, entity = 'laws') {
  return ({ getState }) => {
    return CommonActions.onEntityClear(entity, id)
  }
}

export function onGeneralDescriptionLawAdd (entity, langCode) {
  return ({ getState }) => {
    return onLocalizedEntityAdd(
      { id: getGeneralDescriptionId(getState()), 'laws': entity },
       GeneralDescriptionSchemas.GENERAL_DESCRIPTION, langCode)
  }
}

export function onListChange (input, value, isAdd) {
  return ({ getState }) => {
    return onEntityListChange('generalDescriptions', getGeneralDescriptionId(getState()), input, value, isAdd)
  }
}

function callModifyAction (typeOfReducer, endpoint, data, typeSchemas, successNextAction, messages) {
  return CommonActions.apiCall2({
    keys: [keyToState, typeOfReducer],
    payload: { endpoint: endpoint, data, messages },
    typeSchemas,
    schemas: GeneralDescriptionSchemas.GENERAL_DESCRIPTION,
    keyToState,
    successNextAction
  })
}

export function getGeneralDescriptionNames (id, keyToState) {
  return CommonActions.apiCall2({
    keys: [keyToState, 'all'],
    payload: { endpoint: 'generalDescription/GetGeneralDescriptionNames', data: { id } },
    schemas: GeneralDescriptionSchemas.GENERAL_DESCRIPTION,
    typeSchemas: [CommonSchemas.LANGUAGE_ARRAY]
  })
}

export function publishGeneralDescription (id, formValues) {
  return (props) => {
    return callModifyAction('all',
      'generalDescription/PublishGeneralDescription',
      {
        id,
        languagesAvailabilities: formValues.map((value, key) => ({ languageId: key, statusId: value })).toList()
      },
      [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY])(props)
  }
}

export function withdrawGeneralDescription (id) {
  return (props) => {
    return callModifyAction('all',
      'generalDescription/WithdrawGeneralDescription',
      { id },
      [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY])(props)
  }
}

export function restoreGeneralDescription (id) {
  return (props) => {
    return callModifyAction('all',
      'generalDescription/RestoreGeneralDescription',
      { id },
      [GeneralDescriptionSchemas.GENERAL_DESCRIPTION_ARRAY])(props)
  }
}

function clearServiceIdModelState () {
  return CommonActions.deleteApiCall(['service'])
}

export function deleteGeneralDescription (id, keyToState, language) {
  return (props) => {
    return callModifyAction('all', 'generalDescription/DeleteGeneralDescription', { id, language }, [], clearServiceIdModelState)(props)
  }
}

export const lockGeneralDescription = (id, keyToState, successNextAction) => {
  return (props) => {
    return CommonActions.apiCall2({
      keys: [keyToState, 'lock'],
      payload: { endpoint: 'generalDescription/LockGeneralDescription', data: { id } },
      saveRequestData: true,
      successNextAction
    })(props)
  }
}

export function unLockGeneralDescription (id) {
  return (props) => {
    return CommonActions.apiCall([keyToState, 'lock'],
      { endpoint: 'generalDescription/UnLockGeneralDescription', data: { id } },
      null, null, undefined, undefined, true)(props)
  }
}

export function isGeneralDescriptionLocked (id) {
  return (props) => {
    return CommonActions.apiCall([keyToState, 'lock'],
      { endpoint: 'generalDescription/IsGeneralDescriptionLocked', data: { id } },
      null, null, undefined, undefined, true)(props)
  }
}

export function isGeneralDescriptionEditable (id) {
  return (props) => {
    return CommonActions.apiCall([keyToState, 'editable'],
									{ endpoint: 'generalDescription/IsGeneralDescriptionEditable', data: { id } },
									null, null, undefined, undefined, true)(props)
  }
}

// / Step 1
export function loadAddGeneralDescriptionStep1 (id, language) {
  return CommonActions.apiCall(
    [keyToState, 'step1Form'],
    { endpoint: 'generalDescription/GetAddGeneralDescription', data: { id, language } },
    EnumDefinitionSchema,
    GeneralDescriptionSchemas.GENERAL_DESCRIPTION, keyToState, false, true)
}

export function saveAllChanges ({ id, language, successNextAction }) {
  return (props) => {
    return callModifyAction(
      'step1Form',
      'generalDescription/SaveAllChanges',
      getSaveStep1Model(props.getState(), { language }),
      [CommonSchemas.CHARGE_TYPE_ARRAY, CommonSchemas.LAW_ARRAY, CommonServiceSchemas.SERVICE_TYPE_ARRAY],
      successNextAction)(props)
  }
}

export function getAnnotations (language) {
  return (props) => {
    return CommonActions.apiCall(
      [keyToState, 'annotation'],
      { endpoint: 'common/GetAnnotations', data: getGeneralDescriptionInfo(props.getState(), { language }) },
      [CommonSchemas.ANNOTATION_ONTOLOGY_TERM_ARRAY],
      GeneralDescriptionSchemas.GENERAL_DESCRIPTION)(props)
  }
}

export function getAnnotationHierarchy (id, pageEntityId) {
  return (props) => {
    return CommonActions.apiCall(
      ['AnnotationOntologyTerm', pageEntityId, 'search'],
      { endpoint: 'common/GetAnnotationHierarchy', data: { id } },
      [],
      CommonSchemas.ANNOTATION_ONTOLOGY_TERM_ARRAY,
      null, null, true)(props)
  }
}
