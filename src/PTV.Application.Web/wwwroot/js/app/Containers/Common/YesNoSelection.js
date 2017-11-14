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
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import { PTVRadioGroup, PTVAutoComboBox } from '../../Components';


const messages = defineMessages({
    yesTitle: {
        id: "Common.Components.Yes.Title",
        defaultMessage: "Kyllä"
        },
    noTitle: {
        id: "Common.Components.No.Title",
        defaultMessage: "Ei"
        },

});

const radioListValues = [{ id: false, message: messages.noTitle }, { id: true, message: messages.yesTitle }];

class YesNoRadioDef extends Component {
    constructor(props){
        super(props);
    }

    static propTypes = {
        name: PropTypes.string,
        value: PropTypes.any.isRequired,
        radioGroupLegend: PropTypes.string,
        tooltip: PropTypes.string,
        onChange: PropTypes.func.isRequired,
	    readOnly: PropTypes.bool
    }

    render (){
        const { name, value, radioGroupLegend, tooltip, onChange, readOnly, splitContainer } = this.props;
        return (
            <PTVRadioGroup
                name={name}
                className={splitContainer ? "col-xs-12" : "col-sm-6"}
                value={value}
                tooltip={tooltip}
                onChange={onChange}
                items={radioListValues}
                radioGroupLegend={radioGroupLegend}
                useFormatMessageData={true}
                readOnly={readOnly} />
        );
    };
};

export const YesNoRadio = injectIntl(YesNoRadioDef);

class YesNoComboDef extends Component {
    constructor(props){
        super(props);
    }

    static propTypes = {
        name: PropTypes.string,
        value: PropTypes.any,
        label: PropTypes.string,
        tooltip: PropTypes.string,
        onChange: PropTypes.func.isRequired,
        order: PropTypes.number,
        validators: PropTypes.array,
        readOnly: PropTypes.bool
    }

    render (){
        const { name, label, tooltip, onChange, order, validators, readOnly, splitContainer, validatedField } = this.props;
        const {formatMessage} = this.props.intl;
        const comboListValues = [{ id: 'true', name: formatMessage(messages.yesTitle) }, { id: 'false', name: formatMessage(messages.noTitle) }];
        const value = this.props.value ? this.props.value.toString() : '';

        return (
            <PTVAutoComboBox
              componentClass={splitContainer ? "col-xs-12" : "col-sm-6 col-md-4"}
              value={value}
              values={comboListValues}
              label={label}
              tooltip={tooltip}
              changeCallback={onChange}
              name={name}
              validators={validators}
              order={order}
              readOnly={readOnly}
              className="limited w80"
              autosize={false}
              validatedField={validatedField}
            />
        );
    };
};

export const YesNoCombo = injectIntl(YesNoComboDef);
