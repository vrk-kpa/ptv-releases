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
import {
  getUserRoleName,
  getApplicationVersion
} from 'selectors/userInfo'
import { getFooterUserName } from './selectors'
import { FormattedMessage, defineMessages, injectIntl, intlShape } from 'util/react-intl'
import 'styles/site.scss'
import { SimpleFooter } from 'sema-ui-components'
import { Link } from 'react-router-dom'
import { PTVIcon } from 'Components'
import cx from 'classnames'
import styles from './styles.scss'
import appMessages from './messages'

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

const Footer = ({ name, role, version, intl:{ formatMessage }, ...rest }) => {
  const roleName = role && roleNames[role.toLowerCase()] || null
  const messages = {
    footerTitle: formatMessage(appMessages.appNameTitle),
    footerDescription: formatMessage(appMessages.footerServiceDescription),
    footerLogoLink: formatMessage(appMessages.suomiFiLink)
  }
  const footerLinks = [
    [
      {
        href: formatMessage(appMessages.appCompanyNameLink),
        linkText: formatMessage(appMessages.appCompanyNameTitle),
        description: formatMessage(appMessages.appContentManagementServiceTitle),
        inline: true,
        external: true
      }, {
        href: formatMessage(appMessages.privacyPolicyLink),
        linkText: formatMessage(appMessages.privacyPolicy),
        external: true
      }, {
        href: formatMessage(appMessages.feedbackLink),
        linkText: formatMessage(appMessages.feedbackTitle),
        external: true
      }, {
        href: formatMessage(appMessages.appAboutLink),
        linkText: formatMessage(appMessages.appServiceNameTitle),
        external: true
      }
    ], [
      {
        href: formatMessage(appMessages.appSourceCodeLink),
        linkText: formatMessage(appMessages.appSourceCodeTitle),
        description: formatMessage(appMessages.appSourceCodeInfo),
        external: true
      }, {
        custom: <Link to='/currentIssues'>{formatMessage(appMessages.currentIssuesInfo)}</Link>
      }
    ]
  ]
  const channels = [
    <a href='https://www.youtube.com/user/suomifitoimitus' target='_blank'>
      <PTVIcon name='icon-youtube' height={24} />
    </a>,
    <a href='http://twitter.com/vm_kapa' target='_blank'>
      <PTVIcon name='icon-twitter2' height={24} />
    </a>,
    <a href='https://www.facebook.com/suomifi/' target='_blank'>
      <PTVIcon name='icon-facebook' height={24} />
    </a>
  ]
  const renderDevelopmentInfo = () => {
    const isProd = getEnvironmentType() === 'prod'
    const infoClass = cx(
      styles.developmentInfo,
      {
        [styles.isProd]: isProd
      }
    )
    return (
      <div className={infoClass}>
        <span><FormattedMessage {...appMessages.appUserNameTitle} values={{ name }} /></span>
        <span>
          <FormattedMessage
            {...appMessages.appUserRoleTitle}
            values={{ role: (roleName && formatMessage(roleName)) || role }}
          />
        </span>
        <span><FormattedMessage {...appMessages.appVersionTitle} values={{ version }} /></span>
      </div>
    )
  }
  return (
    <SimpleFooter
      channels={channels}
      footerLinks={footerLinks}
      messages={messages}
      developmentInfo={renderDevelopmentInfo()}
      {...rest}
    />
  )
}

Footer.propTypes = {
  name: PropTypes.string,
  role: PropTypes.string,
  version: PropTypes.string,
  intl: intlShape.isRequired
}

export default compose(
  connect((state, ownProps) => ({
    name: getFooterUserName(state),
    role: getUserRoleName(state),
    version: getApplicationVersion(state)
  })),
  injectIntl
)(Footer)
