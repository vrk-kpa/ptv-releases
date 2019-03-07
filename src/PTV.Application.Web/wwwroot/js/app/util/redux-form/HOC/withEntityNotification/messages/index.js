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
import { defineMessages } from 'util/react-intl'

export default defineMessages({
  removeConnectionWarningTitle: {
    id: 'WithEntityNotification.RemovedChannelWarning.Title',
    defaultMessage: 'Alla olevat kanavat on poistettu palvelun liitoksista, koska niiden omistajaorganisaatiot ovat muuttaneet kanavien yhteiskäyttöisyyttä. Liitä palveluusi muu vaihtoehtoinen kanava.', // eslint-disable-line
    description: 'Util.ReduxForm.HOC.WithRemoveConnectionWarning.WarningMessage'
  },
  organizationLanguageWarningTitle: {
    id: 'WithEntityNotification.OrganizationLanguageWarning.Title',
    defaultMessage: 'Organisaatiokuvauksen kieliversioita puuttuu; {languages}. Lisää tarvittavat kieliversiot.',
    description: 'Util.ReduxForm.HOC.WithOrganizationLanguageWarning.WarningMessage'
  },
  serviceExpireTitle: {
    id: 'WithEntityNotification.ExpireWarning.Service.Title',
    defaultMessage: 'Palvelu vanhenee {expireOn}. Tarkista, että sisältö on ajantasalla, ja sen jälkeen julkaise palvelun uudelleen, jotta se näkyy käyttäjille jatkossakin.', // eslint-disable-line
    description: 'Util.ReduxForm.HOC.WithEntityExpireWarning.WarningMessage'
  },
  channelExpireTitle: {
    id: 'WithEntityNotification.ExpireWarning.Channel.Title',
    defaultMessage: 'Asiointikanava vanhenee {expireOn}. Tarkista, että sisältö on ajantasalla, ja sen jälkeen julkaise palvelun uudelleen, jotta se näkyy käyttäjille jatkossakin.', // eslint-disable-line
    description: 'Util.ReduxForm.HOC.WithEntityExpireWarning.ChannelWarningMessage'
  },
  timedPublishTitle: {
    id: 'WithEntityNotification.TimedPublishInfo.Title',
    defaultMessage: 'Ajastettu julkaistavaksi'
  },
  serviceTimedPublishDescription: {
    id: 'WithEntityNotification.TimedPublishInfo.Service.Description',
    defaultMessage: 'Tämä palvelu on ajastettu julkaistavaksi {publishOn}'
  },
  channelTimedPublishDescription: {
    id: 'WithEntityNotification.TimedPublishInfo.Channel.Description',
    defaultMessage: 'Tämä asiointikanava on ajastettu julkaistavaksi {publishOn}'
  },
  organizationTimedPublishDescription: {
    id: 'WithEntityNotification.TimedPublishInfo.Organization.Description',
    defaultMessage: 'Tämä organisaatio on ajastettu julkaistavaksi {publishOn}'
  },
  generalDescriptionTimedPublishDescription: {
    id: 'WithEntityNotification.TimedPublishInfo.GeneralDescription.Description',
    defaultMessage: 'Tämä pohjakuvaus on ajastettu julkaistavaksi {publishOn}'
  },
  serviceCollectionTimedPublishDescription: {
    id: 'WithEntityNotification.TimedPublishInfo.ServiceCollection.Description',
    defaultMessage: 'Tämä palvelukokonaisuus on ajastettu julkaistavaksi {publishOn}'
  },
  closeLabel: {
    id: 'WithEntityNotification.CloseLabel.Title',
    defaultMessage: 'Sulje',
    description: 'AppComponents.PreviewDialog.Buttons.Close.Title'
  }
})
