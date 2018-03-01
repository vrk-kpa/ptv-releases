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
import React, { PropTypes, Component } from 'react'
import { injectIntl, defineMessages } from 'react-intl'
import cx from 'classnames'
import Immutable, { OrderedMap } from 'immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import mapDispatchToProps from '../../Configuration/MapDispatchToProps'
import * as appActions from './Actions'
import * as pageActions from './Actions/PageContainerActions'
import { setLanguageTo, forceReload } from 'Containers/Common/Actions'
import { setRelationsDetail } from 'Containers/Relations/ServiceAndChannels/Common/Actions'
import shortId from 'shortid'
import PublishingEntityDialog from 'appComponents/PublishingEntityDialog'
import { updateUI } from 'util/redux-ui/action-reducer'
// / PTV Components
import { PTVValidationManager, PTVConfirmDialog, PTVPreloader, PTVButton, PTVLabel, PTVIcon } from '../../Components'
import { LocalizedComboBox } from '../Common/localizedData'
import { OwnOrgSecurityPublish, OwnOrgSecurityDelete, OwnOrgSecurityRestore, OwnOrgSecurityWithdraw, SecurityCreate } from './../../appComponents/Security'
import VersionLinkContainer from './VersionLinkContainer'
import LanguageBar from 'appComponents/LanguageBar'
// / App Common
import { ButtonRestore,
  ButtonCancel,
  ButtonContinue,
  ButtonDone,
  ButtonDelete,
  ButtonPublish,
  ButtonWithdraw } from './Buttons'
import NotificationContainer from './NotificationContainer'

// / App Helperes
import StepContainer from './StepContainer'
import { publishingStatuses } from './Enums'
import { Link, browserHistory } from 'react-router'
import * as comonServiceActions from '../../Containers/Services/Common/Actions'

// selectors
import * as CommonSelectors from './Selectors'

const messages = defineMessages({
  linkGoBack: {
    id: 'Containers.Common.PageContainer.Link.Back',
    defaultMessage: 'Takaisin'
  },
  languageSelectionFromTitle: {
    id: 'Containers.Common.PageContainer.LanguageFrom.Title',
    defaultMessage: 'Vertailukieli'
  },
  languageSelectionToTitle: {
    id: 'Containers.Common.PageContainer.LanguageTo.Title',
    defaultMessage: 'Kielivalinta'
  },
  languageSelectionLink: {
    id: 'Containers.Common.PageContainer.Language.Link',
    defaultMessage: 'Vertaa kieliversioita'
  },
  availableLanguagesTitle: {
    id: 'Containers.Common.PageContainer.AvailableLanguages.Title',
    defaultMessage: 'Palvelun kieliversiot ({availableLanguagesCount})'
  }
})

class PageContainer extends Component {

  getChildContext = () => {
    return { ValidationManager: this.validationManager }
  };

  validationManager = new PTVValidationManager(this.props.intl.formatMessage);

  constructor (props) {
    super(props)
    this.state = {
      showConfirmDeleteDialog: false,
      showConfirmWithdrawDialog: false,
      showConfirmRestoreDialog: false,
      showConfirmCancelDialog: false,
      showConfirmGoBackDialog: false,
      showSaveDraft: false,
      translate: false,
      messages: new OrderedMap() }
  }

  goBack = () => {
    const editedStep = this.props.pageModeState.get('editedStep')
    if(editedStep || this.props.isNew){
      this.setState({showConfirmGoBackDialog: true})
    }
    else{
      browserHistory.push('/frontpage/search')
    }
  }

  handleAllReadOnlyStates = (value = false) => {
    this.props.steps.forEach((element, index) => {
        this.props.setStepModeActive(this.props.keyToState, index)
      })
  }

  handleSubmitSaveDraftDialog = () => {
    this.setState({
        showSaveDraft: false
      })
  }

  invalidateAllSteps = () => {
    if (typeof this.props.invalidateAllSteps === 'function') {
        this.props.invalidateAllSteps()
      }
    this.props.resetStepMode(this.props.keyToState)
  }

