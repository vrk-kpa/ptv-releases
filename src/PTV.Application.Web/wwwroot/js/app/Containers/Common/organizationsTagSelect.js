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
import React, { PropTypes } from 'react'
import { injectIntl } from 'react-intl'
import { connect } from 'react-redux'
import { PTVLabel } from 'Components'

import { getOrganizationsObjectArray } from '../Manage/Organizations/Common/Selectors'

// import { PTVTagSelect } from '../../Components';
import { LocalizedTagSelect } from '../Common/localizedData'

const OrganizationsTagSelect = ({
  intl,
  organizations,
  selectedOrganizations,
  componentClass,
  id,
  label,
  tooltip,
  placeholder,
  changeCallback,
  validators,
  order,
  readOnly,
  translationWarning,
  ...rest }) => {
  const { formatMessage } = intl
  return (
    <div className={componentClass}>
      <LocalizedTagSelect
        {...rest}
        value={selectedOrganizations}
        id={id}
        label={formatMessage(label)}
        validatedField={label}
        tooltip={tooltip && formatMessage(tooltip)}
        placeholder={placeholder && formatMessage(placeholder)}
        options={organizations}
        changeCallback={changeCallback}
        validators={validators}
        order={order}
        readOnly={readOnly}
      />
      {!readOnly && translationWarning &&
        <PTVLabel labelClass='has-error' >
          {formatMessage(translationWarning)}
        </PTVLabel>
      }
    </div>
  )
}

OrganizationsTagSelect.propTypes = {
  selector: PropTypes.func.isRequired,
  keyToState: PropTypes.string.isRequired,
  readOnly: PropTypes.bool.isRequired,
  order: PropTypes.number,
  validators: PropTypes.array,
  changeCallback: PropTypes.func.isRequired,
  placeholder: PropTypes.object,
  tooltip: PropTypes.object,
  label: PropTypes.object,
  id: PropTypes.string,
  componentClass: PropTypes.string,
  selectedOrganizations: PropTypes.array.isRequired,
  organizations: PropTypes.array.isRequired,
  intl: PropTypes.object.isRequired,
  translationWarning: PropTypes.object
}

function mapStateToProps (state, ownProps) {
  return {
    organizations: getOrganizationsObjectArray(state, ownProps),
    selectedOrganizations: ownProps.selector(state, ownProps)
  }
}

export default connect(mapStateToProps)(injectIntl(OrganizationsTagSelect))
