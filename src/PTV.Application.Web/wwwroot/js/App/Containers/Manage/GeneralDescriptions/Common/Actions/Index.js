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

import { CALL_API, Schemas } from '../../../../../Middleware/Api';
import { onEntityInputChange, onEntityListChange, onEntityObjectChange, onEntityAdd, fakeApiCall } from '../../../../Common/Actions';
//import * as CommonSelectors from '../Selectors';
import { GeneralDescriptionSchemas } from '../../GeneralDescriptions/Schemas';
import * as CommonActions from '../../../../../Containers/Common/Actions';

export function onGeneralDescriptionEntityAdd(property, entity, id, replace= false, schema= GeneralDescriptionSchemas.GENERAL_DESCRIPTION) {
	return () => onEntityAdd({id: id, [property]: entity}, schema, replace);
}

export function onGeneralDescriptionEntityReplace(property, entity, id, schema= GeneralDescriptionSchemas.GENERAL_DESCRIPTION) {
	return () => fakeApiCall({id: id, [property]: entity}, schema);
}

export function onGeneralDescriptionInputChange(input, id, value, isSet, entity='generalDescription') {
	return () => onEntityInputChange(entity, id, input, value, isSet)
}

export function onGeneralDescriptionObjectChange(id, object, isSet, entity='generalDescription') {
	return () => onEntityObjectChange([entity], id, object, isSet)
}

export function onGeneralDescriptionListChange(input, id, value, isAdd, entity='generalDescription') {
	return () => onEntityListChange(entity, id, input, value, isAdd)
}

export const generalDescriptionCall = (endpoint, data) => {
	return CommonActions.apiCall(['generalDescription', 'all'],
		 { endpoint, data },
		 [],
		 GeneralDescriptionSchemas.GENERAL_DESCRIPTION, 'generalDescription');
}