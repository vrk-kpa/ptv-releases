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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Label } from 'sema-ui-components'
import { Field, fieldPropTypes } from 'redux-form/immutable'
import { getPostOffice } from './selectors'
import { localizeProps } from 'appComponents/Localize'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.PostOffice.Title',
    defaultMessage: 'Postitoimipaikka'
  }
})

let PostOfficeRender = ({
  postOffice,
  intl: { formatMessage },
  hideLabel,
  ...rest
}) => {
  return <div>
    {!hideLabel && <Label labelText={formatMessage(messages.label)} labelPosition='top' />}
    <div>{postOffice}</div>
  </div>
}

PostOfficeRender.propTypes = {
  intl: intlShape,
  input: fieldPropTypes.input,
  hideLabel: PropTypes.bool,
  postOffice: PropTypes.string
}

PostOfficeRender = compose(
  injectIntl,
  connect(
    (state, { input }) => ({
      id: input.value,
      postOffice: getPostOffice(state, { id: input.value })
    })
  ),
  localizeProps({
    nameAttribute: 'postOffice'
  })
)(PostOfficeRender)

const PostOffice = props => {
  return (
    <Field
      name='postalCode'
      component={PostOfficeRender}
      {...props}
    />
  )
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: PostOfficeRender }),
  asDisableable
)(PostOffice)
