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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Button, Spinner } from 'sema-ui-components'
import {
  getResultCount,
  getIsSearchingResults
} from 'Routes/Connections/selectors'
import styles from './styles.scss'

const messages = defineMessages({
  clearButtonTitle: {
    id: 'ModalDialog.ClearSelectedGeneralDescription.Button.Clear',
    defaultMessage: 'Tyhjennä'
  },
  searchButton: {
    id: 'Components.Buttons.SearchButton',
    defaultMessage: 'Hae'
  },
  searchResultText: {
    id: 'Routes.Connections.Components.SearchFormFooter.SearchResult.Text',
    defaultMessage: 'Hakutuloksia: {count}'
  }
})

const SearchFormFooter = ({
  resultCount,
  isSearching,
  onClickClear,
  intl: { formatMessage }
}) => {
  return (
    <div className={styles.searchFormFooter}>
      <div className={styles.buttonGroup}>
        <Button
          type='submit'
          children={isSearching && <Spinner /> || formatMessage(messages.searchButton)}
          disabled={isSearching}
          small
        />
        <Button
          onClick={onClickClear}
          secondary
          children={formatMessage(messages.clearButtonTitle)}
          small
        />
      </div>
      <div className={styles.resultText}>{formatMessage(messages.searchResultText, { count: resultCount || 0 })}</div>
    </div>
  )
}
SearchFormFooter.propTypes = {
  intl: intlShape,
  resultCount: PropTypes.number,
  onClickClear: PropTypes.func,
  isSearching: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    resultCount: getResultCount(state),
    isSearching: getIsSearchingResults(state, ownProps)
  }))
)(SearchFormFooter)
