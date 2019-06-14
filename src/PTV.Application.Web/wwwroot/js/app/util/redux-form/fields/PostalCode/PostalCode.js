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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Field, change, formValues, formValueSelector } from 'redux-form/immutable'
import asDisableable from 'util/redux-form/HOC/asDisableable'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withAccessibilityPrompt from 'util/redux-form/HOC/withAccessibilityPrompt'
import { getActiveFormField } from 'selectors/base'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import {
  getPostalCodeOptionsJS,
  getStreetPostalCodeOptionsJS
} from './selectors'
import styles from './styles.scss'
import PostalCodeAsync from './PostalCodeAsync'
import PostalCodeSync from './PostalCodeSync'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { getPathForOtherField } from 'util/redux-form/util'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isRequired } from 'util/redux-form/validators'

const messages = defineMessages({
  label: {
    id: 'Containers.Channels.Address.PostalCode.Title',
    defaultMessage: 'Postinumero'
  }
})

const PostalCodeOutsideChange = compose(
  injectIntl
)(({ input: { value: message }, intl: { formatMessage } }) => (
  message && <label className={styles.postalCodeOutsideChange}>{formatMessage(message)}</label>
))

class PostalCode extends Component {
  hasPredefinedPostalCodes = () => ((!!this.props.streetPostalCodes &&
    (this.props.streetPostalCodes.length > 0)) ||
    this.props.isReadOnly)

  handleChange = postalCode => {
    this.props.onPostalCodeChanged && this.props.onPostalCodeChanged(postalCode)

    if (this.props.addressType !== 'Street') {
      return
    }

    this.props.dispatch(
      change(
        this.props.formName,
        getPathForOtherField(this.props.activeFieldPath, 'postalCodeOutsideChange'),
        null
      )
    )

    this.props.dispatch(
      change(
        this.props.formName,
        getPathForOtherField(this.props.activeFieldPath, 'postOffice'),
        postalCode && postalCode.value
      )
    )

    if (postalCode && !!postalCode.municipalityId) {
      this.props.dispatch(
        change(
          this.props.formName,
          getPathForOtherField(this.props.activeFieldPath, 'municipality'),
          postalCode.municipalityId
        )
      )
    }
  }

  render () {
    const {
      intl: { formatMessage },
      options,
      ...rest
    } = this.props

    return (
      <div>
        <Field
          component={this.hasPredefinedPostalCodes() ? PostalCodeSync : PostalCodeAsync}
          name='postalCode'
          label={formatMessage(messages.label)}
          onChangeObject={this.handleChange}
          options={this.hasPredefinedPostalCodes() ? this.props.streetPostalCodes : options}
          {...rest}
        />
        <Field
          component={PostalCodeOutsideChange}
          name='postalCodeOutsideChange'
          value={null}
        />
      </div>
    )
  }
}
PostalCode.propTypes = {
  intl: intlShape,
  validate: PropTypes.func,
  dispatch: PropTypes.func,
  onPostalCodeChanged: PropTypes.func,
  activeFieldPath: PropTypes.string.isRequired,
  formName: PropTypes.string.isRequired,
  streetPostalCodes: PropTypes.array,
  required: PropTypes.bool,
  placeholder: PropTypes.string,
  options: ImmutablePropTypes.map,
  isReadOnly: PropTypes.bool,
  addressType: PropTypes.string
}

PostalCodeOutsideChange.propTypes = {
  input: PropTypes.object
}

export default compose(
  injectIntl,
  injectFormName,
  formValues(({ streetIdField }) => ({
    streetId:  streetIdField
  })),
  connect(
    (state, { formName, streetId }) => {
      const activeFieldPath = getActiveFormField(formName)(state)

      return {
        activeFieldPath,
        options: getPostalCodeOptionsJS(state),
        streetPostalCodes: getStreetPostalCodeOptionsJS(state, { id: streetId }),
        triggerOnChange: true,
        addressType: formValueSelector(formName)(state, getPathForOtherField(activeFieldPath, 'streetType'))
      }
    }
  ),
  withValidation({
    label: messages.label,
    validate: isRequired
  }),
  asDisableable,
  injectSelectPlaceholder(),
  withAccessibilityPrompt
)(PostalCode)
