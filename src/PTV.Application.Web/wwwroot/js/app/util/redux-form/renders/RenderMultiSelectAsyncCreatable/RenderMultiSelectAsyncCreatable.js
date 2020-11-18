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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Select } from 'sema-ui-components'
import { List, OrderedSet } from 'immutable'
import { compose } from 'redux'
import withFormFieldAnchor from 'util/redux-form/HOC/withFormFieldAnchor'
import Tooltip from 'appComponents/Tooltip'
import cx from 'classnames'
import styles from './styles.scss'
import { asyncCallApi } from 'Middleware/Api'
import { getEntitiesForIdsJS } from 'selectors/base'
import injectDefaultTexts from '../RenderSelect/injectDefaultTexts'
import { fieldPropTypes } from 'redux-form/immutable'

export const RenderMultiSelectAsyncCreatable = ({
  input,
  label,
  tooltip,
  getQueryData,
  onOptionsLoaded,
  formatResponse,
  url,
  getCustomValue,
  onChangeObject,
  complete = true,
  minLength = 2,
  defaultOptions,
  customOnChange,
  compare,
  options,
  meta: { touched, error },
  renderAsTags,
  createOptionText,
  dispatch,
  ...rest
}) => {
  const value = getCustomValue
  ? getCustomValue(input.value, '', compare)
  : input.value

  const labelChildren = () => tooltip && <Tooltip tooltip={tooltip} />
  const wrapClass = cx(
    styles.autosizeOff,
    styles.multiFix,
    {
      [styles.tagList]: renderAsTags
    }
  )
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
      const { data } = await asyncCallApi(url, getQueryData && getQueryData(query) || query, dispatch)
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

  return (
    <div className={wrapClass}>
      <Select.AsyncCreatable
        label={label}
        labelChildren={rest.labelPosition !== 'inside' && labelChildren()}
        {...input}
        value={options && options.size && value.size && getEntitiesForIdsJS(options, value) ||
          (value && value.size && value)}
        onChange={option => {
          onChangeObject && onChangeObject(option)
          customOnChange ? customOnChange(input, compare)(OrderedSet(option.map(x => x.value)))
            : input.onChange(option && OrderedSet(option.map(x => x.value) || OrderedSet()))
        }}
        loadOptions={loadOptions}
        multi
        size='full'
        clearable
        promptTextCreator={optionLabel => createOptionText + ' "' + optionLabel + '"'}
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
RenderMultiSelectAsyncCreatable.propTypes = {
  input: fieldPropTypes.input,
  meta: PropTypes.object.isRequired,
  label: PropTypes.string.isRequired,
  tooltip: PropTypes.string,
  options: ImmutablePropTypes.map.isRequired,
  renderAsTags: PropTypes.bool,
  createOptionText: PropTypes.string,
  getQueryData: PropTypes.func,
  onOptionsLoaded: PropTypes.func,
  formatResponse: PropTypes.func,
  url: PropTypes.string,
  getCustomValue: PropTypes.func,
  onChangeObject: PropTypes.func,
  complete: PropTypes.bool,
  minLength: PropTypes.number,
  defaultOptions: PropTypes.array,
  customOnChange: PropTypes.func,
  compare: PropTypes.func,
  dispatch: PropTypes.func
}

RenderMultiSelectAsyncCreatable.defaultProps = {
  labelPosition: 'top'
}

export default compose(
  withFormFieldAnchor,
  injectDefaultTexts
)(RenderMultiSelectAsyncCreatable)
