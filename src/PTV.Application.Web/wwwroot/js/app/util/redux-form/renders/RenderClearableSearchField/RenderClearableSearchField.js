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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import styles from './styles.scss'
import filterProps from 'filter-invalid-dom-props'
import { fieldPropTypes } from 'redux-form/immutable'
import cx from 'classnames'
import { Label, Spinner } from 'sema-ui-components'
import { SearchIcon, CrossIcon } from 'appComponents/Icons'

class RenderSearchField extends Component {
  handleOnClear = () => this.props.input.onChange('')
  render () {
    const {
      input,
      componentClass,
      inputClass,
      iconClass,
      label,
      isSearching,
      hideSearchIcon,
      ...rest
    } = this.props
    const wrapClass = cx(styles.wrapp, componentClass)
    const crossIconClass = cx(styles.clear, styles.icon, iconClass)
    const searchIconClass = cx(styles.icon, iconClass)
    const spinnerClass = cx(styles.spinner, iconClass)
    return (
      <div className={wrapClass}>
        {label && <Label labelText={label} />}
        <input
          autoComplete='off'
          className={inputClass}
          {...input}
          {...filterProps(rest)}
        />
        {isSearching && <Spinner className={spinnerClass} />}
        {input.value === ''
          ? hideSearchIcon
            ? null
            : <SearchIcon size={16} componentClass={searchIconClass} />
          : <CrossIcon size={16} componentClass={crossIconClass} onClick={this.handleOnClear} />}
      </div>
    )
  }
}
RenderSearchField.propTypes = {
  input: fieldPropTypes.input,
  label: PropTypes.string,
  componentClass: PropTypes.string,
  inputClass: PropTypes.string,
  iconClass: PropTypes.string,
  onKeyDown: PropTypes.func,
  isSearching: PropTypes.bool,
  hideSearchIcon: PropTypes
}

export default compose(
  withFormFieldAnchor
)(RenderSearchField)
