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
import React, { Component, Fragment } from 'react'
import PropTypes from 'prop-types'
import { change } from 'redux-form/immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withPath from 'util/redux-form/HOC/withPath'
import withState from 'util/withState'
import { Modal, ModalContent, ModalActions, Button } from 'sema-ui-components'
import { getSelectedEntityConcreteType } from 'selectors/entities/entities'
import { getIsAccessibilityRegisterField } from './selectors'
import { entityConcreteTypesEnum, formTypesEnum } from 'enums'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'

const messages = defineMessages({
  promptText: {
    id: 'Util.ReduxForm.HOC.withAccessibilityPrompt.Content',
    defaultMessage: (`
You are changing the visiting address that includes accessibility information. If the service location has moved to new premises, the accessibility information may not apply.

Is the new address in the same building? Select 'Yes' (keep the accessibility information).

Select 'No' (You will lose the accessibility information).
    `)
  },
  noButtonTitle: {
    id: 'Util.ReduxForm.HOC.withAccessibilityPrompt.No',
    defaultMessage: 'Poista tiedot'
  },
  yesButtonTitle: {
    id: 'Util.ReduxForm.HOC.withAccessibilityPrompt.Yes',
    defaultMessage: 'Pidä tiedot'
  },
  cancelButtonTitle: {
    id: 'Util.ReduxForm.HOC.withAccessibilityPrompt.Cancel',
    defaultMessage: 'Peruuta'
  }
})

const withAccessibilityPrompt = ComposedComponent => {
  class InnerComponent extends Component {
    constructor (props) {
      super(props)
      this.state = { isOpen: false }
    }
    get shouldPrompt () {
      const {
        didAnswer,
        selectedEntityType,
        isAccessibilityRegisterField
      } = this.props
      return (
        isAccessibilityRegisterField &&
        !didAnswer &&
        selectedEntityType === entityConcreteTypesEnum.SERVICELOCATIONCHANNEL
      )
    }
    openModal = () => this.setState({ isOpen: true })
    closeModal = () => this.setState({ isOpen: false })
    setAccessibilityRegisterMeta = ({ isChanged, isDeleted }) => {
      const { updateUI, change } = this.props
      updateUI('didAnswer', true)
      change(
        formTypesEnum.SERVICELOCATIONFORM,
        'accessibilityMeta.isChanged',
        isChanged
      )
      change(
        formTypesEnum.SERVICELOCATIONFORM,
        'accessibilityMeta.isDeleted',
        isDeleted
      )
      this.closeModal()
    }
    handleIsNewAddress = () => {
      this.setAccessibilityRegisterMeta({
        isChanged: true,
        isDeleted: false
      })
    }
    handleIsSameAddress = () => {
      this.setAccessibilityRegisterMeta({
        isChanged: true,
        isDeleted: true
      })
    }
    handleOnBlur = (trigger, onEvent) => {
      if (this.shouldPrompt && trigger) {
        this.openModal()
      }
      onEvent && onEvent()
    }
    render () {
      const { isOpen } = this.state
      const { intl: { formatMessage }, triggerOnBlur, triggerOnChange, onChange } = this.props
      return (
        <Fragment>
          <ComposedComponent
            {...this.props}
            onBlur={() => this.handleOnBlur(triggerOnBlur, () => this.props.onBlur && this.props.onBlur(this.props))}
            onChange={(data) => this.handleOnBlur(triggerOnChange, () => onChange && onChange(data, this.props))}
          />
          <Modal
            isOpen={isOpen}
            onRequestClose={this.closeModal}
          >
            <ModalContent>
              {formatMessage(messages.promptText)}
            </ModalContent>
            <ModalActions>
              <Button small onClick={this.handleIsNewAddress}>
                {formatMessage(messages.yesButtonTitle)}
              </Button>
              <Button small onClick={this.handleIsSameAddress}>
                {formatMessage(messages.noButtonTitle)}
              </Button>
              <Button link onClick={this.closeModal}>
                {formatMessage(messages.cancelButtonTitle)}
              </Button>
            </ModalActions>
          </Modal>
        </Fragment>
      )
    }
  }
  InnerComponent.propTypes = {
    didAnswer: PropTypes.bool,
    selectedEntityType: PropTypes.string,
    change: PropTypes.func,
    updateUI: PropTypes.func,
    path: PropTypes.string,
    isAccessibilityRegisterField: PropTypes.bool,
    intl: intlShape
  }
  return compose(
    withPath,
    connect(
      (state, { path }) => ({
        selectedEntityType: getSelectedEntityConcreteType(state),
        isAccessibilityRegisterField: getIsAccessibilityRegisterField(state, { path })
      }), {
        change
      }
    ),
    withState({
      key: 'accessibilityRegisterPrompt',
      redux: true,
      initialState: {
        didAnswer: false
      }
    }),
    injectIntl
  )(InnerComponent)
}

export default withAccessibilityPrompt
