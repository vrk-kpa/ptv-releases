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
import React, { Component, createRef } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
import withState from 'util/withState'
import Trigger from './Trigger'
import List from './List'
import { keyCodes } from 'util/helpers'
import cx from 'classnames'
import styles from './styles.scss'

class ListBox extends Component {
  constructor (props) {
    super(props)
    this.dropdownRef = createRef()
    this.setTriggerRef = element => {
      this.trigger = element
    }
    this.state = {
      focusedOption: props.value
    }
  }

  setHasFocus = value => this.props.updateUI('hasFocus', value)
  setIsOpen = value => {
    if (!this.props.disabled) {
      this.props.updateUI('isOpen', value)
    }
  }
  selectOption = option => {
    const { onChange } = this.props
    this.setFocusedOption(option)
    this.setIsOpen(false)
    onChange(option.value)
    this.trigger && this.trigger.focus()
  }

  handleClickOutsideList = () => {
    this.setHasFocus(false)
  }

  handleTriggerClick = () => {
    const { isOpen } = this.props
    if (!isOpen) {
      this.setIsOpen(true)
    }
    this.setHasFocus(true)
  }

  handleListItemClick = option => {
    this.selectOption(option)
  }

  handleTriggerFocus = () => {
    this.setHasFocus(true)
  }

  handleTriggerBlur = () => {
    this.setHasFocus(false)
  }

  handleListFocus = () => {
    this.setHasFocus(true)
  }

  handleListBlur = () => {
    this.setHasFocus(false)
    this.setIsOpen(false)
  }

  handleTriggerKeyDown = e => {
    e.stopPropagation()
    switch (e.keyCode) {
      case keyCodes.RETURN:
      case keyCodes.SPACE:
        e.preventDefault()
        this.setIsOpen(true)
        break
      case keyCodes.DOWN:
        e.preventDefault()
        this.setIsOpen(true)
        this.focusNextOption(this.state.focusedOption)
        break
      case keyCodes.UP:
        e.preventDefault()
        this.setIsOpen(true)
        this.focusPreviousOption(this.state.focusedOption)
        break
    }
  }

  handleListKeyDown = e => {
    e.stopPropagation()
    switch (e.keyCode) {
      case keyCodes.ESC:
        e.preventDefault()
        this.trigger && this.trigger.focus()
        this.setIsOpen(false)
        break
      case keyCodes.TAB:
        this.trigger && this.trigger.focus()
        this.setIsOpen(false)
        if (e.shiftKey) {
          e.preventDefault()
        } else {
          this.setHasFocus(false)
        }
        break
      case keyCodes.RETURN:
      case keyCodes.SPACE:
        e.preventDefault()
        const { options } = this.props
        const optionIndex = this.getOptionIndex(this.state.focusedOption)
        this.selectOption(options[optionIndex])
        break
      case keyCodes.DOWN:
        e.preventDefault()
        this.focusNextOption(this.state.focusedOption)
        break
      case keyCodes.UP:
        e.preventDefault()
        this.focusPreviousOption(this.state.focusedOption)
        break
      case keyCodes.HOME:
        e.preventDefault()
        this.focusOptionByIndex(0)
        break
      case keyCodes.END:
        e.preventDefault()
        this.focusOptionByIndex(this.props.options.length - 1)
        break
    }
  }

  setFocusedOption = option => {
    this.setState({
      focusedOption: option.value
    })
  }

  getOptionIndex = currentValue => this.props.options.findIndex(option => option.value === currentValue)

  focusNextOption = current => {
    const { options } = this.props
    const optionIndex = this.getOptionIndex(current)
    const newIndex = optionIndex === options.length - 1 ? 0 : optionIndex + 1
    this.setFocusedOption(options[newIndex])
  }

  focusPreviousOption = current => {
    const { options } = this.props
    const optionIndex = this.getOptionIndex(current)
    const newIndex = optionIndex === 0 ? options.length - 1 : optionIndex - 1
    this.setFocusedOption(options[newIndex])
  }

  focusOptionByIndex = index => {
    const { options } = this.props
    this.setFocusedOption(options[index])
  }

  render () {
    const {
      children,
      options,
      isOpen,
      hasFocus,
      value,
      getOptionLabel,
      componentClass,
      triggerClass,
      listClass,
      reduxKey,
      placeholder,
      disabled
    } = this.props
    const containerClass = cx(
      styles.container,
      {
        [styles.isOpen]: isOpen,
        [styles.hasFocus]: hasFocus,
        [styles.noChildren]: !children,
        [styles.placeholder]: !value
      },
      componentClass
    )
    return (
      <div
        className={containerClass}
        ref={this.dropdownRef}
      >
        <Trigger
          triggerRef={this.setTriggerRef}
          onClick={this.handleTriggerClick}
          isOpen={isOpen}
          value={getOptionLabel(value) || placeholder}
          triggerClass={triggerClass}
          id={`${reduxKey}_trigger`}
          onFocus={this.handleTriggerFocus}
          onBlur={this.handleTriggerBlur}
          onKeyDown={this.handleTriggerKeyDown}
          disabled={disabled}
        >
          {children}
        </Trigger>
        {isOpen && (
          <List
            triggerRef={this.triggerRef}
            options={options}
            onListItemClick={this.handleListItemClick}
            selectedValue={value}
            listClass={listClass}
            reduxKey={reduxKey}
            focusedOption={this.state.focusedOption}
            handleListUnmount={this.handleListUnmount}
            onFocus={this.handleListFocus}
            onBlur={this.handleListBlur}
            onKeyDown={this.handleListKeyDown}
            clickOutsideList={this.handleClickOutsideList}
            aria-label={this.props['aria-label']}
            aria-labelledby={this.props['aria-labelledby']}
          />
        )}
      </div>
    )
  }
}

ListBox.propTypes = {
  children: PropTypes.any,
  options: PropTypes.array,
  value: PropTypes.string,
  onChange: PropTypes.func,
  isOpen: PropTypes.bool.isRequired,
  hasFocus: PropTypes.bool.isRequired,
  updateUI: PropTypes.func.isRequired,
  getOptionLabel: PropTypes.func,
  componentClass: PropTypes.string,
  triggerClass: PropTypes.string,
  listClass: PropTypes.string,
  reduxKey: PropTypes.string,
  placeholder: PropTypes.string,
  'aria-label': PropTypes.string,
  'aria-labelledby': PropTypes.string,
  disabled: PropTypes.bool
}

export default compose(
  injectIntl,
  withState({
    redux: true,
    key: ({ reduxKey }) => reduxKey,
    initialState: {
      isOpen: false,
      hasFocus: false
    }
  })
)(ListBox)
