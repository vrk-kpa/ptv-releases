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

// selectors
import * as GeneralDescriptionSelectors from '../../GeneralDescriptions/Selectors'
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors'

// components
import { PTVGroup } from '../../../../../Components'
import { LocalizedTwoLevelCheckBoxList } from '../../../../Common/localizedData'

// actions
import * as generealDescriptionActions from '../../GeneralDescriptions/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// Validators
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators'

const GeneralDescriptionTargetGroups = ({
    intl,
    readOnly,
    translationMode,
    topTargetGroups,
    targetGroups,
    actions,
    isTargetGroupSelected,
    generalTargetGroups,
    language,
    messages }) => {
  const readOnlyTG = readOnly || translationMode === 'view' || translationMode === 'edit'
  const onTargetGroupClick = (input) => (id, isAdd) => {
    actions.onListChange(input, id, isAdd)
  }
  const validatorsTargetGroup = [{ ...PTVValidatorTypes.IS_REQUIRED, errorMessage: messages.errorMessageIsRequired }]
  
  const renderTargetGroups = (targetGroups, readOnly) => {
    return (
            topTargetGroups.map(tg => {
              return <LocalizedTwoLevelCheckBoxList
                componentClass='checkbox-list'
                key={tg}
                id={tg}
                data={targetGroups}
                onClick={onTargetGroupClick('targetGroups')}
                isSelectedSelector={GeneralDescriptionSelectors.getIsSelectedTargetGroup}
                subGroupTitle={intl.formatMessage(messages.subTargetGroupTitle)}
                readOnly={readOnlyTG}
                language={language}
                link={intl.formatMessage(messages.subTargetLink)}
                />
            }
        )
    )
  }

  return (<PTVGroup
    order={50}
    contentClassName='col-xs-12'
    labelClassName='col-xs-12'
    labelTooltip={intl.formatMessage(messages.targetGroupTooltip)}
    labelContent={intl.formatMessage(messages.targetGroupTitle)}
    readOnly={readOnlyTG}
    isAnySelected={isTargetGroupSelected}
    validatedField={messages.targetGroupTitle}
    validators={validatorsTargetGroup}>
    {renderTargetGroups(targetGroups, readOnlyTG)}
  </PTVGroup>)
}

function mapStateToProps (state, ownProps) {
  const isTargetGroupSelected = GeneralDescriptionSelectors.getIsAnyTargetGroupsSelected(state, ownProps)
  return {
    isTargetGroupSelected: isTargetGroupSelected,
    targetGroups: CommonServiceSelectors.getTargetGroups(state, ownProps),
    topTargetGroups: CommonServiceSelectors.getTopTargetGroups(state, ownProps)
  }
}

const actions = [
  generealDescriptionActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(GeneralDescriptionTargetGroups))
