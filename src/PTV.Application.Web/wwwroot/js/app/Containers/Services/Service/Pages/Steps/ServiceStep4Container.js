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
import React, { PropTypes, Component } from 'react'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import { connect } from 'react-redux'
import { compose } from 'redux'
import { Link, browserHistory } from 'react-router'

// actions
import * as relationServiceSearchActions from '../../../../Relations/ServiceAndChannels/ServiceSearch/Actions'
import * as commonServiceAndChannelActions from '../../../../Relations/ServiceAndChannels/Common/Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// components
import { PTVLabel, PTVTable, PTVButton, PTVTooltip } from '../../../../../Components'
import { LocalizedText } from '../../../../Common/localizedData'
import { localizeProps } from 'appComponents/Localize'
import { getDateTimeToDisplay } from '../../../../../Components/PTVDateTimePicker/PTVDateTimePicker'
import LanguageLabel from '../../../../Common/languageLabel'
import TableNameFormatter from '../../../../Common/tableNameFormatter'
import { OwnOrgSecurityCreate, OwnOrgSecurityUpdate } from 'appComponents/Security'

// selectors
import * as ServiceSelectors from '../../Selectors'
import * as ServiceChannelCommonSelectors from '../../../../Relations/ServiceAndChannels/Common/Selectors'

// Messages
import { serviceChannelConnectionsMessages } from '../../Messages'

const connectionDateTimeFormater = ({ modified }) => {
  var dateTime = getDateTimeToDisplay(modified)
  return (
         <PTVTooltip
             labelContent= {dateTime}
             tooltip ={dateTime}
             type= 'special'
             attachToBody
            />
       )
}
const connectionUserFormater = ({ modifiedBy }) => {
  return (
      <PTVTooltip
          labelContent= {modifiedBy}
          tooltip ={modifiedBy || null}
          type ='special'
          attachToBody
        />
    )
}

const connectionTypeFormater = ({ channelTypeId, channelType, language }) => {
  return (
      <LocalizedText
          id={channelTypeId}
          name={channelType}
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
      name:ServiceChannelCommonSelectors.getConnectionChannelName(state, ownProps),
      channelTypeId:ServiceChannelCommonSelectors.getConnectionChannelTypeId(state, ownProps),
      channelType:ServiceChannelCommonSelectors.getConnectionChannelTypeCode(state, ownProps),
      modified:ServiceChannelCommonSelectors.getConnectionModified(state, ownProps),
      modifiedBy:ServiceChannelCommonSelectors.getConnectionModifiedBy(state, ownProps),
      language: ownProps.language
    }
}

const ConnectName = connect(mapStateToPropsConnection)(connectionNameFormater)
const ConnectType = compose(
  connect(mapStateToPropsConnection),
  // localizeProps()
)(connectionTypeFormater)
const ConnectModified = connect(mapStateToPropsConnection)(connectionDateTimeFormater)
const ConnectModifiedBy = connect(mapStateToPropsConnection)(connectionUserFormater)

class ServiceStep4Container extends Component {

  static propTypes = {
      actions: PropTypes.object,
      intl: intlShape.isRequired
    };

  constructor (props) {
      super(props)
      this.nameFormatter = this.nameFormatter.bind(this)
    }

  typeFormatter = (cell, row) => {
      return <ConnectType id={cell} language={this.props.language} />
    }

  dateTimeFormater = (cell, row) => {
      return <ConnectModified id={cell} language={this.props.language} />
    }

  userFormater = (cell, row) => {
      return <ConnectModifiedBy id={cell} language={this.props.language} />
    }

  nameFormatter = (cell, row) => {
      return <ConnectName id={cell} language={this.props.language} />
    }

  handleRelationClick = () => {
      this.props.actions.onServiceSearchListRemove()
      this.props.actions.onChannelSearchListRemove()
      this.props.actions.onServiceAndChannelsListRemove()
      this.props.actions.loadService(this.props.entityId, this.props.language)
      browserHistory.push({ pathname : '/relations', state: { serviceId : this.props.entityId } })
    }

