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
import { injectIntl } from 'react-intl';
import {connect} from 'react-redux';

import { getChargeTypesObjectArray, getChargeTypesFilteredObjectArray } from './Selectors';

import { PTVAutoComboBox } from '../../Components';

const ChargeTypeCombo = ({intl, chargeTypes, chargeType, componentClass, id, label, tooltip, changeCallback, validators, order, readOnly}) => {
    const { formatMessage } = intl;
    return (
         <PTVAutoComboBox
              id = { id }
              componentClass={ componentClass }
              value={chargeType}
              values={chargeTypes}
              label={formatMessage(label)}
              tooltip={formatMessage(tooltip)}
              changeCallback={changeCallback}
              name={name}
              validators={validators}
              order={order}
              readOnly={readOnly}
              className="limited w80"
              autosize={false}
              useFormatMessageData={true}
            />
    )
}

function mapStateToProps(state, ownProps) { 
  return {
    chargeTypes: ownProps.filterCode ? getChargeTypesFilteredObjectArray(state, ownProps) : getChargeTypesObjectArray(state, ownProps),
    chargeType: ownProps.selector(state, ownProps)
  }
}

export default connect(mapStateToProps)(injectIntl(ChargeTypeCombo));