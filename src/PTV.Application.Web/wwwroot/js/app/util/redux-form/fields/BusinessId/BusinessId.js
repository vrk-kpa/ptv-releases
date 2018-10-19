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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Title',
    defaultMessage: 'Y-tunnus'
  },
  tooltip: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Tooltip',
    defaultMessage: 'Kirjota kenttään organisaatiosi Y-tunnus. Jos et muista sitä, voit hakea Y-tunnuksen Yritys- ja yhteisötietojärjestelmästä (YTJ) [https://www.ytj.fi/yrityshaku.aspx?path=1547;1631;1678&kielikoodi=1]. | Tarkista oman organisaatiosi Y-tunnuksen käytön käytäntö: Joillain organisaatioilla on vain yksi yhteinen Y-tunnus, toisilla myös alaorganisaatioilla on omat Y-tunnuksensa. Varmista, että annat alaorganisaatiolle oikean Y-tunnuksen.'
  },
  placeholder: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Organization.BusinessId.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  }
})

const BusinessId = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
    name='bussinesId'
    component={RenderTextField}
    label={formatMessage(messages.label)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    normalize={(value, previousValue) => {
      if (!value) return value
      const transformedValue = value.replace(/[^\d]/g, '')
      if (!previousValue || value.length > previousValue.length) {
        // typing forward
        if (transformedValue.length === 7) {
          return transformedValue + '-'
        }
      }
      if (transformedValue.length <= 7) {
        return transformedValue
      }
      return transformedValue.slice(0, 7) + '-' + transformedValue.slice(7, 8)
    }}
    {...rest}
  />
)
BusinessId.propTypes = {
  intl: intlShape,
  validate: PropTypes.oneOfType([
    PropTypes.func,
    PropTypes.arrayOf(PropTypes.func)
  ])
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable
)(BusinessId)
