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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Cloud
{
    public interface IStorageClient : IDisposable
    {
        Task<bool> SaveFileAsync(string path, string fileName, string fileContent);
        Task<bool> SaveFileAsync(string path, string fileName, string fileContent, CancellationToken token);
        bool SaveFile(string path, string fileName, string fileContent);
        bool UploadFile(string path, string fileName, string filePath);

        Task<List<VmFileInfo>> ListFilesAsync(string path = null, int? count = null);
        Task<List<VmFileInfo>> ListFilesAsync(string path, CancellationToken token, int? count = null);
        List<VmFileInfo> ListFiles(string path = null, int? count = null);

        Task<string> ReadFileAsync(string path, string fileName);
        Task<string> ReadFileAsync(string path, string fileName, CancellationToken token);
        string ReadFile(string path, string fileName);

        Task<bool> DeleteFileAsync(string path, string fileName);
        Task<bool> DeleteFileAsync(string path, string fileName, CancellationToken token);
        bool DeleteFile(string path, string fileName);

        Task<bool> CopyFileAsync(string sourceFolder, string destFolder, string sourceFile, string destFile = null);
        Task<bool> CopyFileAsync(string sourceFolder, string destFolder, string sourceFile, string destFile, CancellationToken token);
        bool CopyFile(string sourceFolder, string destFolder, string sourceFile, string destFile = null);

        Task<bool> MoveFileAsync(string oldFolder, string newFolder, string fileName, string newFileName = null);

        Task<bool> MoveFileAsync(string oldFolder, string newFolder, string fileName, string newFileName,
            CancellationToken token);        
        bool MoveFile(string oldFolder, string newFolder, string fileName, string newFileName = null);

    }
}
