using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Reimbursement.ContextDBConfig;
using Travel_Reimbursement.Models;

namespace Travel_Reimbursement.Controllers;

public class FileController : Controller
{
    private readonly Travel_ReimbursementDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public FileController(Travel_ReimbursementDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment=hostEnvironment;
    }
    
[HttpGet]
  [Authorize]
    public async Task<IActionResult> indexFile()
    {
       try
       {
        var uploadFile=await _context.Filetable.ToListAsync();
        return View(uploadFile);
       }
       catch (Exception exception)
       {
        
        throw new Exception(exception.Message);
       }
      
    }
      public async Task<IActionResult> fileTable(int? id)
      {
        if(id==null)
        {
            return NotFound();
        }
        var fileModel = await _context.Filetable.FirstOrDefaultAsync(m=> m.ImageId == id);
        if(fileModel==null)
        {
            return NotFound();
        }
        return View(fileModel);
      }
      [HttpGet]
      public IActionResult fileUpload()
      {
        return View();
      }
      [HttpPost]
     
      public async Task<IActionResult> fileUpload([Bind("ImageId,Title,ImageFile")] FileModel fileModel)
      {
        if(ModelState.IsValid)
        {
          try{
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(fileModel.ImageFile.FileName);
            string extension = Path.GetExtension(fileModel.ImageFile.FileName);
            fileModel.ImagePath=fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path=Path.Combine(wwwRootPath+"/Image/", fileName);
            using (var fileStream = new FileStream(path,FileMode.Create))
            {
                await fileModel.ImageFile.CopyToAsync(fileStream);
            }

            _context.Add(fileModel);
            await _context.SaveChangesAsync();
            return RedirectToAction("IndexFile");

          }
          catch(Exception exception)
                {
                    ModelState.AddModelError(string.Empty,$"Something went wrong {exception.Message}");
                }
            }
           
            return View(fileModel);  
      }
       

     
    

        public async Task<IActionResult> deleteFile(int? id)
        {
          if(id==null)
          {
            return NotFound();
          }
          var fileModel= await _context.Filetable.FirstOrDefaultAsync(m=>m.ImageId==id);
          if(fileModel==null)
          {
            return NotFound();
          }
          return View(fileModel);
        }
        [HttpPost, ActionName("DeleteFile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteConfirmed(int id)
        {
          try
          {
               var fileModel=await _context.Filetable.FindAsync(id);

          var imagePath=Path.Combine(_hostEnvironment.WebRootPath,"image",fileModel.ImagePath);
          if(System.IO.File.Exists(imagePath))
             System.IO.File.Delete(imagePath);


          _context.Filetable.Remove(fileModel);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(indexFile));
          }
          catch (Exception exception)
          {
            
            throw new Exception(exception.Message);
          }
       
        }

        private bool FileModelExists(int id)
        {
          return _context.Filetable.Any(e => e.ImageId==id);
        }
    
}