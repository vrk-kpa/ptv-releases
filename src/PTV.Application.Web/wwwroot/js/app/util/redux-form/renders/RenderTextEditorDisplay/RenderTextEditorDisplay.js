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

import React from 'react'
import PropTypes from 'prop-types'
import { EditorState } from 'draft-js'
import { Label } from 'sema-ui-components'
import NoDataLabel from 'appComponents/NoDataLabel'
import Tooltip from 'appComponents/Tooltip'
import Editor from 'draft-js-plugins-editor'
import styles from './styles.scss'
import { fieldPropTypes } from 'redux-form/immutable'

const onChange = () => {}

const RenderTextEditorDisplay = ({
  input,
  label,
  getCustomValue,
  compare,
  required,
  tooltip
}) => {
  const emptyEditorState = EditorState.createEmpty()
  const editorState = getCustomValue
    ? getCustomValue(input.value, emptyEditorState, compare)
    : input.value || emptyEditorState
  const editorStatePlainText = editorState && editorState.getCurrentContent &&
    editorState.getCurrentContent().getPlainText() || ''
  return (
    <div className={styles.editorDisplay}>
      <Label labelText={label} />
      {tooltip && <Tooltip tooltip={tooltip} indent='i0' withinText />}
      {editorStatePlainText
        ? <Editor
          editorState={editorState}
          onChange={onChange}
          readOnly
        />
        : <div><NoDataLabel required={required} /></div>
      }
    </div>
  )
}
RenderTextEditorDisplay.propTypes = {
  input: fieldPropTypes.input,
  getCustomValue: PropTypes.func,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  compare: PropTypes.bool,
  required: PropTypes.bool
}

export default RenderTextEditorDisplay

