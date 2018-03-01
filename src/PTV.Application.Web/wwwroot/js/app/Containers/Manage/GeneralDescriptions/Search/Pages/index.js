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
import { connect } from 'react-redux';
import { injectIntl, FormattedMessage, FormattedHTMLMessage } from 'react-intl';

// styles
import '../../../../Services/Styles/LandingPage.scss';
import '../../../../Services/Styles/SearchGeneralDescriptionContainer.scss';

// actions
import * as descriptionActions from '../Actions';
import * as PageContainerActions from '../../../../Common/Actions/PageContainerActions';
import * as commonActions from '../../../../Common/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

// components
import { PTVTextInput, PTVButton, PTVLabel, PTVIcon, PTVCheckBox, PTVOverlay, PTVMessageBox, PTVPreloader, PTVTable, PTVHeaderSection, PTVTooltip, PTVTextEditorNotEmpty } from '../../../../../Components';
import { LocalizedComboBox, LocalizedTextList } from '../../../../Common/localizedData';
import { ButtonSearch, ButtonGoBack, ButtonShowMore } from '../../../../Common/Buttons';
import TextList from '../../../../Common/textList';

// messages
import commonMessages from '../../../../Common/LocalizedMessages';
import * as Messages from '../Messages';

// selectors
import * as GeneralDescriptionSelectors from '../Selectors';
import * as CommonServiceSelectors from '../../../../Services/Common/Selectors';
import * as CommonGeneralDescriptionSelectors from '../../Common/Selectors';
import * as CommonSelectors from '../../../../Common/Selectors';

const GeneralDescriptionPreviewRow = injectIntl(({text, title, intl, children}) => {
    if (!text && !children) {
        return null;
    }
    const render = () => {
        if (children){
            return children;
        }
        return text;
    }
    return (
        <div>
            <label><strong>{intl.formatMessage(title)}</strong></label>
            <div>{render()}</div>
        </div>
    );
});



const GeneralDescriptionPreview = ({
  name,
  alternateName,
  serviceClassesCaption,
  targetGroupsCaption,
  subTargetGroupsCaption,
  shortDescription,
  description,
  additionalInformationDeadLine,
  additionalInformationProcessingTime,
  additionalInformationValidityTime,
  backgroundDescription
}) => (
        <div>
            <GeneralDescriptionPreviewRow text={name} title={Messages.messages.detaileNameTitle} />
            <GeneralDescriptionPreviewRow text={alternateName} title={Messages.messages.detailAlternateNameTitle} />
            <GeneralDescriptionPreviewRow text={shortDescription} title={Messages.messages.detailShortDescriptionTitle} />
            <GeneralDescriptionPreviewRow title={Messages.messages.detailDescriptionTitle}>
              <PTVTextEditorNotEmpty
                value={description}
                name='statutoryDescription'
                readOnly
              />
            </GeneralDescriptionPreviewRow>
            <GeneralDescriptionPreviewRow text={additionalInformationDeadLine} title={Messages.messages.detailInfoDeadline} />
            <GeneralDescriptionPreviewRow text={additionalInformationProcessingTime} title={Messages.messages.detailInfoProcessingTime} />
            <GeneralDescriptionPreviewRow text={additionalInformationValidityTime} title={Messages.messages.detailInfoValidityTime} />
            <GeneralDescriptionPreviewRow title={Messages.messages.detailBackgroundDescriptionTitle}>
              <PTVTextEditorNotEmpty
                value={backgroundDescription}
                name='backgroundDescription'
                readOnly
              />
            </GeneralDescriptionPreviewRow>
            <GeneralDescriptionPreviewRow title={Messages.messages.detailTargetGroupTitle} >
                <LocalizedTextList
                    getListSelector={GeneralDescriptionSelectors.getSelectedGeneralDescriptionTargetGroupEntities}
                    property="name"
                    />
            </GeneralDescriptionPreviewRow>
            <GeneralDescriptionPreviewRow title={Messages.messages.detailSubTargetGroupTitle} >
                <LocalizedTextList
                    getListSelector={GeneralDescriptionSelectors.getSelectedGeneralDescriptionSubTargetGroupEntities}
                    property="name"
                    />
            </GeneralDescriptionPreviewRow>
            <GeneralDescriptionPreviewRow title={Messages.messages.detailServiceClassTitle} >
                <LocalizedTextList
                    getListSelector={GeneralDescriptionSelectors.getSelectedGeneralDescriptionServiceClassEntities}
                    property="name"
                    />
            </GeneralDescriptionPreviewRow>
            <GeneralDescriptionPreviewRow title={Messages.messages.detailOntologyTermTitle} >
                <LocalizedTextList
                    getListSelector={GeneralDescriptionSelectors.getSelectedGeneralDescriptionOntologyTermEntities}
                    property="name"
                    />
            </GeneralDescriptionPreviewRow>
        </div>
    );

