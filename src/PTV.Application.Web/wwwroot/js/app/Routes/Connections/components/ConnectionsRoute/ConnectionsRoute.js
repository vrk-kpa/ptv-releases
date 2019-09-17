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
import withPreviewDialog from 'util/redux-form/HOC/withPreviewDialog'
import SearchArea from 'Routes/Connections/components/SearchArea'
import Workbench from 'Routes/Connections/components/Workbench'
import styles from './styles.scss'
import cx from 'classnames'
import { connect } from 'react-redux'
import { isConnectionsPreview } from 'selectors/selections'

const ConnectionsRoute = ({
  isPreview
}) => {
  const connectionsPageClass = cx(
    styles.form,
    styles.connectionPage
  )
  return (
    <div className={connectionsPageClass}>
      <div className='row'>
        {!isPreview &&
          <div className='col-lg-8'>
            <SearchArea />
          </div>}
        <div className={!isPreview ? 'col-lg-16' : 'col-lg-24'}>
          <Workbench />
        </div>
      </div>
    </div>
  )
}

ConnectionsRoute.propTypes = {
  isPreview: PropTypes.bool
}

export default compose(
  withPreviewDialog,
  connect(state => ({
    isPreview: isConnectionsPreview(state)
  }))
)(ConnectionsRoute)
