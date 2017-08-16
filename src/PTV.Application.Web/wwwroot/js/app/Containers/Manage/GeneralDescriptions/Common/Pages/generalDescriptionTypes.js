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

// components
import { LocalizedRadioGroup } from '../../../../Common/localizedData'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'

const GeneralDescriptionTypes = ({
    messages,
    readOnly,
    language,
    translationMode,
    typeId,
    serviceTypes,
    descriptionAttached,
    serviceTypeFromGeneralDescription,
    actions,
    intl }) => {
  const onInputChange = (input, isSet = false) => value => {
    actions.onGeneralDescriptionInputChange(input, value, isSet)
  }

  return (
    <div className='row form-group'>
      <div className='col-xs-12'>
        <LocalizedRadioGroup
          language={language}
          radioGroupLegend={intl.formatMessage(messages.title)}
          name='ServiceTypeGroup'
          value={typeId}
          tooltip={intl.formatMessage(messages.tooltip)}
          onChange={onInputChange('typeId')}
          items={serviceTypes}
          verticalLayout
          readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        />
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    typeId: GeneralDescriptionSelectors.getGeneralDescriptionServiceTypeId(state, ownProps),
    serviceTypes: CommonServiceSelectors.getServiceTypesObjectArray(state, ownProps)
  }
}

const actions = [
  generealDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(GeneralDescriptionTypes))

