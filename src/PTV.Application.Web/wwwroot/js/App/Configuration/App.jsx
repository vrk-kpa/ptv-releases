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
import PTVNavigation from '../Components/PTVNavigation';
import * as intlActions from '../Intl/Actions';
import { FormattedMessage, defineMessages, injectIntl } from 'react-intl'
import { getAppUrl } from './AppHelpers';
import '../styles/site.scss';
import { FormattedNumber, FormattedDate, IntlMixin } from 'react-intl';
//import Perf from 'react-addons-perf';
import * as IntlSelectors from '../Intl/Selectors';
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
});

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
        this.refs.headerNavigation.setActiveMenuItem('frontPage');
        browserHistory.push('/frontPage');
    }

    componentWillMount = () => {
        const { isTranslatedDataLoaded, selectedLanguage, location : { query : { noTranslation } } } = this.props;
        if (!isTranslatedDataLoaded) {
           this.props.actions.getTranslatedData();
        }
        if ( noTranslation ){
            //console.log('location', selectedLanguage, this.props.location.query, noTranslation);
            let newLanguage = selectedLanguage;
            switch (noTranslation.toLowerCase()){
                case "on":
                    newLanguage = "noTranslation";
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
              {language: 'defaultTranslation', name: "DEFAULT", internal: true },
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
                <li><span><a href={getAppUrl() + "/logoff"}>LOG OFF</a></span></li>
            </ul>
        );
    }

    menuItems = [
              {uid: 'frontPage', route: "/frontPage", routeText: "ETUSIVU"},
              {uid: 'service', route: "/service", routeText: "PALVELUT"},
              {uid: 'channels', route: '/channels', routeText: 'ASIOINTIKANAVAT'},
              {uid: 'relations', route: '/relations', routeText: 'LIITOKSET'},
              {uid: 'manage', route: "/manage", routeText: "HALLINTA"},
              {uid: 'tool', class:'egg-hidden', route: "/tool", routeText: "  "}
            ];

    getSelectedMenu = () => {
        return this.menuItems.find(button => this.props.location.pathname.substring(0, button.route.length) === button.route)
    }

    getEnviromentName = () => {
        let envType = getEnvironmentType() || null;

        switch(envType) {
            case 'prod':
                return "Tuotanto";
            case 'qa':
                return "Hyväksymistestausympäristö";
            case 'test':
                return "Testi";
            case 'trn':
                return "Koulutus";
            case 'dev':
                return "Development";
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

        <nav>
         <PTVNavigation ref="headerNavigation" menuItems={ this.menuItems } subMenu= {this.props.subMenu} defaultMenuItemUid={ selectedId ? selectedId.uid : 'frontPage' }/>
        </nav>
        </header>
    <div className="container">
        {this.props.content}
    </div>
    <footer id="page-footer">
        <div className="container">
            <div className="row">
                <div className="col-xs-12 col-md-6 col-lg-4">
                    <p>{ formatMessage(messages.appNameTitle) }</p>
                    <p>{ formatMessage(messages.appContentManagementServiceTitle) } <a lang="fi" target="_blank" href="http://www.vrk.fi">{formatMessage(messages.appCompanyNameTitle)}</a></p>
                </div>
                <nav>
                    <div className="col-xs-12 col-md-6 col-lg-3">
                        <ul className="link-list">
                            <li>
                                <a lang="fi" target="_blank" href="https://response.questback.com/isa/qbv.dll/ShowQuest?QuestID=4857167&sid=8gLknyGeq2">{ formatMessage(messages.feedbackTitle) }</a>
                            </li>
                            <li>
                                <a lang="fi" target="_blank" href="https://esuomi.fi/palveluntarjoajille/palvelutietovaranto/">{ formatMessage(messages.appServiceNameTitle) }</a>
                            </li>
                        </ul>
                    </div>
                    <div className="col-xs-12 col-md-6 col-md-offset-6 col-lg-offset-0 col-lg-3">
                        <ul className="link-list">
                            <li>
                                <a lang="fi" target="_blank" href="http://beta.suomi.fi">beta.suomi.fi</a>
                            </li>
                        </ul>
                    </div>
                </nav>
            </div>
            <div className="row">
                   <div className="user-info-development col-lg-12">
                    <p>
                       Name: { getUserFirstNameSurname() }, Role: { getUserRole()}, Version: { getAppVersion() }
                    </p>
                   </div>
            </div>
        </div>
    </footer>
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
      isTranslatedDataLoaded: IntlSelectors.getIsTranslatedDataLoaded(state)
    }
}

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(App));
