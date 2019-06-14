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
import { compose } from 'redux'
import { FormSection } from 'redux-form/immutable'
import ExceptionalOpeningHourSelection from 'util/redux-form/sections/ExceptionalOpeningHourSelection'
import {
  Title
} from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.InfoTitle',
    defaultMessage: 'Lisätieto'
  },
  placeholder: {
    id: 'ReduxForm.Sections.Title.Placeholder',
    defaultMessage: 'Anna tarvittaessa otsikko tai lisätieto'
  }
})

class ExceptionalOpeningHours extends FormSection {
  static defaultProps = {
    name: 'exceptionalOpeningHour',
    intl: intlShape
  }
  render () {
    const {
      index,
      compare,
      intl: { formatMessage }
    } = this.props
    return (
      <div>
        <div className='form-group'>
          <Title
            maxLength={100}
            label={formatMessage(messages.label)}
            placeholder={formatMessage(messages.placeholder)}
            isCompareMode={false}
            compare={compare}
          />
        </div>
        <ExceptionalOpeningHourSelection index={index} />
      </div>
    )
  }
}

export default compose(
  injectIntl
)(ExceptionalOpeningHours)
