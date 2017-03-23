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
import { Editor, EditorState, RichUtils, convertToRaw, convertFromRaw } from 'draft-js'
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent'
import PTVLabel from '../PTVLabel'
import PTVBlockStyleButtons from './PTVBlockStyleButtons'
import shortid from 'shortid'
// Styles
import styles from './styles.scss'
import stylesBlocks from './stylesBlocks.scss'

class PTVTextEditor extends Component {

  id = '';
  constructor (props) {
    super(props)

    this.handleLeave = this.handleLeave.bind(this)
    this.handleChange = this.handleChange.bind(this)
    this.toggleBlockStyleType = (type) => this.handleToggleBlockType(type)

    const editorStateValue = !this.props.value
                            ? EditorState.createEmpty()
                            : EditorState.createWithContent(convertFromRaw(JSON.parse(this.props.value)))

    this.state = { ...this.state,
      editorState: editorStateValue,
      charCount: editorStateValue ? (editorStateValue.getCurrentContent().getPlainText('')).length : 0,
      isChangingDirty : false
    }

    this.id = shortid.generate()
  };

  handleToggleBlockType (blockType) {
    this.handleChange(
      RichUtils.toggleBlockType(
        this.state.editorState, // write type to editorState
        blockType
      )
    )
  }

  handleChange (editorState) {
    if (typeof (this.props.changeCallback) === 'function') {
      this.props.changeCallback(JSON.stringify(convertToRaw(editorState.getCurrentContent())))
    }

    this.setState({
      charCount: editorState.getCurrentContent().getPlainText('').length,
      editorState: editorState
    })

    this.state.isChangingDirty = true
  }

  handleLeave () {
    if (typeof (this.props.blurCallback) === 'function') {
      this.props.blurCallback(JSON.stringify(convertToRaw(this.state.editorState.getCurrentContent())))
    }
    this.state.isChangingDirty = false
  }

  renderCounterLabel = (displayData, counterClass) => {
    return <PTVLabel
      labelClass={counterClass || 'counter'} >
      { displayData }
    </PTVLabel>
  }

  render () {
    const { disabled, readOnly } = this.props

    const value = this.state.isChangingDirty
                       ? this.state.editorState
                       : !this.props.value
                            ? EditorState.createEmpty()
                            : EditorState.createWithContent(convertFromRaw(JSON.parse(this.props.value)))

    return (
      <div className={this.props.componentClass}>

        <div className={this.props.labelClass}>
          { this.props.label
          ? <PTVLabel
            htmlFor={this.props.name + '_' + (this.props.id || this.id)}
            readOnly={readOnly}
            tooltip={this.props.tooltip}>
            {getRequiredLabel(this.props)}
          </PTVLabel>
          : null }
        </div>

        { !readOnly
          ? <PTVBlockStyleButtons
            editorState={value}
            onToggle={this.toggleBlockStyleType}
            />
        : null }

    <div
      className='RichEditor-root'
      onClick={this.focus}
      id={this.props.name + '_' + (this.props.id || this.id)} >

      <Editor
        editorState={value}
        onChange={this.handleChange}
        onBlur={this.handleLeave}
        placeholder={this.props.placeholder}
        readOnly={readOnly}
      />
    </div>

        { !readOnly
          ? this.renderCounterLabel(this.props.disabled
            ? this.state.charCount
            : (this.props.maxLength
                ? this.state.charCount + '/' + this.props.maxLength
                : null), this.props.counterClass)
          : null }
      </div>
    )
  }
};

PTVTextEditor.propTypes = {
  id: PropTypes.string,
  value: PropTypes.any,
  name: PropTypes.string.isRequired,
  tooltip: PropTypes.string,
  label: PropTypes.string,
  disabled: PropTypes.bool,
  readOnly: PropTypes.bool,
  changeCallback: PropTypes.func,
  blurCallback: PropTypes.func,
  placeholder: PropTypes.string,
  componentClass: PropTypes.string,
  counterClass:  PropTypes.string,
  labelClass: PropTypes.string,
  maxLength: PropTypes.number
  // validators: React.PropTypes.array,
  // size: PropTypes.string,
}

PTVTextEditor.defaultProps = {
  id: undefined,
  // size: 'full',
  value: EditorState.createEmpty()
}

export default composePTVComponent(PTVTextEditor)



