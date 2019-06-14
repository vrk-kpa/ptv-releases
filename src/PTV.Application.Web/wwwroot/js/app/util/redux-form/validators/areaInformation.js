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
import { Map, List } from 'immutable'
import { createError } from './isValid'
import messages from './messages'

const validateMunicipality = areaInformation => {
  const value = areaInformation.get('municipalities')
  if (!value || value.size === 0) {
    return createError(
      'areaInformation.municipalities',
      messages.isRequired
    )
  }
}
const validateProvince = areaInformation => {
  const value = areaInformation.get('provinces')
  if (!value || value.size === 0) {
    return createError(
      'areaInformation.provinces',
      messages.isRequired
    )
  }
}
const validateBusinessRegions = areaInformation => {
  const value = areaInformation.get('businessRegions')
  if (!value || value.size === 0) {
    return createError(
      'areaInformation.businessRegions',
      messages.isRequired
    )
  }
}
const validateHospitalRegions = areaInformation => {
  const value = areaInformation.get('hospitalRegions')
  if (!value || value.size === 0) {
    return createError(
      'areaInformation.hospitalRegions',
      messages.isRequired
    )
  }
}

const isValidAreaInformation = label => (areaInformation, formValues, props) => {
  const { dispatch } = props
  if (!dispatch) {
    throw new Error(
      `dispatch not provided for isValidAreaInformation(${label}) validator`
    )
  }
  let organizationTypes
  let areaInformationTypes
  let areaTypeTypes
  dispatch(({ getState }) => {
    const state = getState()
    const entitiesPath = ['common', 'entities']
    const reduceFn = (acc, curr) => acc.set(curr.get('id'), curr.get('code'))
    organizationTypes = (state.getIn([...entitiesPath, 'organizationTypes']) || List())
      .reduce(reduceFn, Map())
    areaInformationTypes = (state.getIn([...entitiesPath, 'areaInformationTypes']) || List())
      .reduce(reduceFn, Map())
    areaTypeTypes = (state.getIn([...entitiesPath, 'areaTypes']) || List())
      .reduce(reduceFn, Map())
  })
  const organizationTypeId = formValues.get('organizationType')
  const areaInformationTypeId = areaInformation.get('areaInformationType')
  const areaTypeId = areaInformation.get('areaType')

  const organizationType = organizationTypes.get(organizationTypeId)
  const areaInformationType = areaInformationTypes.get(areaInformationTypeId)
  const areaType = areaTypeTypes.get(areaTypeId)

  // console.log('organizationType:\n', organizationType)
  // console.log('areaInformationType:\n', areaInformationType)
  // console.log('areaType:\n', areaType)
  // console.log('---------------------------------------')

  if (typeof organizationType === 'undefined' || organizationType === null) {
    return createError(label, messages.isRequired)
  }
  if (areaInformationType === 'AreaType' && !areaType) {
    return createError(label, messages.isRequired)
  }
  // Feel free to come up with a better name //
  const shouldValidateAreaInWhichTheOrganisationPrimarilyProvidesServices = (
    organizationType !== 'Municipality' &&
    (
      ['State', 'Organization', 'Company', 'SotePrivate', 'SotePublic']
        .some(x => x === organizationType) && areaInformationType === 'AreaType'
    )
  )
  if (shouldValidateAreaInWhichTheOrganisationPrimarilyProvidesServices) {
    switch (areaType) {
      case 'Municipality': return validateMunicipality(areaInformation)
      case 'Province': return validateProvince(areaInformation)
      case 'BusinessRegions': return validateBusinessRegions(areaInformation)
      case 'HospitalRegions': return validateHospitalRegions(areaInformation)
    }
  }
}

export default isValidAreaInformation
