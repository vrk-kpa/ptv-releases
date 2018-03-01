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
import React from 'react'
import PropTypes from 'prop-types'
import { injectIntl } from 'react-intl'
import { connect } from 'react-redux'
import { getMunicipalitiesObjectArray } from './Selectors'
// import { PTVTagSelect } from '../../Components';
import { LocalizedTagSelect } from '../Common/localizedData';

const Municipalities = ({
  intl: { formatMessage },
  municipalities,
  selectedMunicipalities,
  componentClass,
  id,
  label,
  tooltip,
  placeholder,
  changeCallback,
  validators,
  order,
  readOnly,
  ...rest
}) => (
        <LocalizedTagSelect
        {...rest}
    componentClass={componentClass}
    value={selectedMunicipalities}
    id={id}
    label={formatMessage(label)}
    tooltip={formatMessage(tooltip)}
    placeholder={formatMessage(placeholder)}
    options={municipalities}
    changeCallback={changeCallback}
    validators={validators}
    order={order}
    readOnly={readOnly}
    sortAttribute={'name'}
    isSorted
  />
)

Municipalities.propTypes = {
  intl: PropTypes.object,
  municipalities: PropTypes.array,
  selectedMunicipalities: PropTypes.array,
  componentClass: PropTypes.string,
  id: PropTypes.string,
  label: PropTypes.string,
  tooltip: PropTypes.string,
  placeholder: PropTypes.string,
  changeCallback: PropTypes.func,
  validators: PropTypes.array,
  order: PropTypes.string,
  readOnly: PropTypes.bool
}

function mapStateToProps (state, ownProps) {
  return {
    municipalities: getMunicipalitiesObjectArray(state),
    selectedMunicipalities: ownProps.selector(state, ownProps)
  }
}

export default connect(mapStateToProps)(injectIntl(Municipalities))
