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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import {connect} from 'react-redux';
import { Link, browserHistory } from 'react-router';

import * as serviceStep4Actions from '../../Actions/ServiceStep4Actions';
import * as mainActions from '../../Actions';
import * as relationServiceSearchActions from '../../../../Relations/ServiceAndChannels/ServiceSearch/Actions';
import * as commonServiceAndChannelActions from '../../../../Relations/ServiceAndChannels/Common/Actions';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import { ButtonDelete } from '../../../../Common/Buttons';
import { PTVPreloader, PTVConfirmDialog, PTVOverlay, PTVLabel, PTVAutoComboBox, PTVTable, PTVPopupSearchInput, PTVButton, PTVTooltip, PTVIcon } from '../../../../../Components';
import { translateMessages } from '../../../../../ServerMessages/ServerMessages';
import { getDateTimeToDisplay } from '../../../../../Components/PTVDateTimePicker/PTVDateTimePicker';
import Organizations from '../../../../Common/organizations';
import LanguageLabel from '../../../../Common/languageLabel';
import TableNameFormatter from '../../../../Common/tableNameFormatter';
import { getOrganizationsJS } from '../../../../Manage/Organizations/Common/Selectors';
import * as ServiceSelectors from '../../Selectors';
import * as CommonServiceSelectors from '../../../Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';
import * as CommonChannelsSelectors from '../../../../Channels/Common/Selectors';

import ChannelTypes from '../../../../Common/channelTypes';

const messages = defineMessages({

    attachedChannelsBoxTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Data.Title",
        defaultMessage: "Liitettyjen asiointikanavien lukumäärä on {count}. Voit katsella, muikata ja lisätä niitä Liitokset sivulla."
    },
    attachedChannelsBoxNoDataDescription: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.NoData.Title",
        defaultMessage: "Liitettyjen asiointikanavien lukumäärä on 0. Voit lisätä niitä Liitokset sivulla."
    },
    searchChannelNameTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Name.Title",
        defaultMessage: "Nimi"
    },
    searchChannelNameTooltip: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Name.Tooltip",
        defaultMessage: "Nimi"
    },
    searchChannelNamePlaceholder: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Name.Placeholder",
        defaultMessage: "Hae asiointikanavan nimellä ja valitse"
    },
    searchBoxOrganizationTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    searchBoxOrganizationTooltip: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.Organization.Tooltip",
        defaultMessage: "Organisaatio"
    },
    searchBoxChannelTypeTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.SearchBox.ChannelType.Title",
        defaultMessage: "Kanavatyyppi"
    },
    searchBoxChannelTypeTooltip: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.SearchBox.ChannelType.Tooltip",
        defaultMessage: "Kanavatyyppi"
    },
    resultTableDescriptionResultCount: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.ResultCount.Title",
        defaultMessage: "Hakutuloksia: "
    },
    resultTableMoreChannelsAvailableMessage: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.MoreItemsAvailableMessage.Title",
        defaultMessage: '"Lisää kanavia on saatavilla, kirjoita nimi."'
    },
    resultTableNoResultsFound: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.NoResultsFound",
        defaultMessage: "Ei tuloksia"
    },
    resultTableHeaderNameTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.HeaderColumn.Name.Title",
        defaultMessage: "Nimi"
    },
    resultTableHeaderChannelTypeTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.HeaderColumn.ChannelType.Title",
        defaultMessage: "Kanavatyyppi"
    },
    resultTableHeaderEditedTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.HeaderColumn.Edited.Title",
        defaultMessage: "Muokattu"
    },
    resultTableHeaderCreatedByTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.HeaderColumn.CreatedBy.Title",
        defaultMessage: "Muokkaaja"
    },
    resultTableHeaderDetailTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ResultTable.HeaderColumn.Detail.Title",
        defaultMessage: "Tarkemmat tiedot"
    },
    attachedChannelsBoxTableHeaderConnectedTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.AttachedChannelsBox.HeaderColumn.Connected.Title",
        defaultMessage: "Liitetty"
    },
    attachedChannelsBoxTableHeaderConnectedByTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.AttachedChannelsBox.HeaderColumn.ConnectedBy.Title",
        defaultMessage: "Liittäjä"
    },
    confirmDialogAttachChannel: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.AttachChannel",
        defaultMessage: "Vahvista, että haluat liittää kanava palveluun?"
    },
    confirmDialogAttachAllChannels: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.AttachAllChannels",
        defaultMessage: "Vahvista, että haluat liittää kanavaa palveluun?"
    },
    confirmDialogRemoveChannel: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.RemoveChannel",
        defaultMessage: "Vahvista, että haluat poistaa kanava palveluun?"
    },
    confirmDialogRemoveAllChannels: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.RemoveAllChannels",
        defaultMessage: "Vahvista, että haluat poistaa kanava palveluun?"
    },
    confirmDialogAcceptButton: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.Button.Ok",
        defaultMessage: "Kyllä"
    },
    confirmDialogAcceptCancel: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.ConfirmDialog.Button.Cancel",
        defaultMessage: "Jatka muokkausta"
    },
    detailDialogHeaderTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.DetailDialog.Header.Title",
        defaultMessage: "Tarkemmat tiedot"
    },
    linkToAddRelationTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Add.Title",
        defaultMessage: "Lisää asiointikanavia"
    },
    linkToUpdateRelationTitle: {
        id: "Containers.Services.AddService.Step4.ConnectChannels.RelationLink.Update.Title",
        defaultMessage: "Katsele ja muokkaa liitoksia"
    },
});

