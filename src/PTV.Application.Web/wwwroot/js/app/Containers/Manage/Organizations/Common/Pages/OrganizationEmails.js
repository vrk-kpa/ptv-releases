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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps';

//Actions
import * as commonActions from '../Actions';
import * as organizationActions from  '../../Organization/Actions';

// components
import * as PTVValidatorTypes from '../../../../../Components/PTVValidators';
import Email from '../../../../Common/Emails';

// selectors
import * as CommonOrganizationSelectors from '../Selectors';
import * as OrganizationSelectors from '../../Organization/Selectors';

export const OrganizationEmail = ({
  messages,
  readOnly,
  language,
  translationMode,
  intl,
  organizationId,
  emails,
  collapsible,
  order,
  actions,
  shouldValidate,
  splitContainer
}) => {
  const onAddEmail = (entity) => {
    actions.onLocalizedOrganizationEntityAdd('emails', entity, organizationId, language)
  }
  const onRemoveEmail = (id) => {
    actions.onLocalizedOrganizationListChange('emails', organizationId, id, language)
  }
  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const body = (
    <div>
      <Email {...sharedProps}
        messages={messages}
        items={emails}
        onAddEmail={onAddEmail}
        onRemoveEmail={onRemoveEmail}
        shouldValidate={shouldValidate}
        startOrder={order}
        collapsible={collapsible}
      />
    </div>
  )
  return (!readOnly || readOnly && emails.size !== 0) && body
}
OrganizationEmail.propTypes = {
  messages: PropTypes.object.isRequired
}

function mapStateToProps (state, ownProps) {
  return {
    emails: CommonOrganizationSelectors.getOrganizationEmails(state, ownProps),
    organizationId: CommonOrganizationSelectors.getOrganizationId(state, ownProps)
  }
}
const actions = [
  commonActions,
  organizationActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(OrganizationEmail));
