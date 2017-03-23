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
import React,  {Component, PropTypes} from 'react';
import ImmutablePropTypes from 'react-immutable-proptypes';
import { connect } from 'react-redux';

import PTVCheckBox from '../PTVCheckBox';
import PTVLabel from '../PTVLabel';
import PTVButton from '../PTVButton';
import Immutable from 'immutable';
import styles from './styles.scss';
import cx from 'classnames';

class PTVCheckBoxTwoLevelList extends React.Component {
//const PTVCheckBoxTwoLevelList = ({isSelectedSelector, isDisabledSelector, isSelected, id, data, readOnly, componentClass, subGroupTitle, onClick, language }) => {
    
    constructor(props) {
       super(props);
       this.state = { 
            showLink: this.props.link && this.props.isSelected && this.props.hasChildren
        };
    }

    onMainCheckBoxClick = (event) => {
      this.props.onClick(this.props.id, event.target.checked);
      this.setState({showLink:event.target.checked && this.props.hasChildren});
      // TODO: faster solution should be - send whole list of children, middleware is not prepared yet or send just checked children
      if (!event.target.checked && this.props.hasChildren) {
          this.props.checkBoxChildren.map((id) => { 
            this.props.onClick(id, event.target.checked);
        });
      }
    }
    
    onSubCheckBoxClick = (event) => {
      this.props.onClick(event.target.id, event.target.checked);
    }

    onLinkClick = () => {
      this.setState({showLink:false});
    }

    renderCheckBox = item => {
      const {data, isSelectedSelector, isDisabledSelector, readOnly, language} = this.props;
      
      const checkBox = data.get(item);      
      return (    
        <div key={ item + '_subCheckBox'}>
          <PTVCheckBox
            key={ checkBox.get('id') + '_subCheckBox' }
            id= { checkBox.get('id') }
            isSelectedSelector= { isSelectedSelector }
            isDisabledSelector= { isDisabledSelector }
            onClick={ this.onSubCheckBoxClick } 
            readOnly= { readOnly }
            language= { language}
            className="small">
              { checkBox.get('name') }
          </PTVCheckBox>          
        </div>
        );
    }    

    render() {
      const { isSelectedSelector, isDisabledSelector, isSelected, id, readOnly, componentClass, subGroupTitle, language } = this.props;
      return (
            !readOnly /*|| this.props.mainBox.isSelected*/ ?
              <div className={cx(componentClass, "col-md-4", {'has-children': this.props.hasChildren} )}>
                <PTVCheckBox 
                    key={ id + '_checkBox'}
                    id={ id }
                    isSelectedSelector= { isSelectedSelector }
                    isDisabledSelector= { isDisabledSelector }
                    onClick={ this.onMainCheckBoxClick }
                    readOnly={ readOnly }
                    language= { language} >
                <div>
                  { this.props.checkBox.get('name') }
                  { this.state.showLink ? <PTVButton  type={'link'} onClick={this.onLinkClick}> {this.props.link} </PTVButton> : null }
                </div>                              
                </PTVCheckBox>
                  
                  { this.props.hasChildren && isSelected && !this.state.showLink ?
                    <div className='sub-collection'>
                      {!readOnly ? <PTVLabel>{subGroupTitle}</PTVLabel> : null}
                        {this.props.checkBoxChildren.map(this.renderCheckBox)}
                    </div>
                  : null }
              </div>
            //: null
            :
              <div className={cx(componentClass, "col-xs-12")}>
                { isSelected ?
                  <ul>
                    <li>
                      <PTVLabel>{this.props.checkBox.get('name')}</PTVLabel>
                      { this.props.hasChildren ? 
                        <div className="sub-collection">
                          {this.props.checkBoxChildren.map(this.renderCheckBox)}
                        </div> 
                      : null }
                    </li>
                  </ul>
                : null }
              </div>
        );
    }
}

PTVCheckBoxTwoLevelList.propTypes = {
  id: PropTypes.string.isRequired,
  data: ImmutablePropTypes.map.isRequired,
  readOnly: PropTypes.bool.isRequired,
  componentClass: PropTypes.string,
};

function mapStateToProps(state, ownProps) {
    
  const isSelected = ownProps.isSelectedSelector(state, ownProps);
  const checkBox = ownProps.data.get(ownProps.id);
    const checkBoxChildren = ownProps.data.get(ownProps.id).get('children');
    const hasChildren = checkBoxChildren && checkBoxChildren.size > 0;
  return {
    isSelected,
    checkBox,
    checkBoxChildren,
    hasChildren
  }
}

export default connect(mapStateToProps)(PTVCheckBoxTwoLevelList);
