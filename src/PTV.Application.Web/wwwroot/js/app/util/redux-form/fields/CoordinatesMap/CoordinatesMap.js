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
import { injectIntl, intlShape, defineMessages, FormattedMessage } from 'react-intl'
import { compose } from 'redux'
import { asContainer, withFormStates, asComparable } from 'util/redux-form/HOC'
import MapComponent from 'appComponents/MapComponent'
import { getAddressInfo, getAddressId, getIsAddressValid } from './selectors'
import { connect } from 'react-redux'
import { getSelectedComparisionLanguageCode, getContentLanguageCode } from 'selectors/selections'
import { List, fromJS } from 'immutable'

const messages = defineMessages({
  title: {
    id: 'AddressContainer.OpenMap.Title',
    defaultMessage: 'Näytä kartaa'
  },
  notValid: {
    id: 'AddressContainer.OpenMap.AddressNotValid',
    defaultMessage: 'Täytä kentät: kadunimi, osoitenumero ja postinumero'
  },
  tooltip: {
    id: 'AddressContainer.OpenMap.Info',
    defaultMessage: 'Jos käyntiosoite ei anna kartalla tarkkaa sisäänkäynnin sijaintia, voitluoda sen kartalla'
  }
})

const RenderMap = compose(
  connect((state, ownProps) => ({
    addressInfo: getAddressInfo(state, ownProps),
    language: getContentLanguageCode(state, ownProps),
    comparisionLanguage: getSelectedComparisionLanguageCode(state, ownProps),
    isAddressValid: getIsAddressValid(state, ownProps),
    addressId: getAddressId(state, ownProps)
  })),
  injectIntl,
  withFormStates
)(({
  input,
  intl: { formatMessage },
  language,
  addressInfo,
  addressId,
  isReadOnly,
  comparisionLanguage,
  compare,
  isCompareMode,
  isAddressValid
}) => {
  const isValid = input.value && input.value.some(c => {
    const state = c.get('coordinateState').toLowerCase()
    return state === 'ok' || state === 'enteredbyuser'
  })

  const mainCoordinate = input.value && input.value.filter(c => c.get('isMain'))

  const onMapClick = (data) => {
    data.coordinateState = 'enteredByUser'
    data.isMain = false
    const newCoordinates = mainCoordinate && mainCoordinate.size > 0 && List(mainCoordinate.push(fromJS(data))) ||
      List([fromJS(data)])
    input.onChange(newCoordinates)
  }

  const onResetToDefaultClick = () => {
    input.onChange(List(mainCoordinate))
  }

  return (
    <div>{!isAddressValid && !isValid && formatMessage(messages.notValid) ||
      <MapComponent coordinates={input.value}
        addressInfo={addressInfo}
        onMapClick={onMapClick}
        disabled={isReadOnly}
        mapComponentId={!isCompareMode && 'center' || (compare && 'right' || 'left')}
        contentLanguage={compare && comparisionLanguage || language}
        // isFetching={coordinatesFetching}
        onResetToDefaultClick={onResetToDefaultClick}
        id={addressId} />}</div>
  )
})

const CoordinatesMap = ({
  ...rest
}) => (
  <Field
    name='coordinates'
    component={RenderMap}
    {...rest}
  />
)
CoordinatesMap.propTypes = {
  intl:  intlShape
}

export default compose(
  asContainer({
    title: messages.title,
    tooltip: messages.tooltip
  }),
  asComparable({ DisplayRender: RenderMap }),
)(CoordinatesMap)
