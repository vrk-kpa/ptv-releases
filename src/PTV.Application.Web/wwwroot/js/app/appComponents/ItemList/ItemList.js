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
import ImmutablePropTypes from 'react-immutable-proptypes'

// Components
import Item from './Item'
import { compose } from 'redux'
import withState from 'util/withState'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import cx from 'classnames'

const getKey = (item, getCustomKey) =>
  getCustomKey && typeof getCustomKey === 'function' && getCustomKey(item) ||
    item.get('path')

const renderItems = (items, showForLanguage, getCustomKey) =>
  items && items
    .filter(item => !(showForLanguage && item.get('languages') && !item.get('languages').includes(showForLanguage)))
    .map(item => <Item showForLanguage={showForLanguage}
      key={getKey(item, getCustomKey)} {...item.toJS()} />)

const ItemList = ({
  items,
  title,
  type,
  closable,
  uiKey,
  isOpen,
  updateUI,
  resetUI,
  showForLanguage,
  getCustomKey
}) => {
  const listWrapClass = cx(
    styles.listWrap,
    type === 'error' && styles.error
  )
  const handleClick = () => {
    updateUI('isOpen', false)
  }
  const itemsToShow = renderItems(items, showForLanguage, getCustomKey)
  if (!itemsToShow || !itemsToShow.size) {
    return null
  }
  return (
    (isOpen || !closable) &&
    <div className={listWrapClass}>
      {closable &&
        <div className={styles.closeIcon}>
          <PTVIcon name='icon-cross' onClick={handleClick} />
        </div>
      }
      <strong className={styles.title}>{title}</strong>
      <ul className={styles.list}>
        {itemsToShow}
      </ul>
    </div>
  )
}

ItemList.propTypes = {
  items: PropTypes.oneOfType([
    PropTypes.array,
    ImmutablePropTypes.list
  ]),
  title: PropTypes.node,
  type: PropTypes.string,
  closable: PropTypes.bool,
  uiKey: PropTypes.string,
  getCustomKey: PropTypes.func
  // updateUI: PropTypes.func.isRequired
}

export default compose(
  withState({
    initialState: {
      isOpen: true
    }
  })
)(ItemList)
