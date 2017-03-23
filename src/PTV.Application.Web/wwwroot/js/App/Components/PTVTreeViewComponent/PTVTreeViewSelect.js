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
import cx from 'classnames'
import React, { PropTypes, Component } from 'react'
import { connect } from 'react-redux'
import PTVTreeView from './PTVTreeView'
import PTVTreeList from './PTVTreeList'
import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl'
import PTVConfirmDialog from '../PTVConfirmDialog'
import { List, Iterable } from 'immutable'
import * as ValidationHelper from '../PTVValidations'
import '../Styles/PTVCommon.scss'
import './Styles/PTVTreeView.scss'
import PTVLabel from '../PTVLabel'
import { composePTVComponent, ValidatePTVComponent, getRequiredLabel } from '../PTVComponent'
import PTVCheckBox from '../PTVCheckBox'
import shortId from 'shortid'
import { PTVTooltip, PTVButton } from '../../Components'

const messages = defineMessages({
  deleteModalBody: {
    id: 'Components.Modal.DeleteBody',
    defaultMessage: 'Are you sure you want to remove class {parent} and the subclasses {childrens}?'
  },
  deleteModalAccpet: {
    id: 'Components.Button.Ok',
    defaultMessage: 'Kyllä'
  }
})

class PTVTreeViewSelect extends Component {
  constructor (props) {
    super(props)
    this.state = {
      ...this.state,
      initialized: false,
      deleteConfirm: false,
      parentToDelete:'',
      childrensToDelete:'',
      showTree: !props.treeLinkLabel }
  }

  onNodeSelect = nodeId => (event) => {
    this.props.onNodeSelect(nodeId, event.target.checked)
  }

  onNodeClick = (node) => () => {
    if (this.props.onNodeClick) {
      this.props.onNodeClick(node)
    }
  }

  onToggle = (node) => {
    this.props.onNodeToggle(node)
  }

  renderName = (node, showTooltip) => {
    return (
      <PTVTooltip
        readOnly={!showTooltip}
        labelContent={node.get('name')}
        tooltip={node.get('name')}
        attachToBody
      />
    )
  }

  renderNode = (node, isRootLevel) => {
    const showLabelOutsideCheckbox = this.props.onNodeClick != null
    return (
      <div className={cx('ptv-tree-node', { 'small': !isRootLevel })}>
        <PTVCheckBox
          noCheckableLabel
          small={!isRootLevel} id={shortId.generate()}
          nodeId={node.get('id')}
          isSelectedSelector={this.props.isSelectedSelector}
          language={this.props.language}
          isDisabledSelector={this.props.isDisabledSelector}
          onClick={this.onNodeSelect(node.get('id'))}
        >
          { showLabelOutsideCheckbox ? null : this.renderName(node, this.props.nodeShowTooltip) }
        </PTVCheckBox>
        { showLabelOutsideCheckbox &&
          <div onClick={this.onNodeClick(node)}>
            <PTVLabel>
              { node.get('name') }
            </PTVLabel>
          </div>
        }
      </div>
    )
  }

  onTreeLinkClick = () => {
    this.setState({ showTree:true })
  }

  renderTreeLink = () => {
    return (
      <div className='button-group left'>
        {this.state.showTree
        ? <PTVLabel>
          { this.props.treeLinkLabel }
        </PTVLabel>
        : <PTVButton type='link' onClick={this.onTreeLinkClick}>
          { this.props.treeLinkLabel }
        </PTVButton>}
      </div>
    )
  }

