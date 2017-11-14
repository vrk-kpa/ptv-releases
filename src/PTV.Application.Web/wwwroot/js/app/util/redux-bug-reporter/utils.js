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
import isClientRender from './is-client-render'
export let errorData = {
  errors: [],
  addError: function (error) {
    this.errors.push(error)
  },
  clearErrors: function () {
    this.errors = []
  },
  getErrors: function () {
    return this.errors
  }
}

export const listenToErrors = function () {
  if (isClientRender()) {
    listenToConsoleError()
    listenToOnError()
  }
}
const listenToConsoleError = function () {
  let origError = window.console.error
  if (!origError.bugReporterOverrideComplete) {
    window.console.error = function () {
      var metadata
      var args = Array.prototype.slice.apply(arguments)
      if (args && args[0] && args[0].stack) {
        metadata = {
          errorMsg: args[0].name + ': ' + args[0].message,
          stackTrace: args[0].stack
        }
      } else {
        metadata = {
          errorMsg: args && args[0]
        }
      }
      errorData.addError(metadata)
      origError.apply(this, args)
    }
    window.console.error.bugReporterOverrideComplete = true
  }
}

const listenToOnError = function () {
  var origWindowError = window.onerror
  if (!origWindowError || !origWindowError.bugReporterOverrideComplete) {
    window.onerror = function (errorMsg, url, lineNumber, columnNumber, errorObj) {
      var metadata = {
        errorMsg,
        stackTrace: errorObj && errorObj.stack
      }
      errorData.addError(metadata)
      origWindowError && origWindowError.apply(window, arguments)
    }
    window.onerror.bugReporterOverrideComplete = true
  }
}
