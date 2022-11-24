using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Downloader.AWSS3Service
{
    public class S3Configuration
    {
        public string BucketName { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string RegionEndpoint { get; set; }
        public string LocalAbsolutePath { get; set; }
    }
}