  render () {
    const { formatMessage } = this.props.intl
    return (
      !this.props.readOnly
      ? <div className={cx(this.state.errorClass, this.props.className)}>
        <PTVConfirmDialog
          show={this.state.deleteConfirm}
          acceptCallback={this.acceptRemoveCallback}
          closeCallback={this.closeCallback}
          text={formatMessage(messages.deleteModalBody, {
            parent:this.state.parentToDelete,
            childrens:this.state.childrensToDelete
          })}
          acceptButton={formatMessage(messages.deleteModalAccpet)}
        />
        <div className='row'>
          <div className='col-xs-12'>
            <PTVLabel labelClass={this.props.labelClassName} tooltip={this.props.labelTooltip}>
              { getRequiredLabel(this.props) }
            </PTVLabel>
          </div>
          <div className='col-xs-12'>
            <div className='row'>
              <div className={this.props.treeViewClass} >
                <div className={this.props.treeViewInnerClass} >
                  {this.props.renderTreeLabel && this.props.renderTreeLabel()}
                  {this.props.renderTreeButton && this.props.renderTreeButton()}
                  {this.props.treeLinkLabel && this.renderTreeLink()}
                  {this.state.showTree &&
                    <PTVTreeView
                      contextId={this.props.contextId}
                      tree={this.props.treeSource}
                      renderNode={this.renderNode}
                      onNodeToggle={this.onToggle}
                      searchPlaceholder={this.props.treeSourceSearchPlaceHolder}
                      onNodeSelect={this.onNodeSelect}
                      handleToggle={this.onToggle}
                      searchable
                      renderCustomSearch={this.props.renderCustomSearch}
                      isSearching={this.props.isSearching}
                      searchInTree={this.props.onSearchInTree}
                      clearSearch={this.props.onClearSearch}
                      getNode={this.props.getNode}
                      getNodeIsCollapsed={this.props.getNodeIsCollapsed}
                      getNodeIsFetching={this.props.getNodeIsFetching}
                      getDefaultTranslationText={this.props.getDefaultTranslationText}
                      nodeHeights={this.props.nodeHeights}
                      language={this.props.language}
                    />
                  }
                </div>
              </div>
              <div className='col-md-6'>
                <PTVTreeList
                  header={this.props.treeTargetHeader}
                  onNodeRemove={this.props.onNodeRemove}
                  listSelector={this.props.getSelectedSelector}
                  readOnly={this.props.readOnly}
                  getItemSelector={this.props.getListNode
                    ? this.props.getListNode
                    : this.props.getNode}
                  isDisabledSelector={this.props.isListDisabledSelector
                    ? this.props.isListDisabledSelector
                    : this.props.isDisabledSelector}
                  language={this.props.language}
                  tagPostfix={this.props.tagPostfix}
                  tagColor={this.props.tagColor}
                  tagType={this.props.tagType}
                  tagTooltipContent={this.props.tagTooltipContent}
                  tagTooltipCallback={this.props.tagTooltipCallback}
                  getDefaultTranslationText={this.props.getDefaultTranslationText}
                />
                { this.props.children }
              </div>
              <div className='col-xs-12'>
                <ValidatePTVComponent {...this.props} valueToValidate={this.props.isAnySelected ? 'valid' : ''} />
              </div>
            </div>
          </div>
        </div>
      </div>
      : <PTVTreeList
        className={this.props.readOnlyClass}
        header={this.props.label}
        onNodeRemove={this.props.onNodeRemove}
        listSelector={this.props.getReadOnlySelectedSelector
          ? this.props.getReadOnlySelectedSelector
          : this.props.getSelectedSelector}
        readOnly={this.props.readOnly}
        getItemSelector={this.props.getListNode
          ? this.props.getListNode
          : this.props.getNode}
        language={this.props.language}
      />
    )
  }
}

PTVTreeViewSelect.propTypes = {
  treeViewClass: PropTypes.string,
  resultsClass: PropTypes.string,
  labelClassName: PropTypes.string,
  labelTooltip: PropTypes.string,
  labelContent: PropTypes.string,
  nodeShowTooltip: PropTypes.bool,
  renderTreeLabel: PropTypes.func,
  renderTreeButton: PropTypes.func,
  treeLinkLabel: PropTypes.string,
  tagPostfix: PropTypes.string,
  tagColor: PropTypes.string,
  tagType: PropTypes.string,
  tagTooltipContent: PropTypes.func,
  tagTooltipCallback: PropTypes.func
}

PTVTreeViewSelect.defaultProps = {
  treeViewClass: 'col-md-6',
  resultsClass: 'col-md-6',
  nodeShowTooltip: false
}

const getChildrenSize = (state, ownProps, nodeId) => {
  const props = { ...ownProps, id: nodeId }
  const node = ownProps.getNode(state, props)
  const children = node.get('children') || List()
  const isCollapsed = ownProps.getNodeIsCollapsed(state, props)
  const numberOfChildren = !isCollapsed && children.size > 0 && node.get('areChildrenLoaded')
    ? children.map(x => getChildrenSize(state, ownProps, x))
    : 0

  return Iterable.isIterable(numberOfChildren)
    ? numberOfChildren.reduce((prev, curr) => curr + prev) + numberOfChildren.size
    : numberOfChildren
}

function mapStateToProps (state, ownProps) {
  const nodeHeights = ownProps.treeSource && ownProps.treeSource
    .map(nodeId => getChildrenSize(state, ownProps, nodeId))
    .map(x => 50 + x * 30)
  return {
    isAnySelected: ownProps.getValidationSelector
      ? ownProps.getValidationSelector(state, ownProps).size > 0
      : ownProps.getSelectedSelector(state, ownProps).size > 0,
    nodeHeights: nodeHeights && nodeHeights.size > 0
      ? nodeHeights.toArray()
      : 50
  }
}

export default connect(mapStateToProps)(injectIntl(composePTVComponent(PTVTreeViewSelect)))