    // handleAllNotificationStates = (value = '') => {
    //     let newState = {}

    //     this.props.steps.forEach((element, index) => {
    //       newState['step' + (index + 1) + 'Notification'] = value;
    //     });

    //     this.setState(newState);
    // }

  handleSaveContainer = () => {
    this.setState({ messages: new OrderedMap() })
    if (typeof this.props.removeServerResultAction === 'function') {
        this.props.removeServerResultAction()
      }
        // this.props.resetAllMessage();
    setTimeout(() => {
        const result = this.validationManager.isManagerValid().then((data) => {
            if (data == true) {
            this.props.resetAllMessage()
            this.props.saveAction({ keyToState: this.props.keyToState, language: this.props.languageToCode })
            window.scrollTo(0, 0)
          } else {
            if (data != undefined) {
           this.setState({ messages: data })
         } else {
           this.setState({ messages: new OrderedMap({ message: 'Validation failed, try again!' }) })
         }
          }
          })
      }, 300)
  }

  renderConfirmDialogs = confirmDialogs => {
    const { formatMessage } = this.props.intl
    return confirmDialogs.map((confirmDialog) => {
        const [acceptButton, cancelButton, text] = confirmDialog.messages
        const keyToElement = this.props.keyToState + 'confirmDialog' + confirmDialog.type
        switch (confirmDialog.type) {
            case 'cancel': return (<PTVConfirmDialog
                show={this.state.showConfirmCancelDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => {
                            // this.invalidateAllSteps();
                            // this.setState({ messages: new OrderedMap()});
                            // this.props.steps.forEach((step) => {
                            //     this.callLoadActions(step.loadAction);
                            // });
                            // this.setState({showConfirmCancelDialog: false});
                            // this.handleAllNotificationStates();
                    browserHistory.push(this.props.searchPath ? this.props.basePath + this.props.searchPath : this.props.basePath)
                  }}
                cancelCallback={this.handleCancelOfCancelDialog}
                     />)
            case 'delete': return (<PTVConfirmDialog
                show={this.state.showConfirmDeleteDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => {
                    this.setState({ showConfirmDeleteDialog: false })
                    this.props.deleteAction(this.props.entityId, this.props.keyToState, this.props.languageToCode)
                  }}
                cancelCallback={this.handleCancelOfDeleteDialog}
                     />)
            case 'withdraw': return (<PTVConfirmDialog
                show={this.state.showConfirmWithdrawDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => {
                    this.setState({ showConfirmWithdrawDialog: false })
                    this.props.withdrawAction(this.props.entityId, this.props.keyToState, this.props.languageToCode)
                  }}
                cancelCallback={this.handleCancelOfWithdrawDialog}
                     />)
            case 'restore': return (<PTVConfirmDialog
                show={this.state.showConfirmRestoreDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => {
                    this.setState({ showConfirmRestoreDialog: false })
                    this.props.restoreAction(this.props.entityId, this.props.keyToState, this.props.languageToCode)
                  }}
                cancelCallback={this.handleCancelOfRestoreDialog}
                     />)
            case 'save': return (<PTVConfirmDialog
                show={this.state.showSaveDraft}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => {
                    if (typeof confirmDialog.acceptCallback === 'function') {
                            confirmDialog.acceptCallback()
                          }
                    this.setState({ showSaveDraft: false })
                  }}
                cancelCallback={this.handleCancelSaveDraftDialog}
                     />)
             case 'goBack': return (<PTVConfirmDialog
                show={this.state.showConfirmGoBackDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={() => browserHistory.push(this.props.basePath)}
                cancelCallback={this.handleCancelOfGoBacklDialog}
                     />)
            default: return (<PTVConfirmDialog
                show={confirmDialog.isShown ? confirmDialog.isShown : false}
                {...confirmDialog}
                     />)
          }
      })
  }
  renderCustomDialog = (render) => {
    if (typeof render === 'function') {
      return render()
    }
  }

