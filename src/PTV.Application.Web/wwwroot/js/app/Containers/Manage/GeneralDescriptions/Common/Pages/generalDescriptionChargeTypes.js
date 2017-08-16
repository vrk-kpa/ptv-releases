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
import ChargeType from '../../../../Common/chargeTypeCombo'
import { PTVTextAreaNotEmpty, PTVLabel } from '../../../../../Components'

// actions
import * as generalDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionsSelectors from '../../GeneralDescriptions/Selectors'

const GeneralDescriptionChargeTypes = ({
  messages,
  readOnly,
  language,
  translationMode,
  additionalInformation,
  actions,
  intl
}) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionInputChange(input, value, isSet)
  }
  const onTranslatedInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionTranslatedInputChange(input, value, language, isSet)
  }

  const readOnlyMode = readOnly && translationMode === 'none'

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
            changeCallback={onInputChange('chargeTypeId', true)}
            order={35}
            language={language}
            filterCode='Other'
            selector={GeneralDescriptionsSelectors.getGeneralDescriptionChargeTypeId}
            readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
          />
        </div>
        <div className='row form-group'>
          <PTVTextAreaNotEmpty
            componentClass='col-xs-12'
            minRows={4}
            maxLength={500}
            label={!readOnlyMode ? intl.formatMessage(messages.additionalInfoTitle) : ''}
            placeholder={intl.formatMessage(messages.additionalInfoPlaceholder)}
            value={additionalInformation}
            blurCallback={onTranslatedInputChange('additionalInformation')}
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
    additionalInformation: GeneralDescriptionsSelectors.getGeneralDescriptionAdditionalInformation(state, ownProps)
  }
}

const actions = [
  generalDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(GeneralDescriptionChargeTypes))

