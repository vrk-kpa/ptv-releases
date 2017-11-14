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
import { injectFormName } from 'util/redux-form/HOC'
import { connect } from 'react-redux'
import { injectIntl, defineMessages } from 'react-intl'
import { Label } from 'sema-ui-components'
import { getPreview } from '../../selectors'
import ImmutablePropTypes from 'react-immutable-proptypes'

export const messages = defineMessages({
  title: {
    id : 'ReduxForm.Sections.Producer.ProducerPreview.Title',
    defaultMessage: 'ESIKATSELU'
  },
  subTitle: {
    id : 'ReduxForm.Sections.Producer.ProducerPreview.SubTitle',
    defaultMessage: 'Palvelun tuottaa'
  }
})

const ProducerPreview = ({
  intl: { formatMessage },
  preview
}) => {
  const renderPreview = () => {
    return preview.map(p => (
      p ? <div>
        {p.organizers && <Label labelText={p.organizers} />}
        {p.additionalInformation && <div><Label labelText={p.additionalInformation} /></div>}
      </div> : null)
    )
  }

  return (
    <div>
      <Label
        labelText={formatMessage(messages.title)}
        labelPosition='top'
      />
      <Label
        labelText={formatMessage(messages.subTitle)}
        labelPosition='top'
      />
      {renderPreview()}
    </div>
  )
}

ProducerPreview.propTypes = {
  intl: PropTypes.object.isRequired,
  preview: ImmutablePropTypes.list.isRequired
}

export default compose(
  injectFormName,
  injectIntl,
  connect((state, ownProps) => ({
    preview: getPreview(state, ownProps)
  }))
)(ProducerPreview)
