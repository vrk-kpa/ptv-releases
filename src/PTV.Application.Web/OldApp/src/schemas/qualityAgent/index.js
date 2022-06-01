/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { schema } from 'normalizr'
import { merge } from 'lodash'
import { stringify } from 'querystring'

const qualityAgentError = new schema.Entity('qualityErrors', {}, {
  idAttribute: i => i.fieldId,
  mergeStrategy: (a, b) => merge(a, b),
  processStrategy: (value, parent, key) => {
    const { fieldId, result, ...rest } = value
    return {
      fieldId,
      rules: {
        [rest.ruleId]: {
          ...rest,
          ...result
        }
      }
    }
  }
})

const qualityAgent = qaSchema => new schema.Object({
  result: new schema.Array(qaSchema)
})

const createErrorForField = (fieldId, ruleError) => ({
  fieldId,
  rules: {
    [ruleError.ruleId]: ruleError
  }
})

const filteredPaths = ['additionalInformation', 'description', 'chargeDescription', 'deliveryAddressInText']
const connectionField = 'connectionDescriptions'

const getFieldId = (path) => {
  const paths = path.split(';')
  const fieldId = paths[0]
  if (paths.length > 1 && !fieldId.includes(connectionField)) {
    return [fieldId, ...paths[1].split('.').filter(x => filteredPaths.includes(x))].join('.')
  }
  return fieldId
}

const getQualityAgentSchema = id =>
  qualityAgent(new schema.Entity('qualityErrors', {}, {
    idAttribute: i => id,
    mergeStrategy: (a, b) => merge(a, b),
    processStrategy: (value, parent, key) => {
      const { fieldId, result, processed, ...rest } = value
      const fields = processed && Array.isArray(processed.path) && processed.path || null
      if (fields && fields.length) {
        return fields.reduce((errors, pathId) => {
          const mappedFieldId = getFieldId(pathId)
          errors[mappedFieldId] = createErrorForField(mappedFieldId, { ...rest, ...result, processed })
          return errors
        }, {})
      }
      return {
        [fieldId]: createErrorForField(fieldId, { ...rest, ...result, processed })
      }
    }
  }))

export const QualityAgentSchemas = {
  QUALITY_AGENT: qualityAgent(qualityAgentError),
  getQualityAgentSchema
}
