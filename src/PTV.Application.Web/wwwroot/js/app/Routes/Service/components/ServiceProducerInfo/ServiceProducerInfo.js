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

import {
  ProducerCollection,
  ProducerCollectionDisplay
} from '../ProducerCollection'
import { injectIntl } from 'react-intl'
import { Sticky, StickyContainer } from 'react-sticky'
import { withFormStates } from 'util/redux-form/HOC'

const ServiceProducerInfo = (
  {
    isReadOnly,
    isCompareMode,
    intl: { formatMessage }
  }
) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <StickyContainer>
      <div className='form-row'>
        {!isReadOnly && <div className='row'>
          <div className={basicCompareModeClass}>
            <ProducerCollection />
          </div>
          <div className={basicCompareModeClass}>
            <Sticky>
              <ProducerCollectionDisplay preview />
            </Sticky>
          </div>
        </div> ||
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ProducerCollectionDisplay preview />
          </div>
        </div>}
      </div>
    </StickyContainer>
  )
}

ServiceProducerInfo.propTypes = {
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  intl: PropTypes.object.isRequired
}

export default compose(
  injectIntl,
  withFormStates
)(ServiceProducerInfo)
