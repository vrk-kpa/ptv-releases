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
// import { createContext } from 'react'
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import withState from 'util/withState'
import shortid from 'shortid'
import createCachedSelector from 're-reselect'

const getContextValue = createCachedSelector(
  ({ withError }) => withError || false,
  ({ withWarning }) => withWarning || false,
  ({ updateUI }) => updateUI,
  (withError, withWarning, updateUI) => {
    return {
      withError,
      withWarning,
      updateUI
    }
  }
)(
  ({ fieldName }) => fieldName || 'errorProvider',
)

const ErrorContext = React.createContext({})

class Provider extends Component {
  // constructor (props) {
  //   super(props)
  //   // this.myId = shortid.generate()
  // }

  // componentDidMount () {
  //   this.props.updateUI('setup', this.myId)
  //   // this.props.updateUI(['value', 'updateUI'], this.update)
  // }

  update = (...args) => {
    if (!this.updateDisabled) {
      // console.log('called', this.props, args, this.myId)
      this.props.updateUI(...args)
    } else {
      // console.log('not called', this.props, this.myId)
    }
  }

  componentWillUnmount () {
    this.updateDisabled = true
  }

  getValue = () => {
    // const { value } = this.props
    // if (typeof value.updateUI !== 'function') {
    //   console.log('add update function', this.myId, value, this.props)
    //   value.updateUI = this.update
    //   // return { ...value, updateUI: this.update }
    //   // value[this.myId] = this.props.fieldName
    //   // window[fieldName]
    //   return value
    // } else {
    //   console.log('already has', this.myId, value, this.props)
    //   return value
    // }
    // return this.props.value && this.props.value.set('updateUI', this.update).toJS() || {}

    return getContextValue({ ...this.props.value, updateUI: this.update, fieldName: this.props.fieldName })
  }

  render () {
    const { children } = this.props
    // if (typeof value.get('updateUI') !== 'function') {
    //   value.merge({ updateUI: this.update, [this.myId]: this.props.fieldName })
    //   // value[this.myId] = this.props.fieldName
    //   // window[fieldName]
    //   console.log('add update function', this.myId, value, this.props)
    // } else {
    //   console.log('already has', this.myId, value, this.props)
    // }
    return <ErrorContext.Provider value={this.getValue()}>
      {children}
    </ErrorContext.Provider>
  }
}

// const Provider = ({ children, updateUI, value = {} }) => {
//   if (typeof value.updateUI !== 'function') {
//     value.updateUI = updateUI
//   }
//   return <ErrorContext.Provider value={value}>
//     {children}
//   </ErrorContext.Provider>
// }

Provider.propTypes = {
  children: PropTypes.node,
  updateUI: PropTypes.func,
  value: PropTypes.object
}

export const ErrorUpdateProvider = compose(
  withState({
    // key: ({ fieldName }) => fieldName || 'errorProvider',
    // redux: true,
    // keepImmutable: true,
    initialState: {
      value: {
        withError: false,
        withWarning: false
      }
    }
  }),
)(Provider)

export const ErrorProvider = props =>
  <ErrorContext.Provider value={getErrorValue(props)}>
    {props.children}
  </ErrorContext.Provider>

ErrorProvider.propTypes = {
  children: PropTypes.node
}

const getErrorValue = ({ error, errorMessage, warning }) => ({
  withError: !!(error && errorMessage),
  withWarning: !!warning
})

class UpdateComponent extends Component {
  componentDidMount () {
    const error = getErrorValue(this.props)
    if (this.props.withError !== error.withError || this.props.withWarning !== error.withWarning) {
      this.props.updateUI({ value: { withError: error.withError, withWarning: error.withWarning } })
    }
  }

  componentDidUpdate (prevProps) {
    const error = getErrorValue(this.props)
    if (this.props.withError !== error.withError || this.props.withWarning !== error.withWarning) {
      this.props.updateUI({ value: { withError: error.withError, withWarning: error.withWarning } })
    }
  }

  render () {
    return null
  }
}

UpdateComponent.propTypes = {
  withError: PropTypes.bool,
  withWarning: PropTypes.bool,
  updateUI: PropTypes.func.isRequired
}

export const ErrorUpdateConsumer = props => (
  <ErrorContext.Consumer >
    {context => <UpdateComponent {...props} {...context} />}
  </ErrorContext.Consumer>
)

export const ErrorConsumer = ErrorContext.Consumer
