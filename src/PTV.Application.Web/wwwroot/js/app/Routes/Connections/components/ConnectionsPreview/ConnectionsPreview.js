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
import {
  PreviewActions,
  PreviewConnections
} from 'Routes/Connections/components'
import { defineMessages, injectIntl, intlShape } from 'react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  previewTitle: {
    id: 'Routes.Connections.Components.ConnectionsPreview.Title.Text',
    defaultMessage: 'Yhteenveto'
  },
  previewInfo: {
    id: 'Routes.Connections.Components.ConnectionsPreview.Info.Text',
    defaultMessage: 'Alla on yhteenveto tekemistäsi liitoksista. Voit palata muokkaamaan näitä liitoksia painamalla Muokkaa liitoksia-napista, tai tallentaa liitoksiin tekemäsi muutokset.'
  }
})

class ConnectionsPreview extends PureComponent {
  render () {
    const {
      intl: { formatMessage }
    } = this.props
    return (
      <div className={styles.workbench}>
        <div className={styles.previewHead}>
          <div className='row'>
            <div className='col-lg-18'>
              <h1 className={styles.tabTitle}>{formatMessage(messages.previewTitle)}</h1>
              <p className={styles.tabDescription}>{formatMessage(messages.previewInfo)}</p>
            </div>
            <div className='col-lg-6'>
              <PreviewActions />
            </div>
          </div>
        </div>
        <PreviewConnections />
      </div>
    )
  }
}
ConnectionsPreview.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl
)(ConnectionsPreview)

