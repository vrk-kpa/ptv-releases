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
import { asyncCallApi } from 'Middleware/Api'
import { Select } from 'sema-ui-components'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import styles from './styles.scss'
import injectDefaultTexts from '../RenderSelect/injectDefaultTexts'
import debounce from 'debounce-promise'

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
  options,
  debounceDelay,
  onBlurResetsInput,
  dispatch,
  ...rest
}) => {
  const fetchData = async query => {
    try {
      const { data } = await asyncCallApi(url, getQueryData && getQueryData(query, rest) || query, dispatch)
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

  const debouncedFetchData = debounce(fetchData, debounceDelay)

  const loadOptions = async query => {
    if (!query || query.length < minLength) {
      return defaultOptions && (!query || query.length === 0) && {
        options: formatResponse
          ? formatResponse(defaultOptions)
          : defaultOptions.map(({ value, label }) => ({ value, label })),
        complete
      } || null
    }

    if (debounceDelay && debounceDelay > 0) {
      return await debouncedFetchData(query)
    } else {
      return await fetchData(query)
    }
  }

  const labelChildren = () => tooltip && <Tooltip tooltip={tooltip} />
  const wrapClass = cx(
    styles.autosizeOff
  )
  const onBlur = () => input.onBlur(input.value)
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
        value={options && options.get && options.get(input.value) ||
          input.value && (typeof input.value === 'object' ? input.value : { value: input.value }) ||
          null}
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
        onBlur={onBlur}
        {...rest}
        onBlurResetsInput={onBlurResetsInput} // because of issue #751 in react-select
        cache={false}
        filterOption={() => true} // because of issue #1259 in react-select
        error={error}
        errorMessage={errorMessage}
      />
    </div>
  )
}
RenderAsyncSelect.propTypes = {
  inlineLabel: PropTypes.string,
  debounceDelay: PropTypes.number,
  onBlurResetsInput: PropTypes.bool,
  input: PropTypes.element,
  url: PropTypes.string,
  formatResponse: PropTypes.func,
  onOptionsLoaded: PropTypes.func,
  defaultOptions: PropTypes.array,
  getQueryData: PropTypes.func,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string,
  onChangeObject: PropTypes.func,
  minLength: PropTypes.number,
  complete: PropTypes.bool,
  error: PropTypes.object,
  errorMessage: PropTypes.string,
  options: PropTypes.array,
  dispatch: PropTypes.func
}

RenderAsyncSelect.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderAsyncSelect)
