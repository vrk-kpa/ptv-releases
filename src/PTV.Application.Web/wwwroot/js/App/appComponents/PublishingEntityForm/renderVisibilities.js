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
import React, { PropTypes } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { RadioGroup, RadioButton } from 'sema-ui-components'
import { injectIntl, defineMessages } from 'react-intl'
import { getPublishingStatusDraft, getPublishingStatusPublished, getPublishingStatusModified } from 'Containers/Common/Selectors'

const messages = defineMessages({
  visibleTitle: {
    id: 'Components.PublishingEntityForm.LanguageVisibilityCell.Visible.Title',
    defaultMessage: 'Näkyy loppukäyttäjille'
  },
  notVisibleTitle: {
    id: 'Components.PublishingEntityForm.LanguageVisibilityCell.NotVisible.Title',
    defaultMessage: 'Näkyy vain PTV:ssä'
  }
})

const renderVisibilities = ({
  input,
  publishingStatusDraft,
  publishingStatusPublished,
  publishingStatusModified,
  intl: { formatMessage }
}) => {
  const handleOnChange = value => {
    input.onChange(value)
  }

  return (
    <div>
      <RadioGroup inline name='testi' value={input.value === publishingStatusModified ? publishingStatusPublished : input.value} onChange={handleOnChange}>
        <RadioButton small label={formatMessage(messages.notVisibleTitle)} value={publishingStatusDraft} />
        <RadioButton small label={formatMessage(messages.visibleTitle)} value={publishingStatusPublished} />
      </RadioGroup>
    </div>
  )
}

renderVisibilities.propTypes = {
  input: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  publishingStatusDraft: PropTypes.string.isRequired,
  publishingStatusPublished: PropTypes.string.isRequired
}

export default compose(
  connect((state, ownProps) => ({
    publishingStatusDraft: getPublishingStatusDraft(state).get('id'),
    publishingStatusModified: getPublishingStatusModified(state).get('id'),
    publishingStatusPublished: getPublishingStatusPublished(state).get('id')
  })
  ),
  injectIntl
)(renderVisibilities)
