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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import styles from './styles.scss'
import { Sticky, StickyContainer } from 'react-sticky'
import { Notification, Button } from 'sema-ui-components'
import { withRouter } from 'react-router'
import { isUserOrganizationDraft, isUserOrganizationVersionId } from './selectors'
import withState from 'util/withState'
import { getEnumTypesIsFetching } from 'selectors/base'

const messages = defineMessages({
  noOrganizationTitle: {
    id: 'Util.ReduxForm.HOC.WithNoPublishOrganization.Title',
    defaultMessage: 'Your own organization is in Draft state, use follow link to publish organization'
  },
  linkToOrganization: {
    id: 'Util.ReduxForm.HOC.WithNoPublishOrganization.Link',
    defaultMessage: 'Edit my Organization'
  }
})

const withNoPublishOrganization = ComposedComponent => {
  class InnerComponent extends Component {
    handleCloseNotification = () => {
      return this.props.updateUI('isOpen', false)
    }

    componentWillUnmount () {
      this.props.updateUI('isOpen', true)
    }

    render () {
      return (
        <StickyContainer>
          <div className={styles.wrap}>
            <Sticky>
              <div className={styles.notification}>
                { this.props.show &&
                <Notification
                  title={this.props.intl.formatMessage(messages.noOrganizationTitle)}
                  type='fail'
                  onRequestClose={this.handleCloseNotification}
                >
                  <Button link onClick={() => this.props.history.push('/organization/' + this.props.organizationId)}>
                    {this.props.intl.formatMessage(messages.linkToOrganization)}
                  </Button>
                </Notification>}
              </div>
            </Sticky>
            <ComposedComponent {...this.props} />
          </div>
        </StickyContainer>
      )
    }
  }
  InnerComponent.propTypes = {
    intl: intlShape.isRequired,
    organizationId: PropTypes.string,
    show: PropTypes.bool,
    history: PropTypes.object.isRequired,
    updateUI: PropTypes.func.isRequired
  }
  return compose(
    injectIntl,
    withRouter,
    withState({
      redux: true,
      key: `withNoPublishOrganizationNotification`,
      initialState: {
        isOpen: true
      }
    }),
    connect(
      (state, ownProps) => ({
        show: ownProps.isOpen && isUserOrganizationDraft(state, ownProps) && !getEnumTypesIsFetching(state),
        organizationId: isUserOrganizationVersionId(state, ownProps)
      })
    )
  )(InnerComponent)
}
export default withNoPublishOrganization
