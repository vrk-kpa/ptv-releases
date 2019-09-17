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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { FieldArray, arraySplice } from 'redux-form/immutable'
import Child from 'Routes/Connections/components/Child'
import { getChilds, getCount } from './selectors'
import { getConnectionsMainEntity } from 'selectors/selections'
import { Accordion } from 'appComponents/Accordion'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd'
import styles from './styles.scss'
import { getFormSyncErrorWithPath } from 'selectors/base'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import ConnectionsOrderingInfo from '../ConnectionsOrderingInfo'

const messages = defineMessages({
  services: {
    id: 'withConnectionStep.channel.title',
    defaultMessage: 'Liitetyt palvelut ({connectionCount})'
  },
  channels: {
    id: 'withConnectionStep.service.title',
    defaultMessage: 'Liitetyt asiontikanavat ({connectionCount})'
  }
})

class Childs extends PureComponent {
  onDragEnd = ({ source, destination }) => {
    // dropped outside the list //
    if (!destination) {
      return
    }
    const { arraySplice, field, childs } = this.props
    const { index: sourceIndex } = source
    const { index: destinationIndex } = destination

    const destinationChildChannelType = childs
      .getIn([destinationIndex, 'channelType'])
    const sourceChild = childs.get(sourceIndex)
    const sourceChildChannelType = sourceChild.get('channelType')

    const numOfChildsInEachChannelType = childs
      .groupBy(child => child.get('channelType'))
      .map(child => child.size)

    const isAloneInChannelGroup =
      numOfChildsInEachChannelType.get(sourceChildChannelType) === 1
    const droppedOutsideOfAChannelGroup =
      destinationChildChannelType !== sourceChildChannelType

    if (isAloneInChannelGroup || droppedOutsideOfAChannelGroup) {
      return
    }
    // Insert dragged child at new position //
    arraySplice('connectionsWorkbench', field, sourceIndex, 1)
    // Remove dragged child from old position //
    arraySplice('connectionsWorkbench', field, destinationIndex, 0, sourceChild)
  }
  renderChilds = ({ fields }) => {
    return (
      <div className={styles.children}>
        <Accordion light>
          <Accordion.Title
            className={fields.length === 0 ? styles.disabled : ''}
            validate={false}
            title={this.props.intl.formatMessage(
              messages[this.props.mainEntity],
              { connectionCount: fields.length }
            )}
            inActive={this.props.hasError}
          />
          <Accordion.Content>
            <ConnectionsOrderingInfo visible={this.props.connectionIndex === 0} />
            <DragDropContext onDragEnd={this.onDragEnd}>
              <Droppable droppableId='droppable'>
                {(provided, snapshot) => (
                  <div ref={provided.innerRef}>
                    {fields.map((field, childIndex) => {
                      return (
                        <Draggable
                          key={field}
                          draggableId={field}
                        >
                          {(provided, snapshot) => (
                            <div className={styles.subEntityWrap}>
                              <div
                                ref={provided.innerRef}
                                style={provided.draggableStyle}
                              >
                                <Child
                                  childIndex={childIndex}
                                  connectionIndex={this.props.connectionIndex}
                                  field={field}
                                  showAstiOnly={this.props.showAstiOnly}
                                  dragHandleProps={provided.dragHandleProps}
                                />
                              </div>
                              {provided.placeholder}
                            </div>
                          )}
                        </Draggable>
                      )
                    })}
                    {provided.placeholder}
                  </div>
                )}
              </Droppable>
            </DragDropContext>
          </Accordion.Content>
        </Accordion>
      </div>
    )
  }
  render () {
    return (
      <FieldArray
        name='childs'
        component={this.renderChilds}
      />
    )
  }
}
Childs.propTypes = {
  field: PropTypes.string.isRequired,
  arraySplice: PropTypes.func.isRequired,
  childs: PropTypes.object.isRequired,
  connectionIndex: PropTypes.number.isRequired,
  showAstiOnly: PropTypes.bool,
  intl: intlShape,
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  hasError: PropTypes.bool
}

const errorSelector = getFormSyncErrorWithPath(({ connectionIndex }) => {
  return `connections[${connectionIndex}]`
})

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    childs: getChilds(state, ownProps),
    mainEntity: getConnectionsMainEntity(state),
    hasError: ownProps.formName && errorSelector(state, ownProps) || false
  }), {
    arraySplice
  })
)(Childs)
