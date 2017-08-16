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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { camelize } from 'humps'

import { ButtonAdd, ButtonDelete } from '../../../../../Common/Buttons'
import ServiceProvisionTypes from './ServiceProducerProvisionTypes'
import * as mainActions from '../../../Actions'
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'
import * as PTVValidatorTypes from '../../../../../../Components/PTVValidators'
import PTVLabel from '../../../../../../Components/PTVLabel'
import { LocalizedText } from '../../../../../Common/localizedData'
import * as ServiceSelectors from '../../../Selectors'
import * as CommonServiceSelectors from '../../../../Common/Selectors'
// components
// import {  PTVAutoComboBox } from '../../../../../../Components';
import { LocalizedComboBox } from '../../../../../Common/localizedData'

const messages = defineMessages({
  serviceProducerTitle: {
    id: 'Containers.Services.AddService.Step3.ServiceProducer.ServiceDeliverySystem.Title',
    defaultMessage: 'Valitse toteutustapa'
  },
  serviceProducerTooltip: {
    id: 'Containers.Services.AddService.Step3.ServiceProducer.ServiceDeliverySystem.Tooltip',
    defaultMessage: 'Palvelun tuottaja on taho, joka käytännössä toteuttaa palvelun asiakkaille. Valitusta palvelun toteutustavasta riippuen tuottajan kuvaaminen tapahtuu eri tavalla. Valitse pudotusvalikosta ensin palvelun toteutustapa ja sitten palvelun tuottaja. Jos tuottaja on sama kuin vastuutaho, valitse pudotusvalikosta toteutustapa "itse tuotetut palvelut". Tällöin vastuutaho-kohtaan valitsemasi organisaatio/t kopioituvat tuottaja-kohtaan. Mikäli olet valinnut vastuutaho-kohdassa useamman kuin yhden organisaation ja vain yksi tai osa niistä vastaa palvelun tuottamisesta, voit poistaa ylimääräiset organisaation nimen vieressä olevalla Poista-painikkeella. Mikäli palvelua tuottaa vastuutahon hankkimana ostopalveluna jokin toinen taho, valitse toteutustavaksi "ostopalvelut". Tällöin saat näkyviin organisaatiohakemiston, josta voit valita oikean tuottajan. Jos ostopalvelun tuottajan tiedot puuttuvat palvelutietovarannosta, kirjoita tuottajan nimi ja yhteystiedot vapaaseen tekstikenttään. Mikäli asiakas voi hankkia palvelun palvelusetelillä, valitse "palvelusetelipalvelut". Tällöin saat näkyviin Linkki palvelun tuottajien tietoihin -kentän. Syötä kenttään sen verkkosivun osoite, jolla organisaatiosi ylläpitää ajantasaista listaa tahoista, joilta palvelusetelillä voi hankkia palvelua. Mikäli mikään näistä vaihtoehdoista ei ole sopiva, valitse "Muu". Esimerkiksi jos virasto x tuottaa palvelua ministeriön y toimeksiannosta, valitse "Muu"-vaihtoehto.'
  },
  addProducerButton: {
    id: 'Containers.Services.AddService.Step3.ServiceProducer.Button.AddProducer',
    defaultMessage: 'Lisää palvelun tuottaja'
  },
  serviceProducerReadOnlyTitle: {
    id: 'Containers.Services.AddService.Step3.ServiceProducer.ServiceDeliverySystem.ReadOnlyTitle',
    defaultMessage: 'Toteutustapa'
  }
})

