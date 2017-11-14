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
import styles from '../styles.scss'
import { Popup } from 'appComponents'

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
  ...props
}) => {
  const onNodeSelectFunc = (value) => {
    onChange(id, value)
  }

  return (
    <div>
      <Checkbox
        {...props}
        noCheckableLabel
        small={!isRootLevel}
        id={id}
        nodeId={id}
        label={node.get('name')}
        onChange={onNodeSelectFunc}
      />
    </div>
  )
}

NodeLabelCheckBox.propTypes = {
  id: PropTypes.any,
  node: ImmutablePropTypes.map.isRequired,
  isRootLevel: PropTypes.bool,
  onChange: PropTypes.func.isRequired
}

const renderDescription = (description, language, node) =>
  <div className={styles.detailDescription}>
    <Label labelText={`${node.getIn(['label', language])} (${language.toUpperCase()})`} />
    <p>{description}</p>
  </div>

export const ToolTip = ({ node, tooltipLink }) =>
  node.get('description') && node.get('description').size &&
  <Popup
    position={'right default'}
    trigger={<div className={styles.nodeDetail}>{tooltipLink}</div>}
    maxWidth='mW300'
  >{
    node.get('description').map((description, lang) => renderDescription(description, lang, node))
  }
  </Popup> ||
  null
