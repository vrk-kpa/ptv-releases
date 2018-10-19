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
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asComparable from 'util/redux-form/HOC/asComparable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import { Label } from 'sema-ui-components'
import styles from './styles.scss'
import NoDataLabel from 'appComponents/NoDataLabel'

const messages = defineMessages({
  mainTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.Title',
    defaultMessage: 'Osoitteen perusteella haetut sijaintikoordinaatit: {latitude}, {longitude}'
  },
  userTitle: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.UserTitle',
    defaultMessage: 'Käyttäjän määrittelemä karttapisteen sijainti: {userLatitude}, {userLongitude}'
  },
  notReceived: {
    id: 'Containers.Channels.AddPrintableFormChannel.Address.Map.Coordinates.NotReceived.Title',
    defaultMessage: 'Antamaasi osoitetta ei löytynyt.'
  }
})

const RenderLabel = compose(
  injectIntl
)(({
  input,
  required,
  intl: { formatMessage }
}) => {
  if (!input.value) return null
  const mainCoordinate = input.value.filter(coordinate => coordinate.get('isMain')).first()
  const latitude = mainCoordinate && mainCoordinate.get('latitude')
  const longitude = mainCoordinate && mainCoordinate.get('longitude')
  const userCoordinates = input.value.filter(coordinate =>
    coordinate.get('coordinateState').toLowerCase() === 'enteredbyuser' ||
    coordinate.get('coordinateState').toLowerCase() === 'enteredbyar')
  const notReceived = mainCoordinate && mainCoordinate.get('coordinateState').toLowerCase() === 'notreceived' || false
  return (
    <div>
      {input.value && input.value.size > 0 ? notReceived
        ? <div className={styles.labelWarning}><Label labelText={formatMessage(messages.notReceived)} /></div>
        : <div>
          {latitude && longitude ? <div>{formatMessage(messages.mainTitle, { latitude, longitude })}</div> : null}
          {userCoordinates.map(userCoordinate => {
            const userLatitude = userCoordinate.get('latitude')
            const userLongitude = userCoordinate.get('longitude')
            return <div>{formatMessage(messages.userTitle, { userLatitude, userLongitude })}</div>
          })}
        </div>
        : required && <NoDataLabel required={required} /> || null}
    </div>
  )
})
const CoordinatesLabel = ({
  ...rest
}) => (
  <Field
    name='coordinates'
    component={RenderLabel}
    {...rest}
  />
)
CoordinatesLabel.propTypes = {
  intl: intlShape
}

export default compose(
  asComparable({ DisplayRender: RenderLabel }),
  asDisableable
)(CoordinatesLabel)
