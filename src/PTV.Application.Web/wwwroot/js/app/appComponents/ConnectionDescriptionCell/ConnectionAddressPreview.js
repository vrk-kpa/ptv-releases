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
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import NoDataLabel from 'appComponents/NoDataLabel'
import { getContentLanguageCode } from 'selectors/selections'
import { PostOffice } from 'util/redux-form/fields'
import {
  getPostalCode,
  getCountry
} from './selectors'
import styles from './styles.scss'
import { localizeProps } from 'appComponents/Localize'

const Street = compose(
  localizeProps({
    nameAttribute: 'streetName',
    idAttribute: 'street'
  })
)(({
  additionalInfo,
  postalCode,
  postalCodeId,
  streetNumber,
  streetName
}) => {
  return <div className={styles.previewItem}>
    <address>
      {`
        ${streetName}
        ${streetNumber}
      `} <br />
      <PostOffice postalCodeId={postalCodeId} textOnly />
    </address>
    <div className={styles.additionalInfo}>{additionalInfo}</div>
  </div>
})

const ConnectionAddressPreview = ({
  address,
  languageCode,
  postalCode,
  country,
  ...rest
}) => {
  const addressType = address.get('streetType') || 'Street'
  const additionalInfo = address.get('additionalInformation') && (
    address.get('additionalInformation').get(languageCode) ||
    address.get('additionalInformation').first()
  ) || ''
  const postalCodeId = address.get('postalCode') || ''
  switch (addressType) {
    case 'Street':
      return <Street additionalInfo={additionalInfo}
        street={address.get('street')}
        streetNumber={address.get('streetNumber') || ''}
        postalCode={postalCode}
        postalCodeId={postalCodeId}
      />
    case 'PostOfficeBox':
      const poBox = address.get('poBox') && (
        address.get('poBox').get(languageCode) ||
        address.get('poBox').first()
      ) || ''
      return <div className={styles.previewItem}>
        <address>
          {poBox}
          <br />
          {postalCode}
          <PostOffice postalCodeId={postalCodeId} textOnly />
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
  address: ImmutablePropTypes.map,
  languageCode: PropTypes.string,
  postalCode: PropTypes.string,
  country: PropTypes.string
}

export default compose(
  connect((state, ownProps) => ({
    languageCode: getContentLanguageCode(state, ownProps),
    postalCode: getPostalCode(state, ownProps),
    country: getCountry(state, ownProps)
  }))
)(ConnectionAddressPreview)
