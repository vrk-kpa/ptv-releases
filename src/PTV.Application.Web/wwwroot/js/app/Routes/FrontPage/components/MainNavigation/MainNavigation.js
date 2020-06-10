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
import React, { Fragment } from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { Tabs, Tab } from 'sema-ui-components'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import {
  getTasksSectionTotalCount,
  getIsNavigationItemVisible,
  getVisibleNavigationItemCount
} from 'Routes/FrontPage/selectors'
import { getAdminTasksCount } from 'Routes/Admin/selectors'
import { getIsMassToolActive } from 'Routes/Search/components/MassTool/selectors'
import { isDirty } from 'redux-form/immutable'
import { reset } from 'redux-form'
import { mergeInUIState } from 'reducers/ui'
import { getConnectionsMainEntity } from 'selectors/selections'
import { formTypesEnum, mainNavigationSlugsEnum, breakPointsEnum } from 'enums'
import MainNavigationOption from './MainNavigationOption'
import LabelWithCounter from './LabelWithCounter'
import withState from 'util/withState'
import { push } from 'connected-react-router'
import withWindowDimensionsContext from 'util/redux-form/HOC/withWindowDimensions/withWindowDimensionsContext'
import { withRouter } from 'react-router'
import cx from 'classnames'
import styles from './styles.scss'

const messages = defineMessages({
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
  },
  navigationAdmin: {
    id: 'FrontPage.Navigation.Admin.Title',
    defaultMessage: 'Järjestelmäpääkäyttäjä'
  }
})

const options = [
  {
    slug: mainNavigationSlugsEnum.SEARCH,
    message: messages.navigationSearch,
    index: 0,
    domain: null
  }, {
    slug: mainNavigationSlugsEnum.CONNECTIONS,
    message: messages.navigationRelations,
    index: 1,
    domain: null
  }, {
    slug: mainNavigationSlugsEnum.TASKS,
    message: messages.navigationTasksAndNotifications,
    index: 2,
    domain: null,
    countSelector: getTasksSectionTotalCount
  }, {
    slug: mainNavigationSlugsEnum.ADMIN,
    message: messages.navigationAdmin,
    index: 3,
    domain: 'adminSection',
    countSelector: getAdminTasksCount
  }
]

const mainNavigationOptionRenderer = option => <MainNavigationOption {...option} />
const mainNavigationValueRenderer = option => <MainNavigationOption {...option} />

const MainNavigation = ({
  intl: { formatMessage },
  tasksSectionTotalCount,
  adminTasksCount,
  isConnectionsWorkbenchTouched,
  mergeInUIState,
  resetForm,
  isMassToolActive,
  activeIndex,
  push,
  dimensions,
  isAdminNavigationVisible,
  visibleNavigationItemCount,
  isAnyCheckedFormDirty
}) => {
  const compact = (
    (dimensions.breakPoint === breakPointsEnum.XL && visibleNavigationItemCount > 6) ||
    (dimensions.breakPoint === breakPointsEnum.LG && visibleNavigationItemCount > 4) ||
    (dimensions.breakPoint === breakPointsEnum.MD && visibleNavigationItemCount > 3) ||
    (dimensions.breakPoint === breakPointsEnum.SM && visibleNavigationItemCount > 2) ||
    (dimensions.breakPoint === breakPointsEnum.XS)
  )
  const handleOnChange = ({ slug, index }) => {
    const navigateTo = `/frontpage/${slug}`
    // do not trigger click action when current tab is clicked
    if (index === activeIndex) return
    if (!isConnectionsWorkbenchTouched && !isAnyCheckedFormDirty) {
      mergeInUIState({
        key: 'mainNavigationIndex',
        value: {
          activeIndex: index
        }
      })
    }
    resetForm(formTypesEnum.MASSTOOLSELECTIONFORM)
    if (isMassToolActive) {
      mergeInUIState({
        key: formTypesEnum.MASSTOOLFORM,
        value: {
          isVisible: false
        }
      })
    }
    if (isConnectionsWorkbenchTouched || isAnyCheckedFormDirty) {
      mergeInUIState({
        key: 'navigationDialog',
        value: {
          isOpen: true,
          navigateTo,
          defaultAction: () => mergeInUIState({
            key: 'mainNavigationIndex',
            value: {
              activeIndex: index
            }
          })
        }
      })
    } else {
      push(navigateTo)
    }
  }

  const mainNavigationClass = cx(
    styles.mainNavigation,
    {
      [styles.compact]: compact
    }
  )
  return (
    <Tabs
      index={activeIndex}
      compact={compact}
      selectProps={{
        options,
        value: options[activeIndex],
        valueKey: 'slug',
        onChange: handleOnChange,
        optionRenderer: mainNavigationOptionRenderer,
        valueRenderer: mainNavigationValueRenderer,
        size: 'w240'
      }}
      className={mainNavigationClass}
    >
      <Tab
        onClick={() => handleOnChange(options[0])}
        label={formatMessage(messages.navigationSearch)}
      />
      <Tab
        onClick={() => handleOnChange(options[1])}
        label={formatMessage(messages.navigationRelations)}
      />
      <Tab
        onClick={() => handleOnChange(options[2])}
        label={(
          <LabelWithCounter
            label={formatMessage(messages.navigationTasksAndNotifications)}
            count={tasksSectionTotalCount}
          />
        )}
      />
      {isAdminNavigationVisible && (
        <Tab
          onClick={() => handleOnChange(options[3])}
          label={(
            <LabelWithCounter
              label={formatMessage(messages.navigationAdmin)}
              count={adminTasksCount}
            />
          )}
        />
      ) || <Fragment />}
    </Tabs>
  )
}

