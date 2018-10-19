import React from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'

const AccordionContent = ({
  active,
  isOdd,
  children,
  light,
  dragAndDrop,
  hasMany,
  focused,
  className,
  ...rest
}) => {
  const contentClass = cx(
    styles.accordionContent,
    {
      [styles.active]: active,
      [styles.odd]: isOdd,
      [styles.light]: light,
      [styles.dragAndDrop]: dragAndDrop,
      [styles.hasMany]: hasMany,
      [styles.focused]: focused
    },
    className
  )

  return (
    <div className={contentClass}>
      {children}
    </div>
  )
}
if (module.hot) {
  AccordionContent.typeName = 'AccordionContent'
}

AccordionContent.propTypes = {
  children: PropTypes.any,
  active: PropTypes.bool,
  isOdd: PropTypes.oneOfType([
    PropTypes.bool,
    PropTypes.number
  ]),
  displayName: PropTypes.string,
  light: PropTypes.bool,
  dragAndDrop: PropTypes.bool,
  hasMany: PropTypes.bool,
  focused: PropTypes.bool,
  className: PropTypes.string
}

export default AccordionContent
