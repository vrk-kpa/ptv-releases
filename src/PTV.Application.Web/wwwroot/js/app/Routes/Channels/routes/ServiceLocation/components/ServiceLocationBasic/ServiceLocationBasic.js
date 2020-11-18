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
import { injectIntl, intlShape } from 'util/react-intl'
import { AreaInformation } from 'util/redux-form/sections'
import {
  AlternativeName,
  IsAlternateNameUsed,
  NameEditor,
  Summary,
  Languages,
  Description
} from 'util/redux-form/fields'
import {
  isDefaultAddressExists,
  getGetDefaultSearchedAddress
} from './selectors'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { isRequired } from 'util/redux-form/validators'
import { VisitingAddressCollectionSL } from 'appComponents/VisitingAddressCollection'
import ContactInformation from '../ContactInformation'
import { ChannelOrganization } from 'Routes/Channels/components'
import { Map } from 'immutable'
import ChannelConnectionType from 'appComponents/ChannelConnectionType'
import { formTypesEnum } from 'enums'
import { channelDescriptionMessages } from './messages'
import ServiceCollections from 'Routes/Service/components/ServiceCollections'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getIsPartOfServiceSet } from 'Routes/Service/selectors'

const ServiceLocationBasic = ({
  intl: { formatMessage },
  isCompareMode,
  firstDefaultAddress,
  isFirstAddressExists,
  mapDisabled,
  isReadOnly,
  isPartOfServiceSet,
  ...rest
}) => {
  const defaultAddress = Map({ streetType: 'Street' })
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
            <AlternativeName
              tooltip={formatMessage(channelDescriptionMessages.alternativeNameTooltip)}
              placeholder={formatMessage(channelDescriptionMessages.alternativeNamePlaceholder)}
              useQualityAgent
            />
            <div className='mt-2'>
              <IsAlternateNameUsed />
            </div>
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
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Languages
              label={formatMessage(channelDescriptionMessages.languagesTitle)}
              tooltip={formatMessage(channelDescriptionMessages.languagesTooltip)}
              placeholderFromProps={channelDescriptionMessages.languagesPlaceholder}
              required />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <VisitingAddressCollectionSL
          mapDisabled={mapDisabled}
          required
          defaultItem={defaultAddress}
          firstDefaultItem={isFirstAddressExists && firstDefaultAddress}
          withinGroup={!isReadOnly}
          title={formatMessage(channelDescriptionMessages.visitingAddressGroupTitle)}
          tooltip={formatMessage(channelDescriptionMessages.visitingAddressGroupTooltip)} />
      </div>
      <div className='form-row'>
        <ContactInformation />
      </div>
      <div className='form-row'>
        <AreaInformation />
      </div>
      <div className='form-row'>
        <ChannelConnectionType />
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

ServiceLocationBasic.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  firstDefaultAddress: PropTypes.string,
  isFirstAddressExists: PropTypes.bool,
  mapDisabled: PropTypes.bool,
  isPartOfServiceSet: PropTypes.bool
}

export default compose(
  injectIntl,
  injectFormName,
  withFormStates,
  connect(
    (state, ownProps) => {
      const propsWithOtherFormName = { ...ownProps, formName: formTypesEnum.SERVICELOCATIONADDRESSSEARCHFORM }
      const firstDefaultAddress = getGetDefaultSearchedAddress(state, propsWithOtherFormName)
      const isFirstAddressExists = isDefaultAddressExists(state, propsWithOtherFormName)
      return {
        firstDefaultAddress,
        isFirstAddressExists,
        isPartOfServiceSet: getIsPartOfServiceSet(state, ownProps)
      }
    }
  )
)(ServiceLocationBasic)
