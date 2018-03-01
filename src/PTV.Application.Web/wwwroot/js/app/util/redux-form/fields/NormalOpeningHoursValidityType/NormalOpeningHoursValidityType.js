/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import { formValueSelector } from 'redux-form/immutable'
import OpeningHoursValidityType from '../OpeningHoursValidityType'
import { injectIntl, intlShape } from 'react-intl'
import { DateRange } from 'util/redux-form/fields'
import { injectFormName, asDisableable } from 'util/redux-form/HOC'
import withPath from 'util/redux-form/HOC/withPath'
import { compose } from 'redux'
import { connect } from 'react-redux'

const NormalOpeningHoursValidityType = ({
  intl: { formatMessage },
  isPeriod,
  section,
  index
}) => (
  <div>
    <OpeningHoursValidityType />
    {isPeriod &&
      <div className='form-row'>
        <DateRange />
      </div>
    }
  </div>
)

NormalOpeningHoursValidityType.propTypes = {
  intl: intlShape,
  isPeriod: PropTypes.bool.isRequired,
  section: PropTypes.string,
  index: PropTypes.number
}

export default compose(
  injectIntl,
  injectFormName,
  withPath,
  connect(
    (state, { index, formName, path }) => {
      const dailyOpeningHoursValueSelector = formValueSelector(formName)
      const isPeriod = dailyOpeningHoursValueSelector(state, `${path}.isPeriod`) || false
      return {
        isPeriod
      }
    }
  ),
  asDisableable
)(NormalOpeningHoursValidityType)
