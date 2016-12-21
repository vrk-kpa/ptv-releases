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
import { injectIntl, defineMessages, intlShape, FormattedMessage } from 'react-intl'
import cx from 'classnames';
import Immutable, { OrderedMap, Map } from 'immutable';
import {connect} from 'react-redux';
import mapDispatchToProps from '../../Configuration/MapDispatchToProps';
import * as appActions from './Actions';
import * as pageActions from './Actions/PageContainerActions';
import shortId from 'shortid';

/// PTV Components
import { PTVValidationManager, PTVMessageBox, PTVConfirmDialog, PTVPreloader, PTVButton, PTVLabel, PTVIcon, PTVAutoComboBox } from '../../Components';

/// App Common
import { ButtonCancel, ButtonContinue, ButtonDone, ButtonDelete, ButtonPublish } from './Buttons';
import NotificationContainer from './NotificationContainer';

/// App Helperes
import StepContainer from './StepContainer';
import { publishingStatuses } from './Enums';
import { Link, browserHistory } from 'react-router';
import { getPageModeStateForKey, getPageEntityId } from './Selectors';
import * as comonServiceActions from '../../Containers/Services/Common/Actions';
import shortid from 'shortid';

// selectors
import * as CommonSelectors from './Selectors';

const messages = defineMessages({
    linkGoBack: {
        id: "Containers.Common.PageContainer.Link.Back",
        defaultMessage: "Takaisin"
    },
    languageSelectionFromTitle: {    
        id: "Containers.Common.PageContainer.LanguageFrom.Title",
        defaultMessage: "Vertailukieli"
    },
    languageSelectionToTitle: {    
        id: "Containers.Common.PageContainer.LanguageTo.Title",
        defaultMessage: "Kielivalinta"
    },
    languageSelectionLink: {    
        id: "Containers.Common.PageContainer.Language.Link",
        defaultMessage: "Vertaa kieliversioita"
    },
});

class PageContainer extends Component {

    getChildContext = () => {
        return { ValidationManager: this.validationManager };
    };

    validationManager = new PTVValidationManager();

    constructor(props) {
        super(props);
        this.state = { 
                       showConfirmDeleteDialog: false,
                       showConfirmCancelDialog: false,
                       showSaveDraft: false,
                       translate: false,
                       messages: new OrderedMap() }
    }

    handleAllReadOnlyStates = (value = false) => {
       this.props.steps.forEach((element, index) => {
          this.props.actions.setStepModeActive(this.props.keyToState,index);
        });
    }

    handleSubmitSaveDraftDialog = () => {
        this.setState({
                showSaveDraft: false,               
            })
    }

    invalidateAllSteps = () => {
        if ( typeof this.props.invalidateAllSteps === 'function') {
            this.props.invalidateAllSteps();
        }
        this.props.actions.resetStepMode(this.props.keyToState);
    }

    // handleAllNotificationStates = (value = '') => {
    //     let newState = {}
        
    //     this.props.steps.forEach((element, index) => {
    //       newState['step' + (index + 1) + 'Notification'] = value;
    //     });

    //     this.setState(newState);
    // } 

    handleSaveContainer = () => {
        this.setState({ messages: new OrderedMap() });
        if (typeof this.props.removeServerResultAction === 'function') {
            this.props.removeServerResultAction();
        }
        // this.props.actions.resetAllMessage();
        setTimeout(() => {
        const result = this.validationManager.isManagerValid().then((data) => {

       if (data == true)
       {
           this.props.actions.resetAllMessage();
            this.props.saveAction({keyToState: this.props.keyToState, language: this.props.languageToCode});
            window.scrollTo(0, 0);    
        }
        else {
            if (data != undefined)
            {
                this.setState({ messages: data});
            }
            else { 
                    this.setState({ messages: new OrderedMap({message: 'Validation failed, try again!'})});
                 }
        }
        })
        }, 300);
    }

