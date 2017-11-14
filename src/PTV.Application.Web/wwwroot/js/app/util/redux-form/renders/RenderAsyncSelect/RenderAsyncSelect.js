/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import callApi from 'util/callApi'
import { Select } from 'sema-ui-components'
import { noop } from 'lodash'
import { compose } from 'redux'
import { withErrorMessage, withFormFieldAnchor } from 'util/redux-form/HOC'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'

const RenderAsyncSelect = ({
  input,
  url,
  formatResponse,
  onOptionsLoaded,
  defaultOptions,
  getQueryData,
  inlineLabel,
  label,
  tooltip,
  placeholder,
  onChangeObject,
  minLength = 2,
  complete = true,
  error,
  errorMessage,
  ...rest
}) => {
  const loadOptions = async query => {
    if (!query || query.length < minLength) {
      return defaultOptions && (!query || query.length === 0) && {
        options: formatResponse
          ? formatResponse(defaultOptions)
          : defaultOptions.map(({ value, label }) => ({ value, label })),
        complete
      } || null
    }
    try {
      const { data } = await callApi(url, getQueryData && getQueryData(query) || query)
      if (onOptionsLoaded) {
        onOptionsLoaded(data)
      }
      return {
        options: formatResponse
          ? formatResponse(data)
          : data.map(({ value, label }) => ({ value, label })),
        complete
      }
    } catch (err) {
      console.log('Async select err:\n', err)
    }
  }
  const labelChildren = () => (
    tooltip &&
    <Popup
      trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
      content={tooltip}
    />
  )
  const wrapClass = cx(
    styles.autosizeOff
  )
  return (
    <div className={wrapClass}>
      <Select.Async
        searchable
        ignoreAccents={false}
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
        placeholder={placeholder}
        loadOptions={loadOptions}
        {...input}
        value={rest.options && rest.options.get && rest.options.get(input.value) || input.value || null}
        onChange={option => {
          onChangeObject && onChangeObject(option)
          input.onChange(
            option
              ? option.value
              : null
          )
        }}
        size='full'
        clearable
        {...rest}
        onBlur={noop} // onBlurResetsInput doesn't work react-select #751
        onBlurResetsInput={false}
      />
      { error &&
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
        {errorMessage}
      </div>
    }
    </div>
  )
}
RenderAsyncSelect.propTypes = {
  inlineLabel: PropTypes.string
}

RenderAsyncSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor
)(RenderAsyncSelect)
