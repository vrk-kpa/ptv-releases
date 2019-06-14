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
import styles from '../styles.scss'
import cx from 'classnames'
import { Street } from 'util/redux-form/fields'
import { addressUseCasesEnum, formTypesEnum } from 'enums'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'util/react-intl'
import { connect } from 'react-redux'
import { formValueSelector } from 'redux-form/immutable'
import { SearchIcon } from 'appComponents/Icons'

const messages = defineMessages({
  streetPlaceholder : {
    id: 'SearchAddress.Street.Placeholder',
    defaultMessage: 'Kadunnimi'
  }
})

const SearchAddress = props => {
  const searchAddressClass = cx(
    styles.customContentWrap,
    styles.searchAddress,
    styles.noSeparator
  )
  return (
    <Fragment>
      <div className={searchAddressClass}>
        <Street
          addressUseCase={addressUseCasesEnum.VISITING}
          tooltip={null}
          label={null}
          placeholderFromProps={messages.streetPlaceholder}
          skipValidation
          arrowRenderer={() => <SearchIcon size={16} />}
        />
      </div>
    </Fragment>
  )
}

export default compose(
  injectIntl,
  connect(state => {
    const getFormValues = formValueSelector(formTypesEnum.FRONTPAGESEARCH)
    return {
      postalCodeId: getFormValues(state, 'postalCode')
    }
  })
)(SearchAddress)
