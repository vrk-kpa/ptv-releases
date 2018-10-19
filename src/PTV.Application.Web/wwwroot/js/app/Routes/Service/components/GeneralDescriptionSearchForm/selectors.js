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
import { createSelector } from 'reselect'
import { Map } from 'immutable'
import { getParameterFromProps, getFormValue } from 'selectors/base'
import { EntitySelectors } from 'selectors'
import { getGDPublishedLanguages } from 'Routes/Service/components/ServiceComponents/selectors'

const getSelectedGD = createSelector(
  [
    EntitySelectors.generalDescriptions.getEntities,
    getParameterFromProps('generalDescriptionId')
  ],
  (generalDescriptions, id) => generalDescriptions.get(id) || Map()
)

const getSelectedGDName = createSelector(
  getSelectedGD,
  gd => gd.get('name') || Map()
)

export const getNewServiceName = createSelector(
  [
    getGDPublishedLanguages,
    getFormValue('languagesAvailabilities'),
    getFormValue('name'),
    getSelectedGDName
  ],
  (publishedGDLanguages, languagesAvailabilities, serviceName, selectedGDName) => {
    let newServiceName = Map()
    languagesAvailabilities.forEach((v, k) => {
      newServiceName = publishedGDLanguages.contains(v.get('code'))
        ? newServiceName.set(v.get('code'), selectedGDName.get(v.get('code')))
        : newServiceName.set(v.get('code'), serviceName.get(v.get('code')))
    })
    return newServiceName
  }
)
