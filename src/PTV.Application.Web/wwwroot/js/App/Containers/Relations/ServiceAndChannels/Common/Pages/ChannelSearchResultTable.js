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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import shortId from 'shortid';
import { camelize } from 'humps';

//Actions
import * as commonServiceAndChannelActions from '../../Common/Actions'
import * as channelSearchActions from '../../ChannelSearch/Actions';

// components
import { PTVLabel, PTVTable, PTVPreloader, PTVOverlay, PTVIcon } from '../../../../../Components';
import { ButtonShowMore } from '../../../../Common/Buttons';
import { ColumnFormatter } from '../../../../Common/PublishStatusContainer';
import { getDateTimeToDisplay } from '../../../../../Components/PTVDateTimePicker/PTVDateTimePicker';
import ElectronicContainer from '../../../../Channels/Electronic/Pages';
import PhoneContainer from '../../../../Channels/Phone/Pages';
import PrintableFormContainer from '../../../../Channels/PrintableForm/Pages';
import ServiceLocationContainer from '../../../../Channels/ServiceLocation/Pages';
import WebPageContainer from '../../../../Channels/WebPage/Pages';
import TableNameFormatter from '../../../../Common/tableNameFormatter';

// selectors
import * as ChannelSearchSelectors from '../../ChannelSearch/Selectors';
import { getChannels } from '../../../../Channels/Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';
import * as CommonServiceAndChannelSelectors from '../../Common/Selectors';

// messages
import commonMessages from '../../../../Common/LocalizedMessages';
import { detailMessages } from '../../ChannelSearch/Messages';

// styles
import * as overlayStyles from './OverlayStyles';

// types
import { channelTypes } from './../../../../Common/Enums';

