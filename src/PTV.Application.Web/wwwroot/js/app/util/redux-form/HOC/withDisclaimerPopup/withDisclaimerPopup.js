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
import { compose } from 'redux'
import withState from 'util/withState'
import Popup from 'appComponents/Popup'

const withDisclaimerPopup = WrappedComponent => {
  class InnerComponent extends Component {
    constructor (props) {
      super(props)
      this.extRef = React.createRef()
    }
    render () {
      const {
        disclaimerMessage,
        isOpen,
        updateUI,
        title
      } = this.props
      const handleOnClose = () => updateUI('isOpen', false)
      return (
        <Popup
          trigger={<span ref={this.extRef}><WrappedComponent title={title} /></span>}
          extRef={this.extRef}
          open={isOpen}
          onClose={handleOnClose}
          position='top left'
          maxWidth='mW400'
          content={disclaimerMessage}
        />
      )
    }
  }

  InnerComponent.propTypes = {
    title: PropTypes.string,
    disclaimerMessage: PropTypes.object.required,
    isOpen: PropTypes.bool,
    updateUI: PropTypes.func
  }
  return compose(
    withState({
      key: 'disclaimer',
      initialState: {
        isOpen: true
      }
    })
  )(InnerComponent)
}

export default withDisclaimerPopup
