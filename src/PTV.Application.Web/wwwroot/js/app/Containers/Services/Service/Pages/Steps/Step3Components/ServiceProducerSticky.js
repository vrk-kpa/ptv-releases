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
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import { defineMessages, injectIntl } from 'react-intl'
import { camelize } from 'humps'

// Schemas
import { PTVLabel } from '../../../../../../Components'
import ServiceProducerStickyProvisionTypes from './ServiceProducerStickyProvisionTypes'

// Selectors
import * as ServiceSelectors from '../../../Selectors'
import * as ServiceCommonSelectors from '../../../../Common/Selectors'

// actions
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'

const messages = defineMessages({
  serviceProducersPreviewHeaderTitle:{
    id : 'Containers.Services.AddService.Step3.ServiceProducer.Preview.Header.Title',
    defaultMessage : 'ESIKATSELU'
  },
  serviceProducersPreviewTitle:{
    id : 'Containers.Services.AddService.Step3.ServiceProducer.Preview.Title',
    defaultMessage : 'Palvelun tuottaa'
  }
})

export const ServiceProducerSticky = ({
    serviceProducers,
    intl : { formatMessage },
    language,
    provisionTypes,
    className
 }) => {
  const renderProducer = (producer, index) => {
    const provisionTypeCode = producer.get('provisionTypeId') && provisionTypes.find(type => type.get('id') === producer.get('provisionTypeId')).get('code')
    const data = {
      producerId: producer.get('id'),
      language: language,
      organizationId: producer.get('organizationId'),
      additionalInformation: producer.get('additionalInformation')
    }

    switch (provisionTypeCode && camelize(provisionTypeCode)) {
      case 'selfProduced':
        return <ServiceProducerStickyProvisionTypes.SelfProducerSticky {...data} />
      case 'purchaseServices':
        return <ServiceProducerStickyProvisionTypes.PurchaseServicesSticky {...data} />
      case 'other':
        return <ServiceProducerStickyProvisionTypes.OtherProducerSticky {...data} />
      default:
        return null
    }
  }

  return (
    <div className={className}>
      <div className='entity-preview-header'>
        <div className='entity-preview-title'>
          <PTVLabel labelClass='strong'>
            { formatMessage(messages.serviceProducersPreviewHeaderTitle) }
          </PTVLabel>
        </div>
        {serviceProducers.size > 0 &&
          <div className='entity-preview-label'>
            <PTVLabel labelClass='strong'>
              { formatMessage(messages.serviceProducersPreviewTitle) }
            </PTVLabel>
          </div>
        }
      </div>
      <div className='entity-preview-body'>
        {serviceProducers.size > 0 && serviceProducers.map((producer, index) => renderProducer(producer, index))}
      </div>
    </div>
  )
}

function mapStateToProps (state, ownProps) {
  return {
    serviceProducers: ServiceSelectors.getProducersEntities(state, ownProps),
    provisionTypes: ServiceCommonSelectors.getProvisionTypes(state, ownProps)
  }
}

const actions = []

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(ServiceProducerSticky))
