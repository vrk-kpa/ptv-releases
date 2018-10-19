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
import { compose } from 'redux'
import { branch } from 'recompose'
import { injectIntl } from 'util/react-intl'
import { connect } from 'react-redux'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import cx from 'classnames'
import { PTVIcon } from 'Components'
import Popup from 'appComponents/Popup'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asComparable from 'util/redux-form/HOC/asComparable'
import { getIsReadOnly } from 'selectors/formStates'

const GroupHead = compose(
  injectIntl,
  branch(({ doNotCompareGroupHead }) => !doNotCompareGroupHead, asComparable({ DisplayRender: GroupHead }))
)(({
  isReadOnly,
  groupTitle,
  groupTooltip,
  required,
  formatText
}) => (
  <div className={styles.groupHeader}>
    <div className={styles.groupTitle}>
      <Label labelText={formatText(groupTitle)} labelPosition='top' required={required && !isReadOnly}>
        {!isReadOnly && groupTooltip &&
          <Popup
            trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
            content={groupTooltip}
          />
        }
      </Label>
    </div>
  </div>
))

const asGroup = ({ title, tooltip, required, customReadTitle } = {}) => InnerComponent => {
  const Group = ({ isReadOnly, titleFromProps, tooltipFromProps, withError, doNotCompareGroupHead, ...rest }) => {
    const groupClass = cx(
      styles.group,
      {
        [styles.readOnly]: isReadOnly,
        [styles.withError]: withError,
        [styles.customReadTitle]: customReadTitle
      }
    )
    const formatText = text => text && typeof text === 'object' && text.id && rest.intl.formatMessage(text) || text
    const groupTitle = titleFromProps || title
    const groupTooltip = tooltipFromProps || tooltip
    return (
      <div className={groupClass}>
        {groupTitle &&
          <GroupHead
            isReadOnly={isReadOnly}
            groupTitle={groupTitle}
            groupTooltip={groupTooltip}
            required={required}
            formatText={formatText}
            doNotCompareGroupHead={doNotCompareGroupHead}
          />
        }
        <div className={styles.groupBody}>
          <InnerComponent {...rest} />
        </div>
      </div>
    )
  }
  Group.propTypes = {
    isReadOnly: PropTypes.bool,
    titleFromProps: PropTypes.object,
    tooltipFromProps: PropTypes.object,
    withError: PropTypes.bool,
    doNotCompareGroupHead: PropTypes.bool
  }
  return compose(
    injectFormName,
    injectIntl,
    connect(
      (state, { formName, title, tooltip }) => {
        return {
          isReadOnly: getIsReadOnly(formName)(state),
          titleFromProps: title,
          tooltipFromProps: tooltip
        }
      })
  )(Group)
}

export default asGroup
