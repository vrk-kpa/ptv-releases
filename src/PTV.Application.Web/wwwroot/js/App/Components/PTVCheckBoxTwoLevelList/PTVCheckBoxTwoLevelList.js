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
import Immutable from 'immutable';
import styles from './styles.scss';
import cx from 'classnames';

const PTVCheckBoxTwoLevelList = ({isSelectedSelector, isDisabledSelector, isSelected, id, data, readOnly, componentClass, subGroupTitle, onClick, language }) => {
    const onMainCheckBoxClick = (event) => {
      onClick(id, event.target.checked);
      // TODO: faster solution should be - send whole list of children, middleware is not prepared yet or send just checked children
      if (!event.target.checked && hasChildren) {
          checkBoxChildren.map((id) => { 
            onClick(id, event.target.checked);
        });
      }
    }
    
    const onSubCheckBoxClick = (event) => {
      onClick(event.target.id, event.target.checked);
    }

    const renderCheckBox = item => {
      const checkBox = data.get(item);

      return (    
        <div key={ item + '_subCheckBox'}>
          <PTVCheckBox
            key={ checkBox.get('id') + '_subCheckBox' }
            id= { checkBox.get('id') }
            isSelectedSelector= { isSelectedSelector }
            isDisabledSelector= { isDisabledSelector }
            onClick={ onSubCheckBoxClick } 
            readOnly= { readOnly }
            language= { language}
            className="small">
              { checkBox.get('name') }
          </PTVCheckBox>
        </div>
        );
    }

    const checkBox = data.get(id);
    const checkBoxChildren = data.get(id).get('children');
    const hasChildren = checkBoxChildren && checkBoxChildren.size > 0;

    return (
          !readOnly /*|| this.props.mainBox.isSelected*/ ?
            <div className={cx(componentClass, "col-md-4", {'has-children': hasChildren} )}>
              <PTVCheckBox 
                  key={ id + '_checkBox'}
                  id={ id }
                  isSelectedSelector= { isSelectedSelector }
                  isDisabledSelector= { isDisabledSelector }
                  onClick={ onMainCheckBoxClick }
                  readOnly={ readOnly }
                  language= { language} >
              { checkBox.get('name') }
                  </PTVCheckBox>
                  { hasChildren && isSelected ?
                    <div className='sub-collection'>
                      {!readOnly ? <PTVLabel>{subGroupTitle}</PTVLabel> : null}
                        {checkBoxChildren.map(renderCheckBox)}
                    </div>
                  : null }
        	  </div>
          //: null
          :
            <div className={cx(componentClass, "col-xs-12")}>
              { isSelected ?
                <ul>
                  <li>
                    <PTVLabel>{checkBox.get('name')}</PTVLabel>
                    { hasChildren ? 
                      <div className="sub-collection">
                        {checkBoxChildren.map(renderCheckBox)}
                      </div> 
                    : null }
                  </li>
                </ul>
              : null }
            </div>
        );
}

PTVCheckBoxTwoLevelList.propTypes = {
  id: PropTypes.string.isRequired,
  data: ImmutablePropTypes.map.isRequired,
  readOnly: PropTypes.bool.isRequired,
  componentClass: PropTypes.string,
};

function mapStateToProps(state, ownProps) {
  const isSelected = ownProps.isSelectedSelector(state, ownProps);

  return {
    isSelected
  }
}

export default connect(mapStateToProps)(PTVCheckBoxTwoLevelList);
