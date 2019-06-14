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
import { connect } from 'react-redux'
import { compose, bindActionCreators } from 'redux'
import { getAuthToken, getLogoAddress } from 'Configuration/AppHelpers'
import { formTypesEnum } from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import { mergeInUIState } from 'reducers/ui'
import { getShowReviewBar, getCurrentItemPath } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getFormName } from 'selectors/entities/entities'
import 'styles/site.scss'
import { SimpleHeader, Button } from 'sema-ui-components'
import withNavigationDialog from 'util/redux-form/HOC/withNavigationDialog'
import withReminderDialog from 'util/redux-form/HOC/withReminderDialog'
import { getHeaderUser, getHeaderOrganization, getIsMassSelectionInProgress } from './selectors'
import { getConnectionsMainEntity } from 'selectors/selections'
import cx from 'classnames'
import styles from './styles.scss'
import appMessages from './messages'
import { PTVIcon } from 'Components'
import { isDirty } from 'redux-form/immutable'
import { getIsReadOnlyEvenIfFormNotExist } from 'selectors/formStates'
import shortId from 'shortid'
import { Prompt, withRouter } from 'react-router'
import { applicationInit, deleteSearchResult } from 'actions/init'

const logoAddress = getLogoAddress()
const preventDialogSymbol = Symbol.for(`__PreventNavigationDialog_${shortId.generate()}`)

const shouldDialogBeOpened = (dispatch, path) => {
  let showDialog = false
  dispatch(({ getState }) => {
    const state = getState()
    const isInReview = getShowReviewBar(state)
    const mainConnectionEntity = getConnectionsMainEntity(state)
    const isConnectionsWorkbenchDirty = isDirty('connectionsWorkbench')(state)
    const isConnectionsWorkbenchTouched = mainConnectionEntity && isConnectionsWorkbenchDirty
    const formName = getFormName(state)
    const isReadOnly = formName && getIsReadOnlyEvenIfFormNotExist(state, { formName }) &&
    getIsReadOnlyEvenIfFormNotExist(state, { formName: formTypesEnum.CONNECTIONS })
    const isMassSelectionInProgress = getIsMassSelectionInProgress(state)
    const isAuthenticated = getAuthToken()
    showDialog = isAuthenticated && ((isInReview && getCurrentItemPath(state) !== path) ||
        isConnectionsWorkbenchTouched ||
        (isMassSelectionInProgress && (!!formName && !isReadOnly)) ||
        (!!formName && !isReadOnly))
  })

  return showDialog
}

class PreventNavigationDialogComponent extends React.Component {
  constructor () {
    super()
    this.__trigger = preventDialogSymbol
    this.customRedirectOutside = null
    this.customNavigatePath = null
    this.isLogOff = false
  }

  componentDidMount () {
    window[this.__trigger] = this.show
  }

  componentWillUnmount () {
    this.cleanVars()
    delete window[this.__trigger]
  }

  getUniqeId = (location, action) => {
    const { state, pathname, search } = location
    this.customNavigatePath = pathname + (search && search || '')
    if (state) {
      const { customRedirectOutside, isLogOff, shouldClean } = state
      this.customRedirectOutside = customRedirectOutside
      this.isLogOff = isLogOff || false
      this.shouldClean = shouldClean || false
    }
    return Symbol.keyFor(this.__trigger)
  }
  render () {
    const { when } = this.props
    return (
      <Prompt when={when} message={this.getUniqeId} />
    )
  }

  cleanVars = () => {
    this.customNavigatePath = null
    this.customRedirectOutside = null
    this.isLogOff = false
    this.shouldClean = false
  }

  cleanApp = (isLogOff) => {
    this.props.applicationInit(isLogOff)
  }

  isLeavingSearchPage = (history) => {
    const location = history && history.location
    const pathname = location && location.pathname
    return pathname && (pathname.toLowerCase().indexOf('frontpage/search') > -1)
  }

  show = allowTransitionCallback => {
    const shouldResetSearch = this.isLeavingSearchPage(this.props.history)
    if (shouldDialogBeOpened(this.props.dispatch, this.customNavigatePath)) {
      allowTransitionCallback(false)

      const value = {
        isOpen: true,
        navigateTo: this.customRedirectOutside || this.customNavigatePath || '/frontpage/search',
        external: !!this.customRedirectOutside
      }

      if (shouldResetSearch) {
        this.props.deleteSearchResult()
      }

      if (this.isLogOff || this.shouldClean) {
        value.enhancedConfirmAction = () => this.cleanApp(this.isLogOff)
      }

      this.props.mergeInUIState({
        key: 'navigationDialog',
        value: value
      })
    } else {
      if (shouldResetSearch) {
        this.props.deleteSearchResult()
      }

      if (this.isLogOff || this.shouldClean) {
        this.cleanApp(this.isLogOff)
      }

      if (this.customRedirectOutside) {
        window.location.href = this.customRedirectOutside
      } else {
        allowTransitionCallback(true)
      }
    }

    this.cleanVars()
  }
}

PreventNavigationDialogComponent.propTypes = {
  when: PropTypes.bool,
  mergeInUIState: PropTypes.func.isRequired,
  applicationInit: PropTypes.func.isRequired,
  dispatch: PropTypes.func.isRequired,
  history: PropTypes.object.isRequired,
  deleteSearchResult: PropTypes.func.isRequired
}