function mapStateToPropsPreview(state, ownProps) {
    return {
        name: GeneralDescriptionSelectors.getSelectedGeneralDescriptionNameLocale(state),
        alternateName: GeneralDescriptionSelectors.getSelectedGeneralDescriptionAlternateNameLocale(state),
        shortDescription: GeneralDescriptionSelectors.getSelectedGeneralDescriptionShortDescriptionLocale(state),
        description: GeneralDescriptionSelectors.getSelectedGeneralDescriptionDescriptionLocale(state),
        additionalInformationDeadLine: GeneralDescriptionSelectors.getSelectedGeneralDescriptionDeadlineLocale(state),
        additionalInformationProcessingTime: GeneralDescriptionSelectors.getSelectedGeneralDescriptionProcessingTimeLocale(state),
        additionalInformationValidityTime: GeneralDescriptionSelectors.getSelectedGeneralDescriptionValidityTimeLocale(state),
        backgroundDescription: GeneralDescriptionSelectors.getSelectedGeneralDescriptionBackgroundDescriptionLocale(state)
    }
}

const ConnectedGeneralDescriptionPreview = connect(mapStateToPropsPreview)(GeneralDescriptionPreview);



class SearchGeneralDescriptionContainer extends Component {

    constructor(props) {
        super(props);

        this.state = {
            isVisible: false,
        };
    }

    handleTargetGroupChange = (groupId) => {
        this.props.actions.selectTargetGroup(groupId);
    }

    onInputChange = (input, isSet = false) => value => {
        this.props.actions.onGeneralDescriptionSearchInputChange(input, value, isSet);
    }

    handleServiceClassChange = (scId) => {
        this.props.actions.selectServiceClass(scId);
    }

    handleNameChange = (name) => {
        this.props.actions.onNameChange(name);
    }

    handleSubTargetGroupChange = (subGroupId) => {
        this.props.actions.selectSubTargetGroup(subGroupId);
    }

    componentDidMount() {
        this.props.actions.forceReload('service', false)
        const { areDataValid } = this.props;
        if (!areDataValid) {
            this.props.actions.loadGeneralDescriptions();
        }
    }

    handleSubmit = () => {
        this.props.actions.searchGeneralDescription();
    }

    handleShowMore = () => {
        this.props.actions.searchGeneralDescription(true);
    }

    handleSelection = () => {
        this.props.actions.onPageContainerObjectChange('serviceInfo', {serviceId: null, returnPath: null});
        this.props.actions.setGenerealDescriptionToService();
        browserHistory.push(this.props.selectToPath);
    }

    handleGoBack = () => {
        browserHistory.push(this.props.selectToPath);
    }

    handleAdd = () => {
        //browserHistory.push({ pathname :'/manage/manage/generalDescription'});
        //this.props.actions.setGeneralDescriptionId(shortId.generate());
    }

    onRowSelect = (row, isSelected) => {
        this.props.actions.getGeneralDescriptions(row.id);
    }

    linkFormatter = (cell, row) => {
        return (
            <div className='link-like'>
                <PTVTooltip
                    labelContent={cell}
                    tooltip={cell}
                    type='special'
                    attachToBody
                    />
            </div>
        );
    }

