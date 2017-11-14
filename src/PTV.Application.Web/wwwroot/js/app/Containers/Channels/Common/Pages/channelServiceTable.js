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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { getDateTimeToDisplay } from '../../../../Components/PTVDateTimePicker/PTVDateTimePicker'

// actions
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'

// components
import { PTVTable, PTVTooltip } from '../../../../Components'
import { LocalizedText } from '../../../Common/localizedData'
import TableNameFormatter from '../../../Common/tableNameFormatter'

// selectors
import * as ServicaChannelCommonSelectors from '../../../Relations/ServiceAndChannels/Common/Selectors'
import * as ChannelCommonSelector from '../../Common/Selectors'

const connectionDateTimeFormater = ({ modified }) => {
  var dateTime = getDateTimeToDisplay(modified)
  return (
    <PTVTooltip
      labelContent={dateTime}
      tooltip={dateTime}
      type='special'
      attachToBody
    />
  )
}
const connectionUserFormater = ({ modifiedBy }) => {
  return (
    <PTVTooltip
      labelContent={modifiedBy}
      tooltip={modifiedBy || null}
      type='special'
      attachToBody
    />
  )
}

const connectionTypeFormater = ({ serviceTypeId, serviceType, language }) => {
  return (
    <LocalizedText
      id={serviceTypeId}
      name={serviceType}
      language={language}
    />
  )
}

const connectionNameFormater = ({ name, language }) => {
  return (
    <TableNameFormatter
      content={name}
      language={language}
    />
  )
}

const mapStateToPropsConnection = (state, ownProps) => {
  return {
    name:ServicaChannelCommonSelectors.getConnectionServiceName(state, ownProps),
    serviceTypeId:ServicaChannelCommonSelectors.getConnectionServiceTypeId(state, ownProps),
    serviceType:ServicaChannelCommonSelectors.getConnectionServiceType(state, ownProps),
    modified:ServicaChannelCommonSelectors.getConnectionModified(state, ownProps),
    modifiedBy:ServicaChannelCommonSelectors.getConnectionModifiedBy(state, ownProps),
    language: ownProps.language
  }
}

const ConnectName = connect(mapStateToPropsConnection)(connectionNameFormater)
const ConnectType = compose(connect(mapStateToPropsConnection))(connectionTypeFormater)
const ConnectModified = connect(mapStateToPropsConnection)(connectionDateTimeFormater)
const ConnectModifiedBy = connect(mapStateToPropsConnection)(connectionUserFormater)

export const channelServiceTable = ({ messages, language, intl, connections }) => {
  const typeFormatter = (cell, row) => {
    return <ConnectType id={cell} language={language} />
  }

  const dateTimeFormater = (cell, row) => {
    return <ConnectModified id={cell} language={language} />
  }

  const userFormater = (cell, row) => {
    return <ConnectModifiedBy id={cell} language={language} />
  }

  const nameFormatter = (cell, row) => {
    return <ConnectName id={cell} language={language} />
  }

  const ChannelServiceTableColumnsDefinition = [
        { dataField:'id', isKey:true, hidden:true, columnHeaderTitle:'ID' },
        { dataField:'id', dataSort:true, dataFormat:nameFormatter, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderNameTitle) },
        { dataField:'id', dataFormat:typeFormatter, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderTypeTitle) },
        { dataField:'id', dataFormat: dateTimeFormater, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderAttachedTitle) },
        { dataField:'id', dataFormat: userFormater, columnHeaderTitle:intl.formatMessage(messages.serviceTableHeaderAttachedByTitle) }
  ]

  const selectedChannelServiceSelectRowProp = {
    clickToSelect: true,
    className: 'highlighted',
    hideSelectColumn : false
  }

  return (
    <div>
      <PTVTable
        contentDataSlector={ServicaChannelCommonSelectors.getConnections}
        maxHeight='280px'
        data={connections}
        striped hover
        pagination={false}
        language={language}
        selectRow={selectedChannelServiceSelectRowProp}
        columnsDefinition={ChannelServiceTableColumnsDefinition} />
    </div>
  )
}

channelServiceTable.propTypes = {
  messages: PropTypes.object.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    connections: ChannelCommonSelector.getChannelConnections(state, ownProps)
  }
}

const actions = [
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(channelServiceTable))
