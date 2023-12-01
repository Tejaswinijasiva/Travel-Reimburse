using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Travel_Reimbursement.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Travel_Reimbursement.ContextDBConfig;
using Travel_Reimbursement.Models.Domain;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Travel_Reimbursement.Filters;


namespace Travel_Reimbursement.Controllers
{
    public class ManagerAccountController:Controller
    {
          private readonly Travel_ReimbursementDbContext _context;
          private readonly IWebHostEnvironment _hostEnvironment;

         
           
           public ManagerAccountController(Travel_ReimbursementDbContext context,IWebHostEnvironment hostEnvironment)
           {
           
            _context=context;
            _hostEnvironment=hostEnvironment;

           }
         
        [HttpGet]
        public IActionResult registerManager()
        {
                return View();         
        }
        
        [HttpPost]
      
        public async Task<ActionResult> registerManager(ManagerRegisterModel register)
        {  try
        {
           if (ModelState.IsValid)  
            {  

              await  _context.ManagerRegistertable.AddAsync(register);
              await  _context.SaveChangesAsync();
              TempData["successMessage"]="Please fill your credentials then login!";
               return RedirectToAction("LoginManager");
            }
            else
            {
                TempData["errorMessage"]="Try Again!";
                return View(register);    
            }
        }
        catch (Exception exception)
        {
          throw new Exception(exception.Message);
          
        }
             
                 
            }
        
         [HttpGet]
          public IActionResult loginManager()
        {
           
           return View();
        }

        [HttpPost]
      
