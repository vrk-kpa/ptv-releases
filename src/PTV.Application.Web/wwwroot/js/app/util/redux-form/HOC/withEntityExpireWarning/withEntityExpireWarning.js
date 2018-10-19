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
import { connect } from 'react-redux'
import styles from './styles.scss'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { getExpireOn, getIsWarningVisible } from './selectors'
import { getSelectedEntityType } from 'selectors/entities/entities'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { entityTypesEnum } from 'enums'
import PTVIcon from 'Components/PTVIcon'
import moment from 'moment'
import withState from 'util/withState'

const messages = defineMessages({
  serviceExpireTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityExpireWarning.WarningMessage',
    defaultMessage: 'Palvelu vanhenee {expireOn}. Tarkista, että sisältö on ajantasalla, ja sen jälkeen julkaise palvelun uudelleen, jotta se näkyy käyttäjille jatkossakin.'
  },
  channelExpireTitle: {
    id: 'Util.ReduxForm.HOC.WithEntityExpireWarning.ChannelWarningMessage',
    defaultMessage: 'Asiointikanava vanhenee {expireOn}. Tarkista, että sisältö on ajantasalla, ja sen jälkeen julkaise palvelun uudelleen, jotta se näkyy käyttäjille jatkossakin.'
  }
})

const ExpireOnDialog = compose(
  injectIntl,
  connect(state => ({
    entityType: getSelectedEntityType(state)
  }))
)(({ expireOn, intl: { formatMessage }, onCrossClick, entityType }) => {
  const expireMessage = entityType === entityTypesEnum.CHANNELS
    ? messages.channelExpireTitle
    : messages.serviceExpireTitle
  return (
    <div className={styles.expireMessage}>
      <div>
        <span className={styles.notificationIcon}>!</span>
        {formatMessage(expireMessage, { expireOn: moment(expireOn).format('DD.MM.YYYY') })}
      </div>
      <div className={styles.closeMessage}>
        <PTVIcon name='icon-cross' onClick={onCrossClick} />
      </div>
    </div>)
})

ExpireOnDialog.propTypes = {
  expireOn: PropTypes.number,
  onCrossClick: PropTypes.func,
  intl: intlShape,
  entityType: PropTypes.string
}

const withEntityExpireWarning = WrappedComponent => {
  const InnerComponent = props => {
    const {
      expireOn,
      isWaningVisible,
      isWarningOpen,
      updateUI,
      ...rest
    } = props
    const handleCrossClick = () => {
      updateUI('isWarningOpen', false)
    }
    return (
      <div>
        {isWaningVisible && isWarningOpen && <ExpireOnDialog expireOn={expireOn} onCrossClick={handleCrossClick} />}
        <WrappedComponent {...rest} />
      </div>
    )
  }

  InnerComponent.propTypes = {
    expireOn: PropTypes.number,
    isWaningVisible: PropTypes.bool,
    isWarningOpen: PropTypes.bool,
    updateUI: PropTypes.func,
    intl: intlShape
  }

  return compose(
    injectFormName,
    withState({
      initialState: {
        isWarningOpen: true
      }
    }),
    connect((state, { formName }) => ({
      expireOn: getExpireOn(state),
      isWaningVisible: getIsWarningVisible(state)
    }))
  )(InnerComponent)
}

export default withEntityExpireWarning
