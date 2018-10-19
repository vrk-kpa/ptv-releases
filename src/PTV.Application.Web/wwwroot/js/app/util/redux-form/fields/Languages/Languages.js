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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { localizeList } from 'appComponents/Localize'
import { RenderMultiSelect, RenderMultiSelectDisplay } from 'util/redux-form/renders'
import { getAllLanguages } from './selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  placeholder: {
    id: 'Containers.Services.AddService.Step1.AvailableLanguages.Placeholder',
    defaultMessage: 'Kirjoita ja valitse listasta palvelun kielet.'
  },
  tooltip: {
    id: 'Containers.Services.AddService.Step1.AvailableLanguages.Tooltip',
    defaultMessage: 'Valitse tähän ne kielet, joilla palvelua tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet.'
  }
})

const customSort = values => {
  const ordered = values.slice(0, 3)
  const rest = values.slice(3, values.length).sort((a, b) => a['label'] < b['label'] ? -1 : 1)
  return [...ordered, ...rest]
}

const Languages = ({
  intl: { formatMessage },
  options,
  validate,
  ...rest
}) => (
  <Field
    name='languages'
    label={formatMessage(CommonMessages.languages)}
    labelTop
    tooltip={formatMessage(messages.tooltip)}
    component={RenderMultiSelect}
    options={options}
    {...rest}
  />
)
Languages.propTypes = {
  validate: PropTypes.func,
  options: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(
    state => ({
      options: getAllLanguages(state)
    })
  ),
  asComparable({ DisplayRender: RenderMultiSelectDisplay }),
  localizeList({
    input: 'options',
    idAttribute: 'id',
    nameAttribute: 'label',
    isSorted: customSort
  }),
  asDisableable,
  injectSelectPlaceholder({ placeholder: messages.placeholder })
)(Languages)
