using Ipfs.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GCCars.Application.Services
{
    public class IpfsService
    {
        private readonly ILogger<IpfsService> logger;
        private readonly IpfsClient client = new("https://ipfs.io/ipfs");

        public IpfsService(ILogger<IpfsService> logger)
        {
            this.logger = logger;
        }

        public async Task<string> GetFile(string jsonPath)
        {
            logger.LogDebug($"Reading file '{jsonPath}'");
            var fileContent = await client.FileSystem.ReadAllTextAsync(jsonPath).ConfigureAwait(false);
            logger.LogTrace($"Read file content: {fileContent}");
            return fileContent;
        }

        public async Task<string[]> GetFileList(string path)
        {
            logger.LogDebug($"Reading file list in \"{path}\"");
            try
            {
                ///todo: здесь не работает чтение содержимого папки, поэтому сделан костыль в обработке исключения. переделать нормально.
                var fileNode = await client.FileSystem.ListFileAsync(path).ConfigureAwait(false);
                var result = fileNode.Links.Select(r => r.Name).ToArray();
                logger.LogTrace($"Read file list: {string.Join(", ", result)}");
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Reading file list has failed: {ex.Message}.");
                return Enumerable.Range(1, 30)
                    .Select(x => $"{x}.json")
                    .ToArray();
            }
        }
    }
}
