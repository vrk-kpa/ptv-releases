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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DockerDeployService.Controllers
{
    [Route("DoDeploy")]
    public class DeploymentController : Controller
    {
        private static readonly object antiConcurrentLockDev = new object();
        private static readonly object antiConcurrentLockTest = new object();

        private readonly ServiceEnvSetting settings;

        public DeploymentController(IOptions<ServiceEnvSetting> settings)
        {
            this.settings = settings.Value;
        }
        [HttpGet]
        public IActionResult DoDeploy(string env, string password)
        {
            PtvAppEnvSetting useEnv;
            object useLock;
            switch (env.ToLowerInvariant())
            {
                case "dev":
                {
                    useEnv = settings.Dev;
                    useLock = antiConcurrentLockDev;
                   break;
                }
                case "test":
                {
                    useEnv = settings.Test;
                    useLock = antiConcurrentLockTest;
                    break;
                }
                default: return Content("Unknown environment");
            }
            if (useEnv.Password != password)
            {
                return Content("Wrong password");
            }
            var startTime = Environment.TickCount;
            int exitCode = 0;
            try
            {
                using (Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = useEnv.Command ?? "sh",
                        Arguments = useEnv.Script,
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardError = false,
                        RedirectStandardInput = false
                    }
                })
                {
                    lock (useLock)
                    {
                        proc.Start();
                        if (!proc.WaitForExit(15 * (60 * 1000))) // 15 minutes
                        {
                            throw new Exception("Process timeouted.");
                        }
                    }
                    exitCode = proc.ExitCode;
                }
            }
            catch (Exception e)
            {
                return Content("Error\n"+e.Message);
            }
            var endTime = Environment.TickCount;
            return Content($"Done. In {(endTime-startTime) / 1000} secs. Exit code {exitCode}");
        }
    }
}
