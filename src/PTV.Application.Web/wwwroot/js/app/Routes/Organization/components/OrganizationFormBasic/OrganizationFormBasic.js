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
import React, { Component } from 'react'
import { compose } from 'redux'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import {
  BusinessId,
  Name,
  Description,
  Summary
} from 'util/redux-form/fields'
import asSection from 'util/redux-form/HOC/asSection'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import organizationMessages from '../messages'
import OrganizationWithGroupLevel from '../OrganizationWithGroupLevel'
import OrganizationWithAreaInformation from '../OrganizationWithAreaInformation'
import AlternateNameWithCheckbox from 'appComponents/AlternateNameWithCheckbox'
import TooltipLabel from 'appComponents/TooltipLabel'
import { getIsSoteOrganizationType } from 'util/redux-form/fields/OrganizationType/selectors'
import { injectIntl, intlShape } from 'util/react-intl'
import commonMessages from 'util/redux-form/messages'
import styles from '../styles.scss'
import SoteNote from 'appComponents/SoteNote'

const Business = compose(
  asSection('business')
)(props => <BusinessId {...props} name='code' />)
class OrganizationFormBasic extends Component {
  static defaultProps = {
    name: 'organizationBasic'
  }
  render () {
    const {
      isCompareMode,
      intl: { formatMessage },
      isSoteOrganization,
      isReadOnly
    } = this.props

    const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
    // const indentCompareModeClass = isCompareMode ? 'col-lg-24 mb-4' : 'col-lg-12 mb-4 mb-lg-0'
    return (
      <div>
        <div className='form-row'>
          <OrganizationWithGroupLevel />
        </div>

        <div className='form-row'>
          <OrganizationWithAreaInformation />
        </div>

        <div className='form-row'>
          <div className={styles.mixedField}>
            <div className='row'>
              <div className={basicCompareModeClass}>
                <TooltipLabel
                  componentClass={styles.topLabel}
                  labelProps={{
                    labelText: formatMessage(organizationMessages.nameTitle),
                    required: !isReadOnly
                  }}
                  tooltipProps={{
                    tooltip: formatMessage(organizationMessages.nameTooltip)
                  }}
                />
              </div>
            </div>
            <SoteNote showNote={isSoteOrganization} />
            <div className='row'>
              <div className={basicCompareModeClass}>
                <Name label={null}
                  placeholder={formatMessage(organizationMessages.namePlaceholder)}
                  disabled={isSoteOrganization}
                />
              </div>
            </div>
          </div>
        </div>

        <div className='form-row'>
          <AlternateNameWithCheckbox
            label={formatMessage(commonMessages.alternateName)}
            tooltip={formatMessage(organizationMessages.alternateNameTooltip)}
          />
        </div>

        <div className='form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <Business
                label={formatMessage(organizationMessages.businessLabel)}
                tooltip={formatMessage(organizationMessages.businessTooltip)}
                placeholder={formatMessage(organizationMessages.businessPlaceholder)} />
            </div>
          </div>
        </div>

        <div className='form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <Summary
                label={formatMessage(organizationMessages.shortDescriptionTitle)}
                tooltip={formatMessage(organizationMessages.shortDescriptionTooltip)}
                placeholder={formatMessage(organizationMessages.shortDescriptionPlaceholder)}
                required
                useQualityAgent
                qualityAgentCompare
              />
            </div>
          </div>
        </div>

        <div className='form-row'>
          <Description
            label={formatMessage(organizationMessages.organizationDescriptionTitle)}
            tooltip={formatMessage(organizationMessages.organizationDescriptionTooltip)}
            placeholder={formatMessage(organizationMessages.organizationDescriptionPlaceholder)}
            useQualityAgent
            qualityAgentCompare />
        </div>
      </div>
    )
  }
}

OrganizationFormBasic.propTypes = {
  isCompareMode: PropTypes.bool,
  intl: intlShape,
  isSoteOrganization: PropTypes.bool,
  isReadOnly: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    isSoteOrganization: getIsSoteOrganizationType(state)
  }))
)(OrganizationFormBasic)

