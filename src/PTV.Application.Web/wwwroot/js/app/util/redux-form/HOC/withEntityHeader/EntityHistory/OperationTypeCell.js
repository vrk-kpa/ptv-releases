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
import { FormattedMessage, defineMessages } from 'util/react-intl'

const messages = defineMessages({
  published: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Published',
    defaultMessage: 'Published'
  },
  archived: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Archived',
    defaultMessage: 'Archived'
  },
  draft: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.Draft',
    defaultMessage: 'Draft'
  },
  delete: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.DeleteAction',
    defaultMessage: 'Archive'
  },
  restore: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.RestoreAction',
    defaultMessage: 'Restore'
  },
  withdraw: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.WithdrawAction',
    defaultMessage: 'Withdraw'
  },
  ordered: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationOrderedAction',
    defaultMessage: 'Translation ordered'
  },
  received: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.TranslationReceivedAction',
    defaultMessage: 'Translation received'
  },
  copy: {
    id : 'Util.ReduxForm.HOC.WithEntityHeader.EntityHistory.CopyAction',
    defaultMessage: 'Copy'
  }
})

const OperationTypeCell = ({
  publishingStatus,
  versionMajor,
  versionMinor,
  historyAction
}) => (
  <div>
    {
      {
        'Delete':<FormattedMessage {...messages.delete} />,
        'Restore':<FormattedMessage {...messages.restore} />,
        'Withdraw':<FormattedMessage {...messages.withdraw} />,
        'TranslationOrdered': <FormattedMessage {...messages.ordered} />,
        'TranslationReceived': <FormattedMessage {...messages.received} />,
        'Save':'',
        'Publish':'',
        'Copy': <FormattedMessage {...messages.copy} />,
      }[historyAction] || null
    }
    {' '}
    {{
      'Published': <FormattedMessage {...messages.published} />,
      'Archived': <FormattedMessage {...messages.archived} />,
      'Draft': <FormattedMessage {...messages.draft} />
    }[publishingStatus] || null}
    {' '}
    {versionMajor}.{versionMinor}
  </div>
)
OperationTypeCell.propTypes = {
  publishingStatus: PropTypes.string,
  historyAction: PropTypes.string,
  versionMajor: PropTypes.number,
  versionMinor: PropTypes.number
}

export default OperationTypeCell
