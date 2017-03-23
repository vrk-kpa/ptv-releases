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
import { enumFactory, stringEnumFactory } from './EnumFactory.js';

export const publishingStatuses = enumFactory(['draft', 'published', 'deleted', 'modified', 'oldPublished']);
export const channelType = enumFactory(['eChannel']);
export const validityTypesEnum = enumFactory(['onward', 'period']);
export const streetTypes = enumFactory(['street', 'postBox', 'both']);
export const openingHoursTypes = enumFactory(['normal', 'exceptional', 'special']);
export const openingHoursExtraVisibility = enumFactory(['notSet', 'hide', 'show']);
export const weekdaysEnum = enumFactory(['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday']);

export const permisionTypes = {
    create: 1,
    read: 2,
    update: 4,
    delete: 8,
    publish: 16
}

export const channelTypes = {
    ELECTRONIC: 'eChannel',
    WEB_PAGE: 'webPage',
    PHONE: 'phone',
    PRINTABLE_FORM: 'printableForm',
    SERVICE_LOCATION: 'serviceLocation'
}

// string enums
export const keyToStates = stringEnumFactory([
    'service',
    'organization',
    'generalDescription',
    channelTypes.ELECTRONIC,
    channelTypes.WEB_PAGE,
    channelTypes.PHONE,
    channelTypes.PRINTABLE_FORM,
    channelTypes.SERVICE_LOCATION,
])

export const keyToEntities = {
    'service': 'services',
    'organization': 'organizations',
    'generalDescription': 'generalDescriptions',
    [channelTypes.ELECTRONIC]: 'channels',
    [channelTypes.WEB_PAGE]: 'channels',
    [channelTypes.PHONE]: 'channels',
    [channelTypes.PRINTABLE_FORM]: 'channels',
    [channelTypes.SERVICE_LOCATION]: 'channels',
}