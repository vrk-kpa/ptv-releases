import React from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'

const Cell = props => {
  const {
    children,
    ldWidth,
    dWidth,
    inline
  } = props

  const cellClass = cx(
    styles.cell,
    styles[dWidth],
    styles[ldWidth],
    {
      [styles.inline]: inline
    }
  )

  return <div className={cellClass}>
    {children}
  </div>
}

Cell.propTypes = {
  children: PropTypes.any,
  dWidth: PropTypes.string,
  ldWidth: PropTypes.string,
  inline: PropTypes.bool
}

Cell.defaultProps = {
  dWidth: 'dw200',
  ldWidth: 'ldw300'
}

export default Cell
