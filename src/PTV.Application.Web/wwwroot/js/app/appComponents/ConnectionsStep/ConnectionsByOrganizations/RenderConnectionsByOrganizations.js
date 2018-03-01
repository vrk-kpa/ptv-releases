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
import { connect } from 'react-redux'
import { formValueSelector } from 'redux-form/immutable'
import { injectFormName } from 'util/redux-form/HOC'
import { getSelectedEntityType } from 'selectors/entities/entities'
import OrganizationTitle from './OrganizationTitle'
import ASTIGroup from './ASTIGroup'

const RenderConnectionsByOrganizations = ({
  fields,
  entityType,
  organizationIds
}) => {
  const fieldNames = []
  fields.forEach(organizationId => {
    fieldNames.push(organizationId)
  })
  return (
    <div>
      {fieldNames.map((field, index) => {
        const organizationId = organizationIds[index]
        return <ASTIGroup
          key={index}
          name={field}
          organizationId={organizationId}
          title={<OrganizationTitle organizationId={organizationId} />}
        />
      })}
    </div>
  )
}

RenderConnectionsByOrganizations.propTypes = {
  fields: PropTypes.object,
  entityType: PropTypes.string,
  organizationIds: PropTypes.array
}

export default compose(
  injectFormName,
  connect((state, { formName }) => {
    const formSelector = formValueSelector(formName)
    const connectionsByOrganizations = formSelector(state, 'connectionsByOrganizations')
    const organizationIds = connectionsByOrganizations.keySeq().toJS() || []
    return {
      entityType: getSelectedEntityType(state),
      organizationIds
    }
  })
)(RenderConnectionsByOrganizations)
