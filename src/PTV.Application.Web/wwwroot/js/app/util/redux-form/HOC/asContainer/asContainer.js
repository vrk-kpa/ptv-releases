/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { branch } from 'recompose'
import cx from 'classnames'
import styles from './styles.scss'
import { ToggleButton } from 'appComponents/Buttons'
import PreviewData from './PreviewData'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import withPath from 'util/redux-form/HOC/withPath'
import asComparable from 'util/redux-form/HOC/asComparable'
import withNotificationIcon from 'util/redux-form/HOC/withNotificationIcon'
import { injectIntl, defineMessages } from 'util/react-intl'
import withState from 'util/withState'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import {
  createGetHasError,
  createGetData
} from './selectors'

const messages = defineMessages({
  containsError: {
    id: 'Util.ReduxForm.HOC.AsContainer.ContainsError.Tooltip',
    defaultMessage: 'Content must be valid to able to collapse container'
  }
})

const ContainerTitle = compose(
  withNotificationIcon
)(({ label }) => <span>{label}</span>)

const ContainerHead = compose(
  injectIntl,
  branch(({ doNotCompareContainerHead }) => !doNotCompareContainerHead, asComparable({ DisplayRender: ContainerHead }))
)(({
  isReadOnly,
  containerTitle,
  containerTooltip,
  onClick,
  isAddingNewLanguage,
  hasError,
  collapsible,
  isCollapsed,
  formatText,
  nested,
  intl: { formatMessage }
}) => (
  <div className={styles.containerHead}>
    {(isReadOnly || nested) &&
      <div className={styles.containerTitle}>{formatText(containerTitle)}</div> ||
      <ToggleButton
        onClick={onClick}
        disabled={isAddingNewLanguage || hasError}
        isCollapsed={isCollapsed}
        tooltip={formatText(containerTooltip)}
        showIcon={collapsible && !isAddingNewLanguage && !hasError}
      >
        <ContainerTitle
          label={formatText(containerTitle)}
          notificationText={formatText(messages.containsError)}
          shouldShowNotificationIcon={hasError}
          iconClass={styles.notificationIcon}
        />
      </ToggleButton>
    }
  </div>
))

const asContainer = ({ title,
  tooltip,
  light,
  withCollection,
  uiKey,
  contentWhenCollapsed,
  collapsible = true,
  withoutState,
  dataPaths,
  dataFields,
  isParent,
  isLocalized,
  customReadTitle,
  doNotCompareContainerHead,
  overrideReadonly,
  skipErrorCheck,
  renderStaticElement
}) => WrappedComponent => {
  class InnerComponent extends PureComponent {
    static propTypes = {
      className: PropTypes.string,
      isCollapsed: PropTypes.bool,
      isReadOnly: PropTypes.bool.isRequired,
      onChange: withoutState && PropTypes.func.isRequired || PropTypes.func,
      titleFromProps: PropTypes.object,
      tooltipFromProps: PropTypes.any,
      isDisabled: PropTypes.bool,
      hasError: PropTypes.bool,
      isAddingNewLanguage: PropTypes.bool,
      updateUI: !withoutState && PropTypes.func.isRequired || PropTypes.func,
      previewData: PropTypes.any,
      showEmptyLabel: PropTypes.bool,
      withinGroup: PropTypes.bool,
      withinContainer: PropTypes.bool,
      nested: PropTypes.bool
    }

    handleOnToggle = () => {
      if (withoutState && this.props.onChange) {
        this.props.onChange(!this.props.isCollapsed)
      } else {
        this.props.updateUI('isCollapsed', !this.props.isCollapsed)
      }
    }
    WhenCollapsed = contentWhenCollapsed
    render () {
      let {
        isCollapsed,
        isReadOnly,
        titleFromProps,
        tooltipFromProps,
        isAddingNewLanguage,
        hasError,
        previewData,
        withinGroup,
        withinContainer,
        nested,
        ...rest
      } = this.props
      if (!collapsible) {
        isCollapsed = false
      }
      isCollapsed = nested ? false : isCollapsed
      const isReadOnlyComputed = isReadOnly && !overrideReadonly
      const className = cx(
        styles.containerWrap,
        {
          [styles.isCollapsed]: isCollapsed,
          [styles.withCollection]: withCollection,
          [styles.isReadOnly]: isReadOnlyComputed,
          [styles.isAddingNewLanguage]: isAddingNewLanguage,
          [styles.light]: light,
          [styles.customReadTitle]: customReadTitle,
          [styles.withinGroup]: withinGroup,
          [styles.withinContainer]: withinContainer
        },
        this.props.className
      )
      const formatText = text =>
        text &&
        typeof text === 'object' &&
        text.id &&
        rest.intl.formatMessage(text) || text
      const containerTitle = titleFromProps || title
      const containerTooltip = tooltipFromProps || tooltip
      return (
        <div className={className}>
          {containerTitle &&
            <ContainerHead
              isReadOnly={isReadOnlyComputed}
              containerTitle={containerTitle}
              containerTooltip={containerTooltip}
              onClick={this.handleOnToggle}
              isAddingNewLanguage={isAddingNewLanguage}
              hasError={hasError}
              collapsible={collapsible}
              isCollapsed={isCollapsed}
              nested={nested}
              formatText={formatText}
              doNotCompareContainerHead={doNotCompareContainerHead}
            />
          }
          {!isCollapsed
            ? <div className={styles.containerBody}>
              {typeof renderStaticElement === 'function' && renderStaticElement(this.props)}
              <WrappedComponent {...this.props} />
            </div>
            : this.WhenCollapsed && <this.WhenCollapsed {...this.props} /> ||
              isReadOnlyComputed && <div className={styles.containerBody}>
                {typeof renderStaticElement === 'function' && renderStaticElement(this.props)}
                <WrappedComponent {...this.props} />
              </div> ||
              dataPaths && <PreviewData
                data={previewData}
                dataPaths={dataPaths}
              />
          }
        </div>
      )
    }
  }

  const getPreviewData = createGetData(dataPaths)
  const getHasError = createGetHasError(dataPaths)

  const HOCs = [
    injectIntl,
    injectFormName,
    withFormStates,
    withPath,
    connect((state, { title, tooltip, formName, path }) => {
      const previewData = (path || dataPaths) && getPreviewData(state, { formName, path })
      const hasError = !skipErrorCheck && (path || dataPaths) && getHasError(state, { formName, path })
      return {
        titleFromProps: title,
        tooltipFromProps: tooltip,
        isAddingNewLanguage: getIsAddingNewLanguage(formName)(state),
        hasError,
        previewData
      }
    })
  ]
  return (
    withoutState
      ? compose(...HOCs)
      : compose(
        ...HOCs,
        withState({
          redux: !!uiKey,
          key: uiKey,
          initialState: {
            isCollapsed: true
          }
        })
      )
  )(InnerComponent)
}

export default asContainer
