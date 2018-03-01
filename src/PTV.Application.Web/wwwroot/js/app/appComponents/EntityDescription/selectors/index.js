import { createSelector } from 'reselect'
import { getKey, formAllTypes } from 'enums'
import { getParameterFromProps } from 'selectors/base'

export const getPreviewEntityFormName = createSelector(
  getParameterFromProps('entityConcreteType'),
  entityConcreteType => entityConcreteType && getKey(formAllTypes, entityConcreteType.toLowerCase()) || ''
)
