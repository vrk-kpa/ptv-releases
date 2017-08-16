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
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
import ChargeType from '../../../Common/chargeTypeCombo'
import { PTVTextAreaNotEmpty, PTVLabel } from '../../../../Components'

// actions
import * as serviceActions from '../../Service/Actions'
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ServiceSelectors from '../../Service/Selectors'

import { connectGeneralDescriptionMessages } from '../../Service/Messages'

const ServiceChargeTypes = ({
  messages,
  readOnly,
  language,
  translationMode,
  additionalInformation,
  actions,
  intl,
  serviceNameFromGeneralDescription,
  chargeTypeInfoFromGeneralDescription,
  descriptionAttached,
  isChargeTypeSelected,
  chargeTypeGeneralDescription
}) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onServiceInputChange(input, value, language, isSet)
  }

  const generalDescriptionRO = readOnly || translationMode === 'edit'
  const readOnlyMode = readOnly && translationMode === 'none'
  const isChargeTypeDisabled = descriptionAttached && isChargeTypeSelected

  return (
    <div>
      <PTVLabel
        labelClass='strong'
        tooltip={intl.formatMessage(messages.tooltip)}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}>
        { intl.formatMessage(messages.title) }
      </PTVLabel>
      <div className={readOnlyMode ? '' : 'service-description'}>
        <div className='row form-group'>
          <ChargeType
            componentClass='col-xs-6'
            id='chargeTypes'
            label={!readOnlyMode ? messages.chargeTypeTitle : ''}
            changeCallback={onInputChange('chargeType', true)}
            order={35}
            language={language}
            filterCode='Other'
            selector={
              isChargeTypeDisabled
              ? ServiceSelectors.getChargeTypeGeneralDescription
              : ServiceSelectors.getChargeType
            }
            readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
            disabled={isChargeTypeDisabled} />
        </div>
        { descriptionAttached && chargeTypeInfoFromGeneralDescription &&
          <div className='row form-group'>
            <PTVTextAreaNotEmpty
              componentClass='col-xs-12'
              minRows={4}
              name={'additionalInformationFromGeneralDescription'}
              label={intl.formatMessage(connectGeneralDescriptionMessages.optionConnectedDescriptionTitle) + ': ' + serviceNameFromGeneralDescription}
              value={chargeTypeInfoFromGeneralDescription}
              disabled={!generalDescriptionRO}
              readOnly={generalDescriptionRO} />
          </div>
                     }
        <div className='row form-group'>
          <PTVTextAreaNotEmpty
            componentClass='col-xs-12'
            minRows={4}
            maxLength={500}
            label={!readOnlyMode ? intl.formatMessage(messages.additionalInfoTitle) : ''}
            placeholder={intl.formatMessage(messages.additionalInfoPlaceholder)}
            value={additionalInformation}
            blurCallback={onInputChange('additionalInformation')}
            order={36}
            name='additionalInformation'
            disabled={translationMode === 'view'}
            readOnly={readOnlyMode} />
        </div>
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    additionalInformation: ServiceSelectors.getAdditionalInformation(state, ownProps),
    serviceNameFromGeneralDescription: ServiceSelectors.getServiceNameFromGeneralDescriptionLocale(state, ownProps),
    chargeTypeInfoFromGeneralDescription: ServiceSelectors.getChargeTypeInfoFromGeneralDescriptionLocale(state, ownProps),
    descriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelectedAndAttached(state, ownProps),
    isChargeTypeSelected: ServiceSelectors.getChargeTypeGeneralDescription(state, ownProps)
  }
}

const actions = [
  serviceActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceChargeTypes))

