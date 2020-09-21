/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { getMunicipalityName } from './selectors'
import { localizeProps } from 'appComponents/Localize'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.MunicipalityName.Title',
    defaultMessage: 'Kunnan nimi ja kuntanumero'
  }
})

let MunicipalityNameRender = ({
  municipalityName,
  intl: { formatMessage },
  hideLabel,
  ...rest
}) => {
  return <div>
    {!hideLabel && <Label labelText={formatMessage(messages.label)} labelPosition='top' />}
    <div>{municipalityName}</div>
  </div>
}

MunicipalityNameRender.propTypes = {
  intl: intlShape,
  input: fieldPropTypes.input,
  hideLabel: PropTypes.bool,
  municipalityName: PropTypes.string
}

MunicipalityNameRender = compose(
  injectIntl,
  connect(
    (state, { input }) => ({
      id: input.value,
      municipalityName: getMunicipalityName(state, { municipalityId: input.value })
    })
  ),
  localizeProps({
    nameAttribute: 'municipalityName'
  })
)(MunicipalityNameRender)

const MunicipalityName = props => {
  return (
    <Field
      name='municipality'
      component={MunicipalityNameRender}
      {...props}
    />
  )
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: MunicipalityNameRender }),
  asDisableable
)(MunicipalityName)
