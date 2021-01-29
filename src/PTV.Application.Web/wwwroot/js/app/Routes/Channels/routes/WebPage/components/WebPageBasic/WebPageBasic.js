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
import { injectIntl, intlShape } from 'util/react-intl'
import {
  UserSupport,
  AreaInformation,
  AccessibilityClassification
} from 'util/redux-form/sections'
import {
  Languages,
  NameEditor,
  Summary,
  Description,
  UrlChecker
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { isRequired } from 'util/redux-form/validators'
import { ChannelOrganization } from 'Routes/Channels/components'
import ChannelConnectionType from 'appComponents/ChannelConnectionType'
import { getIsPartOfServiceSet } from 'Routes/Service/selectors'
import ServiceCollections from 'Routes/Service/components/ServiceCollections'
import {
  channelDescriptionMessages,
  messages,
  phoneNumberMessages
} from './messages'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'

const WebPageBasic = ({
  intl: { formatMessage },
  isCompareMode,
  isPartOfServiceSet,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  return (
    <div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <NameEditor
              label={formatMessage(channelDescriptionMessages.nameTitle)}
              placeholder={formatMessage(channelDescriptionMessages.namePlaceholder)}
              tooltip={formatMessage(channelDescriptionMessages.nameTooltip)}
              required
              useQualityAgent
            />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ChannelOrganization
              label={formatMessage(channelDescriptionMessages.organizationLabel)}
              tooltip={formatMessage(channelDescriptionMessages.organizationInfo)}
              validate={isRequired}
              required
              {...rest} />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Summary
              label={formatMessage(channelDescriptionMessages.shortDescriptionLabel)}
              tooltip={formatMessage(channelDescriptionMessages.shortDescriptionInfo)}
              placeholder={formatMessage(channelDescriptionMessages.shortDescriptionPlaceholder)}
              required
              useQualityAgent
              qualityAgentCompare
            />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <Description
          label={formatMessage(channelDescriptionMessages.descriptionLabel)}
          tooltip={formatMessage(channelDescriptionMessages.descriptionInfo)}
          placeholder={formatMessage(channelDescriptionMessages.descriptionPlaceholder)}
          required
          useQualityAgent
          qualityAgentCompare
        />
      </div>
      <div className='form-row'>
        <UrlChecker
          label={formatMessage(channelDescriptionMessages.urlLabel)}
          tooltip={formatMessage(channelDescriptionMessages.urlTooltip)}
          placeholder={formatMessage(channelDescriptionMessages.urlPlaceholder)}
          required />
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Languages
              label={formatMessage(messages.label)}
              tooltip={formatMessage(messages.tooltip)}
              placeholderFromProps={messages.placeholder}
              required
            />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <UserSupport
          emailProps={
            {
              label: formatMessage(channelDescriptionMessages.emailTitle),
              tooltip: formatMessage(channelDescriptionMessages.emailTooltip),
              placeholder: formatMessage(channelDescriptionMessages.emailPlaceholder)
            }
          }
          dialCodeProps={
            {
              label: formatMessage(phoneNumberMessages.prefixTitle),
              tooltip: formatMessage(phoneNumberMessages.prefixTooltip),
              placeholder: formatMessage(phoneNumberMessages.prefixPlaceHolder)
            }
          }
          phoneNumberProps={
            {
              label: formatMessage(phoneNumberMessages.title),
              tooltip: formatMessage(phoneNumberMessages.tooltip),
              placeholder: formatMessage(phoneNumberMessages.placeholder)
            }
          }
          phoneNumberInfoProps={
            {
              label: formatMessage(phoneNumberMessages.infoTitle),
              tooltip: formatMessage(phoneNumberMessages.infoTooltip),
              placeholder: formatMessage(phoneNumberMessages.infoPlaceholder)
            }
          }
          chargeTypeProps={
            {
              label: formatMessage(phoneNumberMessages.chargeTypeTitle),
              tooltip: formatMessage(phoneNumberMessages.chargeTypeTooltip)
            }
          }
          phoneCostDescriptionProps={
            {
              label: formatMessage(phoneNumberMessages.costDescriptionTitle),
              tooltip: formatMessage(phoneNumberMessages.costDescriptionTooltip),
              placeholder: formatMessage(phoneNumberMessages.costDescriptionPlaceholder)
            }
          } />
      </div>
      <div className='form-row'>
        <AreaInformation />
      </div>
      <div className='form-row'>
        <ChannelConnectionType />
      </div>
      <div>
        <AccessibilityClassification />
      </div>

      {
        isPartOfServiceSet && (
          <div className='form-row'>
            <ServiceCollections />
          </div>
        )
      }
    </div>
  )
}

WebPageBasic.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool,
  isPartOfServiceSet: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect((state, ownProps) => ({
    isPartOfServiceSet: getIsPartOfServiceSet(state, ownProps)
  }))
)(WebPageBasic)
