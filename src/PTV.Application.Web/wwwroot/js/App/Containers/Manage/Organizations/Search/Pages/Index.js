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
import { connect } from 'react-redux';
import { browserHistory } from 'react-router';
import shortId from 'shortid';

// Actions
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';
import * as CommonOrganizationActions from '../../Common/Actions';
import * as organizationsActions from '../Actions';
import * as appActions from '../../../../Common/Actions';

// Components
import { PTVHeaderSection, PTVPreloader, PTVTextInput, PTVTable, PTVLabel, PTVAutoComboBox, PTVTooltip } from '../../../../../Components';
import { ButtonSearch, ButtonShowMore } from '../../../../Common/Buttons';
import commonMessages from '../../../../Common/LocalizedMessages';
import { getDateTimeToDisplay } from '../../../../../Components/PTVDateTimePicker/PTVDateTimePicker';
import PublishingStatusContainer from '../../../../Common/PublishStatusContainer';
import { ColumnFormatter } from '../../../../Common/PublishStatusContainer';
import Organizations from '../../../../Common/organizations';

// selectors
import * as CommonOrganizationSelectors from '../../Common/Selectors';
import * as OrganizationSelectors from '../Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

const messages = defineMessages({
    headerTitle: {
        id: "Containers.Manage.Organizations.Search.Header.Title",
        defaultMessage: "Organisaatiot"
    },
    headerDescription: {
        id: "Containers.Manage.Organizations.Search.Header.Description",
        defaultMessage: "Organisaatiot-osiossa organisaation ylläpitäjä voi selailla oman organisaation tietoja ja organisaation pääkäyttäjä voi ylläpitää organisaation tietoja ja organisaatiorakennetta."
    },
    headerAddOrganizationButton: {
        id: "Containers.Manage.Organizations.Search.Header.Button.Add",
        defaultMessage: "Lisää organisaatio"
    },
    searchBoxHeaderTitle: {
        id: "Containers.Manage.Organizations.Search.SearchBox.Header.Title",
        defaultMessage: "Hae organisaatioita"
    },
    searchBoxHeaderNamTitle: {
        id: "Containers.Manage.Organizations.Search.SearchBox.Header.Name.Title",
        defaultMessage: "Nimi"
    },
    searchBoxHeaderNamePlaceholder: {
        id: "Containers.Manage.Organizations.Search.SearchBox.Header.Name.Placeholder",
        defaultMessage: "Hae organisaation tai alaorganisaation nimellä"
    },
    organizationSearchResultHeaderName: {
        id: "Containers.Manage.Organizations.SearchResult.Header.Name",
        defaultMessage: "Nimi"
    },
    organizationSearchResultHeaderMainOrganizations: {
        id: "Containers.Manage.Organizations.SearchResult.Header.MainOrganizations",
        defaultMessage: "Pääorganisaatio"
    },
    organizationSearchResultHeaderSubOrganizations: {
        id: "Containers.Manage.Organizations.SearchResult.Header.SubOrganizations",
        defaultMessage: "Alaorganisaation näyttäminen"
    },
    organizationSearchResultHeaderEdited: {
        id: "Containers.Manage.Organizations.SearchResult.Header.Edited",
        defaultMessage: "Muokattu"
    },
    organizationSearchResultHeaderModifier: {
        id: "Containers.Manage.Organizations.SearchResult.Header.Modifier",
        defaultMessage: "Muokkaaja"
    },
    organizationResultCount: {
        id: "Containers.Manage.Organizations.SearchResult.Count.Title",
        defaultMessage: "Hakutuloksia: "
    },
    organizationResultMoreThanMax: {
        id: "Containers.Manage.Organizations.SearchResult.Count.Description.Title",
        defaultMessage: "There is more than already shown results. Please be more descriptive in the criteria."
    },
    organizationComboTitle: {
        id: "Containers.Manage.Organizations.Search.Organization.Title",
        defaultMessage: "Organisaatio"
    },
    organizationComboTooltip: {
        id: "Containers.Manage.Organizations.Search.Organization.Tooltip",
        defaultMessage: "Valitse pudotusvalikosta haluamasi organisaatio tai organisaatiotaso."
    },
    publishingStatusTooltip: {
        id: "Containers.Manage.Organizations.Search.PublishingStatus.Tooltip",
        defaultMessage: "Voit hakea luonnostilassa olevia tai julkaistuja palveluja. Luonnostilassa olevat palvelut näkyvät vain ylläpitäjille palvelutietovarannossa."
    },
    organizationSearchResultHeaderPublishingStatus: {
        id: "Containers.Manage.Organizations.SearchResult.Header.PublishingStatus",
        defaultMessage: "Tila"
    },
});

