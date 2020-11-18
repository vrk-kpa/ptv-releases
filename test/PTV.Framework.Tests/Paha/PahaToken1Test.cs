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
    public class PahaToken1Test
    {
        private const string emptyToken =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.Et9HFtf9R3GEMA0IICOfFMVXY7kkTX1wr4qCyhIf58U";

        private const string tokenNoOrgId =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6bnVsbCwiYXBpVXNlck9yZ2FuaXphdGlvbiI6bnVsbCwiZmlyc3ROYW1lIjoiTW9vbWluIiwibGFzdE5hbWUiOiJNeWJsZSIsImV4cCI6MTU4NzAzMTM4MywiaXNzIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJhdWQiOiJodHRwczovL3Rlc3QucHR2LmNsb3VkLmR2di5maSIsIm9yZ2FuaXphdGlvbnMiOiJbe1wiSWRcIjpcImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMFwiLFwiTmFtZVwiOlwiR3Jva2UgT3JnYW5pemF0aW9uXCIsXCJSb2xlc1wiOltcIlBUVl9BRE1JTklTVFJBVE9SXCJdfV0iLCJyb2xlcyI6eyJwdHYiOlsidnJrYWRtaW4iXSwic2VtYSI6WyJhZG1pbiJdfX0.vpnQe0H4WxZ5e2bIEFzudMTykk2TApB8uUbWBAbPfg0";

        private const string tokenNoOrgName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIlwiLFwiUm9sZXNcIjpbXCJQVFZfQURNSU5JU1RSQVRPUlwiXX1dIiwicm9sZXMiOnsicHR2IjpbIlBUVl9NQUlOX1VTRVIiXSwic2VtYSI6WyJhZG1pbiJdfX0.Iv_Ob_cD2ZU3pCiGcKPVTihSSWNVDI8l0a8yS8Va2QM";

        private const string tokenNoEmail =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6IiIsImFjdGl2ZU9yZ2FuaXphdGlvbklkIjoiZTgyMjRkZGYtYjAyZi00NGEzLTk1NzEtMGMxYzJhNDAyNzMwIiwiYXBpVXNlck9yZ2FuaXphdGlvbiI6bnVsbCwiZmlyc3ROYW1lIjoiTW9vbWluIiwibGFzdE5hbWUiOiJNeWJsZSIsImV4cCI6MTU4NzAzMTM4MywiaXNzIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJhdWQiOiJodHRwczovL3Rlc3QucHR2LmNsb3VkLmR2di5maSIsIm9yZ2FuaXphdGlvbnMiOiJbe1wiSWRcIjpcImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMFwiLFwiTmFtZVwiOlwiR3Jva2UgT3JnYW5pemF0aW9uXCIsXCJSb2xlc1wiOltcIlBUVl9BRE1JTklTVFJBVE9SXCJdfV0iLCJyb2xlcyI6eyJwdHYiOlsiUFRWX01BSU5fVVNFUiJdLCJzZW1hIjpbImFkbWluIl19fQ.7GjPqjEfuWgAT4I5An-pOhjfRGmQ5ENLZOAiionThE8";

        private const string tokenExpired =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjAsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJQVFZfQURNSU5JU1RSQVRPUlwiXX1dIiwicm9sZXMiOnsicHR2IjpbIlBUVl9NQUlOX1VTRVIiXSwic2VtYSI6WyJhZG1pbiJdfX0.psWouTPtpEAE8tpsN7m9t5D58Pv2QidqehyRCJG1Rv8";

        private const string tokenNoFirstName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6IiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJQVFZfQURNSU5JU1RSQVRPUlwiXX1dIiwicm9sZXMiOnsicHR2IjpbIlBUVl9NQUlOX1VTRVIiXSwic2VtYSI6WyJhZG1pbiJdfX0.qTHo_tiM-nZK8kUGVGjaWZSRmVi9qyIpJqc6DjMVotQ";

        private const string tokenNoLastName =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiIiwiZXhwIjoxNTg3MDMxMzgzLCJpc3MiOiJodHRwczovL3Rlc3QucHR2LmNsb3VkLmR2di5maSIsImF1ZCI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwib3JnYW5pemF0aW9ucyI6Ilt7XCJJZFwiOlwiZTgyMjRkZGYtYjAyZi00NGEzLTk1NzEtMGMxYzJhNDAyNzMwXCIsXCJOYW1lXCI6XCJHcm9rZSBPcmdhbml6YXRpb25cIixcIlJvbGVzXCI6W1wiUFRWX0FETUlOSVNUUkFUT1JcIl19XSIsInJvbGVzIjp7InB0diI6WyJQVFZfTUFJTl9VU0VSIl0sInNlbWEiOlsiYWRtaW4iXX19.rCtlyyjriWwtZ8d11bCwDNBH__g41bjqgob5sDNcymc";

        private const string tokenNoPtvRole =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXX1dIiwicm9sZXMiOnsic2VtYSI6WyJhZG1pbiJdfX0.OYLmWL4TDHUaR31FN7OrRi4oZ1PCNLSP5vXsdweKadc";

        private const string tokenShirley =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJQVFZfVVNFUlwiXX1dIiwicm9sZXMiOnsicHR2IjpbInVzZXIiXSwic2VtYSI6WyJhZG1pbiJdfX0.TtHs0kFxWQYjtDOIMMl4OdvdtBMCTf41J7AiVMfVJ_0";

        private const string tokenPete =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJQVFZfTUFJTl9VU0VSXCJdfV0iLCJyb2xlcyI6eyJwdHYiOlsiYWRtaW4iXSwic2VtYSI6WyJhZG1pbiJdfX0.RJtZYRtSZJZjWvhzGbKhgll_5DZ6FKyUls8hHYDXos8";

        private const string tokenEeva =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1ODk5NjEwMTAsInNjb3BlIjoibG9naW4iLCJjbGllbnRfaWQiOiIxNzdmZWUwOC0wMjg2LTQzYzItYjJhMC0yMGM3N2RjYmY5NDAiLCJ1c2VybmFtZSI6Imx1Ym9taXIuc29rb2xvdnNreUB0aWV0by5jb20iLCJmaXJzdE5hbWUiOiJSb2JlcnQiLCJsYXN0TmFtZSI6IlRlc3RncmFuIiwiZW1haWwiOiJsdWJvbWlyLnNva29sb3Zza3lAdGlldG8uY29tIiwib3JnYW5pemF0aW9ucyI6W3siaWQiOiI3NzMxOTJlNC0xMjFmLTQwYWQtOGYwZS0yOTlkMTM0OTRiZjciLCJuYW1lIjoiUFRWIEtlbGEgVGVzdGlvcmdhbmlzYWF0aW8gIiwic3ViT3JnIjpmYWxzZSwicm9sZXMiOlsiUFRWX01BSU5fVVNFUiJdfSx7ImlkIjoiYzQ5Yjc5NzctOWFkYi00NGY3LWIwNTgtOGRkMDIxM2RhYTUxIiwibmFtZSI6IlBhbHZlbHV0aWV0b3ZhcmFudG8gKFRlc3RhdXMpICIsInN1Yk9yZyI6ZmFsc2UsInJvbGVzIjpbIlBUVl9NQUlOX1VTRVIiXX0seyJpZCI6ImQ0NWM1OGQ0LTRhOGMtNDY3MC04YzU5LTBlNWY5ZTJkZWQ5OSIsIm5hbWUiOiJQVFYgT3VsdSBUZXN0aW9yZ2FuaXNhYXRpbyAiLCJzdWJPcmciOmZhbHNlLCJyb2xlcyI6WyJQVFZfTUFJTl9VU0VSIl19XSwicm9sZXMiOnsic2VtYSI6WyJ1c2VyIl0sInB0diI6WyJhZG1pbiJdLCJ2cmtzZXJ2aWNlY2F0YWxvZ3VlIjpbIlBUVl9BRE1JTklTVFJBVE9SIl19LCJpZCI6IjE3N2ZlZTA4LTAyODYtNDNjMi1iMmEwLTIwYzc3ZGNiZjk0MCIsImFjdGl2ZU9yZ2FuaXphdGlvbklkIjoiNzczMTkyZTQtMTIxZi00MGFkLThmMGUtMjk5ZDEzNDk0YmY3IiwiaWF0IjoxNTg5ODc0NjA5fQ.bu-CkJzVN3S4e0p1Ljzbtr45m_ZvxSj2zHHpuA6E4GQ";

        private const string tokenApi =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6bnVsbCwiYXBpVXNlck9yZ2FuaXphdGlvbiI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJBUElfVVNFUlwiXX1dIiwicm9sZXMiOnsicHR2IjpbImFwaSJdLCJzZW1hIjpbImFkbWluIl19fQ.o6rke39e92-F6b1-0ZoKAaEwPoRJ8v5kkzReP1cIkPg";

        private const string tokenAsti =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6bnVsbCwiYXBpVXNlck9yZ2FuaXphdGlvbiI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJBUElfQVNUSV9VU0VSXCJdfV0iLCJyb2xlcyI6eyJwdHYiOlsiYXN0aSJdLCJzZW1hIjpbImFkbWluIl19fQ.sdsFu6JbEDiqg3_qQE7KAQYV9y_u5zoKn-8ATtVhCHs";

        private const string tokenRoleInOrgs = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VybmFtZSI6Im1vb21pbkBwdHYuZmkiLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImU4MjI0ZGRmLWIwMmYtNDRhMy05NTcxLTBjMWMyYTQwMjczMCIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImZpcnN0TmFtZSI6Ik1vb21pbiIsImxhc3ROYW1lIjoiTXlibGUiLCJleHAiOjE1ODcwMzEzODMsImlzcyI6Imh0dHBzOi8vdGVzdC5wdHYuY2xvdWQuZHZ2LmZpIiwiYXVkIjoiaHR0cHM6Ly90ZXN0LnB0di5jbG91ZC5kdnYuZmkiLCJvcmdhbml6YXRpb25zIjoiW3tcIklkXCI6XCJlODIyNGRkZi1iMDJmLTQ0YTMtOTU3MS0wYzFjMmE0MDI3MzBcIixcIk5hbWVcIjpcIkdyb2tlIE9yZ2FuaXphdGlvblwiLFwiUm9sZXNcIjpbXCJQVFZfTUFJTl9VU0VSXCJdfV0iLCJyb2xlcyI6eyJzZW1hIjpbImFkbWluIl19fQ.Ml_p8rqNDAEHWcpfMeok1oYcXhDiAlmjvaQUOCQo33s";

        private const string tokenProperListOrgs =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1ODc0NjkwODgsInNjb3BlIjoibG9naW4iLCJjbGllbnRfaWQiOiJiMTI0M2RiZS04YzI3LTRlNDgtYmQyMy1jZGVjOWI2NmUwZDciLCJ1c2VybmFtZSI6InB0di50ZXN0aWtheXR0YWphQHBhaGEudGVzdCIsImZpcnN0TmFtZSI6IlRlaWphIiwibGFzdE5hbWUiOiJUZXN0Z3JhbiIsImVtYWlsIjoicHR2LnRlc3Rpa2F5dHRhamFAcGFoYS50ZXN0Iiwib3JnYW5pemF0aW9ucyI6W3siaWQiOiJkNDVjNThkNC00YThjLTQ2NzAtOGM1OS0wZTVmOWUyZGVkOTkiLCJuYW1lIjoiUFRWIE91bHUgVGVzdGlvcmdhbmlzYWF0aW8gIiwic3ViT3JnIjpmYWxzZX0seyJpZCI6Ijc3MzE5MmU0LTEyMWYtNDBhZC04ZjBlLTI5OWQxMzQ5NGJmNyIsIm5hbWUiOiJQVFYgS2VsYSBUZXN0aW9yZ2FuaXNhYXRpbyAiLCJzdWJPcmciOmZhbHNlfSx7ImlkIjoiYzQ5Yjc5NzctOWFkYi00NGY3LWIwNTgtOGRkMDIxM2RhYTUxIiwibmFtZSI6IlBhbHZlbHV0aWV0b3ZhcmFudG8gKFRlc3RhdXMpICIsInN1Yk9yZyI6ZmFsc2UsInJvbGVzIjpbIlBUVl9VU0VSIl19XSwicm9sZXMiOnsic2VtYSI6WyJhZG1pbiJdLCJwdHYiOlsidXNlciJdfSwiaWQiOiJiMTI0M2RiZS04YzI3LTRlNDgtYmQyMy1jZGVjOWI2NmUwZDciLCJhY3RpdmVPcmdhbml6YXRpb25JZCI6ImM0OWI3OTc3LTlhZGItNDRmNy1iMDU4LThkZDAyMTNkYWE1MSIsImFwaVVzZXJPcmdhbml6YXRpb24iOm51bGwsImlhdCI6MTU4NzM4MjY4N30.aHjK1Sv-rdHvIpro2FXhjwmzxO38lfd3zr8ZU3q4IfFaDXc5nYkkn6oC5ww0EITv9pfp272XLmcyAWrISPHxOhucdnR3LrP68K0UmjlraxbOzuI6HA3FWZRj0b1pVj7MOoroN09sLo2iRS9HllDvlxTgzJt62TJ_zfdmiLHB8Uo2roSYXFP0145KOhmRKlV7eFZZCESiPDJT4FNtOal3SFE1ZzyoBhkj977ZqTIjrl_KDuSOUkPzv993a-lkIZtlR-vDnUEjtKyTtxVkEK8juZWQgm-0d69egXsfNCPZxjCAVBMseaHpfOefeKIY--Ey9KN5MYn77_u8k9wnU98NCg";
        
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
            Assert.IsType<PahaToken1>(token);
            Assert.NotEmpty(token.InternalErrorMessages);
            Assert.NotEmpty(token.InternalWarningMessages);
        }

        [Fact]
        public void ExtractTokenWithoutOrganizationId()
        {
            var encodedToken = new JwtSecurityToken(tokenNoOrgId);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoOrganizationId, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutOrganizationName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoOrgName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.StartsWith(ErrorMessages.NoOrganizationNamePrefix, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutEmail()
        {
            var encodedToken = new JwtSecurityToken(tokenNoEmail);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoEmail, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractExpiredToken()
        {
            var encodedToken = new JwtSecurityToken(tokenExpired);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Single(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.AfterExpiration, token.InternalErrorMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutFirstName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoFirstName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoFirstName, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutLastName()
        {
            var encodedToken = new JwtSecurityToken(tokenNoLastName);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
            Assert.Single(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(ErrorMessages.NoLastName, token.InternalWarningMessages.First());
        }

        [Fact]
        public void ExtractTokenWithoutPtvRole()
        {
            var encodedToken = new JwtSecurityToken(tokenNoPtvRole);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            Assert.IsType<PahaToken1>(token);
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
            
            Assert.IsType<PahaToken1>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(expectedRole, token.PtvRole);
        }

        [Fact]
        public void ExtractTokenWithRoleFromOrganizations()
        {
            var encodedToken = new JwtSecurityToken(tokenRoleInOrgs);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            
            Assert.IsType<PahaToken1>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(UserAccessRightsGroupEnum.PTV_MAIN_USER, token.PtvRole);
        }

        [Fact]
        public void ExtractTokenWithProperOrganizationsList()
        {
            var encodedToken = new JwtSecurityToken(tokenProperListOrgs);
            var token = PahaTokenBase.ExtractPahaToken(encodedToken.Claims.ToList());
            
            Assert.IsType<PahaToken1>(token);
            Assert.Empty(token.InternalWarningMessages);
            Assert.Empty(token.InternalErrorMessages);
            Assert.Equal(UserAccessRightsGroupEnum.PTV_USER, token.PtvRole);
        }
    }
}
