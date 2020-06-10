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
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import CommonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Kirjoita puhelinnumero kansainvälisessä muodossa. Kirjoita ensin maan suuntanumero ja anna kansallinen numero ilman alkunollaa. Jos numeroon ei voi soittaa ulkomailta, voit antaa numeron ilman ulkomaansuuntanumeroa. Voit antaa useita puhelinnumeroita "uusi puhelinnumero" -painikkeella.'
  },
  placeholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. 451234567 (numero ilman alkunollaa)'
  }
})
const PhoneNumber = ({
  intl: { formatMessage },
  tooltip,
  ...rest
}) => (
  <Field
    name='phoneNumber'
    component={RenderTextField}
    label={formatMessage(CommonMessages.phoneNumber)}
    tooltip={tooltip || formatMessage(messages.tooltip)}
    placeholder={formatMessage(messages.placeholder)}
    maxLength={20}
    normalize={value => {
      if (!value) return value
      const plus = value.startsWith('+')
      return (plus ? '+' : '') + value.replace(/[^\d]/g, '')
    }}
    {...rest}
  />
)
PhoneNumber.propTypes = {
  intl: intlShape,
  isMandatory: PropTypes.bool,
  tooltip: PropTypes.oneOfType([
    PropTypes.object,
    PropTypes.string
  ]),
  validate: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.arrayOf(PropTypes.func)
  ])
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable
)(PhoneNumber)
