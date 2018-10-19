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
import { FintoSchemas } from 'schemas'
import { InputFilter } from 'appComponents/DropdownFilter/Inputs'
import { ServiceClassSearchTree } from 'util/redux-form/fields'
import { getServiceClass } from 'util/redux-form/fields/ServiceClassTree/selectors'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import { searchInTree, clearTreeSearch } from 'Containers/Common/Actions/Nodes'
import { mergeInUIState } from 'reducers/ui'

const messages = defineMessages({
  title:{
    id: 'Containers.Services.AddService.Step2.ServiceClass.Title',
    defaultMessage: 'Palveluluokka'
  },
  placeholder:{
    id: 'Search.SearchFilterServiceClassses.Placeholder.Title',
    defaultMessage: '- Kirjoita hakusana -'
  }
})

const renderContent = (all, props) => (
  <ServiceClassSearchTree
    disabled={all}
    filterTree
    simple
    {...props}
  />
)

const ShowValue = compose(
  connect((state, { firstValue, displayValue }) => ({
    displayValue : firstValue && getServiceClass(state, { id: firstValue }).get('name') || displayValue
  }), {
    searchInTree,
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
        props.searchInTree({
          searchSchema: FintoSchemas.SERVICE_CLASS_ARRAY,
          treeType: 'ServiceClass',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('ServiceClass', props.contextId)
      }
    },
    withReduxState: true
  })
)(_ => null)

const SearchFilterServiceClassses = ({
  intl: { formatMessage },
  mergeInUIState,
  ...rest
}) => {
  const clearSearchValue = () => {
    mergeInUIState({
      key: 'serviceClassSearch',
      value: {
        searchValue: ''
      }
    })
  }
  return (
    <Field
      name='serviceClasses'
      component={RenderDropdownFilter}
      content={renderContent}
      title={formatMessage(messages.title)}
      filterName={'serviceClassSearch'}
      ShowValueComponent={ShowValue}
      searchable
      customClear={clearSearchValue}
      placeholder={formatMessage(messages.placeholder)}
      {...rest}
    />
  )
}

SearchFilterServiceClassses.propTypes = {
  mergeInUIState: PropTypes.func,
  formName: PropTypes.string.isRequired,
  selectedValues: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, { mergeInUIState })
)(SearchFilterServiceClassses)
