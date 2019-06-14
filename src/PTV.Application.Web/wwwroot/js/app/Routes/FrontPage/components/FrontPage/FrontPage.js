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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import styles from './styles.scss'
import { Route, Switch, Redirect } from 'react-router-dom'
import { compose } from 'redux'
import { withRouter } from 'react-router'
import withNoPublishOrganization from 'util/redux-form/HOC/withNoPublishOrganization'
import Search from 'Routes/Search'
import Connections from 'Routes/Connections'
import Tasks from 'Routes/Tasks'
import { getTasksSummaryCount, getNumbersIsFetching } from 'Routes/Tasks/selectors'
import { getIsMassSelectionInProgress, getIsPreloaderVisible } from 'Routes/App/selectors'
import { getNotificationsCount } from 'Routes/Tasks/selectors/notifications'
import { getIsMassToolActive } from 'Routes/Search/components/MassTool/selectors'
import { isDirty } from 'redux-form/immutable'
import { reset } from 'redux-form'
import { mergeInUIState } from 'reducers/ui'
import { getConnectionsMainEntity } from 'selectors/selections'
import { formTypesEnum } from 'enums'

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

const resetMassToolForm = () => ({ dispatch }) => {
  dispatch(reset(formTypesEnum.MASSTOOLSELECTIONFORM))
}

const LabelWithCounter = connect((state, ownProps) => ({
  isLoading: getNumbersIsFetching(state)
}))(({ label, count, isLoading }) => (
  <div className={styles.labelWrap}>
    <span>{label}</span>
    {isLoading && <Spinner className={styles.loadCounter} /> || <span className={styles.labelCounter}>{count}</span>}
  </div>
))

const navPathIndices = {
  '/frontpage/search': 0,
  '/frontpage/connections': 1,
  '/frontpage/tasks': 2
}

const FrontPage = ({
  intl: { formatMessage },
  location: { pathname },
  history,
  tasksCount,
  notificationsCount,
  computedMatch: { path },
  isConnectionsWorkbenchTouched,
  mergeInUIState,
  isMassSelectionInProgress,
  resetMassToolForm,
  isMassToolActive,
  isPreloaderVisible
}) => {
  const handleOnTabClick = (navigateTo, currentIndex) => {
    // do not trigger click action when current tab is clicked
    if (currentIndex === navPathIndices[pathname]) return

    if (isMassSelectionInProgress) {
      resetMassToolForm()
    }
    if (isMassToolActive) {
      mergeInUIState({
        key: formTypesEnum.MASSTOOLFORM,
        value: {
          isVisible: false
        }
      })
    }
    if (isConnectionsWorkbenchTouched) {
      mergeInUIState({
        key: 'navigationDialog',
        value: {
          isOpen: true,
          navigateTo
        }
      })
    } else {
      history.push(navigateTo)
    }
  }

  return (
    <div className={styles.page}>
      <header className={styles.pageHeader}>
        <div className={styles.pageHeaderInner}>
          <h1>{formatMessage(messages.pageTitle)}</h1>
          <div>{formatMessage(messages.pageDescription)}</div>
        </div>
      </header>
      <div className={styles.pageNavigation}>
        <Tabs
          index={navPathIndices[pathname]}
          className='indented'
        >
          <Tab
            onClick={() => handleOnTabClick('/frontpage/search', 0)}
            label={formatMessage(messages.navigationSearch)}
          />
          <Tab
            onClick={() => handleOnTabClick('/frontpage/connections', 1)}
            label={formatMessage(messages.navigationRelations)}
          />
          <Tab
            onClick={() => handleOnTabClick('/frontpage/tasks', 2)}
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
              <FrontPageButtonMenu disabled={isPreloaderVisible} />
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
}

FrontPage.propTypes = {
  intl: intlShape,
  location: PropTypes.object.isRequired,
  computedMatch: PropTypes.object.isRequired,
  tasksCount: PropTypes.number.isRequired,
  notificationsCount: PropTypes.number.isRequired,
  history: PropTypes.object.isRequired,
  isConnectionsWorkbenchTouched: PropTypes.bool,
  mergeInUIState: PropTypes.func.isRequired,
  isMassSelectionInProgress: PropTypes.bool,
  resetMassToolForm: PropTypes.func,
  isMassToolActive: PropTypes.bool,
  isPreloaderVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(
    state => {
      const mainConnectionEntity = getConnectionsMainEntity(state)
      const isConnectionsWorkbenchDirty = isDirty(formTypesEnum.CONNECTIONSWORKBENCH)(state)
      const isConnectionsWorkbenchTouched = mainConnectionEntity && isConnectionsWorkbenchDirty
      return {
        tasksCount: getTasksSummaryCount(state),
        notificationsCount: getNotificationsCount(state),
        isConnectionsWorkbenchTouched,
        isMassSelectionInProgress: getIsMassSelectionInProgress(state),
        isMassToolActive: getIsMassToolActive(state),
        isPreloaderVisible: getIsPreloaderVisible(state)
      }
    }, {
      mergeInUIState,
      resetMassToolForm
    }
  ),
  withRouter,
  withNoPublishOrganization
)(FrontPage)
