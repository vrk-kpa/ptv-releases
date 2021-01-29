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
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import { getUiSortingDataDirection } from 'selectors/base'

const ColumnHeaderName = ({ column, name, sortDirection, contentType, onSort, hideSortingIcon }) => {
  const handleOnClick = () => {
    onSort && typeof onSort === 'function' && onSort(column)
  }
  const cellClass = cx(
    styles.headerCell,
    sortDirection && styles[sortDirection],
    {
      [styles.sort]: sortDirection
    }
  )

  return (
    <div
      onClick={handleOnClick}
      className={cellClass}
    >
      <div>{name}</div>
      {!hideSortingIcon && sortDirection &&
        <div className={styles.sortArrows}>
          <div className={styles.topArrow} />
          <div className={styles.bottomArrow} />
        </div>
      }
    </div>
  )
}

ColumnHeaderName.propTypes = {
  name: PropTypes.string.isRequired,
  column: PropTypes.object.isRequired,
  contentType: PropTypes.string.isRequired,
  onSort: PropTypes.func,
  sortDirection: PropTypes.string,
  hideSortingIcon: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  return {
    sortDirection: ownProps.onSort && typeof ownProps.onSort === 'function' && getUiSortingDataDirection(state, {
      contentType: ownProps.contentType,
      column: ownProps.column ? ownProps.column.property : '' || null
    })
  }
}

export default connect(mapStateToProps)(ColumnHeaderName)
