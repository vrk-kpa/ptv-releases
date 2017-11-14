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
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'
import { EditorState, convertFromRaw } from 'draft-js'

// components
import { PTVTextAreaNotEmpty, PTVTextEditorNotEmpty } from '../../../../../Components'

// actions
import * as generalDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionSelector from '../../GeneralDescriptions/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

// helpers
import { getEditorPlainText } from '../../../../../Components/PTVTextEditor'

const ServiceDescriptions = ({
    messages, readOnly, language, translationMode,
    shortDescriptions,
    description,
    serviceUsage,
    userInstruction,    
    serviceTypeCode,
    bcgDescriptionIsFilled,
    actions, intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionTranslatedInputChange(input, value, language, isSet)
  }

  const serviceUsagePlaceholder = () => {
    switch (serviceTypeCode) {
      case 'Service': return intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder)
      case 'PermissionAndObligation': return intl.formatMessage(messages.conditionOfServiceUsagePermissionPlaceholder)
      default: return intl.formatMessage(messages.conditionOfServiceUsageServicePlaceholder)
    }
  }
  const validators = [PTVValidatorTypes.IS_REQUIRED]
  const translatableAreaRO = readOnly && translationMode === 'none'
  const descriptionValidators = bcgDescriptionIsFilled ? [] : [{ ...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageDescriptionIsRequired }]
  return (
    <div>
      <div className='row form-group'>
        <PTVTextAreaNotEmpty
          componentClass='col-xs-6'
          minRows={3}
          maxLength={150}
          label={intl.formatMessage(messages.shortDescriptionTitle)}
          placeholder={intl.formatMessage(messages.shortDescriptionPlaceholder)}
          tooltip={intl.formatMessage(messages.shortDescriptionTooltip)}
          value={shortDescriptions}
          name='generalDescriptionShortDescription'
          blurCallback={onInputChange('shortDescription')}
          order={20}
          disabled={translationMode === 'view'}
          readOnly={translatableAreaRO}
          validatedField={messages.shortDescriptionTitle}
          validators={validators}
          />
      </div>
      <div className='row form-group'>
        <PTVTextEditorNotEmpty
          componentClass='col-xs-12'
          maxLength={2500}
          name={'generalDescriptionDescription'}
          label={intl.formatMessage(messages.descriptionTitle)}
          placeholder={intl.formatMessage(messages.descriptionPlaceholder)}
          value={description}
          blurCallback={onInputChange('description')}
          tooltip={intl.formatMessage(messages.descriptionTooltip)}
          disabled={translationMode === 'view'}
          readOnly={translatableAreaRO}
          skipAsterisk
          validatedField={messages.descriptionTitle}
          validators={descriptionValidators}
          />
      </div>
      <div className='row form-group'>
        <PTVTextEditorNotEmpty
          componentClass='col-xs-12'
          maxLength={2500}
          name={'generalDescriptionconditionOfUsage'}
          label={intl.formatMessage(messages.conditionOfServiceUsageTitle)}
          placeholder={serviceUsagePlaceholder()}
          value={serviceUsage}
          blurCallback={onInputChange('serviceUsage')}
          tooltip={intl.formatMessage(messages.conditionOfServiceUsageTooltip)}
          disabled={translationMode === 'view'}
          readOnly={translatableAreaRO}
          validatedField={messages.conditionOfServiceUsageTitle}
          />
      </div>
      <div className='row form-group'>
        <PTVTextEditorNotEmpty
          componentClass='col-xs-12'
          maxLength={2500}
          name={'generalDescriptionconditionOfUsage'}
          label={intl.formatMessage(messages.serviceUserInstructionTitle)}
          placeholder={intl.formatMessage(messages.serviceUserInstructionPlaceholder)}
          value={userInstruction}
          blurCallback={onInputChange('userInstruction')}
          tooltip={intl.formatMessage(messages.serviceUserInstructionTooltip)}
          disabled={translationMode === 'view'}
          readOnly={translatableAreaRO}
          validatedField={messages.serviceUserInstructionTitle}
          />
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const bcgDescription = GeneralDescriptionSelector.getGeneralDescriptionBackgroundDescription(state, ownProps)
  const bcgDescriptionIsFilled = getEditorPlainText(bcgDescription).length > 0
  return {
    shortDescriptions: GeneralDescriptionSelector.getGeneralDescriptionShortDescription(state, ownProps),
    description: GeneralDescriptionSelector.getGeneralDescriptionDescription(state, ownProps),
    serviceUsage: GeneralDescriptionSelector.getGeneralDescriptionServiceUsage(state, ownProps),
    userInstruction: GeneralDescriptionSelector.getGeneralDescriptionUserInstruction(state, ownProps),
    serviceTypeCode:GeneralDescriptionSelector.getSelectedServiceTypeCode(state, ownProps),
    bcgDescriptionIsFilled
  }
}

const actions = [
  generalDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceDescriptions))

