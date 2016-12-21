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
import React, {PropTypes} from 'react';
import { injectIntl } from 'react-intl';
import {connect} from 'react-redux';
import shortId from 'shortid';

//actions
import * as channelActions from '../../Actions';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

// selectors
import * as CommonSelectors from '../../../Common/Selectors';

// components
import OpeningHours from './OpeningHours';
import OpeningHoursSpecial from './OpeningHoursSpecial';
import OpeningHoursExceptional from './OpeningHoursExceptional';
import { PTVAddItem } from '../../../../../Components';

// enums
import { openingHoursTypes } from '../../../../Common/Enums';

const openingHoursOptions = {
    [openingHoursTypes.special]:{
        typeName: "Special",
        property:"specialHours",
        // expanded: false,
        valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursSpecial, 
        selector: CommonSelectors.getOpeningHoursSpecial
},
    [openingHoursTypes.exceptional]:{
        typeName: "Exceptional",
        property: "exceptionalHours",
        // expanded: false,
        valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursExceptional,
        selector: CommonSelectors.getOpeningHoursExceptional
    },
    [openingHoursTypes.normal]:{
        typeName: "Normal",
        tooltip: "mainTooltipNormal",
        // expanded: true,
        valueEnteredSelector: CommonSelectors.getIsAnyOpeningHoursNormal,
        selector: CommonSelectors.getOpeningHoursNormal
    }
};

const getTypeProperty = (type) => 'openingHours'+openingHoursOptions[type].typeName;

export const OpeningHoursList = ({ messages, readOnly, intl, openingHours, language, translationMode, actions, channelId, type, valueEntered, activeHours, showList }) => {
    const onRemoveOpeningHours = (id) => {
        actions.onChannelListChange(getTypeProperty(type), channelId, id, language);
    }

    const onAddOpeningHours = (entity) => {
        actions.onChannelEntityAdd(getTypeProperty(type), entity, channelId, language);
    }
    
    const onAddButtonClick = (param) => {
        onAddOpeningHours( openingHours.size === 0 ? [{ id: shortId.generate() }, { id: shortId.generate() }]:[{ id: shortId.generate() }]);
    }

    const onShowAddItem = () => {
        actions.onChannelEntityAdd('activeHours', type, channelId, language);
    }

    const onHideAddItem = () => {
        actions.onChannelEntityAdd('activeHours', -1, channelId, language);
    }

    const renderOpeningHours = (type) => (id, index, isNew) => {
        const ohProps = {
            key: id,
            id: id,        
            isNew: isNew,
            isFirst: index === 0,
            openingHoursMessages: messages,
            readOnly: readOnly,
            onAddOpeningHours: onAddOpeningHours,
            openingHoursType: type,
            previewClass: !readOnly ? 'col-md-4' : 'col-xs-12'
        };

        switch (type) {
            case openingHoursTypes.special:
                return (
                    <OpeningHoursSpecial {...ohProps } />
	            );
            case openingHoursTypes.exceptional:
                return (
                    <OpeningHoursExceptional {...ohProps} />
	            );
            default:
                return (
                    <OpeningHours {...ohProps} />
	            );
        }
    }

    return (
        <div className="opening-hours">
            <PTVAddItem
                items = { openingHours }
                readOnly = { readOnly }
                renderItemContent = { renderOpeningHours(type) }
                messages = {{ 
                    "tooltip": intl.formatMessage(messages["mainTooltip"+openingHoursOptions[type].typeName]),
                    "label": intl.formatMessage(messages["mainLabel"+openingHoursOptions[type].typeName]),
                    "addBtnLabel": intl.formatMessage(messages.addBtnLabel),
                    "collapsedInfo": valueEntered ? intl.formatMessage(messages["collapsedInfo"]) : ''
                }}
                onAddButtonClick = { onAddButtonClick }
                onShowAddItem = { onShowAddItem }
                onHideAddItem = { onHideAddItem }
                onRemoveItemClick = { onRemoveOpeningHours }
                collapsible
                //showOnLoad = { openingHoursOptions[type].expanded }
                showList = { showList }
                //showList = { showList }
            />
        </div>            
    )
}

OpeningHoursList.propTypes = {
    // type: PropTypes.string
    showList: PropTypes.bool
}

const mapStateToProps = (type) => (state, ownProps) => {

  return {
      type,
      openingHours: openingHoursOptions[type].selector(state, ownProps),
      valueEntered: openingHoursOptions[type].valueEnteredSelector(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),
      activeHours: CommonSelectors.getActiveOpeningHours(state, ownProps),
  }
}
const actions = [
    channelActions
];

const connectTreeComponent = type => {
    return connect(mapStateToProps(type), mapDispatchToProps(actions))(injectIntl(OpeningHoursList));
}

export const OpeningHoursNormalList = connectTreeComponent(openingHoursTypes.normal); 
export const OpeningHoursExceptionalList = connectTreeComponent(openingHoursTypes.exceptional); 
export const OpeningHoursSpecialList = connectTreeComponent(openingHoursTypes.special); 

export default {
    OpeningHoursNormal: OpeningHoursNormalList,
    OpeningHoursExceptional: OpeningHoursExceptionalList,
    OpeningHoursSpecial: OpeningHoursSpecialList
}
