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
import React, { PureComponent } from 'react'
import PropTypes from 'prop-types'
import { FieldArray } from 'redux-form/immutable'
import cx from 'classnames'
import styles from './styles.scss'
import { PTVIcon } from 'Components'
import { Button, Label } from 'sema-ui-components'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { withFormStates, injectFormName } from 'util/redux-form/HOC'
import { Popup } from 'appComponents'
import { injectIntl, defineMessages } from 'react-intl'
import withState from 'util/withState'
import { Map } from 'immutable'
import { getIsAddingNewLanguage } from 'selectors/formStates'

export const messages = defineMessages({
  buttonOk: {
    id: 'Containers.Services.NameOverwriteDialog.Accept',
    defaultMessage: 'Kyllä'
  },
  buttonCancel: {
    id: 'Buttons.Cancel.Title',
    defaultMessage: 'Peruuta'
  },
  removalPlaceholder: {
    id: 'Common.HOC.AsCollection.RemovalPlaceholder',
    defaultMessage: 'Tämän elementin poistaminen vaikuttaa muihin kieliversioihin, sillä toisessa kieliversiossa on sisältöä.'
  }
})

const asCollection = ({
  name,
  pluralName = name + 's',
  simple,
  shouldHideControls,
  addBtnTitle,
  confirmDeleteMessage
}) => WrappedComponent => {
  class InnerComponent extends PureComponent {
    renderComponent = ({ fields, meta, ...props }) => {
      const innerShouldHideControls = shouldHideControls || this.props.isReadOnly
      const handleOnRemove = index => {
        if (fields.length <= 0) {
          return
        }
        fields.remove(index)
        confirmDeleteMessage && props.updateUI('isOpen', props.isOpen.set(index, false))
      }
      const handleOnAdd = e => {
        e.preventDefault()
        if (fields.length === 0) {
          fields.push(this.props.defaultItem)
        }
        fields.push(this.props.defaultItem)
      }
      const collectionClass = cx(
        styles.collection,
        {
          [styles.simple]: simple,
          [styles.readMode]: innerShouldHideControls
        }
      )
      const newFields = fields.length === 0 && [undefined] || fields
      const collectionItemRemoveClass = cx(
        styles.collectionItemRemove,
        {
          [styles.hideRemove]: fields.length === 0 || innerShouldHideControls
        }
      )
      return (
        <div className={collectionClass}>
          <div className={styles.collectionBody}>
            {newFields.map((field, index) =>
              <div key={index} className={styles.collectionItem}>
                <div className={styles.collectionItemContent}>
                  {!props.isReadOnly && confirmDeleteMessage &&
                  <div className={styles.collectionItemHelpText}>
                    <div className='row'>
                      <div className={this.props.isCompareMode ? 'col-lg-12' : 'col-lg-24'}>
                        <Label infoLabel>
                          {this.props.intl.formatMessage(messages.removalPlaceholder)}
                        </Label>
                      </div>
                    </div>
                  </div>}
                  <WrappedComponent
                    key={index}
                    index={index}
                    name={field || `${this.props.name || pluralName}[${index}]`}
                    {...props}
                  />
                </div>
                {!props.isReadOnly &&
                  <div className={collectionItemRemoveClass}>
                    {confirmDeleteMessage &&
                    <div>
                      <Popup
                        trigger={
                          <PTVIcon
                            name={'icon-cross'}
                        />}
                        open={props.isOpen.get(index)}
                        onClose={() => props.updateUI('isOpen', props.isOpen.set(index, false))}
                        onOpen={() => props.updateUI('isOpen', props.isOpen.set(index, true))}
                        position='top center'
                        maxWidth='mW300'
                      >
                        <div>
                          {this.props.intl.formatMessage(confirmDeleteMessage)}
                          <div className={styles.buttonGroup}>
                            <Button small onClick={() => handleOnRemove(index)}>
                              {this.props.intl.formatMessage(messages.buttonOk)}</Button>
                            <Button small secondary onClick={() => props.updateUI('isOpen', props.isOpen.set(index, false))}>
                              {this.props.intl.formatMessage(messages.buttonCancel)}
                            </Button>
                          </div>
                        </div>
                      </Popup>
                    </div> ||
                    <PTVIcon
                      name={'icon-cross'}
                      onClick={() => handleOnRemove(index)}
                  />}
                  </div>
                }
              </div>
            )}
          </div>
          {!innerShouldHideControls &&
            <div className={styles.collectionFoot}>
              <Button link onClick={handleOnAdd} disabled={this.props.isAddingNewLanguage}>{addBtnTitle}</Button>
            </div>}
        </div>
      )
    }
    render () {
      return (
        <FieldArray
          name={this.props.name || pluralName}
          component={this.renderComponent}
          {...this.props}
        />
      )
    }
  }

  InnerComponent.propTypes = {
    defaultItem: PropTypes.any,
    name: PropTypes.string,
    isReadOnly: PropTypes.bool,
    isCompareMode: PropTypes.bool,
    isAddingNewLanguage: PropTypes.bool
  }

  return compose(
    injectIntl,
    injectFormName,
    withFormStates,
    connect((state, { formName }) => ({
      isAddingNewLanguage: getIsAddingNewLanguage(formName)(state)
    })),
    withState({
      key: 'confirmDialog',
      initialState: {
        isOpen: Map()
      }
    }),
  )(InnerComponent)
}

export default asCollection
