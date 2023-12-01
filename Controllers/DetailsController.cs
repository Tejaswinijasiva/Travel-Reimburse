using Travel_Reimbursement.Models;
using Travel_Reimbursement.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Travel_Reimbursement.ContextDBConfig;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Data.SqlClient;

namespace Travel_Reimbursement.Controllers
{
    public class DetailsController : Controller
    {
        private readonly Travel_ReimbursementDbContext _dbContext;
         
        public DetailsController(Travel_ReimbursementDbContext dbContext)
        {
          
            _dbContext= dbContext;
        }
        [HttpGet]
        [Authorize]
       
        public async Task<IActionResult> index()
        {
            try
            {
                var task=Task.Run(async ()=>
                {
                    var travelTable=await _dbContext.Traveltable.ToListAsync();
                    return travelTable;
                });

                var result=await task;
                return View(result);

            }
            catch(Exception exception)
            {
                ModelState.AddModelError("An error occured.",exception.Message);
                return null;
            }
        }

        [HttpGet]
        public IActionResult create()
        {
            return View();
        }
    
        [HttpPost]
        public async Task<IActionResult> create(AddTravelModel addTravelModel)
        {
            try
                {
                    if(ModelState.IsValid)
                        {
                            if(addTravelModel==null)
                            {
                                throw new ArgumentNullException(nameof(addTravelModel));
                            }
                            if(string.IsNullOrEmpty(addTravelModel.Clientname))
                            {
                                throw new ArgumentException("Name cannot be empty.",nameof(addTravelModel.Clientname));
                            }
                            if(addTravelModel.Clientname=="Admin")
                            {
                                throw new ApplicationException("Cannot create a Client name with the name 'Admin'.");
                            }
                            var employee = new TravelModel()
                            {
                                
                                TravelId=addTravelModel.TravelId,
                                Passportnumber=addTravelModel.Passportnumber,
                                Issue=addTravelModel.Issue,
                                ExpiryDate=addTravelModel.ExpiryDate,
                                Place=addTravelModel.Place,
                                PAN=addTravelModel.PAN,
                                Department=addTravelModel.Department,
                                Project=addTravelModel.Project,
                                Clientname=addTravelModel.Clientname,
                                Country=addTravelModel.Country,
                                Fromdate=addTravelModel.Fromdate,
                                Todate=addTravelModel.Todate,
                                NoofDays=addTravelModel.NoofDays,
                                Perdiem=addTravelModel.Perdiem,
                                Eligible=addTravelModel.Eligible,
                                Date=addTravelModel.Date,
                                Particularofexpenses=addTravelModel.Particularofexpenses,
                                Detailsofexpense=addTravelModel.Detailsofexpense,
                                Currency=addTravelModel.Currency,
                                Amount=addTravelModel.Amount,
                                Mode=addTravelModel.Mode,
                                EmployeeEmail=addTravelModel.EmployeeEmail,
                                Totalperdiem=addTravelModel.Totalperdiem,
                                Totalamount=addTravelModel.Totalamount,
                                Advanceamount=addTravelModel.Advanceamount,
                                Remarks=addTravelModel.Remarks,

                            };
                                await _dbContext.Traveltable.AddAsync(employee);
                                await _dbContext.SaveChangesAsync();
                                return RedirectToAction("index");
           
                        }
                        else
                        {
                            var errorMessage=ModelState.Values.SelectMany(v=>v.Errors).Select(e=>e.ErrorMessage).ToList();
                            TempData["errorMessage"]="Try Again!";
                            return View(addTravelModel);    
                        }
                }
            catch (Exception exception)
            {
                
                ModelState.AddModelError("An error occured.",exception.Message);
            }
            
            return View(addTravelModel);
        }
           
        [HttpGet]
        public async Task<IActionResult> view(int id)
        {
            try
            {
                var employee = await _dbContext.Traveltable.FirstOrDefaultAsync(x=>x.Id==id);
                if(employee!=null)
                {
                    var updateTravelModel=new UpdateTravelModel()
                    {
                    Id=employee.Id,
                    TravelId=employee.TravelId,
                    Passportnumber=employee.Passportnumber, 
                    Issue=employee.Issue,
                    ExpiryDate=employee.ExpiryDate,
                    Place=employee.Place,
                    PAN=employee.PAN,
                    Department=employee.Department,
                    Project=employee.Project,
                    Clientname=employee.Clientname,
                    Country=employee.Country,
                    Fromdate=employee.Fromdate,
                    Todate=employee.Todate,
                    NoofDays=employee.NoofDays,
                    Perdiem=employee.Perdiem,
                    Eligible=employee.Eligible,
                    Date=employee.Date,
                    Particularofexpenses=employee.Particularofexpenses,
                    Detailsofexpense =employee.Detailsofexpense,
                    Currency =employee.Currency,
                    Amount =employee.Amount,
                    Mode =employee.Mode,
                    EmployeeEmail=employee.EmployeeEmail,
                    Totalperdiem =employee.Totalperdiem,
                    Totalamount=employee.Totalamount,
                    Advanceamount=employee.Advanceamount,
                    Remarks=employee.Remarks
                    };
                return await Task.Run(() => View("View",updateTravelModel));
                }
            }
            catch(Exception exception)
                {
                    return View("An error occured.",exception.Message);
                }
                return RedirectToAction("index");  
        }

        [HttpPost]
        public async Task<IActionResult> view(UpdateTravelModel updateTravelModel)
        {  
            try{
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var httpClient = new HttpClient(clientHandler))
         {
            httpClient.BaseAddress = new Uri("http://localhost:5029/api/Travel/");
             httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5029/api/Travel/Get");

        if (response.IsSuccessStatusCode)
        {
            
            var data = response.Content.ReadAsStringAsync().Result;
             
           var employee = JsonConvert.DeserializeObject<List<TravelModel>>(data);
            return View(employee);
        }
        else
        {
            return View("Error");
        }
         
            }
            }
          catch(Exception exception)
                {
                    return View("An error occured.",exception.Message);
                }
    }
 
    

 [HttpPost]
        public async Task<IActionResult> deleteDetails(UpdateTravelModel updateTravelModel)
        {
            try{
             var employee = await _dbContext.Traveltable.FindAsync(updateTravelModel.Id);
            if(employee != null)
            {
                _dbContext.Traveltable.Remove(employee);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("index");
            }
             return RedirectToAction("Employee is null");
            }
             catch(Exception exception)
                {
                    return View("An error occured.",exception.Message);
                }
           
        }

     


        

    }

    
}