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
import {
  RenderRadioButtonGroup,
  RenderRadioButtonGroupDisplay
} from 'util/redux-form/renders'
import { compose } from 'redux'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'

const messages = defineMessages({
  label: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Level.Title',
    defaultMessage: 'Organisaatiotaso *'
  },
  main: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Level.MainOrganization.Title',
    defaultMessage: 'Pääorganisaatio'
  },
  sub: {
    id: 'Containers.Manage.Organizations.Manage.Step1.Level.SubOrganization.Title',
    defaultMessage: 'Alaorganisaatio'
  }
})

const OrganizationGroupLevel = ({
  intl: { formatMessage },
  validate,
  ...rest
}) => {
  const options = [
    {
      value: false,
      label: formatMessage(messages.main)
    },
    {
      value: true,
      label: formatMessage(messages.sub)
    }
  ]
  return (
    <Field
      name='groupLevel'
      label={formatMessage(messages.label)}
      component={RenderRadioButtonGroup}
      options={options}
      {...rest}
    />
  )
}
OrganizationGroupLevel.propTypes = {
  intl: intlShape,
  validate: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable
)(OrganizationGroupLevel)
