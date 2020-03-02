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
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import { getParameterFromProps } from 'selectors/base'
import { getFormValues } from 'redux-form/immutable'
import { getSelectedEntityType, getEntity } from 'selectors/entities/entities'
import { entityTypesEnum } from 'enums'

export const getReduxFormKey = createSelector(
  getParameterFromProps('isAsti'),
  getSelectedEntityType,
  (isAsti, selectedEntityType) => !selectedEntityType
    ? 'connections'
    : !isAsti
      ? 'selectedConnections'
      : selectedEntityType === entityTypesEnum.SERVICES
        ? 'connectionsFlat'
        : 'connectionsByOrganizations'
)

export const getFieldName = createSelector(
  getParameterFromProps('childType'),
  getParameterFromProps('parentIndex'),
  getParameterFromProps('childIndex'),
  getParameterFromProps('groupIndex'),
  getParameterFromProps('reduxFormKey'),
  getSelectedEntityType,
  (childType, parentIndex, childIndex, groupIndex, reduxFormKey, selectedEntityType) => !selectedEntityType
    ? `${reduxFormKey}[${parentIndex}].${childType}[${childIndex}]`
    : Number.isInteger(groupIndex)
      ? `${reduxFormKey}[${groupIndex}][${childIndex}]`
      : `${reduxFormKey}[${childIndex}]`
)

const getValues = createSelector(
  [state => state, getParameterFromProps('formName')],
  (state, formName) => getFormValues(formName)(state) || Map()
)

export const getConnectedEntityFieldValue = createSelector(
  getParameterFromProps('childType'),
  getParameterFromProps('parentIndex'),
  getParameterFromProps('childIndex'),
  getParameterFromProps('groupIndex'),
  getParameterFromProps('reduxFormKey'),
  getValues,
  getSelectedEntityType,
  (childType, parentIndex, childIndex, groupIndex, reduxFormKey, values, selectedEntityType) => !selectedEntityType
    ? values.getIn([reduxFormKey, parentIndex, childType, childIndex])
    : Number.isInteger(groupIndex)
      ? values.getIn([reduxFormKey, groupIndex, childIndex])
      : values.getIn([reduxFormKey, childIndex])
)

export const getMainEntityFieldValue = createSelector(
  getSelectedEntityType,
  getParameterFromProps('reduxFormKey'),
  getParameterFromProps('parentIndex'),
  getValues,
  getEntity,
  (selectedEntityType, reduxFormKey, parentIndex, values, entity) => !selectedEntityType
    ? values.getIn([reduxFormKey, parentIndex, 'mainEntity'])
    : entity || Map()
)
