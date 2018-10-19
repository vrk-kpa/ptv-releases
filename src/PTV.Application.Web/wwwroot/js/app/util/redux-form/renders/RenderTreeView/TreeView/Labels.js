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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Checkbox, Label } from 'sema-ui-components'
import { Map } from 'immutable'
import styles from '../styles.scss'
import cx from 'classnames'
import Popup from 'appComponents/Popup'

export const NodeLabel = ({ node, name }) => {
  return <div>{node.get('name') || name}</div>
}

NodeLabel.propTypes = {
  node: PropTypes.any.isRequired,
  name: PropTypes.string
}

export const NodeLabelCheckBox = ({
  id,
  node,
  isRootLevel,
  onChange,
  checked,
  isCurrentNode = false,
  disabled,
  tabIndex,
  partiallyChecked,
  nodeLabel
}) => {
  const onNodeSelectFunc = (value) => {
    onChange(id, value)
  }
  const nodeLabelClass = cx(
    {
      [styles.isCurrentNode]: isCurrentNode
    }
  )
  return (
    <div className={nodeLabelClass}>
      <Checkbox
        disabled={disabled}
        checked={!!checked}
        small={!isRootLevel}
        id={id}
        label={nodeLabel || node.get('name')}
        onChange={onNodeSelectFunc}
        tabIndex={tabIndex}
        partiallyChecked={partiallyChecked}
      />
    </div>
  )
}

NodeLabelCheckBox.propTypes = {
  id: PropTypes.any,
  node: ImmutablePropTypes.map.isRequired,
  isRootLevel: PropTypes.bool,
  onChange: PropTypes.func.isRequired,
  checked: PropTypes.any,
  isCurrentNode: PropTypes.bool,
  disabled: PropTypes.bool,
  tabIndex: PropTypes.number,
  partiallyChecked: PropTypes.bool,
  nodeLabel: PropTypes.node
}

const renderDescription = (description, language, node) =>
  <div className={styles.detailDescription} key={`${node.get('code')}_${language}`}>
    <Label labelText={`${node.getIn(['label', language])} (${language.toUpperCase()})`} />
    <p>{description}</p>
  </div>

export const ToolTip = ({ node, tooltipLink }) => {
  const description = node.get('description') || Map()
  return (
    description.size &&
      <Popup
        position={'right default'}
        trigger={<div className={styles.nodeDetail}>{tooltipLink}</div>}
        maxWidth='mW300'
      >
        {description.keySeq().toList().map(lang => renderDescription(description.get(lang), lang, node))}
      </Popup> ||
      null
  )
}
