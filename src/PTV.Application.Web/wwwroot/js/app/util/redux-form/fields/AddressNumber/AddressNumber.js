/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the 'Software'), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED 'AS IS', WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
import React from 'react'
import { connect } from 'react-redux'
import { RenderTextField, RenderTextFieldDisplay } from 'util/redux-form/renders'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import asComparable from 'util/redux-form/HOC/asComparable'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import { Field, change } from 'redux-form/immutable'
import { withProps } from 'recompose'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isRequired } from 'util/redux-form/validators'
import PropTypes from 'prop-types'
import {
  getPostalCodeByStreetNumber,
  getFormPostalCodeId,
  getFormStreetId,
  getStreetNumberRangeId
} from './selectors'
import { getActiveFormField } from 'selectors/base'
import { getPathForOtherField } from 'util/redux-form/util'
import { List } from 'immutable'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.StreetNumber.Title',
    defaultMessage: 'Osoitenumero'
  },
  placeholder : {
    id: 'Containers.Channels.Address.StreetNumber.Placeholder',
    defaultMessage: 'esim. 12 A 23'
  },
  outsideChange: {
    id: 'Containers.Channels.Address.StreetNumber.OutsideChange',
    defaultMessage: 'Postinumero päivitettiin osoitenumeron perusteella.'
  }
})

const AddressNumber = ({
  intl: { formatMessage },
  onBlur,
  onChange,
  onRangeChanged,
  streetId,
  postalCodeId,
  updatedPostalCodeId,
  dispatch,
  activeFieldPath,
  formName,
  streetNumberRangeId,
  ...rest
}) => {
  const handleStreetNumberChange = (args, value) => {
    onChange && onChange(args, value)
    dispatch(change(formName, getPathForOtherField(activeFieldPath, 'coordinates'), List()))
  }

  const handleStreetNumberBlur = (args, value) => {
    onBlur && onBlur(args, value)

    dispatch(change(formName, getPathForOtherField(activeFieldPath, 'streetNumberRange'), streetNumberRangeId))

    onRangeChanged && onRangeChanged({
      streetId,
      postalCodeId: updatedPostalCodeId || postalCodeId,
      streetNumberRange: streetNumberRangeId,
      streetNumber: value
    })

    if (!streetId || !postalCodeId || !updatedPostalCodeId) {
      return
    }

    if (postalCodeId !== updatedPostalCodeId) {
      dispatch(change(formName, getPathForOtherField(activeFieldPath, 'postalCode'), updatedPostalCodeId))
      dispatch(change(formName, getPathForOtherField(activeFieldPath, 'postalCodeOutsideChange'), messages.outsideChange))
    }
  }

  return (
    <div>
      <Field
        name='streetNumber'
        component={RenderTextField}
        label={formatMessage(messages.label)}
        placeholder={formatMessage(messages.placeholder)}
        maxLength={30}
        onBlur={handleStreetNumberBlur}
        onChange={handleStreetNumberChange}
        {...rest}
      />
    </div>
  )
}

AddressNumber.propTypes = {
  intl: intlShape,
  onBlur: PropTypes.func,
  onChange: PropTypes.func,
  streetId: PropTypes.string,
  postalCodeId: PropTypes.string,
  updatedPostalCodeId: PropTypes.string,
  dispatch: PropTypes.func,
  activeFieldPath: PropTypes.string,
  formName: PropTypes.string,
  streetNumberRangeId: PropTypes.string,
  onRangeChanged: PropTypes.func
}

export default compose(
  injectIntl,
  asComparable({ DisplayRender: RenderTextFieldDisplay }),
  asDisableable,
  withProps(props => ({
    triggerOnBlur: true
  })),
  withValidation({
    label: messages.label,
    validate: isRequired
  }),
  withAccessibilityPrompt,
  connect((state, ownProps) => ({
    updatedPostalCodeId: getPostalCodeByStreetNumber(state, ownProps),
    postalCodeId: getFormPostalCodeId(state, ownProps),
    streetId: getFormStreetId(state, ownProps),
    activeFieldPath: getActiveFormField(ownProps.formName)(state),
    streetNumberRangeId: getStreetNumberRangeId(state, ownProps)
  }))
)(AddressNumber)
