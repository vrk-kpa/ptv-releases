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
import React from 'react'
import PropTypes from 'prop-types'
import { RenderAsyncSelect } from 'util/redux-form/renders'
import styles from './styles.scss'
import { compose } from 'redux'
import { localizeItem } from 'appComponents/Localize'

const getSearchData = (input) => {
  return {
    searchValue: input,
    treeType: 'OntologyTerm'
    // languages: [props.selectedLanguage, props.language]
  }
}

const OptionValue = compose(
  localizeItem({
    input: 'item',
    output: 'item',
    idAttribute: 'value',
    nameAttribute: 'label',
    getTranslationTexts: item => item.entity && item.entity.translation.Texts || null
  })
)(({ item }) => <span>{item.label}</span>)

const formatDisplay = (ot) => <OptionValue item={ot} />

const formatResponse = data =>
  data.map((ontologyTerm) => ({
    label: ontologyTerm.name,
    value: ontologyTerm.id,
    entity: ontologyTerm
  }))

const filterOption = () => true

const OntologySearch = ({ searchValue, ...rest }) => (
  <div className={styles.ontologyTermSearch}>
    <RenderAsyncSelect
      url='service/GetFilteredList'
      input={{ value: searchValue, name: '' }}
      meta={{}}
      filterOption={filterOption}
      getQueryData={getSearchData}
      formatResponse={formatResponse}
      optionRenderer={formatDisplay}
      valueRenderer={formatDisplay}
      autosize={false}
      {...rest}
    />
  </div>
)

OntologySearch.propTypes = {
  dispatch: PropTypes.func,
  searchValue: PropTypes.string
}

// export default compose(
//   localizeItem({
//     input: 'searchValue',
//     output: 'searchValue',
//     idAttribute: 'value',
//     nameAttribute: 'label'
//   }),
// )(OntologySearch)
export default OntologySearch
