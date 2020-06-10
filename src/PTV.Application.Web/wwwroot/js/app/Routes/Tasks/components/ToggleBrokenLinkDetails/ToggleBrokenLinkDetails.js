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
import { branch } from 'recompose'
import { injectIntl, intlShape } from 'util/react-intl'
import { getActiveBrokenLinkId } from 'Routes/Tasks/selectors'
import { toggleBrokenLinkDetail } from 'Routes/Tasks/actions'
import { mergeInUIState } from 'reducers/ui'
import ToggleButton from 'appComponents/ToggleButton'
import { commonAppMessages } from 'util/redux-form/messages'
import { taskTypesEnum, formTypesEnum } from 'enums'
import withDirtyFormCheck from 'util/redux-form/HOC/withDirtyFormCheck'

const ToggleBrokenLinkDetails = props => {
  const {
    showToggle = true,
    intl: { formatMessage },
    disabled,
    mergeInUIState,
    id,
    taskType,
    activeId,
    toggleTitle,
    dirtyFormsMap,
    toggleBrokenLinkDetail
  } = props

  const titleMessage = toggleTitle || commonAppMessages.toggleDetailsButtonTitle

  const handleDetailClick = () => {
    const checkedForm = taskType === taskTypesEnum.UNSTABLELINKS
      ? formTypesEnum.UNSTABLELINKFORM
      : formTypesEnum.EXCEPTIONLINKFORM
    dirtyFormsMap.get(checkedForm)
      ? mergeInUIState({
        key: 'clearFormDialog',
        value: {
          isOpen: true,
          confirmAction: () => toggleBrokenLinkDetail(activeId, id, taskType),
          forms: [ checkedForm ]
        }
      })
      : toggleBrokenLinkDetail(activeId, id, taskType)
  }
  const isCollapsed = activeId !== id

  return (
    <ToggleButton
      showToggle={showToggle}
      onClick={handleDetailClick}
      isCollapsed={isCollapsed}
      disabled={disabled}
      buttonProps={{
        link: true
      }}
    >
      {formatMessage(titleMessage)}
    </ToggleButton>
  )
}

ToggleBrokenLinkDetails.propTypes = {
  showToggle: PropTypes.bool,
  intl: intlShape,
  disabled: PropTypes.bool,
  mergeInUIState: PropTypes.func,
  id: PropTypes.string,
  taskType: PropTypes.string,
  activeId: PropTypes.string,
  toggleTitle: PropTypes.object,
  dirtyFormsMap: ImmutablePropTypes.map,
  toggleBrokenLinkDetail: PropTypes.func
}

export default compose(
  injectIntl,
  connect((state, { id, taskType }) => ({
    activeId: getActiveBrokenLinkId(state, { taskType })
  }), {
    mergeInUIState,
    toggleBrokenLinkDetail
  }),
  branch(({ taskType }) => taskType === taskTypesEnum.UNSTABLELINKS,
    withDirtyFormCheck({
      forms: [formTypesEnum.UNSTABLELINKFORM]
    }),
    withDirtyFormCheck({
      forms: [formTypesEnum.EXCEPTIONLINKFORM]
    })
  )
)(ToggleBrokenLinkDetails)
