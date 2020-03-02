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
import { ClearableFulltext, DialCode } from 'util/redux-form/fields'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { formValueSelector } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import styles from '../styles.scss'
import cx from 'classnames'

const messages = defineMessages({
  searchPromptText: {
    id: 'Routes.Search.SearchPhone.SearchPrompt.Title',
    defaultMessage: 'Kirjoita maa tai suuntanumero',
    description: { en:'Write country or country code' }
  }
})

const dialCodeValueRenderer = dialCode => dialCode && <span>({dialCode.code})</span>

const SearchPhone = ({
  isLocalNumber,
  phonePlaceholder,
  intl: { formatMessage }
}) => {
  const phoneWithDialClass = cx(styles.phoneWithDial, styles.over)
  const customContentWrapClass = cx(
    styles.customContentWrap,
    styles.noSeparator
  )
  return (
    <Fragment>
      <div className={phoneWithDialClass}>
        <div className={styles.dialCodeWrap}>
          <DialCode
            disabled={isLocalNumber}
            label={null}
            className={styles.dialCode}
            clearable={false}
            valueRenderer={dialCodeValueRenderer}
            searchPromptText={formatMessage(messages.searchPromptText)}
          />
        </div>
        <div className={customContentWrapClass}>
          <ClearableFulltext
            name='phone'
            placeholder={phonePlaceholder}
          />
        </div>
      </div>
    </Fragment>
  )
}

SearchPhone.propTypes = {
  isLocalNumber: PropTypes.bool,
  phonePlaceholder: PropTypes.object,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(state => {
    const getFormValues = formValueSelector(formTypesEnum.FRONTPAGESEARCH)
    return {
      isLocalNumber: getFormValues(state, 'isLocalNumber')
    }
  })
)(SearchPhone)