export const ChannelSearchResultTable =  ({messages, searchedChannels, searchIsFetching, searchAreDataValid, searchChannelsCount, keyToState, pageNumber, searchMaxPageCount, searchIsMoreThanMax, searchIsMoreAvailable,
      newSelectedChannelsIds, channelRelationsIds, connectedServicesIds, connectedServiceIdsMap, connectedChannelRelations, detailRow, intl, actions, language }) => {

    const { formatMessage } = intl;

    const handleShowMore = () => {
        actions.loadChannelSearchResults(true);
    }

    const fromatPublishingStatus = (cell,row) => {
            return <ColumnFormatter cell={cell} row={row} />;
    }

    const generalFormatter = (cell,row) => <span>{ cell }</span>;

    const nameFormatter = (cell, row) => {
        return (<TableNameFormatter content={ cell } language= { language }/>);
    }

    const onAddChannel = (entity) => {

        connectedServicesIds.map(serviceId => {
            let createNewRelation = true;

            connectedChannelRelations
                .filter(chr => chr.get('service') === serviceId)
                .map(chr =>
                {
                    if (chr.get('connectedChannel') === entity.id || chr.get('channelRootId') === entity.rootId)
                    {
                        createNewRelation = false;
                    }
                })

            if (createNewRelation === true) //Add only nonconnected channels
            {
                actions.onConnectedChannelEntityAdd(entity, serviceId, connectedServiceIdsMap.get(serviceId))
            }

        })
    };

    const onRemoveChannel = (id) => {

        connectedChannelRelations
            .filter(chr => chr.get('isNew') === true)
            .map(channelRelation =>
            {
                const serviceId = channelRelation.get('service');
                const relationChannelId = channelRelation.get('id');
                if (channelRelation.get('connectedChannel') === id)
                {
                    actions.onConnectedChannelListChange(connectedServiceIdsMap.get(serviceId), relationChannelId);
                }
            }
        )
    }

    const onRowSelect = (row, isSelected) => {
        if (isSelected === true){ //Add
            onAddChannel(row);
        }
        else{ //Remove
            onRemoveChannel(row.id);
        }
    };

    const onAllRowsSelect = (isSelected, rows) =>{
        rows.forEach(row => onRowSelect(row,isSelected));
    }

    const handleDetailIconClick = (row, event) =>{
        event.stopPropagation();
        actions.setRelationsDetail(row.id, keyToState, camelize(row.subEntityType));
    }

    const onDetailClose = (typeCode) =>{
         actions.setRelationsDetail(null, keyToState,  camelize(typeCode));
    }

    const channelDetailIconFormatter = (cell,row) =>{
        return  <PTVIcon
                    onClick={(e) => handleDetailIconClick(row, e)}
                    name = "icon-info-circle"/>
    }

    const renderChannelSimpleView = (channelType) =>{
        switch(camelize(channelType)){
            case channelTypes.ELECTRONIC: return <ElectronicContainer simpleView = { true }/>;
            case channelTypes.PHONE: return <PhoneContainer simpleView = { true }/>;
            case channelTypes.PRINTABLE_FORM: return <PrintableFormContainer simpleView = { true }/>;
            case channelTypes.SERVICE_LOCATION: return <ServiceLocationContainer simpleView = { true }/>;
            case channelTypes.WEB_PAGE: return <WebPageContainer simpleView = { true }/>;
            default:return null;
        }
    }

    let selectRowProp = {
            mode: "checkbox",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted row-select",
            hideSelectColumn : false,
            // className: "row-select",
            onSelect: onRowSelect,
            onSelectAll: onAllRowsSelect,
            selected: newSelectedChannelsIds
    };
    const resultChannelsColumnsDefinition = [
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {width:"50", dataField:"publishingStatusId", dataFormat: fromatPublishingStatus, columnHeaderTitle:formatMessage(messages.channelResultTableHeaderPublishingStatusTitle)},
        {dataField:"name", dataSort:true, dataFormat:nameFormatter, columnHeaderTitle:formatMessage(messages.channelResultTableHeaderNameTitle), columnClassName:"brand-color"},
        {width:"30", dataFormat:channelDetailIconFormatter},
    ];

    return (
           <div className="form-group">
           <PTVOverlay
                title= {formatMessage(detailMessages.channelDetailTitle)}
                dialogStyles= { overlayStyles.detailDialogStyles }
                overlayStyles = { overlayStyles.detailOverlayStyles }
                contentStyles = { overlayStyles.detailContentStyles }
                isVisible = { detailRow.size != 0 }
                onCloseClicked = { () => onDetailClose(detailRow.get('subEntityType')) }
                >
                {detailRow.size != 0 ?  renderChannelSimpleView(detailRow.get('subEntityType')) : null }
            </PTVOverlay>
               {searchedChannels.size > 0 ?
                    <div>
                    {/*{searchIsMoreThanMax ? <PTVLabel labelClass="strong">{formatMessage(messages.channelResultTableDescriptionResultCount) + (pageNumber*searchMaxPageCount) + '/' + searchChannelsCount}</PTVLabel>
                        : <PTVLabel labelClass="strong">{formatMessage(messages.channelResultTableDescriptionResultCount) + searchChannelsCount}</PTVLabel>}*/}
                    <PTVTable maxHeight="450px"
                        contentDataSlector= { getChannels }
                        data= { searchedChannels }
                        striped={true}
                        hover={true}
                        pagination={false}
                        selectRow={selectRowProp}
                        tableHeaderClass="small row-select"
                        tableBodyClass="small row-select"
                        columnsDefinition={resultChannelsColumnsDefinition}
                        language = { language }
                    />
                    {searchIsMoreAvailable ? <div>
                                            <div className='button-group centered'>{ !searchIsFetching ? <ButtonShowMore onClick={handleShowMore}/> : <PTVPreloader />}</div>
                                            {/*<PTVLabel labelClass="strong">{formatMessage(messages.channelResultTableDescriptionResultCount) + (pageNumber*searchMaxPageCount) + '/' + searchChannelsCount}</PTVLabel>*/}
                                            </div> : null}
                        {/*<PTVLabel labelClass="strong">{formatMessage(messages.channelResultTableDescriptionResultCount) + searchChannelsCount}</PTVLabel>}*/}
                    </div>
                    : searchedChannels.size == 0 && !searchIsFetching && searchAreDataValid ?
                        <PTVLabel labelClass="strong"> {formatMessage(commonMessages.noSeachResults)} </PTVLabel>
                        : searchIsFetching ?
                            <PTVPreloader />
                            : null
                }

            </div>
       );
}

ChannelSearchResultTable.propTypes = {

};

function mapStateToProps(state, ownProps) {
    const props = {keyToState: 'serviceAndChannelChannelSearch', keyToEntities: 'channels'};
    const language = CommonSelectors.getLanguageToCode(state, props);
  return {
      searchedChannels: CommonSelectors.getSearchedEntities(state, props),
      searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
      searchAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
      searchChannelsCount: CommonSelectors.getSearchResultsCount(state, props),
      searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
      searchIsMoreAvailable: CommonSelectors.getSearchResultsIsMoreAvailable(state, props),
      pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
      searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
      newSelectedChannelsIds: CommonServiceAndChannelSelectors.getRelationNewConnectedChannelsIdsJS(state, props),
      connectedServicesIds: CommonServiceAndChannelSelectors.getRelationConnectedServicesIds(state, props),
      channelRelationsIds: CommonServiceAndChannelSelectors.getRelationChannelRelationsOfConnectedServiceIds(state, props),
      connectedServiceIdsMap: CommonServiceAndChannelSelectors.getRelationConnectedServicesIdsMap(state, props),
      connectedChannelRelations: CommonServiceAndChannelSelectors.getRelationConnectedChannelsWithServiceChannelEntities(state, props),
      detailRow:  CommonServiceAndChannelSelectors.getDetailChannel(state, {...props, language}),
      language: CommonSelectors.getLanguageToCode(state, props)
  }
}

const actions = [
    commonServiceAndChannelActions,
    channelSearchActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ChannelSearchResultTable));
