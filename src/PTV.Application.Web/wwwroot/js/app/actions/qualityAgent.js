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
import { apiCall3 } from 'actions'
import { QualityAgentSchemas } from 'schemas/qualityAgent'
import { formValueSelector } from 'redux-form/immutable'
import { getContentLanguageCode } from 'selectors/selections'
import { CLEAR_IN_ENTITIES } from 'reducers/entities'

export const directQualityEntityCheck = (values, store, options) => {
  options.languages.forEach(lang => {
    const data = {
      Input: {
        ...values,
        status: 'Saved'
      },
      Language: lang,
      EntityType: options.entityType,
      Profile: options.profile || 'VRKp'
    }
    // console.log('data', data)
    store.dispatch(clearQualityErrors())
    store.dispatch(apiCall3({
      keys: ['services', 'quality'],
      payload: { endpoint: 'qualityAgent/Check', data },
      schemas: QualityAgentSchemas.getQualityAgentSchema(values.id || values.alternativeId)
    // schemas: QualityAgentSchemas.QUALITY_AGENT
    }))
  })
}

// export const qualityEntityCheck = ({
//   id,
//   unificRootId,
//   alternativeId
// },
// entityType
// ) => {
//   const data = {
//     id,
//     alternativeId,
//     unificRootId,
//     entityType
//   }
//   // console.log('data', data)
//   return apiCall3({
//     keys: ['services', 'quality'],
//     payload: { endpoint: 'qualityAgent/CheckEntity', data },
//     schemas: QualityAgentSchemas.getQualityAgentSchema(unificRootId || alternativeId)
//   })
// }

// export const qualityEntityCheckIfNotRun = ({
//   id,
//   unificRootId,
//   alternativeId,
//   formName
// },
// entityType
// ) => {
//   // console.log(id, unificRootId, alternativeId, formName, entityType)
//   if (unificRootId) {
//     return qualityEntityCheck({ id, unificRootId, alternativeId }, entityType)
//   }
//   return store => {
//     if (formName) {
//       const values = formValueSelector(formName)(store.getState(), 'unificRootId', 'alternativeId', 'id')
//       store.dispatch(qualityEntityCheck(values, entityType))
//     }
//   }
// }

const profiles = {
  'channel':'VRKak',
  'service':'VRKp',
  'organization':'VRKo'
}

export const qualityCheckCancelChanges = ({
  id,
  unificRootId,
  alternativeId,
  formName
},
entityType
) => store => {
  const values = (unificRootId || alternativeId)
    ? { unificRootId, alternativeId }
    : formValueSelector(formName)(store.getState(), 'unificRootId', 'alternativeId', 'id')
  if (values.alternativeId || values.unificRootId) {
    const language = getContentLanguageCode(store.getState())
    const data = {
      Input: {
        id: values.unificRootId,
        alternativeId: values.alternativeId,
        status: 'Cancelled'
      },
      Language: language || 'fi',
      Profile: profiles[entityType]
    }
    // console.log('data', data)
    store.dispatch(clearQualityErrors())
    store.dispatch(apiCall3({
      keys: ['services', 'quality'],
      payload: { endpoint: 'qualityAgent/Check', data },
      schemas: QualityAgentSchemas.getQualityAgentSchema(values.unificRootId || values.alternativeId)
    }))
  }
}

export const clearQualityErrors = () => ({
  type: CLEAR_IN_ENTITIES,
  payload: {
    entityPath: ['qualityErrors']
  }
})
