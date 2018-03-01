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
import {serviceSearchForm, serviceSearchFormInputs} from '../Reducers/Reducer';
import chai from 'chai';
import chaiImmutable from 'chai-immutable';
import { CALL_API } from '../../../Middleware/Api';

const testGuid = 'aaaa-bbbb';
chai.use(chaiImmutable);

describe('Service reducers', () => {
//   it('select service class', () => {
//   	const stateBefore = new Models.ServiceSearchForm;
//   	const stateAfter = new Models.ServiceSearchForm({serviceClassId: testGuid});
//   	chai.expect(serviceSearchForm(stateBefore, actions.selectServiceClass(testGuid)())).equal(stateAfter);
//   })
//   it('select organization', () => {
//   	const stateBefore = new Models.ServiceSearchForm;
//   	const stateAfter = new Models.ServiceSearchForm({organizationId: testGuid});
//   	chai.expect(serviceSearchForm(stateBefore, actions.selectOrganization(testGuid)())).equal(stateAfter);
//   })
//   it('change Name', () => {
//   	const stateBefore = new Models.ServiceSearchForm;
//   	const stateAfter = new Models.ServiceSearchForm({serviceName: testGuid});
//   	chai.expect(serviceSearchForm(stateBefore, actions.onNameChange(testGuid)())).equal(stateAfter);
//   })
//   it('change ontology word', () => {
//   	const stateBefore = new Models.ServiceSearchForm;
//   	const stateAfter = new Models.ServiceSearchForm({ontologyWord: testGuid});
//   	chai.expect(serviceSearchForm(stateBefore, actions.onOntologyWordChange(testGuid)())).equal(stateAfter);
//   })
});
