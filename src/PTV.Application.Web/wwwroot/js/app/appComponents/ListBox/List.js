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
import React, { Component, createRef } from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'

class List extends Component {
  constructor (props) {
    super(props)
    this.listRef = createRef()
  }
  componentDidMount () {
    document.addEventListener('mousedown', this.handleClick, false)
    this.listRef.current.focus()
  }
  componentWillUnmount () {
    document.removeEventListener('mousedown', this.handleClick, false)
  }

  handleClick = event => {
    if (!this.listRef.current.contains(event.target)) {
      this.props.clickOutsideList()
    }
  }

  render () {
    const {
      options,
      onListItemClick,
      selectedValue,
      reduxKey,
      onKeyDown,
      focusedOption,
      onFocus,
      onBlur
    } = this.props
    return (
      <div className={styles.listWrap}>
        <ul
          ref={this.listRef}
          className={styles.list}
          role='listbox'
          tabIndex={-1}
          id={`${reduxKey}_list`}
          aria-activedescendant={`${reduxKey}_listItem_${selectedValue}`}
          onKeyDown={onKeyDown}
          onFocus={onFocus}
          onBlur={onBlur}
          aria-label={this.props['aria-label']}
          aria-labelledby={this.props['aria-labelledby']}
        >
          {options.map(option => {
            const optionValue = option.value
            const optionLabel = option.label
            const isSelected = optionValue === selectedValue
            const isFocused = optionValue === focusedOption
            const listItemClass = cx(
              styles.listItem,
              {
                [styles.selected]: isSelected,
                [styles.focused]: isFocused
              }
            )
            return (
              <li
                className={listItemClass}
                key={optionValue}
                id={`${reduxKey}_listItem_${optionValue}`}
                onClick={() => onListItemClick(option)}
                role='option'
                aria-selected={isSelected}
                aria-label={optionLabel}
              >
                {optionLabel}
              </li>
            )
          })}
        </ul>
      </div>
    )
  }
}

List.propTypes = {
  options: PropTypes.array,
  onListItemClick: PropTypes.func,
  selectedValue: PropTypes.string,
  reduxKey: PropTypes.string,
  onKeyDown: PropTypes.func,
  onFocus: PropTypes.func,
  onBlur: PropTypes.func,
  focusedOption: PropTypes.string,
  clickOutsideList: PropTypes.func,
  'aria-label': PropTypes.string,
  'aria-labelledby': PropTypes.string
}

export default List
