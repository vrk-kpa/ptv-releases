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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { RenderCheckBox, RenderCheckBoxDisplay } from 'util/redux-form/renders'
import { getPublishingStatuses } from './selectors'
import { Field, FormSection } from 'redux-form/immutable'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'

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

class PublishingStatus extends FormSection {
  static defaultProps = { name: 'publishingStatuses' }
  render () {
    const {
      publishingStatuses,
      skipCodes,
      intl: { formatMessage }
    } = this.props
    const filteredStatuses = skipCodes && Array.isArray(skipCodes) && skipCodes.length > 0
      ? publishingStatuses
        .filter(status => {
          const code = status.get('code') && status.get('code').toLowerCase()
          return !skipCodes.includes(code)
        })
      : publishingStatuses
    const checkBoxes = filteredStatuses
      .sortBy(publishingStatus => publishingStatus.get('orderNumber'))
      .map((publishingStatus, index) => {
        const getCheckBoxProps = type => {
          switch (type) {
            case 'Draft':
              return {
                label: formatMessage(publishingStatusMessages.draftLabel),
                type
              }
            case 'Published':
              return {
                label: formatMessage(publishingStatusMessages.publishedLabel),
                type
              }
            case 'Deleted':
              return {
                label: formatMessage(publishingStatusMessages.deletedLabel),
                type
              }
            case 'Modified':
              return {
                label: formatMessage(publishingStatusMessages.modifiedLabel),
                type
              }
            default:
              return {
                label: 'Not defined',
                type: ''
              }
          }
        }

        const checkBoxProps = getCheckBoxProps(publishingStatus.get('code'))
        const status = checkBoxProps.type.toLowerCase()

        return (
          <div key={index} className='publishing-status'>
            <Field
              name={publishingStatus.get('id')}
              component={RenderCheckBox}
              {...checkBoxProps}
              lakka={status === 'draft' || status === 'modified'}
              success={status === 'published'}
              archived={status === 'deleted'}
              small
            />
          </div>
        )
      })
    return (
      <div className={this.props.wrapClass}>
        {checkBoxes}
      </div>
    )
  }
}
PublishingStatus.propTypes = {
  publishingStatuses: ImmutablePropTypes.list.isRequired,
  skipCodes: PropTypes.array,
  intl: intlShape.isRequired
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderCheckBoxDisplay }),
  asDisableable,
  injectIntl,
  connect(
    state => ({
      publishingStatuses: getPublishingStatuses(state)
    })
  )
)(PublishingStatus)
