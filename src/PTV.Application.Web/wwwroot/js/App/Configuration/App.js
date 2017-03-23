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
//import Helmet from 'react-helmet';
import React, {PropTypes, Component} from 'react';
//import RouterHandler from '../Components/RouterHandler.react';
//import mapDispatchToProps from '../../common/app/mapDispatchToProps';
// import mapStateToProps from './MapStateToProps';
import mapDispatchToProps from './MapDispatchToProps';
import {connect} from 'react-redux';
import PTVNavigation from 'Components/PTVNavigation';
import * as intlActions from 'Intl/Actions';
import { FormattedMessage, defineMessages, injectIntl } from 'react-intl'
import { getAppUrl } from './AppHelpers';
import '../styles/site.scss';
//import Perf from 'react-addons-perf';
import * as IntlSelectors from 'Intl/Selectors';
import * as AppSelectors from './selectors';
import { Link, browserHistory } from 'react-router';

//window.Perf = Perf;

const messages = defineMessages({
    feedbackTitle: {
        id: "Configuration.App.Feedback.Title",
        defaultMessage: "Palaute"
    },
    appNameTitle: {
        id: "Configuration.App.Name.Title",
        defaultMessage: "PALVELUTIETOVARANTO"
    },
    appVersionTitle: {
        id: "Configuration.App.Version.Title",
        defaultMessage: "Versio: {version}"
    },
    appUserNameTitle: {
        id: "Configuration.App.User.Name",
        defaultMessage: "Nimi: {name}"
    },
    appUserRoleTitle: {
        id: "Configuration.App.User.Role",
        defaultMessage: "Rooli: {role}"
    },
    appCompanyNameTitle: {
        id: "Configuration.App.Company.Name.Title",
        defaultMessage: "Väestörekisterikeskus"
    },
    appServiceNameTitle: {
        id: "Configuration.App.Service.Name.Title",
        defaultMessage: "Tietoa Palvelutietovarannosta"
    },
    appContentManagementServiceTitle: {
        id: "Configuration.App.Content.Management.Service.Title",
        defaultMessage: "Tämän sisällönhallintapalvelun teille tarjoaa"
    },
    privacyPolicy: {
        id: "Configuration.App.PrivacyPolicy.Title",
        defaultMessage: "Tietosuojaseloste"
    },
    appSourceCodeInfo: {
        id: "Configuration.App.SourceCode.LinkInfo",
        defaultMessage: "Suomi.fi-palvelutietovarannossa käytetään avoimen lähdekoodin ohjelmistoja."
    },
    appSourceCodeLink: {
        id: "Configuration.App.SourceCode.LinkTitle",
        defaultMessage: "Lisenssitiedot ja lähdekoodi"
    },
    menuItemFrontPage: {
        id: "Configuration.App.MenuItem.FrontPage",
        defaultMessage: "ETUSIVU"
    },
    menuItemService: {
        id: "Configuration.App.MenuItem.Service",
        defaultMessage: "PALVELUT"
    },
    menuItemChannels: {
        id: "Configuration.App.MenuItem.Channels",
        defaultMessage: "ASIOINTIKANAVAT"
    },
    menuItemRelations: {
        id: "Configuration.App.MenuItem.Relations",
        defaultMessage: "LIITOKSET"
    },
    menuItemManage: {
        id: "Configuration.App.MenuItem.Manage",
        defaultMessage: "HALLINTA"
    },
    languageSelectLogOff: {
        id: "Configuration.App.LanguageSelect.Item.LogOff",
        defaultMessage: "LOG OFF"
    },
    environmentIdentificatorTest: {
        id: "Configuration.App.EnvironmentIdentificator.Test",
        defaultMessage: "Testi"
    },
    environmentIdentificatorQa: {
        id: "Configuration.App.EnvironmentIdentificator.Qa",
        defaultMessage: "Hyväksymistestausympäristö"
    },
    environmentIdentificatorTrn: {
        id: "Configuration.App.EnvironmentIdentificator.Trn",
        defaultMessage: "Koulutus"
    },
    environmentIdentificatorProd: {
        id: "Configuration.App.EnvironmentIdentificator.Prod",
        defaultMessage: "Tuotanto"
    },
    environmentIdentificatorDev: {
        id: "Configuration.App.EnvironmentIdentificator.Dev",
        defaultMessage: "Development"
    },
});

