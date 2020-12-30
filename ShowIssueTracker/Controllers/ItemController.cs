using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Http; 
using Microsoft.Extensions.Configuration; 
using System.Diagnostics;
using System.IO;
using ShowIssueTracker.Models;
using ShowIssueTracker.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Identity;

namespace ShowIssueTracker.Controllers
{
    [Authorize]
    public class ItemController : Controller
    {
        private IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDocumentDBRepository<Item> Respository;
        private readonly INadaRepository _nadaRepository;
        private readonly InternalProperties _internalProp;
        public ItemController(IDocumentDBRepository<Item> Respository, 
            IConfiguration Configuration, INadaRepository nadaRepository, 
            IOptions<InternalProperties> internalProp, UserManager<ApplicationUser> userManager)
        {
            this.Respository = Respository;
            _configuration = Configuration;
            _nadaRepository = nadaRepository;
            _internalProp = internalProp.Value;
            _userManager = userManager;
        }

        
        [ActionName("Index")]
        public async Task<IActionResult> Index()
        {
           
            var items = await Respository.GetItemsAsync(d => !d.Completed); //await Respository.GetItemsAsync(d => !d.Completed);
            return View(items);
        }

        [ActionName("Resolved")]
        public async Task<IActionResult> Resolved()
        {
            var items = await Respository.GetItemsAsync(d => d.Completed ); //await Respository.GetItemsAsync(d => !d.Completed);
            return View(items);
        }

        [ActionName("Internal")]
        public async Task<IActionResult> Internal()
        {
            var items = await Respository.GetItemsAsync(d => d.Completed && d.Email.Contains("nada.org")); //await Respository.GetItemsAsync(d => !d.Completed);
            return View(items);
        }

        [ActionName("External")]
        public async Task<IActionResult> External()
        {
            var items = await Respository.GetItemsAsync(d => d.Completed && !(d.Email.Contains("nada.org"))); //await Respository.GetItemsAsync(d => !d.Completed);
            return View(items);
        }




#pragma warning disable 1998
        [AllowAnonymous]
        [ActionName("Create")]
        public async Task<IActionResult> CreateAsync()
        {
            Item item = new Item()
            {
                EntryTime = DateTime.Today,
                LastSavedTime = DateTime.Today
            };
            return View("Create", item);
        }

        [AllowAnonymous]
        [ActionName("Post")]
        public async Task<IActionResult> CreateAsync(string name, string email)
        {

            Item item = new Item()
            {
                 FullName = name,
                 Email =email,
                 EntryTime = DateTime.Today, 
                 LastSavedTime = DateTime.Today
            };

            return View("Create", item);
        }

#pragma warning restore 1998
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Create")]
        //[ValidateAntiForgeryToken]
        [RequestSizeLimit(2147483647)]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> CreateAsync(Item item, List<IFormFile> files)
        {
                 //////////////
                bool allVideosUploaded = false;
                var i = 0;
                var count = files.Count();

                if(count >0 )
                {
                    foreach (var formFile in files)
                    {

                        var uploadSuccess = false;
                        string uploadedUri = null;
                        if (formFile.Length <= 0)
                        {
                            continue;
                        }
                         
                        //OPTION B: read directly from stream for blob upload
                        using (var stream = formFile.OpenReadStream())
                        {

                            (uploadSuccess, uploadedUri) = await UploadToBlob(item.FullName.Trim() + i + (new Random()).Next(100, 1000) + formFile.FileName, null, stream);
                            TempData["uploadedUri"] = uploadedUri;
                            item.BlobUrl = uploadedUri;
                            item.EntryTime = DateTime.Now;
                            item.Status = "New";

                            item.LastSavedBy = item.Email;
                            item.LastSavedTime = DateTime.Now;


                            i = i + 1;
                            if (uploadSuccess)

                            {
                                await Respository.CreateItemAsync(item);
                                // return View("UploadSuccess");
                            }
                            else
                            {
                                return View("UploadError");
                            }
                            if (i == count)
                            {
                                allVideosUploaded = true;
                            }
                        }

                    }
                }
                else
                {
                    item.BlobUrl = "No Files Uploaded";
                    item.EntryTime = DateTime.Now;
                    item.Status = "New";
                    item.LastSavedBy = item.Email;
                    item.LastSavedTime = DateTime.Now;
                    await Respository.CreateItemAsync(item);
                    allVideosUploaded = true;
                }

                if (allVideosUploaded)
                 {
                    /// send email 
                    var emailProp = new SendEmail();
                    emailProp.Subject = _internalProp.secretCode;
                    emailProp.ToEmail = _internalProp.toAddress;
                    var body = " <table> <tr><td>FirstName : "+ item.FullName+ "</td></tr> <tr><td>LastName : " + item.Issue + "</td></tr><tr><td>ContactEmail : " + item.IssueNotes + "</td></tr>  <tr><td>Video URL : " + item.BlobUrl + "</td></tr> </table> ";
                    emailProp.Body = body;
                    emailProp.CCEmail = _internalProp.ccAddress;
                    var dataRtn = await SendEmail(emailProp);
                    
                   return View("UploadSuccess");
                }
                else
                {
                    return View("UploadError");
                }
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> EditAsync([Bind("Id,FullName,Email,Issue,Role,IssueType,Description,Status,AssignedTo,IssueNotes,isComplete,PublicUrl,EntryTime,LastSavedBy,LastSavedTime,Priority,BlobUrl")] Item item)
        public async Task<ActionResult> EditAsync( Item item)
        {
            var user = _userManager.GetUserName(User);
            if (user == null)
            {
                return Json(new { success = false, value = "Please Log in to Continue." });
            }
            if (ModelState.IsValid)
            {
                if(item.Status == "Resolved" || item.Status == "No Action Required")
                {
                    item.Completed = true; 
                }
                else
                {
                    item.Completed = false; 
                }

                item.LastSavedBy = user; 
                item.LastSavedTime = DateTime.Now;

                await Respository.UpdateItemAsync(item.Id, item);
                return RedirectToAction("Index");
            }

            return View(item);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> EditAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Item item = await Respository.GetItemAsync(id);
            
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            Item item = await Respository.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("Id")] string id)
        {
            await Respository.DeleteItemAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Details")]
        public async Task<ActionResult> DetailsAsync(string id)
        {
            Item item = await Respository.GetItemAsync(id);
            return View(item);
        }

        [ActionName("DetailsOld")]
        public async Task<ActionResult> DetailsOldAsync(string id)
        {
            Item item = await Respository.GetItemAsync(id);
            return View(item);
        }

        #region Blob storage 
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
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("issuetracker");// ("uploadblob" + Guid.NewGuid().ToString());
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

        #endregion

        [HttpPost]
        public JsonResult ProgressBar(IList<IFormFile> files)
        {
            return Json(new { state = 0, message = string.Empty });
        }


        [HttpPost]
        public async Task<string> SendEmail([FromBody] SendEmail email)
        {
            var response = await _nadaRepository.SendGridEmailAsync(email.ToEmail, email.Subject, email.Body, email.CCEmail);
            return response;
        }
    }
}