import React, { Component, Children, cloneElement } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { injectIntl } from 'util/react-intl'
import _ from 'lodash'
import withState from 'util/withState'
import styles from './styles.scss'
import AccordionTitle from './AccordionTitle'
import AccordionContent from './AccordionContent'

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
    return (
      <div className={styles.accordion}>
        {this.renderChildren()}
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
  light: PropTypes.bool
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
