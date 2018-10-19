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
import React, { Component, Fragment } from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import * as intlActions from 'Intl/Actions'
import { injectIntl, intlShape } from 'util/react-intl'
import 'styles/site.scss'
import * as IntlSelectors from 'Intl/Selectors'
import { Route, Switch, Redirect } from 'react-router-dom'
import { compose } from 'redux'
import ServerErrorDialog from 'appComponents/ServerErrorDialog'
import { getEnumTypesIsFetching } from 'selectors/base'
import { getUserInfoIsFetching } from 'selectors/userInfo'
import styles from './styles.scss'
import cx from 'classnames'
import Preloader from 'appComponents/Preloader'
import Footer from './Footer'
import Header from './Header'
import Login from 'Routes/Login'
import AuthorizedRoute from 'Configuration/AuthorizedRoute'
import ServiceForm from 'Routes/Service'
import Logout from 'Routes/Logout/components/LogoutForm'
import FrontPage from 'Routes/FrontPage'
import Channels from 'Routes/Channels'
import GeneralDescription from 'Routes/GeneralDescription'
import Organization from 'Routes/Organization'
import CurrentIssues from 'Routes/CurrentIssues'
import { loadEnums } from 'actions/init'
import messages from './messages'
import withInit from 'util/redux-form/HOC/withInit/withInit'
import { withRouter } from 'react-router'
import { hot } from 'react-hot-loader'
import { getLogoAddress } from 'Configuration/AppHelpers'
import queryString from 'query-string'
import ServiceCollectionForm from 'Routes/ServiceCollection'
import withMassToolForm from 'util/redux-form/HOC/withMassToolForm'
import withScrollToBottom from 'util/redux-form/HOC/withScrollToBottom'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import withNotification from 'util/redux-form/HOC/withNotification'
import { formTypesEnum } from 'enums'

const logoAddress = getLogoAddress()

const ContentComponent = compose(
  withScrollToBottom({
    uiKey: 'pageBody'
    // showScrollIndicator: true
  })
)(props => {
  return (
    <Fragment>
      <ServerErrorDialog />
      <Switch>
        {!logoAddress && <Route path='/login' component={Login} />}
        <Route path='/logout' component={Logout} />
        <AuthorizedRoute path='/service' component={ServiceForm} />
        <AuthorizedRoute path='/channels' component={Channels} />
        <AuthorizedRoute path='/generalDescription' component={GeneralDescription} />
        <AuthorizedRoute path='/organization' component={Organization} />
        <AuthorizedRoute path='/currentIssues' component={CurrentIssues} />
        <AuthorizedRoute path='/frontpage' component={FrontPage} />
        <AuthorizedRoute path='/serviceCollection' component={ServiceCollectionForm} />
        <Redirect to='/frontpage' />
      </Switch>
    </Fragment>
  )
})

const InnerContent = compose(
  withMassToolForm
)(({
  forwardedRef,
  isLoading,
  ...rest
}) => {
  return (
    isLoading && <Preloader className={styles.contentPreloader} /> ||
    <div className={styles.pageBody} ref={forwardedRef}>
      <div className='container'>
        <ContentComponent
          {...rest}
          forwardedRef={forwardedRef}
        />
      </div>
    </div>
  )
})

class ContentComponentWrap extends Component {
  constructor (props) {
    super(props)
    this.pageBodyRef = React.createRef()
  }

  render () {
    return (
      <InnerContent forwardedRef={this.pageBodyRef} {...this.props} />
    )
  }
}

ContentComponentWrap.propTypes = {
  isLoading: PropTypes.bool
}

const Content = compose(
  withRouter,
  withInit({
    init: loadEnums
  }),
  connect(
    state => ({
      isLoading: getEnumTypesIsFetching(state) || getUserInfoIsFetching(state)
    })
  ),
  withNotification({
    formName: formTypesEnum.MASSTOOLFORM
  })
)(ContentComponentWrap)

