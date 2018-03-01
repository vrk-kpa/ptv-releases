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
import { EntitySelectors } from 'selectors'
import { isTranslationReceived } from './selectors'
import { getContentLanguageCode } from 'selectors/selections'
import { Button } from 'sema-ui-components'
import { ComposedCell, ValueCell, NoDataLabel } from 'appComponents'
import { defineMessages, injectIntl } from 'react-intl'
import { OrganizationCell } from 'appComponents/TranslationOrderDialog/components'
import styles from './styles.scss'
import { copyToClip } from 'util/helpers'
import { mergeInUIState } from 'reducers/ui'
import { change } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import { List } from 'immutable'

const messages = defineMessages({
  copyText: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.Copy.Text',
    defaultMessage: 'Jos haluat ottaa yhteyttä käännöstoimistoon sähköpostitse, sisällytä yhteydenottoon tilauksen. Voit kopioida tiedot alla olsevasta linkistä.'
  },
  copyLink: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.Copy.Link',
    defaultMessage: 'Kopioi tilauksen tunnisteet'
  },
  reorderText: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.Reorder.Text',
    defaultMessage: 'Jos lähdekieleen tulle muutoksia tai kieliversiohin kaivataan myöhemmin uusia käännöksiä, voit lähettää uusintakäännöksen tämän tilauksen alta. Käännös tehdää silloin vain muuttuneilta osin. Tilaa uusintakäännös-linkki näytetään, kun käännös on tallennettu.'
  },
  reorderLink: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.Reorder.Link',
    defaultMessage: 'Tilaa uusintakäännös'
  },
  updatedContentText: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.UpdatedContent.Text',
    defaultMessage: 'Jos käännöksen saavuttua teidän organisaatiossanne tehdään teksteinin muutoskia, voit lähettää viimeisimmätversiot tiedoksi käännöstoimiston käännösmuistiin täman tilauksen. Lähetä tiedoksi -linkki näytetään, kun käännös on tallennettu.'
  },
  updatedContentLink: {
    id: 'Containers.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.UpdatedContent.Link',
    defaultMessage: 'Lähetä tiedoksi'
  },
  additionalInformationTitle: {
    id: 'Components.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.AdditionalInformation.Title',
    defaultMessage: 'Lisätieto:'
  },
  orderInformation: {
    id: 'Components.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.orderInformation.Title',
    defaultMessage: 'Tilauksen tunnisteet:'
  },
  orderContentId: {
    id: 'Components.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.orderContentId.Title',
    defaultMessage: 'Sisältö id:'
  },
  orderIdentifier: {
    id: 'Components.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.orderIdentifier.Title',
    defaultMessage: 'Tilausnumero:'
  },
  orderContentName: {
    id: 'Components.TranslationOrderDialog.OrderTable.AdditionalInformationDetail.orderContentName.Title',
    defaultMessage: 'Nimi:'
  }
})

