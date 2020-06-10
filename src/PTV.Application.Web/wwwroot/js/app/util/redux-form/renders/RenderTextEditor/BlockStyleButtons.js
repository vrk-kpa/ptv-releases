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
import { compose } from 'redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import withState from 'util/withState'
import { PTVIcon } from 'Components'
import { Select } from 'sema-ui-components'
import cx from 'classnames'
import stylesCSS from './styles.scss'

const messages = defineMessages({
  titleText: {
    id: 'TextEditor.BlockStyle.Title.H3',
    defaultMessage: 'Otsikko'
  },
  bodyText: {
    id: 'TextEditor.BlockStyle.BodyText',
    defaultMessage: 'Leipätekstiä'
  }
})

const BlockStyleText = compose(
  injectIntl,
  withState({
    initialState: {
      textOption: 'body'
    }
  })
)(({ style, blockType, onToggle, updateUI, textOption, intl: { formatMessage } }) => {
  const handleTextOptionChange = ({ value, currentValue, style }) => {
    if (value !== currentValue) {
      onToggle(style)
      updateUI('textOption', value)
    }
  }
  const textOptions = [
    {
      value: 'title',
      label: formatMessage(messages.titleText)
    },
    {
      value: 'body',
      label: formatMessage(messages.bodyText)
    }
  ]
  const currentValue = blockType === style ? 'title' : 'body'
  return (
    <Select
      options={textOptions}
      onChange={props => handleTextOptionChange({ style, currentValue, ...props })}
      value={currentValue}
      className={stylesCSS.blockStyleText}
    />
  )
})

const BlockStyleButtons = ({
  onToggle,
  styles,
  editorState,
  counter
}) => {
  const blockStyles = {
    H1: { label: 'H1', style: 'header-one' },
    H2: { label: 'H2', style: 'header-two' },
    H3: { label: 'H3', style: 'header-three', type: 'header', name: 'header-three' },
    H4: { label: 'H4', style: 'header-four' },
    H5: { label: 'H5', style: 'header-five' },
    H6: { label: 'H6', style: 'header-six' },
    UL: { label: 'UL', style: 'unordered-list-item', name: 'icon-list-bullet' },
    OL: { label: 'OL', style: 'ordered-list-item', name: 'icon-list-number' }
  }
  const blockType = editorState
    .getCurrentContent()
    .getBlockForKey(editorState.getSelection().getStartKey())
    .getType()
  return (
    <div className={stylesCSS.textEditorBar}>
      <div className={stylesCSS.styleButtons}>
        {styles.map(style => blockStyles[style]).map(({ type, name, style }, k) => (
          <div key={name + '_' + k}
            className={cx(
              stylesCSS.styleButton,
              {
                [stylesCSS.styleButtonActive]: blockType === style,
                [stylesCSS.noSeparator]: type === 'header'
              }
            )}>
            {type === 'header'
              ? <BlockStyleText
                style={style}
                onToggle={onToggle}
                blockType={blockType}
              />
              : <PTVIcon name={name} onClick={() => onToggle(style)} />
            }
          </div>
        ))}
      </div>
      {counter}
    </div>
  )
}
BlockStyleButtons.propTypes = {
  onToggle: PropTypes.func,
  styles: PropTypes.array,
  editorState: PropTypes.object,
  counter: PropTypes.node
}

export default BlockStyleButtons