const Footer = ({name, role, version}) => (
     <footer id="page-footer">
        <div className="container">
            <div className="row">
                <div className="col-xs-12 col-md-6 col-lg-4">
                    <ul className="link-list">
                        <li>
                            <p><FormattedMessage {...messages.appNameTitle} /></p>
                        </li>
                        <li>
                            <p><FormattedMessage {...messages.appContentManagementServiceTitle} />
                                <a target="_blank" href="http://www.vrk.fi"><FormattedMessage {...messages.appCompanyNameTitle} /></a>
                            </p>
                        </li>
                        <li>
                            <a target="_blank" href="https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/kayttoonotto/asiakas-ja-kayttajarekisterin-tietosuojaseloste/"><FormattedMessage {...messages.privacyPolicy}/></a>
                        </li>
                    </ul>
                </div>
                <div className="col-xs-12 col-md-6 col-lg-4">
                    <ul className="link-list">
                        <li>
                            <a target="_blank" href="https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2"><FormattedMessage {...messages.feedbackTitle}/></a>
                        </li>
                        <li>
                            <a target="_blank" href="https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/"><FormattedMessage {...messages.appServiceNameTitle }/></a>
                        </li>
                        <li>
                            <p><FormattedMessage {...messages.appSourceCodeInfo} />
                                <a target="_blank" href="https://github.com/vrk-kpa/ptv-releases"><FormattedMessage {...messages.appSourceCodeLink} /></a>
                            </p>
                        </li>
                    </ul>
                </div>
                <div className="col-xs-12 col-md-6 col-md-offset-6 col-lg-offset-0 col-lg-4">
                    <ul className="link-list">
                        <li>
                            <a target="_blank" href="http://beta.suomi.fi">beta.suomi.fi</a>
                        </li>
                    </ul>
                </div>
            </div>
            <div className="row">
                   <div className="user-info-development col-lg-12">
                    <p className="delimiter pipe">
                       <span><FormattedMessage {...messages.appUserNameTitle }  values = {{ name }} /></span>
                       <span><FormattedMessage {...messages.appUserRoleTitle } values = {{ role }} /></span>
                       <span><FormattedMessage {...messages.appVersionTitle } values = {{ version }} /></span>
                    </p>
                   </div>
            </div>
        </div>
    </footer>
)

const mapsStateToFooter = (state, ownProps) => {
    return {
        name: AppSelectors.getUserName(),
        role: AppSelectors.getUserRoleName(),
        version: AppSelectors.getApplicationVersion()
    }
}

const ConnectedFooter = connect(mapsStateToFooter)(Footer);

class LanguageButton extends Component {
    constructor(props) {
        super(props);
    }

    static propTypes = {
        language: PropTypes.string.isRequired,
        selectedLanguage: PropTypes.string.isRequired,
        onClick: PropTypes.func.isRequired
    };

    onClick = () => {
        this.props.onClick(this.props.language);
    }

    getSelectedLanguageClass = (selectedLanguage, languageButton) => (
        selectedLanguage == languageButton ? "selected" : ""
    );

    render() {
        const { selectedLanguage, language } = this.props;
        return (
            <li className={this.getSelectedLanguageClass(selectedLanguage, language)}><span><a lang={language} href="#" onClick={this.onClick}>{this.props.children}</a></span></li>
        );
    }
}

class App extends Component {
    static propTypes = {
        actions: PropTypes.object.isRequired,
    };

      constructor(props) {
        super(props);
    }

    onButtonClick = (value) => {
        this.props.actions.changeLanguage(value);
    }

    handleLogoClick = (event) => {
        event.preventDefault();
        browserHistory.push('/frontPage');
    }

