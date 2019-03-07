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
import { injectIntl, intlShape, defineMessages } from 'util/react-intl'
import withFormStates from 'util/redux-form/HOC/withFormStates'
import { getIsAddingNewLanguage } from 'selectors/formStates'
import { DragDropContext, Droppable, Draggable } from 'react-beautiful-dnd'
import withState from 'util/withState'
import withPath from 'util/redux-form/HOC/withPath'
import injectFormName from 'util/redux-form/HOC/injectFormName'
import { Map } from 'immutable'
import { StatelessAccordion } from 'appComponents/Accordion'
import createCachedSelector from 're-reselect'
import { arraySplice, fieldArrayPropTypes } from 'redux-form/immutable'
import { getFormValueWithPath } from 'selectors/base'
import NoDataLabel from 'appComponents/NoDataLabel'
import { isUndefined } from 'lodash'

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
    defaultMessage: 'Tämän elementin poistaminen vaikuttaa muihin kieliversioihin, sillä toisessa kieliversiossa on sisältöä.' // eslint-disable-line
  }
})

const getPath = ({ path, collectionName }) => {
  return [path, collectionName].filter(x => !!x).join('.')
}
const getItems = createCachedSelector(
  getFormValueWithPath(getPath),
  result => result || Map()
)(
  (_, { formName, path, collectionName }) => {
    return `${formName}.${getPath({ path, collectionName })}`
  }
)

class AccordionCollectionItem extends PureComponent {
  innerShouldHideControls = () => {
    return this.props.shouldHideControls || this.props.isReadOnly
  }
  handleOnAdd = e => {
    const { fields, defaultItem, firstDefaultItem, updateUI, customAddCallback, dispatch } = this.props
    e.preventDefault()
    updateUI('activeIndex', fields.length)
    updateUI('focusIndex', fields.length)
    if (firstDefaultItem && fields.length === 0) {
      fields.push(firstDefaultItem)
      if (typeof customAddCallback === 'function') {
        dispatch(customAddCallback({ path:`${this.props.path}[0]`, formName:this.props.formName }))
      }
    } else {
      fields.push(defaultItem)
    }
  }
  handleOnDragEnd = ({ source, destination }) => {
    if (!destination) return
    const { index: sourceIndex } = source
    const { index: destinationIndex } = destination
    if (sourceIndex !== destinationIndex) {
      // We need to get sourceChild before splice //
      const sourceChild = this.props.items.get(sourceIndex)
      // Make focus index follow dragged item //
      if (this.props.focusIndex !== -1) {
        this.props.updateUI('focusIndex', destinationIndex)
      }
      // Insert dragged child at new position //
      this.props.arraySplice(
        this.props.formName,
        this.props.path,
        sourceIndex,
        1
      )
      // Remove dragged child from old position //
      this.props.arraySplice(
        this.props.formName,
        this.props.path,
        destinationIndex,
        0,
        sourceChild
      )
    }
  }
  handleTitleOnClick = index => {
    const { focusIndex, updateUI, activeIndex } = this.props
    if (focusIndex !== index) {
      updateUI('focusIndex', index)
    }
    updateUI(
      'activeIndex',
      index === activeIndex
        ? -1
        : index
    )
  }
  handleOnRemove = index => {
    const { focusIndex, updateUI } = this.props
    if (index <= focusIndex && index !== 0) {
      updateUI('focusIndex', focusIndex - 1)
    }
  }
  render () {
    const innerShouldHideControls = this.innerShouldHideControls()
    const {
      WrappedComponent,
      RemoveButton,
      AddButton,
      Title,
      fields,
      dragAndDrop,
      draggable,
      items,
      addBtnTitle,
      addNewBtnTitle,
      comparable,
      disclaimerMessage,
      ...wrappedComponentProps
    } = this.props
    const itemCount = items && items.size
    const hasMany = itemCount > 1
    const isEmpty = itemCount === 0
    const showPlaceholder = isEmpty && this.props.isReadOnly
    const collectionClass = cx(
      styles.collection,
      {
        [styles.simple]: this.props.simple,
        [styles.stacked]: this.props.stacked,
        [styles.readMode]: innerShouldHideControls,
        [styles.nested]: this.props.nested,
        [styles.showNumbering]: hasMany
      }
    )
    return (
      <div className={collectionClass}>
        <DragDropContext onDragEnd={this.handleOnDragEnd}>
          <div className={styles.collectionBody}>
            <Droppable droppableId='droppable'>
              {provided => (
                <div ref={provided.innerRef}>
                  {showPlaceholder && <NoDataLabel /> || fields.map((field, index) => {
                    const isOpen = this.props.activeIndex === index
                    const hasFocus = this.props.focusIndex === index
                    const collectionItemClass = cx(
                      styles.collectionItem,
                      {
                        [styles.isOpen]: isOpen,
                        [styles.hasFocus]: hasFocus,
                        [styles.placeholder]: showPlaceholder
                      }
                    )
                    const isDefaultDraggable = (this.props.activeIndex === -1 ||
                      this.props.activeIndex >= itemCount) &&
                      !this.props.isCompareMode &&
                      hasMany
                    const showDisclaimer = !!disclaimerMessage && index !== 0 && !items.getIn([index, 'id'])
                    return (
                      <div key={`${this.props.collectionName}_${index}`} className={styles.collectionItemWrap}>
                        <Draggable draggableId={field}>
                          {provided => (
                            <div className={collectionItemClass}>
                              <div ref={provided.innerRef} style={provided.draggableStyle}>
                                <StatelessAccordion.Title
                                  dragAndDrop={dragAndDrop}
                                  comparable={comparable}
                                  showDisclaimer={showDisclaimer}
                                  disclaimerMessage={disclaimerMessage}
                                  hasMany={hasMany}
                                  isReadOnly={this.props.isReadOnly}
                                  title={Title && <Title
                                    items={items}
                                    index={index}
                                    className={styles.wrap}
                                    comparable={comparable}
                                    {...wrappedComponentProps}
                                  />}
                                  active={isOpen}
                                  focused={hasFocus}
                                  validate={false}
                                  onClick={() => this.handleTitleOnClick(index)}
                                  draggable={
                                    isUndefined(draggable)
                                      ? isDefaultDraggable
                                      : isDefaultDraggable && draggable
                                  }
                                  draggableProps={provided.dragHandleProps}
                                  RemoveButton={(
                                    <div
                                      onClick={() => this.handleOnRemove(index)}
                                    >
                                      <RemoveButton
                                        items={items}
                                        index={index}
                                        fields={this.props.fields}
                                        innerShouldHideControls={innerShouldHideControls}
                                        isReadOnly={this.props.isReadOnly}
                                      />
                                    </div>
                                  )}
                                />
                                <StatelessAccordion.Content
                                  active={isOpen && !this.props.isReadOnly}
                                  focused={hasFocus}
                                  dragAndDrop={dragAndDrop}
                                  hasMany={hasMany}
                                >
                                  <div className={styles.collectionItemContent}>
                                    <WrappedComponent
                                      key={index}
                                      index={index}
                                      name={field || `${this.props.collectionName}[${index}]`}
                                      {...wrappedComponentProps}
                                    />
                                  </div>
                                </StatelessAccordion.Content>
                              </div>
                              {provided.placeholder}
                            </div>
                          )}
                        </Draggable>
                      </div>
                    )
                  })}
                  {provided.placeholder}
                </div>
              )}
            </Droppable>
          </div>
          {!innerShouldHideControls && (
            <div className={styles.collectionFoot}>
              <AddButton
                secondary={isEmpty}
                small={isEmpty}
                link={!isEmpty}
                onClick={this.handleOnAdd}
                disabled={this.props.isAddingNewLanguage}
                intl={this.props.intl}
                addBtnTitle={isEmpty && addNewBtnTitle || addBtnTitle}
                fields={fields}
                setOpenIndex={this.props.setOpenIndex}
              />
            </div>
          )}
        </DragDropContext>
      </div>
    )
  }
}

