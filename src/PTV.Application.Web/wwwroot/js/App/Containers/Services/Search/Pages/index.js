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
import React, { PropTypes, Component } from 'react';
import { Link } from 'react-router';
import { browserHistory } from 'react-router';
import { List } from 'immutable';
import { connect } from 'react-redux';
import { injectIntl, intlShape, FormattedMessage } from 'react-intl'
import shortId from 'shortid';
// Components and containers
import { getDateTimeToDisplay } from '../../../../Components/PTVDateTimePicker/PTVDateTimePicker';
import { PTVHeaderSection, PTVPreloader, PTVMessageBox, PTVTextInput, PTVLabel, PTVCheckBox, PTVAutoComboBox, PTVTable, PTVTooltip } from '../../../../Components';
import { ButtonSearch, ButtonShowMore } from '../../../Common/Buttons';
import PublishingStatusContainer from '../../../Common/PublishStatusContainer';
import { ColumnFormatter } from '../../../Common/PublishStatusContainer';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import Organizations from '../../../Common/organizations';
import LanguageSelect from '../../../Common/languageSelect';

// Messages
import commonMessages from '../../../Common/LocalizedMessages';
import * as Messages from '../Messages';

// Actions
import * as landingActions from '../Actions';
import * as serviceActions from '../../Service/Actions';
import * as appActions from '../../../Common/Actions';
import * as commonServiceActions from '../../Common/Actions';
import * as nodeActions from '../../../Common/Actions/Nodes';
import { callApiDirect } from '../../../../Middleware/Api';

// Styles
import '../../Styles/LandingPage.scss';

// Selectors
import * as CommonSelectors from '../../../Common/Selectors';
import { getServiceClassesObjectArray, getServiceTypesObjectArray } from '../../Common/Selectors';
import { getServices } from '../../Service/Selectors';

import * as ServiceSearchSelectors from '../Selectors';

