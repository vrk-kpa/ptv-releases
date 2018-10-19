import { getFormValues, initialize } from 'redux-form/immutable'

export const makeCurrentFormStateInitial = () => ({ dispatch, getState }) => {
  const state = getState()
  const getCurrentFormValues = getFormValues('connectionsWorkbench')
  const formValues = getCurrentFormValues(state)
  dispatch(initialize('connectionsWorkbench', formValues))
}
