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
import { PTVIcon } from 'Components'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'

const ExternalLinkIcon = ({
  size = 18,
  onClick,
  componentClass,
  disabled,
  ...rest
}) => {
  const className = cx(
    styles.icon,
    {
      [styles.disabled]: disabled
    },
    componentClass
  )
  return (
    <PTVIcon
      name='icon-external-link'
      width={size}
      height={size}
      onClick={disabled ? null : onClick}
      componentClass={className}
      {...rest}
    />
  )
}

ExternalLinkIcon.propTypes = {
  size: PropTypes.number,
  onClick: PropTypes.func,
  componentClass: PropTypes.string,
  disabled: PropTypes.bool
}

export default ExternalLinkIcon
