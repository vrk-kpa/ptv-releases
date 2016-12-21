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
import {Map} from 'immutable';
import {bindActionCreators} from 'redux';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';
import { PTVHeaderSection, PTVTextInput, PTVCheckBox, PTVAutoComboBox, PTVOverlay, PTVPreloader, PTVLabel, PTVTable, PTVTooltip } from '../../../../Components';
import PTVCheckBoxList from '../../../../Components/PTVCheckBoxList/PTVCheckBoxListNew';

// Components
import { ButtonSearch, ButtonShowMore } from '../../../Common/Buttons.jsx';
import * as ChannelActions from '../Actions';
import * as CommonChannelActions from '../../Common/Actions';
import PublishingStatusContainer from '../../../Common/PublishStatusContainer';
import { ColumnFormatter } from '../../../Common/PublishStatusContainer';
import { browserHistory } from 'react-router';
import commonMessages from '../../../Common/LocalizedMessages';
import * as appActions from '../../../Common/Actions';
import { getDateTimeToDisplay } from '../../../../Components/PTVDateTimePicker/PTVDateTimePicker';
import Organizations from '../../../Common/organizations';
import LanguageSelect from '../../../Common/languageSelect';

// selectors
import * as ChannelSearchSelectors from '../Selectors';
import * as CommonSelectors from '../../../Common/Selectors';
import { getChannels } from '../../Common/Selectors';

// messages
import * as Messages from '../Messages';

const PhoneNumberTypeComponent = ({phoneNumber, intl: {formatMessage}}) => {
    const typeId = phoneNumber.get('typeId');
    return(<span>{typeId ? formatMessage({ id: typeId, defaultMessage: '' }) : ''}</span>);
}

function mapStateToPropsPhone(state, ownProps) {
    return {
        phoneNumber: CommonSelectors.getPhoneNumber(state, ownProps.id)
    }
}

const PhoneNumberType = connect(mapStateToPropsPhone)(injectIntl(PhoneNumberTypeComponent));

class SearchChannelMainContainer extends Component {

