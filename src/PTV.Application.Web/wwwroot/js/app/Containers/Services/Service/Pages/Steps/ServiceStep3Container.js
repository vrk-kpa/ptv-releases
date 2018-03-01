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
import { injectIntl } from 'react-intl'
// Actions
import * as mainActions from '../../Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// Components
import ServiceProducer from './Step3Components/ServiceProducer'
import ServiceProducerSticky from './Step3Components/ServiceProducerSticky'
import LanguageLabel from '../../../../Common/languageLabel'
import { Sticky, StickyContainer } from 'react-sticky'
import styles from './styles.scss'

export const ServiceStep3Container = ({
  actions,
  readOnly,
  language,
  translationMode,
  splitContainer,
  keyToState
}) => {
  const sharedProps = { readOnly, translationMode, language, keyToState }
  const stickyProps = { readOnly: true, translationMode, language, keyToState }
  const componentClass = !readOnly && translationMode === 'none' ? 'col-md-6' : 'col-xs-12'
  return (
    <div className='step-3'>
      <LanguageLabel {...sharedProps}
        splitContainer={splitContainer}
            />
      <StickyContainer>
        <div className='row'>
          <div className={componentClass}>
            <ServiceProducer {...sharedProps} />
          </div>
          {!readOnly && translationMode === 'none' &&
            <div className={componentClass}>
              <Sticky stickyClassName='ptv-sticky'>
                <ServiceProducerSticky className='entity-preview' {...stickyProps} />
              </Sticky>
            </div>}
        </div>
      </StickyContainer>
    </div>
  )
}

ServiceStep3Container.propTypes = {
  actions: PropTypes.object.isRequired
}

const actions = [
  mainActions
]

export default injectIntl(connect(null, mapDispatchToProps(actions))(ServiceStep3Container))
