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
import { compose } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import { RadioButton } from 'sema-ui-components'
import { getFormValues } from 'redux-form/immutable'
import { Map } from 'immutable'

const RadioButtonCell = ({
    name,
    id,
    formName,
    formProperty,
    checked,
    onChange
}) => {
  const onClickInside = () => {
    if (!checked && onChange && typeof onChange === 'function') {
      onChange(id)
    }
  }
  return (
    <div className={cx('cell', styles.cellRadio)} onClick={onClickInside}>
      <RadioButton checked={checked} />
    </div>
  )
}

RadioButtonCell.propTypes = {
  name: PropTypes.any,
  onChange: PropTypes.func,
  checked: PropTypes.bool.isRequired,
  id: PropTypes.string.isRequired,
  formName: PropTypes.string.isRequired,
  formProperty: PropTypes.string.isRequired
}

export default compose(
  connect((state, ownProps) => {
    const formState = getFormValues(ownProps.formName)(state) || Map()
    return {
      checked: ownProps.id === formState.get(ownProps.formProperty) || false
    }
  })
)(RadioButtonCell)
