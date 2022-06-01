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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { Field, change } from 'redux-form/immutable'
import { List } from 'immutable'
import { getProvisionTypesObjectArray,
getFormOrganizations,
getProvisionTypeSelfProducedId } from './selectors'
import { connect } from 'react-redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { localizeList } from 'appComponents/Localize'
import { compose } from 'redux'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { RenderSelect, RenderSelectDisplay, RenderRadioButtonGroup } from 'util/redux-form/renders'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Services.AddService.Step3.ServiceProducer.ServiceDeliverySystem.Title',
    defaultMessage: 'Valitse toteutustapa'
  }
})

const ProvisionType = ({
  intl: { formatMessage },
  options,
  radio,
  initialValusesForSelfProducers,
  selfProducedId,
  formName,
  index,
  change,
  ...rest
}) => {
  const handleOnChange = (event, newValue, previousValue) => {
    if (newValue === selfProducedId) {
      change(formName, `serviceProducers[${index}].selfProducers`, initialValusesForSelfProducers)
    } else if (newValue !== previousValue && previousValue === selfProducedId) {
      change(formName, `serviceProducers[${index}].selfProducers`, List())
    }
  }
  return (
    <Field
      name='provisionType'
      component={radio ? RenderRadioButtonGroup : RenderSelect}
      label={formatMessage(messages.label)}
      onChange={handleOnChange}
      options={options.map(option => ({
        label: option.name,
        value: option.id
      }))}
      {...rest}
    />
  )
}

ProvisionType.propTypes = {
  intl: intlShape,
  radio: PropTypes.bool,
  change: PropTypes.func.isRequired,
  selfProducedId: PropTypes.string.isRequired,
  index: PropTypes.number.isRequired,
  formName: PropTypes.string.isRequired,
  initialValusesForSelfProducers: ImmutablePropTypes.orderedSet.isRequired,
  options: PropTypes.array
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  injectFormName,
  asDisableable,
  connect(
    (state, ownProps) => ({
      options: getProvisionTypesObjectArray(state, ownProps),
      initialValusesForSelfProducers: getFormOrganizations(state, ownProps),
      selfProducedId: getProvisionTypeSelfProducedId(state, ownProps)
    })
  , { change }),
  localizeList({
    input: 'options'
  }),
  injectSelectPlaceholder()
)(ProvisionType)
