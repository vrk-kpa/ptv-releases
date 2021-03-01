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
import { change } from 'redux-form/immutable'
import { mergeInUIState } from 'reducers/ui'
import { OrderedSet } from 'immutable'
import {
  formTypesEnum
} from 'enums'
import { apiCall3, deleteApiCall } from 'actions'
import { EntitySchemas } from 'schemas'
import { organizationDialogName } from 'Routes/Search/components/SearchFilters/dialogs'

export const closeOrganizationDialog = () => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: organizationDialogName,
    value: {
      isOpen: false
    }
  }))
}

export const clearSelectedOrganization = () => ({ dispatch }) => {
  dispatch(change(formTypesEnum.ADMINMAPPINGFORM, 'organizationIds', OrderedSet()))
}

export const clearSearchedOrganization = () => ({ dispatch }) => {
  dispatch(deleteApiCall(['adminMappingForm', 'search']))
}

export const searchMapping = (selectedOrganizations) => ({ dispatch }) => {
  dispatch(
    apiCall3({
      keys: [formTypesEnum.ADMINMAPPINGFORM, 'search'],
      payload: {
        endpoint: 'admin/GetMappings',
        data: { data: selectedOrganizations && selectedOrganizations.map(x => x.value) }
      },
      schemas: EntitySchemas.GET_SEARCH(EntitySchemas.SAHA_MAPPING)
    })
  )
  dispatch(mergeInUIState({
    key: 'removeMappingDialogParams',
    value: {
      ptvId:null,
      sahaId:null,
      selectedOrganizations:null
    }
  }))
  dispatch(mergeInUIState({
    key: 'updateMappingDialogParams',
    value: {
      ptvId:null,
      sahaId:null,
      selectedOrganizations:null
    }
  }))
}

export const openRemoveOrganizationDialog = (ptvId, sahaId, selectedOrganizations) => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: 'removeMappingDialogParams',
    value: {
      ptvId,
      sahaId,
      selectedOrganizations
    }
  }))
  dispatch(mergeInUIState({
    key: 'removeMappingDialog',
    value: {
      isOpen: true
    }
  }))
}

export const removeMapping = (ptvId, sahaId, selectedOrganizations) => ({ dispatch }) => {
  dispatch(
    apiCall3({
      keys: [formTypesEnum.ADMINMAPPINGFORM, 'remove'],
      payload: {
        endpoint: 'admin/RemoveMapping',
        data: { ptvOrganizationId: ptvId, sahaId }
      },
      successNextAction: (dispatch) => searchMapping(selectedOrganizations)
    })
  )
}

export const openUpdateOrganizationDialog = (ptvId, sahaId, selectedOrganizations) => ({ dispatch }) => {
  dispatch(mergeInUIState({
    key: 'updateMappingDialogParams',
    value: {
      ptvId,
      sahaId,
      selectedOrganizations
    }
  }))
  dispatch(mergeInUIState({
    key: 'updateMappingDialog',
    value: {
      isOpen: true
    }
  }))
}

export const updateMapping = (ptvId, sahaId, selectedOrganizations, newOrganizationId) => ({ dispatch }) => {
  let newSearchIds = selectedOrganizations
  if (!selectedOrganizations.some(x => x.value === newOrganizationId)) {
    newSearchIds = newSearchIds.concat({ value:newOrganizationId })
    dispatch(change(formTypesEnum.ADMINMAPPINGFORM, 'organizationIds', OrderedSet(newSearchIds.map(x => x.value))))
  }
  dispatch(
    apiCall3({
      keys: [formTypesEnum.ADMINMAPPINGFORM, 'update'],
      payload: {
        endpoint: 'admin/UpdateMapping',
        data: { ptvOrganizationId: newOrganizationId, sahaId }
      },
      successNextAction: (dispatch) => searchMapping(newSearchIds)
    })
  )
}
