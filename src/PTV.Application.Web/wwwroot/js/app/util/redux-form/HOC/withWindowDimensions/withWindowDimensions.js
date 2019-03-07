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
import { WindowDimensionsProvider } from './context'
import { debounce } from 'lodash'

const withWindowDimensions = WrappedComponent => {
  class WindowDimensions extends Component {
    state = this.getDimensions();

    componentDidMount () {
      this.mounted = true
      window.addEventListener('resize', debounce(this.updateDimensions, 10))
    }

    componentWillUnmount () {
      window.removeEventListener('resize', this.updateDimensions)
      this.mounted = false
    }

    getDimensions () {
      const w = window
      const d = document
      const documentElement = d.documentElement
      const body = d.getElementsByTagName('body')[0]
      const width = w.innerWidth || documentElement.clientWidth || body.clientWidth
      const height = w.innerHeight || documentElement.clientHeight || body.clientHeight
      return { windowWidth: width, windowHeight: height }
    }

    updateDimensions = () => {
      if (this.mounted) {
        this.setState(this.getDimensions())
      }
    };

    render () {
      return (
        <WindowDimensionsProvider value={this.state}>
          <WrappedComponent {...this.props} />
        </WindowDimensionsProvider>
      )
    }
  }
  return WindowDimensions
}

export default withWindowDimensions
