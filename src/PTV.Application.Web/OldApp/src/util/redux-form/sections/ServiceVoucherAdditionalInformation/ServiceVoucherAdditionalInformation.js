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
import { compose } from 'redux'
import { AdditionalInformation } from 'util/redux-form/fields'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import asCollection from 'util/redux-form/HOC/asCollection'

export const voucherMessages = defineMessages({
  additionlInformationTitle:{
    id: 'Containers.Services.AddService.VoucherNoUrl.AdditionlInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  additionlInformationPlaceholder:{
    id: 'Containers.Services.AddService.VoucherNoUrl.AdditionlInformation.Placeholder',
    defaultMessage: 'Kirjoita lisätietoa palvelusetelipalvelusta'
  }
})

const ServiceVoucherAdditionalInformation = ({
  isCompareMode,
  compare,
  intl: { formatMessage }
}) => {
  return (
    <div>
      <div className='row'>
        <div className={'col-lg-24'}>
          <AdditionalInformation
            isCompareMode={isCompareMode}
            name='additionalInformation'
            label={formatMessage(voucherMessages.additionlInformationTitle)}
            placeholder={formatMessage(voucherMessages.additionlInformationPlaceholder)}
            counter
            rows={3}
            multiline
            maxLength={150}
            isLocalized={false}
            noTranslationLock
            useQualityAgent
            compare={compare}
          />
        </div>
      </div>
    </div>
  )
}

ServiceVoucherAdditionalInformation.propTypes = {
  isCompareMode: PropTypes.bool,
  intl: intlShape.isRequired,
  compare: PropTypes.bool
}
export default compose(
  injectIntl,
  asLocalizableSection('serviceVouchers'),
  asCollection({
    name: 'serviceVoucher'
  }),
  asSection('serviceVouchers'),
  withFormStates
)(ServiceVoucherAdditionalInformation)
