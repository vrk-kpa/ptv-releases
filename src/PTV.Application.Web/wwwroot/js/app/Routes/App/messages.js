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
import { defineMessages } from 'util/react-intl'

export default defineMessages({
  feedbackTitle: {
    id: 'Configuration.App.Feedback.Title',
    defaultMessage: 'Palaute'
  },
  feedbackLink: {
    id: 'Configuration.App.Feedback.Link',
    defaultMessage: 'https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2'
  },
  questbackTitle: {
    id: 'Configuration.App.Questback.Title',
    defaultMessage: 'Ilmoita virheellinen osoite'
  },
  questbackLink: {
    id: 'Configuration.App.Questback.Link',
    defaultMessage: 'https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4952203&sid=mRluuB4V5f',
    description: {
      sv: 'https://response.questback.com/vestrekisterikeskus/vco2revkwg'
    }
  },
  appNameTitle: {
    id: 'Configuration.App.Name.Title',
    defaultMessage: 'PALVELUTIETOVARANTO'
  },
  appVersionTitle: {
    id: 'Configuration.App.Version.Title',
    defaultMessage: 'Versio: {version}'
  },
  appUserNameTitle: {
    id: 'Configuration.App.User.Name',
    defaultMessage: 'Nimi: {name}'
  },
  appUserRoleTitle: {
    id: 'Configuration.App.User.Role',
    defaultMessage: 'Rooli: {role}'
  },
  appServiceNameTitle: {
    id: 'Configuration.App.Service.Name.Title',
    defaultMessage: 'Tietoa Palvelutietovarannosta'
  },
  appAboutLink: {
    id: 'Configuration.App.Service.Name.Link',
    defaultMessage: 'https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/'
  },
  privacyPolicy: {
    id: 'Configuration.App.PrivacyPolicy.Title',
    defaultMessage: 'Tietosuojaseloste'
  },
  privacyPolicyLink: {
    id: 'Configuration.App.PrivacyPolicy.Link',
    defaultMessage: 'https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/kayttoonotto/asiakas-ja-kayttajarekisterin-tietosuojaseloste/' // eslint-disable-line
  },
  accessibilityStatementTitle: {
    id: 'Configuration.App.AccessibilityStatement.Title',
    defaultMessage: 'Accessibility Statement'
  },
  accessibilityStatementLink: {
    id: 'Configuration.App.AccessibilityStatement.Link',
    defaultMessage: '#'
  },
  menuItemFrontPage: {
    id: 'Configuration.App.MenuItem.FrontPage',
    defaultMessage: 'ETUSIVU'
  },
  menuItemService: {
    id: 'Configuration.App.MenuItem.Service',
    defaultMessage: 'PALVELUT'
  },
  menuItemChannels: {
    id: 'Configuration.App.MenuItem.Channels',
    defaultMessage: 'ASIOINTIKANAVAT'
  },
  menuItemRelations: {
    id: 'Configuration.App.MenuItem.Relations',
    defaultMessage: 'LIITOKSET'
  },
  menuItemManage: {
    id: 'Configuration.App.MenuItem.Manage',
    defaultMessage: 'HALLINTA'
  },
  menuLabel: {
    id: 'Configuration.App.Menu.Label',
    defaultMessage: 'Valikko',
    description: {
      fi: 'Valikko',
      sv: 'Meny',
      en: 'Menu'
    }
  },
  languageSelectLogOff: {
    id: 'Configuration.App.LanguageSelect.Item.LogOff',
    defaultMessage: 'Kirjaudu ulos Palvelutietovarannosta'
  },
  environmentIdentificatorTest: {
    id: 'Configuration.App.EnvironmentIdentificator.Test',
    defaultMessage: 'Testi'
  },
  environmentIdentificatorQa: {
    id: 'Configuration.App.EnvironmentIdentificator.Qa',
    defaultMessage: 'Hyväksymistestausympäristö'
  },
  environmentIdentificatorTrn: {
    id: 'Configuration.App.EnvironmentIdentificator.Trn',
    defaultMessage: 'Koulutus'
  },
  environmentIdentificatorProd: {
    id: 'Configuration.App.EnvironmentIdentificator.Prod',
    defaultMessage: 'Tuotanto'
  },
  environmentIdentificatorDev: {
    id: 'Configuration.App.EnvironmentIdentificator.Dev',
    defaultMessage: 'Development'
  },
  suomiFiTitle: {
    id: 'Configuration.App.SuomiFi.Title',
    defaultMessage: 'beta.suomi.fi'
  },
  suomiFiLink: {
    id: 'Configuration.App.SuomiFi.Link',
    defaultMessage: 'http://beta.suomi.fi'
  },
  userNoAccessMessage: {
    id: 'Configuration.App.UserNoAccess.Message',
    defaultMessage: 'User has no access to application! Check access rights and assigned organization.'
  },
  footerServiceDescription: {
    id: 'Configuration.App.Footer.ServiceDescription.Text',
    defaultMessage: 'Public services for your life events'
  },
  myProfileTitle: {
    id: 'Configuration.App.Header.MyProfile.Title',
    defaultMessage: 'Oma profiili'
  },
  myOrganizationTitle: {
    id: 'Configuration.App.Header.MyOrganization.Title',
    defaultMessage: 'Oma organisaatio'
  },
  userManagementTitle: {
    id: 'Configuration.App.Header.UserManagement.Title',
    defaultMessage: 'Käyttäjähallinta'
  },
  statisticsTitle: {
    id: 'Configuration.App.Header.Statistics.Title',
    defaultMessage: 'Tilastot'
  },
  supportTitle: {
    id: 'Configuration.App.Header.Support.Title',
    defaultMessage: 'Tuki'
  },
  aboutTitle: {
    id: 'Configuration.App.Header.About.Title',
    defaultMessage: 'Tietoa Suomi.fi-palveluhallinnasta'
  },
  preloaderLabel : {
    id: 'Configuration.App.Preloader.Label',
    defaultMessage: 'PTV tiedot latautuvat'
  },
  facebookLink: {
    id: 'Footer.Facebook.Link',
    defaultMessage: 'https://www.facebook.com/suomifi/'
  },
  twitterLink: {
    id: 'Footer.Twitter.Link',
    defaultMessage: 'https://twitter.com/suomifi'
  },
  youtubeLink: {
    id: 'Footer.YouTube.Link',
    defaultMessage: 'https://www.youtube.com/user/suomifitoimitus'
  },
  onlineCourseTitle: {
    id: 'Configuration.App.OnlineCourse.Title',
    defaultMessage: 'PTV-ajokortti'
  },
  onlineCourseLink: {
    id: 'Configuration.App.OnlineCourse.Link',
    defaultMessage: '#'
  }
})
