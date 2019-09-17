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
import { defineMessages } from 'util/react-intl'

export default defineMessages({
  languageVersionApproveMissing: {
    id: 'AppComponents.MassPublishTable.Errors.LanguageVersionsApproveMissing',
    defaultMessage: 'Another language versions needs to be approved otherwise it will be revert to draft. ({languages})'
  },
  languageVersionShouldBeApproved: {
    id: 'AppComponents.MassPublishTable.Errors.LanguageVersionHasToBeApproved',
    defaultMessage: 'Language version needs to be approved otherwise it will be revert to draft.'
  },
  cannotPublishTitle: {
    id: 'AppComponents.MassPublishTable.Errors.Title',
    defaultMessage: 'Cannot publish, see following errors:'
  }
})
