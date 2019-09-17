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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { List } from 'immutable'
import styles from './styles.scss'
import { localizeMessage } from 'appComponents/ValidationMessage/ValidationMessage'

const messages = defineMessages({
  warningList: {
    id : 'Util.ReduxForm.HOC.WithErrorMessage.WarningList.Title',
    defaultMessage: 'Laatuohjeet'
  }
})

const WarningList = ({ warnings, keyProp, formatMessage }) => {
  return warnings && <ul className={styles.otherWarnings}>
    {warnings.map((warning, index) => {
      return warning.get('title') && (
        <Fragment key={keyProp ? warning.get(keyProp) : index}>
          <li>
            {localizeMessage(warning.get('title'), null, formatMessage)}
          </li>
          <WarningList warnings={warning.get('subMessages')} key='id' />
        </Fragment>
      )
    }).toArray()}
  </ul> || null
}

WarningList.propTypes = {
  warnings: ImmutablePropTypes.list,
  intl: intlShape
}

// const WarningList = compose(
//   injectIntl
// )(WarningListDef)

const WarningComponent = ({ formWarnings, formatMessage, otherWarnings }) => {
  return (
    <div className={styles.warningList}>
      <h6>{formatMessage(messages.warningList)}</h6>
      {formWarnings && (
        <ul>
          {typeof formWarnings.map === 'function'
            ? formWarnings.map((w, i) => <li key={i}>{w}</li>)
            : <li>{formWarnings}</li>
          }
        </ul>
      )}
      <WarningList warnings={otherWarnings} keyProp='id' formatMessage={formatMessage} />
    </div>
  )
}

WarningComponent.propTypes = {
  formWarnings: PropTypes.oneOfType([
    PropTypes.array,
    ImmutablePropTypes.list
  ]),
  formatMessage: PropTypes.func.isRequired,
  otherWarnings: ImmutablePropTypes.list
}

export default WarningComponent
