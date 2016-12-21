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
import appReducer from './reducer';
import createFetch from './createFetch';
import {applyMiddleware, compose, createStore} from 'redux';
import createLogger from 'redux-logger';
import shortid from 'shortid';
import promiseMiddleware from 'redux-promise-middleware';
import apiMiddleware from '../Middleware/Api';
import { loadState, saveState } from './AppHelpers';
import throttle from 'lodash/throttle';
import { Record, Map, List } from 'immutable';

const injectMiddleware = deps => ({ dispatch, getState }) => next => action =>
  next(typeof action === 'function'
    ? action({ ...deps, dispatch, getState })
    : action
  );

const createUniversalFetch = initialState => {
  const serverUrl =
    process.env.SERVER_URL || // Must be set for React Native production app.
    (process.env.IS_BROWSER
      ? '' // Browser can handle relative urls.
      : 'http://localhost:8000' // Failback for dev.
    );
  return createFetch(serverUrl);
};

const enableLogger =
  process.env.NODE_ENV !== 'production' &&
  process.env.IS_BROWSER;

const enableDevToolsExtension = (typeof window.devToolsExtension === 'function');
  //process.env.NODE_ENV !== 'production' &&
  //process.env.IS_BROWSER &&
  //window.devToolsExtension;

export default function configureStore(options) {
	const {
	    createEngine,
	    initialState= localStorage.getItem('forceLoadState') === 'true' ? loadState() : undefined,
	    platformDeps = {},
	    platformMiddleware = []
  	} = options;

  	// We should use it later, we should also think, how and where we will send the state log
  	//const engineKey = `redux-storage:${initialState.config.appName}`;
	//const engine = createEngine && createEngine(engineKey); // No server engine.
	//const firebase = new Firebase(initialState.config.firebaseUrl);

    let reducer = appReducer;

    const middleware = [
    	...platformMiddleware,
    	injectMiddleware({
      		...platformDeps,
      	//	//engine,
      	//	fetch: createUniversalFetch(initialState),
      	//	//firebase,
      		getUid: () => shortid.generate(),
      		now: () => Date.now(),
      	//	//validate
    	}),
    	apiMiddleware,
    	promiseMiddleware({
      		promiseTypeSuffixes: ['START', 'SUCCESS', 'ERROR']
    	})
  	];

  	// Logger must be the last middleware in chain.
  	if (enableLogger) {
    	const logger = createLogger({
      	collapsed: true,
      	// Convert immutable to JSON.
      	stateTransformer: state => JSON.parse(JSON.stringify(state))
    	});

    	middleware.push(logger);
  	}

  	const createStoreWithMiddleware = enableDevToolsExtension
    	? compose(applyMiddleware(...middleware), window.devToolsExtension())
    	: applyMiddleware(...middleware);

  	const store = createStoreWithMiddleware(createStore)(reducer, initialState);

		store.subscribe(throttle(() => {
				const state = store.getState();
				saveState(state.setIn(['common','apiCalls'], Map()));
		}, 2000));

    return store;
}

