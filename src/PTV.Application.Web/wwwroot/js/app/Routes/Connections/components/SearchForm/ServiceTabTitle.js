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
import React from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { getConnectionsMainEntity } from 'selectors/selections'
import { entityTypesEnum } from 'enums'
import TabTitle from './TabTitle.js'

const messages = defineMessages({
  serviceTabTitle: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTab.Title',
    defaultMessage: 'Valitse palveluja työpöydälle'
  },
  serviceMainTabTitle1: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTabMainEntity.Title1',
    defaultMessage: 'Valitse'
  },
  serviceMainTabTitle2: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTabMainEntity.Title2',
    defaultMessage: 'palveluja työpöydälle'
  },
  serviceSubTabTitle1: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTabSubEntity.Title1',
    defaultMessage: 'Liitä'
  },
  serviceSubTabTitle2: {
    id: 'Routes.Connections.Components.SearchForm.ServiceTabSubEntity.Title2',
    defaultMessage: 'palveluja työpöydän asiointikanaviin'
  }
})

const ServiceTabTitle = ({
  intl: { formatMessage },
  mainEntity
}) => {
  return mainEntity
    ? mainEntity === entityTypesEnum.SERVICES
      ? <TabTitle type='main' msgPart1={messages.serviceMainTabTitle1} msgPart2={messages.serviceMainTabTitle2} />
      : <TabTitle type='sub' msgPart1={messages.serviceSubTabTitle1} msgPart2={messages.serviceSubTabTitle2} />
    : <span>{formatMessage(messages.serviceTabTitle)}</span>
}
ServiceTabTitle.propTypes = {
  intl: intlShape,
  mainEntity: PropTypes.string
}

export default compose(
  injectIntl,
  connect(state => ({
    mainEntity: getConnectionsMainEntity(state)
  }))
)(ServiceTabTitle)
