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
import { injectIntl, intlShape } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import {
  Name,
  UrlChecker
} from 'util/redux-form/fields'

// will be renamed as web page and used also for organizations instead of attachment
const Attachment = ({
  intl: { formatMessage },
  isCompareMode,
  nameTooltip,
  urlTooltip,
  splitView,
  nameMessages = {},
  urlCheckerMessages = {}
}) => {
  const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='row'>
        <div className={basicCompareModeClass}>
          <Name
            isCompareMode={isCompareMode}
            name={'name'}
            isLocalized={false}
            tooltip={nameTooltip && formatMessage(nameTooltip) || null}
            skipValidation
            noTranslationLock
            {...nameMessages}
          />
        </div>
      </div>
      <div className='form-row'>
        <UrlChecker
          isCompareMode={isCompareMode}
          name={'urlAddress'}
          tooltip={urlTooltip && formatMessage(urlTooltip) || null}
          isLocalized={false}
          splitView={splitView}
          {...urlCheckerMessages}
        />
      </div>
    </div>
  )
}

Attachment.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool,
  nameTooltip: PropTypes.object,
  urlTooltip: PropTypes.object,
  urlCheckerMessages: PropTypes.object,
  nameMessages: PropTypes.object,
  splitView: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates
)(Attachment)
