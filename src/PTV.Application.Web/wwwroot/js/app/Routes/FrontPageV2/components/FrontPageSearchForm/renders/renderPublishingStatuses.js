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
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { getPublishingStatuses } from 'Routes/FrontPageV2/selectors'
import { Checkbox } from 'sema-ui-components'
import cx from 'classnames'
import { PTVIcon } from '../../../../../Components'

const publishingStatusMessages = defineMessages({
  title: {
    id: 'FrontPage.PublishingStatus.Title',
    defaultMessage: 'Tila'
  },
  tooltip: {
    id: 'FrontPage.PublishingStatus.Tooltip',
    defaultMessage: 'NO TRANSLATION YET'
  },
  draftLabel: {
    id: 'FrontPage.PublishingStatus.Draft.Title',
    defaultMessage: 'Luonnos'
  },
  publishedLabel: {
    id: 'FrontPage.PublishingStatus.Published.Title',
    defaultMessage: 'Julkaistu'
  },
  deletedLabel: {
    id: 'FrontPage.PublishingStatus.Deleted.Title',
    defaultMessage: 'Arkistoitu'
  },
  modifiedLabel: {
    id: 'FrontPage.PublishingStatus.Modified.Title',
    defaultMessage: 'Muokattu'
  }
})

const renderPublishingStatuses = ({
  input,
  publishingStatuses,
  intl: { formatMessage }
}) => {
  const checkBoxes = publishingStatuses
    .sortBy(publishingStatus => publishingStatus.get('orderNumber'))
    .map((publishingStatus, index) => {
      const getCheckBoxProps = type => {
        switch (type) {
          case 'Draft':
            return {
              // draft: true,
              label: formatMessage(
                publishingStatusMessages.draftLabel
              ),
              type
            }
          case 'Published':
            return {
              // alpha: true,
              label: formatMessage(
                publishingStatusMessages.publishedLabel
              ),
              type
            }
          case 'Deleted':
            return {
              // beta: true,
              label: formatMessage(
                publishingStatusMessages.deletedLabel
              ),
              type
            }
          case 'Modified':
            return {
              // archived: true,
              label: formatMessage(
                publishingStatusMessages.modifiedLabel
              ),
              type
            }
        }
      }
      const handleOnChange = value => {
        const publishingStatusId = publishingStatus.get('id')
        if (value) {
          input.onChange(input.value.push(publishingStatusId))
        } else {
          const index = input.value.indexOf(publishingStatusId)
          input.onChange(input.value.delete(index))
        }
      }

      const checkBoxProps = getCheckBoxProps(publishingStatus.get('code'))

      return (
        <div key={index} className='publishing-status'>
          <Checkbox
            checked={input && input.value && input.value.contains(publishingStatus.get('id'))}
            {...checkBoxProps}
            onChange={handleOnChange}
            small
          />
          <PTVIcon name={`icon-${checkBoxProps.type.toLowerCase()}`} width={16} height={16} />
        </div>
      )
    })
  return (
    <div>
      <div className='form-field'>
        <strong>
          {formatMessage(publishingStatusMessages.title)}
        </strong>
      </div>
      <div className='flex'>
        {checkBoxes}
      </div>
    </div>
  )
}
renderPublishingStatuses.propTypes = {
  input: PropTypes.object.isRequired,
  publishingStatuses: PropTypes.array.isRequired,
  intl: intlShape.isRequired
}

export default compose(
  injectIntl,
  connect(
    state => ({
      publishingStatuses: getPublishingStatuses(state)
    })
  ),
  injectIntl
)(renderPublishingStatuses)