    generalDataFormatter = (cell,row) => {
      const content = typeof(cell) === 'string' ? cell : cell.fi || ''
       return (
           <PTVTooltip
                labelContent = { content }
                tooltip = { content ? content : null }
                type = 'special'
                attachToBody
            />
       );
    }

    serviceClassDataFormatter = (cell, row) => {
        return (
            <LocalizedTextList
                id={row.id}
                renderList={this.generalDataFormatter}
                getListSelector={GeneralDescriptionSelectors.getGeneralDescriptionServiceClasses}
                property="name"
                />
        );
    }

    handleDetailIconClick(row, event) {
        //event.stopPropagation();
        this.setState({ isVisible: true });
    }

    onDetailClose = () => {
         this.setState({ isVisible: false });
    }

    detailIconFormatter = (cell,row) =>{
        return <PTVIcon className="color-bright-azure" width="28" height="28" name="icon-info" onClick={(e) => this.handleDetailIconClick(row, e)} />
    }

    render() {
        var selectRowProp = {
            mode: "radio",
            clickToSelect: true,
            className: "highlighted",
            onSelect: this.onRowSelect,
            hideSelectColumn: false,
            selected: this.props.generalDescriptionItemSelectedId == null ? [] : [this.props.generalDescriptionItemSelectedId]
        }

        const {formatMessage} = this.props.intl;
        const {isFetching, pageNumber, searchMaxPageCount, searchIsMoreThanMax, generalDescriptionName, targetGroupId, serviceClassId, subTargetGroupId, targetGroups, serviceClasses, subTargetGroups, generalDescriptionItemSelectedId, generalDescriptionItemSelected, generalDescriptions, searchIsFetching, searchAreDataValid, searchCount, showSelectionConfirmButton, selectToPath } = this.props;
        const description = selectToPath ? {...Messages.messages.generalDescriptionHeaderText} : {...Messages.messages.generalDescriptionHeaderSearchText};

        const descriptionSearchResultsColumnsDefinition = [
            { dataField: "id", isKey: true, hidden: true, columnHeaderTitle: 'ID' },
            { dataField: "name", dataSort: true, dataFormat: this.generalDataFormatter, columnHeaderTitle: this.props.intl.formatMessage(Messages.messages.generalDescriptionsResultHeaderName) },
            { dataField: "alternateName", hidden: true, columnHeaderTitle: 'alternateName' },
            { dataField: "serviceClasses", dataFormat: this.serviceClassDataFormatter, columnHeaderTitle: this.props.intl.formatMessage(Messages.messages.generalDescriptionsResultHeaderServiceClass) },
            { dataField: "shortDescription", dataFormat: this.generalDataFormatter, columnHeaderTitle: this.props.intl.formatMessage(Messages.messages.generalDescriptionsResultHeaderShortDescription) },
            { dataField: "description", hidden: true, columnHeaderTitle: 'Description' },
            { dataFormat: this.detailIconFormatter, columnHeaderTitle: this.props.intl.formatMessage(Messages.messages.generalDescriptionDetailIcon) }
        ];

        return (
            <div>
                <PTVOverlay
                    title={formatMessage(Messages.messages.detaileHeaderTitle)}
                    isVisible = { this.state.isVisible }
                    onCloseClicked = { this.onDetailClose }
                >
                    {generalDescriptionItemSelectedId ? <ConnectedGeneralDescriptionPreview /> : null}
                </PTVOverlay>

                <PTVHeaderSection
                    titleClass="card-title"
                    title={selectToPath ? formatMessage(Messages.messages.generalDescriptionHeaderTitle) : formatMessage(Messages.messages.generalDescriptionHeaderSearchTitle)}
                    buttonText="Add general description"
                    buttonClick={this.handleAdd}
                    secondButtonText={formatMessage(Messages.messages.generalDescriptionGoBack)}
                    secondButtonClick={this.handleGoBack}
                    buttonRoute=""
                    buttonDisabled={true}
                    showSecondButton={ selectToPath != null }
                    showButton={false}
                    />

                <div className="box box-white">
                    <div className="form-wrap">
                      <div className="step-1">
                        {isFetching ?
                            <PTVPreloader />
                            :
                            <form className="form-inline  service-search" role="form">
                                <div className="row general-description-header-text">
                                    <FormattedHTMLMessage {...description}></FormattedHTMLMessage>
                                </div>
                                <div className="row form-group">
                                    <div className="col-md-6">
                                        <PTVTextInput
                                            value={generalDescriptionName}
                                            id="1" placeholder={formatMessage(Messages.messages.namePlaceholder)}
                                            label={formatMessage(Messages.messages.nameTitle)}
                                            componentClass="row"
                                            labelClass="col-sm-4"
                                            name='generalDescriptionName'
                                            inputclass="col-sm-8"
                                            changeCallback={this.onInputChange('generalDescriptionName')}
                                            maxLength="100"
                                            onEnterCallBack={this.handleSubmit} />
                                    </div>
                                    <div className="col-md-6">
                                        <LocalizedComboBox
                                            value={targetGroupId}
                                            name='targetGroupId'
                                            id="3"
                                            label={formatMessage(Messages.messages.targetGroupComboTitle)}
                                            tooltip={formatMessage(Messages.messages.targetGroupComboTooltip)}
                                            componentClass="row"
                                            labelClass="col-sm-6"
                                            autoClass="col-sm-6"
                                            values={targetGroups}
                                            changeCallback={this.onInputChange('targetGroupId')}
                                            className="limited w200" />
                                    </div>
                                </div>
                                <div className="row form-group">
                                    <div className="col-md-6">
                                        <LocalizedComboBox
                                            value={serviceClassId}
                                            name='serviceClassId'
                                            id="2"
                                            label={formatMessage(Messages.messages.serviceClassComboTitle)}
                                            tooltip={formatMessage(Messages.messages.serviceClassComboTooltip)}
                                            componentClass="row"
                                            labelClass="col-sm-5"
                                            autoClass="col-sm-7"
                                            values={serviceClasses}
                                            changeCallback={this.onInputChange('serviceClassId')}
                                            className="limited w280" />
                                    </div>
                                    <div className="col-md-6">
                                        {subTargetGroups.length > 0 ?
                                            <LocalizedComboBox
                                                value={subTargetGroupId}
                                                name='subTargetGroupId'
                                                id="4"
                                                label={formatMessage(Messages.messages.subTargetGroupComboTitle)}
                                                componentClass="row"
                                                labelClass="col-sm-5"
                                                autoClass="col-sm-7"
                                                values={subTargetGroups}
                                                changeCallback={this.onInputChange('subTargetGroupId')}
                                                className="limited w280" />
                                            : null}
                                    </div>
                                </div>
                                <div className="row">
                                    <div className="col-xs-12 ">
                                        <div className="button-group">
                                            <ButtonSearch className="btn btn-primary col-md-offset-11" onClick={this.handleSubmit} />
                                        </div>
                                    </div>
                                </div>
                                <div className="row">
                                    {showSelectionConfirmButton ?
                                        <div className="col-xs-12">
                                            <div className="button-group">
                                                <PTVButton onClick={this.handleSelection}>
                                                   {formatMessage(Messages.messages.generalDescriptionConfirm)}
                                                </PTVButton>
                                            </div>
                                        </div>
                                        : null}
                                    <div className="col-xs-12">
                                        {generalDescriptions.size > 0 ?
                                            <div>
                                                {searchIsMoreThanMax ? <PTVLabel labelClass="main">{formatMessage(Messages.messages.generalDescriptionResultCount) + (pageNumber * searchMaxPageCount) + '/' + searchCount}</PTVLabel>
                                                    : null /*<PTVLabel labelClass="main">{formatMessage(Messages.messages.generalDescriptionResultCount) + searchCount}</PTVLabel>*/}
                                                <PTVTable
                                                    contentDataSlector={CommonGeneralDescriptionSelectors.getGeneralDescriptions}
                                                    data={generalDescriptions}
                                                    striped={true}
                                                    hover={true}
                                                    selectRow={selectRowProp}
                                                    tableBodyClass="selectable"
                                                    tableHeaderClass="selectable"
                                                    columnsDefinition={descriptionSearchResultsColumnsDefinition} />
                                                {searchIsMoreThanMax ? <div>
                                                    <div className='button-group centered'>{!searchIsFetching ? <ButtonShowMore onClick={this.handleShowMore} /> : <PTVPreloader />}</div>
                                                    <PTVLabel labelClass="main">{formatMessage(Messages.messages.generalDescriptionResultCount) + (pageNumber * searchMaxPageCount) + '/' + searchCount}</PTVLabel>
                                                </div> : null /*<PTVLabel labelClass="main">{formatMessage(Messages.messages.generalDescriptionResultCount) + searchCount}</PTVLabel>*/}

                                            </div>
                                            : generalDescriptions.size == 0 && !searchIsFetching && searchAreDataValid ?
                                                <PTVLabel labelClass="strong">
                                                    {formatMessage(commonMessages.noSeachResults)}
                                                </PTVLabel>
                                                : searchIsFetching ?
                                                    <PTVPreloader />
                                                    : null}
                                    </div>
                                    {showSelectionConfirmButton ?
                                        <div className="col-xs-12">
                                            <div className="button-group">
                                                <PTVButton onClick={this.handleSelection}>
                                                    {formatMessage(Messages.messages.generalDescriptionConfirm)}
                                                </PTVButton>
                                            </div>
                                        </div>
                                        : null}
                                </div>
                            </form>}
                          </div>
                    </div>
                </div>
            </div>
        );
    }
}