export const ServiceProducerComponent = ({
  isNew,
  id,
  actions,
  onAddProducer,
  intl,
  producer,
  provisionTypes,
  provisionTypeId,
  selectedProvisionType,
  organizationId,
  additionalInformation,
  readOnly,
  order,
  language,
  translationMode,
  selfProducedId,
  defaultOrganizers }) => {

  const validators = [PTVValidatorTypes.IS_REQUIRED]

  const onInputChange = (input, defId, defValues) => value => {
    const inputObject = value === defId
    ? {
      id: id,
      [input]: value,
      organizers: defValues.toJS()
    }
     : {
       id: id,
       [input]: value
     }

    if (!isNew) {
      actions.onProducerObjectChange(id, inputObject, language, true)
    } else {
      onAddProducer([inputObject], language)
    }
  }

  const getContentByProvisionType = (
    code,
    producerId,
    organizationId,
    additionalInformation,
    readOnly,
    language,
    translationMode) => {
    let selectedCode = camelize(code)

    const data = {
      producerId: producerId,
      readOnly: readOnly,
      language: language,
      translationMode: translationMode,
      organizationId: organizationId,
      additionalInformation: additionalInformation
    }

    switch (selectedCode) {
      case 'selfProduced':
        return <ServiceProvisionTypes.SelfProducer {...data} />
      case 'purchaseServices':
        return <ServiceProvisionTypes.PurchaseServices {...data} />
      case 'other':
        return <ServiceProvisionTypes.OtherProducer {...data} />
      default:
        return null
    }
  }

  return (
    !(readOnly || translationMode === 'view' || translationMode === 'edit')
      ? <div className='service-producer item-row'>

        <div>
          <div className='row'>
            <div className='col-xs-12'>
              <LocalizedComboBox id='Services.AddService.Step3.ProvisionType' role='alert'
                order={90}
                tooltip={intl.formatMessage(messages.serviceProducerTooltip)}
                label={intl.formatMessage(messages.serviceProducerTitle)}
                name='serviceProvider'
                value={provisionTypeId}
                values={provisionTypes}
                changeCallback={onInputChange('provisionTypeId', selfProducedId, defaultOrganizers)}
                componentClass='row'
                labelClass='col-xs-12'
                autoClass='col-xs-12'
                className='limited w480'
                language={language}
                validatedField={messages.serviceProducerTitle}
                validators={validators}
              />
            </div>
          </div>
          <div className='row'>
            <div className='col-xs-12'>
              {selectedProvisionType ? getContentByProvisionType(selectedProvisionType, producer.get('id'), organizationId, additionalInformation, readOnly, language, translationMode) : null}
            </div>
          </div>
        </div>
      </div>
      : <div className='row'>
        <div className='col-xs-12 col-sm-4 col-lg-6'>
          {selectedProvisionType
            ? <div>
              <PTVLabel labelClass='strong'>{intl.formatMessage(messages.serviceProducerReadOnlyTitle)}</PTVLabel>
              <LocalizedText
                id={provisionTypeId}
                name={selectedProvisionType}
                language={language}
              />
              {getContentByProvisionType(selectedProvisionType, producer.get('id'), organizationId, additionalInformation, readOnly, language, translationMode)}
            </div>
            : null}
        </div>
      </div>
  )
}

ServiceProducerComponent.propTypes = {
  intl: intlShape.isRequired,
  actions: PropTypes.object.isRequired,
  producer: PropTypes.object.isRequired
}

// / maps state to props
function mapStateToProps (state, ownProps) {
  const producer = ServiceSelectors.getProducer(state, ownProps)
  const isSelfProducerSelected = ServiceSelectors.isSelfProducerSelected(state, ownProps)
  const selectedProvisionType = ServiceSelectors.getSelectedProvisionTypeCode(state, ownProps)
  const provisionTypes = selectedProvisionType && camelize(selectedProvisionType) === 'selfProduced'
    ? CommonServiceSelectors.getProvisionTypesObjectArray(state)
    : isSelfProducerSelected
      ? CommonServiceSelectors.getRestProvisionTypesObjectArray(state)
      : CommonServiceSelectors.getProvisionTypesObjectArray(state)

  return {
    producer: producer,
    provisionTypeId: producer.get('provisionTypeId'),
    provisionTypes: provisionTypes,
    selectedProvisionType: selectedProvisionType,
    organizationId: ServiceSelectors.getSelectedProducerOrganizationId(state, ownProps),
    additionalInformation: ServiceSelectors.getSelectedProducerAdditionalInformation(state, ownProps),
    selfProducedId: ServiceSelectors.getProvisionTypesSelfProducedId(state),
    defaultOrganizers: ServiceSelectors.getSelectedOrganizersWithMainOrganization(state, ownProps)
   // details: selectedProvisionType ? ServiceSelectors.getServiceProducersDetail(state, { id: ownProps.id, type: camelize(selectedProvisionType), language: ownProps.language }) : null,
   // detailsDefault: selectedProvisionType ? ServiceSelectors.getFromServiceProducersDetail(state, { id: ownProps.id, type: camelize(selectedProvisionType), keyToState: 'service' }) : null
  }
}

const actions = [
  mainActions
]
export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceProducerComponent))
