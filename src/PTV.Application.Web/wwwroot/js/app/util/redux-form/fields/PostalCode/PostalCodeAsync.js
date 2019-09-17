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
import { connect } from 'react-redux'
import { compose } from 'redux'
import { getPostalCodeDefaultOptionsJS, getPostalCodeById } from './selectors'
import { RenderAsyncSelect, RenderAsyncSelectDisplay } from 'util/redux-form/renders'
import PropTypes from 'prop-types'
import asComparable from 'util/redux-form/HOC/asComparable'
import { localizeList } from 'appComponents/Localize'
import { CodeListSchemas } from 'schemas'
import { normalize } from 'normalizr'

const PostalCodeRender = compose(
  connect((state, { input: { value } }) => {
    return {
      defaultOptions: value && getPostalCodeDefaultOptionsJS(state, { id: value })
    }
  })
)(RenderAsyncSelect)

const filterOption = () => true

const formatResponse = data => {
  const result = data.map((pCode) => ({
    postOffice: pCode.postOffice.toUpperCase(),
    label: pCode.code,
    value: pCode.id,
    municipalityId: pCode.municipalityId,
    entity: pCode
  }))

  return result
}

const inputProps = { maxLength: 10 }

const PostalCodeAsync = ({ onChangeObject, dispatch, ...rest }) => {
  const formatDisplay = ({ label }) => {
    return <div>{label}</div>
  }

  const formatOption = ({ label, postOffice }) => {
    return <span>{label} {postOffice}</span>
  }

  const handleOnChange = object => {
    const entity = object && object.entity

    entity && dispatch({
      type: 'POSTAL_CODES',
      response: normalize(entity, CodeListSchemas.POSTAL_CODE)
    })

    onChangeObject && onChangeObject(object)
  }

  return (
    <PostalCodeRender
      filterOption={filterOption}
      url='common/GetPostalCodes'
      formatResponse={formatResponse}
      valueRenderer={formatDisplay}
      formatDisplay={formatDisplay}
      optionRenderer={formatOption}
      minLength={1}
      inputProps={inputProps}
      debounceDelay={300}
      onChangeObject={handleOnChange}
      {...rest}
    />
  )
}

PostalCodeAsync.propTypes = {
  postalCodeLabel: PropTypes.string,
  label: PropTypes.string,
  postOffice: PropTypes.string,
  onChangeObject: PropTypes.func,
  dispatch: PropTypes.func
}

export default compose(
  asComparable({ DisplayRender: RenderAsyncSelectDisplay }),
  connect((state, ownProps) => {
    return {
      postalCodeLabel: getPostalCodeById(state, ownProps.id)
    }
  }),
  localizeList({
    input: 'item',
    output: 'item',
    idAttribute: 'value',
    nameAttribute: 'postOffice',
    getTranslationTexts: item => item.entity.translation.texts
  })
)(PostalCodeAsync)
