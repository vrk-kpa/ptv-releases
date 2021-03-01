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
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { connect } from 'react-redux'
import { isDirty } from 'redux-form/immutable'
import {
  Keywords,
  ServiceClassTree,
  IndustrialClassTree,
  LifeEventTree,
  OntologyTermTree,
  TargetGroupTree
} from 'util/redux-form/fields'
import { injectIntl, intlShape } from 'util/react-intl'
import { getIsKR1Selected, getIsKR2Selected } from './selectors'

const isErrorWarning = (meta, props) => {
  return !meta.touched && !meta.dirty && !props.generalDescriptionIsDirty
}

const ServiceClassificationAndKeywords = (
  {
    intl: { formatMessage },
    isKR1Selected,
    isKR2Selected,
    generalDescriptionIsDirty
  }
) => {
  return (
    <div>
      <div className='form-row'>
        <TargetGroupTree
          generalDescriptionIsDirty={generalDescriptionIsDirty}
          isErrorWarning={isErrorWarning}
        />
      </div>
      <div className='form-row'>
        <ServiceClassTree
          generalDescriptionIsDirty={generalDescriptionIsDirty}
          isErrorWarning={isErrorWarning}
        />
      </div>
      <div className='form-row'>
        <OntologyTermTree
          generalDescriptionIsDirty={generalDescriptionIsDirty}
          isErrorWarning={isErrorWarning}
        />
      </div>
      {isKR1Selected && <div className='form-row'>
        <LifeEventTree />
      </div>}
      {isKR2Selected && <div className='form-row'>
        <IndustrialClassTree />
      </div>}

      <div className='form-row'>
        <Keywords />
      </div>
    </div>
  )
}

ServiceClassificationAndKeywords.propTypes = {
  intl: intlShape.isRequired,
  isKR1Selected: PropTypes.bool.isRequired,
  isKR2Selected: PropTypes.bool.isRequired,
  generalDescriptionIsDirty: PropTypes.bool.isRequired
}

export default compose(
  injectIntl,
  injectFormName,
  connect((state, ownProps) => ({
    isKR1Selected: getIsKR1Selected(state, ownProps),
    isKR2Selected: getIsKR2Selected(state, ownProps),
    generalDescriptionIsDirty: isDirty(ownProps.formName)(state, 'generalDescriptionId')
  })))(ServiceClassificationAndKeywords)