    // confirm dialog handlers
  handleCancelOfDeleteDialog = () => {
    this.setState({ showConfirmDeleteDialog: false })
  }

  handleCancelOfWithdrawDialog = () => {
    this.setState({ showConfirmWithdrawDialog: false })
  }

  handleCancelOfRestoreDialog = () => {
    this.setState({ showConfirmRestoreDialog: false })
  }

  handleCancelSaveDraftDialog = () => {
    browserHistory.push(this.props.basePath)
  }

  handleCancelOfCancelDialog = () => {
    this.setState({ showConfirmCancelDialog: false })
  }

  handleCancelOfGoBacklDialog = () => {
    this.setState({ showConfirmGoBackDialog: false })
  }

    // confirm dialog handlers end

  renderButtons = (buttons, disabled, line) => {
    const publishingStatus = this.props.publishingStatus
    return buttons.map((button) => {
      const keyToElement = line + 'button' + button.type
      switch (button.type) {
        case 'cancel': {
          return <ButtonCancel key={keyToElement} disabled={disabled} onClick={this.handleCancelButton} />
        }
        case 'done':
        case 'continue': {
          const Element = button.type === 'continue' ? ButtonContinue : ButtonDone
          return <SecurityCreate keyToState={this.props.keyToState}>
            <Element key={keyToElement} disabled={disabled} onClick={() => { this.handleSaveContainer() }} />
          </SecurityCreate>
        }
        case 'delete': {
          return publishingStatus != null &&
            (publishingStatus === publishingStatuses.published || publishingStatus === publishingStatuses.draft || publishingStatus === publishingStatuses.modified)
            ? <OwnOrgSecurityDelete keyToState={this.props.keyToState} >
              <ButtonDelete {...button} key={keyToElement} disabled={disabled} onClick={this.handleDeleteButton} />
            </OwnOrgSecurityDelete>
            : null
        }
        case 'withdraw': {
          return publishingStatus != null &&
            (publishingStatus === publishingStatuses.published)
            ? <OwnOrgSecurityWithdraw keyToState={this.props.keyToState} >
              <ButtonWithdraw secondary {...button} key={keyToElement} disabled={disabled} onClick={this.handleWithdrawButton} />
            </OwnOrgSecurityWithdraw>
            : null
        }
        case 'publish': {
          return publishingStatus != null && (publishingStatus === publishingStatuses.draft || publishingStatus === publishingStatuses.modified)
            ? <OwnOrgSecurityPublish keyToState={this.props.keyToState} >
                <ButtonPublish {...button} key={keyToElement} disabled={disabled} onClick={this.handlePublishButton} />
              </OwnOrgSecurityPublish>
            : null
        }
        case 'restore': {
          return publishingStatus != null && (publishingStatus === publishingStatuses.oldPublished || publishingStatus === publishingStatuses.deleted)
            ? <OwnOrgSecurityRestore keyToState={this.props.keyToState} >
                <ButtonRestore {...button} key={keyToElement} disabled={disabled} onClick={this.handleRestoreButton} />
              </OwnOrgSecurityRestore>
            : null
        }
        default:
          return null
      }
    })
  }

  renderButtonGroup = (disabled = false, line) => {
    const buttons = this.props.readOnly ? [{ type: 'delete' }, { type: 'publish' }, { type: 'restore' }, { type: 'withdraw' }] : [{ type: 'cancel' }, { type: 'continue' }]

    return (
          <div className='button-group flex justify-content-end'>
              { !this.state.translate ? this.renderButtons(buttons, disabled, line) : null }
            </div>
        )
  }

