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
import { defineMessages, injectIntl } from 'react-intl'
import {
  asContainer,
  asSection,
  injectFormName,
  withFormStates
} from 'util/redux-form/HOC'
import { ServiceVoucher } from 'util/redux-form/fields'
import { ServiceVouchers } from 'util/redux-form/sections'
import cx from 'classnames'
import styles from './styles.scss'
import { formValueSelector } from 'redux-form/immutable'

export const voucherMessages = defineMessages({
  title: {
    id: 'Containers.Services.AddService.Step1.VoucherSection.Title',
    defaultMessage: 'Palvelusetelipalvelut'
  }
})

const ServiceVoucherSection = ({
  isReadOnly,
  isChecked
}) => {
  const serviceVoucherToggleClass = cx(
    styles.serviceVoucherToggle,
    {
      [styles.isReadOnly]: isReadOnly
    }
  )
  return (
    <div>
      <div className={serviceVoucherToggleClass}>
        <ServiceVoucher />
      </div>
      {isChecked && <ServiceVouchers /> || null}
    </div>
  )
}
ServiceVoucherSection.propTypes = {
  isReadOnly: PropTypes.bool,
  isChecked: PropTypes.bool.isRequired
}

export default compose(
  injectFormName,
  connect((state, { formName }) => ({
    isChecked: formValueSelector(formName)(state, 'serviceVouchers.serviceVoucherInUse') || false
  })),
  asContainer({
    title: voucherMessages.title,
    withCollection: true,
    dataPaths: 'serviceVouchers.serviceVoucherInUse'
  }),
  asSection('serviceVouchers'),
  injectIntl,
  withFormStates
)(ServiceVoucherSection)
