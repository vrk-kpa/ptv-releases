/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
// import Helmet from 'react-helmet';
import React, { Component } from 'react'
import PropTypes from 'prop-types'

class LanguageButton extends Component {
  static propTypes = {
    language: PropTypes.string.isRequired,
    cultureInfo: PropTypes.string,
    selectedLanguage: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired,
    children: PropTypes.any
  };

  onClick = () => {
    // this.props.onClick(this.props.language);
    this.props.onClick(this.props.cultureInfo, this.props.language)
  }

  getSelectedLanguageClass = (selectedLanguage, languageButton) => {
    return (
      selectedLanguage === languageButton ? 'selected' : ''
    )
  };

  render () {
    const { selectedLanguage, language } = this.props
    return (
      <li className={this.getSelectedLanguageClass(selectedLanguage, language)}>
        <span>
          <a
            lang={language}
            href='javascript:;'
            onClick={this.onClick}
            children={this.props.children}
          />
        </span>
      </li>
    )
  }
}

export default LanguageButton
