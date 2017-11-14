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
import { connect } from 'react-redux'
import React from 'react'
import PropTypes from 'prop-types'
import { injectIntl, intlShape } from 'react-intl'

// Components
import GeneralDescriptionType from '../../../Common/Pages/generalDescriptionTypes'
import GeneralDescriptionNames from '../../../Common/Pages/generalDescriptionNames'
import GeneralDescriptionDescriptions from '../../../Common/Pages/generalDescriptionDescriptions'
import GeneralDescriptionChargeTypes from '../../../Common/Pages/generalDescriptionChargeTypes'
import GeneralDescriptionLaws from '../../../Common/Pages/generalDescriptionLaws'
import GeneralDescriptionTypeAdditionalInfo from '../../../Common/Pages/generalDescriptionTypeAdditionalInfo'
import GeneralDescriptionTargetGroups from '../../../Common/Pages/generalDescriptionTargetGroups'
import GeneralDescriptionServiceClasses from '../../../Common/Pages/generalDescriptionServiceClasses'
import GeneralDescriptionLifeEvents from '../../../Common/Pages/generalDescriptionLifeEvents'
import GeneralDescriptionIndustrialClasses from '../../../Common/Pages/generalDescriptionIndustrialClasses'
import GeneralDescriptionOntologyTerms from '../../../Common/Pages/generalDescriptionOntologyTerms'

import PublishingStatus from '../../../../../Common/PublishingStatus'
import LanguageLabel from '../../../../../Common/languageLabel'

// Actions
import * as mainActions from '../../Actions'
import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'

// Selectors
import * as ServiceSelectors from '../../Selectors'

// Messages
import * as Messages from '../../Messages'

export const Step1Container = props => {
  const { readOnly, language, publishingStatus, translationMode, splitContainer, entityId, keyToState, intl: { formatMessage } } = props
  const sharedProps = { readOnly, translationMode, language, entityId, keyToState }

  return (
    <div>
      <div className='form-step'>
        <LanguageLabel {...sharedProps}
          splitContainer={splitContainer}
        />
        {entityId
          ? <PublishingStatus publishingStatus={publishingStatus} language={language} pageType='generalDescription' />
          : null}

        <GeneralDescriptionType {...sharedProps}
          messages={Messages.serviceTypeMessages}
              />
        <GeneralDescriptionNames {...sharedProps}
          messages={Messages.namesMessages}
              />
        <GeneralDescriptionDescriptions {...sharedProps}
          messages={Messages.descriptionMessages}
              />
        <GeneralDescriptionChargeTypes {...sharedProps}
          messages={Messages.chargeTypeMessages}
              />
        <GeneralDescriptionTypeAdditionalInfo {...sharedProps}
          messages={Messages}
              />
        <GeneralDescriptionLaws {...sharedProps}
          messages={Messages.lawsMessages}
              />
      </div>
      <div className='form-step'>
        <h3>{formatMessage(Messages.descriptionMessages.generalDescriptionStep2Title)}</h3>
        <GeneralDescriptionTargetGroups {...sharedProps}
          messages={Messages.targetGroupMessages}
              />
        <GeneralDescriptionServiceClasses {...sharedProps}
          messages={Messages.serviceClassMessages}
              />
        <GeneralDescriptionOntologyTerms {...sharedProps}
          messages={Messages.ontologyTermMessages}
              />
        <GeneralDescriptionLifeEvents {...sharedProps}
          messages={Messages.lifeEventMessages}
              />
        <GeneralDescriptionIndustrialClasses {...sharedProps}
          messages={Messages.industrialClassMessages}
              />
      </div>
    </div>
  )
}

Step1Container.propTypes = {
  readOnly: PropTypes.bool,
  language: PropTypes.string,
  publishingStatus: PropTypes.string,
  translationMode: PropTypes.string,
  splitContainer: PropTypes.bool,
  entityId: PropTypes.any,
  keyToState: PropTypes.any,
  intl: intlShape
}

function mapStateToProps (state, ownProps) {
  return {
    publishingStatus: ServiceSelectors.getPublishingStatus(state, ownProps)
  }
}

const actions = [
  mainActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(Step1Container))
