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
import withFormStates from 'util/redux-form/HOC/withFormStates'
import {
  DialCode,
  PhoneNumber
} from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

const messages = defineMessages({
  title: {
    id: 'Containers.Channels.Common.FaxNumber.Title',
    defaultMessage: 'Faksinumero'
  },
  tooltip: {
    id: 'Containers.Channels.Common.FaxNumber.Tooltip',
    defaultMessage: 'To be defined...'
  }
})

const FaxNumber = ({
  isCompareMode,
  splitView,
  isReadOnly,
  intl: { formatMessage }
}) => {
  return (
    <div>
      {isCompareMode || splitView
      ? <div>
        <div className='collection-form-row'>
          <DialCode
            isCompareMode={isCompareMode}
            isReadOnly={isReadOnly}
          />
        </div>
        <div className='collection-form-row'>
          <PhoneNumber
            isCompareMode={isCompareMode}
            label={formatMessage(messages.title)}
            tooltip={formatMessage(messages.tooltip)}
            isReadOnly={isReadOnly}
          />
        </div>
      </div>
      : <div className='collection-form-row'>
        <div className='row'>
          <div className='col-lg-6 mb-4 mb-lg-0'>
            <DialCode
              isCompareMode={isCompareMode}
              isReadOnly={isReadOnly}
            />
          </div>
          <div className='col-lg-6'>
            <PhoneNumber
              isCompareMode={isCompareMode}
              label={formatMessage(messages.title)}
              tooltip={formatMessage(messages.tooltip)}
              isReadOnly={isReadOnly}
            />
          </div>
        </div>
      </div>
      }

    </div>
  )
}
FaxNumber.propTypes = {
  isCompareMode: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  splitView: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  withFormStates
)(FaxNumber)
