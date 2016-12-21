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

// actions
import * as channelActions from '../../Common/Actions';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import StreetAddress from '../../../Common/Address';
import ChannelFormReceiver from './channelFormReceiver';
import { PTVTextArea } from '../../../../Components';

// types
import { addressTypes } from '../../../Common/Helpers/types';

// selectors
import * as CommonSelectors from '../Selectors';

const selectedAddressesSelector = {
    [addressTypes.DELIVERY]: CommonSelectors.getDeliveryAdresses, 
    [addressTypes.POSTAL]: CommonSelectors.getPostalAdresses, 
    [addressTypes.VISITING]: CommonSelectors.getVisitingAdresses, 
}

export const ChannelAddresses = ({readOnly, addressType, addresses, keyToState, language, translationMode, splitContainer, channelId, actions}) => {
    const sharedProps = { readOnly, language, translationMode, splitContainer };

    const getFormReceiverComponent = () => {
        return(
            <ChannelFormReceiver {...sharedProps}
                keyToState= { keyToState }
            /> 
        )
    }

    const onAddAddress = (entity) => {
        actions.onChannelEntityAdd(addressType, addressType == addressTypes.DELIVERY ? entity[0] : entity, channelId, language);
    }

    const onRemoveAddress = (id) => {
        actions.onChannelListChange([addressType], channelId, id, language);
    }

    return (
        <StreetAddress {...sharedProps}
            showTypeSelection= { addressType != addressTypes.VISITING }
            addresses= { addresses }
            type= { addressType == undefined }
            addressType= { addressType }
            collapsible= { addressType != addressTypes.VISITING }
            shouldValidate = { addressType != addressTypes.DELIVERY }
            isAdditionalInformationVisible= { addressType != addressTypes.DELIVERY }
            isAdditionalInformationTextAreaVisible= { addressType == addressTypes.DELIVERY }
            receiver= { addressType != addressTypes.DELIVERY ? null : getFormReceiverComponent() }
            onAddAddress= { onAddAddress }
            onRemoveAddress= { onRemoveAddress }
            coordinatesHidden= { addressType != addressTypes.VISITING }
        />
    )
}

ChannelAddresses.propTypes = {
    addressType: PropTypes.string.isRequired,
    keyToState: PropTypes.string.isRequired,
    readOnly: PropTypes.bool.isRequired,

};

function mapStateToProps(state, ownProps) {

  return {
      channelId: CommonSelectors.getChannelId(state, ownProps),
      addresses: selectedAddressesSelector[ownProps.addressType](state, ownProps),
  }
}

const actions = [
    channelActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(ChannelAddresses);