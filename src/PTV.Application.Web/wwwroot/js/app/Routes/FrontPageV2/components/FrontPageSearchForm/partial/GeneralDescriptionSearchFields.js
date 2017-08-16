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
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import { injectIntl, intlShape } from 'react-intl'
import { renderTargetGroups, renderServiceClasses } from '../renders'

const GeneralDescriptionFields = ({ intl:{ locale } }) => (
  <div className='form-group'>
    <div className='row'>
      <div className='col-md-4'>
        <Field
          locale={locale}
          name='serviceClassId'
          component={renderServiceClasses}
          className='form-field'
          searchable
        />
      </div>
      <div className='col-md-4'>
        <Field
          locale={locale}
          name='targetGroupId'
          component={renderTargetGroups}
          className='form-field'
        />
      </div>
    </div>
  </div>
)

GeneralDescriptionFields.propTypes = {
  intl: intlShape.isRequired
}

export default compose(injectIntl)(GeneralDescriptionFields)
