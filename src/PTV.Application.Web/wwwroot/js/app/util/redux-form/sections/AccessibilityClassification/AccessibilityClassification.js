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
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  Name,
  UrlChecker,
  WcagLevelType,
  AccessibilityClassificationLevelType
} from 'util/redux-form/fields'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asSection from 'util/redux-form/HOC/asSection'
import asContainer from 'util/redux-form/HOC/asContainer'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withPath from 'util/redux-form/HOC/withPath'
import {
  getIsWcagLevelTypeVisible,
  getIsNameAndUrlAddressVisible,
  getAccessibilityClassificationCode,
  getFormAccessibilityClassificationLevelTypeId
}
  from './selectors'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import commonMessages from 'util/redux-form/messages'

const messages = defineMessages({
  containerTooltip: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.Tooltip',
    defaultMessage: 'Saavutettavuustiedot'
  },
  nameTitle: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.Name.Title',
    defaultMessage: 'Nimi'
  },
  nameTooltip: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.Name.Tooltip',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.Name.Placeholder',
    defaultMessage: 'Nimi'
  },
  webAddressTitle: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.webAddress.Title',
    defaultMessage: 'Verkko-osoite'
  },
  webAddressTooltip: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.webAddress.Tooltip',
    defaultMessage: 'Verkko-osoite'
  },
  webAddressPlaceholder: {
    id: 'Util.ReduxForm.Sections.AccessibilityClassification.webAddress.Placeholder',
    defaultMessage: 'Verkko-osoite'
  }
})

const AccessibilityClassification = ({
  isCompareMode,
  splitView,
  intl: { formatMessage },
  isWcagLevelTypeVisible,
  isNameAndUrlAddressVisible,
  accessibilityClassificationLevelTypeCode
}) => {
  const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='collection-form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <AccessibilityClassificationLevelType
              isCompareMode={isCompareMode}
              clearable={false}
            />
          </div>
        </div>
      </div>
      {isWcagLevelTypeVisible &&
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <WcagLevelType
                isCompareMode={isCompareMode}
                code={accessibilityClassificationLevelTypeCode}
                required
              />
            </div>
          </div>
        </div>
      }

      {isNameAndUrlAddressVisible &&
        <Fragment>
          <div className='collection-form-row'>
            <div className='row'>
              <div className={basicCompareModeClass}>
                <Name
                  label={formatMessage(messages.nameTitle)}
                  tooltip={formatMessage(messages.nameTooltip)}
                  placeholder={formatMessage(messages.namePlaceholder)}
                  isCompareMode={isCompareMode}
                  skipValidation
                  isLocalized={false}
                  noTranslationLock
                  required
                />
              </div>
            </div>
          </div>
          <div className='collection-form-row'>
            <div className='row'>
              <div className={basicCompareModeClass}>
                <UrlChecker
                  label={formatMessage(messages.webAddressTitle)}
                  tooltip={formatMessage(messages.webAddressTooltip)}
                  placeholder={formatMessage(messages.webAddressPlaceholder)}
                  isCompareMode={isCompareMode}
                  isLocalized={false}
                  required
                />
              </div>
            </div>
          </div>
        </Fragment>
      }
    </div>
  )
}

AccessibilityClassification.propTypes = {
  isCompareMode: PropTypes.bool,
  splitView: PropTypes.bool,
  isWcagLevelTypeVisible: PropTypes.bool,
  isNameAndUrlAddressVisible: PropTypes.bool,
  accessibilityClassificationLevelTypeCode: PropTypes.string,
  intl: intlShape.isRequired
}

export default compose(
  injectIntl,
  withFormStates,
  asContainer({
    title: commonMessages.accessibilityClassifications,
    tooltip: messages.containerTooltip,
    dataPaths: 'accessibilityClassifications'
  }),
  asLocalizableSection('accessibilityClassifications'),
  asSection(),
  withPath,
  connect(
    (state, ownProps) => {
      const accessibilityClassificationLevelType = getFormAccessibilityClassificationLevelTypeId(state, ownProps)
      return {
        isWcagLevelTypeVisible: getIsWcagLevelTypeVisible(state, ownProps),
        isNameAndUrlAddressVisible: getIsNameAndUrlAddressVisible(state, ownProps),
        accessibilityClassificationLevelTypeCode: getAccessibilityClassificationCode(state, { id: accessibilityClassificationLevelType })
      }
    }
  ),
)(AccessibilityClassification)