const confirmationEventTypes =  { ADD_ALL_ITEMS: 'ADD_ALL_ITEMS',
                                  REMOVE_ALL_ITEMS: 'REMOVE_ALL_ITEMS'
                                };
                                
const maxChannelItemsCount = 20;

class ServiceStep4Container extends Component {

    static propTypes = {
        actions: PropTypes.object,
        intl: intlShape.isRequired
    };

    validators = [PTVValidatorTypes.IS_REQUIRED];

    constructor(props) {
        super(props);

        this.state = {
               isConfirmDialogVisible: false,
               confirmationEventType: null,
               confirmMessage: messages.confirmDialogDefaultMessage,
               rowId: null,
               selectedRow: null,
               isVisible: false,
            };

        this.handlePopupSearchInputDataChange = this.handlePopupSearchInputDataChange.bind(this);

        this.onRowSelect = this.onRowSelect.bind(this);
        this.onAllRowsSelect = this.onAllRowsSelect.bind(this);

        this.channelDetailIconFormatter = this.channelDetailIconFormatter.bind(this);
        this.attachedChannelRemoveIconFormatter = this.attachedChannelRemoveIconFormatter.bind(this);

        this.acceptConfirmDialogCallback = this.acceptConfirmDialogCallback.bind(this);
        this.closeConfirmDialogCallback = this.closeConfirmDialogCallback.bind(this);
        this.nameFormatter = this.nameFormatter.bind(this);
        //this.showNotification = this.showNotification.bind(this);
    }

    acceptConfirmDialogCallback(){
        
      const { searchedChannels, attachedChannels } = this.props;  

      switch (this.state.confirmationEventType) {
        case confirmationEventTypes.ADD_ALL_ITEMS:
        {            
            this.onAddAllAttachedChannels(searchedChannels, attachedChannels);            
            break;
        }
        case confirmationEventTypes.REMOVE_ALL_ITEMS:
        {
            this.onRemoveAllAttachedChannels(attachedChannels);
            break;
        }
        default:
        {
            break;
        }
      }

      this.closeConfirmDialogCallback();
    }