class App extends Component {
  static propTypes = {
    changeLanguage: PropTypes.func.isRequired,
    intl: intlShape.isRequired,
    selectedLanguage: PropTypes.string,
    history: PropTypes.object,
    location: PropTypes.object.isRequired,
    isLoading: PropTypes.bool,
    isInReview: PropTypes.bool
  };

  handleOnLanguageChange = ({ value }) => {
    this.props.changeLanguage(value)
  }

  componentWillMount = () => {
    const { selectedLanguage, location } = this.props

    const parsedQueryString = queryString.parse(location.search)
    const notranslation = parsedQueryString.notranslation !== undefined
      ? parsedQueryString.notranslation
      : parsedQueryString.noTranslation
    if (notranslation !== undefined) {
      let newLanguage = selectedLanguage
      switch (notranslation && notranslation.toLowerCase()) {
        case 'nodots':
          newLanguage = 'notranslationdisplay'
          break
        case 'off':
          newLanguage = 'fi'
          break
        default:
          newLanguage = 'notranslation'
          break
      }
      if (newLanguage !== selectedLanguage) {
        this.props.changeLanguage(newLanguage)
      }
    }
  }

  enviroment = window.getEnvironmentType()
  isDev = this.enviroment === 'dev'
  isTesting = this.enviroment === 'dev' || this.enviroment === 'test' || this.enviroment === 'qa'
  languageItems = [
    { value: 'fi', label: 'SUOMEKSI', isVisible: true },
    { value: 'sv', label: 'PÅ SVENSKA', isVisible: true },
    { value: 'en', label: 'IN ENGLISH', isVisible: true },
    { value: 'default', label: 'DEFAULT', isVisible: this.isDev },
    { value: 'notranslation', label: 'NO TRANSLATION', isVisible: this.isDev },
    { value: 'notranslationdisplay', label: 'NO TRANSLATION (without dots)', isVisible: this.isDev }
  ];
  languageOptions = this.languageItems.filter(lang => lang.isVisible)

  getEnviromentName = () => {
    let envType = this.enviroment || null

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
    let environmentName = this.getEnviromentName()
    const {
      selectedLanguage,
      history,
      isLoading,
      isInReview
    } = this.props
    // const pathName = history && history.location && history.location.pathname
    const pageWrapClass = cx(
      styles.pageWrap,
      {
        [styles.isLoading]: isLoading,
        [styles.isInReview]: isInReview
      }
    )
    return (
      <div className={pageWrapClass}>
        <div className={styles.environmentLabel}>{ environmentName }</div>
        <Header
          languageOptions={this.languageOptions}
          selectedLanguage={selectedLanguage}
          onLanguageChange={this.handleOnLanguageChange}
          history={history}
          includeLanguageSwitcher
        />
        <div className={styles.content}><Content /></div>
        {!isInReview && <Footer
          languageOptions={this.languageOptions}
          onLanguageChange={this.handleOnLanguageChange}
          selectedLanguage={selectedLanguage}
          // pathName={pathName}
        />}
      </div>
    )
  }
}

export default compose(
  hot(module),
  injectIntl,
  connect((state, ownProps) => {
    // const languageFromCookie = currentCulture ? /uic=([a-z]{2})-[A-Z]{2}/.exec(currentCulture)[1] : null
    const selectedLanguage = IntlSelectors.getSelectedLanguage(state)
    // const selectedLanguage = languageFromCookie || IntlSelectors.getSelectedLanguage(state) || 'fi'
    return {
      selectedLanguage,
      isTranslatedDataLoaded: IntlSelectors.getIsLocalizationMessagesLoaded(state),
      isLoading: getEnumTypesIsFetching(state) || getUserInfoIsFetching(state),
      isInReview: getShowReviewBar(state)
    }
  }, {
    ...intlActions,
    loadEnums
  })
)(App)
