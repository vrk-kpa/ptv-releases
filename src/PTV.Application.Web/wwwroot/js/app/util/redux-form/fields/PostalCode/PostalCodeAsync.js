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
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import { apiCall3 } from 'actions'
import { CodeListSchemas } from 'schemas'
import withLazyLoading from 'util/redux-form/HOC/withLazyLoading'
import { getIsPostalCodeInStore, getPostalCodeById } from './selectors'
import { compose } from 'redux'
import PropTypes from 'prop-types'
import { localizeItem } from 'appComponents/Localize'
import { connect } from 'react-redux'
import asComparable from 'util/redux-form/HOC/asComparable'

const PostalCodeRender = compose(
  withLazyLoading({
    loadItemByIdAction: postalCodeId => (
      apiCall3({
        keys: ['enums', 'GetPostalCodeById'],
        payload: {
          endpoint: 'common/GetPostalCodeById',
          data: { Id: postalCodeId }
        },
        schemas: CodeListSchemas.POSTAL_CODE
      })
    ),
    getIsItemInStoreAction: getIsPostalCodeInStore
  })
)(RenderAsyncSelect)

const PostalCodeDisplay = connect(
  (state, { id }) => ({
    label: getPostalCodeById(state, { id })
  })
)(({ label }) => <div>{label}</div>)

const OptionValue = compose(
  localizeItem({
    input: 'item',
    output: 'item',
    idAttribute: 'value',
    nameAttribute: 'postOffice',
    getTranslationTexts: item => item.entity.translation.texts
  })
)(({ item }) => <span>{item.label} {item.postOffice.toUpperCase()}</span>)

const filterOption = () => true

const formatResponse = data =>
  data.map((pCode) => ({
    postOffice: pCode.postOffice.toUpperCase(),
    label: pCode.code,
    value: pCode.id,
    municipalityId: pCode.municipalityId,
    entity: pCode
  }))

const formatDisplay = ({ label, value }) => label
  ? <div>{label}</div>
  : <PostalCodeDisplay id={value} />

const formatOption = (postalCode) => <OptionValue item={postalCode} />

const inputProps = { maxLength: 10 }

const PostalCodeAsync = ({
  ...rest
}) => (
  <PostalCodeRender
    filterOption={filterOption}
    url='common/GetPostalCodes'
    formatResponse={formatResponse}
    valueRenderer={formatDisplay}
    formatDisplay={formatDisplay}
    optionRenderer={formatOption}
    minLength={1}
    inputProps={inputProps}
    {...rest}
  />
)

formatDisplay.propTypes = {
  label: PropTypes.string,
  value: PropTypes.string
}

export default compose(
  asComparable({ DisplayRender: RenderAsyncSelectDisplay })
)(PostalCodeAsync)
