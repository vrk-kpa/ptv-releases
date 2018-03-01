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
import { ServiceForm } from 'Routes/Service/components'
import { entityConcreteTypesEnum } from 'enums'
import EntitySelectors from 'selectors/entities/entities'
import { routeInit } from 'util/helpers/baseRouteInit'
import { mergeInUIState } from 'reducers/ui'
import { deleteApiCall } from 'Containers/Common/Actions'

const init = (store, nextState) => {
  const { getState, dispatch } = store
  const { params: { id }, location: { hash } } = nextState
  const isLoaded = EntitySelectors.services.getIsEntityLoaded(getState(), { id })
  if (isLoaded && hash) {
    return
  }
  dispatch(deleteApiCall(['service', entityConcreteTypesEnum.GENERALDESCRIPTION, 'search']))
  dispatch(mergeInUIState({
    key: 'generalDescriptionSelectContainer',
    value: {
      isCollapsed: true
    }
  }))
  routeInit(store, nextState, entityConcreteTypesEnum.SERVICE)
}

export default store => ({
  path: 'service(/:id)',
  getComponent (nextState, cb) {
    init(store, nextState)
    cb(null, ServiceForm)
  }
})
