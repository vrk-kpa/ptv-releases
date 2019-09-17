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
import cx from 'classnames'
import styles from './styles.scss'
import { PTVIcon } from 'Components'

const Tag = props => {
  const {
    children,
    isDisabled,
    message,
    isVisible,
    isRemovable,
    onTagRemove,
    bgColor,
    textColor,
    normal,
    className
  } = props

  const tagClass = cx(
    styles.tag,
    {
      [styles.disabled]: isDisabled,
      [styles.normal]: normal
    },
    className
  )

  if (!isVisible) return null
  return <div className={tagClass} style={{ backgroundColor: bgColor, color: textColor }}>
    <div>
      {message || children}
    </div>
    {isRemovable &&
      <PTVIcon
        name='icon-cross'
        onClick={onTagRemove}
      />}
  </div>
}

Tag.propTypes = {
  children: PropTypes.any,
  message: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  isRemovable: PropTypes.bool,
  onTagRemove: PropTypes.func,
  bgColor: PropTypes.string,
  textColor: PropTypes.string,
  normal: PropTypes.bool,
  className: PropTypes.string
}

Tag.defaultProps = {
  isVisible: true,
  isRemovable: true
}

export default Tag
