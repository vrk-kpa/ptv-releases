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
import { Schema, arrayOf } from 'normalizr';
import { CommonChannelsSchemas } from '../../../../Channels/Common/Schemas';
import { CommonSchemas } from '../../../../Common/Schemas';
import { ServiceSchemas } from '../../../../Services/Service/Schemas';
import shortId from 'shortid';

//ChannelsRelation 
const channelsRelationsSchema = new Schema('channelRelations', { idAttribute: serviceAndChannel => serviceAndChannel.id });
channelsRelationsSchema.define({  
  connectedChannel: CommonChannelsSchemas.CHANNEL,
  digitalAuthorizations: CommonSchemas.DIGITAL_AUTHORIZATION_ARRAY  
})

//Services
const connectedServicesSchema = new Schema('connectedServices', { idAttribute: service => service.uiId });
connectedServicesSchema.define({
   channelRelations: arrayOf(channelsRelationsSchema),
   service: ServiceSchemas.SERVICE
})

//Main Schema
const relationsSchema = new Schema('relations', { idAttribute: connectedServiceAndChannel => 'serviceAndChannelsId' });
relationsSchema.define({
   connectedServices: arrayOf(connectedServicesSchema),
   connectedChannels: arrayOf(CommonChannelsSchemas.CHANNEL) //Own added channels
})

export const ServiceAndChannelsSchemas = {
  CONNECTED_SERVICE: connectedServicesSchema,
  CONNECTED_SERVICE_ARRAY: arrayOf(connectedServicesSchema),
  CHANNEL_RELATION: channelsRelationsSchema,
  CHANNEL_RELATION_ARRAY: arrayOf(channelsRelationsSchema),
  RELATION: relationsSchema,
  RELATION_ARRAY: arrayOf(relationsSchema),
}