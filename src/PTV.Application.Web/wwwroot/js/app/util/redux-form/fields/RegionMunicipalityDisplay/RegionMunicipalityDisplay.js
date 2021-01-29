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
import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { localizeList } from 'appComponents/Localize'
import { RenderMultiSelectDisplay } from 'util/redux-form/renders'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import {
  getMunicipalitiesJS,
  getRegionMunicipalities
} from './selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { getSelectedLanguage } from 'Intl/Selectors'

const RegionMunicipalityDisplay = ({
  options,
  customValue,
  sortLanguage,
  ...rest
}) => {
  const getCustomValue = () => customValue

  return (<Field
    name='regionMunicipalities'
    component={RenderMultiSelectDisplay}
    options={options}
    getCustomValue={getCustomValue}
    sortLanguage={sortLanguage}
    {...rest}
  />)
}

RegionMunicipalityDisplay.propTypes = {
  customValue: ImmutablePropTypes.list.isRequired,
  options: PropTypes.array.isRequired,
  sortLanguage: PropTypes.string
}

export default compose(
  injectFormName,
  connect((state, ownProps) => ({
    options: getMunicipalitiesJS(state, ownProps),
    customValue: getRegionMunicipalities(state, ownProps),
    sortLanguage: getSelectedLanguage(state)
  })),
  asDisableable,
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label'
  })
)(RegionMunicipalityDisplay)
