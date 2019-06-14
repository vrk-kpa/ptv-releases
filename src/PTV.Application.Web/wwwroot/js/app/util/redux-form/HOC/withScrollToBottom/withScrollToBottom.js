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
import React, { Component, Fragment } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import withState from 'util/withState'
import styles from './styles.scss'
import { getEntityIsFetching } from 'selectors/entities/entities'
import { debounce } from 'lodash'

const getDocumentHeight = () => {
  return Math.max(
    document.body.scrollHeight, document.documentElement.scrollHeight,
    document.body.offsetHeight, document.documentElement.offsetHeight,
    document.body.clientHeight, document.documentElement.clientHeight
  )
}

const ScrollIndicator = ({ scrollProgress, scrollTop }) => {
  return (
    <div className={styles.scrollIndicator} style={{ width: `${scrollProgress}%`, top: `${scrollTop}px` }} />
  )
}

ScrollIndicator.propTypes = {
  scrollProgress: PropTypes.number,
  scrollTop: PropTypes.number
}

const withScrollToBottom = ({
  uiKey,
  showScrollIndicator
}) => WrappedComponent => {
  class InnerComponent extends Component {
    constructor (props) {
      super(props)
      this.handleScroll = this.handleScroll.bind(this)
      this.handleScrollDebounced = debounce(this.handleScroll, 100)
    }

    componentDidMount = () => {
      const el = this.props.scrollEl || window
      el.addEventListener('scroll', this.handleScrollDebounced)
    }

    componentWillUnmount = () => {
      const el = this.props.scrollEl || window
      el.removeEventListener(event, this.handleScrollDebounced)
    }

    isBottom (el) {
      return (
        el
          ? el.scrollHeight - Math.ceil(el.scrollTop) <= el.clientHeight
          : Math.ceil(window.pageYOffset) + window.innerHeight >= getDocumentHeight()
      )
    }

    getScrollProgress (el) {
      return !this.props.bottomReached && (
        el
          ? (el.scrollTop / (el.scrollHeight - el.clientHeight) * 100).toFixed(2)
          : Math.ceil(window.pageYOffset / (getDocumentHeight() - window.innerHeight) * 100).toFixed(2)
      )
    }

    getScrollTop (el) {
      return el ? el.scrollTop : Math.ceil(window.pageYOffset)
    }

    handleScroll = () => {
      if (showScrollIndicator) {
        this.props.updateUI({ 'scrollTop': this.props.scrollEl.offsetTop || window.pageYOffset })
        this.props.updateUI({ 'scrollProgress': this.getScrollProgress(this.props.scrollEl) })
      }
      if (this.isBottom(this.props.scrollEl)) {
        if (this.props.isScrolledContentLoaded) {
          this.props.updateUI({ 'bottomReached': true })
        } else {
          this.props.updateUI({ 'bottomReached': false })
        }
      }
    }

    render () {
      const {
        bottomReached,
        scrollProgress,
        scrollTop,
        isScrolledContentLoaded,
        ...rest
      } = this.props

      return <Fragment>
        {showScrollIndicator && isScrolledContentLoaded && <ScrollIndicator scrollProgress={scrollProgress} scrollTop={scrollTop} />}
        <WrappedComponent {...rest} bottomReached={bottomReached} />
      </Fragment>
    }
  }

  InnerComponent.propTypes = {
    bottomReached: PropTypes.bool,
    scrollProgress: PropTypes.number,
    scrollTop: PropTypes.number,
    updateUI: PropTypes.func,
    scrollEl: PropTypes.instanceOf(Element),
    isScrolledContentLoaded: PropTypes.bool
  }

  return compose(
    withState({
      redux: !!uiKey,
      key: uiKey,
      initialState: {
        bottomReached: false,
        scrollProgress: 0,
        scrollTop: 0
      }
    }),
    connect(state => ({
      isScrolledContentLoaded: !getEntityIsFetching(state)
    }))
  )(InnerComponent)
}

export default withScrollToBottom
