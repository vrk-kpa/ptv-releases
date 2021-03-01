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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import {
  getIsExpireWarningVisible,
  getIsRemoveConnectionWarningVisible,
  getIsOrganizationLanguageWarningVisible,
  getIsTimedPublishInfoVisible,
  getIsTranslationDeliveredInfoVisible,
  getIsTranslationOrderedInfoVisible,
  getIsNotUpdatedWarningVisible
} from './selectors'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import {
  ExpireWarningDialog,
  NotUpdatedWarningDialog,
  RemoveConnectionWarningDialog,
  OrganizationLanguageWarningDialog,
  TimedPublishInfoDialog,
  PublishUnavailableDialog,
  TranslationArrivedInfoDialog,
  TranslationOrderedInfoDialog
} from './components'

const withEntityNotification = WrappedComponent => {
  const EntityNotification = props => {
    const {
      isExpireWarningVisible,
      isNotUpdatedWarningVisible,
      isRemoveConnectionWarningVisible,
      isOrganizationLanguageWarningVisible,
      isTimedPublishInfoVisible,
      isInReview,
      isTranslationDeliveredInfoVisible,
      isTranslationOrderedInfoVisible,
      ...rest
    } = props

    return (
      <div>
        {isExpireWarningVisible && <ExpireWarningDialog />}
        {isNotUpdatedWarningVisible && <NotUpdatedWarningDialog />}
        {isRemoveConnectionWarningVisible && <RemoveConnectionWarningDialog />}
        {isOrganizationLanguageWarningVisible && <OrganizationLanguageWarningDialog />}
        {isTimedPublishInfoVisible && <TimedPublishInfoDialog />}
        {isInReview && <PublishUnavailableDialog />}
        {isTranslationDeliveredInfoVisible && <TranslationArrivedInfoDialog />}
        {isTranslationOrderedInfoVisible && <TranslationOrderedInfoDialog />}
        <WrappedComponent isInReview={isInReview} {...rest} />
      </div>
    )
  }

  EntityNotification.propTypes = {
    isExpireWarningVisible: PropTypes.bool,
    isNotUpdatedWarningVisible: PropTypes.bool,
    isRemoveConnectionWarningVisible: PropTypes.bool,
    isOrganizationLanguageWarningVisible: PropTypes.bool,
    isTimedPublishInfoVisible: PropTypes.bool,
    isTranslationDeliveredInfoVisible: PropTypes.bool,
    isTranslationOrderedInfoVisible: PropTypes.bool,
    isInReview: PropTypes.bool
  }

  return compose(
    injectFormName,
    connect((state, { formName }) => ({
      isExpireWarningVisible: getIsExpireWarningVisible(state),
      isNotUpdatedWarningVisible: getIsNotUpdatedWarningVisible(state),
      isRemoveConnectionWarningVisible: getIsRemoveConnectionWarningVisible(state),
      isOrganizationLanguageWarningVisible: getIsOrganizationLanguageWarningVisible(state),
      isTimedPublishInfoVisible: getIsTimedPublishInfoVisible(state),
      isInReview: getShowReviewBar(state),
      isTranslationDeliveredInfoVisible: getIsTranslationDeliveredInfoVisible(state),
      isTranslationOrderedInfoVisible: getIsTranslationOrderedInfoVisible(state)
    }))
  )(EntityNotification)
}

export default withEntityNotification
