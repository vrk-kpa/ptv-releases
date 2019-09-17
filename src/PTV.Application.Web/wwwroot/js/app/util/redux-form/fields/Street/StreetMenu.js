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
import React, { createRef } from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import messages from './messages'
import InfiniteScroll from 'react-infinite-scroller'
import { Spinner } from 'sema-ui-components'

class StreetMenu extends React.Component {
  constructor (props) {
    super(props)
    this.menuRef = createRef()
  }

  componentDidMount () {
    document.addEventListener('mousedown', this.handleClick, false)
  }
  componentWillUnmount () {
    document.removeEventListener('mousedown', this.handleClick, false)
  }

  handleClick = event => {
    if (!this.menuRef.current.contains(event.target)) {
      this.props.onClickOutsideMenu()
    }
  }

  scrollToIndex = option => {
    this.refs[option.key].scrollIntoView({ block: 'nearest', behavior: 'smooth' })
  }

  renderOption = (option, isFocused) => {
    const optionClass = cx(styles.option,
      isFocused ? styles.focused : null
    )

    const handleMenuItemClick = () => this.props.selectValue(option)
    const handleMenuItemMouseOver = () => this.props.focusOption(option)

    return (
      <div
        key={option.key}
        ref={option.key}
        className={optionClass}
        onMouseOver={handleMenuItemMouseOver}
        onClick={handleMenuItemClick}>
        <div className='row'>
          <div className='col-sm-9'>
            <span className={styles.street}>{option.label}</span>
          </div>
          <div className='col-sm-6 text-right'>
            <span className={styles.postalCode}>{option.postalCode && option.postalCode.code}</span>
          </div>
          <div className='col-sm-9'>
            <span>{option.postalCode && option.postalCode.postOffice}</span>
          </div>
        </div>
      </div>
    )
  }

  render () {
    const { options, focusedOption, formatMessage, menuClass, moreAvailable, onLoadMore } = this.props

    return (
      <div className={menuClass} ref={this.menuRef}>
        <h5 className={styles.optionHeading}>
          <div className='row'>
            <div className='col-sm-9'><span>{formatMessage(messages.menuStreetName)}</span></div>
            <div className='col-sm-6'><span>{formatMessage(messages.menuPostalCode)}</span></div>
            <div className='col-sm-9'><span>{formatMessage(messages.menuPostOffice)}</span></div>
          </div>
        </h5>
        <InfiniteScroll
          pageStart={0}
          loadMore={onLoadMore}
          hasMore={moreAvailable}
          loader={<div key='street-menu-loader'><Spinner /></div>}
          useWindow={false}
          initialLoad={false}>
          {
            options.map((option, index) => {
              const isFocused = option.key === focusedOption.key
              return this.renderOption(option, isFocused)
            })
          }
        </InfiniteScroll>
      </div>
    )
  }
}

StreetMenu.propTypes = {
  options: PropTypes.array,
  focusedOption: PropTypes.object,
  formatMessage: PropTypes.func,
  menuClass: PropTypes.string,
  focusOption: PropTypes.func,
  selectValue: PropTypes.func,
  moreAvailable: PropTypes.bool,
  onLoadMore: PropTypes.func,
  onClickOutsideMenu: PropTypes.func
}

export default StreetMenu
