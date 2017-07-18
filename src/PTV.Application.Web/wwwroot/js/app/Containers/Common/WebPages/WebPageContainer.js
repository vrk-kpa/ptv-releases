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
import classNames from 'classnames'

// components
import * as PTVValidatorTypes from '../../../Components/PTVValidators'
import { PTVAutoComboBox, PTVTextInputNotEmpty, PTVUrlChecker, PTVTextAreaNotEmpty, PTVCheckBox } from '../../../Components'
import { LocalizedComboBox } from '../../Common/localizedData'

// selectors
import * as CommonSelectors from '../Selectors'

// schemas
import { CommonSchemas } from '../../Common/Schemas'

// actions
import * as commonActions from '../Actions'
import mapDispatchToProps from '../../../Configuration/MapDispatchToProps'

export const WebPageContainer = ({
    readOnly,
    language,
    translationMode,
    id,
    index,
    actions,
    intl,
    count,
    componentClass,
    webPageTypes,
    isFetching,
    areDataValid,
    messages,
    urlAddress,
    urlExists,
    shouldValidate,
    withName,
    name,
    withTypes,
    withChecker,
    typeId,
    withOrder,
    isNew,
    onAddWebPage,
    orderNumber,
    splitContainer,
    customConfiguration,
    withAdditionInfo,
    additionalInformation
    }) => {
  const createArray = (number) => {
    var numbers = []
    for (var i = 1; i < number; i++) {
      numbers.push({ id:i, name:i })
    }
    return numbers
  }

  const checkUrl = value => {
    if (customConfiguration.onCustomCheckUrl) {
      customConfiguration.onCustomCheckUrl(value, id)
    } else {
      actions.apiCall(['webPages', id],
            { endpoint: 'url/Check', data: { urlAddress: value, id: id } }, null, CommonSchemas.WEB_PAGE)
    }
  }

  const onInputChange = input => value => {
    if (!isNew) {
      if (customConfiguration.onCustomInputChange) {
        customConfiguration.onCustomInputChange(id, input, value)
      } else {
        actions.onEntityInputChange('webPages', id, input, value)
      }
    } else {
      onAddWebPage([{
        id: id,
        [input]: value,
        orderNumber: orderNumber || 1
      }])
    }
  }

  const validators = [PTVValidatorTypes.IS_REQUIRED]

  const renderChecker = () => {
    return <PTVUrlChecker
      componentClass='url-checker col-xs-12'
      messages={{ tooltip: messages.urlTooltip,
        label: messages.urlLabel,
        placeholder: messages.urlPlaceholder,
        button: messages.urlButton,
        checkerInfo: messages.urlCheckerInfo }}
      id={id}
      value={urlAddress}
      checkUrlCallback={checkUrl}
      blurCallback={onInputChange('urlAddress')}
      showPreloader={isFetching}
      showMessage={areDataValid}
      urlExists={urlExists}
      inputValidators={shouldValidate || (withName && name.length > 0) ? validators : []}
      readOnly={readOnly && translationMode === 'none'}
      disabled={translationMode === 'view'}
      translationMode={translationMode}
      maxLength={500} />
  }

  const { formatMessage } = intl

  return (
    <div className={classNames('channel-webPage-container item-row', componentClass)}>
      <div className='row'>
        { customConfiguration.customRender && customConfiguration.customRender(index)}
        { count > 1 && withOrder
        ? <PTVAutoComboBox
          componentClass={splitContainer ? 'col-xs-12' : 'col-lg-2'}
          value={orderNumber}
          values={createArray(count + 1)}
          changeCallback={onInputChange('orderNumber')}
          label={formatMessage(messages.orderTitle)}
          name='webPageOrder'
          clearable={false}
          order={90}
          readOnly={readOnly || translationMode === 'view' || translationMode === 'edit'}
          className='limited w280'
        />
        : null}
        { withTypes
        ? <LocalizedComboBox
          componentClass={splitContainer ? 'col-xs-12' : 'col-lg-2 col-md-4'}
          value={typeId}
          values={webPageTypes}
          label={formatMessage(messages.typeTitle)}
          validatedField={messages.typeTitle}
          tooltip={formatMessage(messages.typeTooltip)}
          changeCallback={onInputChange('typeId')}
          name='webPageTypeTitle'
          order={100}
          inputProps={{ 'maxLength':'50' }}
          validators={shouldValidate ? validators : []}
          readOnly={readOnly && translationMode === 'none'}
          disabled={translationMode === 'view'}
          className='limited w480'
            />
        : null}
        { withName
        ? <PTVTextInputNotEmpty
          componentClass={splitContainer ? 'col-xs-12' : 'col-lg-8 col-md-6'}
          label={formatMessage(messages.nameTitle)}
          tooltip={formatMessage(messages.nameTooltip)}
          placeholder={formatMessage(messages.namePlaceholder)}
          value={name}
          blurCallback={onInputChange('name')}
          maxLength={110}
          name='webPageName'
          readOnly={readOnly && translationMode === 'none'}
          disabled={translationMode === 'view'} />
        : null }
        { withChecker ? renderChecker() : null }
        { withAdditionInfo
        ? <PTVTextAreaNotEmpty
          componentClass={splitContainer ? 'col-xs-12' : 'col-lg-8 col-md-6'}
          minRows={3}
          maxLength={150}
          label={intl.formatMessage(messages.additionlInformationTitle)}
          placeholder={intl.formatMessage(messages.additionlInformationPlaceholder)}
          value={additionalInformation}
          name='webPageAdditionInformation'
          blurCallback={onInputChange('additionalInformation')}
          disabled={translationMode === 'view'}
          readOnly={readOnly && translationMode === 'none'}
          validatedField={messages.additionlInformationTitle} />
        : null }
      </div>
    </div>
  )
}

