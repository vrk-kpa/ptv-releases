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

// components
import { IndustrialClassTree } from '../../../../Common/FintoTree'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'

const GeneralDescriptionIndustrialClasses = ({
    messages,
    readOnly,
    language,
    translationMode,
    actions,
    intl,
    keyToState,
    selectedCount,
    visibleIndustrialClasses }) => {
  const onListChange = (input) => (value, isAdd) => {
    actions.onListChange(input, value, isAdd)
  }
  const onListRemove = (input) => (value) => {
    actions.onListChange(input, value, false)
  }

  return (
    <div className='form-group'>
      {visibleIndustrialClasses &&
      <IndustrialClassTree
        treeViewClass='col-md-6'
        resultsClass='col-md-6'
        labelTooltip={intl.formatMessage(messages.tooltip)}
        label={intl.formatMessage(messages.title)}
        validatedField={messages.title}
        onNodeSelect={onListChange('industrialClasses')}
        onNodeRemove={onListRemove('industrialClasses')}
        treeTargetHeader={intl.formatMessage(messages.targetListHeader, { count: selectedCount })}
        order={60}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
        isSelectedSelector={GeneralDescriptionSelectors.getIsSelectedIndustrialClass}
        getSelectedSelector={GeneralDescriptionSelectors.getSelectedIndustrialClasses}
        language={language}
        keyToState={keyToState}
        />}
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  const industrialClasses = GeneralDescriptionSelectors.getSelectedIndustrialClasses(state, ownProps)
  const visibleIndustrialClasses = GeneralDescriptionSelectors.getIsSelectedTargetGroupKR2(state, ownProps)
  return {
    selectedCount: industrialClasses.size + '',
    visibleIndustrialClasses
  }
}

const actions = [
  generealDescriptionActions
]

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(GeneralDescriptionIndustrialClasses))

