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
import { connect } from 'react-redux'
import {
  generalDescriptionNamesMessages,
  generalDescriptionDescriptionMessages
} from '../Messages'
import BackgroundInformation from '../BackgroundInformation'
import ServiceTypeAdditionalInformation from '../ServiceTypeAdditionalInformation'
import {
  ServiceType,
  Name,
  Description,
  ConditionOfServiceUsage,
  UserInstruction,
  ShortDescription
} from 'util/redux-form/fields'
import {
  ServiceChargeType
} from 'util/redux-form/sections'
import { injectIntl } from 'react-intl'
import { injectFormName, withFormStates } from 'util/redux-form/HOC'
import {
  getIsOtherServiceTypeThanService,
  getIsAnyDescriptionProvided
} from './selectors'

const GeneralDescriptionBasic = (
  {
    intl: { formatMessage },
    isOtherThanService,
    isCompareMode,
    isAnyDescriptionProvided
  }
) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'

  return (
    <div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ServiceType radio />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Name label={formatMessage(generalDescriptionNamesMessages.nameTitle)}
              placeholder={formatMessage(generalDescriptionNamesMessages.namePlaceholder)}
              tooltip={formatMessage(generalDescriptionNamesMessages.nameTooltip)}
              required />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ShortDescription label={formatMessage(generalDescriptionDescriptionMessages.shortDescriptionTitle)}
              placeholder={formatMessage(generalDescriptionDescriptionMessages.shortDescriptionPlaceholder)}
              tooltip={formatMessage(generalDescriptionDescriptionMessages.shortDescriptionTooltip)}
              multiline
              rows={3}
              counter
              maxLength={150}
              required />
          </div>
        </div>
      </div>

      <div className='form-row'>
        <Description
          label={formatMessage(generalDescriptionDescriptionMessages.descriptionTitle)}
          placeholder={formatMessage(generalDescriptionDescriptionMessages.descriptionPlaceholder)}
          tooltip={formatMessage(generalDescriptionDescriptionMessages.descriptionTooltip)}
          required={!isAnyDescriptionProvided}
          />
      </div>

      <div className='form-row'>
        <ConditionOfServiceUsage
          label={formatMessage(generalDescriptionDescriptionMessages.conditionOfServiceUsageTitle)}
          />
      </div>

      <div className='form-row'>
        <UserInstruction
          label={formatMessage(generalDescriptionDescriptionMessages.serviceUserInstructionTitle)}
          />
      </div>

      <div className='form-row'>
        <ServiceChargeType />
      </div>

      {isOtherThanService &&
        <div className='form-row'>
          <ServiceTypeAdditionalInformation />
        </div>
      }

      <div className='form-row'>
        <BackgroundInformation required={!isAnyDescriptionProvided} />
      </div>

    </div>
  )
}

GeneralDescriptionBasic.propTypes = {
  intl: PropTypes.object.isRequired,
  isOtherThanService: PropTypes.bool.isRequired,
  isCompareMode: PropTypes.bool,
  isAnyDescriptionProvided: PropTypes.bool
}

export default compose(
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    isOtherThanService: getIsOtherServiceTypeThanService(state, ownProps),
    isAnyDescriptionProvided: getIsAnyDescriptionProvided(state, ownProps)
  })),
  injectIntl)(GeneralDescriptionBasic)
