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
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import {
  AreaInformation,
  UserSupport
} from 'util/redux-form/sections'
import {
  Name,
  Summary,
  Description,
  ChannelFormIdentifier
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import FormFilesCollection from '../FormFilesCollection'
import { AttachmentCollectionPRNT } from 'appComponents/AttachmentCollection'
import DeliveryAddressCollection from 'appComponents/DeliveryAddressCollection/DeliveryAddressCollection'
import { isRequired } from 'util/redux-form/validators'
import { ChannelOrganization } from 'Routes/Channels/components'
import { Map } from 'immutable'
import ChannelConnectionType from 'appComponents/ChannelConnectionType'

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa palvelupisteestä. Mikäli palvelupiste on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä palvelupisteestä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään palvelun.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa palvelupiste mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa yleisesti, mitä asioita palvelupisteessä voi hoitaa. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain palvelupistettä, älä palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  },
  emailTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  emailTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  emailPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. palvelupiste@organisaatio.fi'
  }
})

export const phoneNumberMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Mikäli verkkoasioinnin käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. +35845123467'
  },
  chargeTypeTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  chargeTypeTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun. Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  },
  costDescriptionTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  costDescriptionPlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  costDescriptionTooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  },
  infoTitle:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  infoTooltip:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Tooltip',
    defaultMessage: 'Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu.'
  },
  infoPlaceholder:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.AdditionalInformation.Placeholder',
    defaultMessage:'esim. Vaihde'
  },
  prefixTitle:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Title',
    defaultMessage:'Maan suuntanumero'
  },
  prefixPlaceHolder:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage:'esim. +358'
  },
  prefixTooltip:{
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage:'esim. +358'
  },
  finnishServiceNumberName: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.FinishServiceNumber.Name',
    defaultMessage: 'Suomalainen palvelunumero'
  }
})

export const attachmentMessages = defineMessages({
  namePlaceholder: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Name.Placeholder',
    description: 'Containers.Channels.AddPrintableFormChannel.Step1.WebPage.Name.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  descriptionTitle: {
    id: 'Container.ChannelAttachmentContainer.DescriptionLabel',
    defaultMessage: 'Kuvaus'
  },
  descriptionPlaceholder: {
    id: 'Container.ChannelAttachmentContainer.Description.Placeholder',
    defaultMessage: 'paikkamerkkiteksti'
  },
  tooltip: {
    id: 'Containers.Channels.AddPrintableFormChannel.Step1.Attachment.Tooltip',
    defaultMessage: 'Voit antaa linkkejä verkkoasiointikanavaan liittyviin ohjedokumentteihin tai -sivuihin.'
  }
})

const PrintableFormBasic = ({
  intl: { formatMessage },
  isCompareMode,
  ...rest
}) => {
  const basicCompareModeClass = isCompareMode ? 'col-lg-24' : 'col-lg-12'
  const defaultAddress = Map({ streetType: 'Street' })
  return (
    <div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <Name
              label={formatMessage(channelDescriptionMessages.nameTitle)}
              placeholder={formatMessage(channelDescriptionMessages.namePlaceholder)}
              required />
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
              {...rest}
            />
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
              required />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <Description
          label={formatMessage(channelDescriptionMessages.descriptionLabel)}
          tooltip={formatMessage(channelDescriptionMessages.descriptionInfo)}
          placeholder={formatMessage(channelDescriptionMessages.descriptionPlaceholder)}
          required />
      </div>
      <div className='form-row'>
        <div className='row'>
          <div className={basicCompareModeClass}>
            <ChannelFormIdentifier />
          </div>
        </div>
      </div>
      <div className='form-row'>
        <FormFilesCollection required />
      </div>
      <div className='form-row'>
        <DeliveryAddressCollection defaultItem={defaultAddress} />
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
          }
        />
      </div>
      <div className='form-row'>
        <AttachmentCollectionPRNT
          nameProps={{
            label: formatMessage(channelDescriptionMessages.nameTitle),
            placeholder: formatMessage(attachmentMessages.namePlaceholder)
          }}
          descriptionProps={{
            label: formatMessage(attachmentMessages.descriptionTitle),
            placeholder: formatMessage(attachmentMessages.descriptionPlaceholder)
          }}
          tooltip={attachmentMessages.tooltip}
        />
      </div>
      <div className='form-row'>
        <AreaInformation />
      </div>
      <div className='form-row'>
        <ChannelConnectionType />
      </div>
    </div>
  )
}

PrintableFormBasic.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates
)(PrintableFormBasic)
