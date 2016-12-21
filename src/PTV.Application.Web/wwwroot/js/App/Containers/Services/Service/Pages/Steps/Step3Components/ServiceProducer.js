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
import { connect } from 'react-redux';
import { defineMessages, injectIntl } from 'react-intl';
import shortId from 'shortid';

// Schemas
import { PTVAddItem } from '../../../../../../Components';
import ServiceProducerComponent from './ServiceProducerComponent';

// Selectors
import * as ServiceSelectors from '../../../Selectors';

// actions
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps';
import * as serviceActions from '../../../Actions';

const messages = defineMessages({
    serviceProducersTitle:{
        id : "Containers.Services.AddService.Step3.ServiceProducer.Header.Title", 
        defaultMessage : "Palvelun toteutustapa ja tuottaja"
    }
});

export const ServiceProducer = ({serviceProducers, readOnly, intl, actions, language, translationMode }) => {
    const renderProducer = (producerId, index, isNew) => {
	    return (
                <ServiceProducerComponent 
                    key={producerId}
                    isNew= { isNew }
                    {...{ id:producerId} }
                    order={ index } 
                    readOnly={ readOnly }
                    onAddProducer= { actions.onProducerAdd }
                    language = { language } 
                    translationMode = { translationMode } 
                    />		        
	    );
    }

    const removeProducerButtonClick = (id) => {
        actions.onRemoveProducer(id, language);    
    }

    const onAddButtonClick = (object, count) => {
        actions.onProducerAdd(serviceProducers.size === 0 ? [{ id: shortId.generate(), order: 1 }, { id: shortId.generate(), order: 2 }] : [{ id: shortId.generate(), order: ++count }], language)
    }
    const readOnlyClass = ( readOnly || translationMode == "view" || translationMode == "edit" ) ? "readonly" : "";

    return (
        <PTVAddItem
            items = { serviceProducers }
            componentClass = { readOnlyClass }
            readOnly = { readOnly || translationMode == "view" || translationMode == "edit" }
            renderItemContent = { renderProducer }
            messages = {{ "label": intl.formatMessage(messages.serviceProducersTitle) }}
            onAddButtonClick = { onAddButtonClick }
            onRemoveItemClick = { removeProducerButtonClick }
            collapsible = { false } 
            multiple />   
    )
}

function mapStateToProps(state, ownProps) {
  return {   
        serviceProducers: ServiceSelectors.getProducers(state, ownProps)
    }      
}

const actions = [
    serviceActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceProducer));