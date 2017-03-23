import React, { PropTypes } from 'react'
import {
  Checkbox,
  SearchField,
  TextField,
  Select
} from 'sema-ui-components'
import InfoBox from 'appComponents/InfoBox'

export const renderSearchField = ({
  input,
  label,
  tooltip,
  inlineLabel,
  ...rest
}) => (
  <div>
    { !inlineLabel &&
      <div className='form-field tooltip-label'>
        <strong>
          {label}
          <InfoBox tip={tooltip} />
        </strong>
      </div>
    }
    <SearchField {...rest}
      label={inlineLabel ? label : null}
    >
      <input {...input} />
    </SearchField>
  </div>
)
export const renderCheckBox = ({
  input,
  ...rest
}) => (
  <Checkbox
    checked={!!input.value}
    {...input}
    {...rest}
  />
)
export const renderSelect = ({
  input,
  ...rest
}) => (
  <Select
    {...input}
    {...rest}
  />
)
export const renderTextField = ({
  input,
  label,
  inlineLabel,
  ...rest
}) => (
  <div>
    { !inlineLabel &&
      <div className='form-field'>
        <strong>
          {label}
        </strong>
      </div>
    }
    <TextField
      label={inlineLabel ? label : null}
      {...input}
      {...rest}
    />
  </div>
)
