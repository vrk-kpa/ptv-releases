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
import React, { Fragment } from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import withState from 'util/withState'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Checkbox } from 'sema-ui-components'
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { Map, OrderedSet } from 'immutable'
import { PTVIcon } from 'Components'

const messages = defineMessages({
  selectAll: {
    id: 'AppComponents.DropdownFilter.SelectAll.Title',
    defaultMessage: 'Kaikki'
  },
  selected: {
    id: 'AppComponents.DropdownFilter.Selected.Title',
    defaultMessage: '{count} valittu'
  },
  placeholder: {
    id: 'AppComponents.DropdownFilter.Placeholder.Title',
    defaultMessage: '- Kirjoita hakusana -'
  }
})

const showValue = (value, firstValue, props) => {
  const handleClearAll = event => {
    event.stopPropagation()
    props.clearAll()
    props.onChange(OrderedSet())
  }
  return props.ShowValueComponent &&
  <Fragment>
    <props.ShowValueComponent
      displayValue={value}
      firstValue={firstValue}
      isFocused={props.isFocused}
      isOpen={props.isOpen}
      filterName={props.filterName}
      placeholder={props.placeholder}
      searchValue={props.searchValue}
    />
    {props.value.size > 0 && <PTVIcon
      name='icon-cross'
      componentClass={styles.clearAll}
      onClick={handleClearAll} />
    }
  </Fragment> ||
  value
}

export class DropdownFilter extends React.Component {
  // state = { isOpen: false }

  componentDidMount () {
    document.addEventListener('mousedown', this.handleClick, false)
  }

  componentWillUnmount () {
    document.removeEventListener('mousedown', this.handleClick, false)
  }

  handleClick = event => {
    const parentNode = this.node.parentNode
    if (this.node === event.target || parentNode.contains(event.target)) {
      // click inside the component
      return
    }
    // click outside the component
    this.handleClickOutside()
  }

  // handleClickOutside = () => this.setState({ isOpen: false })
  handleClickOutside = () => {
    if (this.props.isOpen || this.props.isFocused) {
      this.props.updateUI({
        'isOpen': false,
        'isFocused': false
      })
    }
  }

  // handleTriggerClick = () => this.setState({ isOpen: !this.state.isOpen })
  handleTriggerClick = () => this.props.updateUI({
    'isOpen': !this.props.isOpen,
    'isFocused': true
  })

  handleKeyDown = event => {
    event.stopPropagation()
    switch (event.keyCode) {
      case 9:
        this.handleTab()
        break
      case 32:
        this.handleSpace()
        break
      case 37:
        this.handleArrowLeft()
        break
      case 38:
        this.handleArrowUp()
        break
      case 39:
        this.handleArrowRight()
        break
      case 40:
        this.handleArrowDown()
        break
    }
  }

  handleTab = () => {
    const { updateUI } = this.props
    updateUI('isOpen', false)
    updateUI('isFocused', false)
  }
  handleSpace = () => {
    const { onChange, value, focusedTreeItem } = this.props
    if (!focusedTreeItem) return
    if (value.has(focusedTreeItem)) {
      return onChange(value.delete(focusedTreeItem))
    }
    return onChange(value.add(focusedTreeItem))
  }
  handleArrowLeft = () => {
    if (this.navigationCallback) {
      this.navigationCallback('left')
    }
  }

  handleArrowUp = () => {
    const { focusedTreeItem, updateUI, isOpen } = this.props
    // do nothing when not open
    if (!isOpen) return
    // close when no focused or no options
    if (isOpen && !focusedTreeItem) {
      updateUI('isOpen', false)
    }
    if (!this.setFocus('up')) {
      updateUI('isOpen', false)
    }
  }
  handleArrowRight = () => {
    if (this.navigationCallback) {
      this.navigationCallback('right')
    }
  }

  handleArrowDown = () => {
    const { updateUI, isOpen } = this.props
    // open tree when not open
    if (!isOpen) {
      updateUI('isOpen', true)
    }
    this.setFocus('down')
  }

  focusCallbacks = Map()

  setFocusCallback = (id, focusCallback) => {
    if (typeof focusCallback === 'function') {
      this.focusCallbacks = this.focusCallbacks.set(id || 'root', focusCallback)
    }
  }

  setTreeNaviagationCallback = (focusCallback) => {
    if (typeof focusCallback === 'function') {
      // console.log('navigation registered')
      this.navigationCallback = focusCallback
    } else {
      // console.log('navigation cleared')
      this.navigationCallback = null
    }
  }

  removeFocusCallback = (id, focusCallback) => {
    if (typeof focusCallback === 'function') {
      this.focusCallbacks = this.focusCallbacks.delete(id || 'root')
      if (id) {
        this.props.updateUI('focusedTreeItem', id)
      }
    }
  }

  getFocusItem = (focusedItem, type, skipLevel) => {
    const callBack = this.focusCallbacks.get(focusedItem || 'root')
    let nextFocus = null
    // console.log('search direct callback', focusedItem, callBack, this.focusCallbacks.toJS())
    if (callBack) {
      nextFocus = callBack(focusedItem, type, skipLevel)
      // console.log('by id', focusedItem, nextFocus)
    }

    if (!nextFocus) {
      this.focusCallbacks.forEach(cb => {
        nextFocus = cb(focusedItem, type, skipLevel)
        return !nextFocus
      })
      // console.log('search for all', nextFocus)
    }

    if (nextFocus && !nextFocus.focus) {
      // console.log('go to parent', nextFocus)
      nextFocus = this.getFocusItem(nextFocus.parent, type, true)
      // console.log('found for parent', nextFocus)
    }
    return nextFocus
  }