MainNavigation.propTypes = {
  intl: intlShape,
  tasksSectionTotalCount: PropTypes.number.isRequired,
  adminTasksCount: PropTypes.number.isRequired,
  isConnectionsWorkbenchTouched: PropTypes.bool,
  mergeInUIState: PropTypes.func.isRequired,
  resetForm: PropTypes.func,
  isMassToolActive: PropTypes.bool,
  activeIndex: PropTypes.number,
  push: PropTypes.func.isRequired,
  dimensions: PropTypes.object,
  isAdminNavigationVisible: PropTypes.bool,
  visibleNavigationItemCount: PropTypes.number,
  isAnyCheckedFormDirty: PropTypes.bool
}

export default compose(
  injectIntl,
  withRouter,
  withState({
    redux: true,
    key: 'mainNavigationIndex',
    initialState: ({ activeIndex = 0, location }) => {
      switch (location.pathname) {
        case `/frontpage/${mainNavigationSlugsEnum.CONNECTIONS}`:
          activeIndex = 1
          break
        case `/frontpage/${mainNavigationSlugsEnum.TASKS}`:
          activeIndex = 2
          break
        case `/frontpage/${mainNavigationSlugsEnum.ADMIN}`:
          activeIndex = 3
          break
      }
      return ({ activeIndex })
    }
  }),
  connect(
    state => {
      const mainConnectionEntity = getConnectionsMainEntity(state)
      const isConnectionsWorkbenchDirty = isDirty(formTypesEnum.CONNECTIONSWORKBENCH)(state)
      const isConnectionsWorkbenchTouched = mainConnectionEntity && isConnectionsWorkbenchDirty
      return {
        tasksSectionTotalCount: getTasksSectionTotalCount(state),
        adminTasksCount: getAdminTasksCount(state),
        isConnectionsWorkbenchTouched,
        isMassToolActive: getIsMassToolActive(state),
        isAdminNavigationVisible: getIsNavigationItemVisible(state, { domain: 'adminSection' }),
        visibleNavigationItemCount: getVisibleNavigationItemCount(state),
        isAnyCheckedFormDirty: isDirty(formTypesEnum.UNSTABLELINKFORM)(state) ||
          isDirty(formTypesEnum.EXCEPTIONLINKFORM)(state)
      }
    }, {
      mergeInUIState,
      resetForm: reset,
      push
    }
  ),
  withWindowDimensionsContext
)(MainNavigation)
