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
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import CommonMessages from 'util/redux-form/messages'
import shortId from 'shortid'
import { List, fromJS } from 'immutable'

const messages = defineMessages({

  placeholder : {
    id: 'ReduxForm.Fields.CoordinatesInput.Placeholder',
    defaultMessage: 'enter coordinates'
  },
  tooltip : {
    id: 'ReduxForm.Fields.CoordinatesInput.Tooltip',
    defaultMessage: 'Kirjoita tarkka katuosoite ja tarkka osoitenumero erillisille kentille.'
  }
})
const getCustomValue = (value) => {
  if (value) {
    const coordinate = value.first()
    if (coordinate) {
      const latitude = coordinate.get('latitude')
      const longitude = coordinate.get('longitude')
      if (latitude && longitude) {
        return coordinate.get('latitude') + ', ' + coordinate.get('longitude')
      }

      return coordinate.get('inputValue')
    }
  }
  return ''
}

const CoordinatesInput = ({
  intl: { formatMessage },
  onCoordinatesChange,
  ...rest
}) => {
  const customOnBlur = (input) => (newValue) => {

  }

  const customOnChange = (input) => (newValue) => {
    const re = new RegExp(/^(\-?\d+(\.\d+)?),\s*(\-?\d+(\.\d+)?)$/g)
    const isValueValidCoordinate = re.test(newValue)
    let coordinates = [{
      'isMain': false,
      'inputValue': newValue,
      'coordinateState': isValueValidCoordinate ? 'EnteredByUser' : 'NotOk',
      'id': shortId.generate() }]

    onCoordinatesChange && onCoordinatesChange(coordinates)

    if (isValueValidCoordinate) {
      const coordinateValues = newValue.split(',')
      const coordinate = coordinates[0]
      coordinate.latitude = coordinateValues[0] && coordinateValues[0].trim() || ''
      coordinate.longitude = coordinateValues[1] && coordinateValues[1].trim() || ''
    }

    input.onChange(List(fromJS(coordinates)))
  }

  return <Field
    name='coordinates'
    component={RenderTextField}
    label={formatMessage(CommonMessages.coordinates)}
    placeholder={formatMessage(messages.placeholder)}
    tooltip={formatMessage(messages.tooltip)}
    maxLength={100}
    getCustomValue={getCustomValue}
    customOnBlur={customOnBlur}
    customOnChange={customOnChange}
    {...rest}
  />
}
/* validator regex ^(\-?\d+(\.\d+)?),\s*(\-?\d+(\.\d+)?)$ */
CoordinatesInput.propTypes = {
  intl: intlShape,
  onCoordinatesChange: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  // asLocalizable
)(CoordinatesInput)