class SearchOrganizationContainer extends Component {

    constructor(props) {
        super(props);
    }

    componentDidMount() {
        const { areDataValid } = this.props;
        if (!areDataValid) {
            this.props.actions.loadOrganizationSearch();
        }
    }

    onInputChange = (input, isSet=false) => value => {
        this.props.actions.onOrganizationSearchInputChange(input, value, isSet);
    }
    
    onListChange = (input) => (value, isAdd) => {
        this.props.actions.onOrganizationSearchListChange(input, value, isAdd);
    }

    handleSubmit = () => {
        this.props.actions.loadOrganization();
    }

    handleShowMore = () => {
        this.props.actions.loadOrganization(true);
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

    onRowSelect = (row, isSelected) => {
        this.props.actions.setOrganizationId(row.id);
        browserHistory.push({ pathname :'/manage/manage/organizations'});
    }

    fromatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;
    }

    selectRowProp = {
            mode: "radio",
            clickToSelect: true,
            //bgColor: "rgb(238, 193, 213)",
            className: "highlighted",
            onSelect: this.onRowSelect,
            hideSelectColumn : true
        }

    fromatPublishingStatus = (cell,row) => {
        return <ColumnFormatter cell={cell} row={row} />;
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

    columnsDefinition = [
        {width:"70", dataField:"publishingStatusId", dataFormat: this.fromatPublishingStatus, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderPublishingStatus)},
        {dataField:"id", isKey:true, hidden:true, columnHeaderTitle:'ID'},
        {dataField:"name", dataSort:true, dataFormat:this.linkFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderName)},
        {dataField:"mainOrganization", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderMainOrganizations)},
        {dataField:"subOrganizations", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderSubOrganizations)},
        {dataField:"modified", dataFormat: this.dateTimeFormater, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderEdited)},
        {dataField:"modifiedBy", dataFormat:this.generalDataFormatter, columnHeaderTitle:this.props.intl.formatMessage(messages.organizationSearchResultHeaderModifier)}
    ];

    onAddButtonClick = () => {
        browserHistory.push({ pathname :'/manage/manage/organizations'});
        this.props.actions.setOrganizationId(shortId.generate());
    }

    render() {
        const {formatMessage} = this.props.intl;
        const { organizationName, pageNumber, searchMaxPageCount, keyToState, organizationId, selectedPublishingStatuses, isFetching, areDataValid, searchedOrganizations, organizationsCount, searchIsFetching, serachAreDataValid, searchIsMoreThanMax } = this.props;
        
        return (<div>
            <PTVHeaderSection
                titleClass = "card-title"
                title= {formatMessage(messages.headerTitle)}
                descriptionClass = "lead"
                description = {formatMessage(messages.headerDescription)}
                buttonText = {formatMessage(messages.headerAddOrganizationButton)}
                buttonClick = {this.onAddButtonClick}
                buttonRoute = ""
                buttonDisabled= { false }
            />

            <div className="box box-white">
                <div className="form-wrap">
                    { isFetching ? <PTVPreloader /> :
                    <form className="form-inline  service-search" role="form">
                        <h2><FormattedMessage id="Containers.Manage.Organizations.Search.SearchBox.Header.Title" /></h2>
                        <div className="row form-group">
                            <div className="col-lg-6">
                                <PTVTextInput
                                    value={organizationName}
                                    id="1"
                                    placeholder={formatMessage(messages.searchBoxHeaderNamePlaceholder)}
                                    label={formatMessage(messages.searchBoxHeaderNamTitle)}
                                    labelClass="col-sm-4"
                                    inputclass="col-sm-8"
                                    changeCallback={this.onInputChange('organizationName')}
                                    onEnterCallBack={this.handleSubmit}
                                    name='organizationName'
                                    maxLength={100}
                                    componentClass="row"
                                />
                            </div>
                            <div className="col-lg-6">
                                    <Organizations
                                        value={ organizationId }
                                        id="4"
                                        label={formatMessage(messages.organizationComboTitle)}
                                        tooltip={formatMessage(messages.organizationComboTooltip)}
                                        labelClass="col-sm-4"
                                        autoClass="col-sm-8"
                                        name='organizationId'
                                        changeCallback={this.onInputChange('organizationId')}
                                        componentClass="row"
                                        className="limited w320"
                                        inputProps= { {'maxLength': '100'} } />
                                </div>
                        </div>
                        <div className="row form-group">
                            <PublishingStatusContainer onChangeBoxCallBack={this.onListChange('selectedPublishingStatuses')}
                                tooltip= { formatMessage(messages.publishingStatusTooltip) }
                                isSelectedSelector= { OrganizationSelectors.getIsSelectedPublishingStatus } 
                                containerClass="col-xs-12"
                                labelClass="col-sm-4 col-lg-2"
                                contentClass="col-sm-8 col-lg-10"
                                keyToState= { keyToState } />
                        </div>
                        <div className="button-group">
                            <ButtonSearch onClick={this.handleSubmit}/>
                        </div>
                        { searchedOrganizations.size > 0 ?
                            <div>
                            { searchIsMoreThanMax ? <PTVLabel labelClass="main">{formatMessage(messages.organizationResultCount) + (pageNumber*searchMaxPageCount) + '/' + organizationsCount}</PTVLabel> 
                                : <PTVLabel labelClass="main">{formatMessage(messages.organizationResultCount) + organizationsCount}</PTVLabel>}
                                
                                <PTVTable
                                    contentDataSlector= { CommonOrganizationSelectors.getOrganizations }
                                    data= { searchedOrganizations }
                                    striped={true}
                                    hover={true}
                                    selectRow={this.selectRowProp}
                                    columnsDefinition={this.columnsDefinition} />
                                { searchIsMoreThanMax ? <div>
                                    <div className='button-group centered'>{ !searchIsFetching ? <ButtonShowMore onClick={this.handleShowMore}/> : <PTVPreloader />}</div>
                                    <PTVLabel labelClass="main">{formatMessage(messages.organizationResultCount) + (pageNumber*searchMaxPageCount) + '/' + organizationsCount}</PTVLabel>
                                    </div> : <PTVLabel labelClass="main">{formatMessage(messages.organizationResultCount) + organizationsCount}</PTVLabel>}
                            </div>
                        : searchedOrganizations.size == 0 && !searchIsFetching && serachAreDataValid ?
                            <PTVLabel labelClass="main"> {formatMessage(commonMessages.noSeachResults)} </PTVLabel>
                        : searchIsFetching ?
                            <PTVPreloader />
                        : null}
                    </form> }
                    <div className="clearfix"></div>
                </div>
            </div>
         </div>);
    }
}

SearchOrganizationContainer.defaultProps = {
        keyToState: 'organization',
    };

function mapStateToProps(state, ownProps) {
    const props = { keyToState: 'organization', keyToEntities: 'organizations' };
  return {
    organizationName: OrganizationSelectors.getOrganizationName(state),
    organizationId: OrganizationSelectors.getOrganizationId(state),
    selectedPublishingStatuses: OrganizationSelectors.getSelectedPublishingStauses(state),
    isFetching: CommonSelectors.getSearchIsFetching(state, props),
    areDataValid: CommonSelectors.getSearchAreDataValid(state, props),
    searchedOrganizations: CommonSelectors.getSearchedEntities(state, props),
    organizationsCount: CommonSelectors.getSearchResultsCount(state, props),
    searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
    serachAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
    searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
    searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
    pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
  }
}

const actions = [
    CommonOrganizationActions,
    organizationsActions,
    appActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SearchOrganizationContainer));
