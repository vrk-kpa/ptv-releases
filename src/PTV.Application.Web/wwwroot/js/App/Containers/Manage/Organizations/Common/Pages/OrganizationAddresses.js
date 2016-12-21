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
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Actions
import * as commonActions from '../Actions';
import * as organizationActions from  '../../Organization/Actions';

// components
import StreetAddress from '../../../../Common/Address';

// types
import { addressTypes } from '../../../../Common/Helpers/types';

// selectors
import * as CommonOrganizationSelectors from '../Selectors';
import * as OrganizationSelectors from '../../Organization/Selectors';

// messages
import { postalAddressMessagges, visitingAddressMessagges } from '../../../../Channels/Common/Messages';

const selectedAddressesSelector = {
    [addressTypes.POSTAL]: CommonOrganizationSelectors.getPostalAddresses, 
    [addressTypes.VISITING]: CommonOrganizationSelectors.getVisitingAddresses, 
}

const messages = {
    [addressTypes.POSTAL]: postalAddressMessagges, 
    [addressTypes.VISITING]: visitingAddressMessagges, 
}

export const OrganizationAddresses = ({readOnly, language, translationMode, addressType, addresses, keyToState, organizationId, actions}) => {

    const onAddAddress = (entity) => {
        actions.onLocalizedOrganizationEntityAdd(addressType, entity, organizationId, language);
    }

    const onRemoveAddress = (id) => {
        actions.onOrganizationListChange([addressType], organizationId, id);
    }

    const sharedProps = { readOnly, language, translationMode };

    return (
        <StreetAddress {...sharedProps}
            showTypeSelection= { addressType == addressTypes.POSTAL }
            addresses= { addresses }
            addressType = { addressType }
            shouldValidate = { addressType != addressTypes.DELIVERY }
            isAdditionalInformationVisible= { addressType != addressTypes.DELIVERY }
            onAddAddress= { onAddAddress }
            onRemoveAddress= { onRemoveAddress }
            coordinatesHidden= { addressType != addressTypes.VISITING }
        />
    )
}

OrganizationAddresses.propTypes = {
    addressType: PropTypes.string.isRequired,
    keyToState: PropTypes.string.isRequired,
    readOnly: PropTypes.bool.isRequired
};

function mapStateToProps(state, ownProps) {

  return {
      organizationId: CommonOrganizationSelectors.getOrganizationId(state, ownProps),
      addresses: selectedAddressesSelector[ownProps.addressType](state, ownProps),
  }
}

const actions = [
    commonActions,
    organizationActions
];


export default connect(mapStateToProps, mapDispatchToProps(actions))(OrganizationAddresses);