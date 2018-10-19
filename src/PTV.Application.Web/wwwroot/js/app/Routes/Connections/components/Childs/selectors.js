import { List } from 'immutable'
import { createSelector } from 'reselect'
import { getFormValues } from 'redux-form/immutable'

const getConnectionsWorkbenchValues = getFormValues('connectionsWorkbench')
const getConnectionIndexFromProps = (_, { connectionIndex }) => connectionIndex
export const getChilds = createSelector(
  [getConnectionsWorkbenchValues, getConnectionIndexFromProps],
  (formValues, connectionIndex) =>
    formValues.getIn(['connections', connectionIndex, 'childs']) || List()
)

const getShouldShowOnlyAsti = (_, { showAstiOnly }) => showAstiOnly
export const getCount = createSelector(
  [getChilds, getShouldShowOnlyAsti],
  (childs, shouldShowOnlyAsti) => {
    return childs
      .filter(child => child.getIn(['astiDetails', 'isASTIConnection']) === shouldShowOnlyAsti)
      .count()
  }
)