    closeConfirmDialogCallback(){
      this.setState({
               isConfirmDialogVisible: false,
               confirmationEventType: null,
               confirmMessage: null,
               rowId: null
      });
      //e.preventDefault();
    }

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onServiceInputChange(input, value, this.props.language, isSet);
    }


    handleDetailIconClick(row, event) {

        event.stopPropagation();
        this.setState({ selectedRow: row });
        this.setState({ isVisible: true });
    }

    onRowSelect(row, isSelected)
    {
        this.props.actions.onListChange("attachedChannels", row.id, this.props.language, isSelected);         
    }

    onAllRowsSelect(isSelected, rows)
    {
        const { formatMessage } = this.props.intl;
        const { searchedChannels, attachedChannels } = this.props;
        var actualItemsCount = searchedChannels.size;

        if (isSelected === true)
        {
            (actualItemsCount > maxChannelItemsCount)
                ? this.setState({ isConfirmDialogVisible: true,
                                  confirmationEventType: confirmationEventTypes.ADD_ALL_ITEMS,
                                  confirmMessage: messages.confirmDialogAttachAllChannels})
                : this.onAddAllAttachedChannels(rows, attachedChannels);
        }
        else
        {
            (actualItemsCount >  maxChannelItemsCount)
                ? this.setState({
                            isConfirmDialogVisible: true,
                            confirmationEventType: confirmationEventTypes.REMOVE_ALL_ITEMS,
                            confirmMessage: messages.confirmDialogRemoveAllChannels
                          })
                : this.onRemoveAllAttachedChannels(attachedChannels);
        }
    }
    
    onAddAllAttachedChannels(allRows, selectedRows) {                                   
        allRows.forEach((row) => 
        {   
            if (!selectedRows.includes(row.id))
            {
                this.props.actions.onListChange("attachedChannels", row.id, this.props.language, true);                   
            }            
        });        
    }  
    
    onRemoveAllAttachedChannels(selectedRows)
    {        
        selectedRows.forEach((id) => 
        {   
            this.props.actions.onListChange("attachedChannels", id, this.props.language, false);                    
        });        
    } 

    handlePopupSearchInputDataChange(value)
    {
        this.props.actions.loadChannelSearchData(value, this.props.language);
    }

    channelDetailIconFormatter(cell,row){
        return <PTVIcon name="icon-info" className="brand" onClick={(e) => this.handleDetailIconClick(row, e)} />
    }

    tooltipDataFormatter(cell,row){
        return (
            <PTVTooltip
                labelContent = { cell }
                tooltip = { cell }
                type = 'special'
                attachToBody
            />
       );
    }

    nameFormatter(cell, row) {
        return (<TableNameFormatter content={ cell } language= { this.props.language }/>);
    }

    attachedChannelRemoveIconFormatter(cell,row){
        return ( 
                   <ButtonDelete onClick={() => this.props.actions.onListChange("attachedChannels", row.id, this.props.language, false)}
                                   secondary={true}
                                   withIcon={true}
                                   iconName="icon-times-circle" />  
              );
    }

    typeFormatter = (cell, row) => {
        const {formatMessage} = this.props.intl;
        if (cell) {
            return formatMessage({ id: cell.id, defaultMessage: cell.name});
        }
        return '';
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
    
    handleRelationClick = () => {
        this.props.actions.onServiceSearchListRemove();
        this.props.actions.onChannelSearchListRemove();
        this.props.actions.onServiceAndChannelsListRemove();    
        this.props.actions.loadService(this.props.entityId, this.props.language);
        browserHistory.push({ pathname : '/relations', state: { serviceId : this.props.entityId}});
    }

    step4ServiceChannelDataColumnsDefinition = [
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {dataField:"name", dataSort:true, dataFormat:this.nameFormatter.bind(this), columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderNameTitle)},
        {dataField:"type", dataFormat:this.typeFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderChannelTypeTitle)},
        {dataField:"modified", dataFormat: this.dateTimeFormater, columnHeaderTitle:this.props.intl.formatMessage(messages.attachedChannelsBoxTableHeaderConnectedTitle)},
        {dataField:"modifiedBy", columnHeaderTitle:this.props.intl.formatMessage(messages.attachedChannelsBoxTableHeaderConnectedByTitle)},
        {dataFormat:this.channelDetailIconFormatter.bind(this), columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderDetailTitle)},
        {dataFormat:this.attachedChannelRemoveIconFormatter.bind(this), width:"50", hidden: this.props.readOnly}
    ];

    step4ChannelSearchResultsColumnsDefinition = [
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {dataField:"name", dataSort:true, dataFormat:this.nameFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderNameTitle)},
        {dataField:"type", dataFormat:this.typeFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderChannelTypeTitle)},
        {dataFormat:this.channelDetailIconFormatter.bind(this), columnHeaderTitle:this.props.intl.formatMessage(messages.resultTableHeaderDetailTitle)}
    ];
    
    onDetailClose = () =>{
         this.setState({ isVisible: false });
    }

    render() {

        const {formatMessage} = this.props.intl;
        const {
            channelName, organizations, organizationId,
            searchedChannels, searchedChannelsAreDataValid, searchedChannelsIsFetching, searchedChannelsIsMoreThanMax, searchedChannelsCount,
            attachedChannels, attachedChannelsJS, language, splitContainer,
            readOnly, translationMode
            } = this.props;   
		       
        var selectedChannelsSelectRowProp = {
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted",
            hideSelectColumn : false
        }

        var channelsSelectRowProp = {
            mode: "checkbox",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted row-select",
            hideSelectColumn : false,
            // className: "row-select",
            onSelect: this.onRowSelect,
            onSelectAll: this.onAllRowsSelect,
            selected: attachedChannelsJS 
        }

        var detailDialogStyles = {
            width: '400px',
            height: '400px',
            position: 'fixed',
            top: '50%',
            left: '50%',
            marginTop: '-300px',
            marginLeft: '-10%',
            backgroundColor: '#fff',
            borderRadius: '2px',
            zIndex: 110,
            padding: '15px',
            boxShadow: '0 0 4px rgba(0,0,0,.14),0 4px 8px rgba(0,0,0,.28)'            
        };

        var detailOverlayStyles = {
            zIndex: 109
        };

        var detailContentStyles = {
           overflowY: ''
        };

        var moreChannelsAvailableMessage =  searchedChannelsIsMoreThanMax ? formatMessage(messages.resultTableMoreChannelsAvailableMessage) : '';
        const resultTableHeaderNameTitle = formatMessage(messages.resultTableHeaderNameTitle);
        const resultTableHeaderChannelTypeTitle = formatMessage(messages.resultTableHeaderChannelTypeTitle);
        const resultTableHeaderDetailTitle = formatMessage(messages.resultTableHeaderDetailTitle);
        const attachedChannelsBoxTableHeaderConnectedTitle = formatMessage(messages.attachedChannelsBoxTableHeaderConnectedTitle);

        return (
            <div className="step-4">  

                <div>
                   <PTVConfirmDialog
                      show={this.state.isConfirmDialogVisible}
                      acceptCallback= {this.acceptConfirmDialogCallback}
                      cancelCallback ={this.closeConfirmDialogCallback}
                      text= { this.state.confirmMessage ? formatMessage(this.state.confirmMessage) : ''}
                      acceptButton ={formatMessage(messages.confirmDialogAcceptButton)}
                      cancelButton = { formatMessage(messages.confirmDialogAcceptCancel) }
                     />

                    <PTVOverlay
                        ref="detailInfo"
                        title= {formatMessage(messages.detailDialogHeaderTitle)}
                        dialogStyles= { detailDialogStyles }
                        overlayStyles = { detailOverlayStyles }
                        contentStyles = { detailContentStyles }
                        isVisible = { this.state.isVisible }
                        onCloseClicked = { this.onDetailClose }
                        >
                        {this.state.selectedRow ? (<div>
                            <h3>{resultTableHeaderNameTitle}</h3>
                            <p>{this.state.selectedRow.name}</p>
                            <h3>{resultTableHeaderChannelTypeTitle}</h3>
                            <p>{this.typeFormatter(this.state.selectedRow.type)}</p>
                            <h3>{attachedChannelsBoxTableHeaderConnectedTitle}</h3>
                            <p>{ getDateTimeToDisplay(this.state.selectedRow.modified)}</p>
                            <h3>{formatMessage(messages.attachedChannelsBoxTableHeaderConnectedByTitle)}</h3>
                            <p>{this.state.selectedRow.modifiedBy}</p>
                            </div>) : null }
                    </PTVOverlay>
                    <LanguageLabel
                        language = { language }
                        splitContainer = { splitContainer }
                    />
                    <div className="row form-group">
                        <div className="col-xs-12">
                            {attachedChannels && attachedChannels.size > 0 ?
                                <div>
                                    <div className="row">
                                        <div className="col-lg-8">
                                            <PTVLabel>{formatMessage(messages.attachedChannelsBoxTitle,{count:attachedChannels.size} )}</PTVLabel>
                                        </div>
                                        <div className="col-lg-4">
                                            <PTVButton className="right" onClick={this.handleRelationClick}>
                                                {formatMessage(messages.linkToUpdateRelationTitle)}
                                            </PTVButton>
                                        </div>
                                    </div>
                                    <PTVTable
                                        contentDataSlector= { CommonChannelsSelectors.getChannels }
                                        maxHeight="280px"
                                        data= { attachedChannels }
                                        striped={true} hover={true}
                                        pagination={false}
                                        language= { language }
                                        selectRow={selectedChannelsSelectRowProp}
                                        columnsDefinition={this.step4ServiceChannelDataColumnsDefinition} />
                                </div>
                            : 
                                <div className="row"> 
                                    <div className="col-lg-8">
                                        <PTVLabel>{formatMessage(messages.attachedChannelsBoxNoDataDescription )}</PTVLabel>
                                    </div>
                                    <div className="col-lg-4">
                                        <PTVButton className="right" onClick={this.handleRelationClick}>
                                            {formatMessage(messages.linkToAddRelationTitle)}
                                        </PTVButton>
                                    </div>
                                </div>
                            }
                        </div>
                    </div>
                                                                  
         
                </div>                            
            
                {!(readOnly || translationMode == "view" || translationMode == "edit")?
                <div>  
                    <div className="row form-group">
                        <div className="col-sm-4 col-md-3 col-lg-2">
                            <PTVLabel
                                tooltip={formatMessage(messages.searchChannelNameTooltip)} >
                                {formatMessage(messages.searchChannelNameTitle)}
                            </PTVLabel>
                        </div>
                        
                        <div className="col-sm-8 col-md-9 col-lg-10">
                            <PTVPopupSearchInput
                                inputChangeCallback = { this.handlePopupSearchInputDataChange }
                                inputPlaceholder = {formatMessage(messages.searchChannelNamePlaceholder)}
                                inputValue = { channelName }
                                >
                                <div className="row">
                                    <div className="col-xs-12">
                                        {searchedChannelsIsFetching ? <PTVPreloader /> : <div>
                                        {searchedChannels && searchedChannels.size > 0 ?
                                            <div>
                                                <PTVLabel labelClass="strong">
                                                    {formatMessage(messages.resultTableDescriptionResultCount)} {       
                                                    searchedChannelsCount}
                                                    &nbsp; &nbsp;
                                                    {moreChannelsAvailableMessage}
                                                </PTVLabel>
                                                                                                
                                                <PTVTable
                                                    contentDataSlector= { CommonChannelsSelectors.getChannels }
                                                    maxHeight="280px"
                                                    data={searchedChannels}
                                                    striped={true}
                                                    language= { language }
                                                    selectRow={channelsSelectRowProp}
                                                    tableBodyClass="selectable"
                                                    tableHeaderClass="selectable"
                                                    columnsDefinition={this.step4ChannelSearchResultsColumnsDefinition}
                                                />
                                            </div>
                                        : <div> {formatMessage(messages.resultTableNoResultsFound)} </div>
                                        } </div> }
                                    </div>
                                </div>
                            </PTVPopupSearchInput>
                        </div>
                    </div>                      
         
                    <div className="row form-group">
                        <Organizations
                            value={ organizationId }
                            id="serviceChannelOrganizations"
                            label={formatMessage(messages.searchBoxOrganizationTitle)}
                            tooltip={formatMessage(messages.searchBoxOrganizationTooltip)}
                            labelClass="col-sm-4 col-md-3 col-lg-2"
                            autoClass="col-sm-8 col-md-9 col-lg-10"
                            name='searchBoxOrganization'
                            changeCallback={ this.onInputChange('serachOrganizationId') }
                            virtualized= { true }
                            className="limited w480"  />
                    </div>       
            
                    <div className="row">                               
                        <ChannelTypes
                            labelClass="col-sm-4 col-md-3 col-lg-2"
                            autoClass="col-sm-8 col-md-9 col-lg-10"
                            id= "serviceChannelTypes"
                            label={messages.searchBoxChannelTypeTitle}
                            tooltip={messages.searchBoxChannelTypeTooltip}
                            placeholder= {messages.searchBoxChannelTypeTooltip}
                            changeCallback= {this.onInputChange('searchChannelTypes', true)}
                            order= {40}
                            selector= { ServiceSelectors.getSelectedChannelTypesItemsJS }
                            useFormatMessageData = { true }
                            readOnly= { readOnly }
                            language = { language }
                        />      
                    </div>  
                </div>:null}        
            </div>
       );
    }
}

