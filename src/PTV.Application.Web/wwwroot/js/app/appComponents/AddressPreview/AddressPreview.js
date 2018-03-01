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
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  StreetAddressPreview,
  POBoxAddressPreview,
  ForeignAddressPreview,
  NoDataLabel
} from 'appComponents'
import { Label } from 'sema-ui-components'
import { NoAddressAdditionalInformation } from 'util/redux-form/fields'
import { injectFormName } from 'util/redux-form/HOC'
import { formValueSelector } from 'redux-form/immutable'
import {
  formTypesEnum,
  addressUseCasesEnum
} from 'enums'
import CommonMessages from 'util/redux-form/messages'
import { injectIntl, intlShape } from 'react-intl'
import styles from './styles.scss'

const AddressPreview = ({
  addressType,
  isAddressFilledIn,
  intl: { formatMessage },
  ...rest
}) => {
  let addressPreviewTitle = ''
  switch (rest.addressUseCase) {
    case addressUseCasesEnum.VISITING:
      addressPreviewTitle = CommonMessages.visitingAddresses
      break
    case addressUseCasesEnum.POSTAL:
      addressPreviewTitle = CommonMessages.postalAddresses
      break
    case addressUseCasesEnum.DELIVERY:
      addressPreviewTitle = CommonMessages.deliveryAddress
      break
    default:
      addressPreviewTitle = ''
      break
  }

  if (!isAddressFilledIn) {
    return (
      <div>
        <Label labelText={formatMessage(addressPreviewTitle)} />
        <NoDataLabel
          required={
            rest.addressUseCase === addressUseCasesEnum.VISITING &&
            rest.formName === formTypesEnum.SERVICELOCATIONFORM
          } />
      </div>
    )
  }
  switch (addressType) {
    case 'Street':
      return <StreetAddressPreview
        title={formatMessage(addressPreviewTitle)}
        subTitle={formatMessage(CommonMessages.streetAddressType)}
        {...rest} />
    case 'PostOfficeBox':
      return <POBoxAddressPreview
        title={formatMessage(addressPreviewTitle)}
        subTitle={formatMessage(CommonMessages.poboxAddressType)}
        {...rest} />
    case 'Foreign':
      return <ForeignAddressPreview
        title={formatMessage(addressPreviewTitle)}
        subTitle={formatMessage(CommonMessages.foreignAddressType)}
        {...rest} />
    default:
      return <div>
        <div>
          <Label>
            {formatMessage(addressPreviewTitle)}
            <span className={styles.subTitle}>({formatMessage(CommonMessages.noAddressType)})</span>
          </Label>
        </div>
        <NoAddressAdditionalInformation {...rest} />
      </div>
  }
}

AddressPreview.propTypes = {
  addressType: PropTypes.oneOf(['Street', 'PostOfficeBox', 'Foreign', 'NoAddress']),
  isAddressFilledIn: PropTypes.bool,
  intl: intlShape
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, { formName, path }) => ({
    isAddressFilledIn: !!formValueSelector(formName)(state, path)
  }))
)(AddressPreview)
