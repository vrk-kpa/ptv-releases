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
import styles from './styles.scss'
import Popup from 'appComponents/Popup'
import withState from 'util/withState'
import cx from 'classnames'
import { compose } from 'redux'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { fieldArrayPropTypes } from 'redux-form/immutable'

export const messages = defineMessages({
  promptForRemoval: {
    id : 'Util.ReduxForm.Sections.Laws.CrossIcon.PromptTitle',
    defaultMessage: 'Haluatko poistaa elementin? Poistaminen vaikuttaa muihin kieliversioihin.'
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

class PromptRemoveButton extends Component {
  get collectionItemRemoveClass () {
    const shouldHideRemove = this.props.fields.length === 0 ||
                             this.props.innerShouldHideControls
    return cx(
      styles.collectionItemRemove,
      { [styles.hideRemove]: shouldHideRemove }
    )
  }
  handleOnRemove = () => {
    const { fields, index } = this.props
    if (fields.length <= 0) {
      return
    }
    fields.remove(index)
    this.props.updateUI('isOpen', false)
  }
  handleOnOpen = () => this.props.updateUI('isOpen', true)
  handleOnClose = () => this.props.updateUI('isOpen', false)
  render () {
    return !this.props.isReadOnly && (
      <div className={this.collectionItemRemoveClass}>
        <div>
          <Popup
            trigger={<PTVIcon name={'icon-cross'} />}
            open={this.props.isOpen}
            onClose={this.handleOnClose}
            onOpen={this.handleOnOpen}
            position='top center'
            maxWidth='mW300'
          >
            <div>
              {this.props.intl.formatMessage(messages.promptForRemoval)}
              <div className={styles.buttonGroup}>
                <Button small onClick={this.handleOnRemove}>
                  {this.props.intl.formatMessage(messages.buttonOk)}</Button>
                <Button small secondary onClick={this.handleOnClose}>
                  {this.props.intl.formatMessage(messages.buttonCancel)}
                </Button>
              </div>
            </div>
          </Popup>
        </div>
      </div>
    )
  }
}
PromptRemoveButton.propTypes = {
  index: PropTypes.number,
  fields: fieldArrayPropTypes.fields,
  innerShouldHideControls: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  updateUI: PropTypes.func,
  intl: intlShape,
  isOpen: PropTypes.bool
}

export default compose(
  injectIntl,
  withState({
    key: 'confirmDialog',
    initialState: {
      isOpen: false
    }
  })
)(PromptRemoveButton)
