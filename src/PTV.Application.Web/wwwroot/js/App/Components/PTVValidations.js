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
import validator from 'validator';
import Promise from 'bluebird';
import {fromJS, is, List, Map, Set} from 'immutable';
import React from 'react';
import * as PTVValidatorTypes from './PTVValidators';

export function valid() {
  return {valid: true, reason: null}
}

export function invalid(reason) {
  return {valid: false, reason}
}

export function IsEmail(value, msg) {
  return (value ==='' || validator.isEmail(value)) ?
    valid() : invalid(msg || 'Please enter a valid email.')
}

export function IsUrl(value, msg) {
  return (value ==='' || validator.isURL(value)) ?
    valid() : invalid(msg || 'Please enter a valid url.')
}

export function IsRequired(value, msg) {
  return (value != null && ((typeof value !== 'object' && typeof value !== 'string') || Object.keys(value).length > 0 )) ?
    valid() : invalid(msg || 'This field is required')
}

export function HasNumber(value, msg) {
  return (value != null && value.match(/.*[0-9]+.*/i) != null) ?
    valid() : invalid(msg || 'This field should contain at least one digit.')
}

export function IsPostalCode(value, msg) {
  return (value != null && value.match(/^[0-9]{5}$/) != null) ?
    valid() : invalid(msg || 'This field should contain a five digit postal code.')
}

export function IsValidDateTime(value, msg) {
  return (value === '' || value == null || (!isNaN(value) && (Number.isInteger(value) || value.match(/^[0-9]$/) != null) )) ?
    valid() : invalid(msg || 'This field should contain a valid date or time.')
}

export function IsBusinessId(value, msg) {
  return (value ==='' || value.match(/^[0-9]{7}-([0-9]){1}$/) != null) ?
    valid() : invalid(msg || 'This field should contain a seven digit, dash and one digit.')
}
export function HasLength(value, min, max, msg, msgNull, msgShort, msgLong) {
  if (value == null) {
    return invalid(msgNull || msg || 'Value cannot be null.')
  }
  if (min != null && value.length < min) {
    return invalid(msgShort || msg || `Length should be at least ${min}.`)
  }
  if (max != null && value.length > max) {
    return invalid(msgLong || msg || `Length should be at most ${max}.`)
  }
  return valid()
}

export function AreSame(value1, value2, msg) {
  return value1 === value2 ?
    valid() : invalid(msg || 'Values have to match.')
}

export function isRequiredAvailable(validationsArray) {
	return (Array.isArray(validationsArray) && validationsArray) ? validationsArray.some((validator) => validator.rule === PTVValidatorTypes.IS_REQUIRED.rule) : false;
}

export function getRequiredLabel(validationsArray, label) {
	return isRequiredAvailable(validationsArray) ? label + " *" : label;
}
