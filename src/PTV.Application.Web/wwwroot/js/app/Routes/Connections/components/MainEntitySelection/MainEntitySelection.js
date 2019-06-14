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
import { Button } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  setConnectionsEntity
} from 'reducers/selections'
import { change } from 'redux-form/immutable'
import styles from './styles.scss'
import { makeCurrentFormStateInitial } from 'Routes/Connections/actions'
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import { getIsPreloaderVisible } from 'Routes/App/selectors'

const messages = defineMessages({
  serviceTabTitle: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTab.Title',
    defaultMessage: 'Valitse palveluja työpöydälle'
  },
  channelTabTitle: {
    id: 'Routes.Connections.Components.SearchForm.ChannelTab.Title',
    defaultMessage: 'Valitse asiointikanavia työpöydälle'
  }
})

const MainEntitySelection = ({
  setEntity,
  setFormValue,
  intl: { formatMessage },
  makeCurrentFormStateInitial,
  isPreloaderVisible
}) => {
  const handleSelectChannels = () => {
    setEntity('channels')
    setFormValue('connectionsWorkbench', 'mainEntityType', 'channels')
    makeCurrentFormStateInitial()
  }
  const handleSelectServices = () => {
    setEntity('services')
    setFormValue('connectionsWorkbench', 'mainEntityType', 'services')
    makeCurrentFormStateInitial()
  }
  return (
    <div className={styles.buttonGroup}>
      <Button
        small
        secondary
        onClick={handleSelectServices}
        children={formatMessage(messages.serviceTabTitle)}
        disabled={isPreloaderVisible}
      />
      <Button
        small
        secondary
        onClick={handleSelectChannels}
        children={formatMessage(messages.channelTabTitle)}
        disabled={isPreloaderVisible}
      />
    </div>
  )
}
MainEntitySelection.propTypes = {
  setEntity: PropTypes.func.isRequired,
  setFormValue: PropTypes.func.isRequired,
  intl: intlShape,
  makeCurrentFormStateInitial: PropTypes.func.isRequired,
  isPreloaderVisible: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(state => {
    return {
      isPreloaderVisible: getIsPreloaderVisible(state)
    }
  }, {
    setEntity: setConnectionsEntity,
    setFormValue: change,
    makeCurrentFormStateInitial
  })
)(MainEntitySelection)
