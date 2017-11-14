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
import { injectIntl } from 'react-intl'

// / Actions
import * as generalDescriptionActions from '../Actions'
import * as commonActions from '../../Common/Actions'

// / App Configuration
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// / App Containers
import PageContainer from '../../../../Common/PageContainer'

import Step1 from './Steps/Step1'

// / Styles
import '../../../../Services/Styles/LandingPage.scss'
import '../../../../Common/Styles/StepContainer.scss'

// / Messages
import * as Messages from '../Messages'

// / Selectors
import * as CommonSelectors from '../../../../Common/Selectors'
import * as GeneralDescriptionSelectors from '../Selectors'

const GeneralDescriptionContainer = props => {
  const { step1IsFetching, step1AreDataValid, keyToState } = props

  return (
    <PageContainer {...props} className='card general-description-page'
      confirmDialogs={[
        {
          type: 'delete',
          messages: [Messages.deleteMessages.buttonOk, Messages.deleteMessages.buttonCancel, Messages.deleteMessages.text]
        },
        {
          type: 'cancel',
          messages: [Messages.cancelMessages.buttonOk, Messages.cancelMessages.buttonCancel, Messages.cancelMessages.text]
        },
        {
          type: 'save',
          messages: [Messages.saveDraftMessages.buttonOk, Messages.saveDraftMessages.buttonCancel, Messages.saveDraftMessages.text]
        },
        {
          type: 'withdraw',
          messages: [Messages.withdrawMessages.buttonOk, Messages.withdrawMessages.buttonCancel, Messages.withdrawMessages.text]
        },
        {
          type: 'restore',
          messages: [Messages.restoreMessages.buttonOk, Messages.restoreMessages.buttonCancel, Messages.restoreMessages.text]
        },
        { type: 'goBack', messages: [Messages.goBackMessages.buttonOk, Messages.goBackMessages.buttonCancel, Messages.goBackMessages.text] }
      ]}
      readOnly={props.readOnly}
      keyToState={keyToState}
      isTranslatable
      deleteAction={props.actions.deleteGeneralDescription}
      withdrawAction={props.actions.withdrawGeneralDescription}
      restoreAction={props.actions.restoreGeneralDescription}
      saveAction={props.actions.saveAllChanges}
      removeServerResultAction={props.actions.removeServerResult}
      lockAction={props.actions.lockGeneralDescription}
      unLockAction={props.actions.unLockGeneralDescription}
      isLockedAction={props.actions.isGeneralDescriptionLocked}
      isEditableAction={ props.actions.isGeneralDescriptionEditable }
      basePath='/frontpage'
      getEntityStatusSelector={GeneralDescriptionSelectors.getPublishingStatus}
      getUnificRootIdSelector={GeneralDescriptionSelectors.getUnificRootId}
      statusEndpoint='generalDescription/GetGeneralDescriptionStatus'
      invalidateAllSteps={props.actions.cancelAllChanges}
      entitiesType='generalDescriptions'
      steps={[{
        mainTitle: Messages.messages.mainTitle,
        mainTitleView: Messages.messages.mainTitleView,
        mainText: Messages.messages.mainText,
        mainTextView: Messages.messages.mainTextView,
        subTitle: Messages.messages.subTitle1,
        subTitleView: Messages.messages.subTitle1View,
        saveStepAction: props.actions.saveAllChanges,
        loadAction: props.actions.loadAddGeneralDescriptionStep1,
        isFetching: step1IsFetching,
        areDataValid: step1AreDataValid,
        child: Step1
      }
      ]} />
  )
}

GeneralDescriptionContainer.propTypes = {
  actions: PropTypes.object,
  readOnly: PropTypes.bool,
  step1IsFetching: PropTypes.bool.isRequired,
  step1AreDataValid: PropTypes.bool.isRequired,
  keyToState: PropTypes.string
}

function mapStateToProps (state, ownProps) {
  const keyToState = 'generalDescription'
  return {
    step1IsFetching: CommonSelectors.getStep1isFetching(state, { keyToState }),
    step1AreDataValid: CommonSelectors.getStep1AreDataValid(state, { keyToState }),
    keyToState
  }
}

const actions = [
  generalDescriptionActions,
  commonActions
]

export default compose(
  injectIntl,
  connect(mapStateToProps, mapDispatchToProps(actions))
)(GeneralDescriptionContainer)
