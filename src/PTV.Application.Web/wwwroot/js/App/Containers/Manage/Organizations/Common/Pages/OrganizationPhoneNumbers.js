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
import {connect} from 'react-redux';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

// actions
import * as commonActions from '../Actions';
import * as organizationActions from  '../../Organization/Actions';

// components
import PhoneNumbers from '../../../../Common/PhoneNumbers';

// selectors
import * as CommonOrganizationSelectors from '../Selectors';
import * as OrganizationSelectors from '../../Organization/Selectors';

export const OrganizationPhoneNumbers = ({messages, readOnly, language, translationMode, phoneNumbers, actions, organizationId, shouldValidate, children, collapsible, withType}) => {

    const onAddPhoneNumber = (entity) => {
        actions.onOrganizationEntityAdd('phoneNumbers', entity, organizationId);
    }
    
    const onRemovePhoneNumber = (id) => {
        actions.onOrganizationListChange('phoneNumbers', organizationId, id);
    }
  
    const sharedProps = { readOnly, language, translationMode };
    return(
        <PhoneNumbers {...sharedProps}
            messages = { messages }
            items = { phoneNumbers }
            shouldValidate = { shouldValidate }
            onAddPhoneNumber = { onAddPhoneNumber }
            onRemovePhoneNumber = { onRemovePhoneNumber }
            children = { children } 
            withType = { withType }   
            collapsible = { collapsible }        
        />
    )
}

OrganizationPhoneNumbers.propTypes = {
    shouldValidate: PropTypes.bool,
    collapsible: PropTypes.bool,
    withType: PropTypes.bool,
};

OrganizationPhoneNumbers.defaultProps = {
    shouldValidate: false,
    collapsible: false,
    withType: false
};

function mapStateToProps(state, ownProps) {

  return {
      phoneNumbers: CommonOrganizationSelectors.getOrganizationPhoneNumbers(state, ownProps),
      organizationId: CommonOrganizationSelectors.getOrganizationId(state, ownProps),
  }
}

const actions = [
    commonActions,
    organizationActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(OrganizationPhoneNumbers);
