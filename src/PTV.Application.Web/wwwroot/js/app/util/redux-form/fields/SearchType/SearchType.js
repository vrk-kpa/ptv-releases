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
import { compose } from 'redux'
import { connect } from 'react-redux'
import { Field, change } from 'redux-form/immutable'
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import injectSelectPlaceholder from 'appComponents/SelectPlaceholderInjector'
import { RenderListBox } from 'util/redux-form/renders'
import { addressTypesEnum, formTypesEnum, searchTypesEnum, allContentTypesEnum } from 'enums'
import { OrderedSet } from 'immutable'
import styles from './styles.scss'

const messages = defineMessages({
  name: {
    id: 'SearchType.Options.Name',
    defaultMessage: 'Haku sisällön nimellä'
  },
  address: {
    id: 'SearchType.Options.Address',
    defaultMessage: 'Haku osoitteella'
  },
  phone: {
    id: 'SearchType.Options.Phone',
    defaultMessage: 'Haku puhelinnumerolla'
  },
  id: {
    id: 'SearchType.Options.Id',
    defaultMessage: 'Haku PTV-tunnisteella'
  },
  email: {
    id: 'SearchType.Options.Email',
    defaultMessage: 'Haku sähköpostiosoitteella'
  },
  searchBy: {
    id: 'SearchType.SearchBy.Title',
    defaultMessage: 'Search by'
  }
})

const types = {
  name: messages.name,
  address: messages.address,
  phone: messages.phone,
  email: messages.email,
  id: messages.id
}

const SearchType = ({
  intl: { formatMessage },
  change,
  ...rest
}) => {
  const options = Object.keys(types).map((value) => ({ value, label: formatMessage(types[value]) }))
  const getOptionLabel = value => value && formatMessage(types[value])

  const getDefaultContentTypes = (includeGeneralDescription) => {
    const contentTypes = Object.keys(allContentTypesEnum)
      .filter(key => {
        // decide whether GD should be icluded
        return (includeGeneralDescription ||
            allContentTypesEnum[key] !== allContentTypesEnum.GENERALDESCRIPTION) &&
          // and skip joint types as they are selected only virtually (by selecting all child types)
          allContentTypesEnum[key] !== allContentTypesEnum.SERVICE &&
          allContentTypesEnum[key] !== allContentTypesEnum.CHANNEL
      })
      .map(key => allContentTypesEnum[key])

    return OrderedSet(contentTypes)
  }

  const handleSearchTypeChange = value => {
    switch (value) {
      case searchTypesEnum.ADDRESS:
        change(formTypesEnum.FRONTPAGESEARCH, 'streetType', addressTypesEnum.STREET)
        change(formTypesEnum.FRONTPAGESEARCH, 'contentTypes', getDefaultContentTypes(true))
        break
      case searchTypesEnum.PHONE:
        change(formTypesEnum.FRONTPAGESEARCH, 'contentTypes', getDefaultContentTypes(true))
        break
      default:
        change(formTypesEnum.FRONTPAGESEARCH, 'contentTypes', getDefaultContentTypes(false))
        break
    }
  }

  return (
    <Field
      name='searchType'
      reduxKey='searchType'
      component={RenderListBox}
      options={options}
      defaultValue='name'
      getOptionLabel={getOptionLabel}
      aria-label={formatMessage(messages.searchBy)}
      onChangeObject={handleSearchTypeChange}
      componentClass={styles.searchType}
      {...rest}
    />
  )
}
SearchType.propTypes = {
  change: PropTypes.func.isRequired,
  intl: intlShape
}

export default compose(
  injectIntl,
  connect(null, { change }),
  injectSelectPlaceholder()
)(SearchType)
