
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
import { Button } from 'sema-ui-components'
import { PTVIcon } from 'Components'
import { connect } from 'react-redux'
import { formTypesEnum } from 'enums'
import { EntitySelectors } from 'selectors'
import { createSelector } from 'reselect'
import { formValueSelector, change, fieldArrayPropTypes } from 'redux-form/immutable'
import styles from './styles.scss'
import Popup from 'appComponents/Popup'
import withState from 'util/withState'
import cx from 'classnames'
import { compose } from 'redux'
import { Map, List } from 'immutable'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { getAccessibilityRegisters } from 'util/redux-form/fields/Accessibility/selectors'
import { API_CALL_CLEAN } from 'Containers/Common/Actions'
import withFormStates from 'util/redux-form/HOC/withFormStates'

export const messages = defineMessages({
  confirmDeleteMessage: {
    id : 'Util.ReduxForm.HOC.AsCollection.AccessibilityRemoveButton.ConfirmRemove',
    defaultMessage: 'Are you sure you want to delete accessibility?'
  },
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Buttons.Cancel.Title',
    defaultMessage: 'Peruuta'
  },
  removalPlaceholder: {
    id: 'Common.HOC.AsCollection.RemovalPlaceholder',
    defaultMessage: 'Tämän elementin poistaminen vaikuttaa muihin kieliversioihin, sillä toisessa kieliversiossa on sisältöä.'
  }
})

class AccessibilityRemoveButton extends Component {
  shouldShow = () => {
    return !this.props.isReadOnly &&
      (!this.props.isAccessibilityRegsiterAddress || this.props.isMainEntrance)
  }
  handleOnRemove = () => {
    const {
      index,
      fields,
      fields: { length },
      updateUI,
      dispatch
    } = this.props
    updateUI(
      'focusIndex',
      length === 1
        ? 0
        : length - 2
    )
    if (index === 0) {
      dispatch({
        type: API_CALL_CLEAN,
        keys: ['channel', 'addresses', formTypesEnum.SERVICELOCATIONFORM]
      })
    }
    fields.remove(index)
  }
  handleOnAccessibilityRegisterAddressRemove = () => {
    const {
      fields,
      visitingAddresses,
      updateUI,
      changeFormValue
    } = this.props
    if (fields.length <= 0) {
      return
    }
    updateUI('isOpen', false)
    const newVisitingAddresses = visitingAddresses
      .filter(visitingAddress => !visitingAddress.get('accessibilityRegister'))
    changeFormValue(
      formTypesEnum.SERVICELOCATIONFORM,
      'visitingAddresses',
      newVisitingAddresses
    )
    changeFormValue(
      formTypesEnum.SERVICELOCATIONFORM,
      'accessibilityMeta.isDeleted',
      true
    )
    changeFormValue(
      formTypesEnum.SERVICELOCATIONFORM,
      'accessibilityMeta.isChanged',
      false
    )
  }
  handleOnOpen = () => this.props.updateUI('isOpen', true)
  handleOnClose = () => this.props.updateUI('isOpen', false)
  get collectionItemRemoveClass () {
    const shouldHideRemove = this.props.fields.length === 0 ||
                             this.props.innerShouldHideControls
    return cx(
      styles.collectionItemRemove,
      { [styles.hideRemove]: shouldHideRemove }
    )
  }
  render () {
    return this.shouldShow() && (
      <div className={this.collectionItemRemoveClass}>
        {this.props.isAccessibilityRegsiterAddress
          ? (
            <Popup
              trigger={<PTVIcon name='icon-cross' />}
              open={this.props.isOpen}
              onClose={this.handleOnClose}
              onOpen={this.handleOnOpen}
              position='top center'
              maxWidth='mW300'
            >
              <div>
                {this.props.intl.formatMessage(messages.confirmDeleteMessage)}
                <div className={styles.buttonGroup}>
                  <Button small onClick={this.handleOnAccessibilityRegisterAddressRemove}>
                    {this.props.intl.formatMessage(messages.buttonOk)}</Button>
                  <Button small secondary onClick={this.handleOnClose}>
                    {this.props.intl.formatMessage(messages.buttonCancel)}
                  </Button>
                </div>
              </div>
            </Popup>
          )
          : (
            <PTVIcon
              name='icon-cross'
              onClick={this.handleOnRemove}
            />
          )
        }
      </div>
    )
  }
}
AccessibilityRemoveButton.propTypes = {
  index: PropTypes.number,
  fields: fieldArrayPropTypes.fields,
  innerShouldHideControls: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  updateUI: PropTypes.func,
  changeFormValue: PropTypes.func,
  intl: intlShape,
  isOpen: PropTypes.bool,
  isMainEntrance: PropTypes.bool,
  visitingAddresses: PropTypes.oneOfType([
    PropTypes.array,
    PropTypes.object
  ]),
  isAccessibilityRegsiterAddress : PropTypes.bool
}

const getServiceLocationVisitingAddresses = createSelector(
  state => formValueSelector(formTypesEnum.SERVICELOCATIONFORM)(state, 'visitingAddresses'),
  visitingAddresses => visitingAddresses || List()
)
const getAccesibilityRegister = createSelector(
  [
    getAccessibilityRegisters,
    (_, { index }) => index
  ],
  (accessibilityRegisters, index) => {
    return accessibilityRegisters.get(index) || Map()
  }
)
const getIsMainEntrance = createSelector(
  getAccesibilityRegister,
  accessibilityRegister => !!accessibilityRegister.get('isMainEntrance')
)
const getIsAccessibilityRegisterAddresss = createSelector(
  [
    getServiceLocationVisitingAddresses,
    (_, { index }) => index
  ],
  (visitingAddresses, index) => !!visitingAddresses.getIn([index, 'accessibilityRegister'])
)

export default compose(
  injectIntl,
  withFormStates,
  connect((state, ownProps) => ({
    isMainEntrance: getIsMainEntrance(state, ownProps),
    visitingAddresses: getServiceLocationVisitingAddresses(state, ownProps),
    isAccessibilityRegsiterAddress: getIsAccessibilityRegisterAddresss(state, ownProps)
  }), {
    changeFormValue: change
  }),
  withState({
    key: 'confirmDialog',
    initialState: {
      isOpen: false
    }
  })
)(AccessibilityRemoveButton)

