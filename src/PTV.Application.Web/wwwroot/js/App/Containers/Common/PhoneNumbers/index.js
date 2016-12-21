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

// components
import PhoneNumberContainer from './PhoneNumberContainer';
import { PTVAddItem } from '../../../Components';

// selectors
import * as CommonSelectors from '../Selectors';

export const PhoneNumbers = ({readOnly, items, intl, messages, language, translationMode, splitContainer, onAddPhoneNumber, onRemovePhoneNumber, shouldValidate, collapsible, item, children, withType}) => {
    const addPhoneNumber = (entity) =>{
        onAddPhoneNumber(items ? [entity] : entity)
    }

    const sharedProps = { readOnly, language, translationMode, splitContainer }
    const renderPhoneNumber = (id, index, isNew) => {
        return (
            <PhoneNumberContainer {...sharedProps}
                key={ item || id }
                isNew= { isNew }
                phoneId = { item || id }
                count = { index || 1 }
                onAddPhoneNumber = { addPhoneNumber }
                onRemovePhoneNumber = { onRemovePhoneNumber }
                messages={ messages }
                shouldValidate={ shouldValidate }               
                children = { children }                
                withType = { withType }                
            />
	    );
    };

    const onAddButtonClick = () => {
        onAddPhoneNumber(items.size === 0 ? [{ id: shortId.generate() }, { id: shortId.generate() }] : [{ id: shortId.generate() }]);
    }
    const title = messages.sectionTitle ? messages.sectionTitle : messages.title;
    return(
            <PTVAddItem                
                items = { items }
                readOnly = { readOnly && translationMode == 'none' }
                renderItemContent = { renderPhoneNumber }
                messages = {{ "label": intl.formatMessage(title) }}
                onAddButtonClick = { onAddButtonClick }
                onRemoveItemClick = { onRemovePhoneNumber }
                collapsible = { collapsible && translationMode == 'none' }
                multiple = { translationMode == 'none' }
                />
    )
}

PhoneNumbers.propTypes = {
        items: ImmutablePropTypes.list,
        item: PropTypes.any,       
        readOnly: PropTypes.bool,
        shouldValidate: PropTypes.bool,
        withType: PropTypes.bool,
        onAddPhoneNumber: PropTypes.func.isRequired,
        onRemovePhoneNumber: PropTypes.func,
    };
PhoneNumbers.defaultProps = {
    
};

export default injectIntl(PhoneNumbers);