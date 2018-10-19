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
import { compose } from 'redux'
import { branch } from 'recompose'
import { getAuthToken, deleteCookie, getLogoAddress } from 'Configuration/AppHelpers'
import { deleteUserInfo } from 'actions/init'
import { ptvCookieTokenName, pahaCookieTokenName, formTypesEnum } from 'enums'
import { injectIntl, intlShape } from 'util/react-intl'
import { mergeInUIState, deleteUIState } from 'reducers/ui'
import { getShowReviewBar } from 'util/redux-form/HOC/withMassToolForm/selectors'
import { getFormName } from 'selectors/entities/entities'
import 'styles/site.scss'
import { SimpleHeader, Button } from 'sema-ui-components'
import withNavigationDialog from 'util/redux-form/HOC/withNavigationDialog'
import { getHeaderUser, getHeaderOrganization, getIsMassSelectionInProgress } from './selectors'
import { setSelectedEntity } from 'reducers/selections'
import { getConnectionsMainEntity } from 'selectors/selections'
import { Link } from 'react-router-dom'
import cx from 'classnames'
import styles from './styles.scss'
import appMessages from './messages'
import { PTVIcon } from 'Components'
import { isDirty, reset } from 'redux-form/immutable'
import { getIsReadOnly } from 'selectors/formStates'

const logoAddress = getLogoAddress()

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
  connect(null, {
    mergeInUIState
  }),
)(({
  showDialog,
  mergeInUIState,
  href,
  text
}) => {
  const handleClick = e => {
    if (showDialog) {
      e.preventDefault()
      mergeInUIState({
        key: 'navigationDialog',
        value: {
          isOpen: true,
          navigateTo: href,
          external: true
        }
      })
    }
  }
  return (
    <a className={styles.menuLink} href={href} onClick={handleClick}>
      {text}
    </a>
  )
})

MenuLink.propTypes = {
  showDialog: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  defaultAction: PropTypes.func,
  navigateTo: PropTypes.string,
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
  connect(null, {
    mergeInUIState,
    setSelectedEntity
  })
)(({
  showDialog,
  mergeInUIState,
  history
}) => {
  const resetEntity = () => { setSelectedEntity({ id: null }) }
  const onLogoClick = () => {
    if (showDialog) {
      mergeInUIState({
        key: 'navigationDialog',
        value: {
          isOpen: true,
          navigateTo: logoAddress || '/frontpage',
          external: !!logoAddress,
          defaultAction: resetEntity
        }
      })
    } else {
      if (logoAddress) {
        window.location.href = logoAddress
      } else {
        resetEntity()
        history.push('/frontpage')
      }
    }
  }
  return <LogoContent onClick={onLogoClick} />
})

const Header = ({
  isAuthenticated,
  user,
  activeOrganization,
  intl:{ formatMessage },
  showDialog,
  deleteUserInfo,
  deleteUIState,
  resetForm,
  mergeInUIState,
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
    <MenuLinks links={userLinks} showDialog={showDialog} />,
    <MenuLinks links={otherLinks} showDialog={showDialog} />
  ]

  const handleLogOffOnClick = (event) => {
    const defaultFunctions = () => {
      deleteCookie(ptvCookieTokenName)
      deleteCookie(pahaCookieTokenName)
      deleteUserInfo()
      deleteUIState()
      resetForm(formTypesEnum.MASSTOOLFORM)
      resetForm(formTypesEnum.MASSTOOLSELECTIONFORM)
    }
    if (showDialog) {
      event.preventDefault()
      mergeInUIState({
        key: 'navigationDialog',
        value: {
          isOpen: true,
          navigateTo: logoAddress ? window.location.href = logoAddress : '/login',
          external: !!logoAddress,
          defaultAction: defaultFunctions
        }
      })
    } else {
      defaultFunctions()
      logoAddress ? window.location.href = logoAddress : rest.history.push('/login')
    }
  }

  const headerClass = cx({
    [styles.notLoggedIn]: !isAuthenticated
  })

  return (
    <SimpleHeader
      menuItems={menuItems}
      logo={<Logo showDialog={showDialog} history={rest.history} />}
      user={user}
      activeOrganization={activeOrganization}
      isAuthenticated={!!isAuthenticated}
      className={headerClass}
      {...rest}
    />
  )
}

Header.propTypes = {
  isAuthenticated: PropTypes.any,
  user: PropTypes.object,
  activeOrganization: PropTypes.object,
  showDialog: PropTypes.bool,
  intl: intlShape.isRequired,
  deleteUIState: PropTypes.func,
  deleteUserInfo: PropTypes.func,
  resetForm: PropTypes.func.isRequired,
  mergeInUIState: PropTypes.func
}

export default compose(
  connect((state, ownProps) => {
    const isInReview = getShowReviewBar(state)
    const mainConnectionEntity = getConnectionsMainEntity(state)
    const isConnectionsWorkbenchDirty = isDirty('connectionsWorkbench')(state)
    const isConnectionsWorkbenchTouched = mainConnectionEntity && isConnectionsWorkbenchDirty
    const formName = getFormName(state)
    const isReadOnly = formName && getIsReadOnly(formName)(state)
    const isMassSelectionInProgress = getIsMassSelectionInProgress(state)
    return {
      isAuthenticated: getAuthToken(),
      user: getHeaderUser(state),
      activeOrganization: getHeaderOrganization(state),
      showDialog: isInReview ||
        isConnectionsWorkbenchTouched ||
        (!!formName && !isReadOnly) ||
        isMassSelectionInProgress
    }
  }, {
    deleteUserInfo,
    deleteUIState,
    resetForm: reset,
    mergeInUIState
  }),
  branch(({ showDialog }) => showDialog, withNavigationDialog),
  injectIntl
)(Header)
