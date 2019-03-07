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
import { injectIntl, intlShape, defineMessages, FormattedMessage } from 'util/react-intl'
import asGroup from 'util/redux-form/HOC/asGroup'
import { Description } from 'util/redux-form/fields'
import { LawsExpanded } from 'util/redux-form/sections'
import CommonMessages from 'util/redux-form/messages'

export const bckgMessages = defineMessages({
  backgroundAreaTitle: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Title',
    defaultMessage: 'Tausta ja lainsäädäntö'
  },
  backgroundAreaTooltip: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Tooltip',
    defaultMessage: 'Tausta ja lainsäädäntö'
  },
  backgroundDescriptionTooltip: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Tooltip',
    defaultMessage: 'Taustakuvaus'
  },
  backgroundDescriptionPlaceholder: {
    id: 'Containers.GeneralDescription.BackgroundDescription.Description.Placeholder',
    defaultMessage: 'Taustakuvaus'
  }
})

const BackgroundInformation = ({
  intl: { formatMessage },
  required
}) => {
  return (
    <div>
      <div className='form-row'>
        <Description
          name='backgroundDescription'
          label={formatMessage(CommonMessages.backgroundDescription)}
          placeholder={formatMessage(bckgMessages.backgroundDescriptionPlaceholder)}
          tooltip={formatMessage(bckgMessages.backgroundDescriptionTooltip)}
          required={required}
        />
      </div>
      <div className='form-row'>
        <LawsExpanded />
      </div>
    </div>
  )
}

BackgroundInformation.propTypes = {
  intl: intlShape,
  required: PropTypes.bool
}

export default compose(
  injectIntl,
  asGroup({
    title: bckgMessages.backgroundAreaTitle,
    tooltip: <FormattedMessage {...bckgMessages.backgroundAreaTooltip} />
  })
)(BackgroundInformation)
