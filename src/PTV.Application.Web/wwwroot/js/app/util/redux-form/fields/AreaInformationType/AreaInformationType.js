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
import { Field } from 'redux-form/immutable'
import { getAreaInformationTypesObjectArray } from './selectors'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { localizeList } from 'appComponents/Localize'
import { compose } from 'redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import {
  RenderSelect,
  RenderSelectDisplay,
  RenderRadioButtonGroup,
  RenderRadioButtonGroupDisplay
} from 'util/redux-form/renders'
import { asDisableable, asComparable } from 'util/redux-form/HOC'

const messages = defineMessages({
  label: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Type.Title',
    defaultMessage: 'Aluetieto täytetty organisaation mukaan. Voit muuttaa valintaa.'
  }
})

const AreaInformationType = ({
  intl: { formatMessage },
  options,
  radio,
  ...rest
}) => (
  <Field
    name='areaInformationType'
    component={radio ? RenderRadioButtonGroup : RenderSelect}
    label={formatMessage(messages.label)}
    options={options.map(option => ({
      label: option.name,
      value: option.id
    }))}
    {...rest}
  />
)
AreaInformationType.propTypes = {
  intl: intlShape,
  radio: PropTypes.bool,
  options: PropTypes.array
}

export default compose(
  injectIntl,
  asComparable({
    getDisplayRenderFromProps: ({ radio }) => radio
      ? RenderRadioButtonGroupDisplay
      : RenderSelectDisplay
  }),
  connect(
    state => ({
      options: getAreaInformationTypesObjectArray(state)
    })
  ),
  asDisableable,
  localizeList({
    input: 'options'
  }),
  injectSelectPlaceholder()
)(AreaInformationType)