  handleCustomClear = (id) => {
    return <PTVIcon name='icon-cross' componentClass='action' onClick={() => this.handleLanguageFromClear(id)} />
  }
  handleLanguageFromClear = (id) => {
    this.setState({ translate: false })
  }
  handleLanguageFromChange = (id) => {
    this.props.setLanguageFrom(this.props.keyToState, id)
  }
  handleLanguageToChange = (id) => {
    this.props.setLanguageTo(this.props.keyToState, id)
  }
  handleAddTranslation = () => {
    this.props.setTranslationMode(this.props.keyToState)
    this.setState({ translate: true })
  }
  renderTranslationGroup = (linkDisabled) => {
    const { formatMessage } = this.props.intl
    const { languages, languageFrom, languageTo, keyToState } = this.props
    return (
            this.props.isTranslatable && this.props.publishingStatus != publishingStatuses.deleted
              ? <div className='row'>
                <LocalizedComboBox
                 componentClass='col-xs-3'
                 value={languageTo}
                 values={languages}
                 label={formatMessage(messages.languageSelectionToTitle)}
                 changeCallback={this.handleLanguageToChange}
                 name='languagesTo'
                 readOnly={false}
                 clearable={false}
                 className='limited w280'
                />
                {!this.state.translate && !this.props.isNew && !linkDisabled
                  ? <div className='col-xs-3'>
                    <PTVLabel>
                        <PTVButton
                            type='link'
                            onClick={this.handleAddTranslation}> { formatMessage(messages.languageSelectionLink) }
                          </PTVButton>
                      </PTVLabel>
                  </div>
                : null}
                {this.state.translate
                  ? <LocalizedComboBox
                    componentClass='col-xs-3'
                    value={languageFrom}
                    values={languages}
                    label={formatMessage(messages.languageSelectionFromTitle)}
                    iconAction={() => this.handleCustomClear(languageFrom)}
                    changeCallback={this.handleLanguageFromChange}
                    name='languagesTo'
                    readOnly={false}
                    clearable={false}
                    className='limited w280'
                    />
                : null}
              </div> : null)
  }

    // Button handleres

  handleDeleteButton = () => {
    this.props.resetAllMessage()
    this.setState({ showConfirmDeleteDialog: true })
    window.scrollTo(0, 0)
  }

  handleWithdrawButton = () => {
    this.props.resetAllMessage()
    this.setState({ showConfirmWithdrawDialog: true })
    window.scrollTo(0, 0)
  }

  handleRestoreButton = () => {
    this.props.resetAllMessage()
    this.setState({ showConfirmRestoreDialog: true })
    window.scrollTo(0, 0)
  }

  handleCancelButton = () => {
        // this.props.removeServerResultAction();
        // this.props.resetAllMessage();
    this.setState({ showConfirmCancelDialog: true })
    window.scrollTo(0, 0)
  }

  handlePublishButton = () => {
    this.props.updateUI([this.props.keyToState], 'isOpen', true)
  }
    // Button handlers end

  getViewMode = (readOnly) => {
    const mode = (this.props.publishingStatus == publishingStatuses.deleted) ? ''

            : readOnly ? 'view' : this.props.isNew ? 'add' : 'edit'
            // console.log('mode', mode, readOnly)
    return mode
  }

