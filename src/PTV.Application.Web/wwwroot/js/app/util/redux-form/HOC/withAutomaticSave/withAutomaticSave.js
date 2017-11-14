/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
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
import { Notification } from 'sema-ui-components'
import { getFormValues, hasSubmitSucceeded, isSubmitting, initialize } from 'redux-form/immutable'
import callApi from 'util/callApi'
import { convertToRaw, convertFromRaw, EditorState } from 'draft-js'
import { get, set } from 'lodash'
import { fromJS } from 'immutable'
import {
  getSelectedEntityId,
  getSelectedEntityConcreteType
} from 'selectors/entities/entities'
import { getContentLanguageId } from 'selectors/selections'
import { getUserName } from 'selectors/userInfo'

const withAutomaticSave = ({ period = 10, draftJSFields }) => WrappedComponent => {
  class WrappedComponentInner extends Component {
    static propTypes = {
      isSubmitting: PropTypes.bool,
      hasSubmitted: PropTypes.bool,
      hasSubmitSucceeded: PropTypes.bool,
      form: PropTypes.string,
      entityType: PropTypes.string,
      entityId: PropTypes.string,
      languageId: PropTypes.string,
      userName: PropTypes.string,
      initialize: PropTypes.func
    }
    constructor (...args) {
      super(...args)
      this.saveInterval = null
      this.stateId = null // id of a state returned from server
      this.state = {
        latestSave: null, // time of latest automatic save
        saving: false, // is currently saving form state
        loadingState: false, // loading state from server
        loadedState: null, // previous form state loaded from server
        notificationShown: false // is pop up shown
      }
    }
    saveState = async () => {
      // Previous save request is still in flight //
      if (this.state.saving) {
        return
      }
      // Form is successfully submited or is being submited
      if (this.props.isSubmitting || this.props.hasSubmitted) {
        return
      }
      this.setState({ saving: true })
      const { form, formState, entityId, entityType, languageId } = this.props
      try {
        if (!form || !formState || !entityId || !entityType || !languageId) {
          return
        }
        const automaticSaveState = {
          formName: form,
          state: JSON.stringify(formState),
          entityId,
          entityType,
          languageId
        }
        if (this.stateId) {
          automaticSaveState.id = this.stateId
        }
        const { id: savedStateId } = await callApi('formstate/save', automaticSaveState)
        this.stateId = savedStateId
        this.setState({ latestSave: moment() })
      } catch (err) {
        console.warn('Saving form state failed')
      } finally {
        this.setState({ saving: false })
      }
    }
    showNotification = () => this.setState({ notificationShown: true })
    hideNotification = () => this.setState({ notificationShown: false })
    startAutomaticSave = () => {
      this.saveInterval = setInterval(() => this.saveState(), period * 1000)
    }
    stopAutomaticSave = () => {
      clearInterval(this.saveInterval)
    }
    discardPreviousState = async () => {
      try {
        await callApi('formstate/delete', { id: this.stateId })
      } catch (err) {
        console.warn(`Discarding form state failed: ${err}`)
      }
    }
    loadPreviousState = async () => {
      try {
        const { state } = await callApi('formstate/GetById', { id: this.stateId })
        return JSON.parse(state)
      } catch (err) {
        console.watn(`Loading previous state failed: ${err}`)
      }
    }
    getPreviousStateId = async () => {
      try {
        const { id, exists } = await callApi('formstate/search', {
          formName: this.props.form,
          entityType: this.props.entityType,
          entityId: this.props.entityId,
          languageId: this.props.languageId,
          userName: this.props.userName
        })
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
      try {
        // Load previous state //
        let previousState = await this.loadPreviousState()
        previousState = _convertFromRaw(previousState)
        this.props.initialize(this.props.form, previousState)
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

    async componentDidMount () {
      // Try to loadd saved previous state //
      const formStateId = await this.getPreviousStateId()
      // Previous state found, save it and ask user if he wants to use it //
      if (formStateId) {
        this.stateId = formStateId
        this.showNotification()
      } else { // No previous state found start saving current one
        this.startAutomaticSave()
      }
    }
    componentWillUnmount () {
      this.stopAutomaticSave()
    }
    render () {
      const {
        latestSave,
        saving,
        loadingState,
        notificationShown
      } = this.state
      return (
        <div>
          {saving
            ? <span>saving...</span>
            : <span>{latestSave && latestSave.format('H:mm:ss')}</span>}
          {notificationShown &&
            <Notification
              title='Lomakkeen lähetys epäonnistui!'
              description='Verkkoyhteys palvelimeen katkesi kesken lähetyksen. '
              borderStyle='borderRed'
              retryLabel='Yritä uudelleen klikkaamalla tästä'
            >
              <b style={{ cursor: 'pointer' }} onClick={this.onResume}>
                {loadingState
                  ? 'Resuming...'
                  : 'Resume editing'}
              </b>
              <b style={{ cursor: 'pointer' }} onClick={this.onDiscard}>Discard</b>
            </Notification>}
          <WrappedComponent {...this.props} />
        </div>
      )
    }
  }
  const _convertToRaw = formValues => {
    draftJSFields.forEach(draftJSField => {
      formValues = formValues.updateIn(draftJSField.split('.'), editor => {
        return editor.map(languageEditor => {
          const currentContent = languageEditor.getCurrentContent()
          return convertToRaw(currentContent)
        })
      })
    })
    return formValues.toJS()
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
    return fromJS(formValues)
  }
  return connect(
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
        userName: getUserName(state)
      }
    }, {
      initialize
    }
  )(WrappedComponentInner)
}

export default withAutomaticSave
