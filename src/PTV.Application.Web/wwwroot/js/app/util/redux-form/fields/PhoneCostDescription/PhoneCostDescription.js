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
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { Field, formValues, change } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withPath from 'util/redux-form/HOC/withPath'
import shortId from 'shortid'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'
import { getLanguageCode } from 'selectors/selections'
import { connect } from 'react-redux'
import { polyfillFieldValue } from 'actions/phones'
import PropTypes from 'prop-types'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  placeholder: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  tooltip: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  }
})

const PhoneCostDescription = ({
  intl: { formatMessage },
  polyfillFieldValue,
  simple,
  ...rest
}) => (
  <Field
    name='chargeDescription'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    multiline
    rows={3}
    {...rest}
    onBlur={simple ? null : polyfillFieldValue}
  />
)
PhoneCostDescription.propTypes = {
  intl: intlShape,
  polyfillFieldValue: PropTypes.func,
  simple: PropTypes.bool
}

export default compose(
  injectIntl,
  withPath,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  formValues('order'),
  connect((state, ownProps) => ({
    contentLanguage: getLanguageCode(state, ownProps),
    polyfillFieldValue: polyfillFieldValue({
      dispatch: ownProps.dispatch,
      formName: ownProps.formName,
      path: ownProps.path
    })
  })),
  withQualityAgent({
    key: ({ contentLanguage = 'fi', order, collectionPrefix }) => `${collectionPrefix}.${contentLanguage}.${order}.chargeDescription`,
    customAction: ({ formName, order, path }) => (store) => {
      typeof order === 'undefined' && store.dispatch(change(formName, `${path}.order`, shortId.generate()))
    }
  })
)(PhoneCostDescription)
