﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;


namespace truckPRO_api.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            var awsOptions = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
            };
            
            var credentials = new BasicAWSCredentials(
                configuration["AWS:AccessKey"], 
                configuration["AWS:SecretKey"]);
            
            _amazonS3 = new AmazonS3Client(credentials, awsOptions);
            _bucketName = configuration["AWS:BucketName"];
        }

        public async Task<List<string>> UploadPhotos(List<IFormFile> files)
        {
            var urls = new List<string>();
            Console.WriteLine($"In Uploading {files.Count}");
            foreach (var file in files)
            {
                
                var url = await UploadFileAsync(file);
                Console.WriteLine(url);
                urls.Add(url);

            }
            return urls;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileName = Guid.NewGuid() + "_" + file.FileName;
            //Console.WriteLine($"{fileName} {file.FileName}");

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType
            };

            var response = await _amazonS3.PutObjectAsync(request);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"successssssssssss - 'https://{_bucketName}.s3.amazonaws.com/{fileName}'");
                return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
            }
            else
            {
                throw new Exception("File upload failed. Response code: " + response.HttpStatusCode);
            }
        }

        public List<string> GenerateSignedUrls(List<string> fileUrls)
        {
            var signedUrls = new List<string>();

            foreach (var fileUrl in fileUrls)
            {
                var fileName = GetFileNameFromUrl(fileUrl);

                var request = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    Expires = DateTime.UtcNow.AddMinutes(30) 
                };

                var signedUrl = _amazonS3.GetPreSignedURL(request);
                signedUrls.Add(signedUrl);
            }

            return signedUrls;
        }

        private string GetFileNameFromUrl(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            return uri.AbsolutePath.TrimStart('/'); 
        }


        
    }
}
