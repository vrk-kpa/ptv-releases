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
import HeaderFormatter from 'appComponents/HeaderFormatter'
import Spacer from 'appComponents/Spacer'
import CellHeaders from 'appComponents/Cells/CellHeaders'
import ConnectionsTitle from './ConnectionsTitle'
import { getConnectionsMainEntity, getConnectionsAddToAllEntities } from 'selectors/selections'
import cx from 'classnames'
import styles from './styles.scss'
import { injectIntl } from 'util/react-intl'

const ConnectionsHeader = ({
  mainEntity,
  isBatchAdding
}) => {
  const headingsClass = cx(
    {
      [styles.indented]: !isBatchAdding
    }
  )
  return (
    <div>
      <ConnectionsTitle />
      {mainEntity &&
        <div>
          <Spacer marginSize='m10' />
          <div className={headingsClass}>
            <div className='row'>
              <div className='col-lg-12'>
                <HeaderFormatter label={CellHeaders.name} />
              </div>
              <div className='col-lg-4'>
                <HeaderFormatter label={CellHeaders.contentType} />
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  )
}
ConnectionsHeader.propTypes = {
  mainEntity: PropTypes.oneOf(['channels', 'services']),
  isBatchAdding: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state) => ({
    mainEntity: getConnectionsMainEntity(state),
    isBatchAdding: getConnectionsAddToAllEntities(state)
  }))
)(ConnectionsHeader)
