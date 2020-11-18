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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using PTV.Framework.Enums;
using PTV.Framework.Paha;
using Xunit;

namespace PTV.Framework.Tests.Paha
{
    public class PahaToken2Test
    {
        private const string emptyToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJsb2dpblN0eWxlIjoid2VhayJ9.kVCBQ-h5x3jl6rtHSzLQmB0mnj0d1ze7B0SEuboTe5Y";

        private const string tokenNoOrgId =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOm51bGwsImFjdGl2ZU9yZ2FuaXphdGlvbk5hbWVGaSI6Ikdyb2tlIE9yZ2FuaXphdGlvbiBTdW9taSIsImFjdGl2ZU9yZ2FuaXphdGlvbk5hbWVTdiI6Ikdyb2tlIE9yZ2FuaXphdGlvbiBSdW90c2kiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRW4iOiJHcm9rZSBPcmdhbml6YXRpb24gTG9udG9vIiwiZmlyc3ROYW1lIjoiTW9vbWluIiwibGFzdE5hbWUiOiJNeWJsZSIsImVtYWlsIjoibW9vbWluQHB0di5maSIsImxvZ2luU3R5bGUiOiJzdHJvbmciLCJzYW1sSXNzdWVyIjoiVlJLIiwiZXhwaXJlc0luIjoxNTg2MjM0MzgyNTIzLCJhdmFpbFNlcnZpY2VzIjpbInB0diIsInNlbWEiXSwicm9sZXMiOnsicHR2IjpbInVzZXIiXSwic2VtYSI6WyJhZG1pbiJdfX0.6gKwf8WowakkBpPlFqoNgN6bHVFm6QRUJzp9CxwxfUI";

        private const string tokenNoOrgName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOm51bGwsImFjdGl2ZU9yZ2FuaXphdGlvbk5hbWVTdiI6Ikdyb2tlIE9yZ2FuaXphdGlvbiBSdW90c2kiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRW4iOiJHcm9rZSBPcmdhbml6YXRpb24gTG9udG9vIiwiZmlyc3ROYW1lIjoiTW9vbWluIiwibGFzdE5hbWUiOiJNeWJsZSIsImVtYWlsIjoibW9vbWluQHB0di5maSIsImxvZ2luU3R5bGUiOiJzdHJvbmciLCJzYW1sSXNzdWVyIjoiVlJLIiwiZXhwaXJlc0luIjoxNTg2MjM0MzgyNTIzLCJhdmFpbFNlcnZpY2VzIjpbInB0diIsInNlbWEiXSwicm9sZXMiOnsicHR2IjpbInVzZXIiXSwic2VtYSI6WyJhZG1pbiJdfX0.WYYgmVCT8u2wHnP-dP5fBcUxWq6DLqw6WATMABX3m2g";

        private const string tokenNoEmail =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJHcm9rZSBPcmdhbml6YXRpb24gU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJHcm9rZSBPcmdhbml6YXRpb24gUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiR3Jva2UgT3JnYW5pemF0aW9uIExvbnRvbyIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJlbWFpbCI6bnVsbCwibG9naW5TdHlsZSI6InN0cm9uZyIsInNhbWxJc3N1ZXIiOiJWUksiLCJleHBpcmVzSW4iOjE1ODYyMzQzODI1MjMsImF2YWlsU2VydmljZXMiOlsicHR2Iiwic2VtYSJdLCJyb2xlcyI6eyJwdHYiOlsidXNlciJdLCJzZW1hIjpbImFkbWluIl19fQ.pw1HjLHUbE1bDms-8JClvPSKf2uDmyG4byRowhC_f7s";

        private const string tokenExpired =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjAsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJHcm9rZSBPcmdhbml6YXRpb24gU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJHcm9rZSBPcmdhbml6YXRpb24gUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiR3Jva2UgT3JnYW5pemF0aW9uIExvbnRvbyIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJlbWFpbCI6Im1vb21pbkBwdHYuZmkiLCJsb2dpblN0eWxlIjoic3Ryb25nIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJwdHYiLCJzZW1hIl0sInJvbGVzIjp7InB0diI6WyJ1c2VyIl0sInNlbWEiOlsiYWRtaW4iXX19.uAegSkcaDhxaPhwLWkmjRIRNYs-z9ppJSoTKAkOPCds";

        private const string tokenNoFirstName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJHcm9rZSBPcmdhbml6YXRpb24gU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJHcm9rZSBPcmdhbml6YXRpb24gUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiR3Jva2UgT3JnYW5pemF0aW9uIExvbnRvbyIsImZpcnN0TmFtZSI6IiIsImxhc3ROYW1lIjoiTXlibGUiLCJlbWFpbCI6Im1vb21pbkBwdHYuZmkiLCJsb2dpblN0eWxlIjoic3Ryb25nIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJwdHYiLCJzZW1hIl0sInJvbGVzIjp7InB0diI6WyJ1c2VyIl0sInNlbWEiOlsiYWRtaW4iXX19.AoWnfGBPkgqnscWPA4vKRdAHoU-z1vZ0PhVP8nkVLwY";

