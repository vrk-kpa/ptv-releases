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
import { CALL_API } from '../../../Middleware/Api';
import * as CommonActions from './';


export const SEARCH_IN_TREE_REQUEST = 'SEARCH_IN_TREE_REQUEST';
export const SEARCH_IN_TREE_SUCCESS = 'SEARCH_IN_TREE_SUCCESS';
export const SEARCH_IN_TREE_FAILURE = 'SEARCH_IN_TREE_FAILURE';

export function searchInTree(searchSchema, treeType, value, id) {
		return (props) => {
			const payload = CommonActions.onEntityObjectChange('searches', treeType, { id, 'name': value}).payload;
			return CommonActions.apiCall([treeType, 'search'], {...payload, ...{ endpoint: 'service/GetFilteredTree', data: { id, searchValue: value, treeType: treeType }  }} ,
			[],
			searchSchema)(props);
		};
}

export const clearSearch = (keyToState) => {
	const payload = CommonActions.onEntityObjectChange('searches', keyToState, { id: null, name: null }).payload;
			
	let result = CommonActions.clearApiCall([keyToState, 'search'], { model: { id: null } });
	result['payload'] = {...payload, ...result.payload};
	return result;
}

export function loadNodeChildren(treeNodeSchema, treeType, node) {
		return (props) => {
			return CommonActions.apiCall(['nodes', node.get('id')], { endpoint: 'service/GetFintoTree', data: {treeItem: node, treeType: treeType }} ,
			[],
			treeNodeSchema)(props);
		};
}

export const UPDATE_NODE = 'UPDATE_NODE'

export function updateNode(id, property, value){	
	return () => ({
		type: UPDATE_NODE,
		payload: {
			id,
			property,
			value
		} 
	});
}