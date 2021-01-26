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
import cx from 'classnames'
import styles from './styles.scss'
import { keyCodes } from 'util/helpers'
import { CrossIcon } from 'appComponents/Icons'

const Chip = props => {
  const {
    children,
    message,
    thumbnail,
    isDisabled,
    isRemovable,
    onSelect,
    onRemove,
    small,
    className
  } = props

  const chipClass = cx(
    styles.chip,
    {
      [styles.disabled]: isDisabled,
      [styles.small]: small
    },
    className
  )

  const handleKeyDown = e => {
    e.stopPropagation()
    switch (e.keyCode) {
      case keyCodes.BACKSPACE:
        e.preventDefault()
        onRemove && onRemove()
        break
      case keyCodes.RETURN:
      case keyCodes.SPACE:
        e.preventDefault()
        onSelect && onSelect()
        break
    }
  }

  return (
    <div
      className={chipClass}
      onClick={onSelect}
      role='button'
      tabIndex={0}
      onKeyDown={handleKeyDown}
    >
      {thumbnail}
      <div className={styles.text}>
        {message || children}
      </div>
      {isRemovable &&
        <CrossIcon
          onClick={onRemove}
          componentClass={styles.icon}
        />}
    </div>
  )
}

Chip.propTypes = {
  children: PropTypes.any,
  message: PropTypes.string,
  thumbnail: PropTypes.node,
  isDisabled: PropTypes.bool,
  isRemovable: PropTypes.bool,
  onSelect: PropTypes.func,
  onRemove: PropTypes.func,
  small: PropTypes.bool,
  className: PropTypes.string
}

export default Chip
