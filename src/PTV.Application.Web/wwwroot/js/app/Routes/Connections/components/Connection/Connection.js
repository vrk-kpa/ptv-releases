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
import { getConnectionsAddToAllEntities } from 'selectors/selections'
import { getIsConnectionEntityActive } from 'Routes/Connections/selectors'
import { Field, FormSection } from 'redux-form/immutable'
import AstiChilds from 'Routes/Connections/components/AstiChilds'
import Childs from 'Routes/Connections/components/Childs'
import MainEntity from 'Routes/Connections/components/MainEntity'
import ActiveEntityCheckbox from 'Routes/Connections/components/ActiveEntityCheckbox'
import cx from 'classnames'
import styles from './styles.scss'
import { injectIntl } from 'util/react-intl'

class Connection extends FormSection {
  static defaultProps = { name: 'connection' }
  render () {
    const { connectionIndex, isBatchAdding, isConnectionEntityActive } = this.props
    const mainEntityWrapClass = cx(
      styles.mainEntityWrap,
      {
        [styles.disabledSelection]: !isBatchAdding && !isConnectionEntityActive,
        [styles.indented]: !isBatchAdding
      }
    )
    return (
      <div className={mainEntityWrapClass}>
        {!isBatchAdding &&
          <div className={styles.mainEntitySelection}>
            <ActiveEntityCheckbox connectionIndex={connectionIndex} />
          </div>
        }
        <div className={styles.mainEntity}>
          <MainEntity connectionIndex={connectionIndex} />
          <Childs
            connectionIndex={connectionIndex}
            field={`${this.props.name}.childs`} />
          <AstiChilds connectionIndex={connectionIndex} />
        </div>
      </div>
    )
  }
}
Connection.propTypes = {
  field: PropTypes.string.isRequired,
  connectionIndex: PropTypes.number.isRequired,
  isBatchAdding: PropTypes.bool,
  isConnectionEntityActive: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, ownProps) => ({
    isBatchAdding: getConnectionsAddToAllEntities(state),
    isConnectionEntityActive: getIsConnectionEntityActive(state, ownProps)
  }))
)(Connection)