    renderConfirmDialogs = confirmDialogs => {
        const {formatMessage} = this.props.intl;
        return confirmDialogs.map((confirmDialog) => {
            const [acceptButton, cancelButton, text] = confirmDialog.messages;
            const keyToElement = this.props.keyToState + 'confirmDialog' + confirmDialog.type;
            switch (confirmDialog.type) {
                case 'cancel': return (<PTVConfirmDialog 
                        show = { this.state.showConfirmCancelDialog } 
                            acceptButton= { formatMessage(acceptButton) }
                            cancelButton= { formatMessage(cancelButton) }
                            text= { formatMessage(text) }
                            key = { keyToElement }
                            acceptCallback={() => {
                            // this.invalidateAllSteps();
                            // this.setState({ messages: new OrderedMap()});
                            // this.props.steps.forEach((step) => {
                            //     this.callLoadActions(step.loadAction);
                            // });
                            // this.setState({showConfirmCancelDialog: false});
                            // this.handleAllNotificationStates();
                            browserHistory.push(this.props.searchPath ? this.props.basePath + this.props.searchPath : this.props.basePath);
                        }}
                        cancelCallback= { this.handleCancelOfCancelDialog }
                     />)
                case 'delete': return (<PTVConfirmDialog 
                        show = { this.state.showConfirmDeleteDialog } 
                            acceptButton= { formatMessage(acceptButton) }
                            cancelButton= { formatMessage(cancelButton) }
                            text= { formatMessage(text) }
                            key = { keyToElement }
                            acceptCallback={() => {
                                this.setState({showConfirmDeleteDialog: false});
                                this.props.deleteAction(this.props.entityId, this.props.keyToState, this.props.languageToCode);
                        }}
                        cancelCallback= { this.handleCancelOfDeleteDialog }
                     />)
                case 'save': return (<PTVConfirmDialog 
                        show = { this.state.showSaveDraft} 
                        acceptButton= { formatMessage(acceptButton) }
                        cancelButton= { formatMessage(cancelButton) }
                        text= { formatMessage(text) }
                        key = { keyToElement }
                        acceptCallback={() => {
                            if (typeof confirmDialog.acceptCallback ==='function') {
                                confirmDialog.acceptCallback();
                            }
                            this.setState({showSaveDraft: false});
                        }}
                        cancelCallback= { this.handleCancelSaveDraftDialog }
                     />)
                default: return (<PTVConfirmDialog 
                        show = { confirmDialog.isShown ? confirmDialog.isShown : false } 
                        { ...confirmDialog }
                     />)
            } 
            
        })
    }

    // confirm dialog handlers
    handleCancelOfDeleteDialog = () => {
        this.setState({showConfirmDeleteDialog: false})
    }

    handleCancelSaveDraftDialog = () => {
        browserHistory.push(this.props.basePath);
    }

    handleCancelOfCancelDialog = () => {
        this.setState({showConfirmCancelDialog: false})
    }

    // confirm dialog handlers end

    renderButtons = (buttons, disabled, line) => {
        const publishingStatus = this.props.publishingStatus; 
        return buttons.map((button) => {
                    const keyToElement = line + 'button' + button.type;
                    switch (button.type) {
                        case 'cancel': {
                            return <ButtonCancel key= { keyToElement } disabled={ disabled } onClick={ this.handleCancelButton }/>
                        }
                        case 'done': 
                        case 'continue': {
                            const Element = button.type == 'continue' ? ButtonContinue : ButtonDone;
                            return <Element key= { keyToElement } disabled={ disabled } onClick={ () => {
                                this.handleSaveContainer();
                            }}/>
                        }
                        case 'delete': {
                            return publishingStatus != null && (publishingStatus == publishingStatuses.published || publishingStatus == publishingStatuses.draft) ? <ButtonDelete { ...button } key= { keyToElement } disabled={ disabled } onClick={ this.handleDeleteButton } /> : null;
                        }
                        case 'publish': {
                            return publishingStatus != null && publishingStatus == publishingStatuses.draft ? <ButtonPublish { ...button } key= { keyToElement } disabled={ disabled } onClick={ this.handlePublishButton }/> : null;
                        }
                        default:
                            return null;
                    }})
    }

