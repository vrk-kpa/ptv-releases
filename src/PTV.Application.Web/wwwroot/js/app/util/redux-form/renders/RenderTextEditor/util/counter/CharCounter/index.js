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
import unionClassNames from 'union-class-names'
import punycode from 'punycode'

class CharCounter extends Component {
  static propTypes = {
    theme: PropTypes.any
  };

  getCharCount (editorState) {
    const { includeSpaces } = this.props
    const decodeUnicode = (str) => punycode.ucs2.decode(str) // func to handle unicode characters
    const plainText = editorState.getCurrentContent().getPlainText('')
    // eslint-disable-next-line
    const regex = /(?:\r\n|\r|\n){2,}/g // match two or more consecutive characters or character groups (\r: carriage return, \n: new line)
    const cleanString = includeSpaces ? plainText.replace(regex, '\n')
      : plainText.replace(regex, '\n').trim() // replace multiple occurrence of above characters with single new line
    return decodeUnicode(cleanString).length
  }

  getClassNames (count, limit) {
    const { theme = {}, className } = this.props
    const defaultStyle = unionClassNames(theme.counter, className)
    const overLimitStyle = unionClassNames(theme.counterOverLimit, className)
    return count > limit ? overLimitStyle : defaultStyle
  }

  render () {
    const { store, limit } = this.props
    const count = this.getCharCount(store.getEditorState())
    const classNames = this.getClassNames(count, limit)

    return <span className={classNames}>{count}</span>
  }
}

export default CharCounter