function mapStateToProps(state, ownProps) {
    const props = { keyToState: 'generalDescription', keyToEntities: 'generalDescriptions' };
    const selectToPath = GeneralDescriptionSelectors.getServiceInfoReturnPath(state);
    const generalDescriptionItemSelectedId = GeneralDescriptionSelectors.getGeneralDescriptionSelectedId(state);
    return {
        generalDescriptionItemSelectedId,
        generalDescriptionItemSelected: GeneralDescriptionSelectors.getGeneralDescriptionSelectedItem(state),
        generalDescriptions: CommonSelectors.getSearchedEntities(state, props),
        generalDescriptionName: GeneralDescriptionSelectors.getGeneralDescriptionName(state),
        targetGroupId: GeneralDescriptionSelectors.getGeneralDescriptionTargetGroupId(state),
        targetGroups: GeneralDescriptionSelectors.getTopTargetGroupsObjectArray(state),
        serviceClassId: GeneralDescriptionSelectors.getGeneralDescriptionServiceClassId(state),
        serviceClasses: GeneralDescriptionSelectors.geServiceClassesObjectArray(state),
        subTargetGroups: GeneralDescriptionSelectors.getSubTargetGroupsObjectArray(state),
        subTargetGroupId: GeneralDescriptionSelectors.getGeneralDescriptionSubTargetGroupId(state),
        isFetching: CommonSelectors.getSearchIsFetching(state, props),
        areDataValid: CommonSelectors.getSearchAreDataValid(state, props),
        searchCount: CommonSelectors.getSearchResultsCount(state, props),
        searchIsFetching: CommonSelectors.getSearchResultsIsFetching(state, props),
        searchAreDataValid: CommonSelectors.getSearchResultsAreDataValid(state, props),
        searchIsMoreThanMax: CommonSelectors.getSearchResultsIsMoreThanMax(state, props),
        searchMaxPageCount: CommonSelectors.getSearchResultsMaxPageCount(state, props),
        pageNumber: CommonSelectors.getSearchResultsPageNumber(state, props),
        showSelectionConfirmButton: selectToPath && generalDescriptionItemSelectedId,
        selectToPath
    }
}

const actions = [
    descriptionActions,
    PageContainerActions,
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SearchGeneralDescriptionContainer));
