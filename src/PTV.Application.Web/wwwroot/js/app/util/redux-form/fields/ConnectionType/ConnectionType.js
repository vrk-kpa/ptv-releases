/**
* The MIT License
* Copyright (c) 2016 Population Register Centre (VRK)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape } from 'util/react-intl'
import { getServiceChannelConnectionTypesObjectArray } from './selectors'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { localizeList } from 'appComponents/Localize'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { createSelector } from 'reselect'

const getOptions = createSelector(
  getServiceChannelConnectionTypesObjectArray,
  connectionTypes => connectionTypes.map(option => ({
    label: option.name,
    value: option.id
  }))
)

const ConnectionType = ({
  options,
  intl: { formatMessage },
  validate,
  onConnectionTypeChange,
  ...rest
}) => {
  return (
    <Field
      name='connectionType'
      component={RenderRadioButtonGroup}
      options={options}
      label={null}
      // label={formatMessage(messages.title)}
      // validate={translateValidation(validate, formatMessage, messages.title)}
      onChange={onConnectionTypeChange}
      {...rest}
    />
  )
}
ConnectionType.propTypes = {
  options: PropTypes.array,
  intl: intlShape,
  validate: PropTypes.func,
  onConnectionTypeChange: PropTypes.func
}

export default compose(
  injectIntl,
  injectFormName,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  connect(
    state => ({
      options: getOptions(state)
    })
  ),
  asDisableable,
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  })
)(ConnectionType)
