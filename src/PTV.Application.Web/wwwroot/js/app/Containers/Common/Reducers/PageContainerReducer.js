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
import {combineReducers} from 'redux';
import * as ActionTypes from '../Actions/PageContainerActions';
import * as ActionTypesService from '../../Services/Service/Actions';
import * as ActionTypesOrganization from '../../Manage/Organizations/Organization/Actions';
import * as ActionTypesElectronic from '../../Channels/Electronic/Actions';
import * as ActionTypesPhone from '../../Channels/Phone/Actions';
import * as ActionTypesPrintableForm from '../../Channels/PrintableForm/Actions';
import * as ActionTypesServiceLocation from '../../Channels/ServiceLocation/Actions';
import * as ActionTypesWebPage from '../../Channels/WebPage/Actions';
import * as CommonActionsTypes from '../Actions';

import { Map } from 'immutable'
import merge from 'lodash/merge'
import union from 'lodash/union'
import getResults, { merger, mergerWithReplace }  from '../../../Middleware';

function updateState(state = Map(), index, key){
    return state.set(key, index)
}
function deleteState(state = Map(), key){
    return state.delete(key)
}
const initialState = Map({
  searchDomain: 'services'
})
 function pageModeState(state = initialState, action) {
    if (action.keyToState && action.response && action.response.model){
         state = state.mergeDeep({[action.keyToState]: { id: action.response.model.result}});
     }

     if (action.payload && action.payload.pageModeState){
         state = state.mergeDeep(action.payload.pageModeState);
     }

     if (action.request && action.request.pageModeState){
         state = state.mergeDeep(action.request.pageModeState);
     }

     if (action.pageSetup){
         if(action.pageSetup.id != state.getIn([action.pageSetup.keyToState,'id'])){
             state = state.set(action.pageSetup.keyToState,deleteState(state.get(action.pageSetup.keyToState),'editedStep'));
             state = state.set(action.pageSetup.keyToState,deleteState(state.get(action.pageSetup.keyToState),'languageFrom'));
             //state = state.set(action.pageSetup.keyToState,deleteState(state.get(action.pageSetup.keyToState),'languageTo'));
         }
         state = state.mergeDeep({[action.pageSetup.keyToState]: { id: action.pageSetup.id}});

     }
     if (action.relations){
         if(typeof action.relations.readOnly === 'boolean'){
            state = state.mergeDeep({[action.relations.keyToState]: { readOnly: action.relations.readOnly}});
         }
         if(typeof action.relations.isExpanded === 'boolean'){
            state = state.set('serviceAndChannelServiceSearch',deleteState(state.get('serviceAndChannelServiceSearch'),'isExpanded'));
            state = state.set('serviceAndChannelChannelSearch',deleteState(state.get('serviceAndChannelChannelSearch'),'isExpanded'));
            state = state.mergeDeep({[action.relations.keyToState]: { isExpanded: action.relations.isExpanded}});
         }
         if(typeof action.relations.detailId !== 'undefined' || typeof action.relations.detailId === 'string' && action.relations.detailId.length > 0){
            state = state.mergeDeep({[action.relations.keyToState]: { detailId: action.relations.detailId}});
            state = state.mergeDeep({[action.relations.entityKeyToState]: { id: action.relations.detailId}});
         }
         if(typeof action.relations.relationId !== 'undefined' || typeof action.relations.relationId === 'string' && action.relations.relationId.length > 0){
            state = state.mergeDeep({[action.relations.keyToState]: { relationId: action.relations.relationId}});
         }
         if((typeof action.relations.confirmationType !== 'undefined' || typeof action.relations.confirmationType === 'string' && action.relations.confirmationType.length > 0)
          && (typeof action.relations.confirmationValue !== 'undefined')){
            state = state.mergeDeep({[action.relations.keyToState]: { confirmationType: action.relations.confirmationType, confirmationValue: action.relations.confirmationValue}});
         }
         if(typeof action.relations.tabId !== 'undefined' || typeof action.relations.tabId === 'string' && action.relations.tabId.length > 0){
            state = state.mergeDeep({[action.relations.keyToState]: { tabId: action.relations.tabId}});
         }
     }

    if (action.payload && action.payload.pageModeState) {
      state = state.mergeWith(merger, action.payload.pageModeState);
    }

  switch (action.type) {
    case ActionTypes.SET_STEP_MODE_ACTIVE:
      return state.set(
        action.payload.keyToState,
        updateState(state.get(action.payload.keyToState), action.payload.stepIndex, 'editedStep')
      )
    case ActionTypes.SET_STEP_MODE_INACTIVE:
      return state.set(
        action.payload.keyToState,
        deleteState(state.get(action.payload.keyToState), 'editedStep')
      )
    case ActionTypes.RESET_STEP_MODE:
      return state.set(action.payload.keyToState, new Map())
    case CommonActionsTypes.SET_LANGUAGE_FROM:
      return state.set(
        action.payload.keyToState,
        updateState(state.get(action.payload.keyToState), action.payload.languageId, 'languageFrom')
      )
    case CommonActionsTypes.FORCE_RELOAD:
      return state.set(
        action.payload.keyToState,
        updateState(state.get(action.payload.keyToState), action.payload.forceReload, 'forceReload')
      )
    case CommonActionsTypes.SET_LANGUAGE_TO:
      return state.set(
        action.payload.keyToState,
        updateState(state.get(action.payload.keyToState), action.payload.languageId, 'languageTo').set('languageCode', action.payload.languageCode || '')
      )
    default: return state
  }
}

export default pageModeState;
