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
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { PTVLabel, PTVConfirmDialog } from '../../Components'
import { SecurityUpdate, OwnOrgSecurityUpdate } from '../../appComponents/Security'
import { ButtonSaveDraft, ButtonUpdate, ButtonCancelUpdate } from './Buttons'
import NotificationContainer from './NotificationContainer'
import './Styles/StepContainer.scss'
import cx from 'classnames'

// selectors
import * as CommonSelectors from './Selectors'

const messages = defineMessages({
  saveButton: {
    id: 'Components.StepContainer.SaveButton',
    defaultMessage: 'Tallenna luonnos'
  }
})
class StepContainer extends Component {
  static propTypes = {
    intl: intlShape.isRequired,
    saveChangesCallback: PropTypes.func,
    readOnly: React.PropTypes.bool
  };

  static defaultProps = {
    readOnly: false
  }
  constructor (props) {
    super(props)
    this.onHandleSaveChanges = this.onHandleSaveChanges.bind(this)
    this.onHandleUpdateStep = this.onHandleUpdateStep.bind(this)
    this.onHandleCancelUpdateStep = this.onHandleCancelUpdateStep.bind(this)
    this.onHandleSaveStep = this.onHandleSaveStep.bind(this)
    this.state = {
      showConfirmDeleteDialog: false
    }
  }

  callLoadActions = (loadActions, langCode) => {
    if (Array.isArray(loadActions)) {
      loadActions.forEach((loadAction) => loadAction(this.props.entityId, langCode, this.props.keyToState))
    } else if (typeof loadActions === 'function') {
      loadActions(this.props.entityId, langCode, this.props.keyToState)
    }
  }

  componentDidMount () {
    if (!this.props.areDataValid) {
      this.callLoadActions(this.props.loadAction, this.props.languageToCode)
    }
  }

  componentWillReceiveProps = (nextProps) => {
    if (this.props.languageTo != '' && (this.props.languageTo != nextProps.languageTo)) {
      this.callLoadActions(this.props.loadAction, nextProps.languageToCode)
    }
    if (this.props.languageFrom != '' && (this.props.languageFrom != nextProps.languageFrom || (!this.props.translationMode && nextProps.translationMode))) {
      this.callLoadActions(this.props.loadAction, nextProps.languageFromCode)
    }
  }

  onHandleSaveChanges () {
    if (this.props.saveChangesCallback) {
      this.props.saveChangesCallback()
    }
  }
  onHandleUpdateStep () {
    this.props.updateStepCallback()
  }
  onHandleCancelUpdateStep () {
    this.setState({ showConfirmCancelDialog: true })
  }
  onHandleSaveStep () {
    this.props.saveStepCallback()
  }

  renderButtons = () => {
    return this.props.buttonsVisible ? this.returnButtonComponents() : null
  }

  returnButtonComponents = () => {
    const btnGroupClass = cx('button-group', this.props.mode)

    switch (this.props.mode) {
      case 'add':
                // temporary removed REL 1.2
        return null
      case 'view':
        return (
          <OwnOrgSecurityUpdate keyToState={this.props.keyToState}>
            <div className={btnGroupClass}>
              <ButtonUpdate className='up' onClick={this.props.updateStepCallback ? this.onHandleUpdateStep : null} disabled={this.props.buttonsDisabled} />
            </div>
          </OwnOrgSecurityUpdate>
        )
      case 'edit':
        return (
          <SecurityUpdate keyToState={this.props.keyToState}>
            <div className={btnGroupClass}>
              <ButtonCancelUpdate className='up' onClick={this.onHandleCancelUpdateStep} disabled={this.props.buttonsDisabled} />
              <ButtonSaveDraft className='up' onClick={this.onHandleSaveStep} disabled={this.props.buttonsDisabled} />
            </div>
          </SecurityUpdate>
        )
      default:
        return null

    }
  }

