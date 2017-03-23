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
import React, { PropTypes } from 'react'
import { Field } from 'redux-form/immutable'
import {
  renderOrganizations,
  renderPublishingStatuses,
  renderLanguages
} from '../renders'
import {
  renderSearchField
} from 'Utils/ReduxForm/renders'
import { SearchTypeSwitcher } from 'Routes/FrontPageV2/components'
import 'Routes/FrontPageV2/styles/style.scss'
import { injectIntl, defineMessages } from 'react-intl'
import { compose } from 'redux'

const messages = defineMessages({
  nameTitle: {
    id: 'FrontPage.CommonSearchFields.Name.Title',
    defaultMessage: 'Hakusana'
  },
  nameTooltip: {
    id: 'FrontPage.CommonSearchFields.Name.Tooltip',
    defaultMessage: 'Hakusana'
  }
})

const CommonSearchFields = ({ intl: { locale, formatMessage } }) => (
  <div>
    <div className='form-group'>
      <div className='row'>
        <div className='col-md-8 col-lg-4'>
          <Field
            name='name'
            label={formatMessage(messages.nameTitle)}
            tooltip={formatMessage(messages.nameTooltip)}
            component={renderSearchField}
            className='form-field'
          />
        </div>
        <div className='col-md-12 col-lg-8'>
          <Field
            name='selectedPublishingStatuses'
            locale ={locale}
            component={renderPublishingStatuses}
            className='form-field'
          />
        </div>
      </div>
    </div>
    <div className='form-group'>
      <div className='row'>
        <div className='col-md-4'>
          <SearchTypeSwitcher />
        </div>
        <div className='col-md-4'>
          <Field
            locale ={locale}
            name='organizationId'
            component={renderOrganizations}
            className='form-field'
            searchable
          />
        </div>
        <div className='col-md-4'>
          <Field
            locale ={locale}
            name='languages'
            component={renderLanguages}
            className='form-field'
          />
        </div>
      </div>
    </div>
  </div>
)

CommonSearchFields.propTypes = {
  intl: PropTypes.object.isRequired
}

export default compose(injectIntl)(CommonSearchFields)
