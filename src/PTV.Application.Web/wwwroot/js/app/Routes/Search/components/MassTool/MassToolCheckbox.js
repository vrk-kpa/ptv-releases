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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { RenderCheckBox } from 'util/redux-form/renders'
import { Field, formValues } from 'redux-form/immutable'
import { Map } from 'immutable'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withMassToolActive from './withMassToolActive'
import { getDisabledForRow } from './selectors'
import { massToolTypes } from 'enums'
import { withProps } from 'recompose'

const RenderCheckBoxWithDisablingSameSelected = compose(
  withProps(({ disabled, id, input: { value } }) => ({
    disabled: disabled || !!(value && id !== value.get('id'))
  }))
)(RenderCheckBox)

class Checkbox extends Component {
  getValue = (value) => {
    // Unchecked entities with same unificRootId
    // if (this.props.massToolType === massToolTypes.RESTORE) {
    //   const { id } = this.props
    //   return !!value && id === value.get('id')
    // }
    return !!value
  }

  customOnChange = input => value => {
    const { entityType, subEntityType, unificRootId, id } = this.props
    const approved = true // select all values in summary masstool dialog
    const newValue = value && (input.value || Map({ entityType, subEntityType, unificRootId, id, approved })) || null

    input.onChange(newValue)
  }

  shouldDisableSameSelected = massToolType => (
    massToolType === massToolTypes.RESTORE || massToolType === massToolTypes.PUBLISH
  )

  render () {
    const { id, unificRootId, massToolType, ...rest } = this.props
    return (<Field
      {...rest}
      component={this.shouldDisableSameSelected(massToolType)
        ? RenderCheckBoxWithDisablingSameSelected
        : RenderCheckBox
      }
      name={`selected.${massToolType === massToolTypes.ARCHIVE ? id : unificRootId}`}
      getCustomValue={this.getValue}
      customOnChange={this.customOnChange}
      id={id}
    />)
  }
}

Checkbox.propTypes = {
  id: PropTypes.string.isRequired,
  entityType: PropTypes.string.isRequired,
  subEntityType: PropTypes.string.isRequired,
  unificRootId: PropTypes.string.isRequired,
  massToolType: PropTypes.string.isRequired
}

export default compose(
  withMassToolActive,
  formValues({ massToolType: 'type' }),
  connect(
    (state, ownProps) => ({
      disabled: getDisabledForRow(state, ownProps)
    })
  )
)(Checkbox)
