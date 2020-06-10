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
import { SearchType, ClearableFulltext } from 'util/redux-form/fields'
import 'Routes/FrontPage/styles/style.scss'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import Tooltip from 'appComponents/Tooltip'
import SearchTypeAdditionalCustomContent from 'Routes/Search/components/SearchTypeAdditionalCustomContent'
import styles from '../styles.scss'
import commonStyles from '../../styles.scss'
import { searchTypesEnum, formTypesEnum } from 'enums'
import { connect } from 'react-redux'
import { formValueSelector } from 'redux-form/immutable'
import { SearchPhone } from 'Routes/Search/components/SearchPhone'
import { SearchAddress } from 'Routes/Search/components/SearchAddress'

const messages = defineMessages({
  namePlaceholder: {
    id: 'FrontPage.SearchName.Placeholder',
    defaultMessage: 'Sisällön nimi'
  },
  phonePlaceholder: {
    id: 'FrontPage.SearchPhone.Phone.Placeholder',
    defaultMessage: 'Puhelinnumero'
  },
  idPlaceholder: {
    id: 'FrontPage.SearchId.Placeholder',
    defaultMessage: 'PTV-tunniste'
  },
  emailPlaceholder: {
    id: 'FrontPage.SearchEmail.Placeholder',
    defaultMessage: 'Sähköpostiosoite'
  },
  nameTooltip: {
    id: 'FrontPage.SearchName.Tooltip',
    defaultMessage: 'Voit hakea palvelun, asiointikanavan tai organisaation nimellä.'
  },
  idTooltip: {
    id: 'FrontPage.SearchId.Tooltip',
    defaultMessage: 'ID search tooltip.'
  },
  emailTooltip: {
    id: 'FrontPage.SearchEmail.Tooltip',
    defaultMessage: 'Email search tooltip.'
  },
  phoneTooltip: {
    id: 'SearchPhone.LocalNumber.Tooltip',
    defaultMessage: 'Search PHONE: local number tooltip'
  }
})

const renderSearchComponent = ({ searchType, formatMessage, disabled }) => {
  switch (searchType) {
    case searchTypesEnum.ADDRESS:
      return (
        <div>
          <SearchType className={styles.searchType} disabled={disabled}>
            <SearchAddress disabled={disabled} />
          </SearchType>
        </div>
      )

    case searchTypesEnum.PHONE:
      return (
        <div className={styles.searchComponent}>
          <SearchType className={styles.searchType} disabled={disabled}>
            <SearchPhone
              phonePlaceholder={messages.phonePlaceholder}
              disabled={disabled}
            />
          </SearchType>
          <Tooltip tooltip={formatMessage(messages.phoneTooltip)} indent='i5' className={styles.searchTooltip} />
        </div>
      )

    default:
      const lowerSearchType = searchType.toLowerCase()
      const placeholderName = `${lowerSearchType}Placeholder`
      const tooltipName = `${lowerSearchType}Tooltip`
      return (
        <div className={styles.searchComponent}>
          <SearchType className={styles.searchType} disabled={disabled}>
            <div className={commonStyles.customContentWrap}>
              <ClearableFulltext
                name={lowerSearchType}
                placeholder={messages[placeholderName]}
                disabled={disabled}
              />
            </div>
          </SearchType>
          <Tooltip tooltip={formatMessage(messages[tooltipName])} indent='i5' className={styles.searchTooltip} />
        </div>
      )
  }
}

const CommonSearchFields = ({
  intl: { formatMessage },
  searchType,
  disabled
}) => {
  return (
    <div className={styles.commonSearchFields}>
      <div className='row align-items-center'>
        <div className='col-md-18 col-lg-12 col-xl-11'>
          { renderSearchComponent({ searchType, formatMessage, disabled }) }
        </div>
        <SearchTypeAdditionalCustomContent disabled={disabled} />
      </div>
    </div>
  )
}

CommonSearchFields.propTypes = {
  intl: intlShape.isRequired,
  searchType: PropTypes.string.isRequired,
  disabled: PropTypes.bool
}

CommonSearchFields.defaultProps = {
  searchType: searchTypesEnum.NAME
}

export default compose(
  injectIntl,
  connect(state => ({
    searchType: formValueSelector(formTypesEnum.FRONTPAGESEARCH)(state, 'searchType')
  }))
)(CommonSearchFields)
