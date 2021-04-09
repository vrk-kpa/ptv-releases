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
import cx from 'classnames'
import _ from 'lodash'
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import Portal from '../Portal'
import PTVIcon from 'Components/PTVIcon'
import withState from 'util/withState'
import styles from './styles.scss'
import IntlProvider from 'Intl/IntlProvider'
export const POSITIONS = [
  'right default',
  'left default',
  'top left',
  'top right',
  'bottom right',
  'bottom left',
  'right center',
  'left center',
  'top center',
  'bottom center'
]
const CENTER_POSITIONS = [
  'top center',
  'bottom center',
  'right center',
  'left center'
]

const POPUP_CHARACTER_COUNT_TRESHOLD = 400

const getUnhandledProps = (Component, props) => {
  // Note that `handledProps` are generated automatically during build with `babel-plugin-transform-react-handled-props`
  const { handledProps = [] } = Component

  return Object.keys(props).reduce((acc, prop) => {
    if (prop === 'childKey') return acc
    if (handledProps.indexOf(prop) === -1) acc[prop] = props[prop]
    return acc
  }, {})
}

class Popup extends Component {
  componentDidUpdate (prevProps) {
    if (
      prevProps.windowWidth !== this.props.windowWidth ||
      prevProps.windowHeight !== this.props.windowHeight
    ) {
      this.coords = this.props.extRef
        ? this.props.extRef.current && this.props.extRef.current.getBoundingClientRect()
        : this.coords
      this.setPopupStyle()
    }
  }

  computePopupStyle (positions) {
    const style = { position: this.props.fixed ? 'fixed' : 'absolute' }

    const { pageYOffset, pageXOffset } = window
    const { clientWidth, clientHeight } = document.documentElement

    if (_.includes(positions, 'right')) {
      style.right = Math.round(clientWidth - (this.coords.right + pageXOffset))
      style.left = 'auto'
      style.top = Math.round(this.coords.top + pageYOffset - 15)
      style.bottom = 'auto'
    } else if (_.includes(positions, 'left')) {
      style.left = Math.round(this.coords.left + pageXOffset)
      style.right = 'auto'
      style.top = Math.round(this.coords.top + pageYOffset - 15)
      style.bottom = 'auto'
    } else {
      const xOffset = (this.coords.width - this.popupCoords.width) / 2
      style.left = Math.round(this.coords.left + xOffset + pageXOffset)
      style.right = 'auto'
    }

    if (_.includes(positions, 'top')) {
      style.bottom = Math.round(clientHeight - (this.coords.top + pageYOffset) + 8)
      style.top = 'auto'
    } else if (_.includes(positions, 'bottom')) {
      style.top = Math.round(this.coords.bottom + pageYOffset + 8)
      style.bottom = 'auto'
    } else {
      if (_.includes(positions, 'center')) {
        const yOffset = (this.coords.height + this.popupCoords.height) / 2
        style.top = Math.round(this.coords.bottom + pageYOffset - yOffset)
        style.bottom = 'auto'
      }

      const xOffset = this.popupCoords.width + 16
      if (_.includes(positions, 'right')) {
        style.right -= xOffset
      } else {
        style.left -= xOffset
      }
    }

    return style
  }

  isStyleInViewport (style) {
    const { pageYOffset, pageXOffset } = window
    const { clientWidth, clientHeight } = document.documentElement

    const element = {
      top: style.top,
      left: style.left,
      width: this.popupCoords.width,
      height: this.popupCoords.height
    }
    if (_.isNumber(style.right)) {
      element.left = clientWidth - style.right - element.width
    }
    if (_.isNumber(style.bottom)) {
      element.top = clientHeight - style.bottom - element.height
    }

    // hidden on top
    if (element.top < pageYOffset) return false
    // hidden on the bottom
    if (element.top + element.height > pageYOffset + clientHeight) return false
    // hidden the left
    if (element.left < pageXOffset) return false
    // hidden on the right
    if (element.left + element.width > pageXOffset + clientWidth) return false

    return true
  }

  getIntegerFromString (stringToParse) {
    return stringToParse.match(/\d+/g).map(Number)[0] || null
  }

  setPopupStyle () {
    if (!this.coords || !this.popupCoords) return
    let isInViewport = false
    let position
    let style = {}
    const {
      dark,
      maxWidth,
      maxHeight,
      updateUI
    } = this.props
    const positions = dark ? CENTER_POSITIONS : POSITIONS
    for (let i = 0; !isInViewport && i < positions.length; i++) {
      position = positions[i]
      style = this.computePopupStyle(position)
      isInViewport = this.isStyleInViewport(style)
    }

    // adjust tooltip if it does not fit in viewport
    if (!isInViewport) {
      const { pageYOffset, pageXOffset } = window
      const { clientWidth, clientHeight } = document.documentElement
      if (this.popupCoords.width > clientWidth) {
        style.left = pageXOffset
        style.right = 'auto'
        style.width = clientWidth
        style.maxWidth = clientWidth
      } else {
        style.left = pageXOffset + (clientWidth - this.popupCoords.width) / 2
        style.right = 'auto'
        style.width = dark ? 'auto' : this.getIntegerFromString(maxWidth)
        style.maxWidth = dark ? 'auto' : this.getIntegerFromString(maxWidth)
      }
      if (this.popupCoords.height > clientHeight) {
        style.top = pageYOffset
        style.bottom = 'auto'
        style.height = clientHeight
      } else {
        style.top = pageYOffset + (clientHeight - this.popupCoords.height) / 2
        style.bottom = 'auto'
        style.height = dark ? 'auto' : this.getIntegerFromString(maxHeight)
      }
      position = 'overlay'
    }

    style = _.mapValues(style, value => _.isNumber(value) ? value + 'px' : value)
    updateUI({ 'style': style, 'position': position })
  }

