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
import React from 'react'
import { RenderMultiSelect } from 'util/redux-form/renders'
import RegionMunicipalityDisplay from 'util/redux-form/fields/RegionMunicipalityDisplay'
import withEmptyCheck from 'util/redux-form/HOC/withEmptyCheck'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asComparable from 'util/redux-form/HOC/asComparable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { localizeList } from 'appComponents/Localize'
import { getHospitalRegionsForIdsJs } from './selectors'

const List = compose(
  injectFormName,
  withFormStates,
  connect(
    (state, ownProps) => {
      const value = getHospitalRegionsForIdsJs(state,
        { ids:  ownProps.input.value, formName: ownProps.formName, disabledAll: ownProps.isReadOnly })
      return {
        value
      }
    }
  ),
  localizeList({
    input: 'value',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  withEmptyCheck(value => value && value.length > 0)
)(RenderMultiSelect)

const valueRenderer = (option) => {
  return (
    <RegionMunicipalityDisplay municipalityId={option.value} label={option.label} />
  )
}

const AreaTypeHospitalRegion = props => (
  <Field
    name='hospitalRegions'
    component={List}
    renderAsTags
    valueRenderer={valueRenderer}
    {...props}
  />
)

export default compose(
  asComparable({ DisplayRender: List }),
  asDisableable
)(AreaTypeHospitalRegion)
