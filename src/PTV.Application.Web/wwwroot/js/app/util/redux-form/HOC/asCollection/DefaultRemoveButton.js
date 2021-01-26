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
import { connect } from 'react-redux'
import { PTVIcon } from 'Components'
import styles from './styles.scss'
import cx from 'classnames'
import { gethideRemoveButton } from './selectors'
import { fieldArrayPropTypes } from 'redux-form/immutable'

class DefaultRemoveButton extends Component {
  shouldShow = () => {
    return !this.props.isReadOnly && !this.props.hideRemoveButton
  }
  handleOnRemove = () => {
    const { fields, index } = this.props
    if (fields.length <= 0) {
      return
    }
    fields.remove(index)
  }
  get collectionItemRemoveClass () {
    const shouldHideRemove = this.props.fields.length === 0 ||
                             this.props.innerShouldHideControls
    return cx(
      styles.collectionItemRemove,
      { [styles.hideRemove]: shouldHideRemove }
    )
  }
  render () {
    return this.shouldShow() && (
      <div className={this.collectionItemRemoveClass}>
        <PTVIcon
          name={'icon-cross'}
          onClick={this.handleOnRemove}
        />
      </div>
    )
  }
}

DefaultRemoveButton.propTypes = {
  index: PropTypes.number,
  fields: fieldArrayPropTypes.fields,
  innerShouldHideControls: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  hideRemoveButton: PropTypes.bool
}

export default compose(
  connect((state, ownProps) => ({
    hideRemoveButton: gethideRemoveButton(state, ownProps)
  }))
)(DefaultRemoveButton)
