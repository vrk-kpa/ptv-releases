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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import cx from 'classnames'
import { connect } from 'react-redux'
import AdminPage from '../AdminPage'
import AdminTasks from '../AdminTasks'
import AdminMapping from '../AdminMapping'
import { messages } from '../../messages'
import styles from './styles.scss'
import { Tabs, Tab, Label } from 'sema-ui-components'
import Tooltip from 'appComponents/Tooltip'
import { default as LabelWithCounter } from 'appComponents/Label'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import { getAdminTasksCount } from 'Routes/Admin/selectors'
import withState from 'util/withState'

class AdminRoute extends Component {
  handleTabOnChange = index => this.props.updateUI('activeAdminTabIndex', index)
  render () {
    const {
      intl: { formatMessage },
      activeAdminTabIndex,
      adminTasksCount
    } = this.props
    const formClass = cx(
      styles.form,
      styles.adminPage
    )
    return (
      <div className={formClass}>
        <div className={styles.adminPageLabel}>
          <Label labelText={formatMessage(messages.pageTitle)} labelPosition='top'>
            <Tooltip tooltip={formatMessage(messages.pageTooltip)} />
          </Label>
        </div>
        <div>
          <Tabs index={activeAdminTabIndex} onChange={this.handleTabOnChange} className={styles.taskTabs}>
            <Tab
              label={(
                <LabelWithCounter
                  label={formatMessage(messages.adminPageTabName)}
                  count={adminTasksCount}
                />
              )}
            >
              <AdminPage />
            </Tab>
            <Tab label={formatMessage(messages.sahaPtvTabName)}>
              <AdminMapping />
            </Tab>
          </Tabs>
        </div>
      </div>
    )
  }
}
AdminRoute.propTypes = {
  intl: intlShape,
  activeAdminTabIndex: PropTypes.number,
  adminTasksCount: PropTypes.number,
  updateUI: PropTypes.func
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    adminTasksCount: getAdminTasksCount(state, ownProps)
  })),
  withPreviewDialog,
  withState({
    redux: true,
    key: 'adminTabIndex',
    initialState: {
      activeAdminTabIndex: 0
    }
  })
)(AdminRoute)
