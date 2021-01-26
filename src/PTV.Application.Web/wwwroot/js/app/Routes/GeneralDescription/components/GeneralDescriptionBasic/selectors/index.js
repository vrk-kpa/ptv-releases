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
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import { getFormValue } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { EditorState } from 'draft-js'
import { getContentLanguageCode } from 'selectors/selections'

// form selectors
export const getFormServiceTypeId = createSelector(
    getFormValue('serviceType'),
    values => values || ''
)

export const getServiceTypeService = createSelector(
    EntitySelectors.serviceTypes.getEntities,
    serviceTypes => serviceTypes.find(st => st.get('code').toLowerCase() === 'service') || Map()
)

export const getServiceTypeServiceId = createSelector(
    getServiceTypeService,
    serviceTypeService => serviceTypeService.get('id') || ''
)

export const getIsOtherServiceTypeThanService = createSelector(
    [getFormServiceTypeId, getServiceTypeServiceId],
    (formServiceType, serviceTypeServiceId) => formServiceType !== serviceTypeServiceId
)

export const getFormDescription = createSelector(
  [
    getFormValue('description'),
    getContentLanguageCode
  ],
  (value, languageCode) => {
    const emptyEditorState = EditorState.createEmpty()
    const editorState = value && value.get(languageCode) || emptyEditorState
    return editorState && editorState.getCurrentContent && editorState.getCurrentContent().getPlainText() || ''
  }
)

export const getFormBackgroundDescription = createSelector(
  [
    getFormValue('backgroundDescription'),
    getContentLanguageCode
  ],
  (value, languageCode) => {
    const emptyEditorState = EditorState.createEmpty()
    const editorState = value && value.get(languageCode) || emptyEditorState
    return editorState && editorState.getCurrentContent && editorState.getCurrentContent().getPlainText() || ''
  }
)

export const getIsAnyDescriptionProvided = createSelector(
  [
    getFormDescription,
    getFormBackgroundDescription
  ],
  (description, bgDescription) => !!(description || bgDescription)
)
