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
import {
  Name,
  ShortDescription,
  UrlChecker,
  AdditionalInformation
} from 'util/redux-form/fields'
import { FormSection } from 'redux-form/immutable'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import styles from './styles.scss'

class Attachment extends FormSection {
  static propTypes = {
    isCompareMode: PropTypes.bool,
    hideDescription: PropTypes.bool
  }

  static defaultProps = {
    name: 'attachment',
    hideDescription: false
  }
  render () {
    const {
      isCompareMode,
      hideDescription,
      urlCheckerProps = {},
      nameProps = {},
      descriptionProps = {},
      splitView,
      isReadOnly,
      collectionPrefix = 'attachments',
      compare
    } = this.props
    const basicCompareModeClass = isCompareMode || splitView ? 'col-lg-24 mb-2' : 'col-lg mb-2 mb-lg-0'
    return (
      <div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className={basicCompareModeClass}>
              <Name
                isCompareMode={isCompareMode}
                name={'name'}
                isLocalized={false}
                skipValidation
                {...nameProps}
                isReadOnly={isReadOnly}
                noTranslationLock
                useQualityAgent
                collectionPrefix={collectionPrefix}
                compare={compare}
              />
            </div>
            <div className='col-lg'>
              {!hideDescription &&
              <AdditionalInformation
                isCompareMode={isCompareMode}
                name='description'
                counter
                maxLength={150}
                isLocalized={false}
                {...descriptionProps}
                noTranslationLock
                useQualityAgent
                multiline={false}
                collectionPrefix={collectionPrefix}
                postFix='description'
                compare={compare}
              />}
            </div>
          </div>
        </div>
        <div className='collection-form-row'>
          <div className='row'>
            <div className='col-lg-24 mb-4'>
              <UrlChecker
                isCompareMode={isCompareMode}
                name={'urlAddress'}
                isLocalized={false}
                splitView={splitView}
                {...urlCheckerProps}
                isReadOnly={isReadOnly}
              />
            </div>
          </div>
        </div>
      </div>
    )
  }
}

export default compose(
  withFormStates
)(Attachment)
