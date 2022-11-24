using Amazon.S3.Model;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amazon;

namespace S3Downloader.AWSS3Service
{
    public class AWS3Service : IAWS3Service
    {
        private IAmazonS3 _amazonS3 { get; set; }
        private S3Configuration _s3Configuration { get; set; }
        private string _absolutePath { get; set; }
        public async Task<bool> InitS3Client(S3Configuration configuration)
        {
            bool initSuccess = false;
            if (await TestBucketConnection(configuration))
            {
                _s3Configuration = configuration;
                _amazonS3 = new AmazonS3Client(_s3Configuration.AWSAccessKey, _s3Configuration.AWSSecretKey, RegionEndpoint.GetBySystemName(_s3Configuration.RegionEndpoint));
                initSuccess = true;
            }
            return initSuccess;
        }

        private async Task<bool> TestBucketConnection(S3Configuration configuration)
        {
            IAmazonS3 amazonS3 = new AmazonS3Client(configuration.AWSAccessKey, configuration.AWSSecretKey, RegionEndpoint.GetBySystemName(configuration.RegionEndpoint));
            var bucketExists = await amazonS3.DoesS3BucketExistAsync(configuration.BucketName);
            amazonS3.Dispose();
            return bucketExists;
        }

        public bool SetDownloadLocation(string absolutePath)
        {
            bool isSet = false;
            if (IsDirectoryWritable(absolutePath))
            {
                _absolutePath = absolutePath;
                isSet = true;
            }
            return isSet;
        }

        public async Task DownloadFileAsync(string fileKey)
        {
            using (var obj = await _amazonS3.GetObjectAsync(_s3Configuration.BucketName, fileKey))
            {
                var cancelSource = new CancellationTokenSource();
                var fullPath = string.Format("{0}\\{1}", _absolutePath, fileKey);
                await obj.WriteResponseStreamToFileAsync(fullPath, false, cancelSource.Token);
            }
        }

        public async Task FullS3Download()
        {
            string continuationToken = null;
            int maxKeys = 10;
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = _s3Configuration.BucketName,
                MaxKeys = maxKeys,
                ContinuationToken = continuationToken
            };

            while (true)
            {
                ListObjectsV2Response response = await _amazonS3.ListObjectsV2Async(request);

                foreach (var s3Object in response.S3Objects)
                {
                    await DownloadFileAsync(s3Object.Key);
                    Console.WriteLine("Downloaded: " + s3Object.Key);
                }

                request = new ListObjectsV2Request
                {
                    BucketName = _s3Configuration.BucketName,
                    MaxKeys = maxKeys,
                    ContinuationToken = response.NextContinuationToken
                };

                if (response.KeyCount < maxKeys)
                {
                    break;
                }
            }
        }

        public async Task<string> ListObjectsV2Async(string continuationToken)
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = _s3Configuration.BucketName,
                MaxKeys = 10,
                ContinuationToken = continuationToken
            };

            ListObjectsV2Response response = await _amazonS3.ListObjectsV2Async(request);

            foreach (var s3Object in response.S3Objects)
            {
                string displayText = string.Format(@"{0} {1} {2}", s3Object.LastModified, s3Object.Owner, s3Object.Key);
                Console.WriteLine(displayText);
            }
            return response.NextContinuationToken;
        }

        public bool IsValidRegionName(string systemName)
        {
            bool isValid = false;
            var region = RegionEndpoint.GetBySystemName(systemName);
            if (region.DisplayName != "Unknown")
            {
                isValid = true;
            }
            return isValid;
        }

        private bool IsDirectoryWritable(string dirPath)
        {
            return Directory.Exists(dirPath);
            //try
            //{
            //    using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
            //    { }
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
    }
}
