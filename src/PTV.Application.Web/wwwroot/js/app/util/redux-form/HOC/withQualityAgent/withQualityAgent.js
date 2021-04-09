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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import styles from './styles.scss'
import {
  getQualityErrors,
  getHighlightErrors,
  getProcessedData
} from './selectors'
import { qualityCheck } from './actions'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { QualityAgentContext } from 'util/redux-form/HOC/withQualityAgentProvider/context'

const shouldCheckDefault = (value, previousValue) => {
  if (value.length && previousValue.length) {
    const currentWords = value.split(' ')
    const previousWords = previousValue.split(' ')
    return currentWords.length !== previousWords.length
  }
  return false
}

const getOptions = (options, props, providerOptions) => {
  return typeof options === 'function'
    ? options(props, providerOptions) || {}
    : options || {}
}

const getWarningId = (key, props, providerOptions, options) => {
  if (typeof key === 'function') {
    return key(
      props,
      providerOptions,
      getOptions(options, props, providerOptions)
    )
  }
  return key
}

const composeWarnings = compose(
  connect((state, { fieldId, useQualityAgent, formName, entityPrefix }) => ({
    warnings: useQualityAgent && getQualityErrors(state, { fieldId, formName, entityPrefix }) || null,
    highlightErrors: useQualityAgent && getHighlightErrors(state, { fieldId, formName }) || null,
    processedData: useQualityAgent && getProcessedData(state, { fieldId, formName })
  }))
)

const withQualityAgent = ({
  key,
  property,
  options,
  shouldCheck = shouldCheckDefault,
  getValue = value => value,
  customAction
} = {}) => WrappedComponent => {
  const Warnings = composeWarnings(WrappedComponent)

  class QualityAgentInnerComponent extends Component {
    qualityCheck = (event, value, previousValue, name) => {
      // console.log('validace', event, value, previousValue, name, this.props)
      this.props.useQualityAgent &&
      this.props.dispatch(
        qualityCheck({
          value,
          getValue,
          language: this.props.language || this.props.contentLanguage,
          property,
          options: getOptions(options, this.props, this.context),
          props: this.props,
          providerOptions: this.context,
          customAction
        })
      )
    }

    // qualityCheckOnChange = (event, value, previousValue, name) => {
    //   // if (this.props.useQualityAgent &&
    //   //   shouldCheck(getValue(value, this.props.language), getValue(previousValue, this.props.language))
    //   // ) {
    //   // console.log('validace', getValue(value, 'fi'), getValue(previousValue, 'fi'), name, key)
    //   this.qualityCheck(event, value, previousValue, name)
    //   // }
    // }

    render () {
      return <div className={styles.withQualityAgent}>
        <Warnings
          {...this.props}
          onChange={this.qualityCheck}
          // onBlur={this.qualityCheck}
          fieldId={getWarningId(key, this.props, this.context, options)}
          entityPrefix={this.context.entityPrefix}
        />
      </div>
    }
  }

  QualityAgentInnerComponent.propTypes = {
    dispatch: PropTypes.func.isRequired,
    useQualityAgent: PropTypes.bool,
    language: PropTypes.string,
    formName: PropTypes.string
  }

  QualityAgentInnerComponent.contextType = QualityAgentContext

  return compose(
    injectFormName
  )(QualityAgentInnerComponent)
}

export default withQualityAgent
