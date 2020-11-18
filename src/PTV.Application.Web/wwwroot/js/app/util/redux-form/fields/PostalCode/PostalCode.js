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
import { getPathForOtherField, combinePathAndField } from 'util/redux-form/util'
import withValidation from 'util/redux-form/HOC/withValidation'
import { isRequired } from 'util/redux-form/validators'
import { getSuitableStreet } from 'util/redux-form/sections/StreetAddress/actions'
import { getContentLanguageCode } from 'selectors/selections'
import { getSelectedLanguage } from 'Intl/Selectors'
import { EntitySelectors } from 'selectors'

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

  callDispatchChange = (fieldName, value) => {
    const {
      dispatch,
      formName,
      path
    } = this.props
    dispatch(change(formName, combinePathAndField(path, fieldName), value))
  }

  handleChange = postalCode => {
    this.props.onPostalCodeChanged && this.props.onPostalCodeChanged(postalCode)

    if (this.props.addressType !== 'Street') {
      return
    }

    this.callDispatchChange('postalCodeOutsideChange', null)
    this.callDispatchChange('postOffice', postalCode && postalCode.value)
    this.callDispatchChange('municipality', postalCode && postalCode.municipalityId)

    const suitableStreet = getSuitableStreet({ ...this.props, postalCodeId: postalCode && postalCode.value })
    if (suitableStreet) {
      this.callDispatchChange('street', suitableStreet.id)
      this.callDispatchChange('streetName', suitableStreet.streetName)
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
  path: PropTypes.string.isRequired,
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
    (state, ownProps) => {
      const activeFieldPath = getActiveFormField(ownProps.formName)(state)

      return {
        activeFieldPath,
        options: getPostalCodeOptionsJS(state),
        streetPostalCodes: getStreetPostalCodeOptionsJS(state, { id: ownProps.streetId }),
        triggerOnChange: true,
        addressType: ownProps.addressType ||
          formValueSelector(ownProps.formName)(state, getPathForOtherField(activeFieldPath, 'streetType')),
        language: getContentLanguageCode(state, ownProps) || getSelectedLanguage(state),
        streetName: formValueSelector(ownProps.formName)(state, getPathForOtherField(activeFieldPath, 'streetName')),
        streetId: formValueSelector(ownProps.formName)(state, getPathForOtherField(activeFieldPath, 'street')),
        streets: EntitySelectors.streets.getEntities(state, ownProps),
        streetNumbers: EntitySelectors.streetNumbers.getEntities(state, ownProps),
        translatedItems: EntitySelectors.translatedItems.getEntities(state, ownProps)
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
