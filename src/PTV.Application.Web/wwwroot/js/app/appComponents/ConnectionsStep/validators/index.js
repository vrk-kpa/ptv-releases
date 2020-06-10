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

const checkLocalized = check => ({ value }) => {
  return value != null && value.size > 0 && value.some(x => check({ value: x }))
}

const checkTextValue = ({ value }) => (typeof value === 'string' && value.trim().length > 0)

const checkList = ({ value }) => {
  return value != null && value.size > 0
}

const checkType = ({ value }) => {
  return value != null && value !== ''
}

const isValid = (validationFunc) =>
  (value) => {
    const result = validationFunc({ value })
    return result ? { result: 'ok' } : null
  }

const isNotNullInPath = isValid(checkType)
const isNotEmptyInPath = isValid(checkLocalized(checkTextValue))
const isNotEmptyListInPath = isValid(checkList)
const isNotEmptyLocalizedListInPath = isValid(checkLocalized(checkList))

export const additionalInformationValidators = [
  { path: 'basicInformation.description', validate: isNotEmptyInPath },
  { path: 'basicInformation.chargeType', validate: isNotNullInPath },
  { path: 'basicInformation.additionalInformation', validate: isNotEmptyInPath },
  { path: 'digitalAuthorization.digitalAuthorizations', validate: isNotEmptyListInPath },
  { path: 'openingHours.exceptionalOpeningHours', validate: isNotEmptyListInPath },
  { path: 'openingHours.normalOpeningHours', validate: isNotEmptyListInPath },
  { path: 'openingHours.specialOpeningHours', validate: isNotEmptyListInPath },
  { path: 'contactDetails.emails', validate: isNotEmptyLocalizedListInPath },
  { path: 'contactDetails.faxNumbers', validate: isNotEmptyLocalizedListInPath },
  { path: 'contactDetails.phoneNumbers', validate: isNotEmptyLocalizedListInPath },
  { path: 'contactDetails.webPages', validate: isNotEmptyLocalizedListInPath },
  { path: 'contactDetails.postalAddresses', validate: isNotEmptyListInPath }
]
