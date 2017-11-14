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
import { FormSection } from 'redux-form/immutable'
import { FileExtensions, UrlChecker } from 'util/redux-form/fields'

class FormFiles extends FormSection {
  static defaultProps = {
    name: 'formFiles'
  }
  render () {
    const {
     isCompareMode,
     splitView
    } = this.props
    return (
      <div>
        <div className='form-row'>
          <FileExtensions
            isCompareMode={isCompareMode}
            isLocalized={false}
            size='w200'
            required />
        </div>
        <div className='form-row'>
          <UrlChecker
            isCompareMode={isCompareMode}
            splitView={splitView}
            isLocalized={false}
            required />
        </div>
      </div>
    )
  }
}

export default FormFiles