const RenderAdditionalInformationDetail = ({
  intl: { formatMessage },
  isTranslationOrderArrived,
  isContentOfTargetLanguageChanged,
  inputData,
  organizationName,
  language,
  mergeInUIState,
  change
}) => {
  const handleUpdateContentTransalation = () => { }

  const additionalInfo = inputData && inputData.get('additionalInformation') || ''
  const orderContentId = inputData && inputData.get('contentId') || ''
  const orderIdentifier = inputData && inputData.get('orderIdentifier') || ''
  const entityOrganizationId = inputData && inputData.get('entityOrganizationId') || ''
  const entityName = inputData && inputData.get('entityName') || ''
  const targetLanguage = inputData && inputData.get('targetLanguage') || null

  const handleCopyTranslationIdentifier = () => {
    const orderTags = `${formatMessage(messages.orderContentId)} ${orderContentId}, ${formatMessage(messages.orderIdentifier)} ${orderIdentifier}, ${formatMessage(messages.orderContentName)} ${organizationName} ${entityName}`
    copyToClip(orderTags)
  }

  const handleReorderTransalation = () => {
    mergeInUIState({
      key: 'translationOrderDialogActiveIndex',
      value: {
        activeIndex: 0
      }
    })
    mergeInUIState({
      key: 'translationOrderDialogReorderLanguage',
      value: {
        reorderLang: targetLanguage
      }
    })
    change(formTypesEnum.TRANSLATIONORDERFORM, 'requiredLanguages', List([targetLanguage]))
  }
  return (
    <div className={styles.details}>

      <div className={styles.detailsHead}>
        <div className={styles.detailsHeadRow}>
          <div className='row'>
            <div className='col-lg-4'>
              {formatMessage(messages.additionalInformationTitle)}
            </div>
            <div className='col-lg-20'>
              {additionalInfo &&
                <span>{additionalInfo}</span> || <NoDataLabel />
              }
            </div>
          </div>
        </div>
        <div className={styles.detailsHeadRow}>
          <div className='row'>
            <div className='col-lg-4'>
              {formatMessage(messages.orderInformation)}
            </div>
            <div className='col-lg-20'>
              {orderContentId &&
                <span className={styles.infoPart}>
                  <span>{formatMessage(messages.orderContentId)}</span>
                  <span>{orderContentId}</span>
                </span>
              }
              {orderIdentifier &&
                <span className={styles.infoPart}>
                  <span>{formatMessage(messages.orderIdentifier)}</span>
                  <span>{orderIdentifier}</span>
                </span>
              }
              {(entityOrganizationId || entityName) &&
                <span className={styles.infoPart}>
                  <span>{formatMessage(messages.orderContentName)}</span>
                  <span className={styles.inlineInfo}>
                    <OrganizationCell organizationId={entityOrganizationId} />
                    {entityName}
                  </span>
                </span>
              }
            </div>
          </div>
        </div>
      </div>

      <div className={styles.detailsBody}>
        <ComposedCell
          componentClass={styles.subCell}
          sub={<ValueCell value={formatMessage(messages.copyText)} />}>
          <Button
            link
            onClick={handleCopyTranslationIdentifier}
            children={formatMessage(messages.copyLink)}
          />
        </ComposedCell>
        <ComposedCell
          componentClass={styles.subCell}
          sub={<ValueCell value={formatMessage(messages.reorderText)} />}>
          {isTranslationOrderArrived &&
            <Button
              link
              onClick={handleReorderTransalation}
              children={formatMessage(messages.reorderLink)}
            />
          }
        </ComposedCell>
        {/* <ComposedCell
          componentClass={styles.subCell}
          sub={<ValueCell value={formatMessage(messages.updatedContentText)} />}>
          {isContentOfTargetLanguageChanged &&
            <Button
              link
              onClick={handleUpdateContentTransalation}
              children={formatMessage(messages.updatedContentLink)}
            />
          }
        </ComposedCell> */}
      </div>
    </div>
  )
}

RenderAdditionalInformationDetail.propTypes = {
  intl: PropTypes.object.isRequired,
  isTranslationOrderArrived: PropTypes.bool,
  isContentOfTargetLanguageChanged: PropTypes.bool,
  inputData: PropTypes.object,
  organizationName: PropTypes.string,
  language: PropTypes.string,
  mergeInUIState: PropTypes.func,
  change: PropTypes.func
}

RenderAdditionalInformationDetail.defaultProps = {
  isTranslationOrderArrived: false,
  isContentOfTargetLanguageChanged: false
}

export default compose(
  injectIntl,
  connect((state, ownProps) => {
    const organizations = EntitySelectors.organizations.getEntities(state)
    const organizationId = ownProps.inputData && ownProps.inputData.get('entityOrganizationId')
    const organizationName = organizations.getIn([organizationId, 'displayName']) || ''
    const language = getContentLanguageCode(state, ownProps)
    return {
      organizationName,
      language,
      isTranslationOrderArrived: isTranslationReceived(state, ownProps)
    }
  }, {
    mergeInUIState,
    change
  })
)(RenderAdditionalInformationDetail)