    componentWillMount = () => {
        const { isTranslatedDataLoaded, selectedLanguage, location : { query : { noTranslation } } } = this.props;
        if (!isTranslatedDataLoaded) {
           this.props.actions.getMessages();
           this.props.actions.getTranslatedData();
        }
        if ( noTranslation ){
            //console.log('location', selectedLanguage, this.props.location.query, noTranslation);
            let newLanguage = selectedLanguage;
            switch (noTranslation.toLowerCase()){
                case "on":
                    newLanguage = "notranslation";
                    break;
                case "off":
                    newLanguage = "fi";
                    break;
            }
            if (newLanguage !== selectedLanguage){
                this.props.actions.changeLanguage(newLanguage);
            }
        }
    }


    languageItems = [
              {language: 'fi', name: "SUOMEKSI", internal: false},
              {language: 'sv', name: "PA SVENSKA", internal: false},
              {language: 'en', name: "IN ENGLISH", internal: true },
              {language: 'default', name: "DEFAULT", internal: true },
              {language: 'notranslation', name: "NO TRANSLATION", internal: true },
            ];

    renderLanguages = (languages) => {
        const { selectedLanguage } = this.props;
        return (
            <ul className="language-selection">
                {languages.map(l =>
                    l.internal && !isDevelopment ?
                      null :
                    <LanguageButton
                        key={l.language}
                        selectedLanguage={selectedLanguage}
                        language={l.language}
                        onClick={this.onButtonClick}>
                    {l.name}
                    </LanguageButton>
                )
                }
                <li><span><a href={getAppUrl() + "/logoff"}>{ this.props.intl.formatMessage(messages.languageSelectLogOff)}</a></span></li>
            </ul>
        );
    }

    getMenuItems = () => [
              {uid: 'frontPage', route: "/frontPage", routeText: this.props.intl.formatMessage(messages.menuItemFrontPage)},
              {uid: 'service', route: "/service", routeText: this.props.intl.formatMessage(messages.menuItemService)},
              {uid: 'channels', route: '/channels', routeText: this.props.intl.formatMessage(messages.menuItemChannels)},
              {uid: 'relations', route: '/relations', routeText: this.props.intl.formatMessage(messages.menuItemRelations)},
              {uid: 'manage', route: "/manage", routeText: this.props.intl.formatMessage(messages.menuItemManage)},
              {uid: 'tool', class:'egg-hidden', route: "/tool", routeText: "  "}
            ];

    getSelectedMenu = () => {
        return this.getMenuItems().find(button => this.props.location.pathname.substring(0, button.route.length) === button.route)
    }

    getEnviromentName = () => {
        let envType = getEnvironmentType() || null;

        switch(envType) {
            case 'prod':
                return this.props.intl.formatMessage(messages.environmentIdentificatorProd);
            case 'qa':
                return this.props.intl.formatMessage(messages.environmentIdentificatorQa);
            case 'test':
                return this.props.intl.formatMessage(messages.environmentIdentificatorTest);
            case 'trn':
                return this.props.intl.formatMessage(messages.environmentIdentificatorTrn);
            case 'dev':
                return this.props.intl.formatMessage(messages.environmentIdentificatorDev);
            default:
                return '';
        }
    }

    render() {

        const selectedId = this.getSelectedMenu();
        let environmentName = this.getEnviromentName();
        const { formatMessage } = this.props.intl;
        return (

        <div>
        <header id="page-header">
            <div id="site-options">
                <div className="container">

                    { environmentName ?
                        <div className="header-center">
                            <div className="header-center-text">
                                { environmentName }
                            </div>
                        </div>
                    : null }

                    <div className="clearfix">
                        <div className="logo-placeholder">
                            <Link to='/frontPage' onClick={this.handleLogoClick}>
                                <img viewBox="0 0 300 47" height="100%" width="100%" src="../../../images/logo.svg" />
                            </Link>
                        </div>


                        <div className="header-languages">
                            {this.renderLanguages(this.languageItems)}
                        </div>
                    </div>

                </div>
            </div>
        </header>
    <nav>
        </nav>
    <div className="container">
        {this.props.children}
    </div>
        <ConnectedFooter />
    </div>
      );
    }

}

const actions = [
    intlActions,
];

const mapStateToProps = (state, ownProps) => {
    return {
      selectedLanguage: IntlSelectors.getSelectedLanguage(state),
      isTranslatedDataLoaded: IntlSelectors.getIsLocalizationMessagesLoaded(state),
    }
}

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(App));
