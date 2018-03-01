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
import React, {PropTypes, Component} from 'react';
import {connect} from 'react-redux';

import { PTVTag } from '../../../../../Components';
import { LocalizedTag } from '../../../../Common/localizedData';

const getTag = (localized) => (props) =>  {
        if (localized){
            return <LocalizedTag {...props} />
        }
        return <PTVTag {...props} />
    }

export const ServiceAndChannelTag =  ({ id, xa, text, isSelected, onTagRemove, readOnly, localized }) => {

    const Tag = getTag(localized);

    return (
            isSelected?
            <Tag
                id= { id }
                name= { text }
                onTagRemove= { onTagRemove }
                className="inline"
                readOnly= { readOnly }
            />:null
       );
}

const mapStateToProps = (state, ownProps) => {
  return {
      text: ownProps.getTextSelector ? ownProps.getTextSelector(state, ownProps) : ownProps.text,
      isSelected: ownProps.getIsSelectedSelector ? ownProps.getIsSelectedSelector(state, ownProps) : ownProps.isSelected
  }
}


export default connect(mapStateToProps)(ServiceAndChannelTag);