    renderButtonGroup = (disabled=false, line) => {
        const buttons = this.props.readOnly ? [{type: 'delete'}, { type: 'publish' }] : [{ type: 'cancel'}, { type: 'continue' }];

        return (
            <div className='button-group'>
                { !this.state.translate ? this.renderButtons(buttons, disabled, line) : null }
            </div>
        )
    }

    handleCustomClear = (id) => {
        return <PTVIcon name="icon-cross" componentClass="action" onClick={ () => this.handleLanguageFromClear(id) } />
    }
    handleLanguageFromClear = (id) => {
        this.setState({ translate: false });
    }
    handleLanguageFromChange = (id) =>{
        this.props.actions.setLanguageFrom(this.props.keyToState,id)
    }
    handleLanguageToChange = (id) =>{
        this.props.actions.setLanguageTo(this.props.keyToState,id)
    }
    handleAddTranslation = () =>{
        this.props.actions.setTranslationMode(this.props.keyToState);
        this.setState({ translate: true })
    }
    renderTranslationGroup = () => {
        const {formatMessage} = this.props.intl;
        const {languages, languageFrom, languageTo, keyToState }  = this.props;
        return (
            this.props.isTranslatable && this.props.publishingStatus != publishingStatuses.deleted?
             <div className = "row">
                <PTVAutoComboBox
                    componentClass="col-xs-3"
                    value = { languageTo }
                    values = { languages }
                    label = { formatMessage(messages.languageSelectionToTitle) }
                    changeCallback = { this.handleLanguageToChange }
                    name='languagesTo'                                     
                    readOnly= { false }
                    clearable = { false }
                    className = "limited w280"
                />
                {!this.state.translate && !this.props.isNew?
                    <div className="col-xs-3">
                        <PTVLabel>
                            <PTVButton                    
                                type = 'link'
                                onClick = { this.handleAddTranslation }> { formatMessage(messages.languageSelectionLink) }
                            </PTVButton>
                        </PTVLabel>
                    </div>
                :null}
                {this.state.translate?
                    <PTVAutoComboBox
                        componentClass="col-xs-3"
                        value = { languageFrom }
                        values = { languages }
                        label = { formatMessage(messages.languageSelectionFromTitle) }
                        iconAction = { () => this.handleCustomClear(languageFrom) }
                        changeCallback = { this.handleLanguageFromChange }
                        name='languagesTo'                                       
                        readOnly= { false }
                        clearable = { false }
                        className = "limited w280"
                    />
                :null}
            </div>:null)
    }

    // Button handleres

    handleDeleteButton = () => {
        // this.props.removeServerResultAction();
        this.props.actions.resetAllMessage();
        this.setState({showConfirmDeleteDialog: true})
        window.scrollTo(0, 0);
    }

    handleCancelButton = () => {
        // this.props.removeServerResultAction();
        //this.props.actions.resetAllMessage();
        this.setState({showConfirmCancelDialog: true})
        window.scrollTo(0, 0);
    }

    handlePublishButton = () => {
        // this.props.removeServerResultAction();
        this.props.actions.resetAllMessage();
        this.props.publishAction(this.props.entityId, this.props.keyToState, this.props.languageToCode);
        window.scrollTo(0, 0);
    }
    // Button handlers end 

    getViewMode = (readOnly) => {
        const mode = ( this.props.publishingStatus == publishingStatuses.deleted ) ? '' : 
            
            readOnly ? 'view' : this.props.isNew ? 'add' : 'edit';
            // console.log('mode', mode, readOnly)
        return mode;
    }

