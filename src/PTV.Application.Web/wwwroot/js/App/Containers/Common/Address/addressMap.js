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
import React from 'react';
import {connect} from 'react-redux';
import MapComponent from 'Components/mapComponent.js';
import mapDispatchToProps from 'Configuration/MapDispatchToProps';
import { CommonSchemas } from '../Schemas';
import * as commonActions from '../Actions';
import * as CommonSelectors from '../Selectors';

export const AddressMap = ({coordinates, actions, language, id}) => {

  const onMapClick = (data) => {
      actions.onLocalizedEntityAdd({id, coordinates: [data]}, CommonSchemas.ADDRESS, language)
  }

  const onMarkerClick = (id) => {

  }

  return (
    <div>
        <MapComponent coordinates={coordinates} onMapClick={onMapClick} onMarkerClick={onMarkerClick}/>
    </div>
  );
};

function mapStateToProps(state, ownProps) {
    const coordinates = CommonSelectors.getCoordinatesForAddressId(state, ownProps);
  return {
      coordinates
  }
}

export default connect(mapStateToProps, mapDispatchToProps([commonActions]))(AddressMap);