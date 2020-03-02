/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import * as signalR from '@microsoft/signalr'
import { getSignalRHubConnection } from 'selectors/signalR'
import { setHubConnection, clearHubRequestSent } from 'reducers/signalR'
import { isArray } from 'util'
import { getAuthToken } from 'Configuration/AppHelpers'
import { showResponseMessages } from 'actions'
import { start, invokeHub } from './actions'

const withSignalRHub = ({
  hubName,
  showNotification,
  hubLink,
  onErrorAction,
  onInfoAction,
  onWarningAction,
  actionDefinition,
  messageReceived
}) => WrappedComponent => {
  class SignalREnhancement extends Component {
    signalRhandleMessages = (response) => {
      this.props.dispatch(
        showResponseMessages({
          key: hubName,
          showNotification,
          onErrorAction,
          onInfoAction,
          onWarningAction,
          messages: response.messages
        })
      )
    }

    invoke = (action, data) => {
      this.props.dispatch(invokeHub({ hubConnection: this.hubConnection, hubName, action, data }))
    }

    receiveMessage = (message) => {
      this.props.dispatch(clearHubRequestSent({ hubName }))
      typeof messageReceived === 'function' && messageReceived(message, this.props)
    }

    componentDidMount () {
      if (!this.props.hubConnection) {
        this.hubConnection = new signalR.HubConnectionBuilder()
          // .withUrl(hubLink, { accessTokenFactory: () => getAuthToken() })
          .withUrl(`${hubLink}?access_token=${getAuthToken()}`)
          .configureLogging(signalR.LogLevel.Warning)
          .build()
        if (actionDefinition && isArray(actionDefinition)) {
          actionDefinition.forEach((element) => {
            if (element.type && element.action && typeof element.action === 'function') {
              this.hubConnection.on(element.type, element.action(this.props))
            }
          })
        }

        this.hubConnection.on('ReceiveMessage', this.receiveMessage)

        if (showNotification) {
          this.hubConnection.on('RServiceResult', this.signalRhandleMessages)
        }

        this.hubConnection.onclose(() => start(this.hubConnection))

        start(this.hubConnection)

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
      return <WrappedComponent
        {...this.props}
        hubName={hubName}
        hubConnection={this.hubConnection}
        hubInvoke={this.invoke}
      />
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
