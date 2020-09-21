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
import { compose } from 'redux'
import { connect } from 'react-redux'
import cx from 'classnames'
import styles from './styles.scss'
import { injectIntl } from 'util/react-intl'
import NoDataLabel from 'appComponents/NoDataLabel'
import ConnectionBasicInformation from './ConnectionBasicInformation'
import ConnectionAstiInformation from './ConnectionAstiInformation'
import ConnectionContactDetails from './ConnectionContactDetails'
import ConnectionAuthorization from './ConnectionAuthorization'
import ConnectionOpeningHours from './ConnectionOpeningHours'
import {
  getHasBasicInformation,
  getHasAstiInformation,
  getHasOpeningHours,
  getHasContactDetails,
  getHasDigitalAuthorization
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'

const ConnectionDescriptionCell = ({
  index,
  parentIndex,
  isAsti,
  hasBasicInformation,
  hasAstiInformation,
  hasOpeningHours,
  hasContactDetails,
  hasDigitalAuthorization,
  childPath
}) => {
  const hasConnectionDetails = hasBasicInformation || hasOpeningHours || hasContactDetails || hasDigitalAuthorization
  return (
    <div className={cx('cell', styles.connectionDescriptionCell)}>
      {!hasConnectionDetails && <NoDataLabel /> || <React.Fragment>
        {hasBasicInformation && <ConnectionBasicInformation field={`${childPath}.basicInformation`} />}
        {isAsti && hasAstiInformation && <ConnectionAstiInformation field={`${childPath}.astiDetails`} />}
        {hasOpeningHours && <ConnectionOpeningHours field={`${childPath}`} />}
        {hasContactDetails && <ConnectionContactDetails field={`${childPath}.contactDetails`} />}
        {hasDigitalAuthorization && <ConnectionAuthorization field={`${childPath}.digitalAuthorization`} />}
      </React.Fragment>}
    </div>
  )
}

ConnectionDescriptionCell.propTypes = {
  index: PropTypes.any,
  parentIndex: PropTypes.any,
  isAsti: PropTypes.bool,
  hasBasicInformation: PropTypes.bool,
  hasAstiInformation: PropTypes.bool,
  hasOpeningHours: PropTypes.bool,
  hasContactDetails: PropTypes.bool,
  hasDigitalAuthorization: PropTypes.bool,
  childPath: PropTypes.string
}

export default compose(
  injectIntl,
  injectFormName,
  withLanguageKey,
  connect((state, { connection, formName, isAsti, parentIndex, index, languageKey }) => {
    const childType = isAsti ? 'astiChilds' : 'childs'
    const childPath = `connections[${parentIndex}].${childType}[${index}]`
    return {
      hasBasicInformation: getHasBasicInformation(state, {
        formName,
        languageKey,
        field:
        `${childPath}.basicInformation`
      }),
      hasAstiInformation: getHasAstiInformation(state, { formName, languageKey, field: `${childPath}.astiDetails` }),
      hasOpeningHours: getHasOpeningHours(state, { formName, languageKey, field: `${childPath}.openingHours` }),
      hasContactDetails: getHasContactDetails(state, { formName, languageKey, field: `${childPath}.contactDetails` }),
      hasDigitalAuthorization: getHasDigitalAuthorization(state, {
        formName,
        languageKey,
        field:
        `${childPath}.digitalAuthorization`
      }),
      childPath
    }
  })
)(ConnectionDescriptionCell)
