/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { Select } from 'sema-ui-components'
import debounce from 'debounce-promise'
import styles from './styles.scss'
import { asyncCallApi } from 'Middleware/Api'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import withErrorMessage from 'util/redux-form/HOC/withErrorMessage'
import injectDefaultTexts from '../RenderSelect/injectDefaultTexts'

const RenderAsyncSelectCreatable = props => {
  const {
    url,
    getQueryData,
    onOptionsLoaded,
    formatResponse,
    complete = true,
    debounceDelay,
    minLength = 2,
    defaultOptions,
    tooltip,
    labelPosition,
    onBlurResetsInput,
    onChangeObject,
    onBlurObject,
    input,
    options,
    dispatch,
    ...rest
  } = props

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

  const handleOnChange = option => {
    onChangeObject && onChangeObject(option)
    input.onChange(option && option.value)
  }

  const handleOnBlur = props => {
    onBlurObject && onBlurObject(props, input && input.value)
    input.onBlur(input.value)
  }

  const wrapClass = cx(
    styles.autosizeOff
  )

  return (
    <div className={wrapClass}>
      <Select.AsyncCreatable
        {...props}
        searchable
        ignoreAccents={false}
        labelChildren={labelPosition !== 'inside' && labelChildren()}
        size='full'
        clearable
        cache={false}
        filterOption={() => true} // because of issue #1259 in react-select
        loadOptions={loadOptions}
        onBlurResetsInput={onBlurResetsInput} // because of issue #751 in react-select
        onChange={handleOnChange}
        onBlur={handleOnBlur}
        value={options && options.get && options.get(input.value) ||
          input.value && (typeof input.value === 'object' ? input.value : { value: input.value }) ||
          null}
      />
    </div>
  )
}

RenderAsyncSelectCreatable.propTypes = {
  url: PropTypes.string,
  getQueryData: PropTypes.func,
  onOptionsLoaded: PropTypes.func,
  options: PropTypes.array,
  formatResponse: PropTypes.func,
  complete: PropTypes.bool,
  debounceDelay: PropTypes.number,
  minLength: PropTypes.number,
  defaultOptions: PropTypes.array,
  tooltip: PropTypes.string,
  labelPosition: PropTypes.string,
  onBlurResetsInput: PropTypes.bool,
  onChangeObject: PropTypes.func,
  onBlurObject: PropTypes.func,
  input: PropTypes.object,
  dispatch: PropTypes.func
}

RenderAsyncSelectCreatable.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withErrorMessage,
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderAsyncSelectCreatable)
