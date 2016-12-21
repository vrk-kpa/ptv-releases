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
import { connect } from 'react-redux';
import { injectIntl } from 'react-intl';

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators';
import { PTVTextInput } from '../../../Components';

// selectors
import * as CommonSelectors from '../Selectors';

// schemas
import { CommonSchemas } from '../../Common/Schemas';

// actions
import * as commonActions from '../Actions';
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps';

export const Business = ({businessId, actions, intl, readOnly, language, translationMode, count, startOrder, 
       componentClass, messages, isNew, onAddBusiness, shouldValidate, code }) => {
          
   const onInputChange = input => value => {
        if (!isNew) {
            actions.onEntityInputChange('business', businessId, input, value);
        } else {
            onAddBusiness({
                id: businessId,
                [input]: value
            })
        }
    }    
    
   const validatorsBussiness =[PTVValidatorTypes.IS_BUSINESSID];
   const validators = [PTVValidatorTypes.IS_REQUIRED];
   const { formatMessage } = intl;
   
   return (
            <div>
                <PTVTextInput
                    componentClass="col-lg-6"
                    label={formatMessage(messages.businessIdTitle)}
                    tooltip={formatMessage(messages.businessIdTooltip)}
                    value= {code}
                    blurCallback={onInputChange('code')}
                    name="businessCode"
                    maxLength = {9}
                    validators = { validatorsBussiness }
                    readOnly= { readOnly && translationMode == "none" } 
                    disabled= { translationMode == "view" }
                />                                                                           
            </div>
    );
};

Business.propTypes = {
    businessId: PropTypes.string.isRequired,
    messages: PropTypes.object.isRequired,
    readOnly: PropTypes.bool,
    shouldValidate: PropTypes.bool,
    isNew: PropTypes.bool,
    componentClass: PropTypes.string,
    count: PropTypes.number,    
    onAddBusiness: PropTypes.func,   
};

Business.defaultProps = {
  isNew: false,
  readOnly: false,
  shouldValidate: false
}

function mapStateToProps(state, ownProps) {
  return {
      code: CommonSelectors.getBusinessCode(state, ownProps.businessId)      
  }
}

const actions = [
    commonActions
];

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Business));
