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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { PTVIcon } from 'Components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import styles from './styles.scss'
import filterProps from 'filter-invalid-dom-props'
import { fieldPropTypes } from 'redux-form/immutable'

const CrossIcon = props => (
  <PTVIcon
    name='icon-cross'
    componentClass={styles.icon}
    {...props}
  />
)
const SearchIcon = props => (
  <PTVIcon
    name='icon-search'
    componentClass={styles.icon}
    {...props}
  />
)

class RenderSearchField extends Component {
  handleOnClear = () => this.props.input.onChange('')
  render () {
    const {
      input,
      ...rest
    } = this.props
    return (
      <div className={styles.wrapp}>
        <input
          {...input}
          {...filterProps(rest)}
        />
        {input.value === ''
          ? <SearchIcon />
          : <CrossIcon onClick={this.handleOnClear} />}
      </div>
    )
  }
}
RenderSearchField.propTypes = {
  input: fieldPropTypes.input,
  label: PropTypes.string,
  onKeyDown: PropTypes.func
}

export default compose(
  withFormFieldAnchor
)(RenderSearchField)
