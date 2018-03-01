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
import { combineReducers } from 'redux-immutable'

import { reducer as ui } from 'util/redux-ui'

import intl from '../Intl/Reducer'
import maps from 'reducers/maps'
import currentIssues from 'reducers/currentIssues'
import frontPageRows from 'reducers/frontPage'
import selections from 'reducers/selections'
import formStates from 'reducers/formStates'
import automaticSave from 'reducers/automaticSave'
import notifications from 'reducers/notifications'
import buttonActivity from 'Components/PTVButton/actions'
import common from '../Containers/Common/Reducers'
import pageModeState from '../Containers/Common/Reducers/PageContainerReducer'
import { reducer as form } from 'redux-form/immutable'
import { UNTOUCH_ALL_FRONT_PAG_SEARCH } from 'Routes/FrontPage/routes/Search/actions'
import routing from 'reducers/routing/routing'
import ui2 from 'reducers/ui'

const appReducer = combineReducers({
  automaticSave,
  intl,
  common,
  routing,
  pageModeState,
  selections,
  formStates,
  ui,
  ui2,
  notifications,
  form: form.plugin({
    frontPageSearch:(state, action) => {
      if (action && action.type && action.type === UNTOUCH_ALL_FRONT_PAG_SEARCH){
        let newState = state
        return newState.set('initial', newState.get('values'))
      }
      return state
    }
  }),
  maps,
  currentIssues,
  frontPageRows,
  // will be removed after redesign
  buttonActivity
})

export default appReducer
