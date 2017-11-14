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
import React, { PropTypes, Component } from 'react'
import { injectIntl, intlShape } from 'react-intl'
import { connect } from 'react-redux'

import mapDispatchToProps from '../../../../../../Configuration/MapDispatchToProps'
import { PTVTextInput } from '../../../../../../Components'
import * as ServiceSelectors from '../../../Selectors'
import * as OrganizationSelectors from '../../../../../Manage/Organizations/Common/Selectors'
import OrganizationsTagSelect from '../../../../../Common/organizationsTagSelect'

class SelfProducerSticky extends Component {
  static propTypes = {
    producerId: PropTypes.string.isRequired
  };

  render () {
    const { language, producerId } = this.props
    return (
      <div className='entity-preview-item'>
        <OrganizationsTagSelect
          id={producerId}
          sourceSelector={ServiceSelectors.getSelectedOrganizersItemsWithMainOrganizationJS}
          selector={ServiceSelectors.getSelectedServiceProducerOrganizersItemsJS}
          keyToState={'any'}
          language={language}
          readOnly />
      </div>)
  }
}

// / PurchaseServices component - Ostopalvelut
class PurchaseServicesSticky extends Component {
  static propTypes = {
    name: PropTypes.string.isRequired,
    additionalInformation: PropTypes.string
  };

  render () {
    const { name, additionalInformation, language } = this.props
    return (
      <div className='entity-preview-item'>
        <PTVTextInput
            value={name}
            name='stickyPurchase'
            readOnly />

        {additionalInformation !== '' &&

          <PTVTextInput
            value={additionalInformation}
            name='stickyPurchase'
            readOnly />
        }
      </div>
    )
  }
}

// / Other component - Muu
class OtherProducerSticky extends Component {
  static propTypes = {
    name: PropTypes.string.isRequired,
    additionalInformation: PropTypes.string
  };

  render () {
    const { name, additionalInformation, language } = this.props
    return (
      <div className='entity-preview-item'>
        <PTVTextInput
          value={name && name.length > 0 ? name : additionalInformation}
          name='stickyOtherProducer'
          readOnly />
      </div>
    )
  }
}

// / maps state to props
function mapStateToProps (state, ownProps) {
  const organization = OrganizationSelectors.getOrganizationForId(state, {id: ownProps.organizationId, ...ownProps})
  return {
    name : organization.get('name'),
    selectedOrganizers : ServiceSelectors.getSelectedOrganizersMap(state, ownProps)
  }
}

const actions = []

export default {
  SelfProducerSticky: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(SelfProducerSticky)),
  PurchaseServicesSticky: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(PurchaseServicesSticky)),
  OtherProducerSticky: connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OtherProducerSticky))
}

