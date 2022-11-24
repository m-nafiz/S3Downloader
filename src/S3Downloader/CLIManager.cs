using S3Downloader.AWSS3Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3Downloader
{
    public class CLIManager
    {
        IAWS3Service aws3Service;
        public CLIManager()
        {
            aws3Service = new AWS3Service();
        }

        public async Task TakeInputs()
        {
            Console.WriteLine("Welcome to S3 File downloader");

            await SetupS3Service();

            string input = string.Empty;
            while (input != "exit")
            {
                Console.WriteLine("Enter 1 to list S3 objects");
                Console.WriteLine("Enter 2 to download S3 objects");
                Console.WriteLine("Enter 'exit' to exit");

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        var token = await aws3Service.ListObjectsV2Async(null);
                        while(true)
                        {
                            Console.WriteLine("Enter 'more' to list more S3 objects");
                            Console.WriteLine("Enter 'close' to exit list");
                            input = Console.ReadLine();

                            if (input == "more")
                            {
                                token = await aws3Service.ListObjectsV2Async(token);
                            }
                            else if (input == "close")
                            {
                                break;
                            }
                        } 
                        break;
                    case "2":
                        Console.WriteLine("Enter object key to download a S3 object");
                        input = Console.ReadLine();
                        await aws3Service.DownloadFileAsync(input);
                        Console.WriteLine("S3 object downloaded successfully");
                        break;
                }
            }
        }

        public async Task SetupS3Service()
        {
            string regionName;

            while (true)
            {
                Console.WriteLine("Please enter your AWS region system name. e.g. ca-central-1");
                var regionInput = Console.ReadLine();
                if (aws3Service.IsValidRegionName(regionInput))
                {
                    regionName = regionInput;
                    break;
                }
                else
                {
                    Console.WriteLine("AWS region system name not found, please check your submission");
                }
            }
            S3Configuration config;
            while (true)
            {
                Console.WriteLine("Please enter your S3 bucket name");
                var bucket = Console.ReadLine();
                Console.WriteLine("Please enter your AWS S3 Access Key");
                var accessKey = Console.ReadLine();
                Console.WriteLine("Please enter your AWS S3 Secret Key");
                var secretKey = Console.ReadLine();

                config = new S3Configuration
                {
                    AWSAccessKey = accessKey,
                    AWSSecretKey = secretKey,
                    BucketName = bucket,
                    RegionEndpoint = regionName
                };

                if (await aws3Service.InitS3Client(config))
                {
                    Console.WriteLine("S3 bucket connection successful");
                    break;
                }
                else
                {
                    Console.WriteLine("Bucket does not exist in S3");
                }
            }

            Console.WriteLine("Please enter your download location (provide absolute path)");
            string absolutePath = Console.ReadLine();
            while (!aws3Service.SetDownloadLocation(absolutePath))
            {
                Console.WriteLine("Provide absolute path is not accessible, please try again");
                absolutePath = Console.ReadLine();
            }
        }
    }
}
