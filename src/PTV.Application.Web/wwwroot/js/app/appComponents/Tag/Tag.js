import React from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import { PTVIcon } from 'Components'

const Tag = props => {
  const {
    children,
    isDisabled,
    message,
    isVisible,
    isRemovable,
    onTagRemove,
    bgColor,
    textColor,
    normal
  } = props

  const tagClass = cx(
    styles.tag,
    {
      [styles.disabled]: isDisabled,
      [styles.normal]: normal
    }
  )

  if (!isVisible) return null
  return <div className={tagClass} style={{ backgroundColor: bgColor, color: textColor }}>
    <div>
      {message || children}
    </div>
    {isRemovable &&
      <PTVIcon
        name='icon-cross'
        onClick={onTagRemove}
      />}
  </div>
}

Tag.propTypes = {
  children: PropTypes.any,
  message: PropTypes.string,
  isDisabled: PropTypes.bool,
  isVisible: PropTypes.bool,
  isRemovable: PropTypes.bool,
  onTagRemove: PropTypes.func,
  bgColor: PropTypes.string,
  textColor: PropTypes.string,
  normal: PropTypes.bool
}

Tag.defaultProps = {
  isVisible: true,
  isRemovable: true
}

export default Tag
