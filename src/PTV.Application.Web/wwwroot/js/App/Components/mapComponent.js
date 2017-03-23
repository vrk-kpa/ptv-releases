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
import React, { PropTypes } from 'react';
import Channel from 'oskari-rpc';
import shortId from 'shortid';
import ImmutablePropTypes from 'react-immutable-proptypes';

const ADD_MARKER = 'MapModulePlugin.AddMarkerRequest'
let map = null;
let baseCoordiantes = [];
let iframe = null;
let isInit = false;

export const MapComponent = ({coordinates, onMapClick, onMarkerClick}) => {

  const onReadyCallback = () =>
  {
      if (true) {
          showSupportedFunctionality();
      }

      isInit = true;
      addHandlingOfEvents();
      addAllPoints(coordinates, true);
      baseCoordiantes = coordinates;
  }

  const updatePoints = () => {
      if (map != null && map.isReady()) {
          let pointsToAdd = coordinates.filter((point) => {
              return !baseCoordiantes.some((oldPoint) => {
                  return point.get('id') === oldPoint.get('id');
              });
          });

          addAllPoints(pointsToAdd);
          baseCoordiantes = coordinates;
      }
  }

  const addAllPoints = (points, moveToMain) => {
      points.map(point => {
          map.postRequest(ADD_MARKER, [{x: point.get('longtitude'),
            y: point.get('latitude'),
            color: point.get('isMain') ? '6eb005' : "0874aa",
            msg : '',
            shape: 3, // icon number (0-6)
            size: 3}, point.get('id')]);

        if (moveToMain && point.get('isMain')) {
            map.postRequest('MapMoveRequest', [point.get('longtitude'), point.get('latitude'), 11]);
        }
      });
  }

  const showSupportedFunctionality = () => {
      map.getSupportedEvents((supported) => {
          console.log('Available events', supported);
      });

      map.getSupportedRequests((supported) => {
          console.log('Available requests', supported);
      });

      map.getSupportedFunctions((supported) => {
          console.log('Available functions', supported);
      });
      // supported functions can also be detected by
      if (typeof map.getAllLayers === 'function') {
          map.getAllLayers(function(layers) {
              console.log('Available layers', layers);
          });
      }
  }

  const addHandlingOfEvents = () => {
      if (map.isReady()) {
          map.handleEvent('AfterAddMarkerEvent', handleAfterAddMarkerEvent);
          map.handleEvent('AfterMapMoveEvent', handleAfterMapMoveEvent);
          map.handleEvent('DrawingEvent', handleDrawingEvent);
          map.handleEvent('FeatureEvent', handleFeatureEvent);
          map.handleEvent('FeedbackResultEvent', handleFeedbackResultEvent);
          map.handleEvent('MapClickedEvent', handleMapClickedEvent);
          map.handleEvent('MarkerClickEvent', handleMarkerClickEvent);
          map.handleEvent('SearchResultEvent', handleSearchResultEvent);
          map.handleEvent('UserLocationEvent', handleAfterUserLocationEvent);
      }
  }

  const handleAfterAddMarkerEvent = (data) => {

  }

  const handleAfterMapMoveEvent = (data) => {

  }

  const handleDrawingEvent = (data) => {

  }

  const handleFeatureEvent = (data) => {

  }

  const handleFeedbackResultEvent = (data) => {

  }

  let markerClicked = false;
  const handleMapClickedEvent = (data) => {
      if (!markerClicked) {
          const point = {longtitude: data.lon, latitude: data.lat, id: shortId.generate()}
          onMapClick(point);
      }
      else {
          markerClicked = false;
      }
  }

  const handleMarkerClickEvent = (id) => {
      markerClicked = true;
      onMarkerClick(id);
  }

  const handleSearchResultEvent = (data) => {

  }

  const handleAfterUserLocationEvent = (data) => {

  }

  const initMapObject = props => {
      map = Channel.connect(iframe, "http://demo.paikkatietoikkuna.fi");
      map.onReady(onReadyCallback);
  }

  updatePoints();

  return (
    <div>
        <iframe onLoad={initMapObject} style={ {border: 'none', width: '400px', height: '600px'} } ref={(input) => { iframe = input; }} id="map" src="http://demo.paikkatietoikkuna.fi/published/fi/0fcfd236-5079-49f3-81f3-58af23f534d2"></iframe>
    </div>
  );
};

MapComponent.propTypes = {
        coordinates: ImmutablePropTypes.list.isRequired,
        onMapClick: PropTypes.func.isRequired,
        onMarkerClick: PropTypes.func.isRequired,
    };

export default MapComponent;