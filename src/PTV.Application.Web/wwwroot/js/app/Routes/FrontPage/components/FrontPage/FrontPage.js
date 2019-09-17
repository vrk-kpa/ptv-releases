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
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { SecurityCreateAny } from 'appComponents/Security'
import FrontPageButtonMenu from '../FrontPageButtonMenu/FrontPageButtonMenu'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { Route, Switch, Redirect } from 'react-router-dom'
import { compose } from 'redux'
import withNoPublishOrganization from 'util/redux-form/HOC/withNoPublishOrganization'
import Search from 'Routes/Search'
import Connections from 'Routes/Connections'
import Tasks from 'Routes/Tasks'
import Admin from 'Routes/Admin'
import { getIsPreloaderVisible } from 'Routes/App/selectors'
import MainNavigation from '../MainNavigation'

const messages = defineMessages({
  pageTitle: {
    id: 'FrontPage.Search.Title',
    defaultMessage: 'Palvelutietovaranto'
  },
  pageDescription: {
    id: 'FrontPage.Search.Description',
    defaultMessage: 'Palvelu-ja pohjakuvaukset, asiontikanavat, organisaatiokuvaukset ja rekisterikuvaukset'
  }
})

const FrontPage = ({
  intl: { formatMessage },
  computedMatch: { path },
  isPreloaderVisible
}) => {
  return (
    <div className={styles.page}>
      <header className={styles.pageHeader}>
        <div className={styles.pageHeaderInner}>
          <h1>{formatMessage(messages.pageTitle)}</h1>
          <div>{formatMessage(messages.pageDescription)}</div>
        </div>
      </header>
      <div className={styles.pageNavigationWrap}>
        <MainNavigation />
        <div>
          <SecurityCreateAny isNotAccessibleComponent={null}>
            <FrontPageButtonMenu disabled={isPreloaderVisible} />
          </SecurityCreateAny>
        </div>
      </div>
      <div className={styles.pageContent}>
        <Switch>
          <Route path={`${path}/search`} component={Search} />
          <Route path={`${path}/connections`} component={Connections} />
          <Route path={`${path}/tasks`} component={Tasks} />
          <Route path={`${path}/admin`} component={Admin} />
          <Redirect to={`${path}/search`} />
        </Switch>
      </div>
    </div>
  )
}

FrontPage.propTypes = {
  intl: intlShape,
  computedMatch: PropTypes.object.isRequired,
  isPreloaderVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(state => ({
    isPreloaderVisible: getIsPreloaderVisible(state)
  })),
  withNoPublishOrganization
)(FrontPage)
