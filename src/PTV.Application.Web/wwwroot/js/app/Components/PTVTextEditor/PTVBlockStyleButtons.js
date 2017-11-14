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
import React, { Component, PropTypes } from 'react'
import PTVIcon from 'Components/PTVIcon'
import cx from 'classnames'

class PTVBlockStyleButtons extends Component {

  render () {
    const { editorState } = this.props
    const selection = editorState.getSelection()
    const blockType = editorState
      .getCurrentContent()
      .getBlockForKey(selection.getStartKey())
      .getType()

    const BLOCK_TYPES = [
        // {label: 'H1', style: 'header-one'},
        // {label: 'H2', style: 'header-two'},
        // {label: 'H3', style: 'header-three'},
        // {label: 'H4', style: 'header-four'},
        // {label: 'H5', style: 'header-five'},
        // {label: 'H6', style: 'header-six'},
        { name: 'icon-list-bullet', label: 'UL', style: 'unordered-list-item' },
        { name: 'icon-list-number', label: 'OL', style: 'ordered-list-item' }
    ]

    return (
      <div className='RichEditor-controls'>
        { BLOCK_TYPES.map((type) =>
          <StyleButton
            key={type.label}
            active={type.style === blockType}
            name={type.name}
            onToggle={this.props.onToggle}
            style={type.style}
          />
        ) }
      </div>
    )
  }
};

PTVBlockStyleButtons.propTypes = {
  onToggle: PropTypes.func,
  editorState: PropTypes.any
}

PTVBlockStyleButtons.defaultProps = {
}

export default PTVBlockStyleButtons



class StyleButton extends Component {
  constructor () {
    super()
    this.onToggle = (e) => {
      e.preventDefault()
      this.props.onToggle(this.props.style)
    }
  }

  render () {
    let className = 'RichEditor-styleButton'
    if (this.props.active) {
      className = cx(className, 'RichEditor-activeButton')
    }

    return (
      <span className={className} onMouseDown={this.onToggle}>
        <PTVIcon name={this.props.name} />
      </span>
    )
  }
};

PTVBlockStyleButtons.propTypes = {
  onToggle: PropTypes.func,
  editorState: PropTypes.any,
  active: PropTypes.bool,
  name: PropTypes.string
}

PTVBlockStyleButtons.defaultProps = {
}

