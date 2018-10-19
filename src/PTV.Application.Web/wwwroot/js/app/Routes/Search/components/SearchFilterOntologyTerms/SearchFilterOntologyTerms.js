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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { OntologyTermSearchTree } from 'util/redux-form/fields'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import { FintoSchemas } from 'schemas'
import { InputFilter } from 'appComponents/DropdownFilter/Inputs'
import { searchInList, clearTreeSearch } from 'Containers/Common/Actions/Nodes'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import { getOntologyTerm } from 'util/redux-form/fields/OntologyTermTree/selectors'
import { mergeInUIState } from 'reducers/ui'

const messages = defineMessages({
  title:{
    id: 'Containers.Services.AddService.Step2.OntologyTerms.Title',
    defaultMessage: 'Ontologiakäsitteet'
  },
  placeholder:{
    id: 'Search.SearchFilterOntologyTerms.Placeholder.Title',
    defaultMessage: '- Kirjoita hakusana -'
  }
})

const renderContent = (all, props) => (
  <OntologyTermSearchTree
    disabled={all}
    filterTree
    simple
    {...props} />
)

const ShowValue = compose(
  connect((state, { firstValue, displayValue }) => ({
    displayValue : firstValue && getOntologyTerm(state, { id: firstValue }).get('name') || displayValue
  }), {
    searchInList,
    clearTreeSearch
  }),
  localizeProps({
    idAttribute: 'firstValue',
    nameAttribute: 'displayValue',
    languageTranslationType: languageTranslationTypes.locale
  }),
  SearchFilter.withSearchFilter({
    FilterComponent: InputFilter,
    key: ({ filterName }) => filterName,
    filterFunc: (props, value) => {
      if (value !== '') {
        props.searchInList({
          searchSchema: FintoSchemas.ONTOLOGY_TERM_ARRAY,
          treeType: 'OntologyTerm',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('OntologyTerm', props.contextId)
      }
    },
    withReduxState: true
  })
)(_ => null)

const SearchFilterOntologyTerms = ({
  intl: { formatMessage },
  mergeInUIState,
  ...rest
}) => {
  const clearSearchValue = () => {
    mergeInUIState({
      key: 'ontologyTermSearch',
      value: {
        searchValue: ''
      }
    })
  }
  return (
    <Field
      name='ontologyTerms'
      content={renderContent}
      component={RenderDropdownFilter}
      title={formatMessage(messages.title)}
      filterName='ontologyTermSearch'
      ShowValueComponent={ShowValue}
      searchable
      customClear={clearSearchValue}
      placeholder={formatMessage(messages.placeholder)}
      {...rest}
    />
  )
}

SearchFilterOntologyTerms.propTypes = {
  mergeInUIState: PropTypes.func,
  formName: PropTypes.string.isRequired,
  selectedValues: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, { mergeInUIState })
)(SearchFilterOntologyTerms)
