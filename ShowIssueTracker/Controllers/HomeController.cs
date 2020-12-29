using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; 
using Microsoft.WindowsAzure.Storage.Blob;
using ShowIssueTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks; 
 using Microsoft.Azure.Cosmos.Table;
using ShowIssueTracker.Modal;

namespace ShowIssueTracker.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        //OPTION A: Disables Asp.Net Core's default upload size limit
        //[DisableRequestSizeLimit]
        //OPTION B: Uncomment to set a specified upload file limit
        [RequestSizeLimit(2000000000)]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]

        public async Task<IActionResult> Post(UploadViewModel model, List<IFormFile> files)
        {
            
            var uploadSuccess = false;
            string uploadedUri = null;

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                // NOTE: uncomment either OPTION A or OPTION B to use one approach over another

                // OPTION A: convert to byte array before upload
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    (uploadSuccess, uploadedUri) = await UploadToBlob(formFile.FileName, fileBytes, null);
                    TempData["uploadedUri"] = uploadedUri;

                }

                // OPTION B: read directly from stream for blob upload      
                //using (var stream = formFile.OpenReadStream())
                //{
                //    (uploadSuccess, uploadedUri) = await UploadToBlob(formFile.FileName, null, stream);
                //    TempData["uploadedUri"] = uploadedUri;
                //}

            }

            if (uploadSuccess)

            {
                /// upload to Table
                Submission dataUpload = new Submission(model.firstName, model.lastName)
                {
                    MiddleName = model.MiddleName,
                    DealershipAffiliation = model.DealershipAffiliation,
                    Title = model.Title,
                    city = model.city, 
                    state = model.state,
                    zip = model.zip,
                    YearsInAutomitive = model.YearsInAutomitive,
                    ContactEmail = model.ContactEmail,
                    ContactPhone = model.ContactPhone,
                    BlobUrl = uploadedUri ,
                    Year = "2021",
                    Form = "2021 Women Driving Auto Retail Video Contest"
                };
                string tableName = "NADAUpload";
                string databaseconnectionstring = _configuration["databaseconnectionstring"];
                CloudTable table = await ShowIssueTracker.common.CreateTableAsync(tableName, databaseconnectionstring);

                dataUpload = await InsertOrMergeEntityAsync(table, dataUpload);

                return View("UploadSuccess");
            }
               else
            {
                return View("UploadError");
            }
               
        }

        private async Task<(bool, string)> UploadToBlob(string filename, byte[] imageBuffer = null, Stream stream = null)
        {
            Microsoft.WindowsAzure.Storage.CloudStorageAccount storageAccount = null;
            Microsoft.WindowsAzure.Storage.Blob.CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = _configuration["storageconnectionstring"];

            // Check whether the connection string can be parsed.
            if (Microsoft.WindowsAzure.Storage.CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    Microsoft.WindowsAzure.Storage.Blob.CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'uploadblob' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("womeninworkforce");// ("uploadblob" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateIfNotExistsAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    // Get a reference to the blob address, then upload the file to the blob.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                    List<Task> tasks = new List<Task>();
                    int count = 0;
                    if (imageBuffer != null)
                    {
                        // OPTION A: use imageBuffer (converted from memory stream)
                        await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);
                        //tasks.Add(cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, options, null).ContinueWith((t) =>
                        //{
                        //    sem.Release();
                        //    Interlocked.Increment(ref completed_count);
                        //}));
                        //count++;
                    }
                    else if (stream != null)
                    {
                        // OPTION B: pass in memory stream directly
                        await cloudBlockBlob.UploadFromStreamAsync(stream);
                    }
                    else
                    {
                        return (false, null);
                    }

                    return (true, cloudBlockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
                }
                catch (Microsoft.WindowsAzure.Storage.StorageException ex)
                {
                    return (false, null);
                }
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return (false, null);
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #region remove
        //private static async Task UploadFilesAsync(List<IFormFile> files)
        //{
        //    // Create random 5 characters containers to upload files to.
        //    CloudBlobContainer[] containers = await GetRandomContainersAsync();
        //    var currentdir = System.IO.Directory.GetCurrentDirectory();

        //    // path to the directory to upload
        //    string uploadPath = currentdir + "\\upload";
        //    Stopwatch time = Stopwatch.StartNew();
        //    try
        //    {
        //        Console.WriteLine("Iterating in directory: {0}", uploadPath);
        //        int count = 0;
        //        int max_outstanding = 100;
        //        int completed_count = 0;

        //        // Define the BlobRequestOptions on the upload.
        //        // This includes defining an exponential retry policy to ensure that failed connections are retried with a backoff policy. As multiple large files are being uploaded
        //        // large block sizes this can cause an issue if an exponential retry policy is not defined.  Additionally parallel operations are enabled with a thread count of 8
        //        // This could be should be multiple of the number of cores that the machine has. Lastly MD5 hash validation is disabled for this example, this improves the upload speed.
        //        BlobRequestOptions options = new BlobRequestOptions
        //        {
        //            ParallelOperationThreadCount = 8,
        //            DisableContentMD5Validation = true,
        //            StoreBlobContentMD5 = false
        //        };
        //        // Create a new instance of the SemaphoreSlim class to define the number of threads to use in the application.
        //        SemaphoreSlim sem = new SemaphoreSlim(max_outstanding, max_outstanding);

        //        List<Task> tasks = new List<Task>();
        //        foreach (var formFile in files)
        //        {
        //            if (formFile.Length <= 0)
        //            {
        //                continue;
        //            }
        //        }
        //            // Iterate through the files
        //            foreach (string path in Directory.GetFiles(files))
        //        {
        //            // Create random file names and set the block size that is used for the upload.
        //            var container = containers[count % 5];
        //            string fileName = Path.GetFileName(path);
        //            Console.WriteLine("Uploading {0} to container {1}.", path, container.Name);
        //            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

        //            // Set block size to 100MB.
        //            blockBlob.StreamWriteSizeInBytes = 100 * 1024 * 1024;
        //            await sem.WaitAsync();

        //            // Create tasks for each file that is uploaded. This is added to a collection that executes them all asyncronously.  
        //            tasks.Add(blockBlob.UploadFromFileAsync(path, null, options, null).ContinueWith((t) =>
        //            {
        //                sem.Release();
        //                Interlocked.Increment(ref completed_count);
        //            }));
        //            count++;
        //        }

        //        // Creates an asynchronous task that completes when all the uploads complete.
        //        await Task.WhenAll(tasks);

        //        time.Stop();

        //        Console.WriteLine("Upload has been completed in {0} seconds. Press any key to continue", time.Elapsed.TotalSeconds.ToString());

        //        Console.ReadLine();
        //    }
        //    catch (DirectoryNotFoundException ex)
        //    {
        //        Console.WriteLine("Error parsing files in the directory: {0}", ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        //public static async Task<CloudBlobContainer[]> GetRandomContainersAsync()
        //{
        //    CloudBlobClient blobClient = GetCloudBlobClient();
        //    CloudBlobContainer[] blobContainers = new CloudBlobContainer[5];
        //    for (int i = 0; i < blobContainers.Length; i++)
        //    {
        //        blobContainers[i] = blobClient.GetContainerReference(System.Guid.NewGuid().ToString());
        //        try
        //        {
        //            await blobContainers[i].CreateIfNotExistsAsync();
        //            Console.WriteLine("Created container {0}", blobContainers[i].Uri);
        //        }
        //        catch (StorageException)
        //        {
        //            Console.WriteLine("If you are using the storage emulator, please make sure you have started it. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
        //            Console.ReadLine();
        //            throw;
        //        }
        //    }

        //    return blobContainers;
        //}

        //public static CloudBlobClient GetCloudBlobClient()
        //{
        //    // Load the connection string for use with the application. The storage connection string is stored
        //    // in an environment variable on the machine running the application called storageconnectionstring.
        //    // If the environment variable is created after the application is launched in a console or with Visual
        //    // studio the shell needs to be closed and reloaded to take the environment variable into account.
        //    string storageConnectionString1 = Environment.GetEnvironmentVariable(storageConnectionString );
        //    if (storageConnectionString1 == null)
        //    {
        //        Console.WriteLine(
        //            "A connection string has not been defined in the system environment variables. " +
        //            "Add a environment variable name 'storageconnectionstring' with the actual storage " +
        //            "connection string as a value.");
        //    }
        //    try
        //    {
        //        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString1);
        //        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        //        IRetryPolicy exponentialRetryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(2), 10);
        //        blobClient.DefaultRequestOptions.RetryPolicy = exponentialRetryPolicy;
        //        return blobClient;
        //    }
        //    catch (StorageException ex)
        //    {
        //        Console.WriteLine("Error returned from the service: {0}", ex.Message);
        //        throw;
        //    }
        //}
        #endregion


        public static async Task<Submission> InsertOrMergeEntityAsync(CloudTable table, Submission entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                Submission insertedCustomer = result.Result as Submission;

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedCustomer;
            }
            catch (Microsoft.Azure.Cosmos.Table.StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public static async Task<Submission> RetrieveEntityUsingPointQueryAsync(CloudTable table, string partitionKey, string rowKey)
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<Submission>(partitionKey, rowKey);
                TableResult result = await table.ExecuteAsync(retrieveOperation);
                Submission customer = result.Result as Submission;
                if (customer != null)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
                                                              customer.PartitionKey, customer.RowKey, customer.MiddleName, customer.DealershipAffiliation,
                                                              customer.Title, customer.city, customer.state, customer.zip,
                                                              customer.YearsInAutomitive, customer.ContactEmail, customer.ContactPhone, customer.BlobUrl);
                }

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return customer;
            }
            catch (Microsoft.Azure.Cosmos.Table.StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }
        public static async Task DeleteEntityAsync(CloudTable table, Submission deleteEntity)
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = await table.ExecuteAsync(deleteOperation);

                // Get the request units consumed by the current operation. RequestCharge of a TableResult is only applied to Azure CosmoS DB 
                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (Microsoft.Azure.Cosmos.Table.StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

    }
}
