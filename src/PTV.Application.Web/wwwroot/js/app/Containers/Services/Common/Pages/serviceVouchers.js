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
import mapDispatchToProps from '../../../../Configuration/MapDispatchToProps'
import { injectIntl, FormattedMessage } from 'react-intl'
import { Map } from 'immutable'
import shortId from 'shortid'
// Actions
import * as commonServiceActions from '../Actions'
import * as commonActions from '../../../Common/Actions'
import * as serviceActions from '../../Service/Actions'
// components
import WebPages from '../../../Common/WebPages'
import { PTVCheckBox } from '../../../../Components'
// selectors
import * as ServiceSelectors from '../../Service/Selectors'
import * as CommonServiceSelectors from '../Selectors'
import * as CommonSelectors from '../../../Common/Selectors'
// schemas
import { CommonSchemas } from '../../../Common/Schemas'

export const ServiceVouchers = ({
  messages,
  readOnly,
  language,
  translationMode,
  vouchers,
  vouchersEntities,
  vouchersOrdered,
  actions,
  serviceId,
  collapsible,
  shouldValidate,
  splitContainer,
  contentProps
}) => {
  const onAddWebPage = (entity) => {
    actions.onVoucherAdd(entity, language)
  }
  const onRemoveWebPage = (id) => {
    actions.onListChange('serviceVouchers', id, language)
    const deletedEntity = vouchersEntities.find(entity => entity.get('id') === id)
    let newMap = vouchersEntities.map(entity => {
      if (entity.get('orderNumber') > deletedEntity.get('orderNumber')) {
        const chngeEntity = entity.set('orderNumber', (entity.get('orderNumber') - 1))
        return chngeEntity
      }
      return entity
    })
    newMap = newMap.filter(entity => entity.get('id') !== id).toJS()
    actions.onServiceEntityReplace(
      'serviceVouchers',
      Array.isArray(newMap)
        ? newMap
        : [newMap],
      serviceId
    )
  }
  const onEntityInputChange = (id, input, value) => {
    actions.onEntityInputChange('serviceVouchers', id, input, value)
  }

  const onCustomCheckUrl = (value, id) => {
    actions.apiCall(['serviceVouchers', id],
            { endpoint: 'url/Check', data: { urlAddress: value, id: id } }, null, CommonSchemas.SERVICE_VOUCHER)
  }

  const onInputCheckboxChange = (input, isSet = false) => value => {
    if (value.target.checked && vouchers.size === 0) {
      onAddWebPage([{
        id: shortId.generate(),
        orderNumber: 1
      }])
    }

    actions.onServiceInputChange(input, value.target.checked, language, isSet)
  }

  const checkBoxRender = index => {
    return (
      index === 0 &&
      <div className='col-xs-12'>
        <PTVCheckBox
          id={serviceId}
          className='strong'
          isSelectedSelector={ServiceSelectors.getIsActiveVoucher}
          onClick={onInputCheckboxChange('isActiveVoucher')}
          readOnly={readOnly && translationMode === 'none'}
          isDisabled={translationMode === 'view'}
          language={language}
          keyToState='services'
        >
          {readOnly
            ? <FormattedMessage {...messages.checkboxIsSelectedTitle} />
            : <FormattedMessage {...messages.checkboxTitle} />
          }

        </PTVCheckBox>
      </div>
    )
  }

  const sharedProps = { readOnly, language, translationMode, splitContainer, ...contentProps }
  const customConfiguration = {
    onCustomInputChange:onEntityInputChange,
    onCustomCheckUrl:onCustomCheckUrl,
    customUrlAddressSelectors: CommonSelectors.getServiceVoucherUrlAddress,
    customNameSelectors: CommonSelectors.getServiceVoucherName,
    customOrderNumberSelectors: CommonSelectors.getServiceVoucherOrderNumber,
    customUrlExistsSelectors: CommonSelectors.getServiceVoucherUrlExists,
    customIsFetchingSelectors: CommonSelectors.getServiceVoucherIsFetching,
    customAreDataValidSelectors: CommonSelectors.getServiceVoucherAreDataValid,
    customAdditionalInformationSelectors: CommonSelectors.getServiceVoucherAdditionalInformation,
    customRender: checkBoxRender
  }

  const body = (
    <WebPages {...sharedProps}
      messages={messages}
      shouldValidate={shouldValidate}
      onAddWebPage={onAddWebPage}
      onRemoveWebPage={onRemoveWebPage}
      collapsible={collapsible}
      customConfiguration={customConfiguration}
      includeReset={false}
    />
  )
  return (!readOnly || readOnly && vouchers.size !== 0) && body
}

ServiceVouchers.propTypes = {
  shouldValidate: PropTypes.bool
}

ServiceVouchers.defaultProps = {
  shouldValidate: false
}

function mapStateToProps (state, ownProps) {
  const vouchers = ServiceSelectors.getServiceVouchers(state, ownProps)
  const vouchersOrdered = ServiceSelectors.getSortedServiceVouchers(state, ownProps)
  const contentProps = ServiceSelectors.getIsActiveVoucher(state, ownProps)
        ? {
          items: ownProps.readOnly ? vouchersOrdered : vouchers,
          withName: true,
          withOrder: true,
          withAdditionInfo: true }
        : {
          items: Map(),
          withChecker: false,
          withAddLink: false }
  return {
    vouchers,
    vouchersOrdered,
    vouchersEntities: ServiceSelectors.getServiceVouchersEntities(state, ownProps),
    serviceId: CommonServiceSelectors.getServiceId(state, ownProps),
    contentProps
  }
}

const actions = [
  commonActions,
  serviceActions,
  commonServiceActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(ServiceVouchers))
