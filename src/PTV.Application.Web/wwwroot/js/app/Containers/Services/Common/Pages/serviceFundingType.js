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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { injectIntl } from 'react-intl';

// components
import { LocalizedComboBox } from '../../../Common/localizedData'

// actions
import mapDispatchToProps from 'Configuration/MapDispatchToProps'
import * as serviceActions from '../../Service/Actions';

// selectors
import * as CommonSelectors from 'Containers/Common/Selectors'
import * as ServiceSelectors from '../../Service/Selectors'

// Validators
import * as PTVValidatorTypes from '../../../../Components/PTVValidators'

const FundingType = ({
    messages,
    readOnly,
    translationMode,
    disabled,
    language,
    fundingTypeId,
    fundingTypes,
    actions,
    intl }) => {

      const validators = [PTVValidatorTypes.IS_REQUIRED, PTVValidatorTypes.IS_REQUIRED_NOT_EMPTY_GUID]
      const onInputChange = (input, isSet=false) => value => {
        actions.onServiceInputChange(input, value, language, isSet);
    }

  return (
    <div className='row form-group'>
      <LocalizedComboBox
        componentClass='col-md-6'
        language={language}
        value={fundingTypeId}
        values={fundingTypes}
        label={intl.formatMessage(messages.title)}
        tooltip={intl.formatMessage(messages.tooltip)}
        validatedField={messages.fundingTypeTitle}
        changeCallback={onInputChange('fundingTypeId')}
        name='FundingType'
        validators={validators}
        autosize={false}
        disabled={disabled}
        readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
      />
    </div>
  )
}

FundingType.propTypes = {
  changeCallback: PropTypes.func.isRequired,
  messages: PropTypes.any.isRequired,
  readOnly: PropTypes.bool.isRequired,
  language: PropTypes.string.isRequired,
  fundingType: PropTypes.string,
  fundingTypes: PropTypes.array.isRequired,
  actions: PropTypes.object.isRequired,
  intl: PropTypes.object.isRequired,
  disabled: PropTypes.bool,
  validators: PropTypes.array
}

function mapStateToProps (state, ownProps) {
  return {
    fundingTypes: CommonSelectors.getFundingTypesObjectArray(state, ownProps),
    fundingTypeId: ServiceSelectors.getFundingTypeId(state, ownProps)
  }
}

const actions = [serviceActions]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(FundingType))

