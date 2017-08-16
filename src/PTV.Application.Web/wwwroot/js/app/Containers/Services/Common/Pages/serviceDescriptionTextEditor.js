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
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// components
import { PTVTextEditorNotEmpty, PTVTextAreaNotEmpty, PTVLabel } from '../../../../Components'

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// selectors
import * as ServiceSelectors from '../../Service/Selectors'

// messages
import { connectGeneralDescriptionMessages } from '../../Service/Messages';

// helpers
import { getEditorPlainText } from '../../../../Components/PTVTextEditor'

const ServiceDescriptionTextEditor = ({
    title, tooltip, placeHolder,
    readOnly, translationMode, maxLength, minRows,
    name, value, validatedField, valueFromGeneralDescription,
    descriptionAttached,
    serviceNameFromGeneralDescription,
    blurCallback,
    validators,
    intl }) => {

  const translatableAreaRO = readOnly && translationMode == 'none'
  const generalDescriptionRO = readOnly || translationMode == 'edit'
  const isValidDescriptionAttached = descriptionAttached && getEditorPlainText(valueFromGeneralDescription)

  return (
    <div>
      { isValidDescriptionAttached
        ? <PTVLabel
          labelClass='strong'
          tooltip={tooltip}
          readOnly={generalDescriptionRO}>
          { title }
        </PTVLabel>
      : null }
      <div className={translatableAreaRO ? '' : isValidDescriptionAttached ? 'service-description' : ''}>
        { isValidDescriptionAttached
          ? <div className='row form-group'>
            <PTVTextEditorNotEmpty
              componentClass='col-xs-12'
              name={name + 'FromGeneralDescription'}
              label={intl.formatMessage(connectGeneralDescriptionMessages.optionConnectedDescriptionTitle) + ': ' + serviceNameFromGeneralDescription}
              value={valueFromGeneralDescription}
              disabled={!generalDescriptionRO}
              readOnly={generalDescriptionRO}
              validatedField={validatedField}
            />
          </div>
          : null}

        <div className='row form-group'>
          <PTVTextEditorNotEmpty
            componentClass='col-xs-12'
            maxLength={maxLength}
            name={name}
            label={isValidDescriptionAttached ? translatableAreaRO ? '' : intl.formatMessage(connectGeneralDescriptionMessages.additionalDescription) : title}
            placeholder={placeHolder}
            value={value}
            blurCallback={blurCallback}
            validators={validators}
            tooltip={isValidDescriptionAttached ? null : tooltip}
            disabled={translationMode == 'view'}
            readOnly={translatableAreaRO}
            validatedField={validatedField}
            />
        </div>
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    descriptionAttached: ServiceSelectors.getIsGeneralDescriptionSelectedAndAttached(state, ownProps),
    serviceNameFromGeneralDescription: ServiceSelectors.getServiceNameFromGeneralDescriptionLocale(state, ownProps)
  }
}

const actions = [
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceDescriptionTextEditor))



