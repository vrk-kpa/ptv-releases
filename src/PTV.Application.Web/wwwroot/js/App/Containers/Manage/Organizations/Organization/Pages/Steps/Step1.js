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
import Immutable, {Map} from 'immutable';
import {defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps';

// actions
import * as organizationActions from '../../Actions';

// components
import * as PTVValidatorTypes from '../../../../../../Components/PTVValidators';
import OrganizationGroupLevelContainer from '../../../Common/Pages/OrganizationGroupLevelContainer';
import OrganizationTypeMunicipalityContainer from '../../../Common/Pages/OrganizationTypeMunicipalityContainer';
import OrganizationEmails from '../../../Common/Pages/OrganizationEmails';
import OrganizationPhoneNumbers from '../../../Common/Pages/OrganizationPhoneNumbers';
import OrganizationWebPages from '../../../Common/Pages/OrganizationWebPages';
import OrganizationDescriptionContainer from '../../../Common/Pages/OrganizationDescriptionContainer';
import OrganizationAddresses from '../../../Common/Pages/OrganizationAddresses';
import PTVAddItem from '../../../../../../Components/PTVAddItem';
import LanguageLabel from '../../../../../Common/languageLabel';
import PublishingStatus from '../../../../../Common/PublishingStatus';

// selectors
import * as OrganizationSelectors from '../../Selectors';
import * as CommonSelectors from '../../../../../Common/Selectors';
import * as CommonOrganizationSelectors from '../../../Common/Selectors';

// messages
import * as Messages from '../../Messages';
import { translateMessages } from '../../../../../../ServerMessages/ServerMessages';

// types
import { addressTypes } from '../../../../../Common/Helpers/types';

const step1Messages = defineMessages({
    showContacts: {
        id: "Containers.Manage.Organizations.Manage.Step1.ShowContacts",
        defaultMessage: "Lisää yhteystiedot"
    },
    showPostalAddress: {
        id: "Containers.Manage.Organizations.Manage.Step1.showPostalAddress",
        defaultMessage: "Lisää postiosoite"
    },
    showVisitingAddress: {
        id: "Containers.Manage.Organizations.Manage.Step1.showVisitingAddress",
        defaultMessage: "Lisää käyntiosoite"
    },
    addPostalAddress: {
        id: "Containers.Manage.Organizations.Manage.Step1.addPostalAddress",
        defaultMessage: "Lisää uusi postiosoite"
    },
    addVisitingAddress: {
        id: "Containers.Manage.Organizations.Manage.Step1.addVisitingAddress",
        defaultMessage: "Lisää uusi käyntiosoite"
    },
});

export const OrganizationStep1Container = ({intl: {formatMessage}, readOnly, publishingStatus, keyToState, language, translationMode, splitContainer}) => {
    const sharedProps = { readOnly, translationMode, language, keyToState, splitContainer  };
    const validators = [PTVValidatorTypes.IS_REQUIRED];
    const validatorsBussiness =[PTVValidatorTypes.IS_BUSINESSID];

    const renderContacts = () => {

        return (
            <div>
                <OrganizationEmails {...sharedProps}
                    messages = { Messages.emailMessages }
                    collapsible
                />

                <OrganizationPhoneNumbers {...sharedProps}
                    messages= { Messages.phoneNumberMessages }
                    collapsible
                />

                <OrganizationWebPages {...sharedProps}
                    messages= { Messages.webPageMessages }
                    collapsible
                    withOrder
                    withName
                />

                <OrganizationAddresses {...sharedProps}
                    addressType= { addressTypes.POSTAL }
                />

                <OrganizationAddresses {...sharedProps}
                    addressType= { addressTypes.VISITING }
                />
            </div>
        );
    }

    return (
            <div>
            <LanguageLabel {...sharedProps}
                splitContainer = { splitContainer }
            />

            <PublishingStatus publishingStatus={ publishingStatus } pageType="organization"
            />
            <OrganizationGroupLevelContainer {...sharedProps}
                messages = { Messages.groupLevelContainerMessages }
            />

            <OrganizationTypeMunicipalityContainer {...sharedProps}
                messages = { Messages.typeMunicipalityContainerMessages }
                useFormatMessageData
            />

            <OrganizationDescriptionContainer {...sharedProps}
                messages = { Messages.messagesDescriptionContainer }
            />

            <PTVAddItem {...sharedProps}
                readOnly = { readOnly && translationMode == 'none' }
                collapsible = { !readOnly && translationMode !== "edit" }
                renderItemContent = { renderContacts }
                messages = {{ "label": formatMessage(step1Messages.showContacts) }}
            />
        </div>
    );
}

function mapStateToProps(state, ownProps) {
  return {
    publishingStatus: CommonOrganizationSelectors.getPublishingStatus(state, ownProps),
  }
}

const actions = [
    organizationActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationStep1Container));
