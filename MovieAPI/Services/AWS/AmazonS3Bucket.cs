using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MovieAPI.Models;
using System;
using static MovieAPI.Models.EnumObject;
namespace MovieAPI.Services.AWS
{

    public static class AmazonS3Bucket
    {

        public static async Task<ApiResponse> UploadFile(IAmazonS3 s3Client,IFormFile file, FileType fileType)
        {
            string prefix = "";
            switch (fileType)
            {
                case FileType.Image:
                    prefix = "Image";
                    break;
                case FileType.Video:
                    prefix = "Video";
                    break;
                case FileType.OtherFile:
                    prefix = "OtherFile";
                    break;
            }
            var bucket = AppSettings.AWSS3BucketName;
            var bucketExists = await s3Client.DoesS3BucketExistAsync(bucket);
            if (!bucketExists)
            {
                throw new Exception($"Bucket {bucket} does not exist.");
            }
            var key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}";
            var request = new PutObjectRequest()
            {
                BucketName = bucket,
                Key = key,
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await s3Client.PutObjectAsync(request);
            var expiryUrlRequest = new GetPreSignedUrlRequest()
            {
                BucketName = bucket,
                Key = key,
                Expires = DateTime.UtcNow.AddYears(1)
            };

            string url = s3Client.GetPreSignedURL(expiryUrlRequest);
            return new ApiResponse
            {
                IsSuccess = true,
                Message = $"File {prefix}/{file.FileName} uploaded to S3 successfully!",
                Data = url
            };

        }
        public static async Task<object> GetAllFiles(IAmazonS3 s3Client)
        {
            var bucket = AppSettings.AWSS3BucketName;
            var bucketExists = await s3Client.DoesS3BucketExistAsync(bucket);
            if (!bucketExists)
            {
                throw new Exception($"Bucket {AppSettings.AWSS3BucketName} does not exist.");
            }
            var request = new ListObjectsV2Request()
            {
                BucketName = bucket,
                Prefix = "Image"
            };
            var result = await s3Client.ListObjectsV2Async(request);
            var s3Objects = result.S3Objects.Select(s =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = bucket,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddYears(1),

                };
                return new S3ObjectDto()
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = s3Client.GetPreSignedURL(urlRequest),
                };
            });

            return s3Objects;
        }
        public static async Task<ApiResponse> DeleteFile(IAmazonS3 s3Client, string key, FileType fileType)
        {
            string prefix = "";
            switch (fileType)
            {
                case FileType.Image:
                    prefix = "Image";
                    break;
                case FileType.Video:
                    prefix = "Video";
                    break;
                case FileType.OtherFile:
                    prefix = "OtherFile";
                    break;
            }
            var hostKey = prefix + "\\" + key;
            var bucket = AppSettings.AWSS3BucketName;
            var bucketExists = await s3Client.DoesS3BucketExistAsync(bucket);
            if (!bucketExists)
            {
                throw new Exception($"Bucket {bucket} does not exist");
            }
            await s3Client.DeleteObjectAsync(bucket, hostKey);
            return new ApiResponse
            {
                IsSuccess = true,
                Message = ""
            };
        }
    }
}
