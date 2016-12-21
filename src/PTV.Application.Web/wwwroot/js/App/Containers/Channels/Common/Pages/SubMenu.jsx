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
import { PTVButton }from '../../../../Components/';
import { defineMessages, injectIntl } from 'react-intl'
import { Link, browserHistory } from 'react-router';

import './SubMenu.scss';

const messages = defineMessages({
    buttonTitle: {
        id: "Components.Services.SubMenu.ButtonTitle",
        defaultMessage: "Palvelut"
    },
    subMenuItemElectronic: {
        id: "Components.Services.SubMenu.Item.Electronic",
        defaultMessage: "Verkkoasiointi"
    },
    subMenuItemWebPage: {
        id: "Components.Services.SubMenu.Item.WebPage",
        defaultMessage: "Verkkosivu"
    },
    subMenuItemTransactionForm: {
        id: "Components.Services.SubMenu.Item.TransactionForm",
        defaultMessage: "Tulostettava lomake"
    },
    subMenuItemPhone: {
        id: "Components.Services.SubMenu.Item.Phone",
        defaultMessage: "Puhelinasiointi"
    },
    subMenuItemServiceLocation: {
        id: "Components.Services.SubMenu.Item.ServiceLocation",
        defaultMessage: "Palvelupiste"
    },
    });
    
const SubMenu = props => {
    
    const handleSubMenuItemChanged = (id) => () => {
        browserHistory.push('/channels/search/' + id);
    }
    
    const getButtons = () => {
        const {formatMessage} = props.intl;
        return [{ id: 'eChannel', name: formatMessage(messages.subMenuItemElectronic) },
                { id: 'webPage', name: formatMessage(messages.subMenuItemWebPage) },
                { id: 'printableForm', name: formatMessage(messages.subMenuItemTransactionForm) },
                { id: 'phone', name: formatMessage(messages.subMenuItemPhone) },
                { id: 'serviceLocation', name: formatMessage(messages.subMenuItemServiceLocation) }];
    }
    
    const renderButtons = () => {
        const buttons = getButtons();
        return buttons.map((button) => {
            return <PTVButton key={ button.id } onClick = { handleSubMenuItemChanged(button.id) } className={props.params.id != button.id ? ' not-highlighted' : ''}>{ button.name } </PTVButton>
        })
    }

        return (
            <div className='service-sub-menu'>
                <div className="container row centered">
                    { renderButtons() }
                </div>
            </div>
        ) 
}

export default injectIntl(SubMenu);
