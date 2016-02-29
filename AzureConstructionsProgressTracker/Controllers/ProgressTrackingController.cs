using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;

namespace AzureConstructionsProgressTracker.Controllers
{
    [Authorize]
    public class ProgressTrackingController : Controller
    {
        private readonly ConstructionsProgressTrackerContext _db = new ConstructionsProgressTrackerContext();
        private readonly FilesStorageService _filesStorageService;
        private readonly ServiceBusManager _serviceBusManager;

        public ProgressTrackingController()
        {
            _filesStorageService = new FilesStorageService(ConfigurationManager.ConnectionStrings["AzureStorage"].ConnectionString);
            _serviceBusManager = ServiceBusManager.CreateDefault();
        }

        // GET: ProgressTracking
        public async Task<ActionResult> Index()
        {
            var progressTrackingEntries = _db.ProgressTrackingEntries.Include(p => p.ConstructionProject).OrderByDescending(p => p.EntryDate);
            return View(await progressTrackingEntries.ToListAsync());
        }

        // GET: ProgressTracking/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProgressTrackingEntry progressTrackingEntry = await _db.ProgressTrackingEntries.FindAsync(id);
            if (progressTrackingEntry == null)
            {
                return HttpNotFound();
            }
            return View(progressTrackingEntry);
        }

        // GET: ProgressTracking/Create
        public ActionResult Create()
        {
            ViewBag.ConstructionProjectId = new SelectList(_db.ConstructionProjects, "Id", "Name");
            return View();
        }

        // POST: ProgressTracking/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind(Include = "Id,Notes,PictureReference,ConstructionProjectId")] ProgressTrackingEntry progressTrackingEntry)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var mimeType = MimeMapping.GetMimeMapping(fileName);
                        var filePath = await _filesStorageService.UploadFile(fileName, file.InputStream, mimeType);
                        progressTrackingEntry.PictureReference = filePath;
                    }
                }

                progressTrackingEntry.EntryDate = DateTime.Now;
                _db.ProgressTrackingEntries.Add(progressTrackingEntry);
                await _db.SaveChangesAsync();

                await _serviceBusManager.Enqueue(new ResizePictureMessage
                {
                    Id = progressTrackingEntry.Id,
                    PictureReference = progressTrackingEntry.PictureReference
                });

                return RedirectToAction("Index");
            }

            ViewBag.ConstructionProjectId = new SelectList(_db.ConstructionProjects, "Id", "Name", progressTrackingEntry.ConstructionProjectId);
            return View(progressTrackingEntry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
