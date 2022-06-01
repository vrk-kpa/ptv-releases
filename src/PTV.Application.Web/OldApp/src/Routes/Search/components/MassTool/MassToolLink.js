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
import { PTVIcon } from 'Components'
import { ToggleButton } from 'appComponents/Buttons'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withState from 'util/withState'
import MassToolSelectionCounter from './MassToolSelectionCounter'
import { formTypesEnum } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'

const messages = defineMessages({
  massTool: {
    id: 'FrontPageSearchFrom.MassTool.Link',
    defaultMessage: 'Massatyökalut'
  }
})

class MassToolButton extends Component {
  toggleMassTools = () => {
    const { isVisible, updateUI, disabled, customToggle } = this.props
    updateUI('isVisible', disabled ? isVisible : !isVisible)
    customToggle && customToggle(disabled ? isVisible : !isVisible)
  }

  render () {
    const { isVisible, intl: { formatMessage }, disabled } = this.props
    const toggleCartClass = cx(
      styles.toggleCart,
      {
        [styles.expanded]: isVisible
      }
    )
    const isCollapsed = !isVisible
    return (
      <div className={toggleCartClass} onClick={this.toggleMassTools}>
        {isCollapsed && <PTVIcon name='icon-cart' width={32} height={32} componentClass={styles.cartIcon} />}
        <div>
          <ToggleButton
            onClick={() => {}}
            isCollapsed={isCollapsed}
            showIcon
            className={styles.massToolButton}
            size={28}
            disabled={disabled}
          >
            {formatMessage(messages.massTool)}
          </ToggleButton>
          <MassToolSelectionCounter
            className={styles.linkCounter}
          />
        </div>
      </div>
    )
  }
}

MassToolButton.propTypes = {
  isVisible: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  intl: intlShape.isRequired,
  disabled: PropTypes.bool
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: formTypesEnum.MASSTOOLFORM,
    initialState: {
      isVisible: false
    }
  })
)(MassToolButton)
