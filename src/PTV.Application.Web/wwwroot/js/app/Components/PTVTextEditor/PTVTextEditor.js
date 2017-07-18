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
import cx from 'classnames'

export const getEditorPlainText = (data) => {
  const editorContent = !data
    ? EditorState.createEmpty()
    : EditorState.createWithContent(convertFromRaw(JSON.parse(data)))
  return editorContent.getCurrentContent().getPlainText('')
}

class PTVTextEditor extends Component {

  id = '';
  constructor (props) {
    super(props)

    this.handleLeave = this.handleLeave.bind(this)
    this.handleChange = this.handleChange.bind(this)
    this.handleBeforeInput = this.handleBeforeInput.bind(this)
    this.handlePastedText = this.handlePastedText.bind(this)
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

  updateState = editorState => {
    if (typeof (this.props.changeCallback) === 'function') {
      this.props.changeCallback(JSON.stringify(convertToRaw(editorState.getCurrentContent())))
    }

    this.setState({
      charCount: editorState.getCurrentContent().getPlainText('').length,
      editorState: editorState
    })

    this.state.isChangingDirty = true
  }

  // not used since PTV-1825
  handleChange = editorState => {
    if (this.props.maxLength) {
      const contentState = editorState.getCurrentContent()
      const oldContent = this.state.editorState.getCurrentContent()
      if (contentState === oldContent || contentState.getPlainText().length <= this.props.maxLength) {
        this.updateState(editorState)
      }
    } else {
      this.updateState(editorState)
    }
  }

  handleLeave () {
    if (typeof (this.props.blurCallback) === 'function') {
      this.props.blurCallback(JSON.stringify(convertToRaw(this.state.editorState.getCurrentContent())))
    }
    this.state.isChangingDirty = false
  }

  handlePastedText = (chars) => {
    const totalLength = this.state.editorState.getCurrentContent().getPlainText().length + chars.length
    return this.props.maxLength && totalLength > this.props.maxLength
  }

  handleBeforeInput = (chars) => {
    const totalLength = this.state.editorState.getCurrentContent().getPlainText().length + chars.length
    return this.props.maxLength && totalLength > this.props.maxLength
  }

  renderCounterLabel = (displayData, counterClass) => {
    return <PTVLabel
      labelClass={counterClass || 'counter text-editor'} >
      { displayData }
    </PTVLabel>
  }

  render () {
    const { disabled, readOnly } = this.props

    const plainTextValue = this.state.editorState.getCurrentContent().getPlainText()

    if (readOnly && plainTextValue === '') {
      return null
    }

    return (
      <div className={cx(this.props.componentClass, 'ptv-texteditor')}>

        <div className='ptv-texteditor-header'>
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

          { !readOnly && !disabled
            ? <PTVBlockStyleButtons
              editorState={this.state.editorState}
              onToggle={this.toggleBlockStyleType}
              />
          : null }
        </div>

        <div
          className={cx('RichEditor-root', { 'readOnly': this.props.readOnly, 'disabled': this.props.disabled })}
          onClick={this.focus}
          id={this.props.name + '_' + (this.props.id || this.id)} >

          <Editor
            editorState={this.state.editorState}
            // change for PTV-1825
            onChange={this.updateState}
            onBlur={this.handleLeave}
            placeholder={this.props.placeholder}
            readOnly={readOnly}
            // handleBeforeInput={this.handleBeforeInput}
            // handlePastedText={this.handlePastedText}
            stripPastedStyles
          />
          { !readOnly
          ? this.renderCounterLabel(this.props.disabled
            ? this.state.charCount
            : (this.props.maxLength
                ? this.state.charCount + '/' + this.props.maxLength
                : null), this.props.counterClass)
          : null }

        </div>
        <ValidatePTVComponent {...this.props} valueToValidate={plainTextValue} />
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
  maxLength: PropTypes.number,
  validators: React.PropTypes.array
}

PTVTextEditor.defaultProps = {
  id: undefined,
  value: EditorState.createEmpty()
}

export default composePTVComponent(PTVTextEditor)

