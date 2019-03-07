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
import NoDataLabel from 'appComponents/NoDataLabel'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getPostalCode,
  getPostOffice,
  getCountry
} from './selectors'
import styles from './styles.scss'

const ConnectionAddressPreview = ({
  address,
  languageCode,
  postalCode,
  postOffice,
  country,
  ...rest
}) => {
  const addressType = address.get('streetType') || 'Street'
  const additionalInfo = address.get('additionalInformation') && (
    address.get('additionalInformation').get(languageCode) ||
    address.get('additionalInformation').first()
  ) || ''
  switch (addressType) {
    case 'Street':
      const streetName = address.get('street') && (
        address.get('street').get(languageCode) ||
        address.get('street').first()
      ) || ''
      const streetNumber = address.get('streetNumber') || ''
      return <div className={styles.previewItem}>
        <address>{`
            ${streetName}
            ${streetNumber}
          `} <br /> {`
            ${postalCode}
            ${postOffice}
          `}
        </address>
        <div className={styles.additionalInfo}>{additionalInfo}</div>
      </div>
    case 'PostOfficeBox':
      const poBox = address.get('poBox') && (
        address.get('poBox').get(languageCode) ||
        address.get('poBox').first()
      ) || ''
      return <div className={styles.previewItem}>
        <address>{`
            ${poBox}
          `} <br /> {`
            ${postalCode}
            ${postOffice}
          `}
        </address>
        <div className={styles.additionalInfo}>{additionalInfo}</div>
      </div>
    case 'Foreign':
      const foreignAddressText = address.get('foreignAddressText') && (
        address.get('foreignAddressText').get(languageCode) ||
        address.get('foreignAddressText').first()
      ) || ''
      return <div className={styles.previewItem}>
        <address>{`
            ${foreignAddressText}
          `} <br /> {`
            ${country}
          `}
        </address>
        <div>{additionalInfo}</div>
      </div>
    default:
      return <NoDataLabel />
  }
}

ConnectionAddressPreview.propTypes = {
  address: PropTypes.array,
  languageCode: PropTypes.string,
  postalCode: PropTypes.string,
  postOffice: PropTypes.string,
  country: PropTypes.string
}

export default compose(
  connect((state, ownProps) => ({
    languageCode: getContentLanguageCode(state, ownProps),
    postalCode: getPostalCode(state, ownProps),
    postOffice: getPostOffice(state, ownProps),
    country: getCountry(state, ownProps)
  }))
)(ConnectionAddressPreview)