  renderSteps = (buttonsDisabled, simpleView) => {
    const { formatMessage } = this.props.intl
    const editStepActivatedOnIndex = this.props.pageModeState && this.props.pageModeState.get('editedStep') ? this.props.isEntityLocked ? this.props.pageModeState.get('editedStep') : undefined : undefined
    return this.props.steps.map((step, index) => {
        const stepIndex = index + 1
        if (this['step' + stepIndex + 'updateCallback'] === undefined) {
                // var notificationState = 'step' + stepIndex + 'Notification'
                // this.state[notificationState] = '';
            this['step' + stepIndex + 'updateCallback'] = this.handleUpdateStep(stepIndex)
          }
        if (this.props.entityId && !this.props.isNew) {
            this['step' + stepIndex + 'saveCallback'] = this.handleSaveStep(this.props.entityId, stepIndex, step.saveStepAction, step.loadAction)
            this['step' + stepIndex + 'cancelUpdateCallback'] = this.handleCancelUpdateStep(this.props.entityId, stepIndex, step.loadAction)
                // this['step' + stepIndex + 'saveToLocalStorage'] = this.handleSaveChangesStep(step.objectToManipulate, 'step' + stepIndex + 'Form');
          }
        let readOnly = this.props.readOnly ? editStepActivatedOnIndex === undefined ? true : editStepActivatedOnIndex != stepIndex : false
        readOnly = this.props.isEntityLocked ? readOnly : true
        const publishingStatus = this.props.pageModeState ? Immutable.fromJS(this.props.pageModeState.get('publishStatus')) : null
        return !step.readOnlyVisible || this.props.readOnly ? <StepContainer
                        // saveChangesCallback={ this.handleSaveChangesStep()}
            mainTitle={step.mainTitle && step.mainTitleView ? this.props.readOnly ? formatMessage(step.mainTitleView) : formatMessage(step.mainTitle) : null}
            mainText={step.mainText && step.mainTextView ? this.props.readOnly ? formatMessage(step.mainTextView) : formatMessage(step.mainText) : null}
            subTitle={step.subTitle && step.subTitleView ? this.props.readOnly ? formatMessage(step.subTitleView) : formatMessage(step.subTitle) : null}
            entityId={this.props.entityId}
            unificRootId={this.props.unificRootId}
            showEntityId={stepIndex === 1 && !this.props.isNew}
            readOnly={readOnly}
            confirmDialogs={this.props.confirmDialogs}
            keyToState={this.props.keyToState}
            stepKey={step.stepKey || `step${stepIndex}Form`}
            areDataValid={step.areDataValid}
            loadAction={step.loadAction}
            key={this.props.keyToState + 'step' + stepIndex}
            notificationMessage={this.state['step' + stepIndex + 'Notification']}
            updateStepCallback={this.props.entityId ? this['step' + stepIndex + 'updateCallback'] : null}
            cancelUpdateStepCallback={this['step' + stepIndex + 'cancelUpdateCallback']}
            saveStepCallback={this['step' + stepIndex + 'saveCallback']}
            draftButton
            draftSaveUp={stepIndex == 1}
            translationMode={this.state.translate}
            mode={this.getViewMode(readOnly)}
            buttonsVisible={(this.state.translate && step.areActionsDisabledInCompare) ? false : !step.readOnlyVisible && this.props.isEditable}
            buttonsDisabled={step.isFetching || buttonsDisabled && editStepActivatedOnIndex !== stepIndex} >
            { step.isFetching ? <PTVPreloader /> : <step.child entityId={this.props.entityId} readOnly={readOnly} publishingStatus={publishingStatus} keyToState={this.props.keyToState} /> }
          </StepContainer> : null
      })
  }

  renderSimpleSteps = () => {
    const { formatMessage } = this.props.intl
    return this.props.steps.map((step, index) => {
         const stepIndex = index + 1
         const publishingStatus = this.props.pageModeState ? Immutable.fromJS(this.props.pageModeState.get('publishStatus')) : null
         return <StepContainer
            subTitle={formatMessage(step.subTitleView)}
            entityId={this.props.entityId}
            unificRootId={this.props.unificRootId}
            showEntityId={stepIndex === 1}
            readOnly
            keyToState={this.props.keyToState}
            simpleViewKeyToState={this.props.simpleViewKeyToState}
            stepKey={`step${stepIndex}Form`}
            areDataValid={step.areDataValid}
            loadAction={step.loadAction}
            key={this.props.keyToState + 'step' + stepIndex}
            translationMode={false}
            mode={'view'}
            simpleView={this.props.simpleView}
                        >
            { !step.areDataValid ? <PTVPreloader /> : <step.child entityId={this.props.entityId} simpleView={this.props.simpleView} readOnly publishingStatus={publishingStatus} keyToState={this.props.keyToState} /> }
          </StepContainer>
       })
  }

