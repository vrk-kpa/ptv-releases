import React, { PropTypes } from 'react'
import './styles.scss'

const Divider = ({
  componentClass
}) => {
  return (
    <hr
      className={componentClass}
      style={{
        borderTop: '1px solid #d2d2d2',
        borderBottom: '1px solid #ffffff'
      }}
    />
  )
}

Divider.propTypes = {
  componentClass: PropTypes.string
}

export default Divider
