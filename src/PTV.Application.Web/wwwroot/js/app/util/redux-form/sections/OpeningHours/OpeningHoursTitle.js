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
import {
  getOpeningHoursNormalTitle,
  getOpeningHoursSpecialTitle,
  getOpeningHoursExceptionalTitle
} from './selectors'
import NoDataLabel from 'appComponents/NoDataLabel'
import { compose } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'

const Title = ({ title, className }) => {
  const titleClass = cx(
    className,
    styles.wrap
  )
  return title && title !== ''
    ? <div className={titleClass}>{title}</div>
    : <NoDataLabel />
}

Title.propTypes = {
  title: PropTypes.string,
  className: PropTypes.string
}

export const OpeningHoursNormalTitle = compose(
  asLocalizable,
  connect(
    (state, ownProps) => ({
      title: getOpeningHoursNormalTitle(state, ownProps)
    })
  )
)(Title)

export const OpeningHoursSpecialTitle = compose(
  asLocalizable,
  connect(
    (state, ownProps) => ({
      title: getOpeningHoursSpecialTitle(state, ownProps)
    })
  )
)(Title)

export const OpeningHoursExceptionalTitle = compose(
  asLocalizable,
  connect(
    (state, ownProps) => ({
      title: getOpeningHoursExceptionalTitle(state, ownProps)
    })
  )
)(Title)
