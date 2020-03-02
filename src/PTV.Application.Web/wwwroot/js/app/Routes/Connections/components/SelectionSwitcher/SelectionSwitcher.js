/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import { Select } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  getConnectionsAddToAllEntities,
  getConnectionsMainEntity
} from 'selectors/selections'
import { setShouldAddChildToAllEntities } from 'reducers/selections'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import styles from './styles.scss'

const messages = defineMessages({
  label: {
    id: 'Routes.Connections.components.SelectionSwitcher.Title',
    defaultMessage: 'Kohdista uudet liitokset:'
  },
  optionAll: {
    id: 'Routes.Connections.components.SelectionSwitcher.Option.All',
    defaultMessage: 'Koko työpöytään'
  },
  selectedServices: {
    id: 'Routes.Connections.components.SelectionSwitcher.Option.SelectedServices',
    defaultMessage: 'Valittuihin palveluihin'
  },
  selectedChannels: {
    id: 'Routes.Connections.components.SelectionSwitcher.Option.SelectedChannels',
    defaultMessage: 'Valittuihin kanaviin'
  }
})

const SelectionSwitcher = ({
  value,
  onChange,
  intl: { formatMessage },
  selectedMainEntity,
  ...rest
}) => {
  const options = [
    {
      value: true,
      label: formatMessage(messages.optionAll),
      isDefault: true
    },
    {
      value: false,
      label: {
        services: formatMessage(messages.selectedServices),
        channels: formatMessage(messages.selectedChannels)
      }[selectedMainEntity]
    }
  ]
  return (
    <div className={styles.smallSelect}>
      <Select
        options={options}
        value={value}
        onChange={({ value }) => onChange(value)}
        label={formatMessage(messages.label)}
        {...rest}
      />
    </div>
  )
}
SelectionSwitcher.propTypes = {
  onChange: PropTypes.func,
  value: PropTypes.bool,
  intl: intlShape,
  selectedMainEntity: PropTypes.oneOf(['services', 'channels'])
}

export default compose(
  injectIntl,
  connect(state => ({
    value: getConnectionsAddToAllEntities(state),
    selectedMainEntity: getConnectionsMainEntity(state)
  }), {
    onChange: setShouldAddChildToAllEntities
  })
)(SelectionSwitcher)
