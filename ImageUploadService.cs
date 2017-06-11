 public string UploadBlogImage(Stream fileStream, string fileName, int bId = 0)
        {
            try
            {
                var g = Guid.NewGuid();
                string fileNameAtAws = "g.ToString() + "_" + fileName.Replace("+", "0");
                AWSCredentials awsCredentials = new BasicAWSCredentials(_configService.AwsAccessKeyId, _configService.AwsSecret);
                AmazonS3Client amazonS3 = new AmazonS3Client(awsCredentials, Amazon.RegionEndpoint.USWest2);
                TransferUtility fileTransferUtility = new TransferUtility(amazonS3);
                TransferUtilityUploadRequest uploadRequest = new TransferUtilityUploadRequest();
              
                uploadRequest.BucketName = _configService.AwsBucketName;
                uploadRequest.InputStream = fileStream;
                uploadRequest.Key = fileNameAtAws;
            
                fileTransferUtility.Upload(uploadRequest);

                ImagesAddRequest request = new ImagesAddRequest();
                request.ImageUrl = fileNameAtAws;
                request.Title = fileName;

                int imgId = _imagesService.Insert(request, _userService.GetCurrentUserId());
                Image update = new Image();
                update.ImageUrl = fileNameAtAws;


                if (imgId > 0 && bId > 0)
                {
                    _imagesService.UpdateBlogImages(update, bId);
                }

                return fileNameAtAws;
            }
            catch (AmazonS3Exception)
            {
                throw;
            }
        }