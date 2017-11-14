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
import { Field, FieldArray } from 'redux-form/immutable'
import {
  Name,
  ShortDescription,
  UrlChecker
} from 'util/redux-form/fields'
import { asContainer } from 'util/redux-form/HOC'

const renderAttachments = ({
  fields
}) => {
  // This sucks //
  if (fields && fields.length === 0) {
    fields.push()
  }
  return (
    <div>
      <div onClick={() => { fields.push() }}>
        Add
      </div>
      <div>
        {fields.map((attachment, index) => {
          const handleOnRemove = () => {
            if (fields.length <= 1 || index === 0) {
              return
            }
            fields.remove(index)
          }
          return (
            <div>
              {index !== 0 &&
                <div onClick={handleOnRemove}>
                  remove
                </div>}
              <Name
                name={`${attachment}.name`}
                skipValidation
              />
              <ShortDescription
                name={`${attachment}.description`}
                counter
                maxLength={150}
              />
              <UrlChecker name={`${attachment}.url`} />
            </div>
          )
        })}
      </div>
    </div>
  )
}

const Attachements = () => (
  <FieldArray
    name='attachements'
    component={renderAttachments}
  />
)

export default compose(
  asContainer({ title: 'Attachments' })
)(Attachements)
