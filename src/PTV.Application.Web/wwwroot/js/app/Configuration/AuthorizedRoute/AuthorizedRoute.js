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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { Route, Redirect } from 'react-router-dom'
import { connect } from 'react-redux'
import { FormattedMessage, defineMessages } from 'util/react-intl'
import { getAuthToken, getLogoAddress } from '../AppHelpers'
import { getHasAccess, getUserInfoAreDataValid } from 'selectors/userInfo'
import { Alert } from 'sema-ui-components'

const messages = defineMessages({
  userNoAccessMessage: {
    id: 'Configuration.App.UserNoAccess.Message',
    defaultMessage: 'User has no access to application! Check access rights and assigned organization.'
  }
})

const logoAddress = getLogoAddress()

const NoAccess = () => (
  <span className='space after'>
    <Alert info>
      <FormattedMessage {...messages.userNoAccessMessage} />
    </Alert>
  </span>
)
class AuthorizedRoute extends Component {
  render () {
    const logged = getAuthToken()
    const { component: Component, hasAccess, userDataAreValid, ...rest } = this.props
    if (!hasAccess && logoAddress) {
      window.location.href = logoAddress
      return null
    }
    return (
      <Route
        {...rest}
        render={props => logged && userDataAreValid
          ? hasAccess
            ? <Component {...this.props} />
            : <NoAccess />
          : <Redirect to={{ pathname: '/login', state: { from: props.location } }} />
        }
      />
    )
  }
}

AuthorizedRoute.propTypes = {
  logged: PropTypes.any,
  component: PropTypes.func.isRequired,
  hasAccess: PropTypes.bool,
  userDataAreValid: PropTypes.bool
}

export default connect(state => ({
  hasAccess: getHasAccess(state),
  userDataAreValid: getUserInfoAreDataValid(state)
}))(AuthorizedRoute)
