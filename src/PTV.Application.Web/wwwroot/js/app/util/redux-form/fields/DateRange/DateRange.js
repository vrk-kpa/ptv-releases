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
import { Fields } from 'redux-form/immutable'
import { compose } from 'redux'
import { RenderDateRangePicker } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'react-intl'

const messages = defineMessages({
  fromLabel : {
    id: 'Containers.Channels.Common.OpeningHours.StartDate.Title',
    defaultMessage: 'Alkaa'
  },
  toLabel : {
    id: 'Containers.Channels.Common.OpeningHours.EndDate.Title',
    defaultMessage: 'Päättyy'
  }
})

const DateRange = ({
  names,
  intl: { formatMessage },
  ...props
}) => {
  return (
    <Fields
      names={[
        'dateFrom',
        'dateTo'
      ]}
      component={(RenderDateRangePicker)}
      fromLabel={formatMessage(messages.fromLabel)}
      toLabel={formatMessage(messages.toLabel)}
      {...props}
    />
  )
}
DateRange.propTypes = {
  names: PropTypes.array,
  intl: intlShape
}

export default compose(
  injectIntl
)(DateRange)
