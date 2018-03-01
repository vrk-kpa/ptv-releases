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
import { Select } from 'sema-ui-components'
import { List, OrderedSet } from 'immutable'
import { compose } from 'redux'
import { withFormFieldAnchor } from 'util/redux-form/HOC'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'

export const RenderSelect = ({
  input,
  label,
  tooltip,
  options,
  meta: { touched, error },
  renderAsTags,
  ...rest
}) => {
  let value = (input && input.value) || []
  if (List.isList(input.value)) {
    value = value.toJS()
  }
  if (OrderedSet.isOrderedSet(input.value)) {
    value = value.toJS()
  }
  const labelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    />
  )
  const wrapClass = cx(
    {
      [styles.tagList]: renderAsTags
    }
  )

  return (
    <div className={wrapClass}>
      <Select.Creatable
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
        clearable
        {...rest}
        onBlur={() => {}} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
      />
      { touched && error &&
        <div
          style={{
            color: '#C13832',
            lineHeight: '20px',
            fontSize: '13px',
            marginTop: '5px',
            marginBottom: '5px',
            textAlign: 'left',
            display: 'inline-block',
            fontWeight: '600'
          }}
        >
          {error.message}
        </div>
      }
    </div>
  )
}
RenderSelect.propTypes = {
  input: PropTypes.object.isRequired,
  meta: PropTypes.object.isRequired,
  label: PropTypes.string.isRequired,
  tooltip: PropTypes.string,
  options: PropTypes.array.isRequired,
  renderAsTags: PropTypes.bool
}

RenderSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withFormFieldAnchor
)(RenderSelect)