class ServiceLandingPage extends Component {

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onServiceSearchInputChange(input, value, isSet);
    }

    onListChange = (input) => (value, isAdd) => {
        this.props.actions.onServiceSearchListChange(input, value, isAdd);
    }

    componentDidMount() {
        const { areDataValid } = this.props;
        if (!areDataValid) {
            this.props.actions.loadServiceSearch();
            this.props.actions.loadTranslatableLanguages();
        }
    }

    handleSubmit = () => {
        this.props.actions.loadServices();
    }

     handleShowMore = () => {
        this.props.actions.loadServices(true);
    }

    onRowSelect = (row, isSelected) => {
        this.props.actions.setServiceId(row.id);
        browserHistory.push({ pathname :'/service/manageService' });
    }

    handleAddService = () => {
        browserHistory.push({ pathname :'/service/manageService'});
        this.props.actions.setServiceId(shortId.generate());
    }

    linkFormatter = (cell,row) => {
       return (
            <div className='link-like'>
                <PTVTooltip
                    labelContent = { cell }
                    tooltip = { cell }
                    type = 'special'
                    attachToBody
                />
            </div>
       );
    }

    generalDataFormatter = (cell,row) => {
       return (
           <PTVTooltip
                labelContent = { cell }
                tooltip = { cell ? cell : null }
                type = 'special'
                attachToBody
            />
       );
    }

    fromatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;
    }

    dateTimeFormatter = (cell,row) => {
       var dateTime = getDateTimeToDisplay(cell);
       return (
           <PTVTooltip
                labelContent = { dateTime }
                tooltip = { dateTime }
                type = 'special'
                attachToBody
            />
       );
    }

    formatServiceType = (cell, row) => {
        return row.serviceTypeId ? this.props.intl.formatMessage({ id: row.serviceTypeId, defaultMessage: row.serviceType }) : '';
    }

    columnsDefinition = [
            {width:"70", dataField:"publishingStatus", dataFormat: this.fromatPublishingStatus, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderPublishingStatus)},
            {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
            {dataField:"name", dataSort:true, dataFormat:this.linkFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderName)},
            {dataField:"serviceType", dataFormat:this.formatServiceType, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderServiceType)},
            {dataField:"serviceClassName", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderServiceClass)},
            {dataField:"connectedChannelsCount", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderConnectedChannels)},
            {dataField:"ontologyTermsText", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderOntologyWords)},
            {dataField:"modified", dataFormat:this.dateTimeFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderEdited)},
            {dataField:"modifiedBy", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.serviceSearchResultHeaderModifier)}
        ];

    selectRowProp = {
            mode: "radio",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted",
            onSelect: this.onRowSelect,
            hideSelectColumn : true
        }

    ontologyTermOptions = (input, callBack) => {
            if (input == "" || input.length < 3){
                callBack(null, { options: []})
                return;
            }
            const data = { 
                searchValue: input, 
                treeType: 'OntologyTerm',
                resultType: 'List'
            };
            const call = callApiDirect('service/GetFilteredList', data)
                .then((json) => {

                return { options: json.model.map(x => {
                        return {
                            id: x.id,
                            name: x.name,
                            code: x.name,
                        }
                    }),
                    complete: true};
            });
            return call;
    }

    onOntologyTermSearch = (id, value) =>  {
        if (value){      
            this.props.actions.searchInTree(CommonSchemas.ONTOLOGY_TERM_ARRAY, "OntologyTerm", value.name, value.id);
        }
        else {
            props.actions.clearSearch(treeType);
        }
    }

    render() {
        const { serviceClasses, isFetching,
            serviceSearchForm, publishingStatuses, serviceName, language,
            prefiledOrganizationId, organizationId, ontologyWord,
            serviceClassId, serachIsFetching, servicesCount,
            searchedServices, serachAreDataValid, searchMaxPageCount,
            serviceTypes, serviceTypeId, keyToState, pageNumber, searchIsMoreThanMax} = this.props;

        const { formatMessage } = this.props.intl;

        return (
                <div>
                   <PTVHeaderSection
                       titleClass = "card-title"
                       title= {formatMessage(Messages.messages.pageHeaderTitle)}
                       descriptionClass = "lead"
                       description = {formatMessage(Messages.messages.pageHeaderDescription)}
                       buttonText = {formatMessage(Messages.messages.pageHeaderAddServiceButton)}
                       buttonClick = {this.handleAddService}
                    />

                    <div className="box box-white">
                      <div className="form-wrap">
                        { isFetching ? <PTVPreloader /> :
                        <form className="form-inline  service-search" role="form">
                            <h2 className="search-title"><FormattedMessage id="Containers.Services.LandingPage.Search.Header.Title" defaultMessage="Hae palveluita"/></h2>
                            <div className="row form-group">
                                <div className="col-lg-6">
                                    <PTVTextInput
                                        value={serviceName}
                                        id="1"
                                        placeholder={formatMessage(Messages.messages.namePlaceholder)}
                                        label={formatMessage(Messages.messages.nameTitle)}
                                        labelClass="col-sm-4"
                                        inputclass="col-sm-8"
                                        changeCallback={this.onInputChange('serviceName')}
                                        onEnterCallBack={this.handleSubmit}
                                        name='serviceName'
                                        maxLength={100}
                                        componentClass="row"/>
                                </div>
                                <div className="col-lg-6">
                                    <Organizations
                                        value={ organizationId }
                                        id="4"
                                        label={formatMessage(Messages.messages.organizationComboTitle)}
                                        tooltip={formatMessage(Messages.messages.organizationComboTooltip)}
                                        labelClass="col-sm-4"
                                        autoClass="col-sm-8"
                                        name='organizationId'
                                        changeCallback={ this.onInputChange('organizationId') }
                                        componentClass="row"
                                        virtualized= { true }
                                        className="limited w320"
                                        inputProps= { {'maxLength': '100'} } />
                                </div>
                            </div>
                                <div className="row form-group">
                                    <div className="col-lg-6">
                                        <PTVAutoComboBox
                                            value={serviceClassId}
                                            id="3"
                                            label={formatMessage(Messages.messages.serviceClassComboTitle)}
                                            tooltip={formatMessage(Messages.messages.serviceClassComboTooltip)}
                                            labelClass="col-sm-4"
                                            autoClass="col-sm-8"
                                            name='serviceClassId'
                                            values={serviceClasses}
                                            changeCallback={this.onInputChange('serviceClassId')}
                                            componentClass="row"
                                            className="limited w320" />
                                </div>
                                <div className="col-lg-6">
                                    <PTVAutoComboBox
                                        async={true}
                                        id="2"
                                        placeholder={formatMessage(Messages.messages.ontologyKeysPlaceholder)}
                                        label={formatMessage(Messages.messages.ontologyKeysTitle)}
                                        tooltip={formatMessage(Messages.messages.ontologyKeysTooltip)}
                                        labelClass="col-sm-4"
                                        autoClass="col-sm-8"
                                        name='OntologyTermSearch'
                                        value = { ontologyWord }
                                        values= { this.ontologyTermOptions }
                                        changeCallback= { this.onInputChange('ontologyWord') }
                                        componentClass="row"
                                        className="limited w320"
                                    />
                                    
                                </div>
                            </div>

                            <div className="row form-group">
                                    <div className="col-lg-6">
                                        <PTVAutoComboBox
                                            value={serviceTypeId}
                                            id="3"
                                            label={formatMessage(Messages.messages.serviceTypeComboTitle)}
                                            tooltip={formatMessage(Messages.messages.serviceTypeComboTooltip)}
                                            labelClass="col-sm-4"
                                            autoClass="col-sm-8"
                                            name='serviceType'
                                            values={serviceTypes}
                                            changeCallback={this.onInputChange('serviceTypeId')}
                                            componentClass="row"
                                            className="limited w320"
                                            useFormatMessageData = {true}
                                            />
                                </div>
                                    <PublishingStatusContainer onChangeBoxCallBack={this.onListChange('selectedPublishingStatuses')}
                                        tooltip= { formatMessage(Messages.messages.publishingStatusTooltip) }
                                        isSelectedSelector= { ServiceSearchSelectors.getIsSelectedPublishingStatus }
                                        containerClass="col-lg-6"
                                        labelClass="col-sm-4"
                                        contentClass="col-sm-8"
                                        keyToState= { keyToState } />
                            </div>
                            
                            <div className="row form-group">
                                    <div className="col-lg-6">
                                        <LanguageSelect
                                            keyToState= 'service'
                                            />
                                    </div>
                            </div>

                            <div className="row">
                                <div className="col-xs-12 button-group">
                                    <ButtonSearch onClick={this.handleSubmit}/>
                                </div>
                            </div>
                            <div className="row">
                                <div className="col-xs-12">
                                    {searchedServices.size > 0 ?
                                        <div>
                                        {searchIsMoreThanMax ? <PTVLabel labelClass="strong">{formatMessage(Messages.messages.serviceResultCount) + (pageNumber*searchMaxPageCount) + '/' + servicesCount}</PTVLabel>
                                            : <PTVLabel labelClass="strong">{formatMessage(Messages.messages.serviceResultCount) + servicesCount}</PTVLabel>}
                                        <PTVTable 
                                                contentDataSlector= { getServices }
                                                data= { searchedServices }
                                                striped={true}
                                                hover={true}
                                                selectRow={this.selectRowProp}
                                                columnsDefinition={this.columnsDefinition} 
                                                language = { language }/>
                                        {searchIsMoreThanMax ? <div>
                                                                <div className='button-group centered'>{ !serachIsFetching ? <ButtonShowMore onClick={this.handleShowMore}/> : <PTVPreloader />}</div>
                                                                <PTVLabel labelClass="strong">{formatMessage(Messages.messages.serviceResultCount) + (pageNumber*searchMaxPageCount) + '/' + servicesCount}</PTVLabel>
                                                              </div> :
                                            <PTVLabel labelClass="strong">{formatMessage(Messages.messages.serviceResultCount) + servicesCount}</PTVLabel>}
                                        </div>
                                        : searchedServices.size == 0 && !serachIsFetching && serachAreDataValid ?
                                           <PTVLabel labelClass="strong"> {formatMessage(commonMessages.noSeachResults)} </PTVLabel>
                                           : serachIsFetching ?
                                             <PTVPreloader />
                                               : null
                                    }
                                </div>
                            </div>
                        </form> }
                        <div className="clearfix"></div>
                        </div>
                    </div>
                </div>
       );
    }
}

