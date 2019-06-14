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
import { localizeProps } from 'appComponents/Localize'
import { connect } from 'react-redux'
import { getContentLanguageCode } from 'selectors/selections'
import {
  getPostalCode,
  getPostOffice
} from './selectors'
import { compose } from 'redux'

const AddressCell = ({
  street,
  streetNumber,
  postalCode,
  postOffice,
  languageCode,
  ...rest
}) => {
  return (
    <div>
      {street + ' ' + streetNumber + ', ' + postalCode + ' ' + postOffice }
    </div>
  )
}

AddressCell.propTypes = {
  street: PropTypes.any,
  postalCode: PropTypes.string,
  postOffice: PropTypes.string,
  streetNumber: PropTypes.string,
  languageCode: PropTypes.string
}
export default compose(
  connect(
    (state, ownProps) => ({
      languageCode: ownProps.languageCode || getContentLanguageCode(state, ownProps),
      postalCode: getPostalCode(state, { id: ownProps.postalCode }),
      postOffice: getPostOffice(state, { id: ownProps.postalCode })
    }),
  ),
  localizeProps({
    nameAttribute: 'street',
    idAttribute: 'street'
  })
)(AddressCell)
