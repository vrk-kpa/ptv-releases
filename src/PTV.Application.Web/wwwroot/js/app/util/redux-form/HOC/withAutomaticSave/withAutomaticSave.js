/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import moment from 'moment'
import { Notification, Spinner, Button } from 'sema-ui-components'
import {
  getFormValues,
  hasSubmitSucceeded,
  isSubmitting,
  initialize
} from 'redux-form/immutable'
import { asyncCallApi } from 'Middleware/Api'
import { convertToRaw, convertFromRaw, EditorState } from 'draft-js'
import { get, set } from 'lodash'
import { setReadOnly } from 'reducers/formStates'
import { fromJS } from 'immutable'
import { compose } from 'redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { apiCall3 } from 'actions'
import {
  getSelectedEntityId,
  getSelectedEntityConcreteType
} from 'selectors/entities/entities'
import { getContentLanguageId } from 'selectors/selections'
import { formActions, formActionsTypesEnum, formEntityTypes } from 'enums'
import { getUserEmail } from 'selectors/userInfo'
import {
  clearSavedFormInitialState,
  saveFormInitialState
} from 'reducers/automaticSave'
import {
  markImmutableObjectKeysWithItsType,
  convertMarkedImmutableObjectsToItsType
} from './util'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { Sticky, StickyContainer } from 'react-sticky'
import { entityConcreteTexts } from 'util/redux-form/messages'

const messages = defineMessages({
  timeStampText: {
    id: 'Util.ReduxForm.WithAutomaticSave.TimeStamp.Title',
    defaultMessage: 'Tallennettu väliaikaismuistiin'
  },
  resumeNotificationTitle: {
    id: 'Util.ReduxForm.WithAutomaticSave.ResumeNotification.Title',
    defaultMessage: 'Failed to save form'
  },
  resumeNotificationText: {
    id: 'Util.ReduxForm.WithAutomaticSave.ResumeNotification.Text',
    defaultMessage: 'The {entityType} you were editing is saved to temp memory but not to database.'
  },
  discardButtonTitle: {
    id: 'Components.Buttons.Discard',
    defaultMessage: 'Discard'
  },
  resumeButtonTitle: {
    id: 'Util.ReduxForm.WithAutomaticSave.Actions.ResumeEdit.Title',
    defaultMessage: 'Resume editing'
  }
})

