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
import React, { PureComponent } from 'react'
import { Email, AdditionalInformation } from 'util/redux-form/fields'
import { withFormStates } from 'util/redux-form/HOC'
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'react-intl'

export const organizationEmailMessages = defineMessages({
  emailLabel: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.Title',
    defaultMessage: 'Sähköpostiosoite'
  },
  emailTooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.Tooltip',
    defaultMessage: "Kirjoita organisaation yleinen sähköpostiosoite, esim. kirjaamoon tai asiakaspalveluun. Voit antaa useita sähköpostiosoitteita 'uusi sähköpostiosoite' -painikkeella."
  },
  emailPlaceholder: {
    id: 'Containers.Manage.Organizations.Manage.Step2.Email.Placeholder',
    defaultMessage: 'esim. asiakaspalvelu@yritys.fi'
  }
})

class OrganizationEmail extends PureComponent {
  static defaultProps = {
    name: 'email'
  }
  render () {
    const {
      isCompareMode,
      splitView,
      intl: { formatMessage },
      ...rest
    } = this.props
    const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
    const indentCompareModeClass = isCompareMode || splitView ? 'col-lg-24 mb-2' : 'col-lg-12 mb-2 mb-lg-0'
    return (
      <div className='row'>
        <div className={indentCompareModeClass}>
          <Email isCompareMode={isCompareMode}
            label={formatMessage(organizationEmailMessages.emailLabel)}
            tooltip={formatMessage(organizationEmailMessages.emailTooltip)}
            placeholder={formatMessage(organizationEmailMessages.emailPlaceholder)}
            isLocalized={false}
            {...rest} />
        </div>
        <div className={basicCompareModeClass}>
          <AdditionalInformation isLocalized={false} isCompareMode={isCompareMode} />
        </div>
      </div>
    )
  }
}

export default compose(
  injectIntl,
  withFormStates
)(OrganizationEmail)
