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
import EntityDescription from 'appComponents/EntityDescription'
import ChannelTypeCell from 'appComponents/Cells/ChannelTypeCell'
import ServiceTypeCell from 'appComponents/Cells/ServiceTypeCell'
import { entityTypesEnum } from 'enums'
import { fieldPropTypes } from 'redux-form/immutable'
import styles from './styles.scss'

const EntityRow = ({
  input,
  entityType
}) => {
  return (
    <div className='row align-items-center'>
      <div className='col-lg-12'>
        <EntityDescription
          name={input.value.get('name')}
          entityId={input.value.get('id')}
          unificRootId={input.value.get('unificRootId')}
          organizationId={input.value.get('organization')}
          contentClass={styles.content}
          preview={false}
        />
      </div>
      <div className='col-lg-4'>
        {entityType === entityTypesEnum.SERVICES
          ? <ServiceTypeCell
            serviceTypeId={input.value.get('serviceType')}
          />
          : <ChannelTypeCell
            channelTypeId={input.value.get('channelTypeId')}
          />}
      </div>
    </div>
  )
}

EntityRow.propTypes = {
  input: fieldPropTypes.input,
  entityType: PropTypes.string
}

export default EntityRow
