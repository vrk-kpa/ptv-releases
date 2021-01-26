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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getIsSearchingResults } from 'Routes/Connections/selectors'
import { Fulltext } from 'util/redux-form/fields'
import { SearchIcon } from 'appComponents/Icons'
import { SearchButton } from 'appComponents/Buttons'
import styles from './styles.scss'

const messages = defineMessages({
  searchLabel: {
    id: 'Routes.Connections.Components.SearchServicesForm.SearchField.Title',
    defaultMessage: 'Hae palveluita'
  },
  searchPlaceholder: {
    id: 'Routes.Connections.Components.SearchServicesForm.SearchField.Placeholder',
    defaultMessage: 'Hae...'
  }
})

const SearchFieldSubmit = ({
  intl: { formatMessage },
  isSearching,
  label
}) => {
  return (
    <div className={styles.searchFieldSubmit}>
      <Fulltext
        small
        label={label}
        placeholder={formatMessage(messages.searchPlaceholder)}
      />
      <SearchButton
        type='submit'
        isSearching={isSearching}
        disabled={isSearching}
        className={styles.searchButton}
      >
        <SearchIcon size={20} inverse />
      </SearchButton>
    </div>
  )
}

SearchFieldSubmit.propTypes = {
  intl: intlShape.isRequired,
  isSearching: PropTypes.bool,
  label: PropTypes.string
}

export default compose(
  injectIntl,
  connect(state => ({
    isSearching: getIsSearchingResults(state)
  }))
)(SearchFieldSubmit)
