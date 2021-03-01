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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withState from 'util/withState'
import { mergeInUIState } from 'reducers/ui'
import { getTotalCount, getApprovedCount } from './selectors'
import { MassToolCartIcon, MassToolCartLink, MassToolCartDialog } from 'Routes/Search/components/MassTool/MassToolCart'
import { ToggleButton } from 'appComponents/Buttons'
import Paragraph from 'appComponents/Paragraph'
import { PTVIcon } from 'Components'
import { Button } from 'sema-ui-components'
import CancelReviewDialog from './CancelReviewDialog'
import cx from 'classnames'
import styles from './styles.scss'

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
  },
  approvalDescription: {
    id: 'MassTool.Review.Description',
    defaultMessage: 'Vain hyväksytyt sisällöt voidaan massajulkaista. Siirry sisällöstä toiseen sivutusnapilla ja tarkasta jokainen sisältö erikseen ja merkkaa hyväksytyksi. Hyväksymisvalinta aktivoituu vasta kun olet vierittänyt sivun alas asti.' // eslint-disable-line
  },
  cartAcceptedLabel: {
    id: 'MassTool.Review.Cart.AcceptedLabel',
    defaultMessage: 'hyväksyttyä'
  },
  dialogTitle: {
    id: 'MassTool.Review.CartDialog.Title',
    defaultMessage: 'Hyväksytyt kieliversiot: {approvedCount}/{count}'
  },
  dialogDescription: {
    id: 'MassTool.Review.CartDialog.Description',
    defaultMessage: 'Listasta näet, mitkä sisällöt on hyväksytty julkaistavaksi. Voit siirtyä hyväksymään sisältöä klikkaamalla nimien linkkejä.', // eslint-disable-line
    description: 'MassTool.CartDialog.Description'
  }
})

const ReviewBarHead = ({
  count,
  approvedCount,
  approved,
  mergeInUIState,
  isCollapsed,
  updateUI,
  intl: { formatMessage }
}) => {
  const toggleReviewBar = () => {
    updateUI('isCollapsed', !isCollapsed)
  }
  const handleApprovalCancel = () => {
    mergeInUIState({
      key: 'cancelReviewDialog',
      value: {
        isOpen: true
      }
    })
  }
  const reviewBarHeadClass = cx(
    styles.reviewBarHead,
    {
      [styles.collapsed]: isCollapsed
    }
  )
  return (
    <div className={reviewBarHeadClass}>
      <div className='d-flex align-items-center'>
        <ToggleButton
          onClick={toggleReviewBar}
          isCollapsed={isCollapsed}
          showIcon
          showTooltip
          tooltip={formatMessage(messages.approvalHeadlineTooltip)}
          children={formatMessage(messages.approvalHeadlineTitle)}
          className={styles.reviewBarToggle}
          size={26}
        />
        {!isCollapsed && (
          <Button className={styles.cancelButton} link onClick={handleApprovalCancel}>
            <span className={styles.cancelText}>
              {formatMessage(messages.approvalHeadlineCancel)}
            </span>
            <span className={styles.cancelIcon}>
              <PTVIcon name='icon-cross' width={26} height={26} />
            </span>
          </Button>
        )}
      </div>
      {!isCollapsed && (
        <div className='row align-items-center'>
          <div className='col-md-14 col-lg-19'>
            <Paragraph
              children={formatMessage(messages.approvalDescription)}
              className={styles.reviewBarDescription}
            />
          </div>
          <div className='col-md-10 col-lg-5 d-flex'>
            <div className={styles.reviewCart}>
              <MassToolCartIcon />
              <div className='d-flex flex-column'>
                <MassToolCartLink className={styles.reviewBarCartLink} />
                {`${approvedCount}/${count} ${formatMessage(messages.cartAcceptedLabel)}`}
              </div>
            </div>
          </div>
        </div>
      )}
      <MassToolCartDialog
        title={formatMessage(messages.dialogTitle, { approvedCount, count })}
        description={formatMessage(messages.dialogDescription)}
      />
      <CancelReviewDialog />
    </div>
  )
}

ReviewBarHead.propTypes = {
  approvedCount: PropTypes.number,
  approved: PropTypes.bool,
  count: PropTypes.number,
  mergeInUIState: PropTypes.func,
  updateUI: PropTypes.func.isRequired,
  isCollapsed: PropTypes.bool.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    approvedCount: getApprovedCount(state, ownProps),
    count: getTotalCount(state, ownProps)
  }), {
    mergeInUIState
  }),
  withState({
    initialState: {
      isCollapsed: false
    }
  })
)(ReviewBarHead)
