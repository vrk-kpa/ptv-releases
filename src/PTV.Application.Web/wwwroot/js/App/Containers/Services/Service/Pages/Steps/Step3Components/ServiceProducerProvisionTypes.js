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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import {connect} from 'react-redux';
import { List} from 'immutable';

import * as mainActions from '../../../Actions';
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps';
import * as PTVValidatorTypes from '../../../../../../Components/PTVValidators'; 
import { PTVLabel, PTVTextInput, PTVTextAreaNotEmpty } from '../../../../../../Components';
import { LocalizedComboBox } from '../../../../../Common/localizedData';
import * as ServiceSelectors from '../../../Selectors';
import * as OrganizationSelectors from '../../../../../Manage/Organizations/Common/Selectors';
import Organizations from '../../../../../Common/organizations';

const messages = defineMessages({
    producerSelectionTitle: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Title",
        defaultMessage: "Tuottaja"
    },
    producerSearchBoxPlaceholder: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Placeholder",
        defaultMessage: "Hae organisaatioista"
    },
    producerSearchBoxTooltip: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.SearchProducer.Tooltip",
        defaultMessage: "Tuottaja"
    },
    producerLinkTitle: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Title",
        defaultMessage: "Linkki palvelun tuottajien tietoihin"
    },
    producerLinkPlaceholder: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Placeholder",
        defaultMessage: "Linkki palvelun tuottajien tietoihin"
    },
    producerLinkTooltip: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.ProducerLink.Tooltip",
        defaultMessage: "Linkki palvelun tuottajien tietoihin"
    },
    producerExternalDescriptionTitle: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.ExternalDescription.Title",
        defaultMessage: "Jos tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen."
    },
    producerExternalDescriptionTooltip: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.ExternalDescription.Tooltip",
        defaultMessage: "Tuottaja ei löydy Palvelutietovarannosta, voit lisätä ulkopuolisen kuvauksen."
    },
    producerFreeDescriptionPlaceholder: {
        id : "Containers.Services.AddService.Step3.ServiceProducer.FreeDescription.Placeholder",
        defaultMessage: "Kirjoita vapaa kuvaus"
    },
    producerLinkTitleView: {
        id : "Containers.Services.ViewService.Step3.ServiceProducer.Title",
        defaultMessage: "Palvelun tuottaja:"
    },
});

function onInputChange (props,type,input,isSet=false) { return (value) => {
        props.actions.onProducerObjectChange(props.producerId,{ [type] : {[input]: value} },props.language, isSet);
    }
}

class SelfProducer extends Component {
    constructor(props) {
        super(props);      
    }

    validators = [PTVValidatorTypes.IS_REQUIRED]; 

    static propTypes = {
        intl: intlShape.isRequired,
        actions: PropTypes.object.isRequired,
        producerId: PropTypes.string.isRequired,
        details: PropTypes.object.isRequired
    };

   
    renderProducerSelection(){
        const { details, selectedOrganizers, readOnly, translationMode} = this.props;

        switch( selectedOrganizers.size ){
            case 0 :
                return readOnly || translationMode == "view" || translationMode == "edit" ? this.renderReadOnlyProducer() : null;
            case 1 :
                return readOnly || translationMode == "view" || translationMode == "edit" ? this.renderReadOnlyProducer(selectedOrganizers.first()) : this.renderMainProducerAsLabel(selectedOrganizers.first());
            default:
                return readOnly || translationMode == "view" || translationMode == "edit" ? this.renderReadOnlyProducer(selectedOrganizers.find(x => x.get('id') == details.get('producerOrganizationId'))) : this.renderMainProducerSelectable(details.get('producerOrganizationId'), selectedOrganizers);
        }
    }

