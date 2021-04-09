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

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace PTV.SoapServices.Interfaces.Lingsoft
{
    /// <summary>
    /// Interface for implementation of entity LingsoftClientManager
    /// </summary>
    public interface ILingsoftClientManager : IDisposable
    {
        /// <summary>
        /// New order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        IOrderResponse NewOrder(IOrder order);

        /// <summary>
        /// Update order
        /// </summary>
        /// <param name="llsoWorkID"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        IUpdateOrderResponse UpdateOrder(string llsoWorkID, IOrder order);

        /// <summary>
        /// Order status
        /// </summary>
        /// <param name="llsoWorkID"></param>
        /// <returns></returns>
        IOrderStatusResponse OrderStatus(string llsoWorkID);

        /// <summary>
        /// Cancel order
        /// </summary>
        /// <param name="llsoWorkID"></param>
        /// <returns></returns>
        ICancelOrderResponse CancelOrder(string llsoWorkID);

        /// <summary>
        /// Client credentials
        /// </summary>
        ClientCredentials ClientCredentials { get; }

        /// <summary>
        /// Endpoint
        /// </summary>
        ServiceEndpoint Endpoint { get; }

        /// <summary>
        /// Inner channel
        /// </summary>
        IClientChannel InnerChannel { get; }

        /// <summary>
        /// State
        /// </summary>
        CommunicationState State { get; }
    }
}
