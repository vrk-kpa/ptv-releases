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
import * as actions from '../Actions/Actions';
import { expect } from 'chai';
import { CALL_API } from '../../../Middleware/Api';

const testGuid = 'aaaa-bbbb';

describe('Service actions', () => {
  it('select organization', () => {
  	const action = actions.selectOrganization(testGuid)();
  	const { type, payload } = action;
  	expect(type).equal('SERVICE_SELECT_ORGANIZATION');
  	expect(payload.organizationId).equal(testGuid);
  }),
  it('select service class', () => {
  	const action = actions.selectServiceClass(testGuid)();
  	const { type, payload } = action;
  	expect(type).equal('SERVICE_SELECT_SERVICE_CLASS');
  	expect(payload.serviceClassId).equal(testGuid);
  })
  it('name changed', () => {
  	const action = actions.onNameChange(testGuid)();
  	const { type, payload } = action;
  	expect(type).equal('SERVICE_NAME_CHANGED');
  	expect(payload.serviceName).equal(testGuid);
  })
  it('ontologyWord changed', () => {
  	const action = actions.onOntologyWordChange(testGuid)();
  	const { type, payload } = action;
  	expect(type).equal('SERVICE_ONTOLOGY_WORD_CHANGED');
  	expect(payload.ontologyWord).equal(testGuid);
  })
  it('load search form', () => {
  	const action = actions.loadServiceSearch();
  	const { [CALL_API]: {types}, [CALL_API]: {payload}, serviceSearchForm } = action;
  	expect(types).to.contain('SERVICE_GET_SERVICE_SEARCH_REQUEST');
  	expect(types).to.contain('SERVICE_GET_SERVICE_SEARCH_SUCCESS');
  	expect(types).to.contain('SERVICE_GET_SERVICE_SEARCH_FAILURE');
  	expect(serviceSearchForm).equal('serviceSearchForm');
  	expect(payload.endpoint).equal('service/GetServiceSearch');
  })
  it('load services', () => {
  	const data = {
  		id: '1',
  		name: 'Joe'
  	}
  	const action = actions.loadServices(data);
  	const { [CALL_API]: {types}, [CALL_API]: {payload}, serviceSearchResults } = action;
  	expect(types).to.contain('SERVICE_SEARCH_REQUEST');
  	expect(types).to.contain('SERVICE_SEARCH_SUCCESS');
  	expect(types).to.contain('SERVICE_SEARCH_FAILURE');
  	expect(serviceSearchResults).equal('serviceSearchResults');
  	expect(payload.endpoint).equal('service/SearchServices');
  	expect(payload.data).equal(data);
  })
});
