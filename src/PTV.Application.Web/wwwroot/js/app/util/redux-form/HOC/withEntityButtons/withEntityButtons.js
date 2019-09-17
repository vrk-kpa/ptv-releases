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
import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { compose } from 'redux'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import CancelDialog from 'appComponents/CancelDialog'
import LanguageTabs from 'util/redux-form/fields/LanguageTabs'
import styles from './styles.scss'
import cx from 'classnames'
import { breakPointsEnum } from 'enums'
import { injectIntl } from 'util/react-intl'
import withState from 'util/withState'
import { withRouter } from 'react-router'
import { WindowDimensionsConsumer } from 'util/redux-form/HOC/withWindowDimensions/context'
import ButtonStrip from './ButtonStrip'
import VisibilitySensor from 'react-visibility-sensor'
import { mergeInUIState } from 'reducers/ui'
import { getLanguagesAvailabilitiesCount } from './selectors'

const withEntityButtons = ({
  formNameToSubmit = '',
  getHasErrorSelector,
  onEdit,
  onCancel
} = {}) => ComposedComponent => {
  const EntityButtons = ({ onCancelSuccess, languagesCount, mergeInUIState, ...props }) => {
    const formName = formNameToSubmit || props.formName
    const handleEntityButtonsVisibilityChange = isVisible => {
      isVisible && mergeInUIState({
        key: 'entityReview',
        value: {
          reviewed: true
        }
      })
    }
    return (
      <WindowDimensionsConsumer>
        {value => {
          const compact = (
            (value.breakPoint === breakPointsEnum.LG && languagesCount >= 4) ||
            (value.breakPoint === breakPointsEnum.MD && languagesCount >= 3) ||
            (value.breakPoint === breakPointsEnum.SM && languagesCount >= 2) ||
            (value.breakPoint === breakPointsEnum.XS)
          )
          const entityNavigationClass = cx(
            styles.entityNavigation,
            {
              [styles.compact]: compact
            }
          )
          return (
            <div>
              <div className={entityNavigationClass}>
                <LanguageTabs
                  getHasErrorSelector={getHasErrorSelector}
                  compact={compact}
                />
                <ButtonStrip {...props} formName={formName} onEdit={onEdit} />
              </div>
              <ComposedComponent {...props} formName={formName} />
              <VisibilitySensor onChange={handleEntityButtonsVisibilityChange}>
                <ButtonStrip inside {...props} formName={formName} onEdit={onEdit} />
              </VisibilitySensor>
              <CancelDialog name={formName} onCancelSuccess={onCancelSuccess} />
            </div>
          )
        }}
      </WindowDimensionsConsumer>
    )
  }

  EntityButtons.propTypes = {
    formName: PropTypes.string,
    isArchived: PropTypes.bool.isRequired,
    onCancelSuccess: PropTypes.func,
    languagesCount: PropTypes.number,
    mergeInUIState: PropTypes.func
  }

  return compose(
    injectFormName,
    injectIntl,
    withFormStates,
    withRouter,
    withState({
      initialState: {
        buttonClicked: 'any'
      }
    }),
    connect((state, { formName }) => ({
      languagesCount: getLanguagesAvailabilitiesCount(state, { formName })
    }), {
      onCancelSuccess: onCancel,
      mergeInUIState
    })
  )(EntityButtons)
}

export default withEntityButtons
