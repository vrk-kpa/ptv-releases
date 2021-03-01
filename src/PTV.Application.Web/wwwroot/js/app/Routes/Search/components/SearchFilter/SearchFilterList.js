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
import { injectIntl, intlShape } from 'util/react-intl'
import SearchFilterItem from './SearchFilterItem'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import cx from 'classnames'
import styles from './styles.scss'
import { messages } from '../SearchFilters/messages'
import { messages as commonMessages } from 'Routes/messages'

const SearchFilterList = props => {
  const {
    intl: { formatMessage },
    items,
    isAllSelected,
    isFirst,
    isLast,
    className,
    filterName
  } = props

  const categoryListClass = cx(
    styles.categoryList,
    {
      [styles.isFirst]: isFirst,
      [styles.isLast]: isLast
    },
    className
  )
  const isNoneSelected = !isAllSelected && Array.isArray(items) ? items.length === 0 : items.size === 0
  return (
    <div className={categoryListClass}>
      {isAllSelected && (
        <SearchFilterItem label={formatMessage(messages.allSelectedLabel)} className={styles.plain} />
      )}
      {isNoneSelected && (
        <SearchFilterItem label={formatMessage(commonMessages.noneSelectedLabel)} className={styles.plain} />
      )}
      {!isAllSelected && !isNoneSelected && items.map(item => (
        <SearchFilterItem key={item.value} filterName={filterName} {...item} />
      ))}
    </div>
  )
}

SearchFilterList.propTypes = {
  intl: intlShape,
  items: PropTypes.array,
  isAllSelected: PropTypes.bool,
  isFirst: PropTypes.bool,
  isLast: PropTypes.bool,
  className: PropTypes.string,
  filterName: PropTypes.string
}

export default compose(
  injectIntl
)(SearchFilterList)
