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
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { localizeList, languageTranslationTypes } from 'appComponents/Localize'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { getFileExtensions } from './selectors'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Type.Title',
    defaultMessage: 'Tiedostomuoto'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Type.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta lomakkeen tiedostomuoto. Jos lomakkeesta on tarjolla useampi eri tiedostomuoto, vie kukin tiedostomuoto erikseen.'
  }
})

const FileExtension = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => (
  <Field
    name='type'
    label={formatMessage(messages.label)}
    tooltip={formatMessage(messages.tooltip)}
    component={RenderSelect}
    {...rest}
  />
)
FileExtension.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  connect(state => ({
    options: getFileExtensions(state)
  })),
  localizeList({
    input: 'options',
    idAttribute: 'value',
    nameAttribute: 'label',
    languageTranslationType: languageTranslationTypes.locale
  }),
  asComparable({ DisplayRender: RenderSelectDisplay }),
  asDisableable,
  injectSelectPlaceholder()
)(FileExtension)
