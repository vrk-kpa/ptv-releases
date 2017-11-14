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
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'
import { EditorState, convertFromRaw } from 'draft-js'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// components
import Laws from '../../../../Common/Laws'
import { PTVTextEditorNotEmpty, PTVLabel } from '../../../../../Components'

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

export const GeneralDescriptionLaws = ({
  messages,
  readOnly,
  language,
  translationMode,
  laws,
  backgroundDescription,
  descriptionIsFilled,
  actions,
  intl
}) => {
  const onAddLaw = (entity) => {
    actions.onGeneralDescriptionLawAdd(entity, language)
  }

  const onRemoveLaw = (id) => {
    actions.onGeneralDescriptionLawRemove(id, language)
    actions.onGeneralDescriptionLawEntityRemove(id)
  }

  const onInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionTranslatedInputChange(input, value, language, isSet)
  }
  const validators = descriptionIsFilled ? [] : [{ ...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageDescriptionIsRequired }]
  const sharedProps = { readOnly, language, translationMode }
  const readOnlyMode = readOnly && translationMode === 'none'
  return (
    <div>
      <PTVLabel
        labelClass='strong'
        tooltip={intl.formatMessage(messages.backgroundAreaTooltip)}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}>
        {intl.formatMessage(messages.backgroundAreaTitle)}
      </PTVLabel>
      <div className={readOnlyMode ? '' : 'service-description'}>
        <div className='row form-group'>
          <PTVTextEditorNotEmpty
            componentClass='col-xs-12'
            maxLength={2500}
            name={'generalDescriptionBackgroundDescription'}
            label={intl.formatMessage(messages.backgroundDescriptionTitle)}
            placeholder={intl.formatMessage(messages.backgroundDescriptionPlaceholder)}
            value={backgroundDescription}
            blurCallback={onInputChange('backgroundDescription')}
            tooltip={intl.formatMessage(messages.backgroundDescriptionTooltip)}
            disabled={translationMode === 'view'}
            readOnly={readOnly && translationMode === 'none'}
            skipAsterisk
            validatedField={messages.backgroundDescriptionTitle}
            validators={validators}
          />
        </div>

        <Laws {...sharedProps}
          messages={messages}
          items={laws}
          onAddLaw={onAddLaw}
          onRemoveLaw={onRemoveLaw}
          collapsible={false}
          withName
          isHidden={readOnly && !laws.size}
        />
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const description = GeneralDescriptionSelectors.getGeneralDescriptionDescription(state, ownProps)
  const descriptionContent = !description
                            ? EditorState.createEmpty()
                            : EditorState.createWithContent(convertFromRaw(JSON.parse(description)))
  const descriptionIsFilled = descriptionContent.getCurrentContent().getPlainText('').length > 0
  return {
    laws: GeneralDescriptionSelectors.getGeneralDescriptionLaws(state, ownProps),
    backgroundDescription: GeneralDescriptionSelectors.getGeneralDescriptionBackgroundDescription(state, ownProps),
    descriptionIsFilled
  }
}

const actions = [
  generealDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(GeneralDescriptionLaws))