        private const string tokenNoLastName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJHcm9rZSBPcmdhbml6YXRpb24gU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJHcm9rZSBPcmdhbml6YXRpb24gUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiR3Jva2UgT3JnYW5pemF0aW9uIExvbnRvbyIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiIiwiZW1haWwiOiJtb29taW5AcHR2LmZpIiwibG9naW5TdHlsZSI6InN0cm9uZyIsInNhbWxJc3N1ZXIiOiJWUksiLCJleHBpcmVzSW4iOjE1ODYyMzQzODI1MjMsImF2YWlsU2VydmljZXMiOlsicHR2Iiwic2VtYSJdLCJyb2xlcyI6eyJwdHYiOlsidXNlciJdLCJzZW1hIjpbImFkbWluIl19fQ.MPH4vsUI4DnTvSQLA6l7yc0wd8gsVDDp8gctvajZgi4";

        private const string tokenNoPtvRole =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJHcm9rZSBPcmdhbml6YXRpb24gU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJHcm9rZSBPcmdhbml6YXRpb24gUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiR3Jva2UgT3JnYW5pemF0aW9uIExvbnRvbyIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJlbWFpbCI6Im1vb21pbkBwdHYuZmkiLCJsb2dpblN0eWxlIjoic3Ryb25nIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJwdHYiLCJzZW1hIl0sInJvbGVzIjp7InNlbWEiOlsiYWRtaW4iXX19.qOnV3C0on2a0eziMWYZcU4ajav-sJE1P0_t5FT9v0EQ";

        private const string tokenShirley =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiTWlrYW4gMy5zIGRldiB0ZXN0aW5nIExvbnRvbyIsImZpcnN0TmFtZSI6IlRlc3RpaGVua2lsw7YiLCJsYXN0TmFtZSI6IlR1bm5pc3RldHR1IiwiZW1haWwiOiJoYXJyaUBjb3J0ZXhwYXJ0bmVycy5maSIsImxvZ2luU3R5bGUiOiJ3ZWFrIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJlaWRlbnRpZmljYXRpb24iLCJzZXJ2aWNlY2F0YWxvZ3VlIl0sInJvbGVzIjp7ImVpZGVudGlmaWNhdGlvbiI6WyJ1c2VyIl0sInNlcnZpY2VjYXRhbG9ndWUiOlsidXNlciJdfX0.N6wcwSmJDu7QDBgLUTyZ5cYzTrTmDFnBM5a1UXBonyQ";

        private const string tokenPete =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiTWlrYW4gMy5zIGRldiB0ZXN0aW5nIExvbnRvbyIsImZpcnN0TmFtZSI6IlRlc3RpaGVua2lsw7YiLCJsYXN0TmFtZSI6IlR1bm5pc3RldHR1IiwiZW1haWwiOiJoYXJyaUBjb3J0ZXhwYXJ0bmVycy5maSIsImxvZ2luU3R5bGUiOiJ3ZWFrIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJlaWRlbnRpZmljYXRpb24iLCJzZXJ2aWNlY2F0YWxvZ3VlIl0sInJvbGVzIjp7ImVpZGVudGlmaWNhdGlvbiI6WyJ1c2VyIl0sInNlcnZpY2VjYXRhbG9ndWUiOlsiYWRtaW4iXX19.QizgD-VGUo7rikNxLntPVzmJt4EmzDO2Q1GlLvhWA4A";

        private const string tokenEeva =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiTWlrYW4gMy5zIGRldiB0ZXN0aW5nIExvbnRvbyIsImZpcnN0TmFtZSI6IlRlc3RpaGVua2lsw7YiLCJsYXN0TmFtZSI6IlR1bm5pc3RldHR1IiwiZW1haWwiOiJoYXJyaUBjb3J0ZXhwYXJ0bmVycy5maSIsImxvZ2luU3R5bGUiOiJ3ZWFrIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJlaWRlbnRpZmljYXRpb24iLCJzZXJ2aWNlY2F0YWxvZ3VlIl0sInJvbGVzIjp7ImVpZGVudGlmaWNhdGlvbiI6WyJ1c2VyIl0sInNlcnZpY2VjYXRhbG9ndWUiOlsiYWRtaW4iXX0sInZya1NlcnZpY2VzIjp7InNlcnZpY2VjYXRhbG9ndWUiOlsidnJrYWRtaW4iXX19.afSlKR8HD055ZU68_-ZrOa0xVy8EO8i9DMRQEiiM2RI";

        private const string tokenApi =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiTWlrYW4gMy5zIGRldiB0ZXN0aW5nIExvbnRvbyIsImZpcnN0TmFtZSI6IlRlc3RpaGVua2lsw7YiLCJsYXN0TmFtZSI6IlR1bm5pc3RldHR1IiwiZW1haWwiOiJoYXJyaUBjb3J0ZXhwYXJ0bmVycy5maSIsImxvZ2luU3R5bGUiOiJ3ZWFrIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJlaWRlbnRpZmljYXRpb24iLCJzZXJ2aWNlY2F0YWxvZ3VlIl0sInJvbGVzIjp7ImVpZGVudGlmaWNhdGlvbiI6WyJ1c2VyIl0sInNlcnZpY2VjYXRhbG9ndWUiOlsiYXBpIl19fQ.tcf7-Th198h81IfjEPYjaWaykAMctPuw_n0ZjWjsqxs";

