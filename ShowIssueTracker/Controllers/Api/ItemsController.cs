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

namespace ShowIssueTracker.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public class ItemsController : Controller
    {

        private IConfiguration _configuration;
        private readonly IDocumentDBRepository<Item> Respository;
        private readonly INadaRepository _nadaRepository;
        private readonly InternalProperties _internalProp;

        public ItemsController(IDocumentDBRepository<Item> Respository,
            IConfiguration Configuration, INadaRepository nadaRepository,
            IOptions<InternalProperties> internalProp)
        {
            this.Respository = Respository;
            _configuration = Configuration;
            _nadaRepository = nadaRepository;
            _internalProp = internalProp.Value;
        }

        [HttpGet]
        [ActionName("GetAllItems")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await Respository.GetItemsAsync(d => !d.Completed); //await Respository.GetItemsAsync(d => !d.Completed);
            return Ok(items);
        }

        [HttpGet]
        [ActionName("GetAllResolvedItems")]
        public async Task<IActionResult> GetAllResolvedItems()
        {
            var items = await Respository.GetItemsAsync(d => d.Completed); //await Respository.GetItemsAsync(d => !d.Completed);
            return Ok(items);
        }

        [HttpGet]
        [ActionName("GetAllInternalItems")]
        public async Task<IActionResult> GetAllInternalItems()
        {
            var items = await Respository.GetItemsAsync(d => !d.Completed && d.Email.Contains("nada.org")); //await Respository.GetItemsAsync(d => !d.Completed);
            return Ok(items);
        }

        [HttpGet]
        [ActionName("GetAllExternalItems")]
        public async Task<IActionResult> GetAllExternalItems()
        {
            var items = await Respository.GetItemsAsync(d => !d.Completed && !(d.Email.Contains("nada.org"))); //await Respository.GetItemsAsync(d => !d.Completed);
            return Ok(items);
        }


    }
}