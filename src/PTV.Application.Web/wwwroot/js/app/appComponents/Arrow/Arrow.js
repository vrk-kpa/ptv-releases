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
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'

const Arrow = ({
  direction,
  gapSide,
  gap,
  secondary,
  ...rest
}) => {
  const arrowClass = cx(
    styles.arrow,
    styles[direction],
    styles[gapSide],
    styles[gap],
    { [styles.secondary]: secondary }
  )
  return (
    <span className={arrowClass}>
      <PTVIcon
        name='icon-arrow-up'
        {...rest}
      />
    </span>
  )
}

Arrow.propTypes = {
  direction: PropTypes.oneOf(['north', 'east', 'south', 'west']),
  gapSide: PropTypes.oneOf(['top', 'right', 'bottom', 'left']),
  gap: PropTypes.string,
  secondary: PropTypes.bool
}

Arrow.defaultProps = {
  gap: 'g0'
}

export default Arrow