    renderSteps = (buttonsDisabled, simpleView) => {
        const {formatMessage} = this.props.intl;
        const editStepActivatedOnIndex = this.props.pageModeState && this.props.pageModeState.get('editedStep') ? this.props.pageModeState.get('editedStep') : undefined ;
        return this.props.steps.map((step, index) => {
            const stepIndex = index + 1;
            if (this['step' + stepIndex + 'updateCallback'] === undefined)
            {
                // var notificationState = 'step' + stepIndex + 'Notification'
                // this.state[notificationState] = '';                
                this['step' + stepIndex + 'updateCallback'] = this.handleUpdateStep(stepIndex);
            }
            if (this.props.entityId && !this.props.isNew){
                this['step' + stepIndex + 'saveCallback'] = this.handleSaveStep(this.props.entityId, stepIndex, step.saveStepAction, step.loadAction);
                this['step' + stepIndex + 'cancelUpdateCallback'] = this.handleCancelUpdateStep(this.props.entityId, stepIndex, step.loadAction);
                //this['step' + stepIndex + 'saveToLocalStorage'] = this.handleSaveChangesStep(step.objectToManipulate, 'step' + stepIndex + 'Form');
            }
            const readOnly = this.props.readOnly ? editStepActivatedOnIndex === undefined ? true : editStepActivatedOnIndex != stepIndex : false;
            const publishingStatus = this.props.pageModeState ? Immutable.fromJS(this.props.pageModeState.get('publishStatus')) : null;
            return !step.readOnlyVisible || this.props.readOnly ? <StepContainer 
                        //saveChangesCallback={ this.handleSaveChangesStep()}
                        mainTitle= { step.mainTitle && step.mainTitleView ? this.props.readOnly ? formatMessage(step.mainTitleView) : formatMessage(step.mainTitle) : null }
                        mainText= { step.mainText && step.mainTextView ? this.props.readOnly ? formatMessage(step.mainTextView) : formatMessage(step.mainText) : null }
                        subTitle= { step.subTitle && step.subTitleView ? this.props.readOnly ? formatMessage(step.subTitleView) : formatMessage(step.subTitle) : null }
                        entityId= { this.props.entityId }
                        showEntityId= { stepIndex === 1 && !this.props.isNew }
                        readOnly={ readOnly }
                        keyToState = { this.props.keyToState }
                        stepKey = { `step${stepIndex}Form`}
                        areDataValid= { step.areDataValid }
                        loadAction= { step.loadAction }
                        key= { this.props.keyToState + 'step' + stepIndex }
                        notificationMessage = { this.state['step' + stepIndex + 'Notification'] } 
                        updateStepCallback= { this.props.entityId ? this['step' + stepIndex + 'updateCallback'] : null } 
                        cancelUpdateStepCallback={ this['step' + stepIndex + 'cancelUpdateCallback'] }
                        saveStepCallback={ this['step' + stepIndex + 'saveCallback'] }
                        draftButton= { true }
                        draftSaveUp= { stepIndex == 1 }
                        translationMode = {this.state.translate}
                        mode={ this.getViewMode(readOnly) }
                        buttonsVisible= { !step.readOnlyVisible }
                        buttonsDisabled= { buttonsDisabled && editStepActivatedOnIndex != stepIndex } >
                        { step.isFetching ? <PTVPreloader /> : <step.child entityId= { this.props.entityId } readOnly={ readOnly } publishingStatus = { publishingStatus } keyToState={ this.props.keyToState}/> }
                   </StepContainer> : null;
        })
    }

