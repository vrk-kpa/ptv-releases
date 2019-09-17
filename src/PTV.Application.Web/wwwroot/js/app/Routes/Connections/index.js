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
import { ConnectionsRoute } from 'Routes/Connections/components'
import { compose } from 'redux'
import { entityTypesEnum } from 'enums'
import { getPresetConnectionsEntityInfo } from './selectors'
import withInit from 'util/redux-form/HOC/withInit'
import { mergeInUIState } from 'reducers/ui'
import {
  setConnectionsMainEntity,
  setConnectionsEntity,
  setConnectionsView,
  setSelectedEntity
} from 'reducers/selections'

const init = ({ location }) => ({ dispatch, getState }) => {
  const mainEntityInfo = getPresetConnectionsEntityInfo(getState(), { location })
  const mainEntity = mainEntityInfo && mainEntityInfo.type
  const connectionsEntity = mainEntity && {
    [entityTypesEnum.CHANNELS]: entityTypesEnum.SERVICES,
    [entityTypesEnum.SERVICES]: entityTypesEnum.CHANNELS
  }[mainEntity] || null
  dispatch(setSelectedEntity({ id: null }))
  dispatch(setConnectionsEntity(connectionsEntity))
  dispatch(setConnectionsMainEntity(mainEntity))
  dispatch(setConnectionsView(false))
  dispatch(mergeInUIState({
    key: 'mainNavigationIndex',
    value: {
      activeIndex: 1
    }
  }))
}

export default compose(
  withInit({
    init
  })
)(ConnectionsRoute)
