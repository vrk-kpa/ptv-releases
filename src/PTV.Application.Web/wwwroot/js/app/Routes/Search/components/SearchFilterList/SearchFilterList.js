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
import { injectIntl, intlShape } from 'util/react-intl'
import SearchFilterItem from './SearchFilterItem'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { Button } from 'sema-ui-components'
import cx from 'classnames'
import styles from './styles.scss'
import { mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import { messages } from '../SearchFilter/messages'

const SearchFilterList = props => {
  const {
    intl: { formatMessage },
    category,
    items,
    isAllSelected,
    isFirst,
    dialogName,
    children,
    mergeInUIState
  } = props

  const handleOnClick = () => {
    mergeInUIState({
      key: dialogName,
      value: {
        isOpen: true
      }
    })
  }

  const categoryNameClass = cx(
    styles.categoryName,
    {
      [styles.isFirst]: isFirst
    }
  )

  const categoryListClass = cx(
    styles.categoryList,
    {
      [styles.isFirst]: isFirst
    }
  )

  return (
    <div className='row'>
      <div className='col-6 col-sm-5 col-md-4 col-lg-3'>
        <Button link onClick={handleOnClick} className={categoryNameClass} type='button' >
          {formatMessage(category)}:
        </Button>
      </div>
      <div className='col-18 col-sm-19 col-md-20 col-lg-21'>
        <div className={categoryListClass}>
          {
            isAllSelected
              ? <span className={styles.categoryAll}>{formatMessage(messages.allSelectedLabel)}</span>
              : items.map(item => <SearchFilterItem key={item.value} {...item} />)
          }
        </div>
      </div>
      { children }
    </div>
  )
}

SearchFilterList.propTypes = {
  intl: intlShape,
  category: PropTypes.object,
  items: PropTypes.array,
  isAllSelected: PropTypes.bool,
  isFirst: PropTypes.bool,
  dialogName: PropTypes.string,
  children: PropTypes.node,
  mergeInUIState: PropTypes.func
}

export default compose(
  injectIntl,
  connect(null, {
    mergeInUIState
  })
)(SearchFilterList)
