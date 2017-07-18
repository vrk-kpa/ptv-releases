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
import React, { PropTypes } from 'react'
import { Select } from 'sema-ui-components'
import { List } from 'immutable'
import { connect } from 'react-redux'
import { getTranslatableLanguages } from 'Routes/FrontPageV2/selectors'
import { injectIntl, intlShape, defineMessages } from 'react-intl'
import { compose } from 'redux'
import { localizeList } from 'appComponents/Localize'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import InfoBox from 'appComponents/InfoBox'

const messages = defineMessages({
  title: {
    id: 'FrontPage.SelectLanguage.Title',
    defaultMessage: 'Kielivalinta'
  },
  tooltip: {
    id: 'FrontPage.SelectLanguage.Tooltip',
    defaultMessage: 'Haku kohdentaa vain yhtä kieltä kohden. Jos haet palvelua ruotsin tai englannin kielisen nimen mukaan, valitse haluamasi kieli pudotusvalikosta.'
  }
})

const renderLanguages = ({
  input,
  languages,
  intl: { formatMessage },
  inlineLabel,
  ...rest
}) => {
  let value = (input && input.value) || []
  if (List.isList(input.value)) {
    value = value.toJS()
  }
  return (
    <div>
      {!inlineLabel &&
        <div className='form-field tooltip-label'>
          <strong>
            {formatMessage(messages.title)}
            <InfoBox tip={formatMessage(messages.tooltip)} />
          </strong>
        </div>
      }
      <Select
        label={inlineLabel && formatMessage(messages.title)}
        options={languages}
        {...input}
        value={value}
        onChange={obj => {
          input.onChange(List(obj.map(x => x.value))) // This is just awful tbh
        }}
        size='full'
        multi
        {...rest}
      />
    </div>
  )
}
renderLanguages.propTypes = {
  input: PropTypes.object.isRequired,
  languages: PropTypes.array.isRequired,
  intl: intlShape.isRequired,
  inlineLabel: PropTypes.bool
}

renderLanguages.defaultProps = {
  inlineLabel: false
}

export default compose(
  connect(
    state => ({
      languages: getTranslatableLanguages(state)
    })
  ),
  localizeList({
    input: 'languages',
    idAttribute: 'id',
    nameAttribute: 'label'
  }),
  injectIntl,
  injectSelectPlaceholder
)(renderLanguages)
