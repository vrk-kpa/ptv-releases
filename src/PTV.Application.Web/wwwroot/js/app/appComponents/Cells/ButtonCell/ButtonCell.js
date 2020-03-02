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
import { compose } from 'redux'
import { connect } from 'react-redux'
import {
  Button,
  Spinner
} from 'sema-ui-components'
import { getFormValues } from 'redux-form/immutable'
import { EntitySelectors } from 'selectors'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import { Map } from 'immutable'

const messages = defineMessages({
  buttonSelect: {
    id: 'appComponents.Buttons.Select',
    defaultMessage: 'Valitse'
  },
  buttonSelected: {
    id: 'appComponents.Buttons.Selected',
    defaultMessage: 'Valittu'
  }
})

const ButtonCell = ({
  id,
  selected,
  isLoading,
  onClick,
  btnTitle,
  intl: { formatMessage }
}) => {
  const handleClick = (e) => {
    e.preventDefault()
    if (!selected && onClick && typeof onClick === 'function') {
      onClick(id)
    }
  }
  const title = selected && formatMessage(messages.buttonSelected) || formatMessage(messages.buttonSelect)
  return (
    <Button
      secondary
      small
      onClick={handleClick}
      disabled={isLoading || selected}
      children={isLoading && selected && <Spinner /> || title}
    />
  )
}

ButtonCell.propTypes = {
  onClick: PropTypes.func,
  selected: PropTypes.bool.isRequired,
  isLoading: PropTypes.bool,
  id: PropTypes.string.isRequired,
  btnTitle: PropTypes.string,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const formState = getFormValues(ownProps.formName)(state) || Map()
    return {
      selected: ownProps.id === formState.get(ownProps.formProperty) || false,
      isLoading: EntitySelectors.generalDescriptions.getEntityIsFetching(state)
    }
  })
)(ButtonCell)
