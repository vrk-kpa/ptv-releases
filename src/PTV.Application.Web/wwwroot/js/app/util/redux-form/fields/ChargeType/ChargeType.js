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
import {
  RenderRadioButtonGroup,
  RenderSelect,
  RenderRadioButtonGroupDisplay,
  RenderSelectDisplay
} from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { localizeList } from 'appComponents/Localize'
import { getDefaultChargeType, getChargeTypes } from './selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'

const messages = defineMessages({
  label : {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  }
})

const ChargeType = ({
  options,
  intl: { formatMessage },
  validate,
  radio,
  defaultValue,
  ...rest
}) => (
  <Field
    name='chargeType'
    component={radio ? RenderRadioButtonGroup : RenderSelect}
    options={options}
    label={formatMessage(messages.label)}
    tooltip={formatMessage(messages.tooltip)}
    defaultValue={defaultValue}
    //validate={translateValidation(validate, formatMessage, messages.label)}
    {...rest}
  />
)
ChargeType.propTypes = {
  options: PropTypes.array.isRequired,
  radio: PropTypes.bool.isRequired,
  useDefaultValue: PropTypes.bool,
  intl: intlShape,
  validate: PropTypes.func,
  defaultValue: PropTypes.string
}
ChargeType.defaultProps = {
  radio: true
}

export default compose(
  injectIntl,
  asComparable({
    getDisplayRenderFromProps: ({ radio }) => radio
      ? RenderRadioButtonGroupDisplay
      : RenderSelectDisplay
  }),
  asDisableable,
  connect(
    (state, { useDefaultValue = true }) => ({
      options: getChargeTypes(state),
      defaultValue: useDefaultValue ? getDefaultChargeType(state) : null
    })
  ),
  localizeList({
    input:'options',
    idAttribute: 'value',
    nameAttribute: 'label'
  }),
  injectSelectPlaceholder()
)(ChargeType)
