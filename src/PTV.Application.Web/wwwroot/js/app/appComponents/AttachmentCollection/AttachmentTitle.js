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
import {
  injectIntl,
  intlShape
} from 'util/react-intl'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import NoDataLabel from 'appComponents/NoDataLabel'
import { getAttachmentTitle } from './selectors'

const AttachmentTitle = ({
  className,
  attachmentTitle,
  index,
  items,
  intl: { formatMessage },
  ...rest
}) => {
  return (
    attachmentTitle && <div className={className} style={{ lineHeight: '30px' }}>
      <span>{attachmentTitle}</span>
    </div> || <NoDataLabel />
  )
}

AttachmentTitle.propTypes = {
  className: PropTypes.string,
  attachmentTitle: PropTypes.string,
  index: PropTypes.number,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  connect((state, ownProps) => ({
    attachmentTitle: getAttachmentTitle(state, ownProps)
  }))
)(AttachmentTitle)
