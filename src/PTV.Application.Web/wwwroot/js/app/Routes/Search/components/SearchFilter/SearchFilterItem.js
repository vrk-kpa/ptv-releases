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
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { CrossIcon } from 'appComponents/Icons'
import Chip from 'appComponents/Chip'
import { removeSearchFilter } from 'Routes/Search/actions'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import cx from 'classnames'
import styles from './styles.scss'

const SearchFilterItem = props => {
  const {
    intl: { formatMessage },
    label,
    message,
    value,
    className,
    onClick,
    removeSearchFilter,
    filterName,
    formName
  } = props

  const searchFilterItemClass = cx(
    styles.searchFilterItem,
    {
      [styles.removable]: !!value
    },
    className
  )

  const handleOnRemove = () => {
    onClick && typeof onClick === 'function'
      ? onClick()
      : removeSearchFilter(value, filterName, formName)
  }

  return (
    <div data-value={value} className={searchFilterItemClass}>
      {value && (
        <Chip
          message={label || formatMessage(message)}
          onRemove={handleOnRemove}
          isRemovable
          small
        />
      ) || <span className={styles.text}>{label || formatMessage(message)}</span>
      }
    </div>
  )
}

SearchFilterItem.propTypes = {
  intl: intlShape,
  label: PropTypes.string,
  message: PropTypes.object,
  value: PropTypes.any,
  className: PropTypes.string,
  onClick: PropTypes.func,
  removeSearchFilter: PropTypes.func,
  filterName: PropTypes.string,
  formName: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  connect(null, { removeSearchFilter })
)(SearchFilterItem)
