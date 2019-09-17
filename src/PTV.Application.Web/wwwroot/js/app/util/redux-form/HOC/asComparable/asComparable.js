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
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import styles from './styles.scss'
import cx from 'classnames'

const asComparable = ({
  DisplayRender,
  DisplayComponent,
  getDisplayRenderFromProps
} = {}) => WrappedComponent => {
  const InnerComponent = props => {
    const {
      isCompareMode,
      isReadOnly
    } = props
    if (getDisplayRenderFromProps) {
      DisplayRender = getDisplayRenderFromProps(props)
    }
    const DisplayField = DisplayComponent
      ? props => <DisplayComponent {...props} />
      : props => DisplayRender &&
        <WrappedComponent {...props} component={DisplayRender} /> ||
        <WrappedComponent {...props} />
    const sideOneClass = cx(
      styles.compareSide,
      styles.sideOne
    )
    const sideTwoClass = cx(
      styles.compareSide,
      styles.sideTwo
    )
    return (
      !isCompareMode
        ? isReadOnly
          ? <DisplayField {...props} />
          : <WrappedComponent {...props} />
        : isReadOnly
          ? <div className='row'>
            <div className='col-lg-12'>
              <div className={sideOneClass}>
                <DisplayField {...props} />
              </div>
            </div>
            <div className='col-lg-12'>
              <div className={sideTwoClass}>
                <DisplayField {...props} compare />
              </div>
            </div>
          </div>
          : <div className='row'>
            <div className='col-lg-12'>
              <div className={sideOneClass}>
                <WrappedComponent {...props} />
              </div>
            </div>
            <div className='col-lg-12'>
              <div className={sideTwoClass}>
                <WrappedComponent {...props} compare />
              </div>
            </div>
          </div>
    )
  }
  InnerComponent.propTypes = {
    isCompareMode: PropTypes.bool,
    isReadOnly: PropTypes.bool
  }
  return compose(
    injectFormName,
    withFormStates
  )(InnerComponent)
}

export default asComparable

