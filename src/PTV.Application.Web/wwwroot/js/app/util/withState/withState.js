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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { withStateHandlers } from 'recompose'
import { compose } from 'redux'
import { isObject, isArray, isString, isFunction } from 'lodash'
import { setInUIState, mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import shortid from 'shortid'
import { fromJS, Map } from 'immutable'
import { getParameterFromProps, getParameters } from 'selectors/base'
import createCachedSelector from 're-reselect'

const allowedArgumentsMessage =
  `please check calling of updateUI.\n` +
  `Allowed arguments are:\n` +
  `   updateUI(stateToMerge: Object),\n` +
  `   updateUI(path: Array | String, value: Any)\n`
const throwNotObjectError = state => {
  throw Error(
    `\n\nwithState HOC error, expected object instead got updateUI(${typeof stateToMerge})\n` +
    allowedArgumentsMessage
  )
}
const throwPathValueError = (path, value) => {
  throw Error(
    `\n\nwithState HOC error, expected path and value instead got updateUI(${typeof path}, ${typeof value})\n` +
    allowedArgumentsMessage
  )
}

const getUIState = state => state.get('ui2') || Map()

const _calculateKey = (key, props) => {
  if (isFunction(key)) {
    const newkey = key(props)
    // console.log('calculate key', props, newkey)
    if (!props || !newkey) {
      console.error('key is not set correctly', props, newkey, key)
    }
    return newkey
  }
  return key
}

const getCachedKey = (state, props, key) => _calculateKey(key, props)

const getStateAlreadyExist = createCachedSelector(
  getUIState,
  getCachedKey,
  (UIState, key) => UIState.has(key)
)(
  getCachedKey
)

const getInitialState = createCachedSelector(
  getParameterFromProps('initialState'),
  getParameters,
  (initialState, props) => typeof initialState === 'function'
    ? initialState(props)
    : initialState
)(
  getCachedKey
)
// const getJSUIState = (key, initialState, keepImmutable) => createSelector(
export const getJSUIStateByKey = createCachedSelector(
  getUIState,
  getCachedKey,
  getInitialState,
  getParameterFromProps('keepImmutable'),
  (UIState, key, initialState, keepImmutable) => {
    const result = UIState.get(key)
    return result
      ? keepImmutable && result.toObject() || result.toJS()
      : initialState
  }
)(
  getCachedKey
)

const withReduxState = ({
  initialState,
  keepImmutable,
  key = shortid.generate()
}) => WrappedComponent => {
  class StateComponent extends PureComponent {
    componentWillMount () {
      const { stateExist, setInUIState } = this.props
      if (!stateExist) {
        setInUIState({
          key: _calculateKey(key, this.props),
          value: isFunction(initialState) ? initialState(this.props) : initialState
        })
      }
    }
    updateUI = (...args) => {
      const { mergeInUIState, setInUIState } = this.props
      const calculatedKey = _calculateKey(key, this.props)
      if (args.length === 1) { // Deep merge with current state
        const [stateToMerge] = args
        if (!isObject(stateToMerge)) {
          throwNotObjectError(stateToMerge)
        }
        mergeInUIState({
          key: calculatedKey,
          value: stateToMerge
        })
      } else if (args.length > 1) { // Allow to set value at arbitary depth
        const [path, value] = args
        if (!isArray(path) && !isString(path)) {
          throwPathValueError(path, value)
        }
        setInUIState({
          key: calculatedKey,
          path,
          value
        })
      }
    }
    render () {
      const { uiState, stateExist } = this.props
      if (typeof uiState === 'function') {
        console.error(uiState)
      }
      return (
        <WrappedComponent
          {...this.props}
          {...uiState}
          updateUI={this.updateUI}
        />
      )
    }
  }
  StateComponent.propTypes = {
    stateExist: PropTypes.bool,
    setInUIState: PropTypes.func,
    mergeInUIState: PropTypes.func,
    uiState: PropTypes.object
  }

  return connect((state, ownProps) => {
    const stateExist = getStateAlreadyExist(state, ownProps, key)
    const uiState = getJSUIStateByKey(state, { ...ownProps, initialState, keepImmutable }, key)
    return {
      stateExist,
      uiState
    }
  }, {
    mergeInUIState,
    setInUIState
  })(StateComponent)
}

const withComponentState = (initialState, keepImmutable) => withStateHandlers(initialState, {
  updateUI: state => (...args) => {
    if (args.length === 1) { // Deep merge with current state
      const [stateToMerge] = args
      if (!isObject(stateToMerge)) {
        throwNotObjectError(stateToMerge)
      }
      return fromJS(state).mergeDeep(fromJS(stateToMerge)).toJS()
    } else if (args.length > 1) { // Allow to set value at arbitary depth
      const [path, value] = args
      if (!isArray(path) && !isString(path)) {
        throwPathValueError(path, value)
      }

      const newState = fromJS(state).setIn(path.split('.'), value)
      return keepImmutable ? newState.toObject() : newState.toJS()
    }
  }
})

const withState = ({
  initialState,
  key,
  keepImmutable = false,
  redux = false
}) => WrappedComponent => compose(
  redux
    ? withReduxState({ initialState, key, keepImmutable })
    : withComponentState(initialState, keepImmutable)
)(WrappedComponent)

export default withState
