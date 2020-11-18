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
import {
  ShortDescriptionDisplay,
  OrganizationDisplay,
  SelfProducersDisplay
} from 'util/redux-form/fields'
import asSection from 'util/redux-form/HOC/asSection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getIsAlreadySelfProducedForIndex,
  getIsAnySelectedForIndex,
  getIsOtherWithOrganizationForIndex,
  getIsAnyValueForForm } from './selectors'
import { connect } from 'react-redux'
import styles from './styles.scss'

const ProducerDisplay = ({
  isSelfProduced,
  isAnySelected,
  isOtherWithOrganization,
  index,
  readOnly,
  isFilled,
  ...rest
}) => {
  const renderSelfProducers = () => {
    return isFilled &&
      <div className={styles.selfProducerPreview}>
        <SelfProducersDisplay index={index} {...rest} />
      </div> ||
      null
  }

  const renderOthersProducers = () => {
    return (
      isFilled &&
      <div className={styles.otherProducersPreview}>
        <OrganizationDisplay {...rest} />
        {!isOtherWithOrganization && <ShortDescriptionDisplay
          name={'additionalInformation'}
          {...rest} />}
      </div> ||
      null)
  }

  const renderProducers = () => {
    return isSelfProduced ? renderSelfProducers() : renderOthersProducers()
  }

  return (
    <div>
      <div>
        { /* readOnly && <ProducerTypeDisplay index={index} /> */ }
        {isAnySelected ? renderProducers() : null}
      </div>
    </div>
  )
}

ProducerDisplay.propTypes = {
  isSelfProduced: PropTypes.bool.isRequired,
  isFilled: PropTypes.bool.isRequired,
  readOnly: PropTypes.bool,
  isOtherWithOrganization: PropTypes.bool.isRequired,
  isAnySelected: PropTypes.bool.isRequired,
  index: PropTypes.number
}

export default compose(
  injectFormName,
  connect((state, ownProps) => ({
    isOtherWithOrganization: getIsOtherWithOrganizationForIndex(state, ownProps),
    isSelfProduced: getIsAlreadySelfProducedForIndex(state, ownProps),
    isAnySelected: getIsAnySelectedForIndex(state, ownProps),
    isFilled: getIsAnyValueForForm(state, ownProps)
  })),
  asSection('serviceProducers')
)(ProducerDisplay)
