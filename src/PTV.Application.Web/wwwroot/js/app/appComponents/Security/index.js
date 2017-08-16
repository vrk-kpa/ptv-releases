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
import { createSecurity, createSecurityAnyDomain } from './composeSecurity'
import { permisionTypes, securityOrganizationCheckTypes } from '../../Containers/Common/Enums'
console.log(securityOrganizationCheckTypes, securityOrganizationCheckTypes.byOrganization)

export const SecurityRead = createSecurity(permisionTypes.read)
export const SecurityUpdate = createSecurity(permisionTypes.update)
export const SecurityCreate = createSecurity(permisionTypes.create)
export const SecurityDelete = createSecurity(permisionTypes.delete)

// checks security if exists in any domains
export const SecurityCreateAny = createSecurityAnyDomain(permisionTypes.create)

// organization specific
export const OwnOrgSecurityRead = createSecurity(permisionTypes.read, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityUpdate = createSecurity(permisionTypes.update, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityCreate = createSecurity(permisionTypes.create, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityDelete = createSecurity(permisionTypes.delete, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityWithdraw = createSecurity(permisionTypes.delete, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityPublish = createSecurity(permisionTypes.publish, securityOrganizationCheckTypes.byOrganization)
export const OwnOrgSecurityRestore = createSecurity(permisionTypes.publish, securityOrganizationCheckTypes.byOrganization)

export const NotOwnOrgSecurityCreate = createSecurity(permisionTypes.create, securityOrganizationCheckTypes.otherOrganization)