WebPageContainer.propTypes = {
  id: PropTypes.string.isRequired,
  messages: PropTypes.object.isRequired,
  readOnly: PropTypes.bool,
  shouldValidate: PropTypes.bool,
  isNew: PropTypes.bool,
  componentClass: PropTypes.string,
  count: PropTypes.number,
  withTypes: PropTypes.bool,
  withName: PropTypes.bool,
  withOrder: PropTypes.bool,
  withAdditionInfo: PropTypes.bool,
  withChecker: PropTypes.bool,
  onAddWebPage: PropTypes.func,
  customConfiguration: PropTypes.any
}

WebPageContainer.defaultProps = {
  withTypes: false,
  withOrder: false,
  isNew: false,
  readOnly: false,
  shouldValidate: false,
  withName: false,
  withAdditionInfo: false,
  withActive: false,
  withChecker: true,
  customConfiguration:{}
}

function mapStateToProps (state, ownProps) {
  const useCustom = ownProps.customConfiguration
  return {
    webPageTypes: useCustom && useCustom.customTypesSelector
                    ? useCustom.customTypesSelector(state, ownProps)
                    : CommonSelectors.getWebPageTypesObjectArray(state, ownProps),
    typeId: useCustom && useCustom.customTypeIdSelector
                    ? useCustom.customTypeIdSelector(state, ownProps)
                    : CommonSelectors.getWebPageTypeId(state, ownProps),
    urlAddress: useCustom && useCustom.customUrlAddressSelectors
                    ? useCustom.customUrlAddressSelectors(state, ownProps)
                    : CommonSelectors.getWebPageUrlAddress(state, ownProps),
    name: useCustom && useCustom.customNameSelectors
                    ? useCustom.customNameSelectors(state, ownProps)
                    : CommonSelectors.getWebPageName(state, ownProps),
    orderNumber: useCustom && useCustom.customOrderNumberSelectors
                    ? useCustom.customOrderNumberSelectors(state, ownProps)
                    : CommonSelectors.getWebPageOrderNumber(state, ownProps),
    urlExists: useCustom && useCustom.customUrlExistsSelectors
                    ? useCustom.customUrlExistsSelectors(state, ownProps)
                    : CommonSelectors.getWebPageUrlExists(state, ownProps),
    isFetching: useCustom && useCustom.customIsFetchingSelectors
                    ? useCustom.customIsFetchingSelectors(state, ownProps)
                    : CommonSelectors.getWebPageIsFetching(state, ownProps),
    areDataValid: useCustom && useCustom.customAreDataValidSelectors
                    ? useCustom.customAreDataValidSelectors(state, ownProps)
                    : CommonSelectors.getWebPageAreDataValid(state, ownProps),
    additionalInformation: useCustom && useCustom.customAdditionalInformationSelectors
                    ? useCustom.customAdditionalInformationSelectors(state, ownProps)
                    : CommonSelectors.getWebPageAdditionalInformation(state, ownProps)
  }
}

const actions = [
  commonActions
]

export default connect(mapStateToProps, mapDispatchToProps(actions))(injectIntl(WebPageContainer))