ServiceLandingPage.propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired
    };

ServiceLandingPage.defaultProps = {
        keyToState: 'service',
    };

function mapStateToProps(state, ownProps) {
    const props = { keyToState: 'service', keyToEntities: 'services' };
  return {
    serviceClasses: getServiceClassesObjectArray(state),
    publishingStatuses: CommonSelectors.getPublishingStatusesImmutableList(state),
    isFetching: CommonSelectors.getSearchIsFetching(state, props),
    areDataValid: CommonSelectors.getSearchAreDataValid(state, props),
    //prefiledOrganizationId: ServiceSearchSelectors.getPrefilledOrganizationId(state),
    serviceName: ServiceSearchSelectors.getServiceName(state),
    organizationId: ServiceSearchSelectors.getOrganizationId(state),
    ontologyWord: ServiceSearchSelectors.getOntologyWord(state),
    serviceClassId: ServiceSearchSelectors.getServiceClassId(state),
    searchedServices: CommonSelectors.getSearchedEntities(state, props),
    servicesCount: CommonSelectors.getSearchResultsCount(state, props),
    pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
    serachIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
    serachAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
    searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
    searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
    serviceTypes: getServiceTypesObjectArray(state),
    serviceTypeId: ServiceSearchSelectors.getServiceTypeId(state),
    language: CommonSelectors.getSearchResultsLanguage(state, props),
  }
}

const actions = [
    landingActions,
    serviceActions,
    commonServiceActions,
    appActions,
    nodeActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceLandingPage));
