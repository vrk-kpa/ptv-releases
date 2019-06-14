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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
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

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita verkkosivukanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  nameTooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Name.Tooltip',
    defaultMessage: 'Web channel name tooltip.'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa verkkosivusta. Mikäli verkkosivusta vastaa usempi alaorganisaatio, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä verkkosivun keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.  '
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa verkkosivun sisältö mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa mistä asiasta verkkosivu tarjoaa lisätietoa ja miten sitä voi käyttää (erityisesti, jos kyseessä on esim. demo, chat tai etäpalvelu). Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain verkkosivukanavaa, älä palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  },
  urlLabel: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Title',
    defaultMessage: 'Verkko-osoite'
  },
  urlTooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  urlPlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.UrlChecker.Placeholder',
    defaultMessage: 'Kopioi ja liitä tarkka verkko-osoite.'
  },
  emailTitle: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  emailTooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli tulostettavan lomakkeen käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  emailPlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.Email.Placeholder',
    defaultMessage: 'Esim. osoite@organisaatio.fi'
  },
  prefixTitle:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Title',
    defaultMessage:'Maan suuntanumero'
  },
  prefixPlaceHolder:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage:'esim. +358'
  },
  prefixTooltip:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage:'esim. +358'
  }
})

const messages = defineMessages({
  urlCheckerTooltip:  {
    id: 'Containers.Channels.AddElectronicChannel.Step1.UrlChecker.Tooltip',
    defaultMessage: 'Syötä verkko-osoite, josta verkkoasiointikanava löytyy. Anna mahdollisimman tarkka verkko-osoite, josta verkkoasiointi avautuu, älä esimerkiksi organisaatiosi verkkoasioinnin etusivua. Kopioi ja liitä verkko-osoite ja  tarkista verkko-osoitteen toimivuus Testaa osoite -painikkeesta.'
  },
  areaInformationInfoText: {
    id: 'Containers.Services.AddService.Step1.AreaInformation.Type.Title',
    defaultMessage: 'Aluetieto täytetty organisaation mukaan. Voit muuttaa valintaa.'
  },
  label: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Title',
    defaultMessage: 'Kielet, joilla verkkosivu on saatavilla'
  },
  tooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Tooltip',
    defaultMessage: 'Valitse tähän ne kielet, joilla verkkosivu tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet.'
  },
  placeholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.LanguageProvided.Placeholder',
    defaultMessage: 'Kirjoita ja valitse listasta verkkosivun kielet.'
  }
})

export const phoneNumberMessages = defineMessages({
  title: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.Title',
    defaultMessage: 'Puhelinnumero'
  },
  tooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.Tooltip',
    defaultMessage: 'Mikäli verkkosivukanavan käyttöön on mahdollista saada tukea puhelimitse, kirjoita kenttään tukipalvelun puhelinnumero. Kirjoita puhelinnumero kansainvälisessä muodossa ilman välilyöntejä (esim. +358451234567). Älä kirjoita kenttään organisaation yleisiä puhelinnumeroita, esim. vaihteen numeroa. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  placeholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneNumber.PlaceHolder',
    defaultMessage: 'esim. +35845123467'
  },
  chargeTypeTitle: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneCost.Title',
    defaultMessage: 'Puhelun maksullisuus'
  },
  chargeTypeTooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneCost.Tootltip',
    defaultMessage: 'Ensimmäinen vaihtoehto tarkoittaa, että puhelu maksaa lankaliittymästä soitettaessa paikallisverkkomaksun (pvm), matkapuhelimesta soitettaessa matkapuhelinmaksun (mpm) tai ulkomailta soitettaessa ulkomaanpuhelumaksun.Valitse "täysin maksuton" vain, jos puhelu on soittajalle kokonaan maksuton. Jos puhelun maksu muodostuu muulla tavoin, valitse "muu maksu" ja kuvaa puhelun hintatiedot omassa kentässään.'
  },
  costDescriptionTitle: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Title',
    defaultMessage: 'Puhelun hintatiedot'
  },
  costDescriptionPlaceholder: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Placeholder',
    defaultMessage: 'esim. Pvm:n lisäksi jonotuksesta veloitetaan...'
  },
  costDescriptionTooltip: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhoneCostOtherDescription.Tooltip',
    defaultMessage: 'Kerro sanallisesti, mitä puhelinnumeroon soittaminen maksaa.'
  },
  infoTitle:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto'
  },
  infoTooltip:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Tooltip',
    defaultMessage: 'Voit tarvittaessa antaa puhelinnumerolle lisätiedon, jolla voit tarkentaa, mistä numerosta on kyse. Esimerkiksi vaihde tai asiakaspalvelu.'
  },
  infoPlaceholder:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.AdditionalInformation.Placeholder',
    defaultMessage:'esim. Vaihde'
  },
  prefixTitle:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Title',
    defaultMessage:'Maan suuntanumero'
  },
  prefixPlaceHolder:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Placeholder',
    defaultMessage:'esim. +358'
  },
  prefixTooltip:{
    id: 'Containers.Channels.AddWebPageChannel.Step1.PhonePrefix.Tooltip',
    defaultMessage:'esim. +358'
  },
  finnishServiceNumberName: {
    id: 'Containers.Channels.AddWebPageChannel.Step1.FinishServiceNumber.Name',
    defaultMessage: 'Suomalainen palvelunumero'
  }
})

const WebPageBasic = ({
  intl: { formatMessage },
  isCompareMode,
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
              placeholder={formatMessage(messages.placeholder)}
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
    </div>
  )
}

WebPageBasic.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates
)(WebPageBasic)
