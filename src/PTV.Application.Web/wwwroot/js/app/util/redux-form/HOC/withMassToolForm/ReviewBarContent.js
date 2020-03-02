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
import { formValues } from 'redux-form/immutable'
import { Checkbox } from 'util/redux-form/fields'
import asSection from 'util/redux-form/HOC/asSection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Button } from 'sema-ui-components'
import Arrow from 'appComponents/Arrow'
import SkipReviewedContentDialog from './SkipReviewedContentDialog'
import {
  getIsEntityReviewed,
  getCurrentItem,
  getTotalCount,
  getMappedStepIndex,
  getShowReminderDialog
} from './selectors'
import {
  getIsAnyInTranslation
} from 'selectors/entities/entities'
import { getReviewCurrentStep } from 'selectors/selections'
import { getReviewListCount } from 'appComponents/MassPublish/selectors'
import withState from 'util/withState'
import withPath from 'util/redux-form/HOC/withPath'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { moveToStep } from './actions'
import cx from 'classnames'
import { mergeInUIState } from 'reducers/ui'

const messages = defineMessages({
  approveCheckboxLabel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Form.ApproveCheckboxLabel.Title',
    defaultMessage: 'Hyväksy julkaistavaksi'
  },
  previousButton: {
    id: 'Components.Buttons.PreviousButton',
    defaultMessage: 'Edellinen'
  },
  nextButton: {
    id: 'Components.Buttons.NextButton',
    defaultMessage: 'Seuraava'
  },
  massPublishButton: {
    id: 'MassTool.Review.PublishButton.Title',
    defaultMessage: 'Massajulkaise hyväksytyt'
  },
  pageIndexTitle: {
    id: 'MassTool.Review.PageIndex.Title',
    defaultMessage: 'Sivu'
  }
})

const ReviewBar = ({
  approvedCount,
  languageAvailability,
  language,
  languageId,
  showReminder,
  statusId,
  hasError,
  currentStep,
  totalSteps,
  intl: { formatMessage },
  isEntityReviewed,
  setReviewCurrentStep,
  updateUI,
  mergeInUIState,
  moveToStep,
  isItemApproved,
  isEditable,
  id
}) => {
  const handleOnNext = () => {
    if (showReminder) {
      mergeInUIState({
        key: 'reminderDialog',
        value: { isOpen: true }
      })
    } else if (!isItemApproved && !(!isEntityReviewed || hasError || !isEditable)) {
      mergeInUIState({
        key: 'skipReviewedContentDialog',
        value: { isOpen: true }
      })
    } else {
      window.scrollTo(0, 0)
      moveToStep(currentStep + 1)
    }
  }

  const handleOnPrevious = () => {
    if (showReminder) {
      mergeInUIState({
        key: 'reminderDialog',
        value: { isOpen: true }
      })
    } else {
      window.scrollTo(0, 0)
      moveToStep(currentStep - 1)
    }
  }

  const handleOnPublish = () => {
    if (showReminder) {
      mergeInUIState({
        key: 'reminderDialog',
        value: { isOpen: true }
      })
    } else {
      updateUI({ isOpen: true })
    }
  }

  const isFirstStep = currentStep === 0
  const isLastStep = currentStep === totalSteps - 1

  const prevNavButtonClass = cx(
    styles.navigationButton,
    styles.previousButton,
    {
      [styles.prevDisabled]: isFirstStep
    }
  )
  const nextNavButtonClass = cx(
    styles.navigationButton,
    {
      [styles.nextButton]: !isItemApproved || isLastStep,
      [styles.nextDisabled]: isLastStep,
      [styles.nextPrimary]: isItemApproved && !isLastStep
    }
  )
  const approvalCheckboxClass = cx(
    styles.approvalCheckbox,
    {
      [styles.approved]: isItemApproved,
      [styles.reviewed]: isEntityReviewed,
      [styles.hasError]: hasError
    }
  )
  return (
    <div className={styles.reviewBarContent}>
      <div className='row align-items-center'>
        <div className='col-lg-8'>
          <div className={approvalCheckboxClass}>
            <Checkbox
              key={currentStep}
              name='approved'
              label={formatMessage(messages.approveCheckboxLabel)}
              disabled={(!isEntityReviewed || hasError || !isEditable) && !isItemApproved}
            />
          </div>
        </div>
        <div className='col-lg-8'>
          <div className={styles.reviewBarButtonGroup}>
            {totalSteps > 1 && (
              <Button
                onClick={handleOnPrevious}
                small
                secondary
                disabled={isFirstStep}
                className={prevNavButtonClass}
              >
                <Arrow direction='west' gap='g5' gapSide='right' secondary />
                {formatMessage(messages.previousButton)}
              </Button>
            )}
            <div className={styles.pageIndex}>
              {`${formatMessage(messages.pageIndexTitle)} ${currentStep + 1}/${totalSteps}`}
            </div>
            {totalSteps > 1 && (
              <Button
                onClick={handleOnNext}
                small
                secondary={!isItemApproved || isLastStep}
                disabled={isLastStep}
                className={nextNavButtonClass}
              >
                {formatMessage(messages.nextButton)}
                <Arrow direction='east' gap='g5' gapSide='left' secondary />
              </Button>
            )}
          </div>
        </div>
        <div className='col-lg-8 d-flex'>
          <Button
            children={formatMessage(messages.massPublishButton)}
            onClick={handleOnPublish}
            small
            secondary={!isLastStep}
            disabled={approvedCount === 0}
            className={styles.massPublishButton}
          />
        </div>
      </div>
      <SkipReviewedContentDialog />
    </div>
  )
}

ReviewBar.propTypes = {
  approvedCount: PropTypes.number,
  languageAvailability: PropTypes.string,
  language: PropTypes.string,
  languageId: PropTypes.string,
  statusId: PropTypes.string,
  hasError: PropTypes.bool,
  showReminder: PropTypes.bool,
  currentStep: PropTypes.number,
  totalSteps: PropTypes.number,
  isEntityReviewed: PropTypes.bool,
  setReviewCurrentStep: PropTypes.func,
  updateUI: PropTypes.func,
  mergeInUIState: PropTypes.func,
  moveToStep: PropTypes.func.isRequired,
  isItemApproved: PropTypes.bool,
  isEditable: PropTypes.bool,
  id: PropTypes.string.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect(
    (state, { formName }) => ({
      name: `review[${getMappedStepIndex(state, { formName })}]`,
      currentStep: getReviewCurrentStep(state)
    })
  ),
  asSection(),
  formValues({
    type: 'meta.type',
    id: 'id',
    language: 'language',
    languageId: 'languageId',
    meta: 'meta',
    isItemApproved: 'approved'
  }),
  withPath,
  connect((state, ownProps) => {
    const currentItem = getCurrentItem(state, { id: ownProps.id, type: ownProps.type, language: ownProps.language })
    return {
      languageId: ownProps.languageId,
      statusId: currentItem.get('statusId'),
      hasError: currentItem.get('hasError') || getIsAnyInTranslation(state),
      isEntityReviewed: getIsEntityReviewed(state),
      isEditable: currentItem.get('isEditable'),
      totalSteps: getTotalCount(state, ownProps),
      showReminder: getShowReminderDialog(state),
      approvedCount: getReviewListCount(state, { ...ownProps, approved: true })
    }
  }, {
    moveToStep,
    mergeInUIState
  }),
  withState({
    redux: true,
    key: 'MassToolDialog',
    initialState: {
      isOpen: false
    }
  }),
)(ReviewBar)