     renderSimpleSteps = () => {
        const {formatMessage} = this.props.intl;
        return this.props.steps.map((step, index) => {
            const stepIndex = index + 1;            
            const publishingStatus = this.props.pageModeState ? Immutable.fromJS(this.props.pageModeState.get('publishStatus')) : null;
            return <StepContainer 
                        subTitle= { formatMessage(step.subTitleView) }
                        entityId= { this.props.entityId }
                        showEntityId= { stepIndex === 1 }
                        readOnly={ true }
                        keyToState = { this.props.keyToState }
                        stepKey = { `step${stepIndex}Form`}
                        areDataValid= { step.areDataValid }
                        loadAction= { step.loadAction }
                        key= { this.props.keyToState + 'step' + stepIndex }                                                
                        translationMode = {false}
                        mode={ "view" }
                        >
                        { !step.areDataValid ? <PTVPreloader /> : <step.child entityId= { this.props.entityId } readOnly={ true } publishingStatus = { publishingStatus } keyToState={ this.props.keyToState}/> }
                   </StepContainer>;
        })
    }

    handleUpdateStep = (stepIndex) => () => {
        this.setInitialUpdateState(stepIndex);
        this.props.actions.setStepModeActive(this.props.keyToState,stepIndex);        
    }

    handleSaveStep = (id, stepIndex, saveAction) => () => {
        this.setInitialUpdateState(stepIndex);
        setTimeout(() => {
        const result = this.validationManager.isManagerValid().then((data) => {                    
            if (data == true){
                this.props.actions.resetAllMessage();
                this.props.actions.setStepModeInActive(this.props.keyToState);
                saveAction({id, keyToState: this.props.keyToState, language: this.props.languageToCode});
            }
            else {
                this.props.actions.setStepModeActive(this.props.keyToState,stepIndex);
                if(data != undefined){
                    this.setState({ messages: data});
                }
                else{ 
                    this.setState({ messages: new OrderedMap({message: 'Validation failed, try again!'})});
                }
            }
        })}, 300); 
    }

    setInitialUpdateState = (stepIndex) => {
        this.setState({ messages: new OrderedMap()});
        // this.handleSubComponentNotification('', stepIndex);
       // this.props.removeServerResultAction();
    }

    callLoadActions = (loadActions, entityId) => {
        if (Array.isArray(loadActions)) {
            loadActions.forEach((loadAction) => loadAction(entityId, this.props.languageToCode))
        }
        else if (typeof loadActions === 'function') {
            loadActions(entityId, this.props.languageToCode);
        }
    }

    handleCancelUpdateStep = (entityId, stepIndex, loadAction ) => () => {
        this.setInitialUpdateState(stepIndex);
        this.callLoadActions(loadAction, entityId);
        this.props.actions.setStepModeInActive(this.props.keyToState);
    }

    componentDidMount = () => { 
        // console.log('did mount page')
        if (typeof this.props.removeServerResultAction === 'function') {
            this.props.removeServerResultAction();
        }
        this.props.actions.resetAllMessage();
        
        if (this.props.isTranslatable) {
            this.props.actions.loadTranslatableLanguages();
        }

        if (!this.props.publishStatusesLoad){
            this.props.actions.loadPublishingStatuses();
        }

        if (this.props.statusEndpoint && this.props.entityId){
            // this.props.actions.getEntityStatus(this.props.entityId,this.props.statusEndpoint,this.props.keyToState);
        }
        if(!this.props.simpleView){
            window.scrollTo(0, 0);  
        }               
    }

    handleServerResult = nextProps => {
        if (!nextProps.isNew && nextProps.entityId != this.props.entityId) {
            this.setState({
                        showSaveDraft: true
                    });
             this.props.steps.forEach((step) => {
                     this.callLoadActions(step.loadAction, nextProps.entityId);
            })
        }
    } 

    componentWillReceiveProps = (nextProps) => {
        this.handleServerResult(nextProps);
    }

    // getStepsIsFetching = () => {
    //     return this.props.steps.some(step => {
    //         // console.log(this.props.keyToState, step.isFetching);
    //         return step.isFetching == true;
    //     })
    // }

    onMessageClose = (id) => {
        this.props.actions.resetMessages(id);
    }

