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
import { getReviewCurrentStep } from 'selectors/selections'
import { getTotalCount } from './selectors'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import TooltipLabel from 'appComponents/TooltipLabel'
import { PTVIcon } from 'Components'
import { Button } from 'sema-ui-components'
import styles from './styles.scss'
import { formValues } from 'redux-form/immutable'
import injectFromName from 'util/redux-form/HOC/injectFormName'
import { mergeInUIState } from 'reducers/ui'
import CancelReviewDialog from './CancelReviewDialog'
import cx from 'classnames'

const messages = defineMessages({
  approvalHeadlineTitle: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Head.ApprovalHeadline.Title',
    defaultMessage: 'Massajulkaisu - hyväksymisvaihe'
  },
  approvalHeadlineTooltip: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Head.ApprovalHeadline.Tooltip',
    defaultMessage: 'Approval headline tooltip placeholder'
  },
  approvalHeadlineCancel: {
    id: 'Util.ReduxForm.HOC.WithMassToolForm.Head.ApprovalHeadline.Cancel',
    defaultMessage: 'Keskeytä'
  }
})

const ReviewBarHead = ({
  count,
  currentStep,
  approved,
  mergeInUIState,
  intl: { formatMessage }
}) => {
  const handleApprovalCancel = () => {
    mergeInUIState({
      key: 'cancelReviewDialog',
      value: {
        isOpen: true
      }
    })
  }
  const currentStepLabel = currentStep + 1
  const labelText = `${formatMessage(messages.approvalHeadlineTitle)} (${currentStepLabel}/${count})`
  const reviewBarHeadClass = cx(
    styles.reviewBarHead,
    {
      [styles.approved]: approved
    }
  )
  return (
    <div className={reviewBarHeadClass}>
      <h5 className={styles.headTitleWrap}>
        <TooltipLabel
          labelProps={{ labelText }}
          tooltipProps={{ tooltip: formatMessage(messages.approvalHeadlineTooltip) }}
        />
      </h5>
      <Button className={styles.cancelButton} link onClick={handleApprovalCancel}>
        <span className={styles.cancelText}>
          {formatMessage(messages.approvalHeadlineCancel)}
        </span>
        <span className={styles.cancelIcon}>
          <PTVIcon name='icon-cross' width={26} height={26} />
        </span>
      </Button>
      <CancelReviewDialog />
    </div>
  )
}

ReviewBarHead.propTypes = {
  currentStep: PropTypes.number,
  approved: PropTypes.bool,
  count: PropTypes.number,
  mergeInUIState: PropTypes.func,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFromName,
  connect((state, ownProps) => ({
    currentStep: getReviewCurrentStep(state),
    count: getTotalCount(state, ownProps)
  }), {
    mergeInUIState
  }),
  formValues(({ currentStep }) => ({ approved: `review[${currentStep}].approved` }))
)(ReviewBarHead)
