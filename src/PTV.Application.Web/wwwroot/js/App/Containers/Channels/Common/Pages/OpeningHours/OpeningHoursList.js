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
import { injectIntl } from 'react-intl'
import { connect } from 'react-redux'
import shortId from 'shortid'

// actions
import * as channelActions from '../../Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// selectors
import * as CommonSelectors from '../../../Common/Selectors'

// components
import OpeningHours from './OpeningHours'
import OpeningHoursSpecial from './OpeningHoursSpecial'
import OpeningHoursExceptional from './OpeningHoursExceptional'
import { PTVAddItem } from '../../../../../Components'
import { composePTVComponent, ValidatePTVComponent } from '../../../../../Components/PTVComponent'

// enums
import { openingHoursTypes } from '../../../../Common/Enums'

const openingHoursOptions = {
  [openingHoursTypes.special]:{
    typeName: 'Special',
    property:'specialHours',
        // expanded: false,
    valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursSpecial,
    valueValidSelector: CommonSelectors.getIsEveryOpeningHoursSpecialValid,
    selector: CommonSelectors.getOpeningHoursSpecial
  },
  [openingHoursTypes.exceptional]:{
    typeName: 'Exceptional',
    property: 'exceptionalHours',
        // expanded: false,
    valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursExceptional,
    valueValidSelector: CommonSelectors.getIsEveryOpeningHoursExceptionalValid,
    selector: CommonSelectors.getOpeningHoursExceptional
  },
  [openingHoursTypes.normal]:{
    typeName: 'Normal',
    tooltip: 'mainTooltipNormal',
        // expanded: true,
    valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursNormal,
    valueValidSelector: CommonSelectors.getIsEveryOpeningHoursNormalValid,
    selector: CommonSelectors.getOpeningHoursNormal
  }
}

const OpeningHoursValidation = composePTVComponent(({ isInvalidInfo, hiddenElement, ...props }) => (
  <div style={isInvalidInfo ? hiddenElement : null}>
    <ValidatePTVComponent
      {...props}
      valueToValidate={isInvalidInfo ? '' : 'valid'}
    />
  </div>
))

const getTypeProperty = (type) => 'openingHours' + openingHoursOptions[type].typeName

export const OpeningHoursList = props => {
  const {
    messages,
    keyToState,
    readOnly,
    intl,
    openingHours,
    language,
    translationMode,
    actions,
    channelId,
    type,
    isValueEntered,
    isValueValid,
    activeHours,
    showList
  } = props
  const onRemoveOpeningHours = (id) => {
    actions.onChannelListChange(getTypeProperty(type), channelId, id, language)
  }

  const onAddOpeningHours = (entity) => {
    actions.onChannelEntityAdd(getTypeProperty(type), entity, channelId, language)
  }

  const onAddButtonClick = (param) => {
    onAddOpeningHours(openingHours.size === 0
    ? [{ id: shortId.generate() }, { id: shortId.generate() }]
    : [{ id: shortId.generate() }])
  }

  const onShowAddItem = () => {
    actions.onChannelEntityAdd('activeHours', type, channelId, language)
  }

  const onHideAddItem = () => {
    actions.onChannelEntityAdd('activeHours', -1, channelId, language)
  }

  const renderOpeningHours = (type) => (id, index, isNew) => {
    const ohProps = {
      key: id,
      id: id,
      isNew: isNew,
      openingHoursMessages: messages,
      readOnly: readOnly,
      onAddOpeningHours: onAddOpeningHours,
      openingHoursType: type,
      keyToState: keyToState,
      previewClass: !readOnly ? 'col-md-4' : 'col-xs-12'
    }

    switch (type) {
      case openingHoursTypes.special:
        return (
          <OpeningHoursSpecial {...ohProps} />
        )
      case openingHoursTypes.exceptional:
        return (
          <OpeningHoursExceptional {...ohProps} />
        )
      default:
        return (
          <OpeningHours {...ohProps} />
        )
    }
  }
  const hiddenElement = { 'display': 'none' }
  const isInvalidInfo = isValueEntered && !isValueValid
  return (
    <div className='opening-hours'>
      <PTVAddItem
        items={openingHours}
        readOnly={readOnly}
        renderItemContent={renderOpeningHours(type)}
        messages={{
          'tooltip': intl.formatMessage(messages['mainTooltip' + openingHoursOptions[type].typeName]),
          'label': intl.formatMessage(messages['mainLabel' + openingHoursOptions[type].typeName]),
          'addBtnLabel': intl.formatMessage(messages.addBtnLabel),
          'collapsedInfo': isValueEntered ? intl.formatMessage(messages['collapsedInfo']) : '',
          'invalidInfo': isInvalidInfo ? intl.formatMessage(messages['invalidInfo']) : ''
        }}
        onAddButtonClick={onAddButtonClick}
        onShowAddItem={onShowAddItem}
        onHideAddItem={onHideAddItem}
        onRemoveItemClick={onRemoveOpeningHours}
        collapsible
        showList={showList}
      />
      <OpeningHoursValidation {...props} isInvalidInfo={isInvalidInfo} hiddenElement={hiddenElement} />
    </div>
  )
}

OpeningHoursList.propTypes = {
  showList: PropTypes.bool
}

const mapStateToProps = (type) => (state, ownProps) => {
  const isValueEntered = openingHoursOptions[type].valueEnteredSelector(state, ownProps)
  const isValueValid = openingHoursOptions[type].valueValidSelector(state, ownProps)
  return {
    type,
    openingHours: openingHoursOptions[type].selector(state, ownProps),
    channelId: CommonSelectors.getChannelId(state, ownProps),
    activeHours: CommonSelectors.getActiveOpeningHours(state, ownProps),
    isValueValid,
    isValueEntered
  }
}
const actions = [
  channelActions
]

const connectTreeComponent = type => {
  return connect(mapStateToProps(type), mapDispatchToProps(actions))(injectIntl(OpeningHoursList))
}

export const OpeningHoursNormalList = connectTreeComponent(openingHoursTypes.normal)
export const OpeningHoursExceptionalList = connectTreeComponent(openingHoursTypes.exceptional)
export const OpeningHoursSpecialList = connectTreeComponent(openingHoursTypes.special)

export default {
  OpeningHoursNormal: OpeningHoursNormalList,
  OpeningHoursExceptional: OpeningHoursExceptionalList,
  OpeningHoursSpecial: OpeningHoursSpecialList
}
