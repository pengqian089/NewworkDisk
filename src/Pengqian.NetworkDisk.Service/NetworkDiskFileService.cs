using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Pengqian.NetworkDisk.Infrastructure;
using Pengqian.NetworkDisk.Public.Entity;
using Pengqian.NetworkDisk.Public.ViewModel;

namespace Pengqian.NetworkDisk.Service
{
    public class NetworkDiskFileService : BasicService<NetworkDiskFile>
    {
        /// <summary>
        /// 网盘位于服务端的根目录
        /// </summary>
        private readonly string _rootPath;

        public NetworkDiskFileService(string rootPath) : this(rootPath, DbTools.DefaultOption)
        {
        }

        public NetworkDiskFileService(string rootPath, DbOption option) : base(option)
        {
            _rootPath = rootPath;
        }


        public async Task Upload(VmNetworkDiskFile viewModel, Stream stream)
        {
            var ndFile = Mapper.Map<NetworkDiskFile>(viewModel);

            var storagePath = Path.Combine(
                _rootPath, // 根目录
                ndFile.Owner.Id, // 帐号
                string.Join(Path.DirectorySeparatorChar, ndFile.Path)); // 路径
            var filePath = Path.Combine(storagePath, ndFile.FileName);
            if (File.Exists(filePath))
            {
                throw new Exception($"文件【{ndFile.FileName}】在当前目录已存在！");
            }

            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            // var dbFile = await Repository.SearchFor(x => x.Md5 == ndFile.Md5).SingleOrDefaultAsync();
            // if (dbFile != null)
            // {
            //     //Todo is exists
            //     return;
            // }


            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await stream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            var dbFileInfo = await GetNetworkDiskFile(ndFile.FileName, ndFile.Path, ndFile.Owner.Id);
            if (dbFileInfo == null)
            {
                await Repository.InsertAsync(ndFile);
            }
            else
            {
                await Repository.UpdateAsync(dbFileInfo);
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="fileName"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task Delete(string[] virtualPath, string fileName, VmUserInfo userInfo)
        {
            var dbFile = await GetNetworkDiskFile(fileName, virtualPath, userInfo.Id);
            if (dbFile != null)
            {
                var filePath = Path.Combine(_rootPath, dbFile.Owner.Id,
                    string.Join(Path.DirectorySeparatorChar, dbFile.Path), fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                await Repository.DeleteAsync(x => x.Id == dbFile.Id);
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task Delete(string[] virtualPath, VmUserInfo userInfo)
        {
            var list = await Repository.SearchFor(x =>
                    x.Owner.Id == userInfo.Id)
                .ToListAsync();
            list = list.Where(x => x.Path.StartWith(virtualPath)).ToList();
            var storagePath = Path.Combine(
                _rootPath,
                userInfo.Id,
                string.Join(Path.DirectorySeparatorChar, virtualPath));
            var dirInfo = new DirectoryInfo(storagePath);
            if (!dirInfo.Exists)
            {
                return;
            }

            if (dirInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                dirInfo.Delete(true);
            }
            else
            {
                var fileInfo = new FileInfo(dirInfo.FullName);
                if (!fileInfo.Exists)
                {
                    throw new Exception("path error.");
                }

                fileInfo.Delete();
            }

            var ids = list.Select(y => y.Id).ToArray();
            await Repository.DeleteAsync(x => ids.Contains(x.Id));
        }

        /// <summary>
        /// 检索当前用户上传的文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public async Task<List<VmNetworkDiskFile>> SearchMyFile(string fileName, VmUserInfo userInfo)
        {
            var list = await Repository.SearchFor(x =>
                    x.Owner.Id == userInfo.Id &&
                    x.FileName.Contains(fileName))
                .ToListAsync();
            return Mapper.Map<List<VmNetworkDiskFile>>(list);
        }

        /// <summary>
        /// 根据文件名称、路径、帐号查找文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task<NetworkDiskFile> GetNetworkDiskFile(string fileName, string[] path, string account)
        {
            var list = await Repository.SearchFor(x =>
                x.FileName == fileName &&
                x.Owner.Id == account).ToListAsync();
            var dbFile = list.SingleOrDefault(x => x.Path.SequenceEqual(path));
            return dbFile;
        }

        /// <summary>
        /// 对文件重命名
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="fileName"></param>
        /// <param name="userInfo"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public async Task Rename(string[] virtualPath, string fileName, VmUserInfo userInfo, string newName)
        {
            var dbFile = await GetNetworkDiskFile(fileName, virtualPath, userInfo.Id);
            if (dbFile != null)
            {
                var filePath = Path.Combine(_rootPath, dbFile.Owner.Id,
                    string.Join(Path.DirectorySeparatorChar, dbFile.Path), fileName);
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    var newFile = Path.Combine(_rootPath, dbFile.Owner.Id,
                        string.Join(Path.DirectorySeparatorChar, dbFile.Path), newName);
                    fileInfo.MoveTo(newFile);

                    var update = Builders<NetworkDiskFile>.Update.Set(x => x.FileName, newName);
                    //await Repository.DeleteAsync(x => x.Id == dbFile.Id);
                    await Repository.UpdateAsync(x => x.Id == dbFile.Id, update);
                }
            }
        }

        public async Task RenameFolder(string[] virtualPath, VmUserInfo userInfo, string newName)
        {
            var list = await Repository.SearchFor(x =>
                    x.Owner.Id == userInfo.Id)
                .ToListAsync();
            list = list.Where(x => x.Path.StartWith(virtualPath)).ToList();
            var storagePath = Path.Combine(
                _rootPath,
                userInfo.Id,
                string.Join(Path.DirectorySeparatorChar, virtualPath));
            var dirInfo = new DirectoryInfo(storagePath);
            if (!dirInfo.Exists)
            {
                return;
            }

            virtualPath[^1] = newName;
            var newPath = Path.Combine(
                _rootPath,
                userInfo.Id,
                string.Join(Path.DirectorySeparatorChar, virtualPath));
            dirInfo.MoveTo(newPath);

            list.ForEach(x => x.Path[virtualPath.Length - 1] = newName);

            //var writes = new List<WriteModel<NetworkDiskFile>>();
            //Expression<Func<NetworkDiskFile, bool>> express = x => x.Id == 
            var writes = list.Select(x =>
                new UpdateManyModel<NetworkDiskFile>(new ExpressionFilterDefinition<NetworkDiskFile>(y => y.Id == x.Id),
                    Builders<NetworkDiskFile>.Update.Set(y => y.Path, x.Path))).ToList();
            await Repository.Collection.BulkWriteAsync(writes);
            //await Repository.UpdateAsync()
        }
    }
}