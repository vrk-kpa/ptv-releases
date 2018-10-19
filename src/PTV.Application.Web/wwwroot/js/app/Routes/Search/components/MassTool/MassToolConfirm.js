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
import {
  Field,
  fieldPropTypes,
  formValues
} from 'redux-form/immutable'
import { Button } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { messages } from './messages'
import TooltipLabel from 'appComponents/TooltipLabel'
import {
  // MassPublishCustomContent,
  MassCopyCustomContent
  // CreateTaskListCustomContent
} from './MassToolCustomContent'
import {
  confirmMassPublishSelection,
  confirmMassCopySelection,
  confirmMassArchiveSelection,
  clearSelection
} from './actions'
import {
  getSelectedCount,
  getSelectedLanguageVersionsCount
  // getEntitiesForReviewCount
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
// import withMassDialog from 'util/redux-form/HOC/withMassDialog'
import withNotificationIcon from 'util/redux-form/HOC/withNotificationIcon'
// import { branch } from 'recompose'
import { massToolTypes, MASSTOOL_SELECTION_LIMIT } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'

const ConfirmButton = compose(
  injectIntl,
  withNotificationIcon
)(({
  massToolType,
  // filteredCount,
  intl: { formatMessage },
  ...rest
}) => {
  const buttonClass = cx(
    {
      [styles.publishConfirmButton]: massToolType === massToolTypes.PUBLISH
    }
  )
  return <Button
    small
    children={formatMessage(messages[`${massToolType}ConfrimButton`])}
    // disabled={!filteredCount}
    className={buttonClass}
    {...rest}
  />
})
class RenderMassToolConfirmDef extends Component {
  onClick = () => {
    const { massToolType, organization } = this.props
    switch (massToolType) {
      case massToolTypes.PUBLISH:
        return this.props.confirmMassPublishSelection({
          formName: this.props.meta.form,
          selected: this.props.input.value
        })
      case massToolTypes.COPY:
        return this.props.confirmMassCopySelection({ selected: this.props.input.value, organization })
      case massToolTypes.ARCHIVE:
        return this.props.confirmMassArchiveSelection({ selected: this.props.input.value })
    }
  }

  handleClear = () => {
    this.props.clearSelection()
  }

  getCustomDisabled = type => {
    switch (type) {
      case massToolTypes.COPY:
        return !this.props.organization
      default:
        return false
    }
  }

  render () {
    const {
      intl: { formatMessage },
      count,
      totalCount,
      // filteredCount,
      massToolType
    } = this.props
    const isEmpty = count === 0 // && !filteredCount && massToolType === massToolTypes.PUBLISH
    const limitReached = totalCount > MASSTOOL_SELECTION_LIMIT
    const customDisabled = this.getCustomDisabled(massToolType)
    let notificationText = null
    if (isEmpty) {
      notificationText = formatMessage(messages.emptySelection)
    } else if (limitReached) {
      notificationText = formatMessage(messages.limitReached, { limit: MASSTOOL_SELECTION_LIMIT })
    }
    return (
      <div className={styles.massToolConfirmWrap}>
        <div className={styles.massToolConfirmHeader}>
          <TooltipLabel
            labelProps={{ labelText: formatMessage(messages[`${massToolType}Title`]) }}
            tooltipProps={{ tooltip: formatMessage(messages[`${massToolType}Tooltip`]) }}
            componentClass={styles.tooltipLabel}
          />
          <p>
            <span>
              {formatMessage(messages[`${massToolType}SelectionInstruction`], { selected: count, totalCount })}
            </span>
          </p>
        </div>
        <div className={styles.massToolConfirmFooter}>
          <div className='row align-items-center'>
            {/* <MassPublishCustomContent componentClass='col-lg-6' /> */}
            <MassCopyCustomContent componentClass='col-lg-8' />
            {/* <CreateTaskListCustomContent componentClass='col-lg-12' /> */}
            <div className='col-lg-12'>
              <div className={styles.buttonGroup}>
                <ConfirmButton
                  massToolType={massToolType}
                  // filteredCount={filteredCount}
                  disabled={isEmpty || limitReached || customDisabled}
                  onClick={this.onClick}
                  notificationText={notificationText}
                  shouldShowNotificationIcon={limitReached}
                  className={styles.notification}
                  afterButton
                />
                <Button
                  small
                  secondary
                  disabled={isEmpty}
                  onClick={this.handleClear}
                  children={formatMessage(messages.clearButton)}
                />
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }
}

RenderMassToolConfirmDef.propTypes = {
  intl: intlShape.isRequired,
  input: fieldPropTypes.input,
  meta: fieldPropTypes.meta,
  confirmMassPublishSelection: PropTypes.func.isRequired,
  confirmMassCopySelection: PropTypes.func.isRequired,
  confirmMassArchiveSelection: PropTypes.func.isRequired,
  clearSelection: PropTypes.func.isRequired,
  count: PropTypes.number.isRequired,
  totalCount: PropTypes.number.isRequired,
  // filteredCount: PropTypes.number.isRequired,
  massToolType: PropTypes.string,
  organization: PropTypes.string
}

const RenderMassToolConfirm = compose(
  injectIntl,
  injectFormName,
  formValues({ massToolType: 'type', organization: 'organization' }),
  connect(
    (state, ownProps) => {
      return {
        count: getSelectedCount(state, { selected: ownProps.input.value }),
        totalCount: getSelectedLanguageVersionsCount(state, {
          selected: ownProps.input.value, formName: ownProps.formName
        })
        // filteredCount: getEntitiesForReviewCount(state, {
        //   selected: ownProps.input.value, formName: ownProps.formName
        // })
      }
    }, {
      confirmMassPublishSelection,
      confirmMassCopySelection,
      confirmMassArchiveSelection,
      clearSelection
    }
  )
)(RenderMassToolConfirmDef)

const MassToolConfirm = props => (
  <Field
    name='selected'
    component={RenderMassToolConfirm}
  />
)

export default MassToolConfirm
