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
import { ServiceClassTree } from '../../../../Common/FintoTree'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'

// validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

const GeneralDescriptionServiceClasses = ({
    messages,
    readOnly,
    language,
    translationMode,
    actions,
    intl,
    keyToState,
    selectedCount }) => {
  const onListChange = (input) => (value, isAdd) => {
    actions.onListChange(input, value, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onListChange(input, value, false)
  }

  const validatorsServiceClasses = [{ ...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageIsRequired }]

  return (
    <div className='form-group'>
      <ServiceClassTree
        treeViewClass='col-md-6'
        resultsClass='col-md-6'
        labelTooltip={intl.formatMessage(messages.tooltip)}
        label={intl.formatMessage(messages.title)}
        onNodeSelect={onListChange('serviceClasses')}
        onNodeRemove={onListRemove('serviceClasses')}
        treeTargetHeader={intl.formatMessage(messages.targetListHeader, { count: selectedCount })}
        order={60}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        isSelectedSelector={GeneralDescriptionSelectors.getIsSelectedServiceClass}
        getSelectedSelector={GeneralDescriptionSelectors.getSelectedServiceClasses}
        language={language}
        keyToState={keyToState}
        validatedField={messages.title}
        validators={validatorsServiceClasses}
        />
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const serviceClasses = GeneralDescriptionSelectors.getSelectedServiceClasses(state, ownProps)

  return {
    selectedCount: serviceClasses.size + ''
  }
}

const actions = [
  generealDescriptionActions
]

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(GeneralDescriptionServiceClasses))

