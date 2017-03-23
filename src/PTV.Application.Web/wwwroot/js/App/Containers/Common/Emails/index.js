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
import { injectIntl } from 'react-intl';
import ImmutablePropTypes from 'react-immutable-proptypes';
import shortId from 'shortid';
import classNames from 'classnames';

// components
import Email from './Email';
import { PTVAddItem } from '../../../Components';

// selectors
import * as CommonSelectors from '../Selectors';

export const Emails = ({readOnly, items, intl, messages, onAddEmail, onRemoveEmail, shouldValidate, collapsible, withInfo, componentClass, 
    translationMode, splitContainer, item}) => {
    const addEmail = (entity) =>{
        onAddEmail(items ? [entity] : entity)
    }

    const renderEmail = (id, index, isNew) => {
        return (
             <div className= { classNames("email-container item-row", componentClass) }>
                <Email
                    key={ item || id }
                    isNew= { isNew }
                    emailId = { item || id }
                    count = { index || 1 }
                    onAddEmail = { addEmail }
                    onRemoveEmail = { onRemoveEmail }
                    messages={ messages }
                    shouldValidate={ shouldValidate }
                    withInfo = { withInfo }
                    readOnly= { readOnly }
                    translationMode = { translationMode }
                    splitContainer = { splitContainer }                
                />
            </div>
	    );
    };

    const onAddButtonClick = () => {
        onAddEmail(items.size === 0 ? [{ id: shortId.generate() }, { id: shortId.generate() }] : [{ id: shortId.generate() }]);
    }

    return(
            <PTVAddItem                
                items = { items }
                readOnly = { readOnly && translationMode == 'none' }
                renderItemContent = { renderEmail }
                messages = {{ "label": intl.formatMessage(messages.title) }}
                onAddButtonClick = { onAddButtonClick }
                onRemoveItemClick = { onRemoveEmail }
                collapsible = { collapsible && translationMode == 'none' }
                multiple = { translationMode == 'none' || translationMode == 'edit' }
                />
    )
}

Emails.propTypes = {
        items: ImmutablePropTypes.list,
        item: PropTypes.any,
        withInfo: PropTypes.bool,
        readOnly: PropTypes.bool,
        shouldValidate: PropTypes.bool,
        onAddEmail: PropTypes.func.isRequired,
        onRemoveEmail: PropTypes.func
    };
Emails.defaultProps = {
   withInfo : true 
};

export default injectIntl(Emails);