	    static propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired,
        channelType: PropTypes.any.isRequired
    };

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onChannelSearchInputChange(input, value, isSet, this.props.channelType);
    }
    
    onListChange = (input) => (value, isAdd) => {
        this.props.actions.onChannelsSearchListChange(input, value, isAdd, this.props.channelType);
    }

    constructor(props) {
        super(props);
        this.linkFormatter = this.linkFormatter.bind(this);
        this.onRowSelect = this.onRowSelect.bind(this);

    }

    handleSubmit = () => {
        this.props.actions.loadChannelSearchResults(this.props.channelType);
    }

    handleShowMore = () => {
        this.props.actions.loadChannelSearchResults(this.props.channelType, true);
    }

    linkFormatter(cell,row){
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

    loadData = props => {
        const { areDataValid, isFetching, channelType } = props;
        if (!isFetching && !areDataValid) {
            this.props.actions.loadChannelSearch(channelType);
        }
    }

    componentWillReceiveProps(newProps) {
        this.loadData(newProps);
    }

    componentDidMount() {
        this.loadData(this.props);
        this.props.actions.loadTranslatableLanguages();
    }

    fromatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;
    }

    onRowSelect(row, isSelected){

        const { channelType } = this.props;
        var pathname = null;

        pathname = '/channels/manage/'+ channelType;
        this.props.actions.setChannelId(channelType, row.id);
        browserHistory.push({ pathname : pathname, state: { entityId : row.id, readOnly: true, publishingStatus: { type: row.publishingStatus.type } }});
    }

    formatPhoneType = (cell, row) => {
        return <PhoneNumberType id= {row.phoneNumber}/>
    }

    dateTimeFormater(cell,row){
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

    onAddButtonClick = () => {
        this.props.actions.setChannelId(this.props.channelType);
    }

	render() {

        var selectRowProp = {
            mode: "radio",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted",
            hideSelectColumn : true,
            onSelect: this.onRowSelect
        }

		const { formatMessage } = this.props.intl;
        const { isFetching, channelName, organizationId,
            channelFormIdentifier, phoneNumberTypes, searchAreDataValid,
            searchIsFetching, searchedChannels, channelSearchBox, channelSearchResults,
            searchIsMoreThanMax, searchChannelsCount, pageNumber, searchMaxPageCount, language } = this.props;
        const { channelType } = this.props;

    const resultChannelsColumnsDefinition = [
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {width:"70", dataField:"publishingStatus", dataFormat: this.fromatPublishingStatus, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages[`${channelType}ChannelResultTableHeaderPublishingStatusTitle`])},
        {dataField:"name", dataSort:true, dataFormat:this.linkFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages[`${channelType}ChannelResultTableHeaderNameTitle`])},
        {hidden: channelType != "phone" ? true : false, dataSort:true, dataFormat: this.formatPhoneType, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.phoneChannelResultTableHeaderPhoneTypeTitle)},
        {hidden: channelType != "printableForm" ? true : false, dataField:"formIdentifier", dataFormat: this.generalDataFormatter, dataSort:true, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages.printableFormChannelSearchBoxFormIdentifierTitle)},
        {width:"100", dataField:"connectedServices", columnHeaderTitle:this.props.intl.formatMessage(Messages.messages[`${channelType}ChannelResultTableHeaderServicesCount`])},
        {dataField:"modified", dataFormat: this.dateTimeFormater, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages[`${channelType}ChannelResultTableHeaderConnectedTitle`])},
        {dataField:"modifiedBy", dataFormat: this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(Messages.messages[`${channelType}ChannelResultTableHeaderConnectedByTitle`])}
    ];

     	return (

 			  	<div>
	             	<PTVHeaderSection
	                   titleClass = "card-title"
	                   title=  {formatMessage(Messages.messages[`${channelType}ChannelHeaderSectionTitle`])}
	                   descriptionClass = "lead"
	                   description = {formatMessage(Messages.messages[`${channelType}ChannelHeaderSectionDescription`])}
	                   buttonText = {formatMessage(Messages.messages[`${channelType}ChannelHeaderSectionButtonTitle`])}
                       buttonClick = {this.onAddButtonClick}
	                   buttonRoute = { `/channels/manage/${channelType}` }
	                />

                <div className="box box-white">
                    <div className="form-wrap">
                         { isFetching ? <PTVPreloader /> :
                        <form className="form-inline" role="form">
                            <h2 className="search-title"> {formatMessage(Messages.messages[`${channelType}ChannelSearchBoxHeaderTitle`])} </h2>

                            <div className="row form-group">
                                <div className="col-lg-6">
                                    <PTVTextInput
                                        value={ channelName }
                                        placeholder={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxNamePlaceholder`])}
                                        label={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxNameTitle`])}
                                        labelClass="col-sm-4"
                                        inputclass="col-sm-8"
                                        name='channelName'
                                        changeCallback={this.onInputChange('channelName')}
                                        onEnterCallBack={this.handleSubmit}
                                        maxLength={100}
                                        componentClass="row"
                                    />
                                </div>
                                <div className="col-lg-6">
                                    <Organizations
                                        value={ organizationId }
                                        label={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxOrganizationTitle`])}
                                        tooltip={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxOrganizationTooltip`])}
                                        labelClass="col-sm-4"
                                        autoClass="col-sm-8"
                                        name='organizationId'
                                        changeCallback={this.onInputChange('organizationId')}
                                        componentClass="row"
                                        className="limited w320"
                                        inputProps= { {'maxLength': '100'} }
                                        />
                                </div>
                            </div>
                            { channelType == 'printableForm' ?
                                <div className="row form-group">
                                    <div className="col-lg-6">
                                        <PTVTextInput
                                            value={ channelFormIdentifier }
                                            placeholder={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxFormIdentifierPlaceholder`])}
                                            label={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxFormIdentifierTitle`])}
                                            tooltip={formatMessage(Messages.messages[`${channelType}ChannelSearchBoxFormIdentifierTooltip`])}
                                            labelClass="col-sm-4"
                                            inputclass="col-sm-8"
                                            name='channelName'
                                            changeCallback={this.onInputChange('channelFormIdentifier')}
                                            onEnterCallBack={this.handleSubmit}
                                            componentClass="row"
                                            maxLength= { 100 }/>
                                    </div>
                                </div> : null }

                            { channelType == "phone" ? (
                                <div className="row form-group">
                                    <PTVCheckBoxList
                                        label = {formatMessage(Messages.messages.searchBoxPhoneNumberTypeTitle)}
                                        tooltip = {formatMessage(Messages.messages.searchBoxPhoneNumberTypeTooltip)}
                                        labelBoxClass="col-lg-3 col-md-5 col-sm-12"
                                        inputBoxClass="col-lg-9 col-md-7 col-sm-12"
                                        box={ phoneNumberTypes }
                                        isSelectedSelector= { (state, props) => ChannelSearchSelectors.getIsSelectedPhoneNumberType(state, {...props, ...{keyToState: this.props.channelType }}) }
                                        onChangeBox = {this.onListChange('selectedPhoneNumberTypes')}
                                        useFormatMessageData = {true}
                                        />
                                </div>
                                )
                            : null }
                            
                            <div className="row form-group">
                                    <div className="col-lg-6">
                                        <LanguageSelect
                                            keyToState= { this.props.channelType }
                                            />
                                    </div>
                            </div>

                            <div className="row">
                               <PublishingStatusContainer onChangeBoxCallBack={this.onListChange('selectedPublishingStatuses')}
                                        tooltip= { formatMessage(Messages.messages[`${channelType}ChannelSearchBoxPublishingStatusTooltip`]) }
                                        isSelectedSelector= { ChannelSearchSelectors.getIsSelectedPublishingStatus } 
	                                  containerClass="col-xs-12"
	                                  labelClass="col-sm-4 col-lg-2"
	                                  contentClass="col-sm-8 col-lg-10"
                                      keyToState= { this.props.channelType }
                                        />
                            </div>

                            <div className="row">
                                <div className="col-xs-12 button-group">
                                    <ButtonSearch className="ptv-button small" onClick={this.handleSubmit}/>
                                </div>
                            </div>
                            
                            <div className="row">
                                <div className="col-xs-12">
                                    { searchedChannels.size > 0 ?
                                        <div>
                                            { searchIsMoreThanMax ? <PTVLabel labelClass="strong">{formatMessage(Messages.messages[`${channelType}ChannelResultTableDescriptionResultCount`]) + (pageNumber*searchMaxPageCount) + '/' + searchChannelsCount }</PTVLabel> :
                                        <PTVLabel labelClass="strong">{formatMessage(Messages.messages[`${channelType}ChannelResultTableDescriptionResultCount`]) + searchChannelsCount }</PTVLabel> }
                                            
                                            <PTVTable
                                                contentDataSlector= { getChannels }
                                                data= { searchedChannels }
                                                striped={true}
                                                hover={true}
                                                pagination={false}
                                                selectRow={selectRowProp}
                                                columnsDefinition={resultChannelsColumnsDefinition}
                                                language = { language }
                                                //columnsDefinition={this.resultChannelsColumnsDefinition}
                                            />
                                            {searchIsMoreThanMax ? <div>
                                                <div className='button-group centered'>{ !searchIsFetching ? <ButtonShowMore onClick={this.handleShowMore}/> : <PTVPreloader />}</div>
                                                <PTVLabel labelClass="strong">{formatMessage(Messages.messages[`${channelType}ChannelResultTableDescriptionResultCount`]) + (pageNumber*searchMaxPageCount) + '/' + searchChannelsCount }</PTVLabel>
                                                </div> : <PTVLabel labelClass="strong">{formatMessage(Messages.messages[`${channelType}ChannelResultTableDescriptionResultCount`]) + searchChannelsCount }</PTVLabel> }
                                        </div>
                                    : searchedChannels.size == 0 && !searchIsFetching && searchAreDataValid ?
                                        <PTVLabel labelClass="strong"> {formatMessage(commonMessages.noSeachResults)} </PTVLabel>
                                        : searchIsFetching ?
                                            <PTVPreloader /> : null
                                    }
                                </div>
                            </div>
                        
                            
                        </form>}
                        <div className='clearfix'></div>
                    </div>
                </div>
	  		</div>
	    )
    }
}

function mapStateToProps(state, ownProps) {
    const props = {keyToState: ownProps.channelType, keyToEntities: 'channels'};
    return {
        isFetching: CommonSelectors.getSearchIsFetching(state, props),
        channelName: ChannelSearchSelectors.getChannelName(state, props),
        organizationId: ChannelSearchSelectors.getOrganizationId(state, props),
        areDataValid: CommonSelectors.getSearchAreDataValid(state, props),
        channelFormIdentifier: ChannelSearchSelectors.getChannelFormIdentifier(state, props),
        phoneNumberTypes: CommonSelectors.getPhoneNumberTypes(state),
        searchedChannels: CommonSelectors.getSearchedEntities(state, props),
        searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
        pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
        searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
        searchAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
        searchChannelsCount: CommonSelectors.getSearchResultsCount(state, props),
        searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
        language: CommonSelectors.getSearchResultsLanguage(state, props),
    }
}

const actions = [
   ChannelActions,
   appActions,
   CommonChannelActions
 ];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SearchChannelMainContainer));
