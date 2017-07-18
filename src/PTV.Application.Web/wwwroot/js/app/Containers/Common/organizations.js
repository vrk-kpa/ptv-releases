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

// import { PTVAutoComboBox } from '../../Components';
import { LocalizedComboBox } from '../Common/localizedData';
import {connect} from 'react-redux';

///Selectors
import * as CommonOrganizationSelectors from '../Manage/Organizations/Common/Selectors';

export const Organizations = ({value, tooltip, id, labelClass, autoClass, label, placeHolder, name, useFormatMessageData, changeCallback, componentClass, inputProps, validators, readOnly, language, translationMode, organizations}) => {
    return (
      <div>
    <LocalizedComboBox
        value={ value }
        label={ label }
        validatedField={label}
        id= { id }
        tooltip= { tooltip }
        placeholder={ placeHolder }
        name= { name }
        values={ organizations }
        changeCallback= { changeCallback }
        componentClass= { componentClass }
        labelClass={ labelClass }
        autoClass={ autoClass }
        inputProps={ inputProps }
        validators={ validators }
        readOnly= { readOnly || translationMode == "view" || translationMode == "edit" }
        virtualized= { true }
        className = "limited full"
        language={language} />
  </div>
    )
}

Organizations.defaultProps = {
    label: 'Organizations',
    name: 'Organizations',
    useFormatMessageData: true,
    readOnly: false
}

function mapStateToProps(state, ownProps) {
  return {
      organizations: CommonOrganizationSelectors.getOrganizationsObjectArray(state, ownProps),
  }
}

export default connect(mapStateToProps)(Organizations);
