import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { withStateHandlers } from 'recompose'
import { compose } from 'redux'
import { set, merge, isObject, isArray, isString, isFunction } from 'lodash'
import { setInUIState, mergeInUIState } from 'reducers/ui'
import { connect } from 'react-redux'
import { createSelector } from 'reselect'
import shortid from 'shortid'
import { fromJS, Map } from 'immutable'

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
const getStateAlreadyExist = key => createSelector(
  getUIState,
  UIState => UIState.has(key)
)
const getJSUIStateByKey = (key, initialState, keepImmutable) => createSelector(
  getUIState,
  UIState => {
    const result = UIState.get(key)
    return result
      ? keepImmutable && result.toObject() || result.toJS()
      : initialState
  }
)

const withReduxState = ({
  initialState,
  keepImmutable,
  key = shortid.generate()
}) => WrappedComponent => {
  const _calculateKey = (key, props) => isFunction(key) ? key(props) : key
  class InnerComponent extends PureComponent {
    componentWillMount () {
      const { stateExist, setInUIState } = this.props
      if (!stateExist) {
        setInUIState({
          key: _calculateKey(key, this.props),
          value: initialState
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
      const { uiState } = this.props
      return (
        <WrappedComponent
          {...this.props}
          {...uiState}
          updateUI={this.updateUI}
        />
      )
    }
  }
  InnerComponent.propTypes = {
    stateExist: PropTypes.bool,
    setInUIState: PropTypes.func,
    mergeInUIState: PropTypes.func,
    uiState: PropTypes.object
  }
  return connect((state, ownProps) => {
    const calculatedKey = _calculateKey(key, ownProps)
    return {
      stateExist: getStateAlreadyExist(calculatedKey)(state),
      uiState: getJSUIStateByKey(calculatedKey, initialState, keepImmutable)(state)
    }
  }, {
    mergeInUIState,
    setInUIState
  })(InnerComponent)
}

const withComponentState = initialState => withStateHandlers(initialState, {
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
      set(state, path, value)
      return {
        ...state
      }
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
    : withComponentState(initialState)
)(WrappedComponent)

export default withState
