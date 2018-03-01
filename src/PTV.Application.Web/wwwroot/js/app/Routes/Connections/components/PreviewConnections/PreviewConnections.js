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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { PreviewConnection } from 'Routes/Connections/components'
import { injectIntl, intlShape } from 'react-intl'
import { getMainEntitiesIds } from 'Routes/Connections/selectors'
import { HeaderFormatter } from 'appComponents'
import CellHeaders from 'appComponents/CellHeaders'
import styles from './styles.scss'

class PreviewConnections extends PureComponent {
  render () {
    const {
      results,
      intl: { formatMessage }
    } = this.props
    return (
      <div>
        <div className={styles.previewHeader}>
          <div className='row'>
            <div className='col-lg-2'>
              <HeaderFormatter label={CellHeaders.languages} />
            </div>
            <div className='col-lg-6'>
              <HeaderFormatter label={CellHeaders.name} />
            </div>
            <div className='col-lg-4'>
              <HeaderFormatter label={CellHeaders.contentType} />
            </div>
            <div className='col-lg-4'>
              <HeaderFormatter label={CellHeaders.connection} />
            </div>
            <div className='col-lg-8'>
              <HeaderFormatter label={CellHeaders.connectionInfo} />
            </div>
          </div>
        </div>
        {results.map(item => (
          <PreviewConnection
            key={item}
            id={item}
          />
        ))}
      </div>
    )
  }
}

export default compose(
  injectIntl,
  connect(state => ({
    results: getMainEntitiesIds(state)
  }))
)(PreviewConnections)

