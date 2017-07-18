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
import { Schema, arrayOf, valuesOf } from 'normalizr';
import { CommonSchemas, defineChildrenSchema } from '../../../Common/Schemas';

const keyWordsSchema = new Schema('keyWords', { idAttribute: kewyWord => kewyWord.id, meta: { localizable: true }});
const serviceTypeSchema = new Schema('serviceTypes', { idAttribute: serviceType => serviceType.id });

defineChildrenSchema(keyWordsSchema);

const provisionTypeSchema = new Schema('provisionTypes', {
    idAttribute: provisionType => provisionType.id
});

const coverageTypeSchema = new Schema('coverageTypes', {
    idAttribute: coverageType => coverageType.id
});
export const CommonServiceSchemas = {
  KEY_WORD: keyWordsSchema,
  KEY_WORD_ARRAY: arrayOf(keyWordsSchema),
  SERVICE_TYPE:serviceTypeSchema,
  SERVICE_TYPE_ARRAY: arrayOf(serviceTypeSchema),
  PROVISION_TYPE:provisionTypeSchema,
  PROVISION_TYPE_ARRAY: arrayOf(provisionTypeSchema),
  COVERAGE_TYPE:coverageTypeSchema,
  COVERAGE_TYPE_ARRAY: arrayOf(coverageTypeSchema)
}