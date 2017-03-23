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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { injectIntl } from 'react-intl';

// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import * as PTVValidatorTypes from '../../../../Components/PTVValidators';
import { YesNoRadio } from '../../../../Containers/Common/YesNoSelection';
import Municipalities from '../../../Common/municipalities';

// selectors
import * as CommonSelectors from '../Selectors';


const ChannelRestrictionRegion = ({ messages, isRestrictedRegion, readOnly, language, translationMode, actions, intl, channelId, keyToState }) => {

    const validators = [PTVValidatorTypes.IS_REQUIRED];

    const onInputChange = (input, isSet=false) => value => {
        actions.onChannelInputChange(input, channelId, value, isSet, language);
    }

    return (
        
        <div className="row form-group">
            <YesNoRadio
                name = 'AddLocationChannelRestrictedRegion'
                value = { isRestrictedRegion }
                radioGroupLegend = { intl.formatMessage(messages.restrictedRegionTitle) }
                tooltip = { intl.formatMessage(messages.restrictedRegionTooltip) }
                onChange = { onInputChange('isRestrictedRegion') }
                readOnly= { readOnly || translationMode == 'view' || translationMode == 'edit'}
            />
            { isRestrictedRegion ?
            <Municipalities
                componentClass= "col-sm-12 col-md-6"
                id= "serviceLocationMunicipalities"
                label= { messages.municipalitiesLabel }
                tooltip= { messages.municipalitiesTooltip }
                placeholder= { messages.municipalitiesPlaceholder }
                changeCallback= { onInputChange('municipalities', true) }
                validators= { validators }
                order= {200}
                language={ language }
                selector= { CommonSelectors.getSelectedMunicipalitiesItemsJS }
                keyToState= { keyToState }
                readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }/>        
            : null}
        </div>
    )
}

function mapStateToProps(state, ownProps) {

  return {
      isRestrictedRegion : CommonSelectors.getIsRestrictedRegion(state, ownProps),
      channelId: CommonSelectors.getChannelId(state, ownProps),     
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelRestrictionRegion));