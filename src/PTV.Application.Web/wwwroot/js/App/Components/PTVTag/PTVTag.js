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
import React,  {Component} from 'react';
import styles from './styles.scss';
import PTVIcon from '../PTVIcon';
import { renderOptions } from '../PTVComponent';
import { injectIntl, intlShape, formattedMessage } from 'react-intl';
import cx from 'classnames';


const PTVTag = (props) => {
    const {onTagRemove, id, name, isDisabled, readOnly, className} = props;

    const onClickTagRemove = () => {
        if (!isDisabled){
            onTagRemove(id);
        }
    }

    const renderOption = () => {
        const option = { id, name };
        if (props.renderOption){
            return props.renderOption(option)
        }

        const data = { ...props, option}
        return renderOptions(data);
    }

    return (
        <li className={cx("tag-wrap", className)} key={ id } >
            <span className={cx("tag-item", isDisabled ? "disabled" : null)}>
                <span className="wrapped">
                    { renderOption() }
                    { !readOnly ?
                        <PTVIcon
                            name="icon-cross"
                            onClick={ onClickTagRemove } />
                    : null}
                </span>
            </span>
        </li>
    )
}

PTVTag.propTypes = {
  intl: intlShape.isRequired,
};

PTVTag.defaultProps = {
    className: ''
}

export default injectIntl(PTVTag);