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
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  getIsExpireWarningVisible,
  getIsRemoveConnectionWarningVisible,
  getIsOrganizationLanguageWarningVisible,
  getIsTimedPublishInfoVisible
} from './selectors'
import {
  ExpireWarningDialog,
  RemoveConnectionWarningDialog,
  OrganizationLanguageWarningDialog,
  TimedPublishInfoDialog
} from './components'

const withEntityNotification = WrappedComponent => {
  const EntityNotification = props => {
    const {
      isExpireWarningVisible,
      isRemoveConnectionWarningVisible,
      isOrganizationLanguageWarningVisible,
      isTimedPublishInfoVisible,
      ...rest
    } = props

    return (
      <div>
        {isExpireWarningVisible && <ExpireWarningDialog />}
        {isRemoveConnectionWarningVisible && <RemoveConnectionWarningDialog />}
        {isOrganizationLanguageWarningVisible && <OrganizationLanguageWarningDialog />}
        {isTimedPublishInfoVisible && <TimedPublishInfoDialog />}
        <WrappedComponent {...rest} />
      </div>
    )
  }

  EntityNotification.propTypes = {
    expireOn: PropTypes.number,
    isExpireWarningVisible: PropTypes.bool,
    isRemoveConnectionWarningVisible: PropTypes.bool,
    notificationIds: PropTypes.array,
    isOrganizationLanguageWarningVisible: PropTypes.bool,
    isTimedPublishInfoVisible: PropTypes.bool,
    publishOn: PropTypes.number,
    isWarningOpen: PropTypes.bool,
    updateUI: PropTypes.func,
    dispatch: PropTypes.func
  }

  return compose(
    injectFormName,
    connect((state, { formName }) => ({
      isExpireWarningVisible: getIsExpireWarningVisible(state),
      isRemoveConnectionWarningVisible: getIsRemoveConnectionWarningVisible(state),
      isOrganizationLanguageWarningVisible: getIsOrganizationLanguageWarningVisible(state),
      isTimedPublishInfoVisible: getIsTimedPublishInfoVisible(state)
    }))
  )(EntityNotification)
}

export default withEntityNotification