  setFocus = (type) => {
    // console.log('set focus for', this.props.focusedTreeItem, type)
    const nextFocus = this.getFocusItem(this.props.focusedTreeItem, type)
    // console.log('focus found', nextFocus)
    if (nextFocus && nextFocus.focus && this.props.focusedTreeItem !== nextFocus.focus) {
      this.navigationCallback = null
      this.props.updateUI('focusedTreeItem', nextFocus.focus)
    }
    return nextFocus
  }

  render () {
    const {
      content,
      includeArrow,
      position,
      title,
      // selectAll,
      // selectAllIfEmpty,
      value,
      clearAll,
      formName,
      className,
      ariaMultiSelectable,
      focusedTreeItem,
      searchable,
      searchValue,
      isOpen,
      isFocused,
      triggerSize,
      intl: { formatMessage } } = this.props
    const dropdownWrapClass = cx(
      styles.dropdownWrap,
      className,
      {
        [styles.isOpen]: isOpen,
        [styles.withFocus]: isFocused,
        [styles.withoutFocus]: !isFocused
      }
    )
    const dropdownClass = cx(
      styles.dropdown,
      styles[position],
      {
        [styles.withArrow]: includeArrow
      }
    )
    // const allSelected = selectAllIfEmpty ? value && value.size === 0 : selectAll
    const allSelected = value && value.size === 0
    const valueCount = value && value.size || 0
    const displayValue = allSelected
      ? formatMessage(messages.selectAll)
      : formatMessage(messages.selected, { count: valueCount })

    const handleOnChange = () => {
      if (!allSelected) {
        clearAll(formName)
      }
      // if (!selectAllIfEmpty) {
      //   updateUI('selectAll', !selectAll)
      // }
    }
    const showPlaceholder = searchable && isFocused && !searchValue
    const valueClass = cx(
      styles.value,
      {
        [styles.placeholder]: showPlaceholder
      }
    )
    const triggerClass = cx(
      styles.trigger,
      styles[triggerSize]
    )
    return (
      <div
        className={dropdownWrapClass}
        ref={node => (this.node = node)}
        role='tree'
        aria-activedescendant={focusedTreeItem}
        aria-multiselectable={ariaMultiSelectable}
        // tabIndex={!searchable && '0' || undefined}
        tabIndex={0}
        onKeyDown={this.handleKeyDown}
      >
        <div className={triggerClass} onClick={this.handleTriggerClick}>
          <div className={styles.title}>{title}</div>
          {/* <div className={valueClass}>{
            showPlaceholder
              ? formatMessage(messages.placeholder)
              : searchValue
                ? ''
                : displayValue
          }</div> */}
          <div className={valueClass}>{
            showValue(displayValue, valueCount === 1 && value.first(), this.props)
          }</div>
        </div>
        {isOpen && (
          <div className={dropdownClass}>
            <Checkbox
              className={styles.optionAll}
              checked={allSelected}
              label={formatMessage(messages.selectAll)}
              onChange={handleOnChange}
              // disabled={selectAllIfEmpty && allSelected}
              disabled={allSelected}
            />
            {content(false, { // !selectAllIfEmpty && selectAll, {
              searchValue,
              focusedTreeItem,
              setFocusCallback: this.setFocusCallback,
              removeFocusCallback: this.removeFocusCallback,
              setTreeNaviagationCallback: this.setTreeNaviagationCallback
            })}
          </div>
        )}
      </div>
    )
  }
}

DropdownFilter.propTypes = {
  className: PropTypes.string,
  title: PropTypes.string,
  content: PropTypes.any,
  includeArrow: PropTypes.bool,
  intl: intlShape.isRequired,
  updateUI: PropTypes.func,
  value: PropTypes.any,
  clearAll: PropTypes.func,
  // selectAllIfEmpty: PropTypes.bool,
  formName: PropTypes.string,
  /** Composed from two positions.
        The first one - main - indicates position related to the dropdown trigger.
        The second one - sub - indicates horizontal / vertical position of the
        dropdown based on the main position 'top', 'bottom' / 'left', 'right'.
        'bottomLeft', 'bottomRight', 'topLeft', 'topRight',
        'leftTop', 'leftBottom', 'rightTop', 'rightBottom'
    */
  position: PropTypes.string,
  // selectAll: PropTypes.bool,
  ariaMultiSelectable: PropTypes.bool,
  focusedTreeItem: PropTypes.string,
  searchable: PropTypes.bool,
  searchValue: PropTypes.string,
  isOpen: PropTypes.bool,
  isFocused: PropTypes.bool,
  triggerSize: PropTypes.string
}
DropdownFilter.defaultProps = {
  position: 'bottomRight'
}

export default compose(
  injectIntl,
  injectFormName,
  withState({
    redux: true,
    key: ({ filterName }) => filterName,
    initialState: {
      // selectAll: false,
      focusedTreeItem: null,
      isOpen: false
    }
  })
)(DropdownFilter)
