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
import React, { PropTypes } from 'react'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { connect } from 'react-redux'
import MapComponent from 'appComponents/MapComponent'
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import { CommonSchemas } from '../Schemas'
import * as commonActions from '../Actions'
import * as CommonSelectors from '../Selectors'

export const AddressMap = ({
  coordinates,
  actions,
  language,
  readOnly,
  addressInfo,
  positionId,
  translationMode,
  coordinatesFetching,
  id
}) => {
  const onMapClick = (data) => {
    actions.onLocalizedEntityAdd({ id,
      coordinates: [data] },
      CommonSchemas.ADDRESS,
      language)
  }

  const onResetToDefaultClick = (id) => {
    actions.onLocalizedEntityInputChange(
      'addresses', id, 'coordinates', [coordinates.filter(p => p.get('isMain')).first().get('id')], language, true)
  }

  return (
    <div>
      <MapComponent coordinates={coordinates}
        addressInfo={addressInfo}
        onMapClick={onMapClick}
        disabled={readOnly}
        mapComponentId={positionId}
        contentLanguage={language}
        isFetching={coordinatesFetching}
        onResetToDefaultClick={onResetToDefaultClick}
        id={id} />
    </div>
  )
}

AddressMap.propTypes = {
  coordinates: ImmutablePropTypes.map.isRequired,
  language: PropTypes.string.isRequired,
  translationMode: PropTypes.string.isRequired,
  readOnly: PropTypes.bool.isRequired,
  coordinatesFetching: PropTypes.bool.isRequired,
  addressInfo: PropTypes.object.isRequired,
  id: PropTypes.string.isRequired,
  actions: PropTypes.object.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    addressInfo: CommonSelectors.getAddressInfo(state, ownProps),
    coordinates: CommonSelectors.getCoordinatesForAddressId(state, ownProps)
  }
}

export default connect(mapStateToProps, mapDispatchToProps([commonActions]))(AddressMap)
