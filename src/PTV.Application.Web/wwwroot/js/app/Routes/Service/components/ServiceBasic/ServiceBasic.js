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
import { connect } from 'react-redux'
import GeneralDescriptionSearchForm from '../GeneralDescriptionSearchForm'
import ServiceCollections from '../ServiceCollections'
import {
  ServiceServiceType,
  ServiceServiceChargeType,
  ServiceDescription,
  ServiceConditionsOfUsage,
  ServiceUserInstruction,
  ServiceDeadlineAdditionalInfo,
  ServiceProcessingTimeAdditionalInfo,
  ServiceValidityTimeAdditionalInfo,
  ServiceLaws
} from '../ServiceComponents'
import {
  serviceNamesMessages,
  serviceDescriptionMessages,
  typeProfessionalAdditionalInfoMessages
} from '../Messages'
import SelfProducersWarningField from '../SelfProducersWarningField'
import {
  FundingType,
  NameEditor,
  AlternateName,
  Organizations,
  Summary,
  ServiceOrganization,
  Languages
} from 'util/redux-form/fields'
import {
  ServiceVoucher,
  AreaInformationExpanded
} from 'util/redux-form/sections'
import { injectIntl, intlShape } from 'util/react-intl'
import { Label } from 'sema-ui-components'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import {
  getIsGeneralDescriptionAttached,
  getServiceTypeCodeSelected
} from 'Routes/Service/selectors'
import { messages } from 'Routes/messages'

const ServiceBasic = ({
  intl: { formatMessage },
  isGDAttached,
  isCompareMode,
  serviceTypeCode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='form-row'>
        <GeneralDescriptionSearchForm {...rest} />
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ServiceServiceType />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <FundingType placeholder={formatMessage(messages.organizationPlaceholder)} required />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <NameEditor
              label={formatMessage(serviceNamesMessages.nameTitle)}
              placeholder={formatMessage(serviceNamesMessages.namePlaceholder)}
              tooltip={formatMessage(serviceNamesMessages.nameTooltip)}
              required
              useQualityAgent
            />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <AlternateName label={formatMessage(serviceNamesMessages.alternateNameTitle)}
              placeholder={formatMessage(serviceNamesMessages.alternateNamePlaceholder)}
              tooltip={formatMessage(serviceNamesMessages.alternateNameTooltip)}
              useQualityAgent
            />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ServiceOrganization
              label={formatMessage(serviceNamesMessages.mainOrganisationTitle)}
              placeholder={formatMessage(messages.organizationPlaceholder)}
              tooltip={formatMessage(messages.organizationTooltip)}
              required
              {...rest}
            />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Organizations
              name='responsibleOrganizations'
              label={formatMessage(serviceNamesMessages.responsibleOrganisationTitle)}
              placeholder={formatMessage(messages.organizationPlaceholder)}
              tooltip={formatMessage(serviceNamesMessages.responsibleOrganisationTooltip)}
            />
            <div className='error'>
              <SelfProducersWarningField />
            </div>
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Summary label={formatMessage(serviceDescriptionMessages.shortDescriptionTitle)}
              placeholder={formatMessage(serviceDescriptionMessages.shortDescriptionPlaceholder)}
              tooltip={formatMessage(serviceDescriptionMessages.shortDescriptionTooltip)}
              required
              useQualityAgent
              qualityAgentCompare
            />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <ServiceDescription required />
      </div>

      <div className='form-row'>
        <ServiceConditionsOfUsage />
      </div>

      <div className='form-row'>
        <ServiceUserInstruction />
      </div>

      <div className='form-row'>
        <ServiceServiceChargeType />
      </div>

      <div className='form-row'>
        <ServiceVoucher />
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Languages required />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <AreaInformationExpanded />
      </div>
      <div className='form-row'>
        <ServiceCollections />
      </div>

      {
        serviceTypeCode !== 'service' &&
        <div>
          {serviceTypeCode === 'qualification' &&
            <Label
              labelText={formatMessage(typeProfessionalAdditionalInfoMessages.qualificationTitle)}
              labelPosition='top'
            />
          }

          {serviceTypeCode === 'obligation' &&
            <Label
              labelText={formatMessage(typeProfessionalAdditionalInfoMessages.obligationTitle)}
              labelPosition='top'
            />
          }

          <div className='form-row'>
            <ServiceDeadlineAdditionalInfo />
          </div>

          <div className='form-row'>
            <ServiceProcessingTimeAdditionalInfo />
          </div>

          <div className='form-row'>
            <ServiceValidityTimeAdditionalInfo />
          </div>

        </div>
      }

      <ServiceLaws />
    </div>
  )
}

ServiceBasic.propTypes = {
  intl: intlShape.isRequired,
  isGDAttached: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool,
  serviceTypeCode: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    serviceTypeCode: getServiceTypeCodeSelected(state, ownProps),
    isGDAttached: getIsGeneralDescriptionAttached(state, ownProps)
  }))
)(ServiceBasic)
