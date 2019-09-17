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
import { Modal, ModalTitle, ModalContent, ModalActions, Button } from 'sema-ui-components'
import { compose } from 'redux'
import { defineMessages, injectIntl } from 'util/react-intl'
import withState from 'util/withState'
import styles from './styles.scss'
import { connect } from 'react-redux'
import { EntitySelectors } from 'selectors'
import { formValueSelector, change } from 'redux-form/immutable'
import { formTypesEnum } from 'enums'
import withFormStates from 'util/redux-form/HOC/withFormStates'

const messages = defineMessages({
  closeDialogButtonTitle: {
    id: 'Buttons.Cancel.Title',
    defaultMessage: 'Peruuta'
  },
  clearDialogButtonTitle: {
    id: 'Buttons.Delete.Title',
    defaultMessage: 'Poista'
  },
  clearGeneralDescriptionTitle: {
    id: 'ModalDialog.ClearSelectedGeneralDescription.Title',
    defaultMessage: 'Haluatko poistaa liitoksen pohjakuvaukseen?'
  },
  clearGeneralDescriptionText: {
    id: 'ModalDialog.ClearSelectedGeneralDescription.Text',
    defaultMessage: 'Menetät pohjakuvauksesta tulevat kuvaustekstit. Luokittelutiedot, kuten kohderyhmä, palveluluokka ja asiasanat jäävät palvelulle.'
  }
})
const getGeneralDescriptionSearchFormSelector = formValueSelector(formTypesEnum.GENERALDESCRIPTIONSEARCHFORM)
const getServiceFormSelector = formValueSelector(formTypesEnum.SERVICEFORM)

const ClearModal = ({
  isOpen,
  updateUI,
  change,
  onClear,
  dispatch,
  intl: { formatMessage }
}) => {
  const handleOnClose = () => updateUI('isOpen', !isOpen)
  const handleOnClear = () => {
    let allOntologyTerms,
      allTargetGroups,
      allServiceClasses,
      allLifeEvents,
      allIndustrialClasses,
      serviceChargeType
    dispatch(({ getState }) => {
      const state = getState()
      const generalDescriptionId = getGeneralDescriptionSearchFormSelector(state, 'generalDescriptionId')
      const generalDescription = EntitySelectors.generalDescriptions.getEntity(state, { id : generalDescriptionId })
      if (generalDescriptionId) {
        allTargetGroups = generalDescription.get('targetGroups')
          .toOrderedSet().union(getServiceFormSelector(state, 'targetGroups'))
          .toOrderedSet().subtract(getServiceFormSelector(state, 'overrideTargetGroups'))
        allServiceClasses = generalDescription.get('serviceClasses')
          .toOrderedSet().union(getServiceFormSelector(state, 'serviceClasses')).toOrderedSet()
        allOntologyTerms = generalDescription.get('ontologyTerms')
          .toOrderedSet().union(getServiceFormSelector(state, 'ontologyTerms')).toOrderedSet()
        allLifeEvents = generalDescription.get('lifeEvents')
          .toOrderedSet().union(getServiceFormSelector(state, 'lifeEvents')).toOrderedSet()
        allIndustrialClasses = generalDescription.get('industrialClasses')
          .toOrderedSet().union(getServiceFormSelector(state, 'industrialClasses')).toOrderedSet()
        serviceChargeType = getServiceFormSelector(state, 'chargeType')
        const serviceChargeTypeInfo = serviceChargeType.get('additionalInformation')
        const generalChargeTypeId = generalDescription.getIn(['chargeType', 'chargeType'])
        const generalChargeTypeInfo = generalDescription.getIn(['chargeType', 'additionalInformation'])
        serviceChargeType = serviceChargeType.set('chargeType', generalChargeTypeId)
        serviceChargeType = serviceChargeType.set('additionalInformation',
          generalChargeTypeInfo.map((x, index) => serviceChargeTypeInfo.get(index) || x))
      }
    })
    change(formTypesEnum.SERVICEFORM, 'ontologyTerms', allOntologyTerms)
    change(formTypesEnum.SERVICEFORM, 'targetGroups', allTargetGroups)
    change(formTypesEnum.SERVICEFORM, 'serviceClasses', allServiceClasses)
    change(formTypesEnum.SERVICEFORM, 'lifeEvents', allLifeEvents)
    change(formTypesEnum.SERVICEFORM, 'industrialClasses', allIndustrialClasses)
    change(formTypesEnum.SERVICEFORM, 'overrideTargetGroups', null)
    change(formTypesEnum.SERVICEFORM, 'chargeType', serviceChargeType)
    updateUI('isOpen', false)
    onClear()
  }
  return (
    <Modal
      isOpen={isOpen}
      onRequestClose={handleOnClose}
    >
      <ModalTitle title={formatMessage(messages.clearGeneralDescriptionTitle)} />
      <ModalContent>
        {formatMessage(messages.clearGeneralDescriptionText)}
      </ModalContent>
      <ModalActions>
        <div className={styles.buttonGroup}>
          <Button
            small
            onClick={handleOnClear}
            children={formatMessage(messages.clearDialogButtonTitle)}
          />
          <Button
            link
            onClick={handleOnClose}
            children={formatMessage(messages.closeDialogButtonTitle)}
          />
        </div>
      </ModalActions>
    </Modal>
  )
}

export default compose(
  withState({
    key: 'clearSelectedGeneralDescription',
    initialState: {
      isOpen: false
    },
    redux: true
  }),
  connect(null, {
    change
  }),
  injectIntl,
  withFormStates
)(ClearModal)
