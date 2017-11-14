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
import { RenderRadioButtonGroup, RenderRadioButtonGroupDisplay } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { asDisableable, asComparable } from 'util/redux-form/HOC'
import { addressUseCases, addressUseCasesEnum } from 'enums'

const messages = defineMessages({
  streetAddressType: {
    id: 'Containers.Channels.Address.Type.Street',
    defaultMessage: 'Katuosoite'
  },
  poboxAddressType: {
    id: 'Containers.Channels.Address.Type.POBox',
    defaultMessage: 'Postilokero-osoite'
  },
  foreignAddressType: {
    id: 'Containers.Channels.Address.Type.Foreign',
    defaultMessage: 'Ulkomainen osoite'
  },
  noAddressType: {
    id: 'Containers.Channels.Address.Type.NoAddress',
    defaultMessage: 'Toimitustieto sanallisesti'
  }
})

const AddressType = ({
  intl: { formatMessage },
  validate,
  addressUseCase,
  ...rest
}) => {
  let options = [
    {
      label: formatMessage(messages.streetAddressType),
      value: 'Street'
    },
    {
      label: formatMessage(messages.poboxAddressType),
      value: 'PostOfficeBox'
    },
    {
      label: formatMessage(messages.foreignAddressType),
      value: 'Foreign'
    },
    {
      label: formatMessage(messages.noAddressType),
      value: 'NoAddress'
    }

  ]
  switch (addressUseCase) {
    case addressUseCasesEnum.POSTAL :
      options = options.filter(option => option.value !== 'NoAddress')
      break
    case addressUseCasesEnum.VISITING :
      options = options.filter(option => option.value !== 'PostOfficeBox' && option.value !== 'NoAddress')
      break
    case addressUseCasesEnum.DELIVERY :
      options = options.filter(option => option.value !== 'Foreign')
      break
  }
  return (
    <Field
      name='streetType'
      component={RenderRadioButtonGroup}
      options={options}
      defaultValue='Street'
      {...rest}
    />
  )
}
AddressType.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  addressUseCase: PropTypes.oneOf(addressUseCases)
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  asDisableable
)(AddressType)
