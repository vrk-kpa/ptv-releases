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
import { CALL_API, Schemas } from '../../../../Middleware/Api'
import { onEntityInputChange, onEntityListChange, onEntityObjectChange, onEntityAdd, fakeApiCall } from '../../../Common/Actions'
import { ServiceSchemas } from '../../Service/Schemas'

export const SERVICE_SET_SERVICE_ID = 'SERVICE_SET_SERVICE_ID'
export function setServiceId (serviceId) {
  return () => ({
    type: SERVICE_SET_SERVICE_ID,
    pageSetup:{
      id: serviceId,
      keyToState: 'service'
    }
  })
}

export function onServiceEntityAdd (property, entity, id, replace = false, schema = ServiceSchemas.SERVICE) {
  return () => onEntityAdd({ id: id, [property]: entity }, schema, replace)
}

export function onServiceEntityReplace (property, entity, id, schema = ServiceSchemas.SERVICE) {
  return () => fakeApiCall({ id: id, [property]: entity }, schema)
}
