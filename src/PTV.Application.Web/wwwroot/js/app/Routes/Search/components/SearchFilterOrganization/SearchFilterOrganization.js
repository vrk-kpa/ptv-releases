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
import { OrganizationSearchTree } from 'util/redux-form/fields'
import { getOrganization, getIsFetching } from 'util/redux-form/fields/OrganizationTree/selectors'
import { RenderDropdownFilter } from 'util/redux-form/renders'
import { InputFilter } from 'appComponents/DropdownFilter/Inputs'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import SearchFilter from 'util/redux-form/HOC/withSearchFilter'
import { EntitySchemas } from 'schemas'
import { searchInTree, clearTreeSearch } from 'Containers/Common/Actions/Nodes'
import { localizeProps, languageTranslationTypes } from 'appComponents/Localize'
import { mergeInUIState } from 'reducers/ui'
// import { getFilteredOrganizationIds, getOrganization } from 'util/redux-form/fields/OrganizationTree/selectors'
const messages = defineMessages({
  title: {
    id: 'Containers.ServiceAndChannels.ChannelSearch.SearchBox.Organization.Title',
    defaultMessage: 'Organisaatio'
  }
})

const renderContent = (all, { focusedTreeItem, searchValue }) => (
  <OrganizationSearchTree disabled={all} searchValue={searchValue} filterTree simple />
)

const ShowValue = compose(
  connect((state, { firstValue, displayValue, searchValue }) => ({
    displayValue: firstValue && getOrganization(state, { id: firstValue }).get('name') || displayValue,
    isSearching: getIsFetching(state, { searchValue })
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
          searchSchema: EntitySchemas.ORGANIZATION_ARRAY,
          treeType: 'Organization',
          value,
          contextId: value
        })
      } else {
        props.clearTreeSearch('Organization', props.contextId)
      }
    },
    withReduxState: true
  })
)(_ => null)

const SearchFilterOrganization = ({
  intl: { formatMessage },
  mergeInUIState,
  ...rest
}) => {
  const clearSearchValue = () => {
    mergeInUIState({
      key: 'organizationSearch',
      value: {
        searchValue: ''
      }
    })
  }
  return (
    <Field
      name='organizationIds'
      component={RenderDropdownFilter}
      ShowValueComponent={ShowValue}
      content={renderContent}
      title={formatMessage(messages.title)}
      filterName='organizationSearch'
      searchable
      customClear={clearSearchValue}
      {...rest}
    />
  )
}

SearchFilterOrganization.propTypes = {
  reset: PropTypes.func,
  mergeInUIState: PropTypes.func,
  formName: PropTypes.string.isRequired,
  firstValue: PropTypes.any,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, { mergeInUIState })
)(SearchFilterOrganization)
