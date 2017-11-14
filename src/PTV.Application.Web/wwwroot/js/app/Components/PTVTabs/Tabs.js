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
import classnames from 'classnames';
import InjectTab from './Tab.js';
import InjectTabContent from './TabContent.js';
import styles from './styles.scss';

const factory = (Tab, TabContent) => {
  class Tabs extends Component {
    static propTypes = {
      children: PropTypes.node,
      className: PropTypes.string,
      disableAnimatedBottomBorder: PropTypes.bool,
      fixed: PropTypes.bool,
      index: PropTypes.number,
      onChange: PropTypes.func,
      framed: PropTypes.bool
    };

    static defaultProps = {
      index: 0,
      fixed: false
    };

    state = {
      pointer: {}
    };

    componentDidMount() {
      !this.props.disableAnimatedBottomBorder && this.updatePointer(this.props.index);
      window.addEventListener('resize', this.handleResize);
      this.handleResize();
    }

    componentWillReceiveProps(nextProps) {
      !this.props.disableAnimatedBottomBorder && this.updatePointer(nextProps.index);
    }

    componentWillUnmount() {
      window.removeEventListener('resize', this.handleResize);
      clearTimeout(this.resizeTimeout);
      clearTimeout(this.pointerTimeout);
    }

    handleHeaderClick = (event) => {
      const idx = parseInt(event.currentTarget.id);
      if (this.props.onChange) this.props.onChange(idx);
    };

    handleResize = () => {
      if (!this.props.fixed) {
        return;
      }

      if (this.resizeTimeout) {
        clearTimeout(this.resizeTimeout);
      }
      this.resizeTimeout = setTimeout(this.handleResizeEnd, 50);
    };

    handleResizeEnd = () => {
      this.updatePointer(this.props.index);
    };

    parseChildren() {
      const headers = [];
      const contents = [];

      React.Children.forEach(this.props.children, (item) => {
        if (item.type === Tab) {
          headers.push(item);
          if (item.props.children) {
            contents.push(<TabContent children={item.props.children} />);
          }
        } else if (item.type === TabContent) {
          contents.push(item);
        }
      });

      return {headers, contents};
    }

    updatePointer(idx) {
      idx = idx >= this.refs.navigation.children.length ? 0 : idx;

      clearTimeout(this.pointerTimeout);
      this.pointerTimeout = setTimeout(() => {
        const startPoint = this.refs.tabs.getBoundingClientRect().left;
        const label = this.refs.navigation.children[idx].getBoundingClientRect();
        this.setState({
          pointer: {
            top: `${this.refs.navigation.getBoundingClientRect().height}px`,
            left: `${label.left - startPoint}px`,
            width: `${label.width}px`
          }
        });
      }, 20);
    }

    renderHeaders(headers) {
      return headers.map((item, idx) => {
        return React.cloneElement(item, {
          id: idx,
          key: idx,
          active: this.props.index === idx,
          onClick: event => {
            this.handleHeaderClick(event);
            item.props.onClick && item.props.onClick(event);
          }
        });
      });
    }

    renderContents(contents) {
      const activeIdx = contents.findIndex((item, idx) => {
        return this.props.index === idx;
      });

      if (contents && contents[activeIdx]) {
        return React.cloneElement(contents[activeIdx], {
          key: activeIdx,
          active: true,
          tabIndex: activeIdx
        });
      }
    }

    render() {
      const {className, fixed, framed} = this.props;
      const {headers, contents} = this.parseChildren();
      const classes = classnames(
        "ptv-tabs",
        className,
        {
          "fixed": fixed,
          "framed": framed,
        }
      );
      return (
        <div ref="tabs" className={classes}>
          <nav className="tab-navigation" ref="navigation">
            {this.renderHeaders(headers)}
          </nav>
          { !this.props.framed ?
            <span className="pointer" style={this.state.pointer} />
          : null }
          {this.renderContents(contents)}
        </div>
      );
    }
  }

  return Tabs;
};

const Tabs = factory(InjectTab, InjectTabContent);
export {factory as tabsFactory};
export default Tabs;