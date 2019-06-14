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
import React, { Component, Fragment } from 'react'
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
import { MassCopyCustomContent } from './MassToolCustomContent'
import { MassToolCart } from './MassToolCart'
import { PTVIcon } from 'Components'
import {
  confirmMassPublishSelection,
  confirmMassCopySelection,
  confirmMassArchiveSelection,
  confirmMassRestoreSelection
} from './actions'
import {
  getSelectedCount,
  getSelectedLanguageVersionsCount
} from './selectors'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { massToolTypes, MASSTOOL_SELECTION_LIMIT } from 'enums'
import cx from 'classnames'
import styles from './styles.scss'
import { withRouter } from 'react-router'

class RenderMassToolConfirmDef extends Component {
  onClick = () => {
    const { massToolType, organization, location } = this.props
    switch (massToolType) {
      case massToolTypes.PUBLISH:
        return this.props.confirmMassPublishSelection({
          formName: this.props.meta.form,
          selected: this.props.input.value,
          returnPath: location.pathname
        })
      case massToolTypes.COPY:
        return this.props.confirmMassCopySelection({ selected: this.props.input.value, organization })
      case massToolTypes.ARCHIVE:
        return this.props.confirmMassArchiveSelection({ selected: this.props.input.value })
      case massToolTypes.RESTORE:
        return this.props.confirmMassRestoreSelection({ selected: this.props.input.value })
    }
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
      massToolType
    } = this.props
    const isEmpty = count === 0
    const limitReached = totalCount > MASSTOOL_SELECTION_LIMIT
    const customDisabled = this.getCustomDisabled(massToolType)
    const isMassPublish = massToolType === massToolTypes.PUBLISH
    const buttonClass = cx(
      {
        [styles.publishConfirmButton]: isMassPublish,
        [styles.copyConfirmButton]: massToolType === massToolTypes.COPY
      }
    )
    const isDisabled = isEmpty || limitReached || customDisabled
    const buttonIconClass = cx(
      styles.confirmButtonArrow,
      {
        [styles.isDisabled]: isDisabled
      }
    )
    return (
      <Fragment>
        <MassToolCart />
        <div className={styles.massToolConfirmFooter}>
          <div className='row align-items-center'>
            <MassCopyCustomContent componentClass='col-sm-12' />
            <div className='mt-2 mt-sm-0 col-sm-12'>
              <div className={styles.buttonGroup}>
                <Button
                  small
                  className={buttonClass}
                  onClick={this.onClick}
                  disabled={isDisabled}
                >
                  <span>{formatMessage(messages[`${massToolType}ConfirmButton`])}</span>
                  {isMassPublish && (
                    <PTVIcon
                      name='icon-arrow-long'
                      componentClass={buttonIconClass}
                      width={16}
                      height={16}
                    />
                  )}
                </Button>
              </div>
            </div>
          </div>
        </div>
      </Fragment>
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
  confirmMassRestoreSelection: PropTypes.func.isRequired,
  count: PropTypes.number.isRequired,
  totalCount: PropTypes.number.isRequired,
  massToolType: PropTypes.string,
  organization: PropTypes.string,
  location: PropTypes.object.isRequired
}

const RenderMassToolConfirm = compose(
  injectIntl,
  injectFormName,
  withRouter,
  formValues({ massToolType: 'type', organization: 'organization' }),
  connect(
    (state, ownProps) => {
      return {
        count: getSelectedCount(state, { selected: ownProps.input.value, formName: ownProps.formName }),
        totalCount: getSelectedLanguageVersionsCount(state, {
          selected: ownProps.input.value, formName: ownProps.formName
        })
      }
    }, {
      confirmMassPublishSelection,
      confirmMassCopySelection,
      confirmMassArchiveSelection,
      confirmMassRestoreSelection
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
