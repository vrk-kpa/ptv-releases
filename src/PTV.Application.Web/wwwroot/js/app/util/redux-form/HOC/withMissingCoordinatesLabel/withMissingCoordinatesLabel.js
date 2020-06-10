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
import {
  getCoordinatesNotFoundForAddress,
  getIsFetchingCoordinates
} from './selectors'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'

const messages = defineMessages({
  notReceived: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.NotReceived.Title',
    defaultMessage: 'No Coordinates for the location based on the address found.'
  }
})

const withMissingCoordinatesLabel = WrappedComponent => {
  const InnerComponent = props => {
    const {
      coordinatesNotFoundForAddress,
      isFetchingCoordinates,
      intl: { formatMessage }
    } = props

    return (
      <div>
        <WrappedComponent {...props} />
        {!isFetchingCoordinates && coordinatesNotFoundForAddress && (
          <div className={styles.labelWarning}>
            <Label labelText={formatMessage(messages.notReceived)} />
          </div>
        )}
      </div>
    )
  }

  InnerComponent.propTypes = {
    coordinatesNotFoundForAddress: PropTypes.bool,
    isFetchingCoordinates: PropTypes.bool,
    intl: intlShape
  }

  return compose(
    injectIntl,
    connect((state, ownProps) => {
      return {
        coordinatesNotFoundForAddress: getCoordinatesNotFoundForAddress(state, ownProps),
        isFetchingCoordinates: getIsFetchingCoordinates(state, ownProps)
      }
    })
  )(InnerComponent)
}

export default withMissingCoordinatesLabel
