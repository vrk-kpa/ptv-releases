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
import { getServiceClassesJS } from 'Routes/FrontPage/routes/Search/selectors'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import { localizeList } from 'appComponents/Localize'
import { RenderSelect, RenderSelectDisplay } from 'util/redux-form/renders'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { translateValidation } from 'util/redux-form/validators'
import { asComparable } from 'util/redux-form/HOC'

const messages = defineMessages({
  title: {
    id: 'FrontPage.ServiceClassCombo.Title',
    defaultMessage: 'Palveluluokka'
  },
  tooltip: {
    id: 'FrontPage.ServiceClassCombo.Tooltip',
    defaultMessage: 'Voit hakea palveluita myös palveluluokan mukaan. Palveluluokka on on aihetunniste, jonka avulla palvelut voidaan ryhmitellä ja löytää. Palvelu voi kuulua useaan eri luokkaan. Valitse pudotusvalikosta haluamasi palveluluokka.'
  }
})

const ServiceClass = ({
  intl: { formatMessage },
  serviceClasses,
  validate,
  ...rest
}) => (
  <Field
    name='serviceClass'
    label={formatMessage(messages.title)}
    tooltip={formatMessage(messages.tooltip)}
    component={RenderSelect}
    options={serviceClasses}
    //validate={translateValidation(validate, formatMessage, messages.title)}
    {...rest}
  />
)
ServiceClass.propTypes = {
  intl: intlShape.isRequired,
  validate: PropTypes.func,
  serviceClasses: PropTypes.array
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderSelectDisplay }),
  connect(
    state => ({
      serviceClasses: getServiceClassesJS(state)
    })
  ),
  localizeList({
    input:'serviceClasses',
    idAttribute: 'value',
    nameAttribute: 'label',
    isSorted: true
  }),
  injectSelectPlaceholder()
)(ServiceClass)
