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
import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { getServiceVoucherTypesJS } from "./selectors";
import { compose } from "redux";
import { Field, change } from "redux-form/immutable";
import { defineMessages, injectIntl, intlShape } from "util/react-intl";
import { localizeList } from "appComponents/Localize";
import {
  RenderRadioButtonGroup,
  RenderRadioButtonGroupDisplay,
} from "util/redux-form/renders";
import injectSelectPlaceholder from "appComponents/SelectPlaceholderInjector";
import asDisableable from "util/redux-form/HOC/asDisableable";
import asComparable from "util/redux-form/HOC/asComparable";
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { List, Map } from 'immutable'


const messages = defineMessages({
  title: {
    id: "Containers.Services.AddService.Step1.ServiceType.Title",
    defaultMessage: "Palvelutyyppi",
  },
  tooltip: {
    id: "Containers.Services.AddService.Step1.ServiceType.Tooltip",
    defaultMessage: "Palvelutyyppi",
  },
});

const ServiceVoucherType = ({
  voucherTypes,
  intl: { formatMessage },
  validate,
  isReadOnly,
  change,
  formName,
  ...rest
}) => {
  const Component = isReadOnly
    ? RenderRadioButtonGroupDisplay
    : RenderRadioButtonGroup;

  const handleOnChange = (evt, newValue) => {
   change(formName, 'serviceVouchers.serviceVouchers', Map())
  };

  return (
    <Field
      name="voucherType"
      label={formatMessage(messages.title)}
      tooltip={formatMessage(messages.tooltip)}
      component={Component}
      options={voucherTypes}
      onChange={handleOnChange}
      {...rest}
    />
  );
};
ServiceVoucherType.propTypes = {
  intl: intlShape.isRequired,
  radio: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  nonField: PropTypes.bool.isRequired,
  validate: PropTypes.func,
  voucherTypes: PropTypes.array,
  change: PropTypes.func.isRequired,
  formName: PropTypes.string.isRequired,
};

ServiceVoucherType.defaultProps = {
  nonField: false,
};

export default compose(
  injectIntl,
  injectFormName,
  asDisableable,
  asComparable({ DisplayRender: RenderRadioButtonGroupDisplay }),
  connect((state, { formName })  => ({
    voucherTypes: getServiceVoucherTypesJS(state),
    formName
  }), {
    change
  }),
  localizeList({
    input: "voucherTypes",
    idAttribute: "value",
    nameAttribute: "label",
  }),
  injectSelectPlaceholder()
)(ServiceVoucherType);
