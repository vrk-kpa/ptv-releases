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
using Microsoft.Extensions.Logging;

namespace PTV.IdentityUserManager
{
    /// <summary>
    /// Contains authentication server logging events.
    /// </summary>
    public static class AuthenticationServerLogEvents
    {
        /// <summary>
        /// Create new user event.
        /// </summary>
        public static EventId CreateNewUser => new EventId(4000, "CreateNewUser");

        /// <summary>
        /// Change password event.
        /// </summary>
        public static EventId ChangePassword => new EventId(4001, "ChangePassword");

        /// <summary>
        /// Change password for user event.
        /// </summary>
        public static EventId ChangePasswordForUser => new EventId(4002, "ChangePasswordForUser");

        /// <summary>
        /// Change or set user role event.
        /// </summary>
        public static EventId ChangeUserRole => new EventId(4003, "ChangeUserRole");

        /// <summary>
        /// Create new client event.
        /// </summary>
        public static EventId CreateNewClient => new EventId(4004, "CreateNewClient");

        /// <summary>
        /// Create new scope event.
        /// </summary>
        public static EventId CreateNewScope => new EventId(4005, "CreateNewScope");

        /// <summary>
        /// Change user info event.
        /// </summary>
        public static EventId ChangeUserInfo => new EventId(4006, "ChangeUserInfo");

        /// <summary>
        /// Set access rights for user event.
        /// </summary>
        public static EventId SetAccessRightsForUser => new EventId(4007, "SetAccessRightsForUser");

        /// <summary>
        /// Request is authenticated but the user can not be found.
        /// </summary>
        public static EventId AuthenticatedUserMissingError => new EventId(4010, "AuthenticatedUserMissingError");

        /// <summary>
        /// STS general event.
        /// </summary>
        public static EventId General => new EventId(4100, "STSGeneral");
    }
}
