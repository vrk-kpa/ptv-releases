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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import SearchForm from 'Routes/Connections/components/SearchForm'
import SearchResult from 'Routes/Connections/components/SearchResult'
import MainEntitySelection from 'Routes/Connections/components/MainEntitySelection'
import { getConnectionsEntity } from 'selectors/selections'
import styles from './styles.scss'
import { injectIntl } from 'util/react-intl'

class SearchConnectionsForm extends PureComponent {
  render () {
    const { selectedEntity } = this.props
    return (
      <div className={styles.searchAreaWrap}>
        <div className={styles.searchArea}>
          {selectedEntity === null
            ? <MainEntitySelection />
            : <SearchForm />}
          <SearchResult />
        </div>
      </div>
    )
  }
}
SearchConnectionsForm.propTypes = {
  selectedEntity: PropTypes.oneOf(['channels', 'services'])
}

export default compose(
  injectIntl,
  connect(state => ({
    selectedEntity: getConnectionsEntity(state)
  }))
)(SearchConnectionsForm)
