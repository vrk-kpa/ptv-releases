import React from 'react'
import PropTypes from 'prop-types'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import asLocalizable from 'util/redux-form/HOC/asLocalizable'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
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

  if (isValueValidCoordinate) {
    const coordinateValues = newValue.split(',')
    const coordinate = coordinates[0]
    coordinate.latitude = coordinateValues[0] && coordinateValues[0].trim() || ''
    coordinate.longitude = coordinateValues[1] && coordinateValues[1].trim() || ''
  }

  input.onChange(List(fromJS(coordinates)))
}

const CoordinatesInput = ({
  intl: { formatMessage },
  ...rest
}) => (
  <Field
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
)

/* validator regex ^(\-?\d+(\.\d+)?),\s*(\-?\d+(\.\d+)?)$ */
CoordinatesInput.propTypes = {
  intl: intlShape
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  // asLocalizable
)(CoordinatesInput)