  step4ServiceChannelDataColumnsDefinition = [
        { dataField:'id', isKey:true, hidden:true, columnHeaderTitle:'ID' },
        { dataField:'id', dataSort:true, dataFormat:this.nameFormatter.bind(this), columnHeaderTitle:this.props.intl.formatMessage(serviceChannelConnectionsMessages.resultTableHeaderNameTitle) },
        { dataField:'id', dataFormat:this.typeFormatter.bind(this), columnHeaderTitle:this.props.intl.formatMessage(serviceChannelConnectionsMessages.resultTableHeaderChannelTypeTitle) },
        { dataField:'id', dataFormat: this.dateTimeFormater.bind(this), columnHeaderTitle:this.props.intl.formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxTableHeaderConnectedTitle) },
        { dataField:'id', dataFormat: this.userFormater.bind(this), columnHeaderTitle:this.props.intl.formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxTableHeaderConnectedByTitle) }
    ];

  render () {
      const { formatMessage } = this.props.intl
      const { connections, language, splitContainer, readOnly, translationMode, simpleView } = this.props

      var selectedChannelsSelectRowProp = {
          clickToSelect: true,
          className: 'highlighted',
          hideSelectColumn : false
        }

      const relationDescriptionEdit = simpleView
                                  ? formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxDataDescription, { count:connections.size })
                                : formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxDataDescription, { count:connections.size }) + ' ' + formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxDescriptionEdit)
      const relationDescriptionAdd = simpleView
                                  ? formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxNoDataDescription)
                                : formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxNoDataDescription) + ' ' + formatMessage(serviceChannelConnectionsMessages.attachedChannelsBoxDescriptionAdd)

      return (
           <div className='step-4'>
              <div>
                  <div className='row form-group'>
                      <div className='col-xs-12'>
                          {connections && connections.size > 0
                                ? <div>
                                  <div className='row'>
                                      <div className='col-lg-8'>
                                          <PTVLabel>{ relationDescriptionEdit }</PTVLabel>
                                        </div>
                                      {!simpleView
                                        ? <OwnOrgSecurityUpdate keyToState='service' domain='relations'>
                                          <div className='col-lg-4'>
                                            <PTVButton className='right' onClick={this.handleRelationClick}>
                                                {formatMessage(serviceChannelConnectionsMessages.linkToUpdateRelationTitle)}
                                              </PTVButton>
                                          </div>
                                        </OwnOrgSecurityUpdate>
                                        : null}
                                    </div>
                                  <PTVTable
                                      contentDataSlector={ServiceChannelCommonSelectors.getConnections}
                                      maxHeight='280px'
                                      data={connections}
                                      striped hover
                                      pagination={false}
                                      language={language}
                                      selectRow={selectedChannelsSelectRowProp}
                                      columnsDefinition={this.step4ServiceChannelDataColumnsDefinition} />
                                </div>
                            :                                <div className='row'>
                                  <div className='col-lg-8'>
                                      <PTVLabel>{ relationDescriptionAdd }</PTVLabel>
                                    </div>
                                  {!simpleView
                                    ? <OwnOrgSecurityCreate keyToState='service' domain='relations'>
                                      <div className='col-lg-4'>
                                        <PTVButton className='right' onClick={this.handleRelationClick}>
                                            {formatMessage(serviceChannelConnectionsMessages.linkToAddRelationTitle)}
                                          </PTVButton>
                                      </div>
                                    </OwnOrgSecurityCreate>
                                    : null}
                                </div>
                            }
                        </div>
                    </div>
                </div>
            </div>
         )
    }
}

function mapStateToProps (state, ownProps) {
  return {
    connections: ServiceSelectors.getServiceConnections(state, ownProps)
  }
}

const actions = [
  relationServiceSearchActions,
  commonServiceAndChannelActions
]

export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(ServiceStep4Container))