function mapStateToProps(state, ownProps) {
  return {
      
    channelName: ServiceSelectors.getChannelName(state, ownProps),    
    organizations: getOrganizationsJS(state, ownProps),    
    organizationId: ServiceSelectors.getOrganizationId(state, ownProps),   
    selectedChannelTypesJS: ServiceSelectors.getSelectedChannelTypesJS(state, ownProps),    
    searchedChannels: ServiceSelectors.getStep4SearchedChannels(state, { keyToState: 'service'} ),
    searchedChannelsAreDataValid: CommonSelectors.getStepInnerSearchAreDataValid(state, { keyToState: 'service' }),
    searchedChannelsIsFetching: CommonSelectors.getStepInnerSearchIsFetching(state, { keyToState: 'service' }),
    searchedChannelsIsMoreThanMax: ServiceSelectors.getStep4SearchedIsMoreThanMax(state, { keyToState: 'service' }),
    searchedChannelsCount: ServiceSelectors.getStep4SearchedIsNumberOfAllItems(state, { keyToState: 'service' }),
    attachedChannels: ServiceSelectors.getSelectedAttachedChannels(state, ownProps),  
    attachedChannelsJS: ServiceSelectors.getSelectedAttachedChannelsJS(state, ownProps)      
   }
}

const actions = [
    serviceStep4Actions,
    mainActions,
    relationServiceSearchActions,
    commonServiceAndChannelActions    
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceStep4Container));

