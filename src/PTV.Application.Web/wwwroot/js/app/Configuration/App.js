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
// import Helmet from 'react-helmet';
import React, { PropTypes, Component } from 'react'
// import RouterHandler from '../Components/RouterHandler.react';
// import mapDispatchToProps from '../../common/app/mapDispatchToProps';
// import mapStateToProps from './MapStateToProps';
import mapDispatchToProps from './MapDispatchToProps'
import { connect } from 'react-redux'
import * as intlActions from 'Intl/Actions'
import { FormattedMessage, defineMessages, injectIntl } from 'react-intl'
import { getAppUrl } from './AppHelpers'
import '../styles/site.scss'
// import Perf from 'react-addons-perf';
import * as IntlSelectors from 'Intl/Selectors'
import {
  getUserName,
  getUserRoleName,
  getApplicationVersion
} from 'selectors/userInfo'
import { Link, browserHistory } from 'react-router'
import { compose } from 'redux'
import ServerErrorDialog from 'appComponents/ServerErrorDialog'
import { getEnumTypesIsFetching } from 'selectors/base'
import { PTVPreloader } from 'Components'
import styles from './styles.scss'
import cx from 'classnames'
import { Alert } from 'sema-ui-components'
// window.Perf = Perf;

const messages = defineMessages({
  feedbackTitle: {
    id: 'Configuration.App.Feedback.Title',
    defaultMessage: 'Palaute'
  },
  feedbackLink: {
    id: 'Configuration.App.Feedback.Link',
    defaultMessage: 'https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2'
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
  appCompanyNameTitle: {
    id: 'Configuration.App.Company.Name.Title',
    defaultMessage: 'Väestörekisterikeskus'
  },
  appCompanyNameLink: {
    id: 'Configuration.App.Company.Name.Link',
    defaultMessage: 'http://www.vrk.fi'
  },
  appServiceNameTitle: {
    id: 'Configuration.App.Service.Name.Title',
    defaultMessage: 'Tietoa Palvelutietovarannosta'
  },
  appAboutLink: {
    id: 'Configuration.App.Service.Name.Link',
    defaultMessage: 'https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/'
  },
  appContentManagementServiceTitle: {
    id: 'Configuration.App.Content.Management.Service.Title',
    defaultMessage: 'Tämän sisällönhallintapalvelun teille tarjoaa'
  },
  privacyPolicy: {
    id: 'Configuration.App.PrivacyPolicy.Title',
    defaultMessage: 'Tietosuojaseloste'
  },
  privacyPolicyLink: {
    id: 'Configuration.App.PrivacyPolicy.Link',
    defaultMessage: 'https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/kayttoonotto/asiakas-ja-kayttajarekisterin-tietosuojaseloste/'
  },
  appSourceCodeInfo: {
    id: 'Configuration.App.SourceCode.LinkInfo',
    defaultMessage: 'Suomi.fi-palvelutietovarannossa käytetään avoimen lähdekoodin ohjelmistoja.'
  },
  appSourceCodeTitle: {
    id: 'Configuration.App.SourceCode.LinkTitle',
    defaultMessage: 'Lisenssitiedot ja lähdekoodi'
  },
  appSourceCodeLink: {
    id: 'Configuration.App.SourceCode.Link',
    defaultMessage: 'https://github.com/vrk-kpa/ptv-releases'
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
  languageSelectLogOff: {
    id: 'Configuration.App.LanguageSelect.Item.LogOff',
    defaultMessage: 'LOG OFF'
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
  currentIssuesInfo: {
    id: 'Configuration.App.CurrentIssuesInfo.Link',
    defaultMessage: 'Tiedotteet'
  },
  userNoAccessMessage: {
    id: 'Configuration.App.UserNoAccess.Message',
    defaultMessage: 'User has no access to application! Check access rights and assigned organization.'
  }
})

const roleNames = defineMessages({
  eeva : {
    id: 'Common.Role.Name.Eeva',
    defaultMessage: 'järjestelmäpääkäyttäjä'
  },
  pete : {
    id: 'Common.Role.Name.Pete',
    defaultMessage: 'pääkäyttäjä'
  },
  shirley : {
    id: 'Common.Role.Name.Shirley',
    defaultMessage: 'ylläpitäjä'
  }
})

const Footer = ({ name, role, version, intl:{ formatMessage } }) => {
  const roleName = roleNames[role.toLowerCase()] || null
  return (
    <footer id='page-footer' className={styles.pageFoot}>
      <div className='container'>
        <div className='row'>
          <div className='col-md-12 col-lg-8'>
            <ul className='link-list'>
              <li>
                <p><FormattedMessage {...messages.appNameTitle} /></p>
              </li>
              <li>
                <p>
                  <span className='space after'>
                    <FormattedMessage {...messages.appContentManagementServiceTitle} />
                  </span>
                  <a target='_blank' className='inside-text'
                    href={formatMessage(messages.appCompanyNameLink)}>
                    <FormattedMessage {...messages.appCompanyNameTitle} />
                  </a>
                </p>
              </li>
              <li>
                <a target='_blank'
                  href={formatMessage(messages.privacyPolicyLink)}>
                  <FormattedMessage {...messages.privacyPolicy} />
                </a>
              </li>
            </ul>
          </div>
          <div className='col-md-12 col-lg-8'>
            <ul className='link-list'>
              <li>
                <a target='_blank'
                  href={formatMessage(messages.feedbackLink)}>
                  <FormattedMessage {...messages.feedbackTitle} />
                </a>
              </li>
              <li>
                <a target='_blank'
                  href={formatMessage(messages.appAboutLink)}>
                  <FormattedMessage {...messages.appServiceNameTitle} />
                </a>
              </li>
              <li>
                <p>
                  <span className='space after'>
                    <FormattedMessage {...messages.appSourceCodeInfo} />
                  </span>
                  <a target='_blank' className='inside-text'
                    href={formatMessage(messages.appSourceCodeLink)}>
                    <FormattedMessage {...messages.appSourceCodeTitle} />
                  </a>
                </p>
              </li>
            </ul>
          </div>
          <div className='col-md-12 col-md-offset-12 col-lg-offset-0 col-lg-8'>
            <ul className='link-list'>
              <li>
                <a target='_blank' href={formatMessage(messages.suomiFiLink)}>{formatMessage(messages.suomiFiTitle)}</a>
              </li>
              <li>
                <Link to='/currentIssues'>{formatMessage(messages.currentIssuesInfo)}</Link>
              </li>
            </ul>
          </div>
        </div>
        <div className='row'>
          <div className='col-24'>
            <p className='delimited-list'>
              <span><FormattedMessage {...messages.appUserNameTitle} values={{ name }} /></span>
              <span><FormattedMessage {...messages.appUserRoleTitle} values={{ role: (roleName && formatMessage(roleName)) || role }} /></span>
              <span><FormattedMessage {...messages.appVersionTitle} values={{ version }} /></span>
            </p>
          </div>
        </div>
      </div>
    </footer>
  )
}

const mapsStateToFooter = (state, ownProps) => {
  return {
    name: getUserName(),
    role: getUserRoleName(),
    version: getApplicationVersion()
  }
}

const ConnectedFooter = connect(mapsStateToFooter)(injectIntl(Footer))

class LanguageButton extends Component {
  static propTypes = {
    language: PropTypes.string.isRequired,
    cultureInfo: PropTypes.string,
    selectedLanguage: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired
  };

  onClick = () => {
        // this.props.onClick(this.props.language);
    this.props.onClick(this.props.cultureInfo, this.props.language)
  }

  getSelectedLanguageClass = (selectedLanguage, languageButton) => {
    return (
        selectedLanguage == languageButton ? 'selected' : ''
    )
  };

  render () {
    const { selectedLanguage, language } = this.props
    return (
      <li className={this.getSelectedLanguageClass(selectedLanguage, language)}>
        <span>
          <a
            lang={language}
            href='javascript:;'
            onClick={this.onClick}
            children={this.props.children}
          />
        </span>
      </li>
    )
  }
}

class App extends Component {
  static propTypes = {
    actions: PropTypes.object.isRequired
  };

  constructor (props) {
    super(props)
  }

  onButtonClick = (culture, language) => {
    // if (culture) {
    //   cookie.save('.AspNetCore.Culture', culture)
    // }
    // if (language === 'default' || language === 'notranslation') {
    //   cookie.remove('.AspNetCore.Culture')
    // }
    this.props.actions.changeLanguage(language)
  }

  handleLogoClick = (event) => {
    event.preventDefault()
    browserHistory.push('/frontPage')
  }

  componentWillMount = () => {
    const { isTranslatedDataLoaded, selectedLanguage, location : { query : { noTranslation } } } = this.props
    if (!isTranslatedDataLoaded) {
      this.props.actions.getMessages()
      this.props.actions.getTranslatedData()
    }
    if (noTranslation) {
            // console.log('location', selectedLanguage, this.props.location.query, noTranslation);
      let newLanguage = selectedLanguage
      switch (noTranslation.toLowerCase()) {
        case 'on':
          newLanguage = 'notranslation'
          break
        case 'off':
          newLanguage = 'fi'
          break
      }
      if (newLanguage !== selectedLanguage) {
        this.props.actions.changeLanguage(newLanguage)
      }
    } else {
      this.props.actions.changeLanguage(selectedLanguage)
    }
  }
  enviroment = getEnvironmentType()
  isDev = this.enviroment === 'dev'
  isTesting = this.enviroment === 'dev' || this.enviroment === 'test' || this.enviroment === 'qa'
  languageItems = [
              { language: 'fi', name: 'SUOMEKSI', cultureInfo: 'c=fi-FI|uic=fi-FI', isVisible: true },
              { language: 'sv', name: 'PÅ SVENSKA', cultureInfo: 'c=sv-SE|uic=sv-SE', isVisible: true },
              { language: 'en', name: 'IN ENGLISH', cultureInfo: 'c=en-US|uic=en-US', isVisible: true },
              { language: 'default', name: 'DEFAULT', cultureInfo: 'c=fi-FI|uic=fi-FI', isVisible: this.isDev },
              { language: 'notranslation', name: 'NO TRANSLATION', cultureInfo: 'c=fi-FI|uic=fi-FI', isVisible: this.isDev }
  ];

  renderLanguages = (languages) => {
    const { selectedLanguage } = this.props
    return (
      <ul className='flex delimited-list'>
        {languages.map(l =>
                    l.isVisible &&
                    <LanguageButton
                      key={l.language}
                      selectedLanguage={selectedLanguage}
                      language={l.language}
                      cultureInfo={l.cultureInfo}
                      onClick={this.onButtonClick}>
                      {l.name}
                    </LanguageButton>
                )
                }
        <li><span><a href={getAppUrl() + '/logoff'}>{ this.props.intl.formatMessage(messages.languageSelectLogOff)}</a></span></li>
      </ul>
    )
  }

  getMenuItems = () => [
              { uid: 'frontPage', route: '/frontPage', routeText: this.props.intl.formatMessage(messages.menuItemFrontPage) },
              { uid: 'service', route: '/service', routeText: this.props.intl.formatMessage(messages.menuItemService) },
              { uid: 'channels', route: '/channels', routeText: this.props.intl.formatMessage(messages.menuItemChannels) },
              { uid: 'relations', route: '/relations', routeText: this.props.intl.formatMessage(messages.menuItemRelations) },
              { uid: 'manage', route: '/manage', routeText: this.props.intl.formatMessage(messages.menuItemManage) },
              { uid: 'tool', class:'egg-hidden', route: '/tool', routeText: '  ' }
  ];

  getSelectedMenu = () => {
    return this.getMenuItems().find(button => this.props.location.pathname.substring(0, button.route.length) === button.route)
  }

  getEnviromentName = () => {
    let envType = getEnvironmentType() || null

    switch (envType) {
      case 'prod':
        return this.props.intl.formatMessage(messages.environmentIdentificatorProd)
      case 'qa':
        return this.props.intl.formatMessage(messages.environmentIdentificatorQa)
      case 'test':
        return this.props.intl.formatMessage(messages.environmentIdentificatorTest)
      case 'trn':
        return this.props.intl.formatMessage(messages.environmentIdentificatorTrn)
      case 'dev':
        return this.props.intl.formatMessage(messages.environmentIdentificatorDev)
      default:
        return ''
    }
  }

  render () {
    const selectedId = this.getSelectedMenu()
    let environmentName = this.getEnviromentName()
    const { formatMessage } = this.props.intl
    return (

      <div className={styles.pageWrap}>
        <header id='page-header' className={styles.pageHead}>
          <div id='site-options'>
            <div className='container'>

              { environmentName
                        ? <div className='environment-wrapper'>
                          <div className='environment-content'>
                            { environmentName }
                          </div>
                        </div>
                    : null }

              <div className='flex space-between'>
                <div className='logo-placeholder'>
                  <Link to='/frontPage' onClick={this.handleLogoClick}>
                        <img viewBox='0 0 300 47' height='100%' width='100%' src='../../../images/logo.svg' />
                      </Link>
                </div>

                <div className='header-languages'>
                  {this.renderLanguages(this.languageItems)}
                </div>
              </div>

            </div>
          </div>
        </header>
        { NoAccess
        ? <div className={cx('container', styles.pageBody)}>
          <span className='space after'>
            <Alert info><FormattedMessage {...messages.userNoAccessMessage} /></Alert>
          </span>
        </div>
        : <div className={cx('container', styles.pageBody)}>
          {this.props.isLoading && <PTVPreloader /> ||
          <div>
            <ServerErrorDialog />
            {this.props.children}
          </div>}
        </div>}
        <ConnectedFooter />
      </div>
    )
  }

}

const actions = [
  intlActions
]

const mapStateToProps = (state, ownProps) => {
  // const languageFromCookie = currentCulture ? /uic=([a-z]{2})-[A-Z]{2}/.exec(currentCulture)[1] : null
  const selectedLanguage = IntlSelectors.getSelectedLanguage(state)
  // const selectedLanguage = languageFromCookie || IntlSelectors.getSelectedLanguage(state) || 'fi'
  return {
    isLoading: getEnumTypesIsFetching(state),
    selectedLanguage,
    isTranslatedDataLoaded: IntlSelectors.getIsLocalizationMessagesLoaded(state)
  }
}

export default compose(
  injectIntl,
  connect(mapStateToProps, mapDispatchToProps(actions)))(App)
