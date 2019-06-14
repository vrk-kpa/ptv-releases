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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import withState from 'util/withState'
import { Spinner } from 'sema-ui-components'

const withInit = ({
  init,
  shouldInitialize,
  isLoadingSelector
}) => InnerComponent => {
  class InitComponent extends PureComponent {
    componentWillMount = () => {
      // console.log('init', this.props.initialized, this.props)
      if (!this.props.initialized && typeof this.props.init === 'function') {
        // this.props.updateUI('initialized', true)
        this.props.init(this.props)
      }
    }

    componentWillReceiveProps (nextProps) {
      const pathIsDifferent = this.props.location && nextProps.location &&
        this.props.location.pathname !== nextProps.location.pathname
      if (typeof shouldInitialize === 'function' && shouldInitialize(this.props, nextProps, pathIsDifferent)) {
        nextProps.init(nextProps)
      }
    }

    render () {
      if (this.props.isLoading) {
        return <Spinner />
      }
      return <InnerComponent {...this.props} />
    }
  }

  InitComponent.propTypes = {
    init: PropTypes.func,
    updateUI: PropTypes.func,
    isLoading: PropTypes.bool,
    location: PropTypes.object,
    initialized: PropTypes.bool.isRequired
  }

  return compose(
    withState({
      initialState: {
        initialized: false
      }
    }),
    connect((state, props) => ({
      isLoading: (typeof isLoadingSelector === 'function') && isLoadingSelector(state, props) ||
        props.isLoading || false
    }), { init })
  )(InitComponent)
}

export default withInit
