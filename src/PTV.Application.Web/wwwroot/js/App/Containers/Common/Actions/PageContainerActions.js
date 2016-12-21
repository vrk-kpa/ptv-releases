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
import { CALL_API, Schemas } from '../../../Middleware/Api';
import * as CommonSelectors from '../Selectors';
import { CommonSchemas } from '../Schemas';
import * as commonActions from '../Actions';

export const SET_STEP_MODE_ACTIVE = 'SET_STEP_MODE_ACTIVE';

export function setStepModeActive(key,index) {
	return () => ({
		type: SET_STEP_MODE_ACTIVE,
		payload: {
			keyToState : key,
			stepIndex : index,
		} 
	});
}

export const SET_STEP_MODE_INACTIVE = 'SET_STEP_MODE_INACTIVE';

export function setStepModeInActive(key) {
	return () => ({
		type: SET_STEP_MODE_INACTIVE,
		payload: {
			keyToState : key
		} 
	});
}

export const RESET_STEP_MODE = 'RESET_STEP_MODE';

export function resetStepMode(key) {
	return () => ({
		type: RESET_STEP_MODE,
		payload: {
			keyToState : key
		} 
	});
}

export function setTranslationMode(keyToState) {
	return ({getState}) => commonActions.setLanguageFrom(keyToState, CommonSelectors.getTranslationLanguageId(getState(), 'sv'))();
}

export const PAGE_ENTITY_STATUS_REQUEST = 'PAGE_ENTITY_STATUS_REQUEST';
export const PAGE_ENTITY_STATUS_SUCCESS = 'PAGE_ENTITY_STATUS_SUCCESS';
export const PAGE_ENTITY_STATUS_FAILURE = 'PAGE_ENTITY_STATUS_FAILURE';

function callGetEntityStatus(resultModel, entityId, endPoint, keyToState) {
	return {
		resultModel,
		keyToState,
		[CALL_API]: {
			types: [PAGE_ENTITY_STATUS_REQUEST,
					PAGE_ENTITY_STATUS_SUCCESS,
					PAGE_ENTITY_STATUS_FAILURE],
			payload: { 
						endpoint: endPoint,						
						data: {
								id: entityId								
							}
					 }
		}
	}
}

export function getEntityStatus(entityId, endPoint, key) {
	return callGetEntityStatus('resultModel', entityId, endPoint, key);
}
