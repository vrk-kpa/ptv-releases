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
import { Select } from 'sema-ui-components'
import { List, OrderedSet } from 'immutable'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import styles from './styles.scss'
import injectDefaultTexts from '../RenderSelect/injectDefaultTexts'
import { fieldPropTypes } from 'redux-form/immutable'
import withNotificationIcon from 'util/redux-form/HOC/withNotificationIcon'

const InvalidLabel = compose(
  withNotificationIcon
)(({ label }) => <span>{label}</span>)

const valueRenderer = (option, tooltip) => option &&
  (option.invalid &&
    <span className={styles.invalid}>
      <InvalidLabel
        label={option.label}
        notificationText={tooltip}
        shouldShowNotificationIcon={tooltip}
        iconClass={styles.notificationIcon}
      />
    </span> || option.label) || null

export const RenderMultiSelect = ({
  input,
  label,
  tooltip,
  getCustomValue,
  options,
  renderAsTags,
  renderAsRemovable,
  isReadOnly,
  display,
  error,
  errorMessage,
  warning,
  invalidItemTooltip,
  ...rest
}) => {
  let value = getCustomValue
    ? getCustomValue(input.value, '')
    : (input && input.value) || []

  if (List.isList(value)) {
    value = value.toJS()
  }
  if (OrderedSet.isOrderedSet(value)) {
    value = value.toJS()
  }
  const labelChildren = () => tooltip && <Tooltip tooltip={tooltip} />
  const wrapClass = cx(
    styles.autosizeOff,
    styles.multiFix,
    {
      [styles.tagList]: renderAsTags,
      [styles.removable]: renderAsRemovable,
      [styles.display]: (isReadOnly || display)
    }
  )
  return (
    <div className={wrapClass}>
      <Select
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
        {...input}
        value={value}
        onChange={obj => {
          input.onChange(obj && OrderedSet(obj.map(x => x.value) || OrderedSet())) // This is just awful tbh
        }}
        options={options}
        multi
        size='full'
        searchable
        clearable
        valueRenderer={(option) => valueRenderer(option, invalidItemTooltip)}
        {...rest}
        onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
        error={error}
        errorMessage={errorMessage}
        warning={warning}
      />
      {warning && <div className={styles.bottomBar}>{warning}</div>}
    </div>
  )
}
RenderMultiSelect.propTypes = {
  input: fieldPropTypes.input,
  error: PropTypes.bool,
  errorMessage: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  invalidItemTooltip: PropTypes.string,
  options: PropTypes.array,
  getCustomValue: PropTypes.func,
  renderAsTags: PropTypes.bool,
  renderAsRemovable: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  display: PropTypes.bool,
  warning: PropTypes.node
}

RenderMultiSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderMultiSelect)
