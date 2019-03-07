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
import ReviewBarHead from './ReviewBarHead'
import ReviewBarContent from './ReviewBarContent'
import { compose } from 'redux'
import styles from './styles.scss'
import { connect } from 'react-redux'
import { getReviewCurrentStep } from 'selectors/selections'
import { getShowReviewBar } from './selectors'

const ReviewBar = props => props.isReviewVisible && (
  <div className={styles.reviewBarWrap}>
    <div className={styles.reviewBar}>
      <div className='container'>
        <ReviewBarHead />
        <ReviewBarContent currentStep={props.currentStep} forwardedRef={props.forwardedRef} />
      </div>
    </div>
  </div>
) || null

ReviewBar.propTypes = {
  currentStep: PropTypes.number,
  isReviewVisible: PropTypes.bool,
  forwardedRef: PropTypes.oneOfType([PropTypes.func, PropTypes.object])
}

export default compose(
  connect(
    state => ({
      currentStep: getReviewCurrentStep(state),
      isReviewVisible: getShowReviewBar(state)
    })
  )
)(ReviewBar)
