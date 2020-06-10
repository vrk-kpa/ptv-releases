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
import { IntlSchemas } from 'Intl/Schemas'

const serviceClass = new schema.Entity('serviceClasses')
const industrialClass = new schema.Entity('industrialClasses', {}, {
  processStrategy: (value, parent, key) => {
    return {
      ...value,
      parentId: key === 'children' ? parent.id : null
    }
  }
})
const lifeEvent = new schema.Entity('lifeEvents')
const targetGroup = new schema.Entity('targetGroups')
const ontologyTerm = new schema.Entity('ontologyTerms')
const annotationOntologyTerms = new schema.Object({
  annotationOntologyTerms: new schema.Array(ontologyTerm)
})
const digitalAuthorization = new schema.Entity('digitalAuthorizations')

export const defineChildrenSchema = (fintoschema) => {
  fintoschema.define({
    children: new schema.Array(fintoschema),
    filteredChildren: new schema.Array(fintoschema),
    translation: IntlSchemas.TRANSLATED_ITEM
  })
}

defineChildrenSchema(industrialClass)
defineChildrenSchema(serviceClass)
defineChildrenSchema(lifeEvent)
defineChildrenSchema(targetGroup)
defineChildrenSchema(ontologyTerm)
defineChildrenSchema(digitalAuthorization)

export const FintoSchemas = {
  INDUSTRIAL_CLASS: industrialClass,
  INDUSTRIAL_CLASS_ARRAY: new schema.Array(industrialClass),
  LIFE_EVENT: lifeEvent,
  LIFE_EVENT_ARRAY: new schema.Array(lifeEvent),
  TARGET_GROUP: targetGroup,
  TARGET_GROUP_ARRAY: new schema.Array(targetGroup),
  ONTOLOGY_TERM: ontologyTerm,
  ONTOLOGY_TERM_ARRAY: new schema.Array(ontologyTerm),
  ANNOTATION_ONTOLOGY_TERMS:annotationOntologyTerms,
  DIGITAL_AUTHORIZATION: digitalAuthorization,
  DIGITAL_AUTHORIZATION_ARRAY: new schema.Array(digitalAuthorization),
  SERVICE_CLASS_ARRAY: new schema.Array(serviceClass),
  SERVICE_CLASS: serviceClass
}
