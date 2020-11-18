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
import React, { Fragment } from 'react'
import styles from '../styles.scss'
import cx from 'classnames'
import { Street } from 'util/redux-form/fields'
import { addressUseCasesEnum, formTypesEnum } from 'enums'
import { compose } from 'redux'
import { injectIntl, defineMessages } from 'util/react-intl'
import { connect } from 'react-redux'
import { formValueSelector } from 'redux-form/immutable'
import { getSelectedLanguage } from 'Intl/Selectors'
import PropTypes from 'prop-types'

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
          inputClass={styles.streetInput}
          crossClass={styles.streetCross}
          customLanguage={props.customLanguage}
        />
      </div>
    </Fragment>
  )
}

SearchAddress.propTypes = {
  customLanguage: PropTypes.string,
  search: PropTypes.func
}

export default compose(
  injectIntl,
  connect(state => {
    const getFormValues = formValueSelector(formTypesEnum.FRONTPAGESEARCH)
    return {
      postalCodeId: getFormValues(state, 'postalCode'),
      customLanguage: getSelectedLanguage(state)
    }
  })
)(SearchAddress)
