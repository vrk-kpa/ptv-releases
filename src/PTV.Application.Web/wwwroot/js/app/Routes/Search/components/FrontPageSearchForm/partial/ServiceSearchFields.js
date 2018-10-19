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
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { injectIntl, intlShape } from 'util/react-intl'
import { getIsKR1Selected, getIsKR2Selected } from '../../../selectors'
import SearchFilterLifeEvents from '../../SearchFilterLifeEvents'
import SearchFilterTargetGroups from '../../SearchFilterTargetGroups'
import SearchFilterServiceTypes from '../../SearchFilterServiceTypes'
import SearchFilterServiceClasses from '../../SearchFilterServiceClasses'
import SearchFilterOntologyTerms from '../../SearchFilterOntologyTerms'
import SearchFilterIndustrialClasses from '../../SearchFilterIndustrialClasses'
import SearchFilterGeneralDescriptionTypes from '../../SearchFilterGeneralDescriptionTypes'

const ServiceSearchFields = ({
  intl : { locale },
  isKR1Selected,
  isKR2Selected,
  gdSelected
}) => (
  <div>
    <div className='row'>
      <div className='col-md-8'>
        <SearchFilterServiceClasses />
      </div>
      <div className='col-md-8'>
        <SearchFilterTargetGroups />
      </div>
      {isKR1Selected &&
      <div className='col-md-8'>
        <SearchFilterLifeEvents />
      </div>}
    </div>
    <div className='row'>
      {gdSelected &&
        <div className='col-md-8 mt-md-2'>
          <SearchFilterGeneralDescriptionTypes />
        </div>}
      {gdSelected &&
        <div className='col-md-8 mt-md-2'>
          <SearchFilterServiceTypes />
        </div>
      }
      <div className='col-md-8 mt-md-2'>
        <SearchFilterOntologyTerms />
      </div>
      {isKR2Selected &&
      <div className='col-md-8 mt-md-2'>
        <SearchFilterIndustrialClasses />
      </div>}
    </div>
  </div>
)

ServiceSearchFields.propTypes = {
  intl: intlShape.isRequired,
  isKR1Selected: PropTypes.bool.isRequired,
  isKR2Selected: PropTypes.bool.isRequired,
  gdSelected: PropTypes.any
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isKR1Selected: getIsKR1Selected(state, ownProps),
    isKR2Selected: getIsKR2Selected(state, ownProps)
  })))(ServiceSearchFields)
