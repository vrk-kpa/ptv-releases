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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Actions
import * as channelSearchActions from '../../ChannelSearch/Actions';

// components
import ServiceAndChannelTag from './ServiceAndChannelTag'


// selectors
import * as ChannelSearchSelectors from '../../ChannelSearch/Selectors';
import * as CommonOrganisationSelectors from '../../../../Manage/Organizations/Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors'
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors'

// messages
import { publishingStatusMessages } from '../../../../Common/PublishStatusContainer'

// types
import { publishingStatuses } from '../../../../Common/Enums'

export const ChannelSearchCriteriaTags =  ({ languageId, statusPublishedId, keyToState, organizationId, channelTypeId, intl, actions }) => {

    const { formatMessage } = intl;

    const listTagRemove = (input) => value => {
        actions.onChannelsSearchListChange(input, value, false);
    }

    const inputTagRemove = (input) => value => {
        actions.onChannelSearchInputChange(input, null, false);
    }

    return (
            <div className="clearfix">
                <ul>
                    <ServiceAndChannelTag
                        id= { languageId }
                        onTagRemove= { inputTagRemove('languageId') }
                        isSelected= { languageId }
                        readOnly
                        localized
                    />
                    <ServiceAndChannelTag
                        id= { organizationId }
                        getTextSelector= { CommonOrganisationSelectors.getOrganizationNameForId }
                        onTagRemove= { inputTagRemove('organizationId') }
                        isSelected= { organizationId }
                        localized
                    />
                    <ServiceAndChannelTag
                        id= { channelTypeId }
                        getTextSelector= { CommonSelectors.geChannelTypeNameForId }
                        onTagRemove= { inputTagRemove('channelTypeId') }
                        isSelected= { channelTypeId }
                        localized
                    />
                </ul>
            </div>
       );
}

ChannelSearchCriteriaTags.propTypes = {

};

function mapStateToProps(state, ownProps) {
    const statusPublishedId =CommonSelectors.getPublishingStatusId(state, publishingStatuses.published);
    const languageId = CommonServiceAndChannelSelectors.getLanguageToForServiceAndChannel(state);
    const organizationId= ChannelSearchSelectors.getOrganizationId(state);
    const channelTypeId= ChannelSearchSelectors.getChannelTypeId(state);
  return {
     statusPublishedId,
     organizationId,
     channelTypeId,
     languageId
  }
}

const actions = [
    channelSearchActions,
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelSearchCriteriaTags));
