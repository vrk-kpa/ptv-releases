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
import { Field, formValueSelector } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asContainer from 'util/redux-form/HOC/asContainer'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import asComparable from 'util/redux-form/HOC/asComparable'
import MapComponent from 'appComponents/MapComponent'
import { connect } from 'react-redux'
import {
  getSelectedComparisionLanguageCode,
  getContentLanguageCode
} from 'selectors/selections'
import { List, fromJS, Map } from 'immutable'
import { EntitySelectors } from 'selectors'
import { setInUIState } from 'reducers/ui'
import { getIsAddressValid, getVisitingAddressFocusIndex } from './selectors'
import { checkAddressTitleValidity } from 'appComponents/AddressTitle/actions'
import { combinePathAndField } from 'util/redux-form/util'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import asDisableable from 'util/redux-form/HOC/asDisableable'

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

const getActiveVisitingAddressPath = (activeIndex) => `visitingAddresses[${activeIndex}]`

const RenderMap = compose(
  injectFormName,
  connect((state, ownProps) => {
    const activeIndex = getVisitingAddressFocusIndex(state, ownProps)
    const activeAddress = ownProps && ownProps.input && ownProps.input.value && ownProps.input.value.get(activeIndex)
    const isDisabledBasedOnType = activeAddress && activeAddress.get('streetType') &&
      activeAddress.get('streetType').toLowerCase() === 'foreign' || false
    const formName = ownProps.formName
    const activeAddressPath = getActiveVisitingAddressPath(activeIndex)
    return {
      language: getContentLanguageCode(state, ownProps),
      comparisionLanguage: getSelectedComparisionLanguageCode(state, ownProps),
      activeIndex,
      isDisabledBasedOnType,
      postalCodes: EntitySelectors.postalCodes.getEntities(state),
      isAddressesValid: getIsAddressValid(state, ownProps),
      municipalities: EntitySelectors.municipalities.getEntities(state),
      postalCodeId: formValueSelector(formName)(state, combinePathAndField(activeAddressPath, 'postalCode')),
      invalidAddress: formValueSelector(formName)(state, combinePathAndField(activeAddressPath, 'invalidAddress')),
      addressType: formValueSelector(formName)(state, combinePathAndField(activeAddressPath, 'streetType'))
    }
  }, {
    setVisitingAddressOpenIndex: ({ focusIndex }) => setInUIState({
      key: 'visitingAddresses',
      path: 'focusIndex',
      value: focusIndex
    })
  }),
  asContainer({
    title: messages.title,
    tooltip: messages.tooltip
  }),
  asComparable({ DisplayRender: RenderMap }),
  injectIntl,
  withFormStates
)(({
  input,
  intl: { formatMessage },
  language,
  isReadOnly,
  isDisabledBasedOnType,
  comparisionLanguage,
  compare,
  isCompareMode,
  activeIndex,
  isAddressesValid,
  postalCodes,
  municipalities,
  setVisitingAddressOpenIndex,
  formName,
  disabled,
  dispatch,
  postalCodeId,
  invalidAddress,
  addressType
}) => {
  const updateCoordinates = (addPoint) => {
    const activeVisitingAddress = input.value.get(activeIndex) || Map()
    const coordinates = activeVisitingAddress && activeVisitingAddress.get('coordinates')
    const mainCoordinate = List.isList(coordinates) && coordinates.filter(c => c.get('isMain')) || List()
    const newCoordinates = addPoint && mainCoordinate.push(fromJS(addPoint)) || mainCoordinate
    const newActiveVisitingAddress = activeVisitingAddress.set('coordinates', newCoordinates)
    const newVisitingAddresses = input.value.set(activeIndex, newActiveVisitingAddress)

    input.onChange(newVisitingAddresses)

    checkAddressTitleValidity({
      invalidAddress,
      addressType,
      dispatch,
      formName,
      coordinates: newCoordinates.toJS(),
      postalCodeId,
      path: getActiveVisitingAddressPath(activeIndex)
    })
  }

  const setActiveAddress = (index) => {
    setVisitingAddressOpenIndex({ focusIndex: index })
  }

  const onMapClick = (data) => {
    data.coordinateState = 'EnteredByUser'
    data.isMain = false
    updateCoordinates(data)
  }

  const onResetToDefaultClick = () => {
    updateCoordinates()
  }

  return (
    <div>{!isAddressesValid && formatMessage(messages.notValid) ||
    <MapComponent addresses={input.value}
      activeIndex={activeIndex}
      onMapClick={onMapClick}
      disabled={disabled || isReadOnly || isDisabledBasedOnType}
      contentLanguage={compare && comparisionLanguage || language}
      postalCodes={postalCodes}
      municipalities={municipalities}
      setActiveAddress={setActiveAddress}
      //     // isFetching={coordinatesFetching}
      onResetToDefaultClick={onResetToDefaultClick}
      id={(!isCompareMode && 'center' || (compare && 'right' || 'left')) + (compare && comparisionLanguage || language)}
    />}</div>
  )
})

const CoordinatesMap = ({
  ...rest
}) => (
  <Field
    name='visitingAddresses'
    component={RenderMap}
    {...rest}
  />
)
CoordinatesMap.propTypes = {
  intl:  intlShape
}

export default compose(
  asDisableable
)(CoordinatesMap)
