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
import { List } from 'immutable'
import { createSelector } from 'reselect'
import { getFormValues } from 'redux-form/immutable'
import { getConnectionsUISortingByIndex } from 'Routes/Connections/selectors'
import { getSelectedLanguage } from 'Intl/Selectors'
import { getSortedListIndices } from 'util/helpers'
import { createTranslatedListSelector } from 'appComponents/Localize/selectors'
import { languageTranslationTypes } from 'appComponents/Localize'
import { getConnectionsMainEntity } from 'selectors/selections'
import { getParameterFromProps, getUiSortingData } from 'selectors/base'
import { entityTypesEnum } from 'enums'

const getConnectionsWorkbenchValues = getFormValues('connectionsWorkbench')
const getConnectionIndexFromProps = (_, { connectionIndex }) => connectionIndex
export const getChilds = createSelector(
  [getConnectionsWorkbenchValues, getConnectionIndexFromProps],
  (formValues, connectionIndex) =>
    formValues.getIn(['connections', connectionIndex, 'childs']) || List()
)

export const getASTIChildren = createSelector(
  getConnectionsWorkbenchValues,
  getConnectionIndexFromProps,
  (formValues, connectionIndex) =>
    formValues.getIn(['connections', connectionIndex, 'astiChilds']) || List()
)

const getSubEntityTypeIds = createSelector(
  getConnectionsMainEntity,
  getParameterFromProps('data'),
  (mainEntity, connections) => mainEntity === entityTypesEnum.SERVICES
    ? connections.map(connection => ({ id: connection.get('channelTypeId') }))
    : connections.map(connection => ({ id: connection.get('serviceType') }))
)

const getTranslatedEntityTypesForSubEntities = createTranslatedListSelector(
  getSubEntityTypeIds, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

const getMainEntityTypeIds = createSelector(
  getConnectionsMainEntity,
  getParameterFromProps('data'),
  (mainEntity, connections) => mainEntity === entityTypesEnum.SERVICES
    ? connections.map(connection => ({ id: connection.get('serviceType') }))
    : connections.map(connection => ({ id: connection.get('channelTypeId') }))
)

const getTranslatedEntityTypesForMainEntities = createTranslatedListSelector(
  getMainEntityTypeIds, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

const getOrganizationsIds = createSelector(
  getParameterFromProps('data'),
  organizations => organizations.map(organization => ({ id: organization.get('organizationId') }))
)

const getTranslatedOrganizations = createTranslatedListSelector(
  getOrganizationsIds, {
    languageTranslationType: languageTranslationTypes.locale
  }
)

export const getSortingOrderForMainEntities = createSelector(
  getParameterFromProps('data'),
  getTranslatedOrganizations,
  getTranslatedEntityTypesForMainEntities,
  getUiSortingData,
  getSelectedLanguage,
  (children, organizations, entityTypes, sortMap, uiLanguage) => {
    const column = sortMap.get('column')
    if (!column) return List()
    const sortDirection = sortMap.get('sortDirection')
    const isLocalized = sortMap.get('isLocalized')
    const items = (column === 'channelTypeId' || column === 'serviceType')
      ? entityTypes.map(type => type.name)
      : column === 'organization'
        ? organizations.map(organization => organization.name)
        : children.map(item => isLocalized
          ? item.getIn([column, uiLanguage]) || item.get(column) && item.get(column).first()
          : item.get(column) && item.get(column)
      )
    return getSortedListIndices(items, sortDirection, uiLanguage)
  }
)

export const getSortingOrder = createSelector(
  getParameterFromProps('data'),
  getTranslatedEntityTypesForSubEntities,
  getConnectionsUISortingByIndex,
  getSelectedLanguage,
  (children, entityTypes, sortMap, uiLanguage) => {
    const column = sortMap.get('column')
    if (!column) return List()
    const sortDirection = sortMap.get('sortDirection')
    const isLocalized = sortMap.get('isLocalized')
    const items = (column === 'channelTypeId' || column === 'serviceTypeId')
      ? entityTypes.map(type => type.name)
      : children.map(item => isLocalized
        ? item.getIn([column, uiLanguage]) || item.get(column) && item.get(column).first()
        : item.get(column) && item.get(column)
      )
    return getSortedListIndices(items, sortDirection, uiLanguage)
  }
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
