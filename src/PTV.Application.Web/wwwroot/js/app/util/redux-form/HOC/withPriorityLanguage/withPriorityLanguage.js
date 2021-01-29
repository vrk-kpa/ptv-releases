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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getLanguageCode } from 'selectors/common'
import { getSelectedLanguages } from 'Routes/Search/components/SearchFilters/selectors'
import { formTypesEnum } from 'enums'
import { getSelectedLanguage } from 'Intl/Selectors'
const withPriorityLanguage = WrappedComponent => compose(
  connect(state => {
    const uiLang = getSelectedLanguage(state)
    const filterLanguages = getSelectedLanguages(state, { formName : formTypesEnum.FRONTPAGESEARCH })
      .map(x => getLanguageCode(state, { languageId:x.value }))
    const selectedLang = filterLanguages.some(x => x === uiLang) ? uiLang : filterLanguages.find(x => true) || uiLang
    return {
      priorityLanguageCode: selectedLang
    }
  })
)(WrappedComponent)

export const formStatesPropTypes = {
  languageCode: PropTypes.string
}

export default withPriorityLanguage
