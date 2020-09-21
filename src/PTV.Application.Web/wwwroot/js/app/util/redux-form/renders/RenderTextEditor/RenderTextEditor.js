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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import {
  RichUtils,
  EditorState,
  CompositeDecorator,
  Editor
} from 'draft-js'
// import Editor from 'draft-js-plugins-editor'
import createCounterPlugin from './util/counter'
import BlockStyleButtons from './BlockStyleButtons'
import styles from './styles.scss'
import { Label } from 'sema-ui-components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import cx from 'classnames'
import withState from 'util/withState'
import Tooltip from 'appComponents/Tooltip'
import { fieldPropTypes } from 'redux-form/immutable'
import { withProps } from 'recompose'
import { getPlainText } from 'util/helpers'

const counterPlugin = createCounterPlugin()
const { CharCounter } = counterPlugin

const HighlightComponent = ({ children }) => (
  <span className={styles.highlight}>{children}</span>
)
HighlightComponent.propTypes = {
  children: PropTypes.any
}

const getTextToDecorate = (
  blockCharSize,
  callback,
  errors,
  totalCount
) => {
  // loop through errors
  errors.forEach(error => {
    const startIndex = error.get('start')
    const length = error.get('length')
    if (totalCount <= startIndex && totalCount + blockCharSize > startIndex) {
      const startPos = startIndex - totalCount
      const endPos = startPos + length
      return callback(startPos, endPos)
    }
  })
}

const getSuccessiveEmptyBlockCount = (key, contentState, count = 1) => {
  const blockAfter = contentState.getBlockAfter(key)
  const hasText = !!(blockAfter && blockAfter.get('text'))
  if (blockAfter && !hasText) {
    const nextKey = blockAfter.get('key')
    return getSuccessiveEmptyBlockCount(nextKey, contentState, ++count)
  }
  return count
}

const CounterComponent = ({ charLimit, counterOutside }) => (
  <div className={cx(styles.counter, { [styles.counterOutside]: counterOutside })}>
    <span><CharCounter limit={charLimit} includeSpaces />/{charLimit}</span>
  </div>
)
CounterComponent.propTypes = {
  charLimit: PropTypes.number,
  counterOutside: PropTypes.bool
}

class RenderTextEditor extends Component {
  handleOnChange = (newState) => {
    const { disabled, input, compare, customOnChange } = this.props
    if (!disabled) {
      const onChange = customOnChange && customOnChange(input, compare) || input.onChange
      onChange && onChange(newState && EditorState.set(
        newState, { decorator: null }
      ))
    }
  }

  handleOnBlur = (event, newState) => {
    const { disabled, updateUI, compare, input, customOnBlur } = this.props
    updateUI({ hasFocus: false })
    if (!disabled) {
      const onBlur = (customOnBlur && customOnBlur(input, compare) || input.onBlur)
      onBlur(event, newState)
    }
  }

  render () {
    const {
      input,
      label,
      getCustomValue,
      error,
      errorMessage,
      warning,
      disabled,
      updateUI,
      hasFocus,
      charLimit,
      compare,
      counter,
      placeholder,
      tooltip,
      required,
      highlightErrors,
      processedData,
      hideBlockStyleButtons,
      styleAs
    } = this.props
    const editorState = (
      getCustomValue
        ? getCustomValue(input.value, EditorState.createEmpty(), compare)
        : input.value
    ) || EditorState.createEmpty()
    counterPlugin.initialize({
      getEditorState: () => editorState
    })

    let totalCount = 0
    const highlightStrategy = (contentBlock, callback, contentState) => {
      const blockCharSize = contentBlock.get('characterList').size
      getTextToDecorate(blockCharSize, callback, highlightErrors, totalCount)
      // empty blocks are not passed to compositeDecorator
      // we have to count them manually in order to maintain
      // correct position of highlighting
      const successiveEmptyBlockCount = getSuccessiveEmptyBlockCount(contentBlock.get('key'), contentState)
      totalCount = totalCount + blockCharSize + successiveEmptyBlockCount
    }

    const customDecorators = [
      {
        strategy: highlightStrategy,
        component: HighlightComponent
      }
    ]

    const editorTextWithoutNewLines = getPlainText(editorState).replace(/\r\n|\r|\n/g, ' ')
    const editorStateWithHighlight = highlightErrors && highlightErrors.size &&
      processedData === editorTextWithoutNewLines
      ? EditorState.set(
        editorState, {
          decorator: new CompositeDecorator(customDecorators)
        }
      )
      : editorState

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
      },
      styles[styleAs]
    )
    const textEditorBodyClass = cx(
      styles.textEditorBody,
      {
        [styles.disabled]: disabled,
        [styles.hasFocus]: hasFocus,
        [styles.hidePlaceholder]: hidePlaceholder,
        [styles.error]: error,
        [styles.warning]: warning
      }
    )
    const bottomBarClass = cx(
      styles.bottomBar,
      {
        [styles.reversed]: !errorMessage && !warning && counter,
        [styles.error]: error
      }
    )
    const showBlockStyleButtons = !disabled && !hideBlockStyleButtons
    return (
      <div className={textEditorClass}>
        <div className={styles.textEditorHead}>
          <Label labelText={label} labelPosition='top' required={required}>
            {tooltip && <Tooltip tooltip={tooltip} />}
          </Label>
        </div>
        <div className={textEditorBodyClass}>
          {showBlockStyleButtons &&
            <BlockStyleButtons
              counter={counter && <CounterComponent charLimit={charLimit} />}
              styles={['H3', 'UL', 'OL']}
              editorState={editorStateWithHighlight}
              onToggle={style => {
                this.handleOnChange(
                  RichUtils.toggleBlockType(
                    editorState,
                    style
                  )
                )
              }}
            />
          }
          <Editor
            {...input}
            onFocus={(e) => {
              if (!disabled) {
                updateUI({ hasFocus: true, visited: true })
              }
              input.onFocus(e)
            }}
            onBlur={this.handleOnBlur}
            editorState={editorStateWithHighlight}
            onChange={this.handleOnChange}
            readOnly={disabled}
            placeholder={placeholder}
            stripPastedStyles
          />
        </div>
        <div className={bottomBarClass}>
          {((error && errorMessage) || warning) &&
            <div>
              {error && <div className={styles.errorMessage}>{errorMessage}</div>}
              {warning}
            </div>
          }
          {hideBlockStyleButtons && counter && <CounterComponent charLimit={charLimit} counterOutside />}
        </div>
      </div>
    )
  }
}

RenderTextEditor.propTypes = {
  input: fieldPropTypes.input,
  label: PropTypes.string,
  getCustomValue: PropTypes.func,
  customOnChange: PropTypes.func,
  customOnBlur: PropTypes.func,
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
  required: PropTypes.bool,
  warning: PropTypes.node,
  highlightErrors: ImmutablePropTypes.list,
  processedData: PropTypes.string,
  hideBlockStyleButtons: PropTypes.bool,
  styleAs: PropTypes.oneOf(['textarea', 'textinput'])
}
RenderTextEditor.defaultProps = {
  charLimit: 2500,
  counter: true
}

export default compose(
  withState({
    initialState: {
      hasFocus: false,
      visited: false
    }
  }),
  withProps(props => ({
    isErrorWarning: (meta, props) => !meta.touched
  })),
  withErrorMessage,
  withFormFieldAnchor
)(RenderTextEditor)
