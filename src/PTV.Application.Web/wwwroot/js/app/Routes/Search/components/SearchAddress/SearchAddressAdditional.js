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
import React, { Fragment } from 'react'
import {
  AddressNumber,
  PostalCode,
  PostOffice
} from 'util/redux-form/fields'
import Tooltip from 'appComponents/Tooltip'
import { formTypesEnum, addressTypesEnum } from 'enums'
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { connect } from 'react-redux'
import { formValueSelector } from 'redux-form/immutable'
import PropTypes from 'prop-types'
import styles from '../styles.scss'

const messages = defineMessages({
  streetNumberPlaceholder: {
    id: 'SearchAddress.StreetNumber.Placeholder',
    defaultMessage: 'Osoitenumero'
  },
  postalCodePlaceholder: {
    id: 'SearchAddress.PostalCode.Placeholder',
    defaultMessage: 'Postinumero'
  },
  postOfficeTooltip: {
    id: 'SearchAddress.PostOffice.Tooltip',
    defaultMessage: 'Search Address: post office tooltip'
  }
})

const SearchAddressAdditional = props => {
  const {
    intl: { formatMessage },
    postalCodeId
  } = props

  return (
    <Fragment>
      <div className='col-10 col-sm-8 col-md-6 col-lg-5 col-xl-4 mt-2 mt-md-0'>
        <div className={styles.searchAddressPropertyNumber}>
          <AddressNumber
            label={null}
            placeholder={formatMessage(messages.streetNumberPlaceholder)}
            skipValidation />
        </div>
      </div>
      <div className='w-100 d-block d-xl-none' />
      <div className='col-10 col-sm-8 col-md-6 col-lg-5 col-xl-4 mt-2 mt-xl-0'>
        <div className={styles.searchAddressPostalCode}>
          <PostalCode
            streetIdField='street'
            label={null}
            placeholderFromProps={messages.postalCodePlaceholder}
            skipValidation
            addressType={addressTypesEnum.STREET}
          />
        </div>
      </div>
      <div className='col-14 col-sm-16 col-lg-7 col-xl-5 mt-2 mt-xl-0'>
        {!!postalCodeId && (
          <div className='d-flex align-item-center'>
            <div className='align-self-center'>
              <PostOffice hideLabel fieldClass={StyleSheet.postOfficeField} />
            </div>
            <Tooltip tooltip={formatMessage(messages.postOfficeTooltip)} />
          </div>
        )}
      </div>
    </Fragment>
  )
}

SearchAddressAdditional.propTypes = {
  intl: intlShape,
  postalCodeId: PropTypes.string
}

export default compose(
  injectIntl,
  connect(state => {
    const getFormValues = formValueSelector(formTypesEnum.FRONTPAGESEARCH)
    return {
      postalCodeId: getFormValues(state, 'postalCode')
    }
  })
)(SearchAddressAdditional)
