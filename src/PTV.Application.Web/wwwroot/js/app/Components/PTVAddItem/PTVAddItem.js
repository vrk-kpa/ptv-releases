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
import React, {Component, PropTypes} from 'react';
import shortId from 'shortid';

//import { defineMessages, injectIntl, intlShape, FormattedMessage } from 'react-intl';
import styles from './styles.scss';
import cx from 'classnames';
import { ButtonAdd } from '../../Containers/Common/Buttons';
import { PTVIcon, PTVLabel } from '../../Components';
import PTVAddItemHeader from './PTVAddItemHeader';
import PTVAddItemFooter from './PTVAddItemFooter';
import { List } from 'immutable';

class PTVAddItem extends Component {
	constructor(props) {
    super(props);
    this.state = {
      visible: this.props.containsData ? true : this.props.readOnly ? true : this.props.showList ? true : this.props.collapsible ? false : true
    }
  };

  toggleVisibility = (visible) => {
    this.setState({ visible: visible });
  }

  renderItem = (item, index) => {

    const id = item.id ? item.id : item; // TODO Remove after channels refactoring
    const metaData = this.props.addButtonItem ? {
      id: id,
      type: this.props.addButtonItem
    } : id;
    return (
      <div className="row" key={id}>
        <div className="col-xs-12">
          { this.props.renderItemContent(item, index) }
          { this.props.multiple && !this.props.readOnly && (index > 0 || this.props.includeReset && index === 0) ?
            <div className="remove-item">
              <PTVIcon name="icon-trash" onClick={ () => this.props.onRemoveItemClick(metaData) } />
            </div>
          : null }
        </div>
      </div>
    );
  }

  componentWillReceiveProps = (nextProps) => {
    if (this.props.readOnly != nextProps.readOnly) {
      this.state.visible = nextProps.containsData ? true : nextProps.readOnly ? true : nextProps.collapsible ? false : true;
    }
    if (this.props.showList != nextProps.showList && nextProps.showList === false) {
      this.setState({ visible: false });
    }
  }

  render() {

    return (
      <div className={cx("add-item-container", this.props.componentClass, { "multiple": this.props.items != null })}>

        { this.state.visible ?
          <PTVAddItemHeader
            messages = { this.props.messages }
            readOnly = { this.props.readOnly }
            collapsible = { this.props.collapsible }
            clearAll = { this.props.clearAll }
            visibility = { this.toggleVisibility }
            onHideAddItem = { this.props.onHideAddItem }
          />
        : null }

        { this.state.visible ?
          <div className="aic-body">
            <div className="aic-body-content">
              { this.props.customComponentsToRender ? this.props.customComponentsToRender : null }
              {
                !this.props.items ? this.props.renderItemContent(shortId.generate(), 0, true) :
                this.props.items && this.props.items.size === 0 ?
                  this.props.renderItemContent(shortId.generate(), 0, true) :
                  this.props.items.map((item, index) =>
                    this.renderItem(item, index)
                  ).toArray()
              }
            </div>
          </div>
        : null }

        { !this.props.readOnly ?
          <PTVAddItemFooter
            messages = { this.props.messages }
            multiple = { this.props.multiple && this.props.items != null }
            countOfItems = { this.props.items != null ? this.props.items.size : 1 }
            onAddButtonClick = { this.props.onAddButtonClick }
            onShowAddItem = { this.props.onShowAddItem }
            addButtonItem = { this.props.addButtonItem }
            visibility = { this.toggleVisibility }
            visible = { this.state.visible }
          />
        : null }

      </div>
    );

  };

}

PTVAddItem.propTypes = {
  componentClass: PropTypes.string,
  items: PropTypes.any,
  addButtonItem: PropTypes.any,
  messages: PropTypes.object,
  onAddButtonClick: PropTypes.func,
  onRemoveItemClick: PropTypes.func,
  renderItemContent: PropTypes.func,
  onShowAddItem: PropTypes.func,
  onHideAddItem: PropTypes.func,
  readOnly: PropTypes.bool,
  collapsible: PropTypes.bool,
  showList: PropTypes.bool,
  clearAll: PropTypes.func,
  multiple: PropTypes.bool,
  includeReset: PropTypes.bool
};

PTVAddItem.defaultProps = {
  multiple: true,
  collapsible: true,
  showList: false,
  includeReset: true
}

//export default injectIntl(PTVAddItem);
export default PTVAddItem;