        public async Task<ActionResult> loginManager(ManagerLoginModel login)
        {
          try
          {
             if (ModelState.IsValid)
            {
              
                var data =await _context.ManagerRegistertable.Where(e => e.Username == login.Username).FirstOrDefaultAsync();
                if (data != null)
                {
                    bool isValid = (data.Username == login.Username && data.Password == login.Password);
                    if (isValid)
                    {
                      
                      HttpContext.Session.SetObjectAsJson("users", login);
                        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, login.Username) },
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        HttpContext.Session.SetString("Username", login.Username);
                        return RedirectToAction("Index", "ManagerAccount");
                    }

                    else
                    {
                        TempData["errorPassword"] = "Invalid Password";
                        return View(login);
                    }
                }
                else
                {
                    TempData["errorUsername"] = "Username not found!";
                    return View(login);
                }
            }
          else
            {
                return View(login);
            }
            
          }
          catch (Exception exception)
          {
            
            throw new Exception(exception.Message);
          }
           
        }
          [Authorize]
       
        public IActionResult logoutManager()
        {
          try{
          HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          var storedCookies=Request.Cookies.Keys;
          foreach(var cookies in storedCookies)
          {
            Response.Cookies.Delete(cookies);
          }
           return RedirectToAction("index","Home");
          }
          catch(Exception exception)
          {
             throw new Exception(exception.Message);
          }
        }

         [HttpGet]
         [CustomAuthorizeFilter]
        public async Task<IActionResult> index()
        {
            var table = await _context.Traveltable.ToListAsync();
            return View(table);
        }
          [HttpGet]
        public async Task<IActionResult> view(int? id,TravelModel travelModel)
        {
          string? tempEmail;
            var worker = await _context.Traveltable.FirstOrDefaultAsync(x=>x.Id==id);
            if(worker!=null)
            {
                var ViewModel=new UpdateTravelModel()
                
                {
                 tempEmail=worker.EmployeeEmail,
                    Id=worker.Id,
                    TravelId=worker.TravelId,
                    Passportnumber=worker.Passportnumber, 
                    Issue=worker.Issue,
                    ExpiryDate=worker.ExpiryDate,
                    Place=worker.Place,
                    PAN=worker.PAN,
                    Department=worker.Department,
                    Project=worker.Project,
                    Clientname=worker.Clientname,
                    Country=worker.Country,
                    Fromdate=worker.Fromdate,
                    Todate=worker.Todate,
                    NoofDays=worker.NoofDays,
                    Perdiem=worker.Perdiem,
                    Eligible=worker.Eligible,
                    Date=worker.Date,
                    Particularofexpenses=worker.Particularofexpenses,
                    Detailsofexpense =worker.Detailsofexpense,
                    Currency =worker.Currency,
                    Amount =worker.Amount,
                    Mode =worker.Mode,
                    EmployeeEmail=worker.EmployeeEmail,
                    Totalperdiem =worker.Totalperdiem,
                    Totalamount=worker.Totalamount,
                    Advanceamount=worker.Advanceamount,
                    Remarks=worker.Remarks
                };
                return await Task.Run(() => View("View",ViewModel));
                TempData["myData"]=tempEmail;
                 Console.WriteLine(tempEmail);
                
            }
            return RedirectToAction("index");
        }
        [HttpPost]
        public async Task<IActionResult> view(UpdateTravelModel model)
        {
        
            var worker= await _context.Traveltable.FindAsync(model.Id);
            if(worker!=null)
            {
              
                    worker.Id=worker.Id;
                    worker.TravelId=worker.TravelId;
                    worker.Passportnumber=worker.Passportnumber; 
                    worker.Issue=worker.Issue;
                    worker.ExpiryDate=worker.ExpiryDate;
                    worker.Place=worker.Place;
                    worker.PAN=worker.PAN;
                    worker.Department=worker.Department;
                    worker.Project=worker.Project;
                    worker.Clientname=worker.Clientname;
                    worker.Country=worker.Country;
                    worker.Fromdate=worker.Fromdate;
                    worker.Todate=worker.Todate;
                    worker.NoofDays=worker.NoofDays;
                    worker.Perdiem=worker.Perdiem;
                    worker.Eligible=worker.Eligible;
                    worker.Date=worker.Date;
                    worker.Particularofexpenses=worker.Particularofexpenses;
                    worker.Detailsofexpense =worker.Detailsofexpense;
                    worker.Currency =worker.Currency;
                    worker.Amount =worker.Amount;
                    worker.Mode =worker.Mode;
                    worker.EmployeeEmail=worker.EmployeeEmail;
                    worker.Totalperdiem =worker.Totalperdiem;
                    worker.Totalamount=worker.Totalamount;
                    worker.Advanceamount=worker.Advanceamount;
                    worker.Remarks=worker.Remarks;

                  await _context.SaveChangesAsync();
              

                  return RedirectToAction("index");

            }
            return RedirectToAction("index");
        }
        [HttpGet]
    public async Task<IActionResult> indexFile()
    {
       
        var uploadFile=await _context.Filetable.ToListAsync();
        return View(uploadFile);
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
     
      public async Task<IActionResult> fileUApload([Bind("ImageId,Title,ImageFile")] FileModel fileModel)
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
            ModelState.AddModelError(string.Empty,$"Something went wrong invalid model");
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
          var fileModel=await _context.Filetable.FindAsync(id);

          var imagePath=Path.Combine(_hostEnvironment.WebRootPath,"image",fileModel.ImagePath);
          if(System.IO.File.Exists(imagePath))
             System.IO.File.Delete(imagePath);


          _context.Filetable.Remove(fileModel);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(indexFile));
        }

        private bool fileModelExists(int id)
        {
          return _context.Filetable.Any(e => e.ImageId==id);
        }
        [HttpGet]
        public async Task<IActionResult> approve(int id,TravelModel travelModel)
        {
           string? myData=TempData["myData"] as string;
           TempData.Keep("myData");
          if(_context.Traveltable!=null){
        var employees = await _context.Traveltable.Where(x => x.Id == id).FirstOrDefaultAsync();
        return View(employees);
        }
        return View();
        }
         [HttpGet]
        public async Task<IActionResult> decline(int id,TravelModel travelModel)
        {
           string? myData=TempData["myData"] as string;
            TempData.Keep("myData");
          if(_context.Traveltable!=null){
        var employees = await _context.Traveltable.Where(x => x.Id == id).FirstOrDefaultAsync();
        return View(employees);
        }
        return View();
        }
         public async Task<IActionResult> approve(TravelModel travelModel)
        {

          
                            string from, pass, messageBody;
                            string? myData=TempData["myData"] as string;
                            Console.WriteLine(myData);
                            MailMessage message = new MailMessage();
                            from = "gayuraji1212@gmail.com";
                            pass = "ayvlczdpqrzbdvjl";
                           string? tempemail="gayuraji1212@gmail.com";
                            messageBody = "Your Request has been Accepted";
                            message.To.Add(new MailAddress(tempemail));
                            message.From = new MailAddress(from);
                            message.Body = messageBody;
                            message.Subject = "Regarding Travel Details Accepted ";
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                            smtp.EnableSsl = true;
                            smtp.Port = 587;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential(from, pass);                  
                            smtp.Send(message);
            return View();
        }

         public async Task<IActionResult> decline(TravelModel travelModel)
        {
                            string from, pass, messageBody;
                            MailMessage message = new MailMessage();
                            from = "gayathri1212200@gmail.com";
                            pass = "ayvlczdpqrzbdvjl";
                            string? tempemail="gayuraji1212@gmail.com";
                            messageBody = "Your Request has been Declined";
                            message.To.Add(new MailAddress(tempemail));
                            message.From = new MailAddress(from);
                            message.Body = messageBody;
                            message.Subject = "Regarding Travel Details Declined";
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                            smtp.EnableSsl = true;
                            smtp.Port = 587;
                            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                            smtp.UseDefaultCredentials = false;
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential(from, pass);                  
                            smtp.Send(message);
            return View();
        }
        
    }
}
