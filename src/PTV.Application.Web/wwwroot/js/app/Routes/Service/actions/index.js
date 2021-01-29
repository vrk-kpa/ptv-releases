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
import { EntitySchemas } from 'schemas'
import { QualityAgentSchemas } from 'schemas/qualityAgent'
import { formTypesEnum } from 'enums'

export const getService = (data, apiCallOptions = {}) => {
  return apiCall3({
    keys: ['services', 'load'],
    payload: { endpoint: 'service/GetService', data },
    schemas: EntitySchemas.SERVICE_FORM,
    ...apiCallOptions
  })
}

export const loadHeader = (id) => {
  return apiCall3({
    keys: ['services', 'header'],
    payload: { endpoint: 'service/GetServiceHeader', data: { id } },
    schemas: EntitySchemas.SERVICE
  })
}

export const qualityCheck = (values) => store => {
  const editorState = values.getIn(['description', 'fi'])
  const desText = editorState && typeof editorState.getCurrentContent === 'function'
    ? editorState.getCurrentContent().getPlainText(';')
    : null
  const data = {
    Input: {
      serviceDescriptions: [{
        language: 'fi',
        type: 'Description',
        value: desText
      }],
      type: 'Service',
      id: '00e48113-501a-42fe-abd9-8b34ed30b7fc'
    },
    Language: 'fi',
    Profile:'VRKp'
  }

  store.dispatch(apiCall3({
    keys: ['services', 'quality'],
    payload: { endpoint: 'qualityAgent/Check', data },
    schemas: QualityAgentSchemas.QUALITY_AGENT,
    formName: formTypesEnum.SERVICEFORM
  }))
}
