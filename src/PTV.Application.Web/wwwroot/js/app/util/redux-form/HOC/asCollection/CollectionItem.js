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
import cx from 'classnames'
import styles from './styles.scss'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { injectIntl, defineMessages } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
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

class CollectionItem extends PureComponent {
  get innerShouldHideControls () {
    return this.props.shouldHideControls || this.props.isReadOnly
  }
  handleOnAdd = e => {
    const { fields, defaultItem } = this.props
    e.preventDefault()
    if (fields.length === 0) {
      fields.push(defaultItem)
    }
    fields.push(defaultItem)
  }
  render () {
    const collectionClass = cx(
      styles.collection,
      {
        [styles.simple]: this.props.simple,
        [styles.stacked]: this.props.stacked,
        [styles.nested]: this.props.nested,
        [styles.readMode]: this.innerShouldHideControls
      }
    )
    const newFields = this.props.fields.length === 0 && [undefined] || this.props.fields
    const {
      WrappedComponent,
      RemoveButton,
      AddButton,
      ...rest
    } = this.props
    return (
      <div className={collectionClass}>
        <div className={styles.collectionBody}>
          {newFields.map((field, index) =>
            <div key={index} className={styles.collectionItem}>
              <div className={styles.collectionItemContent}>
                <WrappedComponent
                  key={index}
                  index={index}
                  name={field || `${this.props.collectionName}[${index}]`}
                  {...rest}
                  // {...this.props}
                />
              </div>
              <RemoveButton
                index={index}
                fields={this.props.fields}
                innerShouldHideControls={this.props.innerShouldHideControls}
                isReadOnly={this.props.isReadOnly}
              />
            </div>
          )}
        </div>
        {!this.innerShouldHideControls &&
          <div className={styles.collectionFoot}>
            <AddButton
              link
              onClick={this.handleOnAdd}
              disabled={this.props.isAddingNewLanguage}
              intl={this.props.intl}
              addBtnTitle={this.props.addBtnTitle}
            />
          </div>}
      </div>
    )
  }
}

CollectionItem.propTypes = {
  stacked: PropTypes.bool,
  nested: PropTypes.bool
}

export default compose(
  injectIntl,
  withFormStates,
  connect((state, { formName }) => ({
    isAddingNewLanguage: getIsAddingNewLanguage(formName)(state)
  }))
)(CollectionItem)
