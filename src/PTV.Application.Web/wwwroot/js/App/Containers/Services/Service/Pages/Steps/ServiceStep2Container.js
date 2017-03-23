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
import React, { PropTypes } from 'react'
import { connect } from 'react-redux'
import { injectIntl } from 'react-intl'

// Components
import ServiceTargetGroups from '../../../Common/Pages/serviceTargetGroups'
import ServiceServiceClasses from '../../../Common/Pages/serviceServiceClasses'
import ServiceOntologyTerms from '../../../Common/Pages/serviceOntologyTerms'
import ServiceLifeEvents from '../../../Common/Pages/serviceLifeEvents'
import ServiceIndustrialClasses from '../../../Common/Pages/serviceIndustrialClasses'
import ServiceKeywords from '../../../Common/Pages/serviceKeywords'
import LanguageLabel from '../../../../Common/languageLabel'

// actions
import * as mainActions from '../../Actions'
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'

// styles
import '../../../styles/ServiceStep2Container.scss'

// selectros
import * as ServiceSelectors from '../../Selectors'

// Messages
import * as Messages from '../../Messages'

export const ServiceStep2Container = props => {
  const { readOnly, isTargetGroupSelected, language, translationMode, splitContainer, keyToState } = props
  const sharedProps = { readOnly, translationMode, language, keyToState }

  return (
    <div className='step-2'>
      <LanguageLabel {...sharedProps}
        splitContainer={splitContainer}
      />
      <ServiceTargetGroups {...sharedProps}
        messages={Messages.serviceTargetGroupMessages}
      />
      <ServiceServiceClasses {...sharedProps}
        messages={Messages.serviceServiceClassMessages}
      />
      <ServiceOntologyTerms {...sharedProps}
        messages={Messages.serviceOntologyTermMessages}
      />
      <ServiceLifeEvents {...sharedProps}
        messages={Messages.serviceLifeEventMessages}
      />
      {isTargetGroupSelected
        ? <ServiceIndustrialClasses {...sharedProps}
          messages={Messages.serviceIndustrialClassMessages}
        />
        : null}
      <ServiceKeywords {...sharedProps}
      />
      <div className='clearfix' />
    </div>
  )
}

ServiceStep2Container.propTypes = {
  actions: PropTypes.object
}

function mapStateToProps (state, ownProps) {
  const kr2Id = ServiceSelectors.getTargetGrougKR2Id(state)
  const isTargetGroupSelected = ServiceSelectors.getIsSelectedTargetGroupWithGeneralDescription(state, { id: kr2Id })
  return {
    isTargetGroupSelected
  }
}

const actions = [
  mainActions
]
export default injectIntl(connect(mapStateToProps, mapDispatchToProps(actions))(ServiceStep2Container))
