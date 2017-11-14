import React from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { getFormSyncErrors } from 'redux-form/immutable'
import { has } from 'lodash'
import CheckMark from './CheckMark'
import { PTVIcon } from 'Components'
import { Popup } from 'appComponents'
import { Label } from 'sema-ui-components'
import { withFormStates } from 'util/redux-form/HOC'

const AccordionTitle = props => {
  const {
    active,
    isOdd,
    title,
    tooltip,
    children,
    onClick,
    formName,
    valid,
    isReadOnly,
    helpText,
    // filter out props unusable for div
    isDisabled,
    isCompareMode,
    validateFields,
    dispatch,
    ...rest
  } = props
  const handleClick = e => {
    e.preventDefault()
    if (onClick) onClick(e, props)
  }
  const titleClass = cx(
    styles.accordionTitle,
    {
      [styles.active]: active,
      [styles.odd]: isOdd
    }
  )
  return (
    <div className={styles.accordionTitleWrap}>
      <div {...rest} className={titleClass}>
        {formName && <CheckMark valid={valid} />}
        <div onClick={handleClick} className={styles.titleToggleArea}>
          <h5>{title}</h5>
          <PTVIcon name={active ? 'icon-angle-up' : 'icon-angle-down'} />
        </div>
        {tooltip &&
          <Popup
            trigger={<PTVIcon name='icon-tip2' width={30} height={30} />}
            content={tooltip}
          />
        }
      </div>
      <div className={styles.accordionTitleContent}>
        {!isReadOnly && active && helpText &&
          <div className='row'>
            <div className={isCompareMode ? 'col-lg-12' : 'col-lg-24'}>
              <Label infoLabel labelText={helpText} />
            </div>
          </div>
        }
        {children}
      </div>
    </div>
  )
}

AccordionTitle.propTypes = {
  title: PropTypes.string,
  tooltip: PropTypes.string,
  children: PropTypes.any,
  active: PropTypes.bool,
  isOdd: PropTypes.oneOfType([
    PropTypes.bool,
    PropTypes.number
  ]),
  onClick: PropTypes.func,
  formName: PropTypes.string,
  helpText: PropTypes.string,
  isReadOnly: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  valid: PropTypes.bool,
  validateFields: PropTypes.any,
  dispatch: PropTypes.func
}

export default compose(
  connect((state, { formName, validateFields }) => {
    const syncErrors = getFormSyncErrors(formName)(state)
    const valid = validateFields &&
      !validateFields.some(field => has(syncErrors, field))
    return {
      valid
    }
  }),
  withFormStates
)(AccordionTitle)
