/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
import { EntitySelectors } from 'selectors'
import { getObjectArray } from 'selectors/base'
import { createSelector } from 'reselect'
import { getFormValues } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { Map } from 'immutable'

const getOrganizationTypesTT1ObjectArray = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  organizationTypes => {
    const tt1 = organizationTypes.find(x => x.get('code').toUpperCase() === 'TT1')
    return getObjectArray(organizationTypes.filter(type => type === tt1)
      .concat(organizationTypes.filter(type => tt1.get('children').includes(type.get('id')))))
  }
)
const getOrganizationTypesTT2ObjectArray = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  organizationTypes => {
    const tt2 = organizationTypes.find(x => x.get('code').toUpperCase() === 'TT2')
    return getObjectArray(organizationTypes.filter(type => type === tt2)
      .concat(organizationTypes.filter(type => tt2.get('children').includes(type.get('id')))))
  }
)

export const getOrganizationTypesObjectArray = createSelector(
  [getOrganizationTypesTT1ObjectArray, getOrganizationTypesTT2ObjectArray],
  (tt1, tt2) => tt1.concat(tt2)
)

export const getIsSoteOrganizationType = createSelector(
  EntitySelectors.organizationTypes.getEntities,
  getFormValues(formTypesEnum.ORGANIZATIONFORM),
  (orgTypes, formValues) => {
    const orgTypeId = formValues && formValues.get('organizationType') || ''
    const orgType = orgTypes.get(orgTypeId) || Map()
    const orgTypeCode = orgType.get('code')
    return orgTypeCode === 'SotePublic' || orgTypeCode === 'SotePrivate'
  }
)
