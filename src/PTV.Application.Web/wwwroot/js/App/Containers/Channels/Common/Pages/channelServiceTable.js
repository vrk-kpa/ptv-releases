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
import shortId from 'shortid';
import { getDateTimeToDisplay } from '../../../../Components/PTVDateTimePicker/PTVDateTimePicker';

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps';

// components
import { PTVTable, PTVTooltip, PTVTranslationText } from '../../../../Components';
import TableNameFormatter from '../../../Common/tableNameFormatter';

// selectors
import * as ServiceSelectors from '../../../Services/Service/Selectors';
import * as ChannelCommonSelector from '../../Common/Selectors';


export const channelServiceTable = ({ messages, language, intl, attachedServices }) => {

   const typeFormatter = (cell, row) =>{
        return  <PTVTranslationText  
                    id= { row.serviceTypeId } 
                    name= { row.serviceType }
                />
        
  }
   const  dateTimeFormater = (cell,row) =>{
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

   const nameFormatter = (cell, row) => {
        return (<TableNameFormatter content={ cell } language= { language }/>);
    }

   const generalDataFormatter = (cell,row) =>{
       return (
           <PTVTooltip
                labelContent = { cell }
                tooltip = { cell ? cell : null }
                type = 'special'
                attachToBody
            />
       );
    }

    const tooltipDataFormatter = (cell,row) =>{
        return (
            <PTVTooltip
                labelContent = { cell }
                tooltip = { cell }
                type = 'special'
                attachToBody
            />
       );
    }

    const ChannelServiceTableColumnsDefinition = [
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {dataField:"name", dataSort:true, dataFormat:nameFormatter, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderNameTitle)},
        {dataField:"type", dataFormat:typeFormatter, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderTypeTitle)},
        {dataField:"modified", dataFormat: dateTimeFormater, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderAttachedTitle)},
        {dataField:"modifiedBy", dataFormat: generalDataFormatter, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderAttachedByTitle)}
    ];

     const selectedChannelServiceSelectRowProp = {
            clickToSelect: true,
            className: "highlighted",
            hideSelectColumn : false
    }

    return (
            <div>           
                <PTVTable
                    contentDataSlector= { ServiceSelectors.getServices }
                    maxHeight="280px"
                    data= { attachedServices }
                    striped={true} hover={true}
                    pagination={false}
                    language={ language }
                    selectRow={selectedChannelServiceSelectRowProp}
                    columnsDefinition={ChannelServiceTableColumnsDefinition} />
            </div> 
       );
}

channelServiceTable.propTypes = {
    messages: PropTypes.object.isRequired,
};

function mapStateToProps(state, ownProps) {
  return { 
      attachedServices: ChannelCommonSelector.getConnectedServices(state, ownProps)  
  }
}

const actions = [    
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(channelServiceTable));