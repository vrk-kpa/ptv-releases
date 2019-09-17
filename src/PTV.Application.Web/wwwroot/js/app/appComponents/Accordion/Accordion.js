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
import React, { Component, Children, cloneElement } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
import _ from 'lodash'
import withState from 'util/withState'
import cx from 'classnames'
import styles from './styles.scss'
import AccordionTitle from './AccordionTitle'
import AccordionContent from './AccordionContent'
import { SharedProvider } from 'appComponents/SharedContext'
class Accordion extends Component {
  handleTitleClick = (e, index) => {
    const {
      isExclusive,
      onTitleClick,
      activeIndex
    } = this.props
    let newIndex
    if (isExclusive) {
      newIndex = index === activeIndex
        ? -1
        : index
    } else {
      // check to see if index is in array, and remove it, if not then add it
      newIndex = _.includes(activeIndex, index)
        ? _.without(activeIndex, index)
        : [...activeIndex, index]
    }
    this.props.updateUI({ activeIndex: newIndex })
    if (onTitleClick) {
      onTitleClick(e, index)
    }
  }

  isIndexActive = index => {
    const {
      isExclusive,
      activeIndex
    } = this.props
    return isExclusive
      ? activeIndex === index
      : _.includes(activeIndex, index)
  }

  renderChildren = () => {
    const { children, light } = this.props
    let titleIndex = 0
    let contentIndex = 0
    const { form: formName = null } = this.context._reduxForm || {}
    return Children.map(children, child => {
      // If conditional rendering of a child returns undefined, null or false,
      // skip child rendering
      if (!child) {
        return
      }

      const isTitle = child.type === AccordionTitle ||
        (module.hot && child.type.typeName === 'AccordionTitle')
      const isContent = child.type === AccordionContent ||
        (module.hot && child.type.typeName === 'AccordionContent')
      if (isTitle) {
        const currentIndex = titleIndex
        let isOdd = currentIndex % 2
        let isFirst = currentIndex === 0
        const active = _.has(child, 'props.active')
          ? child.props.active
          : this.isIndexActive(titleIndex)
        const onClick = e => {
          this.handleTitleClick(e, currentIndex)
          if (child.props.onClick) {
            child.props.onClick(e, currentIndex)
          }
        }
        titleIndex++
        return cloneElement(child, {
          ...child.props,
          active,
          isOdd,
          formName,
          onClick,
          isFirst,
          light
        })
      }

      if (isContent) {
        const active = _.has(child, 'props.active')
          ? child.props.active
          : this.isIndexActive(contentIndex)
        let isOdd = contentIndex % 2
        contentIndex++
        return cloneElement(child, {
          ...child.props,
          active,
          isOdd,
          light
        })
      }

      return child
    })
  }

  render () {
    const newChildren = this.renderChildren()
    const accordionClass = cx(
      styles.accordion,
      this.props.className
    )
    return (
      <div className={accordionClass}>
        {newChildren &&
          newChildren.map((child, index) => (
            <SharedProvider
              key={child.props.keyProp || index}
              isAccordionOpen={child.props.active}
            >
              {child}
            </SharedProvider>
          ))
        }
      </div>
    )
  }
}

Accordion.contextTypes = {
  _reduxForm: PropTypes.object
}
Accordion.propTypes = {
  isExclusive: PropTypes.bool,
  onTitleClick: PropTypes.func,
  updateUI: PropTypes.func.isRequired,
  children: PropTypes.node,
  light: PropTypes.bool,
  className: PropTypes.string
}
Accordion.defaultProps = {
  isExclusive: true,
  isValidated: true
}

export const StatelessAccordion = Accordion
StatelessAccordion.Title = AccordionTitle
StatelessAccordion.Content = AccordionContent

const _Accordion = compose(
  withState({
    initialState: ({ activeIndex = 0 }) => ({
      activeIndex
    })
  }),
  injectIntl
)(Accordion)

_Accordion.Title = AccordionTitle
_Accordion.Content = AccordionContent

export default _Accordion

export const ReduxAccordion = compose(
  withState({
    key: ({ reduxKey }) => reduxKey,
    initialState: ({ activeIndex = 0 }) => ({
      activeIndex
    }),
    redux: true
  }),
  injectIntl
)(Accordion)

ReduxAccordion.Title = AccordionTitle
ReduxAccordion.Content = AccordionContent
