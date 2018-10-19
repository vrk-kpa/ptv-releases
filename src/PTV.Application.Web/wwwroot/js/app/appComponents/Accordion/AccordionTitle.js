import React, { Component } from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import { getFormSyncErrors } from 'redux-form/immutable'
import { has } from 'lodash'
import CheckMark from './CheckMark'
import { PTVIcon } from 'Components'
import Popup from 'appComponents/Popup'
import ArrowDown from 'appComponents/ArrowDown'
import ArrowUp from 'appComponents/ArrowUp'
import Tooltip from 'appComponents/Tooltip'
import { Label } from 'sema-ui-components'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import withDisclaimerPopup from 'util/redux-form/HOC/withDisclaimerPopup'

const TitleContent = compose(
  branch(({ showDisclaimer }) => showDisclaimer, withDisclaimerPopup)
)(({ title }) => <h5>{title}</h5>)

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
    validate = true,
    dispatch,
    isFirst,
    info,
    draggable,
    draggableProps,
    RemoveButton,
    light,
    className,
    hideTooltipOnScroll,
    dragAndDrop,
    comparable,
    hasMany,
    focused,
    showDisclaimer,
    disclaimerMessage,
    arrowSize = 20
  } = props
  const handleClick = e => {
    e.preventDefault()
    if (onClick) onClick(e, props)
  }
  const titleClass = cx(
    styles.accordionTitle,
    {
      [styles.active]: active,
      [styles.odd]: isOdd,
      [styles.isFirst]: isFirst,
      [styles.light]: light,
      [styles.dragAndDrop]: dragAndDrop,
      [styles.focused]: focused,
      [styles.comparable]: comparable,
      [styles.isCompareMode]: isCompareMode,
      [styles.hasMany]: hasMany
    },
    className
  )
  const readOnlyTitleClass = cx(
    styles.accordionTitle,
    {
      [styles.odd]: isOdd,
      [styles.isFirst]: isFirst,
      [styles.light]: light,
      [styles.dragAndDrop]: dragAndDrop,
      [styles.comparable]: comparable,
      [styles.isCompareMode]: isCompareMode
    },
    styles.readOnly
  )
  if (isReadOnly && dragAndDrop) {
    return <div className={readOnlyTitleClass}>
      <h5>{title}</h5>
    </div>
  }
  return (
    <div className={styles.accordionTitleWrap}>
      <div className={titleClass}>
        <div>
          {formName && validate && <CheckMark valid={valid} />}
          <div onClick={handleClick} className={styles.titleToggleArea}>
            <TitleContent
              showDisclaimer={showDisclaimer}
              disclaimerMessage={disclaimerMessage}
              title={title}
            />
            {active
              ? <ArrowUp height={arrowSize} width={arrowSize} />
              : <ArrowDown height={arrowSize} width={arrowSize} />}

          </div>
          {tooltip && <Tooltip
            tooltip={tooltip}
            hideOnScroll={hideTooltipOnScroll}
            showInReadOnly
            withinText />}
        </div>
        <div className={styles.actions}>
          {draggable && draggableProps &&
            <div {...draggableProps}>
              <PTVIcon name='icon-move' componentClass={styles.moveIcon} />
            </div>
          }
          {RemoveButton}
        </div>
        {info && <div className={styles.info}>{info}</div>}
      </div>
      <div className={styles.accordionTitleContent}>
        {active && helpText &&
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
  title: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  tooltip: PropTypes.any,
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
  dispatch: PropTypes.func,
  isFirst: PropTypes.bool,
  info: PropTypes.any,
  light: PropTypes.bool,
  className: PropTypes.string,
  hideTooltipOnScroll: PropTypes.bool,
  dragAndDrop: PropTypes.bool,
  comparable: PropTypes.bool,
  hasMany: PropTypes.bool,
  focused: PropTypes.bool,
  showDisclaimer: PropTypes.bool,
  disclaimerMessage: PropTypes.string,
  arrowSize: PropTypes.number
}

const _AccordionTitle = compose(
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

if (module.hot) {
  _AccordionTitle.typeName = 'AccordionTitle'
}

export default _AccordionTitle
