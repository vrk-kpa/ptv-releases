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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import * as signalR from '@aspnet/signalr'
import { getSignalRHubConnection } from 'selectors/signalR'
import { setHubConnection } from 'reducers/signalR'
import { isArray } from 'util'
import { getAuthToken } from 'Configuration/AppHelpers'
import { property } from 'lodash'
import { getShowErrorsAction, getShowInfosAction } from 'reducers/notifications'

const handleMessages = (messageType, callback, response, dispatch) => {
  const messages = property(`messages.${messageType}`)(response)
  if (messages && messages.length > 0) {
    dispatch(callback(messages))
  }
}

const withSignalRHub = ({
  hubName,
  formName,
  hubLink,
  actionDefinition
}) => WrappedComponent => {
  class SignalREnhancement extends Component {
    signalRhandleMessages = (message) => {
      handleMessages('errors', getShowErrorsAction(formName, true), message, this.props.dispatch)
      handleMessages('infos', getShowInfosAction(formName, true), message, this.props.dispatch)
    }

    start = () => {
      getAuthToken() && this.hubConnection.start()
        .then(() => console.log('SignalR started'))
        .catch(err => {
          console.log('SignalRConnection error: ', err, err.statusCode)
          if (err.statusCode !== 401 && err.statusCode !== 0) {
            setTimeout(() => this.start(), 500)
          }
        })
    }
    componentDidMount () {
      if (!this.props.hubConnection) {
        this.hubConnection = new signalR.HubConnectionBuilder()
          .withUrl(hubLink, { accessTokenFactory: () => getAuthToken() })
          .configureLogging(signalR.LogLevel.Warning)
          .build()
        if (actionDefinition && isArray(actionDefinition)) {
          actionDefinition.forEach((element) => {
            if (element.type && element.action && typeof element.action === 'function') {
              this.hubConnection.on(element.type, element.action(this.props))
            }
          })
        }

        if (formName) {
          this.hubConnection.on('RServiceResult', this.signalRhandleMessages)
        }

        this.hubConnection.onclose(() => this.start())

        this.start()

        this.props.dispatch(setHubConnection(hubName, this.hubConnection))
      } else {
        this.hubConnection = this.props.hubConnection
      }
    }

    componentWillUnmount () {
      this.hubConnection.stop()
      this.hubConnection = undefined
      this.props.dispatch(setHubConnection(hubName, undefined))
    }

    render () {
      return <WrappedComponent {...this.props} hubName={hubName} hubConnection={this.hubConnection} />
    }
  }
  SignalREnhancement.propTypes = {
    hubConnection: PropTypes.object,
    dispatch: PropTypes.func.isRequired
  }
  return compose(
    connect(
      (state) => ({
        hubConnection: getSignalRHubConnection(state, { hubName })
      })
    )
  )(SignalREnhancement)
}

export default withSignalRHub
