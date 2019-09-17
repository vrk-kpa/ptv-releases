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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Localization.Services.Handlers;

namespace PTV.LocalizationTool.App.Handlers
{
    [RegisterService(typeof(CommandHandlerManager), RegisterType.Singleton)]
    internal class CommandHandlerManager
    {
        private Dictionary<string, Type> handlers;
        private IResolveManager resolveManager;

        public CommandHandlerManager(IEnumerable<ITranslationDataHandler> handlers, IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
            this.handlers = handlers.ToDictionary(x => x.Key, x => x.GetType());
        }

        public ITranslationDataHandler Get(string key)
        {
            var type = handlers.TryGet(key);
            return type != null ? resolveManager.Resolve(type) as ITranslationDataHandler : null;
        }
        
        public T Get<T>(Action<T> initAction = null) where T : class, ITranslationDataHandler
        {           
            var handler = resolveManager.Resolve<T>();
            if (handler != null)
            {
                initAction?.Invoke(handler);
            }

            return handler;
        }
    }
}