    renderReadOnlyProducer(organization){
         const {formatMessage} = this.props.intl;
         return (
            <div className="row" >
               <PTVTextInput componentClass="col-lg-12"
                        value={organization ? organization.get('name') : ""}
                        label={formatMessage(messages.producerLinkTitleView)}
                        name='readOnlyProducer' 
                        readOnly={ true }/>
            </div>
        )
    }
    renderMainProducerSelectable(producerOrganizationId, organizers){
        const {formatMessage} = this.props.intl;
        const {readOnly} = this.props;
        const values = organizers.toJS();
       //const selected = organizers.find(x => x.get('id') == producerOrganizationId );

        return (
            <div>
            <div className="row" >
                    <LocalizedComboBox
                        value={producerOrganizationId}
                        values={values}
                        componentClass="col-lg-12"
                        label={formatMessage(messages.producerSelectionTitle)}
                        tooltip={formatMessage(messages.producerSearchBoxTooltip)}
                        changeCallback={onInputChange(this.props,'selfProduced','producerOrganizationId')}
                        className="limited w480"
                        validators={ this.validators }
                        name='selfProducer' />
                </div>               
            </div>
        )
    }

    ///
    renderMainProducerAsLabel(organization){
        const {formatMessage} = this.props.intl;

        return (
            <div>
                <div className="row" >
                    <PTVLabel tooltip={formatMessage(messages.producerSearchBoxTooltip)} labelClass="col-lg-12" >{formatMessage(messages.producerSelectionTitle)}</PTVLabel>
                </div>
                <div className="row" >
                    <PTVLabel labelClass="col-lg-12" >{ organization.get('name') }</PTVLabel>
                </div>            
            </div>
        );
    }

    /// render
    render (){
        return <div>{ this.renderProducerSelection() } </div>;
    }
}

/// VoucherServices component - Palvelusetelipalvelut
class VoucherServices extends Component {
    constructor(props) {
        super(props);
    }


    static propTypes = {
        intl: intlShape.isRequired,
        actions: PropTypes.object.isRequired,
        producerId: PropTypes.string.isRequired,
        details: PropTypes.object.isRequired
    };   
    
    renderReadOnlyVoucher(link, freeDescription){
         const {formatMessage} = this.props.intl;
         return (
            <div className="row" >
               <PTVTextInput componentClass="col-lg-12"
                        value={link + " " + freeDescription}
                        label={formatMessage(messages.producerLinkTitleView)}
                        name='readOnlyVoucher' 
                        readOnly={ true }/>
            </div>
        )
    }

    render (){
        const {formatMessage} = this.props.intl;
        const { details, detailsDefault, readOnly, translationMode} = this.props;
        const link = details.get('link') || detailsDefault.get('link') || "";
        const freeDescription = details.get('freeDescription') || detailsDefault.get('freeDescription') || "";
        return readOnly && translationMode == "none" ?
        this.renderReadOnlyVoucher(link, freeDescription, translationMode) :
        (           
            <div>
                <div className="row form-group">
                    <PTVTextInput componentClass="col-lg-12"
                        value={ link }
                        label={formatMessage(messages.producerLinkTitle)}
                        placeholder={formatMessage(messages.producerLinkPlaceholder)}
                        blurCallback={onInputChange(this.props,'voucherServices','link')}
                        name='linkToProducer' 
                        disabled = { translationMode == "view" }
                        maxLength={ 500 }
                        />
                </div>
                <div className="row">
                    <PTVTextAreaNotEmpty componentClass="col-lg-12"
                        minRows={6} maxLength={150}
                        value={ freeDescription }
                        placeholder={formatMessage(messages.producerFreeDescriptionPlaceholder)}
                        blurCallback={onInputChange(this.props,'voucherServices','freeDescription')}
                        name='linkToProducerDescription'
                        disabled = { translationMode == "view" }                        
                    />
                </div>
            </div>
        )
    }
}

/// PurchaseServices component - Ostopalvelut
class PurchaseServices extends Component {
    constructor(props) {
        super(props);
    }


    static propTypes = {
        intl: intlShape.isRequired,
        actions: PropTypes.object.isRequired,
        producerId: PropTypes.string.isRequired,
        details: PropTypes.object.isRequired
    };
    
    renderReadOnlyPurchase(organization, description){
         const {formatMessage} = this.props.intl;
         return (
            <div className="row" >
               <PTVTextInput componentClass="col-lg-12"
                        value={organization ? organization.name : description}
                        label={formatMessage(messages.producerLinkTitleView)}
                        name='readOnlyPurchase' 
                        readOnly={ true }/>
            </div>
        )
    }
    
