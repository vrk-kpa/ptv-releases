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
import { defineMessages, injectIntl, FormattedMessage, intlShape } from 'util/react-intl'
import {
  ServiceChargeType,
  ShortDescription
} from 'util/redux-form/fields'
import asGroup from 'util/redux-form/HOC/asGroup'
import asSection from 'util/redux-form/HOC/asSection'
import withFormStates from 'util/redux-form/HOC/withFormStates'

export const serviceChargeTypeMessages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step1.ChargeType.Title',
    defaultMessage: 'Maksullisuuden tiedot'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step1.ChargeType.Tooltip',
    defaultMessage: 'Missing'
  },
  additionalInfoTitle: {
    id: 'Containers.Services.AddService.Step1.ChargeTypeAdditionalInfo.Title',
    defaultMessage: 'Maksullisuuden lisätieto'
  },
  additionalInfoPlaceholder: {
    id: 'Containers.Services.AddService.Step1.ChargeTypeAdditionalInfo.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  chargeTypeTitle:{
    id: 'Containers.Services.AddService.Step1.ChargeTypes.Title',
    defaultMessage: 'Maksullisuuden tyyppi'
  }
})

const ServiceChargeTypeSection = ({
  intl: { formatMessage },
  customComponent,
  isCompareMode
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      {customComponent && customComponent() ||
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ServiceChargeType
              labelTop
              label={formatMessage(serviceChargeTypeMessages.chargeTypeTitle)}
            />
          </div>
        </div>
      </div>
      }
      <div className='form-row'>
        <ShortDescription
          name='additionalInformation'
          label={formatMessage(serviceChargeTypeMessages.additionalInfoTitle)}
          placeholder={formatMessage(serviceChargeTypeMessages.additionalInfoPlaceholder)}
          counter
          multiline
          rows={3}
          maxLength={500}
          labelTop
          useQualityAgent
          type='ChargeTypeAdditionalInfo'
        />
      </div>
    </div>
  )
}

ServiceChargeTypeSection.propTypes = {
  intl: intlShape.isRequired,
  customComponent: PropTypes.func,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  asGroup({
    title: serviceChargeTypeMessages.title,
    tooltip: <FormattedMessage {...serviceChargeTypeMessages.tooltip} />
  }),
  asSection('chargeType')
)(ServiceChargeTypeSection)