    render = () => {
        const {formatMessage } = this.props.intl;
        const notification = this.props.notifications;
        const editStepActivatedOnIndex = this.props.pageModeState && this.props.pageModeState.get('editedStep') ? this.props.pageModeState.get('editedStep') : undefined ;
        // const isFetchingOfAnyStep = this.getStepsIsFetching();
        const buttonsDisabled = this.state.showConfirmCancelDialog || this.state.showConfirmDeleteDialog || editStepActivatedOnIndex != undefined || this.props.isFetchingOfAnyStep;
        const preloaderIsShown = this.props.isFetchingOfAnyStep;
        return (!this.props.simpleView ?
                 <div className={ cx('page-container', this.props.className) }>
                    { this.props.params  ?
                        <PTVLabel> 
                            <Link to={this.props.params.id && this.props.basePath ? this.props.basePath + '/search/' + this.props.params.id : '/service'}>
                                <PTVIcon name="icon-angle-left" className="brand-fill" />
                                {formatMessage(messages.linkGoBack)}
                            </Link>
                        </PTVLabel>
                    : null}
                    { preloaderIsShown ? <PTVPreloader /> : null}
                    <NotificationContainer 
                        keyToState = { this.props.keyToState }
                        validationMessages={ this.state.messages.toArray()} 
                        />
                    { this.renderConfirmDialogs(this.props.confirmDialogs) }
                    { this.renderButtonGroup(buttonsDisabled, 'top') }
                    { this.renderTranslationGroup() }
                    { this.renderSteps(buttonsDisabled) }    
                    { this.props.children }
                    { this.renderButtonGroup(buttonsDisabled, 'bottom') }
                    <div className="clearfix"></div>
                </div> : 
                <div className={ cx('page-container', this.props.className) }>
                    {this.renderSimpleSteps()} 
                </div>)
    }

}

PageContainer.propTypes = {
            className: PropTypes.string,
            confirmDialogs: PropTypes.array,
            editButtons: PropTypes.array,
            readOnlyButtons: PropTypes.array,
            readOnly: PropTypes.bool,
            saveAction: PropTypes.func.isRequired,
            steps: PropTypes.array.isRequired,
            keyToState: PropTypes.string.isRequired,
            removeServerResultAction: PropTypes.func
        };

PageContainer.defaultProps = {
    confirmDialogs: [],
    editButtons: [],
    readOnlyButtons: [],
    readOnly: false
}

PageContainer.childContextTypes = {
    ValidationManager: React.PropTypes.object
};

function mapStateToProps(state, ownProps) {
  const pageModeState = getPageModeStateForKey(state, ownProps);
  const entityId = getPageEntityId(state, ownProps);
  const isNew = shortId.isValid(entityId) && entityId.length !== 36;
  const languageTo = pageModeState.get('languageTo')|| CommonSelectors.getTranslationLanguageId(state, 'fi');
  const languageToCode = CommonSelectors.getTranslationLanguageCode(state, {id: languageTo});
  const publishingStatusId = ownProps.getEntityStatusSelector(state, {...ownProps, language : languageToCode});
  const publishStatusesLoad = CommonSelectors.getPublishingStatuses(state).size > 0;
  return {
    isFetchingOfAnyStep: CommonSelectors.getIsFetchingOfAnyStep(state, ownProps),
    notifications: CommonSelectors.getCommonNotifications(state),
    pageModeState,
    publishingStatus : CommonSelectors.getPublishingStatusType(state, publishingStatusId),
    languages: CommonSelectors.getTranslationLanguagesObjectArray(state),
    isNew,
    entityId: !isNew ? entityId : null,
    readOnly: pageModeState.get('readOnly') || !isNew, 
    languageFrom :pageModeState.get('languageFrom') || CommonSelectors.getTranslationLanguageId(state, 'fi'),
    languageTo : languageTo,
    languageToCode: languageToCode,
    publishStatusesLoad
  }
}

const actions = [
    appActions,
    comonServiceActions,
    pageActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PageContainer));