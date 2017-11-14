import React from 'react'
import PropTypes from 'prop-types'
import withState from 'util/withState'
import { compose } from 'redux'

const withShowErrorOnDirty = WrappedComponent => {
  const InnerComponent = ({
    wasFilled,
    updateUI,
    ...rest
  }) => {
    const showError = ({ error, touched, submitFailed }) =>
      (wasFilled && touched || submitFailed) && error
    const onChange = () => !wasFilled && updateUI('wasFilled', true)
    return (
      <WrappedComponent
        showError={showError}
        onChange={onChange}
        {...rest}
      />
    )
  }
  InnerComponent.propTypes = {
    ui: PropTypes.object.isRequired,
    updateUI: PropTypes.func.isRequired
  }
  return compose(
    withState({
      initialState: {
        wasFilled: false
      }
    })
  )(InnerComponent)
}

export default withShowErrorOnDirty
