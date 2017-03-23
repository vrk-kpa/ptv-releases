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
import { createSelector } from 'reselect'
import { List, Map } from 'immutable'
import { getEntities, getEntitiesForIds } from 'Containers/Common/Selectors'
import { getServiceClasses, getTargetGroups, getTopTargetGroups, getOntologyTerms } from 'Containers/Services/Common/Selectors'

export const getGeneralDescriptions = createSelector(
    getEntities,
    search => search.get('generalDescriptions') || new Map()
)

const getId = (state, props) => props.id || props.generalDescriptionId || ''

export const getGeneralDescription = createSelector(
    [getGeneralDescriptions, getId],
    (generalDescriptions, id) => generalDescriptions.get(id) || new Map()
)

const getLocale = (state, props) => props.language || props.languageCode || props.locale || ''

export const getName = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('name') || {}
)

export const getNameLocale = createSelector(
    [getName, getLocale],
    (name, locale) => name.get(locale) || ''
)

export const getAlternateName = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('alternateName') || Map()
)

export const getAlternateNameLocale = createSelector(
    [getAlternateName, getLocale],
    (alternateName, locale) => alternateName.get(locale) || ''
)

export const getShortDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('shortDescription') || Map()
)

export const getShortDescriptionLocale = createSelector(
    [getShortDescription, getLocale],
    (shortDescription, locale) => shortDescription.get(locale) || ''
)

export const getDescription = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('description') || Map()
)

export const getDescriptionLocale = createSelector(
    [getDescription, getLocale],
    (description, locale) => description.get(locale) || ''
)

export const getServiceClassesIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('serviceClasses') || List()
)

export const getGeneralDescriptionServiceClasses = createSelector(
    [getServiceClasses, getServiceClassesIds],
    (serviceClasses, serviceClassesIds) => getEntitiesForIds(serviceClasses, serviceClassesIds) || Map()
)

export const getTargetGroupsIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('targetGroups') || List()
)

export const getGeneralDescriptionTargetGroups = createSelector(
    [getTargetGroupsIds, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => getEntitiesForIds(entities, ids.filter(x => top.includes(x)), List())
)

export const getGeneralDescriptionSubTargetGroups = createSelector(
    [getTargetGroupsIds, getTargetGroups, getTopTargetGroups],
    (ids, entities, top) => getEntitiesForIds(entities, ids.filter(x => !top.includes(x)), List())
)

export const getGeneralDescriptionOntologyTermsIds = createSelector(
    getGeneralDescription,
    generalDescription => generalDescription.get('ontologyTerms') || List()
)

export const getGeneralDescriptionOntologyTerms = createSelector(
     [getGeneralDescriptionOntologyTermsIds, getOntologyTerms],
     (ids, entities) => getEntitiesForIds(entities, ids, List())
)
