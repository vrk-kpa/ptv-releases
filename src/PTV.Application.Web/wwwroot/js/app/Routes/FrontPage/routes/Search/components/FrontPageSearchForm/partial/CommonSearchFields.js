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
  FrontPageOrganization,
  Fulltext
} from 'util/redux-form/fields'
import { SearchTypeSwitcher } from 'Routes/FrontPage/routes/Search/components'
import { ArrowDown, ArrowUp } from 'appComponents'
import 'Routes/FrontPage/styles/style.scss'
import { injectIntl } from 'react-intl'
import { compose } from 'redux'
import cx from 'classnames'
import styles from '../styles.scss'

const CommonSearchFields = ({
  areSubFiltersVisible,
  toggleSubFilters,
  intl: { locale, formatMessage }
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
      <div className='col-lg-8'>
        <Fulltext
          big
          name='name'
          label='' />
      </div>
      <div className='col-md-5'>
        <SearchTypeSwitcher labelPosition='inside' />
      </div>
      <div className='col-md-5'>
        <FrontPageOrganization
          locale={locale}
          name='organizationId'
          labelPosition='inside'
        />
      </div>
      <div className='col-md-2 offset-md-4'>
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
  intl: PropTypes.object.isRequired,
  toggleSubFilters: PropTypes.func.isRequired,
  areSubFiltersVisible: PropTypes.bool.isRequired
}

export default compose(
  injectIntl
)(CommonSearchFields)
