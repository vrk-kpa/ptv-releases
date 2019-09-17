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
import ReactDOM from 'react-dom'
import PropTypes from 'prop-types'
import { throttle } from 'lodash'
import cx from 'classnames'
import styles from './styles.scss'

const portalRoot = document.getElementById('portal-root')

const withPortal = ({ showOverlay, stickyOnScroll, stickyOffset } = {}) => WrappedComponent => {
  class PortalComponent extends React.Component {
    constructor (props) {
      super(props)
      this.el = document.createElement('div')
      this.mountPoint = props.mountPoint || portalRoot
      this.handleScroll = this.handleScroll.bind(this)
      this.handleScrollThrottled = throttle(this.handleScroll)
    }

    componentDidMount () {
      this.mountPoint.appendChild(this.el)
      window.addEventListener('scroll', this.handleScrollThrottled)
    }

    componentWillUnmount () {
      this.mountPoint.removeChild(this.el)
      window.removeEventListener('scroll', this.handleScrollThrottled)
    }

    handleScroll = () => {
      if (!stickyOnScroll) return
      const doc = document.documentElement
      const scrollTop = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0)
      const distanceToEdge = stickyOffset ? stickyOffset - scrollTop : 0
      this.el.style.top = distanceToEdge > 0 ? `${distanceToEdge}px` : `0px`
    }

    render () {
      const { componentClass, isPortalOpen } = this.props
      const portalClass = cx(
        styles.portalComponent,
        {
          [styles.overlay]: showOverlay
        },
        componentClass
      )
      this.el.className = portalClass
      return (
        isPortalOpen
          ? ReactDOM.createPortal(<WrappedComponent {...this.props} />, this.el)
          : <WrappedComponent {...this.props} />
      )
    }
  }

  PortalComponent.propTypes = {
    componentClass: PropTypes.string,
    isPortalOpen: PropTypes.bool,
    mountPoint: PropTypes.instanceOf(Element)
  }

  PortalComponent.defaultProps = {
    isPortalOpen: true
  }

  return PortalComponent
}

export default withPortal