  getPortalProps () {
    const portalProps = {}

    const { on, hoverable } = this.props

    if (hoverable) {
      portalProps.closeOnPortalMouseLeave = true
      portalProps.mouseLeaveDelay = 300
    }

    if (on === 'click') {
      portalProps.openOnTriggerClick = true
      portalProps.closeOnTriggerClick = true
      portalProps.closeOnDocumentClick = true
    } else if (on === 'hover') {
      portalProps.openOnTriggerMouseEnter = true
      portalProps.closeOnTriggerMouseLeave = true
      portalProps.mouseLeaveDelay = 70
      portalProps.mouseEnterDelay = 50
    }

    return portalProps
  }

  hideOnScroll = (e) => {
    this.props.updateUI('closed', true)
    window.removeEventListener('scroll', this.hideOnScroll)
    setTimeout(() => this.props.updateUI('closed', false), 50)

    this.handleClose(e)
  }

  handleClose = (e) => {
    const { onClose } = this.props
    if (onClose) onClose(e, this.props)
  }

  handleOpen = (e) => {
    this.coords = e.currentTarget.getBoundingClientRect()

    const { onOpen, onOpenCallback } = this.props
    if (onOpenCallback) onOpenCallback()
    if (onOpen) onOpen(e, this.props)
  }

  handlePortalMount = (e) => {
    const { onMount, hideOnScroll } = this.props
    if (hideOnScroll) window.addEventListener('scroll', this.hideOnScroll)
    if (onMount) onMount(e, this.props)
  }

  handlePortalUnmount = (e) => {
    const { onUnmount, hideOnScroll } = this.props
    if (hideOnScroll) window.removeEventListener('scroll', this.hideOnScroll)
    if (onUnmount) onUnmount(e, this.props)
  }

  handlePopupRef = (popupRef) => {
    const { extRef } = this.props
    this.coords = extRef ? extRef.current && extRef.current.getBoundingClientRect() : this.coords
    this.popupCoords = popupRef ? popupRef.getBoundingClientRect() : null
    this.setPopupStyle()
  }

  handleCloseClick = (e) => {
    // console.log(e.target)
  }

  // returns width string based on character count, defaults to maxWidth prop
  getWidthBasedOnContent (content) {
    if (content && typeof content === 'string' && content.length > POPUP_CHARACTER_COUNT_TRESHOLD) {
      return 'mW550'
    }
    return this.props.maxWidth
  }

  render () {
    const {
      children,
      className,
      content,
      trigger,
      iconClose,
      position,
      closed,
      // maxWidth,
      maxHeight,
      style,
      open,
      dark,
      on
    } = this.props

    const classes = cx(
      styles.popup,
      position.split(' ').map(pos => styles[pos]),
      // styles[maxWidth],
      styles[this.getWidthBasedOnContent(content)],
      {
        [styles.dark]: dark
      },
      className
    )

    const contentClass = cx(
      styles.popupContent,
      styles[maxHeight],
      styles[on],
      {
        [styles.pre]: content
      }
    )
    if (closed) return trigger

    const unhandled = getUnhandledProps(Popup, this.props)
    const portalPropNames = Portal.handledProps
    const portalProps = _.pick(unhandled, portalPropNames)
    const Element = content ? 'pre' : 'div'

    const popupJSX = (
      <IntlProvider>
        <div className={classes} style={style} ref={this.handlePopupRef}>
          <Element className={contentClass}>{content || children}</Element>
          {iconClose &&
            <PTVIcon name='icon-cross' onClick={this.handleCloseClick} componentClass={styles.popupClose} />
          }
        </div>
      </IntlProvider>
    )
    const mergedPortalProps = { ...this.getPortalProps(), ...portalProps }
    return (
      <Portal
        {...mergedPortalProps}
        trigger={trigger}
        onClose={this.handleClose}
        onMount={this.handlePortalMount}
        onOpen={this.handleOpen}
        onUnmount={this.handlePortalUnmount}
        open={open}
      >
        {popupJSX}
      </Portal>
    )
  }
}

Popup.propTypes = {
  children: PropTypes.node,
  content: PropTypes.node,
  hideOnScroll: PropTypes.bool,
  style: PropTypes.object,
  className: PropTypes.string,
  closed: PropTypes.bool,
  hoverable: PropTypes.bool,
  iconClose: PropTypes.bool,
  on: PropTypes.oneOf(['hover', 'click']),
  onClose: PropTypes.func,
  onMount: PropTypes.func,
  onOpen: PropTypes.func,
  onOpenCallback: PropTypes.func,
  onUnmount: PropTypes.func,
  open: PropTypes.bool,
  position: PropTypes.oneOf(POSITIONS),
  trigger: PropTypes.node,
  updateUI: PropTypes.func.isRequired,
  maxWidth: PropTypes.string,
  maxHeight: PropTypes.string,
  fixed: PropTypes.bool,
  dark: PropTypes.bool,
  extRef: PropTypes.object,
  windowWidth: PropTypes.number,
  windowHeight: PropTypes.number
}

Popup.defaultProps = {
  on: 'click',
  iconClose: true,
  maxWidth: 'mW300',
  maxHeight: 'mH200'
}

export default compose(
  withState({
    initialState: ({ position }) => ({
      position: position || 'right default',
      style: {},
      closed: false
    })
  })
)(Popup)
