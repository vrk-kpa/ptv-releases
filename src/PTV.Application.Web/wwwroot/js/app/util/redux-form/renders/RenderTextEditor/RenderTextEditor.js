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
import { RichUtils, EditorState } from 'draft-js'
import Editor from 'draft-js-plugins-editor'
import createCounterPlugin from './util/counter'
import BlockStyleButtons from './BlockStyleButtons'
import styles from './styles.scss'
import { Label } from 'sema-ui-components'
import { compose } from 'redux'
import { withErrorMessage, withFormFieldAnchor } from 'util/redux-form/HOC'
import cx from 'classnames'
import withState from 'util/withState'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'

const counterPlugin = createCounterPlugin()
const { CharCounter } = counterPlugin

const RenderTextEditor = ({
  input,
  label,
  getCustomValue,
  localizedOnChange,
  error,
  errorMessage,
  disabled,
  updateUI,
  hasFocus,
  charLimit,
  visited,
  meta,
  compare,
  counter,
  placeholder,
  tooltip,
  required,
  ...rest
}) => {
  const editorState = (getCustomValue ? getCustomValue(input.value, EditorState.createEmpty(), compare) : input.value) ||
    EditorState.createEmpty()
  counterPlugin.initialize({
    getEditorState: () => editorState
  })
  let hidePlaceholder = false
  const contentState = editorState.getCurrentContent()
  if (!contentState.hasText()) {
    if (contentState.getBlockMap().first().getType() !== 'unstyled') {
      hidePlaceholder = true
    }
  }
  const textEditorClass = cx(
    styles.textEditor,
    {
      [styles.withCounter]: counter
    }
  )
  const textEditorBodyClass = cx(
    styles.textEditorBody,
    {
      [styles.disabled]: disabled,
      [styles.hasFocus]: hasFocus,
      [styles.hidePlaceholder]: hidePlaceholder,
      [styles.error]: !visited && meta.warning || meta.error
    }
  )
  const bottomBarClass = cx(
    styles.bottomBar,
    {
      [styles.reversed]: !errorMessage && counter,
      [styles.error]: !visited && meta.warning || meta.error
    }
  )

  const onChange = disabled && (() => {}) || (localizedOnChange && localizedOnChange(input, compare) || input.onChange)

  const handleOnChange = (newState) => {
    onChange && onChange(newState)
  }

  return (
    <div className={textEditorClass}>
      <div className={styles.textEditorHead}>
        <Label labelText={label} labelPosition='top' required={required}>
          {tooltip &&
            <Popup
              trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
              content={tooltip}
            />
          }
        </Label>
        {!disabled &&
          <BlockStyleButtons
            styles={['UL', 'OL']}
            editorState={editorState}
            onToggle={style => {
              handleOnChange(
                RichUtils.toggleBlockType(
                  editorState,
                  style
                )
              )
            }}
          />
        }
      </div>
      <div className={textEditorBodyClass}>
        <Editor
          {...input}
          onFocus={() => {
            if (!disabled) {
              updateUI({ hasFocus: true, visited: true })
            }
          }}
          onBlur={() => updateUI({ hasFocus: false })}
          editorState={editorState}
          onChange={handleOnChange}
          readonly={disabled}
          placeholder={placeholder}
          stripPastedStyles
        />
      </div>
      <div className={bottomBarClass}>
        {!visited && meta.warning && <div className={styles.errorMessage}>{errorMessage}</div> ||
          meta.error && errorMessage && <div className={styles.errorMessage}>{errorMessage}</div>}
        {counter &&
          <div className={styles.counter}>
            <span><CharCounter limit={charLimit} includeSpaces />/{charLimit}</span>
          </div>
        }
      </div>
    </div>
  )
}
RenderTextEditor.propTypes = {
  input: PropTypes.object.isRequired,
  label: PropTypes.string,
  getCustomValue: PropTypes.func,
  localizedOnChange: PropTypes.func,
  error: PropTypes.bool,
  compare: PropTypes.bool,
  hasFocus: PropTypes.bool,
  errorMessage: PropTypes.string,
  tooltip: PropTypes.string,
  disabled: PropTypes.bool,
  updateUI: PropTypes.func.isRequired,
  charLimit: PropTypes.number,
  counter: PropTypes.bool,
  placeholder: PropTypes.string,
  required: PropTypes.bool
}
RenderTextEditor.defaultProps = {
  charLimit: 2500,
  counter: true
}

// export default RenderTextEditor
export default compose(
  withState({
    initialState: {
      hasFocus: false,
      visited: false
    }
  }),
  withErrorMessage,
  withFormFieldAnchor
)(RenderTextEditor)
