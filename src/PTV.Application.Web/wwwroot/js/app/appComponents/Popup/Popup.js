import cx from 'classnames'
import _ from 'lodash'
import React, { Component, PropTypes } from 'react'
import { compose } from 'redux'
import { withStateHandlers } from 'recompose'
import Portal from '../Portal'
import PTVIcon from 'Components/PTVIcon'
import withState from 'util/withState'
import styles from './styles.scss'

export const POSITIONS = [
  'top left',
  'top right',
  'bottom right',
  'bottom left',
  'right center',
  'right default',
  'left center',
  'top center',
  'bottom center'
]

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

  computePopupStyle (positions) {
    const style = { position: 'absolute' }

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

  setPopupStyle () {
    if (!this.coords || !this.popupCoords) return
    let position = this.props.position
    let style = this.computePopupStyle(position)

    // adjust position when out of viewport
    // const positions = _.without(POSITIONS, position)
    // for (let i = 0; !this.isStyleInViewport(style) && i < positions.length; i++) {
    //   style = this.computePopupStyle(positions[i])
    //   position = positions[i]
    // }

    style = _.mapValues(style, value => _.isNumber(value) ? value + 'px' : value)
    this.props.updateUI({ 'style': style, 'position': position })
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
    setTimeout(() => this.props.updateUI('closed', true), 50)
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
    const { onMount } = this.props
    if (onMount) onMount(e, this.props)
  }

  handlePortalUnmount = (e) => {
    const { onUnmount } = this.props
    if (onUnmount) onUnmount(e, this.props)
  }

  handlePopupRef = (popupRef) => {
    this.popupCoords = popupRef ? popupRef.getBoundingClientRect() : null
    this.setPopupStyle()
  }

  handleCloseClick = (e) => {
    // console.log(e.target)
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
      maxWidth,
      style,
      open
    } = this.props

    const classes = cx(
      styles.popup,
      position.split(' ').map(pos => styles[pos]),
      styles[maxWidth],
      className
    )

    if (closed) return trigger

    const unhandled = getUnhandledProps(Popup, this.props)
    const portalPropNames = Portal.handledProps
    const portalProps = _.pick(unhandled, portalPropNames)

    const popupJSX = (
      <div className={classes} style={style} ref={this.handlePopupRef}>
        <div className={styles.popupContent}>{content || children}</div>
        {iconClose && <PTVIcon name='icon-cross' onClick={this.handleCloseClick} componentClass={styles.popupClose} />}
      </div>
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
  content: PropTypes.string,
  hideOnScroll: PropTypes.bool,
  style: PropTypes.object,
  className: PropTypes.string,
  hoverable: PropTypes.bool,
  iconClose: PropTypes.bool,
  on: PropTypes.oneOf(['hover', 'click']),
  onClose: PropTypes.func,
  onMount: PropTypes.func,
  onOpen: PropTypes.func,
  onOpenCallback: PropTypes.func,
  onUnmount: PropTypes.func,
  position: PropTypes.oneOf(POSITIONS),
  trigger: PropTypes.node,
  updateUI: PropTypes.func.isRequired,
  maxWidth: PropTypes.string
}

Popup.defaultProps = {
  on: 'click',
  iconClose: true,
  maxWidth: 'mW300'
}

export default compose(
  // UI state is held in HOC state insted of redux store for performance reasons //
  /*
    withStateHandlers(
      initialState: Object | (props: Object) => any,
      stateUpdaters: {
        [key: string]: (state:Object, props:Object) => (...payload: any[]) => Object
      }
    )
  */
  withState({
    initialState: ({ position }) => ({
      position: position || 'right default',
      style: {},
      closed: false
    })
  })
)(Popup)
