using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDACAssignment.Data;
using DDACAssignment.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;

namespace DDACAssignment.Views
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot Configuration = builder.Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Configuration["ConnectionStrings:AzureStorageConnectionString-1"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("test-blob-container");
            return container;
        }

        public ImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Image.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.image_id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> file, [Bind("image_id,image_name,room_image")] Image image)
        {
            long size = file.Sum(f => f.Length);
            var filepath = Path.GetTempFileName();
            System.Diagnostics.Debug.WriteLine("im here");
            foreach (var FormFile in file)
            {
                System.Diagnostics.Debug.WriteLine("im in");
                // Check file format
                if (FormFile.ContentType.ToLower() != "image/jpeg" && FormFile.ContentType.ToLower() != "image/png")
                {
                    return BadRequest("The " + Path.GetFullPath(filepath) + " is not an image file. Please ensure a correct file format is being uploaded.");
                }
                //Check file size whether it is empty or not
                else if (FormFile.Length <= 0)
                {
                    return BadRequest("The " + Path.GetFileName(filepath) + " is empty. Please ensure a correct file is being uploaded.");
                }
                // Limit the file size to 1MB
                else if (FormFile.Length > 1048576)
                {
                    return BadRequest("The " + Path.GetFileName(filepath) + " is more than 1 MB. Please ensure the file is less than 1 MB.");
                }
                else
                {
                    CloudBlobContainer container = GetCloudBlobContainer();
                    CloudBlockBlob blob = container.GetBlockBlobReference(image.image_name + "-image.jpg");
                    try
                    {
                        using (var fileStream = FormFile.OpenReadStream())
                        {
                            blob.UploadFromStreamAsync(fileStream).Wait();
                        }
                        //Get URL from blob storage
                        image.room_image = blob.Uri.OriginalString;
                        if (ModelState.IsValid)
                        {
                            _context.Add(image);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        return View(image);
                    }
                    catch (Exception e)
                    {
                        return BadRequest("Failed to create" + e);
                    }
                }
            }
            return View(image);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("image_id,image_name,room_image")] Image image)
        {
            if (id != image.image_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.image_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.image_id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            string blobname = "";
            using (SqlConnection con = new SqlConnection("Server=tcp:hotel-management.database.windows.net,1433;Initial Catalog=aspnet-DDACAssignment-3DBE6A94-C4E2-4946-B051-DF723269B8F9;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
            {
                con.Open();
                SqlCommand da = new SqlCommand("select image_name from Image where image_id='" + id + "'", con);
                using (SqlDataReader reader = da.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        blobname = reader.GetString(0);
                    }
                }
                con.Close();
            }
            System.Diagnostics.Debug.WriteLine(blobname + "-image.jpg");
            System.Diagnostics.Debug.WriteLine("-image.jpg");
            CloudBlobContainer container = GetCloudBlobContainer();
            CloudBlockBlob blob = container.GetBlockBlobReference(blobname + "-image.jpg");
            blob.DeleteAsync().Wait();

            var image = await _context.Image.FindAsync(id);
            _context.Image.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(string id)
        {
            return _context.Image.Any(e => e.image_id == id);
        }
    }
}
