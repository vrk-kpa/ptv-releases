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
import { PTVLabel, PTVMessageBox } from '../../Components';
import {ButtonSaveDraft, ButtonSave, ButtonUpdate, ButtonCancelUpdate} from './Buttons';
import NotificationContainer from './NotificationContainer';
import './Styles/StepContainer.scss';
import cx from 'classnames';

// selectors
import * as CommonSelectors from './Selectors';

const messages = defineMessages({
    saveButton: {
        id: "Components.StepContainer.SaveButton",
        defaultMessage: "Tallenna luonnos"
        }
});
class StepContainer extends Component {
    static propTypes = {
        intl: intlShape.isRequired,
        saveChangesCallback: PropTypes.func,
        readOnly: React.PropTypes.bool
    };

    static defaultProps = {
        readOnly: false
    }
    constructor(props) {
        super(props);
        this.onHandleSaveChanges = this.onHandleSaveChanges.bind(this);
        this.onHandleUpdateStep = this.onHandleUpdateStep.bind(this);
        this.onHandleCancelUpdateStep = this.onHandleCancelUpdateStep.bind(this);
        this.onHandleSaveStep = this.onHandleSaveStep.bind(this);
    }

    componentDidMount() {
        if (!this.props.areDataValid) {
            this.props.loadAction(this.props.entityId, this.props.languageToCode, this.props.keyToState);
        }
    }

    componentWillReceiveProps = (nextProps) => {
        if(this.props.languageTo!="" && (this.props.languageTo != nextProps.languageTo)){
            this.props.loadAction(this.props.entityId, nextProps.languageToCode, this.props.keyToState)
        }
        if(this.props.languageFrom!="" && (this.props.languageFrom != nextProps.languageFrom)){
            this.props.loadAction(this.props.entityId, nextProps.languageFromCode, this.props.keyToState)
        }
    }

    onHandleSaveChanges() {
        if (this.props.saveChangesCallback){
            this.props.saveChangesCallback();
        }
    }
    onHandleUpdateStep(){
        this.props.updateStepCallback();
    }
    onHandleCancelUpdateStep(){
        this.props.cancelUpdateStepCallback();
    }
    onHandleSaveStep(){
        this.props.saveStepCallback();
    }
    
    renderButtons = () => {
        return this.props.buttonsVisible ? this.returnButtonComponents() : null;
    }

    returnButtonComponents = () => {
        const btnGroupClass = cx('button-group', this.props.mode);

        switch(this.props.mode) {
            case 'add':
                // temporary removed REL 1.2
                return null;
            case 'view':
                return(<div className={btnGroupClass}>
                            <ButtonUpdate className="up" onClick={ this.props.updateStepCallback ? this.onHandleUpdateStep : null } disabled = {this.props.buttonsDisabled}/>
                        </div>);
            case 'edit':
                return (<div className={btnGroupClass}>
                            <ButtonCancelUpdate className="up" onClick={ this.onHandleCancelUpdateStep } disabled = {this.props.buttonsDisabled}/>
                            <ButtonSaveDraft className="up" onClick={this.onHandleSaveStep} disabled = {this.props.buttonsDisabled}/>
                        </div>);
            default:
                return null;

        }
    }

    renderTranslationContent = () =>{
        const translationModeOriginal = this.props.readOnly ? "none" : "view";
        const translationModeNew = this.props.readOnly ? "none" : "edit"; 
        const originalReadOnlyContent = React.cloneElement(this.props.children, {readOnly: true, language: this.props.languageFromCode, translationMode: translationModeOriginal, splitContainer:true });
        const translatedContent = React.cloneElement(this.props.children, {language: this.props.languageToCode, translationMode: translationModeNew, splitContainer:true});
        return (
            <div className = 'row'>
                <div className = 'col-xs-6'>{translatedContent}</div>
                <div className = 'col-xs-6'>{originalReadOnlyContent}</div>
            </div>);                                 
    }
    renderLanguageContent = () =>{
        const languageContent = React.cloneElement(this.props.children, {language: this.props.languageToCode, translationMode: "none", splitContainer:false});
        return languageContent;
    }

    render() {
        const {formatMessage} = this.props.intl;
        const stepClass = cx('step-container', { "readonly": this.props.readOnly, "translation": !this.props.readOnly && this.props.translationMode });        
        return (
            <div className={stepClass}>
                <div className="step-container-header">
                    {this.props.mainTitle?<h2>{this.props.mainTitle}</h2>:null}
                    {this.props.mainText?<p className='main-text'>{this.props.mainText}</p>:null}
                </div>
                <div className="step-container-actions clearfix">
                    <h3 className="step-container-actions-label">{this.props.subTitle}</h3>
                    { this.renderButtons() }
                </div>
                <NotificationContainer keyToState={this.props.keyToState} notificationKey={this.props.stepKey}/>

                <div className="box box-white">
                    <div className="form-wrap">
                        { this.props.showEntityId ? <PTVLabel labelClass='entity-id'><strong>ID: </strong>{ this.props.entityId }</PTVLabel> : null }
            		    { this.props.translationMode ? this.renderTranslationContent() : this.renderLanguageContent() }
                    </div>                   
            	</div>
                <div className="button-group">
                   { this.renderButtons() }
                </div>
            </div>
       );
    }
}
function mapStateToProps(state, ownProps) {
  const languageFrom = CommonSelectors.getLanguageFrom(state, ownProps) || CommonSelectors.getTranslationLanguageId(state, 'fi');
  const languageTo = CommonSelectors.getLanguageTo(state, ownProps) || CommonSelectors.getTranslationLanguageId(state, 'fi');
  return {    
    languageFrom :languageFrom,
    languageTo :languageTo,
    languageFromCode: CommonSelectors.getTranslationLanguageCode(state, {id: languageFrom}),
    languageToCode: CommonSelectors.getTranslationLanguageCode(state, {id: languageTo})                
  }
}
export default connect(mapStateToProps)(injectIntl(StepContainer));