        private const string tokenAsti =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI5OThkY2ZkYi01MzQzLTQzOTktOGVhYS0zN2M3M2I2ZDY0ODIiLCJpYXQiOjE1ODYyMDU1ODIsImlzcyI6IlBBSEEiLCJleHAiOjE1ODYyMzQzODIsImlkIjoiZDNkYmJkMDYtOWJjZC00MWY5LTk2ZjYtN2ZjNzE4OTYzMmZjIiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJhMTM0NWMyMS1mMDNiLTQ0NWUtYTVhMS0zNjI1ZTRiZmI2ZWIiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lRmkiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgU3VvbWkiLCJhY3RpdmVPcmdhbml6YXRpb25OYW1lU3YiOiJNaWthbiAzLnMgZGV2IHRlc3RpbmcgUnVvdHNpIiwiYWN0aXZlT3JnYW5pemF0aW9uTmFtZUVuIjoiTWlrYW4gMy5zIGRldiB0ZXN0aW5nIExvbnRvbyIsImZpcnN0TmFtZSI6IlRlc3RpaGVua2lsw7YiLCJsYXN0TmFtZSI6IlR1bm5pc3RldHR1IiwiZW1haWwiOiJoYXJyaUBjb3J0ZXhwYXJ0bmVycy5maSIsImxvZ2luU3R5bGUiOiJ3ZWFrIiwic2FtbElzc3VlciI6IlZSSyIsImV4cGlyZXNJbiI6MTU4NjIzNDM4MjUyMywiYXZhaWxTZXJ2aWNlcyI6WyJlaWRlbnRpZmljYXRpb24iLCJzZXJ2aWNlY2F0YWxvZ3VlIl0sInJvbGVzIjp7ImVpZGVudGlmaWNhdGlvbiI6WyJ1c2VyIl0sInNlcnZpY2VjYXRhbG9ndWUiOlsiYXBpIl19LCJ2cmtTZXJ2aWNlcyI6eyJzZXJ2aWNlY2F0YWxvZ3VlIjpbImFzdGkiXX19.mTijG182Sk8uK2mpDvzHNtxic8dquHDvTGoFnZVRii8";

        private static class ErrorMessages
        {
            public const string NoOrganizationId = "PahaToken: User's active organization is not set!";

            public const string NoOrganizationNamePrefix =
                "PahaToken: Active user organization has invalid or missing Name!";

            public const string NoEmail = "PahaToken: User has no email!";

            public const string AfterExpiration = "PahaToken: Expiration of token is invalid!";

            public const string NoFirstName = "PahaToken: User's FirstName is not set!";
            
            public const string NoLastName = "PahaToken: User's LastName is not set!";
            
            public const string NoRolePrefix = "PahaToken: Active user's organization has invalid or missing role!";
        }
        
        [Fact]
        public void ExtractEmptyToken()
        {
            var encodedToken = new JwtSecurityToken(emptyToken);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.NotEmpty(token.InternalErrorMessages);
            Assert.NotEmpty(token.InternalWarningMessages);
        }

        [Fact]
        public void ExtractTokenWithoutOrganizationId()
        {
            var encodedToken = new JwtSecurityToken(tokenNoOrgId);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoOrganizationId, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutOrganizationName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoOrgName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.StartsWith(ErrorMessages.NoOrganizationNamePrefix, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutEmail()
        {
            var encodedToken = new JwtSecurityToken(tokenNoEmail);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoEmail, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractExpiredToken()
        {
            var encodedToken = new JwtSecurityToken(tokenExpired);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.AfterExpiration, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutFirstName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoFirstName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoFirstName, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutLastName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoLastName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoLastName, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutPtvRole()
        {
            var encodedToken = new JwtSecurityToken(tokenNoPtvRole);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken2>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.StartsWith(ErrorMessages.NoRolePrefix, token.InternalErrorMessages.First());
        }

        [Theory]
        [InlineData(tokenApi, UserAccessRightsGroupEnum.API_USER)]
        [InlineData(tokenAsti, UserAccessRightsGroupEnum.API_ASTI_USER)]
        [InlineData(tokenEeva, UserAccessRightsGroupEnum.PTV_ADMINISTRATOR)]
        [InlineData(tokenPete, UserAccessRightsGroupEnum.PTV_MAIN_USER)]
        [InlineData(tokenShirley, UserAccessRightsGroupEnum.PTV_USER)]
        public void ExtractTokenCheckRole(string bearer, UserAccessRightsGroupEnum expectedRole)
        {
            var encodedToken = new JwtSecurityToken(bearer);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            
            Assert.IsType<PahaToken2>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(expectedRole, token.PtvRole);
        }
    }
}
