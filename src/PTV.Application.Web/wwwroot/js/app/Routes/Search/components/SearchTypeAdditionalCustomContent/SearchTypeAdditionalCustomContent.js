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
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { SearchPhoneAdditional } from 'Routes/Search/components/SearchPhone'
import { SearchAddressAdditional } from 'Routes/Search/components/SearchAddress'
import SearchFilterMyContent from 'Routes/Search/components/SearchFilterMyContent'
import { formValueSelector } from 'redux-form/immutable'
import { searchTypesEnum, formTypesEnum } from 'enums'
import { defineMessages } from 'util/react-intl'
import styles from '../styles.scss'

const messages = defineMessages({
  localNumberTooltip: {
    id: 'SearchPhone.LocalNumber.Tooltip',
    defaultMessage: 'Search PHONE: local number tooltip'
  }
})

const SearchTypeCustomContent = ({ searchType, disabled }) => {
  const searchPhoneAdditionalClass = 'col-xl-8 col-lg-12 col-md-18 col-sm-24 mt-2 mt-lg-0 align-self-center'
  const searchMyContentClass = 'offset-xl-7 col-xl-6 offset-lg-4 col-lg-8 offset-md-0 col-md-12 offset-sm-0 col-sm-24 mt-2 mt-lg-0 align-self-center' // eslint-disable-line
  const searchMyContentAddressClass = 'col-sm-12 col-md-10 col-lg-8 mt-2'
  const searchMyContentPhoneClass = 'col-xl-5 col-md-12 mt-2 mt-xl-0 align-self-center'
  return ({
    [searchTypesEnum.NAME]: (
      <div className={searchMyContentClass}>
        <SearchFilterMyContent componentClass={styles.flexEndLg} disabled={disabled} />
      </div>
    ),
    [searchTypesEnum.ADDRESS]: (
      <Fragment>
        <SearchAddressAdditional disabled={disabled} />
        <div className='w-100 d-block d-lg-none' />
        <div className={searchMyContentAddressClass}>
          <SearchFilterMyContent disabled={disabled} />
        </div>
      </Fragment>
    ),
    [searchTypesEnum.PHONE]: (
      <Fragment>
        <div className={searchPhoneAdditionalClass}>
          <SearchPhoneAdditional localNumberTooltip={messages.localNumberTooltip} disabled={disabled} />
        </div>
        <div className={searchMyContentPhoneClass}>
          <SearchFilterMyContent componentClass={styles.flexEndXl} disabled={disabled} />
        </div>
      </Fragment>
    ),
    [searchTypesEnum.ID]: (
      <div className={searchMyContentClass}>
        <SearchFilterMyContent componentClass={styles.flexEndLg} disabled={disabled} />
      </div>
    ),
    [searchTypesEnum.EMAIL]: (
      <div className={searchMyContentClass}>
        <SearchFilterMyContent
          componentClass={styles.flexEndLg}
          disabled
        />
      </div>
    )
  }[searchType])
}

SearchTypeCustomContent.propTypes = {
  searchType: PropTypes.string.isRequired
}

SearchTypeCustomContent.defaultProps = {
  searchType: searchTypesEnum.NAME
}

export default compose(
  connect(state => ({
    searchType: formValueSelector(formTypesEnum.FRONTPAGESEARCH)(state, 'searchType')
  }))
)(SearchTypeCustomContent)
