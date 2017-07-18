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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'

// selectors
import * as SortSelectors from '../../selectors'

// types
import { sortDirectionTypes } from '../../../../Containers/Common/Enums'

const ColumnHeaderName = ({ column, name, sortDirection, contentType, onSort }) => {
  const handleOnClick = () => {
    onSort(column)
  }
  //const cellClass = cx('cell', sortDirection && `sort sort-${sortDirection}`)
  const cellClass = cx('header-cell', sortDirection && `sort sort-${sortDirection}`)
  const sortClass = cx(sortDirection && `sort sort-${sortDirection}`)

  return (
    <div
      onClick={handleOnClick}
      className={cellClass}
    >
      <div>{ name }</div>
      { sortDirection &&
        <div className='sort-arrows'>
          <div className='top-arrow'></div>
          <div className='bottom-arrow'></div>
        </div>
      }
    </div>
  )
}

ColumnHeaderName.propTypes = {
  name: PropTypes.string.isRequired,
  column: PropTypes.string.isRequired,
  contentType: PropTypes.string.isRequired,
  onSort: PropTypes.func.isRequired,
  sortDirection: PropTypes.object
}

function mapStateToProps (state, ownProps) {

  return {
    sortDirection: SortSelectors.getUiSortingDataDirection(state, { contentType: ownProps.contentType, column: ownProps.column ? ownProps.column.property : '' })
  }
}

export default connect(mapStateToProps)(ColumnHeaderName)
