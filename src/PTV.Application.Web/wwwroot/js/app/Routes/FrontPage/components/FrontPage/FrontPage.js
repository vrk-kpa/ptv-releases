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
import { Tabs, Tab, Spinner } from 'sema-ui-components'
import { SecurityCreateAny } from 'appComponents/Security'
import FrontPageButtonMenu from '../FrontPageButtonMenu/FrontPageButtonMenu'
import { defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { Route, Switch, Redirect } from 'react-router-dom'
import { compose } from 'redux'
import { withRouter } from 'react-router'
import withNoPublishOrganization from 'util/redux-form/HOC/withNoPublishOrganization'
import Search from 'Routes/Search'
import Connections from 'Routes/Connections'
import Tasks from 'Routes/Tasks'
import { getTasksSummaryCount, getNumbersIsFetching } from 'Routes/Tasks/selectors'
import { getNotificationsCount } from 'Routes/Tasks/selectors/notifications'

const messages = defineMessages({
  pageTitle: {
    id: 'FrontPage.Search.Title',
    defaultMessage: 'Palvelutietovaranto'
  },
  pageDescription: {
    id: 'FrontPage.Search.Description',
    defaultMessage: 'Palvelu-ja pohjakuvaukset, asiontikanavat, organisaatiokuvaukset ja rekisterikuvaukset'
  },
  navigationSearch: {
    id: 'FrontPage.Navigation.Search.Title',
    defaultMessage: 'Kuvaukset'
  },
  navigationRelations: {
    id: 'FrontPage.Navigation.Relations.Title',
    defaultMessage: 'Liitokset'
  },
  navigationTasksAndNotifications: {
    id: 'FrontPage.Navigation.TasksAndNotifications.Title',
    defaultMessage: 'Tehtävät & Ilmoitukset'
  }
})

const LabelWithCounter = connect((state, ownProps) => ({
  isLoading: getNumbersIsFetching(state)
}))(({ label, count, isLoading }) => (
  <div className={styles.labelWrap}>
    <span>{label}</span>
    {isLoading && <Spinner className={styles.loadCounter} /> || <span className={styles.labelCounter}>{count}</span>}
  </div>
))

const FrontPage = ({
  intl: { formatMessage },
  location: { pathname },
  history,
  tasksCount,
  notificationsCount,
  computedMatch: { path }
}) => (
  <div className={styles.page}>
    <header className={styles.pageHeader}>
      <div className={styles.pageHeaderInner}>
        <h1>{formatMessage(messages.pageTitle)}</h1>
        <div>{formatMessage(messages.pageDescription)}</div>
      </div>
    </header>
    <div className={styles.pageNavigation}>
      <Tabs
        index={{ '/frontpage/search': 0, '/frontpage/connections': 1, '/frontpage/tasks': 2 }[pathname]}
        className='indented'
      >
        <Tab
          onClick={() => history.push('/frontpage/search')}
          label={formatMessage(messages.navigationSearch)}
        />
        <Tab
          onClick={() => history.push('/frontpage/connections')}
          label={formatMessage(messages.navigationRelations)}
        />
        <Tab
          onClick={() => history.push('/frontpage/tasks')}
          label={(
            <LabelWithCounter
              label={formatMessage(messages.navigationTasksAndNotifications)}
              count={tasksCount + notificationsCount}
            />
          )}
        />
      </Tabs>
      <div>
        <SecurityCreateAny isNotAccessibleComponent={null}>
          {pathname !== '/frontPage/connections' &&
            <FrontPageButtonMenu />
          }
        </SecurityCreateAny>
      </div>
    </div>
    <div className={styles.pageContent}>
      <Switch>
        <Route path={`${path}/search`} component={Search} />
        <Route path={`${path}/connections`} component={Connections} />
        <Route path={`${path}/tasks`} component={Tasks} />
        <Redirect to={`${path}/search`} />
      </Switch>
    </div>
  </div>
)
FrontPage.propTypes = {
  intl: PropTypes.object,
  location: PropTypes.object.isRequired,
  computedMatch: PropTypes.object.isRequired,
  tasksCount: PropTypes.number.isRequired,
  notificationsCount: PropTypes.number.isRequired,
  history: PropTypes.object.isRequired
}

export default compose(
  connect(
    state => ({
      tasksCount: getTasksSummaryCount(state),
      notificationsCount: getNotificationsCount(state)
    })
  ),
  withRouter,
  withNoPublishOrganization
)(FrontPage)