  handleUpdateStep = (stepIndex) => () => {
    this.props.clearApiCall([this.props.keyToState, 'lock'], { model: { entityLocked: false } })
    this.props.resetAllMessage()
    this.setInitialUpdateState(stepIndex)
    if (typeof this.props.lockAction === 'function') {
      this.props.lockAction(
          this.props.entityId,
          this.props.keyToState,
          ({ response }) => {
            const messages = response && response.messages || null
            if (messages && messages.errors && messages.errors.length === 0) {
              this.props.setStepModeActive(this.props.keyToState, stepIndex)
            }
        }
      )
    }
  }

  handleSaveStep = (id, stepIndex, saveAction) => () => {
    this.setInitialUpdateState(stepIndex)
    setTimeout(() => {
        const result = this.validationManager.isManagerValid().then((data) => {
            if (data == true) {
            this.props.resetAllMessage()
            saveAction({
              id,
              keyToState: this.props.keyToState,
              language: this.props.languageToCode,
              successNextAction: ({ response: { messages } }) => {
                if (this.props.steps[stepIndex-1] && this.props.steps[stepIndex-1].successAction) {
                  this.props.steps[stepIndex-1].successAction(id, this.props.languageToCode)
                }
                if (messages && messages.errors && messages.errors.length === 0) {
                  this.props.setStepModeInActive(this.props.keyToState)
                }
              }
            })
          } else {
            this.props.setStepModeActive(this.props.keyToState, stepIndex)
            if (data != undefined) {
                this.setState({ messages: data })
              } else {
                this.setState({ messages: new OrderedMap({ message: 'Validation failed, try again!' }) })
              }
          }
          })
      }, 300)
  }

  setInitialUpdateState = (stepIndex) => {
    this.setState({ messages: new OrderedMap() })
        // this.handleSubComponentNotification('', stepIndex);
       // this.props.removeServerResultAction();
  }

  callLoadActions = (loadActions, entityId, languageCode) => {
    if (Array.isArray(loadActions)) {
        loadActions.forEach((loadAction) => loadAction(entityId, this.props.languageToCode))
      } else if (typeof loadActions === 'function') {
          loadActions(entityId, languageCode)
        }
  }

  handleCancelUpdateStep = (entityId, stepIndex, loadAction) => () => {
    this.setInitialUpdateState(stepIndex)
    this.callLoadActions(loadAction, entityId, this.props.languageToCode)
    this.props.setStepModeInActive(this.props.keyToState)
    if (typeof this.props.unLockAction === 'function') {
        this.props.unLockAction(this.props.entityId, this.props.keyToState)
      }
  }

  callSpecialActions = (entityId) =>{
    if (typeof this.props.isLockedAction === 'function' && this.props.unificRootId) {
      this.props.isLockedAction(this.props.unificRootId, this.props.keyToState)
      }
    if (typeof this.props.isEditableAction === 'function' && entityId) {
        this.props.isEditableAction(entityId, this.props.keyToState)
      }
  }

  componentDidMount = () => {
        // console.log('did mount page')
    if (typeof this.props.removeServerResultAction === 'function') {
        this.props.removeServerResultAction()
      }
    this.props.resetAllMessage()

    if (this.props.isTranslatable) {
        this.props.loadTranslatableLanguages()
      }

    if (!this.props.publishStatusesLoad) {
        this.props.loadPublishingStatuses()
      }

    this.callSpecialActions(this.props.entityId)

    if (!this.props.simpleView) {
        window.scrollTo(0, 0)
      }
  }

  handleServerResult = nextProps => {
    const step = this.props.pageModeState && this.props.pageModeState.get('editedStep')
    if (step && this.props.steps && this.props.steps[step-1] && this.props.steps[step-1].areActionsDisabledInCompare && this.state.translate) {
      this.props.setStepModeInActive(this.props.keyToState)
    }
    if ((!nextProps.isNew && nextProps.entityId != this.props.entityId) || nextProps.forceReloadProperty && nextProps.entityId) {
        this.props.forceReload(this.props.keyToState, false)
        if(nextProps.unificRootId != this.props.unificRootId){
          this.setState({
              showSaveDraft: true
            })
        }
        this.props.steps.forEach((step) => {
            this.callLoadActions(step.loadAction, nextProps.entityId, this.props.languageToCode)
            if ( this.props.languageToCode !== this.props.languageFromCode) {
              this.callLoadActions(step.loadAction, nextProps.entityId, this.props.languageFromCode)
            }
          })
         this.callSpecialActions(nextProps.entityId)
      }
    if(nextProps.publishingStatus != this.props.publishingStatus){
      this.callSpecialActions(nextProps.entityId)
    }
  }

