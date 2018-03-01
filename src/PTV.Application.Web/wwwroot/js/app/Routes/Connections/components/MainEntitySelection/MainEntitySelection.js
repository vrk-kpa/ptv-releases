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
import { Button } from 'sema-ui-components'
import { connect } from 'react-redux'
import { compose } from 'redux'
import {
  setConnectionsEntity
} from 'reducers/selections'
import { change } from 'redux-form/immutable'
import styles from './styles.scss'

const MainEntitySelection = ({
  setEntity,
  setFormValue
}) => {
  const handleSelectChannels = () => {
    setEntity('channels')
    setFormValue('connectionsWorkbench', 'mainEntityType', 'channels')
  }
  const handleSelectServices = () => {
    setEntity('services')
    setFormValue('connectionsWorkbench', 'mainEntityType', 'services')
  }
  return (
    <div className={styles.buttonGroup}>
      <Button
        small
        secondary
        onClick={handleSelectServices}
        children={<div>Valitse palveluja<br />työpöydälle</div>}
      />
      <Button
        small
        secondary
        onClick={handleSelectChannels}
        children={<div>Valitse asiointikanavia<br />työpöydälle</div>}
      />
    </div>
  )
}
MainEntitySelection.propTypes = {
  setEntity: PropTypes.func.isRequired,
  setFormValue: PropTypes.func.isRequired
}

export default compose(
  connect(null, {
    setEntity: setConnectionsEntity,
    setFormValue: change
  })
)(MainEntitySelection)
