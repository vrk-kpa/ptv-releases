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
import styles from './styles.scss'

const Trigger = ({
  children,
  isOpen,
  value,
  onClick,
  id,
  triggerRef,
  onFocus,
  onBlur,
  onKeyDown,
  disabled
}) => {
  return (
    <div className={styles.triggerWrap}>
      <div className={styles.triggerContentWrap}>
        <div className={styles.trigger}>
          <button
            ref={triggerRef}
            onClick={onClick}
            id={id}
            aria-haspopup='listbox'
            aria-expanded={isOpen}
            type='button'
            onFocus={onFocus}
            onBlur={onBlur}
            onKeyDown={onKeyDown}
            disabled={disabled}
          >
            <span className={styles.triggerText}>{value}</span>
          </button>
        </div>
        {children}
      </div>
    </div>
  )
}

Trigger.propTypes = {
  children: PropTypes.any,
  isOpen: PropTypes.bool,
  value: PropTypes.string,
  onClick: PropTypes.func,
  id: PropTypes.string,
  triggerRef: PropTypes.func,
  onFocus: PropTypes.func,
  onBlur: PropTypes.func,
  onKeyDown: PropTypes.func,
  disabled: PropTypes.bool
}

export default Trigger