    render (){
        const {formatMessage} = this.props.intl;
        const { allOrganizations , details, detailsDefault, readOnly, translationMode} = this.props;  
        const producerOrganizationId = details.get('producerOrganizationId');  
        const freeDescription = details.get('freeDescription') || detailsDefault.get('freeDescription') || "";   
        return readOnly && translationMode == "none" ? 
        this.renderReadOnlyPurchase(allOrganizations.find(x => x.id == producerOrganizationId), freeDescription) :
         (
            <div>
                <div className="row">
                    <Organizations
                        value={ producerOrganizationId }
                        label={ formatMessage(messages.producerSelectionTitle) }
                        tooltip={ formatMessage(messages.producerSearchBoxPlaceholder) }
                        componentClass="col-xs-12"
                        name='producer'
                        changeCallback={ onInputChange(this.props,'purchaseServices','producerOrganizationId') }
                        virtualized= { true }
                        className="limited w480"
                        readOnly = { translationMode == "view" || translationMode == "edit" }   
                        inputProps= { {'maxLength': '100'} } />
                </div>
                {
                    producerOrganizationId == null ?
                    <div className="row">
                        <PTVTextAreaNotEmpty componentClass="col-lg-12"
                            label={formatMessage(messages.producerExternalDescriptionTitle)}
                            counterClass="counter"
                            placeholder={formatMessage(messages.producerFreeDescriptionPlaceholder)}
                            minRows={6} maxLength={150}
                            value={freeDescription}
                            blurCallback={onInputChange(this.props,'purchaseServices','freeDescription')}
                            name='producerTextArea'
                            disabled = { translationMode == "view" }   
                            />
                    </div>
                    : null
                }
            </div>
        )
    }
}

/// Other component - Muu
class OtherProducer extends Component {
    constructor(props) {
        super(props);
    }

    static propTypes = {
        intl: intlShape.isRequired,
        actions: PropTypes.object.isRequired,
        producerId: PropTypes.string.isRequired,
        details: PropTypes.object.isRequired
    };

    renderReadOnlyOtherProducer = (organization) => {
         const {formatMessage} = this.props.intl;
         return (
            <div className="row" >
               <PTVTextInput componentClass="col-xs-12"
                        value={organization ? organization.name : ""}
                        label={formatMessage(messages.producerLinkTitleView)}
                        name='readOnlyOtherProducer' 
                        readOnly={ true }/>
            </div>
        )
    }
    
    render (){
        const {formatMessage} = this.props.intl;
        const { allOrganizations , details , readOnly, translationMode } = this.props;
        const producerOrganizationId = details.get('producerOrganizationId');
       return readOnly || translationMode == "view" || translationMode == "edit" ? 
       this.renderReadOnlyOtherProducer(allOrganizations.find(x => x.id == producerOrganizationId)) :
        (
            <div className="row">
                <Organizations
                        value={ producerOrganizationId }
                        label={ formatMessage(messages.producerSelectionTitle) }
                        tooltip={ formatMessage(messages.producerSearchBoxPlaceholder) }
                        componentClass="col-xs-12"
                        name='otherProducer'
                        changeCallback={ onInputChange(this.props,'other','producerOrganizationId') }
                        virtualized= { true }
                        className="limited w480"
                        readOnly = { translationMode == "view" || translationMode == "edit" }   
                        inputProps= { {'maxLength': '100'} } />
            </div>
        )
    }
}

/// maps state to props
function mapStateToProps(state, ownProps) {
  return {
      selectedOrganizers : ServiceSelectors.getSelectedOrganizersMap(state,ownProps),
      allOrganizations: OrganizationSelectors.getOrganizationsObjectArray(state),
  }
}

const actions = [
    mainActions
];

export default {
    SelfProducer: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SelfProducer)),
    VoucherServices: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(VoucherServices)),
    PurchaseServices: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PurchaseServices)),
    OtherProducer: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OtherProducer))
}

