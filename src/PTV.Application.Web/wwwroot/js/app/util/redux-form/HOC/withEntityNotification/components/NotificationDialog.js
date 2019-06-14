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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import styles from '../styles.scss'
import PTVIcon from 'Components/PTVIcon'
import { Notification } from 'sema-ui-components'
import withState from 'util/withState'

// To be replaced with Notification component from SEMA UI package
// once the new design is approved.
export const NotificationDialogOld = compose(
  withState({
    initialState: {
      isWarningOpen: true
    }
  }),
)(({
  text,
  onCrossClick,
  children,
  isWarningOpen,
  updateUI,
  customAction
}) => {
  const handleCrossClick = () => {
    updateUI('isWarningOpen', false)
    customAction && typeof customAction === 'function' && customAction()
  }
  return (
    isWarningOpen && <div className={styles.messageWrap}>
      <span className={styles.messageIcon}>!</span>
      <p>{text}</p>
      {children}
      <PTVIcon
        name='icon-cross'
        onClick={handleCrossClick}
        componentClass={styles.close}
        className={styles.closeIcon}
      />
    </div>
  )
})

NotificationDialogOld.propTypes = {
  onCrossClick: PropTypes.func,
  text: PropTypes.string,
  children: PropTypes.any,
  isWarningOpen: PropTypes.bool,
  updateUI: PropTypes.func,
  customAction: PropTypes.func
}

const NotificationDialog = compose(
  withState({
    initialState: {
      isNotificationOpen: true
    }
  })
)(({ isNotificationOpen, updateUI, customAction, ...rest }) => {
  const handleCrossClick = () => {
    updateUI('isNotificationOpen', false)
    customAction && typeof customAction === 'function' && customAction()
  }
  return isNotificationOpen && <Notification {...rest} onRequestClose={handleCrossClick} />
})

export default NotificationDialog
