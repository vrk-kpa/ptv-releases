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
import {
  ClearableFulltext
} from 'util/redux-form/fields'
import SearchFilterContentTypes from '../../SearchFilterContentTypes'

import ArrowDown from 'appComponents/ArrowDown'
import ArrowUp from 'appComponents/ArrowUp'
import 'Routes/FrontPage/styles/style.scss'
import { injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import cx from 'classnames'
import SearchFilterOrganization from 'Routes/Search/components/SearchFilterOrganization'
import SearchFilterMyContent from 'Routes/Search/components/SearchFilterMyContent'
import styles from '../styles.scss'

const CommonSearchFields = ({
  areSubFiltersVisible,
  toggleSubFilters,
  intl: { locale }
}) => {
  const toggleButtonClass = cx(
    styles.toggleButton,
    {
      [styles.expanded]: areSubFiltersVisible,
      [styles.collapsed]: !areSubFiltersVisible
    }
  )
  return (
    <div className='row'>
      <div className='col-lg-7'>
        <ClearableFulltext name='name' />
      </div>
      <div className='col-md-5 align-self-center'>
        <SearchFilterMyContent className={styles.noMargin} />
      </div>
      <div className='col-md-5'>
        <SearchFilterContentTypes />
      </div>
      <div className='col-md-5'>
        <SearchFilterOrganization />
      </div>
      <div className='col-md-2 align-self-center'>
        <div
          className={toggleButtonClass}
          onClick={toggleSubFilters}>
          {areSubFiltersVisible ? <ArrowUp /> : <ArrowDown />}
        </div>
      </div>
    </div>
  )
}

CommonSearchFields.propTypes = {
  intl: intlShape.isRequired,
  toggleSubFilters: PropTypes.func.isRequired,
  areSubFiltersVisible: PropTypes.bool.isRequired
}

export default compose(
  injectIntl
)(CommonSearchFields)
