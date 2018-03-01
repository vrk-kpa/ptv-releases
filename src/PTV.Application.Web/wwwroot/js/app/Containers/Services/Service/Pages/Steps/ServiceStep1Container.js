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
import { connect } from 'react-redux'
import React from 'react'
import PropTypes from 'prop-types'
import { injectIntl } from 'react-intl'

// Components
import ServiceType from '../../../Common/Pages/serviceTypes'

import ServiceChargeType from '../../../Common/Pages/serviceChargeTypes'
import ServiceLanguage from '../../../Common/Pages/serviceLanguages'
import ServiceTypeAdditionalInfo from '../../../Common/Pages/serviceTypeAdditionalInfo'
import ServiceGenerealDescription from '../../../Common/Pages/serviceGeneralDescription'
import ServiceNames from '../../../Common/Pages/serviceNames'
import ServiceDescriptions from '../../../Common/Pages/serviceDescriptions'
import PublishingStatus from '../../../../Common/PublishingStatus'
import LanguageLabel from '../../../../Common/languageLabel'
import ServiceGeneralDecriptionLaws from '../../../Common/Pages/serviceGeneralDecriptionLaws'
import ServiceLaws from '../../../Common/Pages/serviceLaws'
import ServiceVouchers from '../../../Common/Pages/serviceVouchers'
import ServiceOrganizations from '../../../Common/Pages/serviceOrganizations'
import ServiceAreaInformation from '../../../Common/Pages/serviceAreaInformation'
import FundingType from '../../../Common/Pages/serviceFundingType'

// Actions
import * as mainActions from '../../Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// Selectors
import * as ServiceSelectors from '../../Selectors'

// Messages
import * as Messages from '../../Messages'

export const ServiceStep1Container = props => {
  const { readOnly, language, publishingStatus, translationMode, splitContainer, entityId } = props
  const sharedProps = { readOnly, translationMode, language, entityId, splitContainer }

  return (
    <div className='step-1'>
      <LanguageLabel {...sharedProps}
            />
      { entityId
                ? <PublishingStatus publishingStatus={publishingStatus} language={language} pageType='service' />
            : null}
      <ServiceGenerealDescription {...sharedProps}
        messages={Messages.connectGeneralDescriptionMessages}
            />
      <ServiceType {...sharedProps}
        messages={Messages.serviceTypeMessages}
            />
      <FundingType {...sharedProps}
        messages={Messages.fundingTypeMessages}
            />
      <ServiceNames {...sharedProps}
        messages={Messages.serviceNamesMessages}
            />
      <ServiceOrganizations {...sharedProps}
        messages={Messages.serviceOrganizationMessages}
            />
      <ServiceDescriptions {...sharedProps}
        messages={Messages.serviceDescriptionMessages}
            />
      <ServiceChargeType {...sharedProps}
        messages={Messages.serviceChargeTypeMessages}
            />
      <ServiceVouchers {...sharedProps}
        messages={Messages.voucherMessages}
        collapsible
        withOrder
        withName
            />
      <ServiceLaws {...sharedProps}
        messages={Messages.lawsMessages}
            />
      <ServiceLanguage {...sharedProps}
        messages={Messages.serviceAvailableLanguageMessages}
            />
      <ServiceAreaInformation {...sharedProps}
        messages={Messages.areaInformationMessages}
            />
      <ServiceTypeAdditionalInfo {...sharedProps}
        messages={Messages}
            />
      <ServiceGeneralDecriptionLaws {...sharedProps}
        messages={Messages.lawsMessages}
            />

    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    publishingStatus: ServiceSelectors.getPublishingStatus(state, ownProps)
  }
}

const actions = [
  mainActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceStep1Container))
