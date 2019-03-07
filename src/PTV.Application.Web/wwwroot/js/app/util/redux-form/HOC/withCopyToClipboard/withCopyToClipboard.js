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
import { connect } from 'react-redux'
import { Button } from 'sema-ui-components'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { getShowInfosAction } from 'reducers/notifications'
import { copyToClip } from 'util/helpers'
import shortId from 'shortid'

const messages = defineMessages({
  copyLinkTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityHeader.VersionLabel.CopyLinkTitle',
    defaultMessage: 'Kopioi'
  },
  copyFeedback: {
    id: 'CopyFeedback.Title',
    defaultMessage: 'The link has been copied to clipboard.'
  }
})

const withCopyToClipboard = WrappedComponent => {
  const InnerComponent = ({
    onCopyToClipboardClick,
    intl: { formatMessage },
    value,
    dispatch,
    className,
    type,
    disabled,
    title,
    ...rest
  }) => {
    const handleCopy = value => {
      if (!disabled) {
        copyToClip(value)
        dispatch(getShowInfosAction(shortId.generate(), true)([{ code: messages.copyFeedback }]))
      }
    }
    return <div className={className}>
      <WrappedComponent value={value} {...rest} />
      {value &&
        <Button
          link
          onClick={() => handleCopy(value)}
          type={type}
          disabled={disabled}
        >
          {title || formatMessage(messages.copyLinkTitle)}
        </Button>
      }
    </div>
  }
  InnerComponent.propTypes = {
    onCopyToClipboardClick: PropTypes.func,
    value: PropTypes.string,
    dispatch: PropTypes.func,
    intl: intlShape,
    className: PropTypes.string,
    type: PropTypes.string,
    disabled: PropTypes.bool,
    title: PropTypes.string
  }
  return compose(
    injectIntl,
    connect(null)
  )(InnerComponent)
}

export default withCopyToClipboard
