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
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { connect } from 'react-redux'
import {
  AreaInformation,
  EmailCollection
} from 'util/redux-form/sections'
import {
  Languages,
  NameEditor,
  Summary,
  UrlChecker,
  Description
} from 'util/redux-form/fields'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asLocalizableSection from 'util/redux-form/HOC/asLocalizableSection'
import asNotLocalized from 'util/redux-form/HOC/asNotLocalized'
import asSection from 'util/redux-form/HOC/asSection'
import { isRequired } from 'util/redux-form/validators'
import {
  PhoneNumberCollectionPhoneChannel as PhoneNumberCollection
} from 'util/redux-form/sections/PhoneNumberCollection/PhoneNumberCollection'
import { getDefaultPhoneNumber } from '../../../../selectors'
import { ChannelOrganization } from 'Routes/Channels/components'
import ChannelConnectionType from 'appComponents/ChannelConnectionType'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Title',
    defaultMessage: 'Kielet, joilla puhelinasiointi on saatavilla'
  },
  tooltip: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Tooltip',
    defaultMessage: 'Valitse tähän ne kielet, joilla puhelinasiointia tarjotaan asiakkaalle. Aloita kielen nimen kirjoittaminen, niin saat näkyviin kielilistan, josta voit valita kielet. '
  },
  placeholder: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.LanguageProvided.Placeholder',
    defaultMessage: 'Kirjoita ja valitse listasta puhelinasioinnin kielet.'
  },
  userSupportTitle: {
    id: 'Containers.Channels.AddElectronicChannel.Step1.Support.Title',
    defaultMessage: 'Käytön tuki'
  }
})

export const channelDescriptionMessages = defineMessages({
  nameTitle: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Name.Title',
    defaultMessage: 'Nimi'
  },
  namePlaceholder: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Name.Placeholder',
    defaultMessage: 'Kirjoita asiointikanavaa kuvaava, asiakaslähtöinen nimi.'
  },
  nameTooltip: {
    id: 'Containers.Channels.AddPhoneChannel.Name.Tooltip',
    defaultMessage: 'Phone channel name tooltip.'
  },
  organizationLabel: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Organization.Title',
    defaultMessage: 'Organisaatio'
  },
  organizationInfo: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Organization.Tooltip',
    defaultMessage: 'Valitse alasvetovalikosta organisaatio tai alaorganisaatio, joka vastaa puhelinasiointikanavasta. Mikäli puhelinasiointi on käytössä useilla alaorganisaatioilla, valitse pelkkä organisaation päätaso.'
  },
  shortDescriptionLabel: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Title',
    defaultMessage: 'Lyhyt kuvaus'
  },
  shortDescriptionInfo: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Tooltip',
    defaultMessage: 'Kirjoita lyhyt kuvaus eli tiivistelmä puhelinasiointikanavan keskeisestä sisällöstä. Lyhyt kuvaus esitetään hakutuloksissa, ja se auttaa hakukoneita löytämään asiointikanavan.'
  },
  shortDescriptionPlaceholder: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.ShortDescription.Placeholder',
    defaultMessage: 'Kirjoita lyhyt tiivistelmä hakukoneita varten.'
  },
  descriptionLabel: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Description.Title',
    defaultMessage: 'Kuvaus'
  },
  descriptionInfo: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Description.Tooltip',
    defaultMessage: 'Kuvaa puhelinasiointikanavan sisältö mahdollisimman selkeästi ja asiakkaan kannalta ymmärrettävästi. Kuvaa, mistä asiasta puhelinasiointikanava tarjoaa lisätietoa ja miten sitä voi käyttää. Käytä selkeää yleiskieltä, vältä hallinnollisia termejä. Kuvaa vain puhelinasiointia, älä palvelua järjestävää organisaatiota tai sen tehtäviä!'
  },
  descriptionPlaceholder: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Description.Placeholder',
    defaultMessage: 'Kirjoita selkeä ja ymmärrettävä kuvausteksti.'
  },
  emailTitle: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Email.Title',
    defaultMessage: 'Sähköposti'
  },
  emailTooltip: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Email.Tooltip',
    defaultMessage: 'Mikäli puhelinasioinnin käyttöön on mahdollista saada tukea sähköpostitse, kirjoita kenttään tukipalvelun sähköpostiosoite. Älä kirjoita kenttään organisaation yleisiä sähköpostiosoitteita, esim. kirjaamoon. Jos käyttäjätukea ei ole saatavissa, jätä kenttä tyhjäksi.'
  },
  emailPlaceholder: {
    id: 'Containers.Channels.AddPhoneChannel.Step1.Email.Placeholder',
    defaultMessage: 'esim. osoite@organisaatio.fi'
  }
})

const Url = compose(
  injectIntl,
  asLocalizableSection('webPage'),
  asSection(),
  asNotLocalized
)(UrlChecker)

const PhoneChannelBasic = ({
  intl: { formatMessage },
  isCompareMode,
  defaultPhoneNumberItem,
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
        <PhoneNumberCollection defaultItem={defaultPhoneNumberItem} required />
      </div>
      <div className='form-row'>
        <Url />
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
        <EmailCollection
          nested
          title={messages.userSupportTitle}
          emailProps={{
            label: formatMessage(channelDescriptionMessages.emailTitle),
            tooltip: formatMessage(channelDescriptionMessages.emailTooltip),
            placeholder: formatMessage(channelDescriptionMessages.emailPlaceholder)
          }}
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

PhoneChannelBasic.propTypes = {
  intl: intlShape,
  isCompareMode: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  connect(state => ({
    defaultPhoneNumberItem: getDefaultPhoneNumber(state)
  }))
)(PhoneChannelBasic)
