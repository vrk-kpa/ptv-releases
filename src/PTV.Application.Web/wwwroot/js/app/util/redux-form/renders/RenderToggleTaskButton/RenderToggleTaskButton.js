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
import React from 'react'
import PropTypes from 'prop-types'
import ImmutablePropTypes from 'react-immutable-proptypes'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, intlShape } from 'util/react-intl'
import { getActiveDetailEntityIds } from 'Routes/Admin/selectors'
import { mergeInUIState } from 'reducers/ui'
import { ToggleButton as ToggleLink } from 'appComponents/Buttons'
import ToggleButton from 'appComponents/ToggleButton'
import { commonAppMessages } from 'util/redux-form/messages'

const RenderToggleTaskButton = props => {
  const {
    showToggle = true,
    intl: { formatMessage },
    disabled,
    mergeInUIState,
    id,
    taskType,
    activeIds,
    toggleTitle,
    asLink
  } = props

  const titleMessage = toggleTitle || commonAppMessages.toggleDetailsButtonTitle

  const handleDetailClick = () => {
    mergeInUIState({
      key: taskType,
      value: {
        activeDetailEntityIds: activeIds.includes(id)
          ? activeIds.filter(currentId => currentId !== id)
          : activeIds.push(id)
      }
    })
  }
  const isCollapsed = !activeIds.includes(id)

  if (asLink) {
    return (
      <ToggleLink
        onClick={handleDetailClick}
        showIcon
        isCollapsed={isCollapsed}
        disabled={disabled}
      >
        {formatMessage(titleMessage)}
      </ToggleLink>
    )
  }

  return (
    <ToggleButton
      showToggle={showToggle}
      onClick={handleDetailClick}
      isCollapsed={isCollapsed}
      disabled={disabled}
      buttonProps={{
        secondary: isCollapsed,
        small: true
      }}
    >
      {formatMessage(titleMessage)}
    </ToggleButton>
  )
}

RenderToggleTaskButton.propTypes = {
  showToggle: PropTypes.bool,
  intl: intlShape,
  disabled: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  id: PropTypes.string,
  taskType: PropTypes.string,
  activeIds: ImmutablePropTypes.list,
  toggleTitle: PropTypes.object,
  asLink: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, { id, taskType }) => ({
    activeIds: getActiveDetailEntityIds(state, { taskType })
  }), {
    mergeInUIState
  })
)(RenderToggleTaskButton)
