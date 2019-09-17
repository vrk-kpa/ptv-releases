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
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import asComparable from 'util/redux-form/HOC/asComparable'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import withQualityAgent from 'util/redux-form/HOC/withQualityAgent'
import { getTextValue } from 'util/redux-form/HOC/withQualityAgent/defaultFunctions'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeName.Title',
    defaultMessage: 'Vaihtoehtoinen nimi'
  },
  placeholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeName.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.AlternativeNumber.Tooltip',
    defaultMessage: 'esim. Kela'
  }
})

const AlternativeName = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='alternateName'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    maxLength={100}
    counter
    {...rest}
  />
)
AlternativeName.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  asLocalizable,
  withQualityAgent({
    key: ({ language = 'fi' }, { entityPrefix = '' }, { type }) => `${entityPrefix}Names.${type}.${language}`,
    property: (_, { entityPrefix }) => `${entityPrefix}Names`,
    options: {
      type: 'AlternativeName'
    },
    getValue: getTextValue
  })
)(AlternativeName)
