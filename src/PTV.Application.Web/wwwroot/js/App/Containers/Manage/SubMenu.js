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
import PTVButton from '../../Components/PTVButton';
import PTVAutoComboBox from '../../Components/PTVAutoComboBox';
import { defineMessages, injectIntl } from 'react-intl'
import { Link, browserHistory } from 'react-router';

import './Styles/SubMenu.scss';

const messages = defineMessages({
    buttonOrganizationTitle: {
        id: "Containers.Manage.SubMenu.Button.Organization.Title",
        defaultMessage: "Organisaatiot"
    },
    buttonGeneralDescriptionTitle: {
        id: "Containers.Manage.SubMenu.Button.GeneralDescription.Title",
        defaultMessage: "Pohjakuvaukset"
    },
    buttonServicePackagesTitle: {
        id: "Containers.Manage.SubMenu.Button.ServicePackages.Title",
        defaultMessage: "Palvelukokonaisuudet"
    },
    buttonUsersTitle: {
        id: "Containers.Manage.SubMenu.Button.Users.Title",
        defaultMessage: "Käyttäjät"
    }
    });

const SubMenu = props => {    
    const getButtons = (props) => {
        const {formatMessage} = props.intl;
        return [{ id: 'generalDescriptions', disabled: false, defaultPath: 'search/generalDescriptions', name: formatMessage(messages.buttonGeneralDescriptionTitle) },
                { id: 'servicePackages', disabled: true, defaultPath: 'search/servicePackages', name: formatMessage(messages.buttonServicePackagesTitle) },
                { id: 'organizations', disabled: false, defaultPath: 'search/organizations',name: formatMessage(messages.buttonOrganizationTitle) },
                { id: 'users', disabled: false, defaultPath: 'search/users', name: formatMessage(messages.buttonUsersTitle) }];
    }

    const renderButtons = (props) => {
        let buttons = getButtons(props);
        return buttons.map((button) => {
            return <PTVButton key={ 'ManageSubMenuButton' + button.id } disabled={ button.disabled } onClick={() => browserHistory.push('/manage/' + button.defaultPath) } small className={ (props.params.id != button.id ? 'not-highlighted' : '') }>{ button.name } </PTVButton>
        })
    }

    return (
            <div className='manage-sub-menu'>
                <div className="container centered row">
                    { renderButtons(props) }
                </div>
            </div>
        )
}

export default injectIntl(SubMenu);
