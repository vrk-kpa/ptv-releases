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
import { TextField, Spinner, Label } from 'sema-ui-components'
// import callApi from 'util/callApi'
import styles from './styles.scss'
import cx from 'classnames'
import { Popup } from 'appComponents'
import { PTVIcon } from 'Components'
import { compose } from 'redux'
import { withErrorMessage, withFormFieldAnchor, withFormStates } from 'util/redux-form/HOC'
import withState from 'util/withState'

const RenderUrlChecker = ({
  isChecking,
  url,
  urlExists,
  updateUI,
  input,
  tooltip,
  placeholder,
  label,
  inlineLabel,
  buttonText,
  getCustomValue,
  localizedOnChange,
  isCompareMode,
  splitView,
  isReadOnly,
  compare,
  required,
  error,
  disabled,
  errorMessage,
  ...rest
}) => {
  const getValue = () => getCustomValue ? getCustomValue(input.value, '', compare) : input.value
  const checkUrl = async e => {
    e.preventDefault()
    // const newUrl = getValue()
    // if (!newUrl || url === input.value) {
    //   return
    // }
    // updateUI({
    //   isChecking: true,
    //   url: input.value
    // })
    // try {
    //   const { data: { urlExists } } = await callApi(
    //     'url/Check', {
    //       urlAddress: newUrl
    //     })
    //   // if (urlExists) {
    //   // inputChange(url)
    //   updateUI({ urlExists })
    //   // } else {
    //   // this.setState({ url: '' })
    //   // this.rest.input.onChange(null)
    //   // }
    // } catch (err) {
    //   console.log(err)
    //   // inputChange(null)
    //   return
    // } finally {
    //   updateUI({ isChecking: false })
    // }
  }
  const textInputClass = cx(
    styles.withIcon,
    styles.withCounter
  )
  const basicClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
  const withCheckClass = isCompareMode || splitView ? 'col-lg-22' : 'col-lg-11'
  // const checkerClass = isCompareMode || splitView ? 'col-lg-2' : 'col-lg-1'
  return (
    <div>
      <Label labelText={label} labelPosition='top' required={required}>
        {tooltip && <Popup trigger={<PTVIcon name='icon-tip2' width={30} height={30} />} content={tooltip} />}
      </Label>
      <div className='row'>
        <div className={!isReadOnly && urlExists !== null && withCheckClass || basicClass}>
          <div className={textInputClass}>
            <TextField
              {...input}
              value={getValue()}
              onChange={localizedOnChange && localizedOnChange(input, compare) || input.onChange}
              onBlur={checkUrl}
              children={isChecking && <Spinner className={styles.spinner} />}
              placeholder={placeholder}
              label={inlineLabel && label}
              counter
              size='full'
              maxLength={500}
              error={error}
              disabled={disabled}
              errorMessage={errorMessage}
              // {...rest}
            />
          </div>
        </div>
        { /* !isReadOnly && urlExists !== null &&
          <div className={checkerClass}>
            <PTVIcon name={(!urlExists && 'icon-cross' || urlExists && 'icon-check')} />
          </div> */
        }
        { /* <div className={isCompareMode ? 'col-lg-8' : 'col-md-6'}>
          <Button
            small
            secondary
            children={buttonText}
            onClick={checkUrl}
            disabled={!url}
          />
        </div> */ }
      </div>
    </div>
  )
}

RenderUrlChecker.propTypes = {
  input: PropTypes.object,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string,
  label: PropTypes.string,
  inlineLabel: PropTypes.string,
  buttonText: PropTypes.string,
  url: PropTypes.string,
  meta: PropTypes.object,
  ui: PropTypes.object,
  updateUI: PropTypes.func,
  getCustomValue: PropTypes.func,
  localizedOnChange: PropTypes.func,
  isCompareMode: PropTypes.bool,
  required: PropTypes.bool,
  error: PropTypes.bool,
  compare: PropTypes.bool,
  urlExists: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  splitView: PropTypes.bool,
  isChecking: PropTypes.bool,
  disabled: PropTypes.bool,
  errorMessage: PropTypes.string
}

export default compose(
  withState({
    initialState: {
      isChecking: false,
      url: '',
      urlExists: null
    }
  }),
  withErrorMessage,
  withFormFieldAnchor,
  withFormStates
)(RenderUrlChecker)
