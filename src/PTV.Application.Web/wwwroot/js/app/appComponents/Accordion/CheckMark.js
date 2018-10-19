import React from 'react'
import PropTypes from 'prop-types'
import styles from './styles.scss'
import cx from 'classnames'

const CheckMark = ({ valid }) => {
  const statusClass = cx(
    styles.default,
    {
      [styles.complete]: valid,
      [styles.error]: typeof valid !== 'undefined' && !valid
    }
  )
  return <div className={statusClass} />
}

CheckMark.propTypes = {
  valid: PropTypes.bool
}

export default CheckMark
