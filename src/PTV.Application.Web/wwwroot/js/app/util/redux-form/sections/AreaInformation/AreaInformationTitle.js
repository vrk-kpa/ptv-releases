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
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  injectIntl,
  intlShape
} from 'util/react-intl'
import withLanguageKey from 'util/redux-form/HOC/withLanguageKey'
import NoDataLabel from 'appComponents/NoDataLabel'
import commonMessages from 'util/redux-form/messages'
import {
  getTranslatedAreaInformationType,
  getTranslatedAreaInformationMunicipalitiesTitle,
  getTranslatedAreaInformationProvincesTitle,
  getTranslatedAreaInformationBusinessRegionsTitle,
  getTranslatedAreaInformationHospitalRegionsTitle,
  getIsLimitedAreaInformationType
} from './selectors'
import cx from 'classnames'
import styles from './styles.scss'

const AreaInformationTitle = ({
  className,
  areaInformationType,
  municipalities,
  provinces,
  businessRegions,
  hospitalRegions,
  isLimitedArea,
  intl: { formatMessage },
  ...rest
}) => {
  const areaInfoTypeCSS = cx(
    styles.type,
    {
      [styles.standalone]: !isLimitedArea
    }
  )
  return (
    areaInformationType && <div className={className}>
      <span className={areaInfoTypeCSS}>{areaInformationType}</span>
      {isLimitedArea && <Fragment>
        {municipalities && <span className={styles.items}>
          <span>{formatMessage(commonMessages.municipalityLabel)}</span>
          <span>{municipalities}</span>
        </span>}
        {provinces && <span className={styles.items}>
          <span>{formatMessage(commonMessages.provinceLabel)}</span>
          <span>{provinces}</span>
        </span>}
        {businessRegions && <span className={styles.items}>
          <span>{formatMessage(commonMessages.businessRegionLabel)}</span>
          <span>{businessRegions}</span>
        </span>}
        {hospitalRegions && <span className={styles.items}>
          <span>{formatMessage(commonMessages.hospitalRegionLabel)}</span>
          <span>{hospitalRegions}</span>
        </span>}
      </Fragment>}
    </div> || <div className={className}><NoDataLabel /></div>
  )
}

AreaInformationTitle.propTypes = {
  className: PropTypes.string,
  areaInformationType: PropTypes.string,
  municipalities: PropTypes.string,
  provinces: PropTypes.string,
  businessRegions: PropTypes.string,
  hospitalRegions: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  withLanguageKey,
  connect((state, ownProps) => ({
    areaInformationType: getTranslatedAreaInformationType(state, ownProps),
    municipalities: getTranslatedAreaInformationMunicipalitiesTitle(state, ownProps),
    provinces: getTranslatedAreaInformationProvincesTitle(state, ownProps),
    businessRegions: getTranslatedAreaInformationBusinessRegionsTitle(state, ownProps),
    hospitalRegions: getTranslatedAreaInformationHospitalRegionsTitle(state, ownProps),
    isLimitedArea: getIsLimitedAreaInformationType(state, ownProps)
  }))
)(AreaInformationTitle)
