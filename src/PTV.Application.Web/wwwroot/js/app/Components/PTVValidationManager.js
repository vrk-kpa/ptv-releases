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
import {PropTypes, Component} from 'react';
import {OrderedMap} from 'immutable';
import { injectIntl } from 'react-intl'
import Promise from 'bluebird';

class PTVValidationManager {

    componentList = new OrderedMap();

    componentListSize = 0;
    notificationReceived = 0;
    areAllValid = () => {
    	return this.componentList.every((component) => component.state.isValid);
    }

    invalidateManager = () => invalidateManager = false;

    attachComponentToParent = (component) => {
    	this.componentList = this.componentList.set(component.uniqueId, component);
    }

    detachComponentFromParent = (uniqueId) => {
    	this.componentList = this.componentList.delete(uniqueId);
    }

    areAllNotificationReceived = () => {
    	return this.notificationReceived == this.componentListSize;
    }

    isManagerValid = () => {

  	return new Promise((resolve, reject) => {
		  this.validateAll();
  		var that = this;
  		(function wait() {
  			if (that.areAllNotificationReceived()) {
  				return resolve(that.getResults());
  			}
  			setTimeout(wait, 100)
  		})();
  	});
    }

    getResults = () => {
		if (this.areAllValid()) {
			return true;
		}
		else {
			return this.componentList.filter((component) => !component.state.isValid).sortBy(component => component.props.order).map((component) => {
					return {field: component.validatedField, message: component.state.message}
    		})
    	}
	}

    notifyMe = (component) => {
    	this.notificationReceived++;
    }


    validateAll = () => {
    	this.notificationReceived = 0;
    	this.componentListSize = this.componentList.size;
    	this.componentList.map((component, index) => component.validateComponent());
    }
}

export default PTVValidationManager
