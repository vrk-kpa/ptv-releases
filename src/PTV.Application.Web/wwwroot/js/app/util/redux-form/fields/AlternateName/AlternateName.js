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
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import withTranslationLock from 'util/redux-form/HOC/withTranslationLock'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  }
})

const AlternateName = ({
  intl: { formatMessage },
  ...rest
}) => {
  return (
    <Field
      name='alternateName'
      component={RenderTextField}
      label={formatMessage(messages.label)}
      placeholder={formatMessage(messages.placeholder)}
      maxLength={100}
      {...rest}
    />
  )
}
AlternateName.propTypes = {
  intl:  intlShape,
  validate: PropTypes.func,
  ui: PropTypes.object,
  updateUI: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  withTranslationLock(),
  asDisableable,
  asLocalizable
)(AlternateName)