  renderTranslationContent = () => {
    const translationModeOriginal = this.props.readOnly ? 'none' : 'view'
    const translationModeNew = this.props.readOnly ? 'none' : 'edit'
    const originalReadOnlyContent = React.cloneElement(this.props.children, { readOnly: true, positionId: 'left', language: this.props.languageFromCode, translationMode: translationModeOriginal, splitContainer:true })
    const translatedContent = React.cloneElement(this.props.children, { language: this.props.languageToCode, positionId: 'right', translationMode: translationModeNew, splitContainer:true })
    return (
      <div className='step-form '>
        <div className='row'>
          <div className='col-xs-6'>{translatedContent}</div>
          <div className='col-xs-6'>{originalReadOnlyContent}</div>
        </div>
      </div>
    )
  }
  renderLanguageContent = () => {
    const languageContent = React.cloneElement(this.props.children, { language: this.props.languageToCode, positionId: 'center', translationMode: 'none', splitContainer:false })
    return languageContent
  }

  handleCancelOfCancelDialog = () => {
    this.setState({ showConfirmCancelDialog: false })
  }

  handleAcceptOfCancelDialog = () => {
    this.setState({ showConfirmCancelDialog: false })
    this.props.cancelUpdateStepCallback()
  }

  renderConfirmDialogs = confirmDialogs => {
    const { formatMessage } = this.props.intl
    return confirmDialogs.map((confirmDialog) => {
        const [acceptButton, cancelButton, text] = confirmDialog.messages
        const keyToElement = this.props.keyToState + 'confirmDialogOnStep' + confirmDialog.type
        switch (confirmDialog.type) {
            case 'cancel': return (<PTVConfirmDialog
                show={this.state.showConfirmCancelDialog}
                acceptButton={formatMessage(acceptButton)}
                cancelButton={formatMessage(cancelButton)}
                text={formatMessage(text)}
                key={keyToElement}
                acceptCallback={this.handleAcceptOfCancelDialog}
                cancelCallback={this.handleCancelOfCancelDialog}
                     />)
            default: return (null)
          }
      })
  }

  render () {
    const { formatMessage } = this.props.intl
    const stepClass = cx('step-container', { 'readonly': this.props.readOnly, 'translation': this.props.translationMode })
    return (
      <div className={stepClass}>
        <div className='step-container-header'>
          {this.props.mainTitle ? <h2>{this.props.mainTitle}</h2> : null}
          {this.props.mainText ? <p className='main-text'>{this.props.mainText}</p> : null}
        </div>
        <div className='step-container-actions clearfix'>
          <h3 className='step-container-actions-label'>{this.props.subTitle}</h3>
          { !this.state.showConfirmCancelDialog ? this.renderButtons() : null }
        </div>
        { this.state.showConfirmCancelDialog ? this.renderConfirmDialogs(this.props.confirmDialogs) : null }
        <NotificationContainer keyToState={this.props.keyToState} notificationKey={this.props.stepKey} />

        <div className='box box-white'>
          <div className={cx('form-wrap', this.props.keyToState)}>
            { this.props.showEntityId &&
              <div className='entity-metadata'>
                <div className='entity-id-root'>
                  <strong>ID: </strong>
                  <PTVLabel copyToClipboard>{ this.props.unificRootId }</PTVLabel>
                </div>
                <div className='entity-id-version'>
                  <strong>IDv: </strong>
                  <PTVLabel copyToClipboard>{ this.props.entityId }</PTVLabel>
                </div>
              </div>
                    }
            { this.props.translationMode ? this.renderTranslationContent() : this.renderLanguageContent() }
          </div>
        </div>
        <div className='button-group'>
          { !this.state.showConfirmCancelDialog ? this.renderButtons() : this.renderConfirmDialogs(this.props.confirmDialogs) }
        </div>
      </div>
    )
  }
}
function mapStateToProps (state, ownProps) {
  const languageFrom = CommonSelectors.getLanguageFrom(state, ownProps) || CommonSelectors.getTranslationLanguageId(state, 'fi')
  const languageTo = ownProps.simpleView ? CommonSelectors.getLanguageTo(state, { keyToState: ownProps.simpleViewKeyToState }) ||
    CommonSelectors.getTranslationLanguageId(state, 'fi') : CommonSelectors.getLanguageTo(state, ownProps) || CommonSelectors.getTranslationLanguageId(state, 'fi')
  return {
    languageFrom :languageFrom,
    languageTo :languageTo,
    languageFromCode: CommonSelectors.getTranslationLanguageCode(state, { id: languageFrom }),
    languageToCode: CommonSelectors.getTranslationLanguageCode(state, { id: languageTo })
  }
}
export default connect(mapStateToProps)(injectIntl(StepContainer))
