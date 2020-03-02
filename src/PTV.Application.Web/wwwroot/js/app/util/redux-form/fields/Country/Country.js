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
import { localizeList } from 'appComponents/Localize'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { getCountryOptionsJS, getCountryDefaultOptionsJS } from './selectors'
import { CodeListSchemas } from 'schemas'
import { normalize } from 'normalizr'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.Country.Title',
    defaultMessage: 'Maa'
  }
})

const formatResponse = data =>
  data.map((country) => ({
    label: country.countryName,
    value: country.id,
    entity: country
  }))

const CountryRender = compose(
  connect(
    (state, { input: { value } }) => ({
      defaultOptions: value && getCountryDefaultOptionsJS(state, { id: value })
    })
  )
)(RenderAsyncSelect)

const Country = ({
  intl: { formatMessage },
  validate,
  dispatch,
  ...rest
}) => {
  const onChangeObject = (object) => {
    object && dispatch({ type: 'COUNTRIES', response: normalize(object.entity, CodeListSchemas.COUNTRY) })
  }

  return (
    <Field
      component={CountryRender}
      formatResponse={formatResponse}
      searchable
      name='country'
      filterOption={() => true}
      label={formatMessage(messages.label)}
      onChangeObject={onChangeObject}
      // validate={translateValidation(validate, formatMessage, messages.label)}
      url='common/GetCountries'
      {...rest}
    />
  )
}
Country.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  dispatch: PropTypes.func
}

export default compose(
  injectIntl,
connect(
    state => ({
      options: getCountryOptionsJS(state)
    })
  ),
  asComparable({ DisplayRender: RenderAsyncSelectDisplay }),
  asDisableable,
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  }),
  injectSelectPlaceholder()
)(Country)
