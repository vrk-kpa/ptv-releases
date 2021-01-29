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
import { defineMessages, injectIntl, intlShape } from 'util/react-intl'
import { entityTypesEnum } from 'enums'
import { Label } from 'sema-ui-components'
import { Accordion } from 'appComponents/Accordion'
import { CommonFilters, ChannelFilters, ServiceFilters } from 'appComponents/Filters'
import { getIsAnyServiceFilterOn, getIsAnyChannelFilterOn } from 'Routes/Connections/selectors'

const messages = defineMessages({
  searchFilterTitle: {
    id: 'Routes.Connections.Components.SearchForm.SearchFilters.Title',
    defaultMessage: 'Rajaa hakua'
  },
  searchFilterInfo: {
    id: 'Routes.Connections.Components.SearchForm.SearchFilters.Info',
    defaultMessage: 'Hakua on rajattu'
  }
})

const SearchFilters = ({
  intl: { formatMessage },
  entityType,
  isFiltered
}) => {
  const entityFilters = {
    [entityTypesEnum.CHANNELS]: <ChannelFilters />,
    [entityTypesEnum.SERVICES]: <ServiceFilters />
  }[entityType]
  return (
    <Accordion light activeIndex={-1}>
      <Accordion.Title
        validate={false}
        title={formatMessage(messages.searchFilterTitle)}
        info={isFiltered && <Label infoLabel labelText={formatMessage(messages.searchFilterInfo)} />}
      />
      <Accordion.Content>
        <div>{entityFilters}</div>
        <div><CommonFilters /></div>
      </Accordion.Content>
    </Accordion>
  )
}
SearchFilters.propTypes = {
  intl: intlShape,
  entityType: PropTypes.string.isRequired,
  isFiltered: PropTypes.bool
}

export default compose(
  injectIntl,
  connect((state, { entityType }) => {
    const isFiltered = {
      [entityTypesEnum.CHANNELS]: getIsAnyChannelFilterOn(state),
      [entityTypesEnum.SERVICES]: getIsAnyServiceFilterOn(state)
    }[entityType]
    return {
      isFiltered
    }
  })
)(SearchFilters)
