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
import { FieldArray } from 'redux-form/immutable'
import Child from 'Routes/Connections/components/Child'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getConnectionsMainEntity } from 'selectors/selections'
import { Accordion } from 'appComponents/Accordion'
import styles from './styles.scss'

const messages = defineMessages({
  services: {
    id: 'withConnectionStep.channelASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelupisteet ({connectionCount})'
  },
  channels: {
    id: 'withConnectionStep.serviceASTI.title',
    defaultMessage: 'ASTI-järjestelmässä liitetyt palvelut ({connectionCount})'
  }
})

class AstiChilds extends PureComponent {
  renderChilds = ({ fields }) => {
    if (fields.length === 0) return null
    return (
      <div className={styles.children}>
        <Accordion light activeIndex={-1}>
          <Accordion.Title
            validate={false}
            title={this.props.intl.formatMessage(
              messages[this.props.mainEntity],
              { connectionCount: fields.length }
            )}
          />
          <Accordion.Content>
            {fields.map((field, childIndex) => {
              return (
                <div className={styles.subEntityWrap}>
                  <Child
                    key={field}
                    childIndex={childIndex}
                    connectionIndex={this.props.connectionIndex}
                    field={field}
                    isAsti
                  />
                </div>
              )
            })}
          </Accordion.Content>
        </Accordion>
      </div>
    )
  }
  render () {
    return (
      <FieldArray
        name='astiChilds'
        component={this.renderChilds}
      />
    )
  }
}
AstiChilds.propTypes = {
  connectionIndex: PropTypes.number.isRequired,
  intl: intlShape,
  mainEntity: PropTypes.oneOf(['channels', 'services'])
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    mainEntity: getConnectionsMainEntity(state)
  }))
)(AstiChilds)
