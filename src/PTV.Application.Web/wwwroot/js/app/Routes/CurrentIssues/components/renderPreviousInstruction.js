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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'react-intl'
import { PTVTextEditorNotEmpty, PTVLabel } from 'Components'
import moment from 'moment'
import { SecurityRead } from 'appComponents/Security'

const messages = defineMessages({
  previousIssuesTexEditorLabel: {
    id: 'CurrentIssues.Header.PreviousIssues.Label',
    defaultMessage: 'Tiedotteet - previous'
  },
  previousIssueModified: {
    id: 'CurrentIssues.Header.PreviousIssues.Modified.Label',
    defaultMessage: 'Muokattu: {modified}'
  },
  previousIssueModifiedBy: {
    id: 'CurrentIssues.Header.PreviousIssues.ModifiedBy.Label',
    defaultMessage: 'Muokkaaja: {modifiedBy}'
  }
})

const renderPreviousInstruction = ({
  input,
  intl: { formatMessage } }) => {
  return (
    <SecurityRead domain='previousIssues'>
      <div className='col-xs-6'>
        <PTVTextEditorNotEmpty
          label={formatMessage(messages.previousIssuesTexEditorLabel)}
          name={'PreviousEnvironemntIssues'}
          value={input.value.get('environmentInstructions') || ''}
          readOnly
        />
        <PTVLabel>
          {formatMessage(messages.previousIssueModified,
            { modified: moment(input.value.get('modified')).format('DD.MM.YYYY HH:mm') }) + ' ' +
          formatMessage(messages.previousIssueModifiedBy, { modifiedBy: input.value.get('modifiedBy') }) }
        </PTVLabel>
      </div>
    </SecurityRead>
  )
}

renderPreviousInstruction.propTypes = {
  input: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired
}

export default compose(
  connect((state, ownProps) => ({
  })
  ),
  injectIntl
)(renderPreviousInstruction)
