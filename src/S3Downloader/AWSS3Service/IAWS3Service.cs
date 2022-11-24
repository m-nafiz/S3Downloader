using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Downloader.AWSS3Service
{
    public interface IAWS3Service
    {
        bool SetDownloadLocation(string absolutePath);
        Task<bool> InitS3Client(S3Configuration configuration);
        Task DownloadFileAsync(string file);
        Task FullS3Download();
        Task<string> ListObjectsV2Async(string continuationToken);
        bool IsValidRegionName(string systemName);
    }
}