AccordionCollectionItem.propTypes = {
  stacked: PropTypes.bool,
  nested: PropTypes.bool,
  isAddingNewLanguage: PropTypes.bool,
  intl: intlShape,
  addBtnTitle: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  addNewBtnTitle: PropTypes.oneOfType([
    PropTypes.string,
    PropTypes.object
  ]),
  activeIndex: PropTypes.number,
  focusIndex: PropTypes.number,
  setOpenIndex: PropTypes.func,
  arraySplice: PropTypes.func,
  updateUI: PropTypes.func,
  collectionName: PropTypes.string,
  formName: PropTypes.string,
  path: PropTypes.string,
  shouldHideControls: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  simple: PropTypes.bool,
  fields: fieldArrayPropTypes.fields,
  items: PropTypes.object,
  defaultItem: PropTypes.object,
  firstDefaultItem: PropTypes.object,
  WrappedComponent: PropTypes.any,
  RemoveButton: PropTypes.any,
  Title: PropTypes.any,
  AddButton: PropTypes.any,
  dragAndDrop: PropTypes.bool,
  draggable: PropTypes.bool,
  comparable: PropTypes.bool,
  isCompareMode: PropTypes.bool,
  disclaimerMessage: PropTypes.object,
  customAddCallback: PropTypes.func,
  dispatch: PropTypes.func
}

export default compose(
  injectIntl,
  withFormStates,
  withPath,
  injectFormName,
  connect((state, { formName, path, collectionName }) => ({
    items: getItems(state, { formName, path, collectionName }),
    isAddingNewLanguage: getIsAddingNewLanguage(formName)(state),
    // Intentionaly overwriting path from withPath HOC //
    path: getPath({ path, collectionName })
  }), {
    arraySplice
  }),
  withState({
    key: ({ path }) => path,
    initialState: {
      activeIndex: -1,
      focusIndex: -1
    },
    redux: true
  })
)(AccordionCollectionItem)
