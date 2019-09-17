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
import { getLastFormInstruction } from 'Routes/CurrentIssues/selectors'
import { Label } from 'sema-ui-components'
import {
  Description
} from 'util/redux-form/fields'
import injectFormName from 'util/redux-form/HOC/injectFormName'

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

const LastInstruction = ({
  isEditable,
  info,
  intl: { formatMessage } }) => {
  return (
    <div className={'col-xs-12'}>
      <Description
        label={formatMessage(messages.currentIssuesTexEditorLabel)}
        tooltip={formatMessage(messages.currentIssuesInfoTooltip)}
        name={'environmentInstructions'}
        isReadOnly={!isEditable}
        isLocalized={false}
      />
      <Label
        infoLabel
        labelText={formatMessage(messages.currentIssueModified, { modified: moment(info.get('modified')).format('DD.MM.YYYY HH:mm') }) + ' ' +
        formatMessage(messages.currentIssueModifiedBy, { modifiedBy: info.get('modifiedBy') })} />
    </div>
  )
}

LastInstruction.propTypes = {
  isEditable: PropTypes.bool.isRequired,
  intl: intlShape.isRequired,
  info: PropTypes.object
}

export default compose(
  injectFormName,
  connect((state, ownProps) => ({
    info: getLastFormInstruction(state, ownProps)
  })
  ),
  injectIntl
)(LastInstruction)