const withAutomaticSave = ({ period = 10, draftJSFields }) => WrappedComponent => {
  class AutomaticSaveComponent extends Component {
    static propTypes = {
      isSubmitting: PropTypes.bool,
      isReadOnly: PropTypes.bool,
      isDisabled: PropTypes.bool,
      hasSubmitted: PropTypes.bool,
      hasSubmitSucceeded: PropTypes.bool,
      form: PropTypes.string,
      entityType: PropTypes.string,
      entityId: PropTypes.string,
      languageId: PropTypes.string,
      userName: PropTypes.string,
      initialize: PropTypes.func,
      turnOffReadOnly: PropTypes.func,
      setLatestFormState: PropTypes.func,
      clearSavedFormInitialState: PropTypes.func,
      formState: PropTypes.object,
      intl: intlShape,
      dispatch: PropTypes.func
    }
    // Getters //
    hasNewFormChanges = () => {
      return this.state.lastSavedStateJSON !== JSON.stringify(this.props.formState)
    }
    isNewLanguage = () => {
      return this.props.languageId !== this.languageId
    }
    canSave = () => {
      return !this.props.isSubmitting &&
             !this.props.hasSubmitted &&
             !this.props.isReadOnly &&
             !this.props.isDisabled &&
             !this.state.saving &&
             this.hasAllRequiredProps() &&
             this.hasNewFormChanges()
    }
    hasAllRequiredProps = () => {
      return this.props.form &&
             this.props.entityType &&
             this.props.entityId &&
             this.props.languageId &&
             this.props.userName
    }
    // Lifecycle //
    constructor (props) {
      super(props)
      this.saveInterval = null
      this.stateId = null // id of a state returned from server
      this.languageId = null // id of a state returned from server
      this.resume = false // info that resume is starting
      this.state = {
        lastSavedAt: null, // time of latest automatic save
        lastSavedStateJSON: null, // last saved state
        saving: false, // is currently saving form state
        loadingState: false, // loading state from server
        loadedState: null, // previous form state loaded from server
        notificationShown: false // is pop up shown
      }
    }
    async componentDidMount () {
      this.props.setLatestFormState()
      // Try to loadd saved previous state //
      const formStateId = await this.getPreviousStateId()
      // Previous state found, save it and ask user if he wants to use it //
      if (formStateId && this.props.isReadOnly) {
        this.stateId = formStateId
        this.showNotification()
      } else { // No previous state found start saving current one
        this.startAutomaticSave()
      }
    }
    componentWillUnmount () {
      this.stopAutomaticSave()
      this.props.clearSavedFormInitialState()
    }
    async componentWillReceiveProps ({ isReadOnly : nextIsReadOnly }) {
      if (!nextIsReadOnly && this.props.isReadOnly !== nextIsReadOnly) {
        if (this.stateId && !this.resume) { // if edit discard all states, resume without discarding //
          await this.onDiscard()
        }
        this.resume = false
      }
    }
    // Methods //
    saveState = async () => {
      if (!this.canSave()) return
      this.setState({ saving: true })
      try {
        const serializedFormState = JSON.stringify(this.props.formState)
        // This prevents initial unnecessary save //
        // in ideal case we would rely on isDirty flag //
        if (this.state.lastSavedStateJSON === null) {
          this.setState({ lastSavedStateJSON: serializedFormState })
          return
        }
        const automaticSaveState = {
          formName: this.props.form,
          state: serializedFormState,
          entityId: this.props.entityId,
          entityType: this.props.entityType
        }
        if (this.stateId) {
          automaticSaveState.id = this.stateId
        }

        const {
          id: savedStateId
        } = await asyncCallApi('formstate/save', automaticSaveState, this.props.dispatch)
        this.stateId = savedStateId
        this.setState({
          lastSavedAt: moment(),
          lastSavedStateJSON: serializedFormState
        })
      } catch (err) {
        console.warn('Saving form state failed')
      } finally {
        this.setState({ saving: false })
      }
    }
    showNotification = () => this.setState({ notificationShown: true })
    hideNotification = () => this.setState({ notificationShown: false })
    startAutomaticSave = () => {
      this.saveInterval = setInterval(this.saveState, period * 1000)
    }
    stopAutomaticSave = () => {
      clearInterval(this.saveInterval)
    }
    discardPreviousState = async () => {
      try {
        this.stateId = null

        await asyncCallApi('formstate/delete', { entityId: this.props.entityId }, this.props.dispatch)
      } catch (err) {
        console.warn(`Discarding form state failed: ${err}`)
      }
    }
    loadPreviousState = async () => {
      try {
        const { state } = await asyncCallApi('formstate/GetById', { id: this.stateId }, this.props.dispatch)
        return JSON.parse(state)
      } catch (err) {
        console.warn(`Loading previous state failed: ${err}`)
      }
    }
    getPreviousStateId = async () => {
      if (!this.hasAllRequiredProps()) {
        return null
      }
      try {
        const { id, exists } = await asyncCallApi('formstate/search', {
          formName: this.props.form,
          entityType: this.props.entityType,
          entityId: this.props.entityId,
          userName: this.props.userName
        }, this.props.dispatch)
        if (exists && id) {
          return id
        }
      } catch (err) {
        console.warn(`Checking for existence of previous state failed: ${err}`)
      }
      return null
    }
    onResume = async () => {
      this.setState({ loadingState: true })
      this.resume = true
      try {
        // Load previous state //
        let previousState = await this.loadPreviousState()
        previousState = _convertFromRaw(previousState)
        this.props.initialize(this.props.form, previousState)
        this.props.turnOffReadOnly()
      } catch (err) {
        console.warn(`Resuming from previous state failed: ${err}`)
      } finally {
        // Start overwriting previous one //
        this.hideNotification()
        this.setState({ loadingState: false })
        this.startAutomaticSave()
      }
    }
    onDiscard = async () => {
      this.hideNotification()
      try {
        // Delete previous state from server //
        await this.discardPreviousState()
      } catch (err) {
        console.warn(`Discarding previous state failed: ${err}`)
      }
      // Start saving current one //
      this.startAutomaticSave()
    }
    render () {
      const {
        lastSavedAt,
        saving,
        loadingState,
        notificationShown
      } = this.state
      const {
        intl: { formatMessage },
        entityType
      } = this.props
      const concreteType = entityConcreteTexts[entityType] && formatMessage(entityConcreteTexts[entityType]) || null
      return (
        <div>
          <span className={styles.timeStampWrap}>
            {saving
              ? <Spinner />
              : lastSavedAt &&
                <span>
                  <span>{formatMessage(messages.timeStampText)}</span>
                  <span className={styles.timeStamp}>{lastSavedAt.format('H:mm:ss')}</span>
                </span>
            }
          </span>
          <StickyContainer>
            <div className={styles.wrap}>
              <Sticky>
                {notificationShown &&
                  <div className={styles.notification}>
                    <Notification
                      title={formatMessage(messages.resumeNotificationTitle)}
                      description={formatMessage(messages.resumeNotificationText, { entityType: concreteType })}
                      type='ok'
                    >
                      <div className={styles.buttonGroup}>
                        <Button link onClick={this.onResume}>
                          {loadingState
                            ? <Spinner className={styles.blockCenter} />
                            : formatMessage(messages.resumeButtonTitle)
                          }
                        </Button>
                        <Button link onClick={this.onDiscard}>
                          {formatMessage(messages.discardButtonTitle)}
                        </Button>
                      </div>
                    </Notification>
                  </div>
                }
              </Sticky>
              <WrappedComponent {...this.props} />
            </div>
          </StickyContainer>
        </div>
      )
    }
  }
  const _convertToRaw = formValues => {
    draftJSFields.forEach(draftJSField => {
      formValues = formValues.updateIn(draftJSField.split('.'), editor => {
        return editor.map(languageEditor => {
          const currentContent = languageEditor && languageEditor.getCurrentContent() || null
          return currentContent && convertToRaw(currentContent)
        })
      })
    })
    return markImmutableObjectKeysWithItsType(formValues).toJS()
  }
  const _convertFromRaw = formValues => {
    draftJSFields.forEach(draftJSField => {
      Object.keys(get(formValues, draftJSField)).map(languageKey => {
        const languagePath = `${draftJSField}.${languageKey}`
        set(formValues, languagePath,
          EditorState.createWithContent(
            convertFromRaw(
              get(formValues, languagePath)
            )
          )
        )
      })
    })
    return convertMarkedImmutableObjectsToItsType(fromJS(formValues))
  }
  return compose(
    injectIntl,
    injectFormName,
    withFormStates,
    connect(
      (state, { form }) => {
        const getFormState = getFormValues(form)
        const getIsSubmiting = isSubmitting(form)
        const getHasSubmitSucceeded = hasSubmitSucceeded(form)
        let formState = getFormState(state)
        formState = _convertToRaw(formState)
        return {
          formState,
          entityId: getSelectedEntityId(state),
          entityType: getSelectedEntityConcreteType(state),
          languageId: getContentLanguageId(state),
          hasSubmitted: getHasSubmitSucceeded(state),
          isSubmitting: getIsSubmiting(state),
          userName: getUserEmail(state)
        }
      },
      (dispatch, { formName, entityId }) => ({
        initialize: (...args) => dispatch(initialize(...args)),
        setLatestFormState: () => dispatch(({ getState }) => {
          const state = getState()
          const getFormState = getFormValues(formName)
          const formState = getFormState(state)
          dispatch(saveFormInitialState(formState))
        }),
        clearSavedFormInitialState: (...args) => dispatch(clearSavedFormInitialState(...args)),
        turnOffReadOnly: () => {
          dispatch(
            apiCall3({
              keys: [formEntityTypes[formName], 'loadEntity'],
              payload: {
                endpoint: formActions[formActionsTypesEnum.LOCKENTITY][formName],
                data: { id: entityId }
              },
              formName,
              successNextAction: () => {
                dispatch(
                  setReadOnly({
                    form: formName,
                    value: false
                  })
                )
              }
            })
          )
        }
      })
    )
  )(AutomaticSaveComponent)
}

export default withAutomaticSave
