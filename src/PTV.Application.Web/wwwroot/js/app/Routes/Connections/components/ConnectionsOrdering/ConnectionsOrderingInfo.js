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
import { getConnectionsMainEntity } from 'selectors/selections'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import Paragraph from 'appComponents/Paragraph'
import { entityTypesEnum } from 'enums'

const messages = defineMessages({
  servicesOrderingInfo: {
    id: 'ConnectionsOrderingInfo.Services.Title',
    defaultMessage: 'You can arrange the services in the desired order. Place the cursor over the arrow icon, left-click on the mouse and drag the line to the desired item. You cannot change the channel type order.' // eslint-disable-line
  },
  channelsOrderingInfo: {
    id: 'ConnectionsOrderingInfo.Channels.Title',
    defaultMessage: 'Voit järjestää kanavat haluamaasi järjestykseen kanavatyypin sisällä. Aseta kursori nuoli-ikonin päälle, paina hiiren vasenta nappia ja raahaa samalla rivi haluamaasi kohtaan listaa. Et voi muuttaa kanavatyyppijärjestystä.' // eslint-disable-line
  }
})

const ConnectionsOrderingInfo = ({
  intl: { formatMessage },
  mainEntity
}) => {
  const connectedEntity = {
    channels: entityTypesEnum.SERVICES,
    services: entityTypesEnum.CHANNELS
  }[mainEntity]
  return (
    <Paragraph>
      {formatMessage(messages[`${connectedEntity}OrderingInfo`])}
    </Paragraph>
  )
}
ConnectionsOrderingInfo.propTypes = {
  intl: intlShape,
  mainEntity: PropTypes.oneOf([
    entityTypesEnum.SERVICES,
    entityTypesEnum.CHANNELS
  ])
}

export default compose(
  injectIntl,
  connect(state => ({
    mainEntity: getConnectionsMainEntity(state)
  }))
)(ConnectionsOrderingInfo)
