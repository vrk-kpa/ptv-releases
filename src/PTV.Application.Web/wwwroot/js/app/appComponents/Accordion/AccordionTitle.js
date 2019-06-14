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
import React, { Component } from 'react'
import PropTypes from 'prop-types'
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import { getFormSyncErrors, getFormSyncWarnings } from 'redux-form/immutable'
import { getIsContentLanguageSet } from 'selectors/selections'
import { Map } from 'immutable'
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
    arrowSize = 20,
    stretchTitle,
    withSearch,
    inActive
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
      [styles.hasMany]: hasMany,
      [styles.stretchTitle]: stretchTitle,
      [styles.inActive]: inActive
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
        <div className={styles.accordionTitleInner}>
          {withSearch &&
            <PTVIcon name='icon-magnifyingGlass' height={26} width={26} componentClass={styles.accordionIcon} />}
          {formName && validate && !withSearch && <CheckMark valid={valid} />}
          <div onClick={!inActive ? handleClick : null} className={styles.titleToggleArea}>
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
          />}
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
  disclaimerMessage: PropTypes.object,
  arrowSize: PropTypes.number,
  stretchTitle: PropTypes.bool,
  withSearch: PropTypes.bool,
  inActive: PropTypes.bool
}

const getValidityStatus = (hasErrors, hasWarnings) => {
  if (hasErrors) return false
  if (hasWarnings) return undefined
  return true
}

const _AccordionTitle = compose(
  withFormStates,
  connect((state, { formName, saveFields, publishFields, isReadOnly }) => {
    const isContentLanguageSet = getIsContentLanguageSet(state)
    const syncErrors = getFormSyncErrors(formName)(state)
    const syncWarnings = getFormSyncWarnings(formName)(state)
    const hasWarnings = Map.isMap(syncWarnings)
      ? publishFields && publishFields.some(field => syncWarnings.has(field))
      : publishFields && publishFields.some(field => has(syncWarnings, field))
    const hasErrors = saveFields && saveFields.some(field => has(syncErrors, field))
    const valid = isContentLanguageSet && !isReadOnly
      ? getValidityStatus(hasErrors, hasWarnings)
      : undefined
    return { valid }
  })
)(AccordionTitle)

if (module.hot) {
  _AccordionTitle.typeName = 'AccordionTitle'
}

export default _AccordionTitle
