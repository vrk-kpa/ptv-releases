import React from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'

const AccordionContent = ({
  active,
  isOdd,
  children,
  light,
  className,
  ...rest
}) => {
  const contentClass = cx(
    styles.accordionContent,
    {
      [styles.active]: active,
      [styles.odd]: isOdd,
      [styles.light]: light
    },
    className
  )

  return (
    <div {...rest} className={contentClass}>
      {children}
    </div>
  )
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
  className: PropTypes.string
}

export default AccordionContent
