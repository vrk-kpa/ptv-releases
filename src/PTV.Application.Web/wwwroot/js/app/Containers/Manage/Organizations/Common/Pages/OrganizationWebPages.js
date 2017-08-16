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
import mapDispatchToProps from '../../../../../Configuration/MapDispatchToProps'
// Actions
import * as commonActions from '../Actions'
import * as organizationActions from '../../Organization/Actions'
// components
import WebPages from '../../../../Common/WebPages'
// selectors
import * as CommonOrganizationSelectors from '../Selectors'

export const OrganizationWebPages = ({
  messages,
  readOnly,
  language,
  translationMode,
  webPages,
  webPageEntities,
  webPagesOrdered,
  actions,
  organizationId,
  withName,
  withTypes,
  collapsible,
  withOrder,
  shouldValidate,
  splitContainer
}) => {
  const onAddWebPage = (entity) => {
    actions.onLocalizedOrganizationEntityAdd('webPages', entity, organizationId, language)
  }
  const onRemoveWebPage = (id) => {
    actions.onLocalizedOrganizationListChange('webPages', organizationId, id, language)
    const deletedEntity = webPageEntities.find(entity => entity.get('id') === id)
    let newMap = webPageEntities.map(entity => {
      if (entity.get('orderNumber') > deletedEntity.get('orderNumber')) {
        const chngeEntity = entity.set('orderNumber', (entity.get('orderNumber') - 1))
        return chngeEntity
      }
      return entity
    })
    newMap = newMap.filter(entity => entity.get('id') !== id).toJS()
    actions.onOrganizationEntityReplace(
      'webPages',
      Array.isArray(newMap)
        ? newMap
        : [newMap],
      organizationId
    )
  }

  const sharedProps = { readOnly, language, translationMode, splitContainer }
  const body = (
    <WebPages {...sharedProps}
      messages={messages}
      items={readOnly ? webPagesOrdered : webPages}
      shouldValidate={shouldValidate}
      withName={withName}
      withTypes={withTypes}
      withOrder
      onAddWebPage={onAddWebPage}
      onRemoveWebPage={onRemoveWebPage}
      collapsible={collapsible}
      includeReset
    />
  )
  return (!readOnly || readOnly && webPages.size !== 0) && body
}

OrganizationWebPages.propTypes = {
  shouldValidate: PropTypes.bool
}

OrganizationWebPages.defaultProps = {
  shouldValidate: false
}

function mapStateToProps (state, ownProps) {
  return {
    webPages: CommonOrganizationSelectors.getOrganizationWebPages(state, ownProps),
    webPagesOrdered: CommonOrganizationSelectors.getSortedOrganizationWebPages(state, ownProps),
    webPageEntities: CommonOrganizationSelectors.getOrganizationWebPageEntities(state, ownProps),
    organizationId: CommonOrganizationSelectors.getOrganizationId(state, ownProps)
  }
}

const actions = [
  commonActions,
  organizationActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(OrganizationWebPages)