export const PreventNavigationDialog = compose(
  injectIntl,
  withRouter,
  connect(null,
    (dispatch) => {
      return {
        ...bindActionCreators({
          mergeInUIState,
          applicationInit,
          deleteSearchResult
        }, dispatch),
        dispatch
      }
    })
)(PreventNavigationDialogComponent)

const UserInfo = ({ user, org, children }) => (
  <div>
    {user && <div className={styles.menuUser}>{user.firstName} {user.lastName}</div>}
    {org && <div className={styles.menuOrganization}>{org.organizationName}</div>}
    {children}
  </div>
)

UserInfo.propTypes = {
  user: PropTypes.object,
  org: PropTypes.object,
  children: PropTypes.any
}

const MenuLink = compose(
)(({
  href,
  text,
  history
}) => {
  const handleClick = e => {
    e.preventDefault()
    history.push({ state: { customRedirectOutside: href } })
  }
  return (
    <a className={styles.menuLink} href={href} onClick={handleClick}>
      {text}
    </a>
  )
})

MenuLink.propTypes = {
  href: PropTypes.string,
  text: PropTypes.string
}

const MenuLinks = ({ links, ...rest }) => {
  return links.map((link, index) => (
    <MenuLink
      key={index}
      href={link.href}
      text={link.linkText}
      {...rest}
    />
  ))
}

const LogoContent = ({ onClick }) => (
  <div className={styles.logo} onClick={onClick}>
    <div className={styles.logoSymbol}>
      <img viewBox='0 0 32 32' height='32' width='32' src='../../../images/suomifi-symbol.svg' />
    </div>
    <div className={styles.logoText}>
      <img viewBox='0 0 255 40' height='32' width='121' src='../../../images/logo.svg' />
    </div>
  </div>
)
LogoContent.propTypes = {
  onClick: PropTypes.func.isRequired
}

const Logo = compose(
)(({
  history
}) => {
  const onLogoClick = () => {
    history.push({
      pathname: !logoAddress && '/frontpage' || null,
      state: {
        shouldClean: true,
        customRedirectOutside: logoAddress || null
      }
    })
  }
  return <LogoContent onClick={onLogoClick} />
})

const Header = ({
  isAuthenticated,
  user,
  activeOrganization,
  intl:{ formatMessage },
  history,
  hidden,
  ...rest
}) => {
  const menuLinks = window.getMenuLinks() || {}
  const language = 'fi' // currently hardcoded as PAHA does not support other languages
  const userLinks = [
    {
      linkText: formatMessage(appMessages.myProfileTitle),
      href: formatMessage({ id: 'MenuLink.OwnProfile', defaultMessage: menuLinks.ownProfile }, { language })
    },
    {
      linkText: formatMessage(appMessages.myOrganizationTitle),
      href: formatMessage({ id: 'MenuLink.OwnOrganization', defaultMessage: menuLinks.ownOrganization }, { language })
    },
    {
      linkText: formatMessage(appMessages.userManagementTitle),
      href: formatMessage({ id: 'MenuLink.Administration', defaultMessage: menuLinks.administration }, { language })
    }
  ]
  const otherLinks = [
    {
      linkText: formatMessage(appMessages.statisticsTitle),
      href: formatMessage({ id: 'MenuLink.Statistics', defaultMessage: menuLinks.statistics }, { language })
    },
    {
      linkText: formatMessage(appMessages.supportTitle),
      href: formatMessage({ id: 'MenuLink.Support', defaultMessage: menuLinks.support }, { language })
    },
    {
      linkText: formatMessage(appMessages.aboutTitle),
      href: formatMessage(
        { id: 'MenuLink.ServiceManagement', defaultMessage: menuLinks.serviceManagement }, { language }
      )
    }
  ]
  const menuItems = [
    <UserInfo user={user} org={activeOrganization}>
      <Button className={styles.logOut} link small onClick={(e) => handleLogOffOnClick(e)}>
        <PTVIcon name='icon-signOut' height={16} onClick={() => {}} componentClass={styles.logOutIcon} />
        {formatMessage(appMessages.languageSelectLogOff)}
      </Button>
    </UserInfo>,
    <MenuLinks links={userLinks} history={history} />,
    <MenuLinks links={otherLinks} history={history} />
  ]

  const handleLogOffOnClick = (event) => {
    event.preventDefault()

    history.push(logoAddress && { state: { customRedirectOutside: logoAddress, isLogOff: true } } ||
      { pathname: '/login', state: { isLogOff: true } })
  }

  const headerClass = cx({
    [styles.notLoggedIn]: !isAuthenticated
  })

  return (
    <div>
      <PreventNavigationDialog when />
      {!hidden && <SimpleHeader
        menuItems={menuItems}
        logo={<Logo history={history} />}
        user={user}
        activeOrganization={activeOrganization}
        isAuthenticated={!!isAuthenticated}
        className={headerClass}
        {...rest}
      />}
    </div>
  )
}

Header.propTypes = {
  isAuthenticated: PropTypes.any,
  user: PropTypes.object,
  activeOrganization: PropTypes.object,
  intl: intlShape.isRequired,
  history: PropTypes.object.isRequired,
  hidden: PropTypes.bool
}

export default compose(
  connect((state) => {
    return {
      isAuthenticated: getAuthToken(),
      user: getHeaderUser(state),
      activeOrganization: getHeaderOrganization(state)
    }
  }),
  withNavigationDialog,
  withReminderDialog,
  injectIntl
)(Header)
