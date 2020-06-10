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
import { RenderRadioButtonGroup } from 'util/redux-form/renders'
import { Field } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { clearSelection } from 'Routes/Search/components/MassTool/actions'

const messages = defineMessages({
  publish: {
    id: 'MassTool.Options.Publish',
    defaultMessage: 'Julkaise',
    description: 'Components.Buttons.PublishButton'
  },
  archive: {
    id: 'MassTool.Options.Archive',
    defaultMessage: 'Arkistoi',
    description: 'Components.Buttons.ArchiveButton'
  },
  copy: {
    id: 'MassTool.Options.Copy',
    defaultMessage: 'Kopioi',
    description: 'Components.Buttons.CopyButton'
  },
  restore: {
    id: 'MassTool.Options.Restore',
    defaultMessage: 'Palauta arkistosta',
    description: 'Components.Buttons.RestoreButton'
  },
  createTaskList: {
    id: 'MassTool.Options.CreateTaskList',
    defaultMessage: 'Luo tehtävä',
    description: { en: 'Create a task list' }
  }
})

let types = {
  publish: messages.publish,
  archive: messages.archive,
  restore: messages.restore,
  copy: messages.copy
  // createTaskList: messages.createTaskList
}

const MassToolType = ({
  intl: { formatMessage },
  validate,
  isAccessibilityRegsiterAddress,
  arrAllEnabled,
  disableTypes,
  areStreetOtherEnabled,
  clearSelection,
  ...rest
}) => {
  const options = Object.keys(types).map((value) => ({ value, label: formatMessage(types[value]) }))
  const handleOnChange = () => {
    clearSelection()
  }
  return (
    <Field
      name='type'
      component={RenderRadioButtonGroup}
      options={options}
      defaultValue={'publish'}
      // label={formatMessage(messages.title)}
      // tooltip={formatMessage(messages.tooltip)}
      onChange={handleOnChange}
      {...rest}
    />
  )
}
MassToolType.propTypes = {
  intl: intlShape.isRequired,
  validate: PropTypes.func,
  clearSelection: PropTypes.func,
  isAccessibilityRegsiterAddress: PropTypes.bool,
  areStreetOtherEnabled: PropTypes.bool,
  disableTypes: PropTypes.bool,
  arrAllEnabled: PropTypes.bool
}

export default compose(
  injectIntl,
  connect(null, { clearSelection })
)(MassToolType)
