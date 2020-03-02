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
// import { createSelector } from 'reselect'

// export const getEntityType = createSelector(
//   getPageModeState,
//   pageModeState => {
//     const result = {
//       services: 'eChannel',
//       eChannel: 'eChannel'
//     }[pageModeState.get('searchDomain')] || null
//     return result
//   }
// )
// export const getEntityId = createSelector(
//   [getPageModeState, getEntityType],
//   (pageModeState, entityType) => {
//     const result = pageModeState.getIn([entityType, 'id'])
//     console.log('result:\n', result)
//     return result
//   }
// )
// export const getLanguageId = createSelector(
//   [getPageModeState, getEntityType],
//   (pageModeState, entityType) => {
//     return pageModeState.getIn([entityType, 'languageTo']) || '77f76576-cd77-43dd-b13f-6aff0f84a4b4'
//   }
// )
