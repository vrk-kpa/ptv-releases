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
import { createSelector } from 'reselect'
import createCachedSelector from 're-reselect'
import { List, Map } from 'immutable'
import messages from './messages'
import { getHasErrorForLanguage } from '../selectors'
import { getMissingLanguagesForApproveTranslated } from 'selectors/massTool'
import validationMessages from 'util/redux-form/HOC/withEntityNotification/messages'

const languageVersionApproveMissing = Map({ message: messages.languageVersionApproveMissing })
const shouldApprove = Map({ message: messages.languageVersionShouldBeApproved })
const publishErrors = Map({ message: validationMessages.mandatoryInfoMissing })

export const getErrorMessagesForUnapproved = createSelector(
  getHasErrorForLanguage,
  hasErrors => hasErrors ? List([shouldApprove, publishErrors]) : List([shouldApprove])
)

export const getErrorMessagesForApproved = createSelector(
  getMissingLanguagesForApproveTranslated,
  languages => languages.length
    ? List([languageVersionApproveMissing.set('options', { languages: languages.map(l => l.name).join(', ') })])
    : List()
)
