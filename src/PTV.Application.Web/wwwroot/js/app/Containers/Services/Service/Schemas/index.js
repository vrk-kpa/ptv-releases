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
import { schema } from 'normalizr'
import { CommonChannelsSchemas } from '../../../Channels/Common/Schemas'
import { CommonSchemas } from '../../../Common/Schemas'
import { GeneralDescriptionSchemas } from '../../../Manage/GeneralDescriptions/GeneralDescriptions/Schemas'

const servicesSchema = new schema.Entity('services', {
  serviceProducers: new schema.Array(serviceProducerSchema),
  serviceVouchers: CommonSchemas.SERVICE_VOUCHER_ARRAY,
  attachedChannels: CommonChannelsSchemas.CHANNEL_ARRAY,
  newKeyWords: CommonSchemas.KEYWORD_ARRAY,
  keyWords: CommonSchemas.KEYWORD_ARRAY,
  generalDescription: GeneralDescriptionSchemas.GENERAL_DESCRIPTION,
  ontologyTerms: CommonSchemas.ONTOLOGY_TERM_ARRAY,
  annotationOntologyTerms: CommonSchemas.ONTOLOGY_TERM_ARRAY,
  serviceClasses: CommonSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: CommonSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: CommonSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: CommonSchemas.CONNECTION_ARRAY,
  laws: CommonSchemas.LAW_ARRAY,
  security: CommonSchemas.SECURITY
},
{ meta: { localizable: true } })

const servicesSchema_v2 = new schema.Entity('services', {
  serviceProducers: new schema.Array(serviceProducerSchema),
  serviceVouchers: CommonSchemas.SERVICE_VOUCHER_ARRAY,
  attachedChannels: CommonChannelsSchemas.CHANNEL_ARRAY,
  newKeyWords: CommonSchemas.KEYWORD_ARRAY,
  keyWords: CommonSchemas.KEYWORD_ARRAY,
  generalDescription: GeneralDescriptionSchemas.GENERAL_DESCRIPTION,
  ontologyTerms: CommonSchemas.ONTOLOGY_TERM_ARRAY,
  annotationOntologyTerms: CommonSchemas.ONTOLOGY_TERM_ARRAY,
  serviceClasses: CommonSchemas.SERVICE_CLASS_ARRAY,
  lifeEvents: CommonSchemas.LIFE_EVENT_ARRAY,
  industrialClasses: CommonSchemas.INDUSTRIAL_CLASS_ARRAY,
  connections: CommonSchemas.CONNECTION_ARRAY,
  laws: CommonSchemas.LAW_ARRAY,
  security: CommonSchemas.SECURITY
})
const serviceProducerSchema = new schema.Entity('serviceProducers')

export const ServiceSchemas = {
  SERVICE: servicesSchema,
  SERVICE_ARRAY: new schema.Array(servicesSchema),
  SERVICE_V2: servicesSchema_v2,
  SERVICE_ARRAY_V2: new schema.Array(servicesSchema_v2)
}