  componentWillUnmount = () => {
    this.props.forceReload(this.props.keyToState, true)
  }

  componentWillReceiveProps = (nextProps) => {
    this.handleServerResult(nextProps)
  }

    // getStepsIsFetching = () => {
    //     return this.props.steps.some(step => {
    //         // console.log(this.props.keyToState, step.isFetching);
    //         return step.isFetching == true;
    //     })
    // }

  onMessageClose = (id) => {
    this.props.resetMessages(id)
  }

  handleOnLanguageClick = (data) => {
    const keyToState = this.props.simpleViewKeyToState
    const entityKeyToState = this.props.keyToState
    this.props.setLanguageTo(keyToState, data.languageId)
  }

  render = () => {
    const { formatMessage } = this.props.intl
    const notification = this.props.notifications
    const step = this.props.pageModeState && this.props.pageModeState.get('editedStep')
    const editStepActivatedOnIndex = step ? this.props.isEntityLocked ? step : undefined : undefined
        // const isFetchingOfAnyStep = this.getStepsIsFetching();
    const buttonsDisabled = this.state.showConfirmCancelDialog
      || this.state.showConfirmDeleteDialog
      || this.state.showConfirmWithdrawDialog
      || this.state.showConfirmRestoreDialog
      || editStepActivatedOnIndex != undefined
      || this.props.isFetchingOfAnyStep
    const preloaderIsShown = this.props.isFetchingOfAnyStep
    return (!this.props.simpleView
                 ? <div className={cx('page-container', this.props.className)}>
                   { this.props.params && !this.state.showConfirmGoBackDialog
                      ? <PTVLabel>
                        <Link onClick={() => this.goBack()} >
                            <PTVIcon name='icon-angle-left' className='brand-fill' componentClass='top-align' />
                            {formatMessage(messages.linkGoBack)}
                          </Link>
                      </PTVLabel>
                    : null}
                   { preloaderIsShown ? <PTVPreloader /> : null}
                   <PublishingEntityDialog
                      type={this.props.keyToState}
                      name={this.props.keyToState}
                      entityId={this.props.entityId} />
                   <NotificationContainer
                     keyToState={this.props.keyToState}
                     validationMessages={this.state.messages.toArray()}
                        />
                   <NotificationContainer
                     keyToState={this.props.keyToState}
                     notificationKey='lock'
                        />
                    <NotificationContainer
                     keyToState={this.props.keyToState}
                     notificationKey='delete'
                        />
                    <VersionLinkContainer
                      keyToState={this.props.keyToState}
                      entitiesType={this.props.entitiesType}
                      location={this.props.location}
                      />
                   { this.renderCustomDialog(this.props.renderCustomDialog)}
                   { this.renderConfirmDialogs(this.props.confirmDialogs) }
                   { this.renderButtonGroup(buttonsDisabled, 'top') }
                   { this.renderTranslationGroup(buttonsDisabled) }
                   { this.renderSteps(buttonsDisabled) }
                   { this.props.children }
                   { this.renderButtonGroup(buttonsDisabled, 'bottom') }
                   <div className='clearfix' />
                 </div>
                : <div className={cx('page-container', this.props.className)}>
                  <div className='form-wrap'>
                    <div className='step-form'>
                      <div className='flex-container available-languages'>
                        <div>
                          <strong>{formatMessage(messages.availableLanguagesTitle, { availableLanguagesCount: this.props.availableLanguagesCount })}</strong>
                        </div>
                        <div>
                          {this.props.steps && this.props.steps[0] && this.props.steps[0].isFetching ? <PTVPreloader small /> :
                          <LanguageBar
                            onClick={this.handleOnLanguageClick}
                            languages={this.props.languagesAvailabilities}
                            selectedLanguage={this.props.selectedLanguage}
                            includeNames
                          />}
                        </div>
                      </div>
                    </div>
                  </div>
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
  // updateUI: PropTypes.func.isRequired
}

PageContainer.defaultProps = {
  confirmDialogs: [],
  editButtons: [],
  readOnlyButtons: [],
  readOnly: false
}

PageContainer.childContextTypes = {
  ValidationManager: React.PropTypes.object
}

function mapStateToProps (state, ownProps) {
  const pageModeState = CommonSelectors.getPageModeStateForKey(state, ownProps)
  const entityId = CommonSelectors.getPageEntityId(state, ownProps)
  const isNew = shortId.isValid(entityId) && entityId.length !== 36
  const languageTo = pageModeState.get('languageTo') || CommonSelectors.getTranslationLanguageId(state, 'fi')
  const languageToCode = CommonSelectors.getTranslationLanguageCode(state, { id: languageTo })
  const languageFrom  = pageModeState.get('languageFrom') || CommonSelectors.getTranslationLanguageId(state, 'fi')
  const languageFromCode  = CommonSelectors.getTranslationLanguageCode(state, { id: languageFrom })
  const unificRootId = ownProps.getUnificRootIdSelector(state, { ...ownProps, language : languageToCode })
  const publishingStatusId = CommonSelectors.getPublsihingStatusIdOfEntity(state, ownProps) ||
    ownProps.getEntityStatusSelector(state, { ...ownProps, language : languageToCode })
  const publishStatusesLoad = CommonSelectors.getPublishingStatuses(state).size > 0
  const isEditable = CommonSelectors.getEditableIsEditable(state, ownProps)
  const simpleViewKeyToState = ownProps.simpleView
    ? ownProps.entitiesType === 'services'
      ? 'serviceAndChannelServiceSearch'
      : 'serviceAndChannelChannelSearch'
    : ownProps.keyToState
  const languagesAvailabilities = CommonSelectors.getEntityLanguageVersions(state, { ...ownProps, id: entityId, simpleViewKeyToState } )
  const selectedLanguage = CommonSelectors.getLanguageTo(state, { keyToState: simpleViewKeyToState }) || CommonSelectors.getTranslationLanguageId(state, 'fi')
  return {
    isFetchingOfAnyStep: CommonSelectors.getIsFetchingOfAnyStep(state, ownProps),
    notifications: CommonSelectors.getCommonNotifications(state),
    pageModeState,
    publishingStatus : CommonSelectors.getPublishingStatusType(state, publishingStatusId),
    languages: CommonSelectors.getTranslationLanguagesObjectArray(state),
    isNew,
    entityId: !isNew ? entityId : null,
    unificRootId: !isNew ? unificRootId : null,
    readOnly: pageModeState.get('readOnly') || !isNew,
    languageFrom :pageModeState.get('languageFrom') || CommonSelectors.getTranslationLanguageId(state, 'fi'),
    languageTo,
    languageToCode,
    languageFrom,
    languageFromCode,
    publishStatusesLoad,
    isEntityLocked : !isNew ? CommonSelectors.getEntityLocked(state, ownProps) : true,
    isEditable,
    languagesAvailabilities,
    availableLanguagesCount: languagesAvailabilities.size,
    simpleViewKeyToState,
    selectedLanguage,
    forceReloadProperty: CommonSelectors.getForceReload(state, ownProps)
  }
}

const onRecordSelectV2 = (id, keyToState, languageTo) => onRecordSelect(id, keyToState, languageTo)

const actions = {
  updateUI,
  ...appActions,
  ...comonServiceActions,
  ...pageActions,
  setRelationsDetail,
  setLanguageTo,
  forceReload
}

export default compose(
  injectIntl,
  connect(mapStateToProps, actions))(PageContainer)
