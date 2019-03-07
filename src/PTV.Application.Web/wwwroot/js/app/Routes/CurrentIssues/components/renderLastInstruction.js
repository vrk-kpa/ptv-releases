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
import { injectIntl, defineMessages, intlShape } from 'util/react-intl'
import moment from 'moment'
import { getIsEditEnabled } from 'Routes/CurrentIssues/selectors'
import { getUserRoleName } from 'selectors/userInfo'
import { Label } from 'sema-ui-components'
import {
  Description
} from 'util/redux-form/fields'
import { fieldPropTypes } from 'redux-form/immutable'

const messages = defineMessages({
  currentIssuesTexEditorLabel: {
    id: 'CurrentIssues.Header.CurrentIssues.Label',
    defaultMessage: 'Tiedotteet - current'
  },
  currentIssuesInfoTooltip: {
    id: 'CurrentIssues.Header.CurrentIssues.Tooltip',
    defaultMessage: 'Kirjoita uutinen muotoon: Päivämäärä, kellonaika ja ensin teksti suomeksi ja mahdollisuuksien mukaan ruotsiksi. Esimerkki alla: 19.5.2017 15:30 PTV:ssä vaikeuksia tallentaa palveluja. Och samma på svenska.'
  },
  currentIssueModified: {
    id: 'CurrentIssues.Header.CurrentIssues.Modified.Label',
    defaultMessage: 'Muokattu: {modified}'
  },
  currentIssueModifiedBy: {
    id: 'CurrentIssues.Header.CurrentIssues.ModifiedBy.Label',
    defaultMessage: 'Muokkaaja: {modifiedBy}'
  }
})

const renderLastInstruction = ({
  input,
  isForEdit,
  isEditEnabled,
  intl: { formatMessage } }) => {
  const onChange = (value) => {
    input.onChange(input.value.set('environmentInstructions', value))
  }

  return (
    <div className={isForEdit ? 'col-xs-6' : 'col-xs-12'}>
      <Description
        label={formatMessage(messages.currentIssuesTexEditorLabel)}
        tooltip={formatMessage(messages.currentIssuesInfoTooltip)}
        name={'CurrentEnvironemntIssues'}
        value={input.value.get('environmentInstructions') || ''}
        readOnly={!isEditEnabled}
        changeCallback={onChange}
      />
      <Label>
        {formatMessage(messages.currentIssueModified,
          { modified: moment(input.value.get('modified')).format('DD.MM.YYYY HH:mm') }) + ' ' +
        formatMessage(messages.currentIssueModifiedBy, { modifiedBy: input.value.get('modifiedBy') }) }
      </Label>
    </div>
  )
}

renderLastInstruction.propTypes = {
  input: fieldPropTypes.input,
  isEditEnabled: PropTypes.bool.isRequired,
  isForEdit: PropTypes.bool.isRequired,
  intl: intlShape.isRequired
}

export default compose(
  connect((state, ownProps) => {
    const role = getUserRoleName(state, ownProps)
    const isForEdit = role === 'Eeva'
    return {
      isEditEnabled: getIsEditEnabled(state, ownProps),
      isForEdit
    }
  }
  ),
  injectIntl
)(renderLastInstruction)
