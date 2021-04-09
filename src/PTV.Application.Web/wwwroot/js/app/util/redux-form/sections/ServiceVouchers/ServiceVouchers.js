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
import {
  Name,
  UrlChecker,
  AdditionalInformation
} from 'util/redux-form/fields'
import asCollection from 'util/redux-form/HOC/asCollection'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'

export const voucherMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Services.AddService.Step1.Voucher.Name.Title',
    defaultMessage: 'Nimi'
  },
  nameTooltip: {
    id: 'Containers.Services.AddService.Step1.Voucher.Name.Tooltip',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Services.AddService.Step1.Voucher.Name.Placeholder',
    defaultMessage: 'Kirjoita palvelusetelituottajan tai verkkosivun nimi'
  },
  urlLabel: {
    id: 'Containers.Services.AddService.Step1.Voucher.Url.Title',
    defaultMessage: 'Linki palvelusetelituottajan tietoihin'
  },
  urlTooltip: {
    id: 'Containers.Services.AddService.Step1.Voucher.Url.Tooltip',
    defaultMessage: 'Linki palvelusetelituottajan tietoihin'
  },
  urlPlaceholder: {
    id: 'Containers.Services.AddService.Step1.Voucher.Url.Placeholder',
    defaultMessage: 'http://www.palveluntuottajat.fi'
  },
  urlButton: {
    id: 'Containers.Services.AddService.Step1.Voucher.Url.Button.Title',
    defaultMessage: 'Testaa osoite'
  },
  urlCheckerInfo: {
    id: 'Containers.Services.AddService.Step1.Voucher.Url.Icon.Tooltip',
    defaultMessage: 'Verkko-osoitetta ei löytynyt, tarkista sen muoto.'
  },
  orderTitle: {
    id: 'Containers.Services.AddService.Step1.Voucher.Order.Title',
    defaultMessage: 'Esitysjärjestys'
  },
  additionlInformationTitle:{
    id: 'Containers.Services.AddService.Step1.Voucher.AdditionlInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  additionlInformationPlaceholder:{
    id: 'Containers.Services.AddService.Step1.Voucher.AdditionlInformation.Placeholder',
    defaultMessage: 'Kirjoita lisätietoa palvelusetelipalvelusta'
  },
  addBtnTitle: {
    id : 'Util.ReduxForm.Sections.ServiceVouchers.AddButton.Title',
    defaultMessage: '+ Uusi palveluseteli'
  }
})

const ServiceVouchers = ({
  isCompareMode,
  splitView,
  compare,
  intl: { formatMessage }
}) => {
  const basciCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='row'>
        <div className={basciCompareModeClass}>
          <Name
            isCompareMode={isCompareMode}
            label={formatMessage(voucherMessages.nameTitle)}
            tooltip={formatMessage(voucherMessages.nameTooltip)}
            placeholder={formatMessage(voucherMessages.namePlaceholder)}
            skipValidation
            isLocalized={false}
            noTranslationLock
            useQualityAgent
            collectionPrefix='serviceVouchers'
            compare={compare}
          />
        </div>
      </div>
      <div className='form-row'>
        <UrlChecker
          isCompareMode={isCompareMode}
          label={formatMessage(voucherMessages.urlLabel)}
          tooltip={formatMessage(voucherMessages.urlTooltip)}
          placeholder={formatMessage(voucherMessages.urlPlaceholder)}
          isLocalized={false}
          splitView={splitView}
        />
      </div>
      <div className='form-row'>
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
          collectionPrefix='serviceVouchers'
          compare={compare}
        />
      </div>
    </div>
  )
}

ServiceVouchers.propTypes = {
  isCompareMode: PropTypes.bool,
  splitView: PropTypes.bool,
  intl: intlShape.isRequired,
  compare: PropTypes.bool
}
export default compose(
  injectIntl,
  asLocalizableSection('serviceVouchers'),
  asCollection({
    name: 'serviceVoucher',
    addBtnTitle: <FormattedMessage {...voucherMessages.addBtnTitle} />
  }),
  asSection('serviceVouchers'),
  withFormStates
)(ServiceVouchers)
