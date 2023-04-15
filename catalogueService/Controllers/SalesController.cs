using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using catalogueService.Interfaces;
using catalogueService.Repositories;
using AutoMapper;
using catalogueService.Database;
using catalogueService.Models;
using catalogueService.requestETresponse;
using System;
using Microsoft.AspNetCore.Authorization;
using catalogueService.Database.DBContextFiles;
using System.Security.Claims;
using catalogueService.Database.DBsets;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Text;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using System.Web;
using iText;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text.pdf.parser;

namespace catalogueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Super Admin")]
    public class SalesController : Controller
    {
        private readonly ISale _saleRep;
        private readonly IMapper _mapper;
        private readonly ICustomer _customerRep;
        private readonly IUser _userRep;
        private readonly ISqlprocess _databaseHandler;
        private readonly catalogueDBContext _dbcontext;
        public SalesController(IOrder orderRep, IMapper mapper, ISale saleRep, 
            ISqlprocess databaseHandler, ICustomer customerRep, IUser userRep, catalogueDBContext dbcontext)
        { 
            this._mapper = mapper;
            this._saleRep = saleRep;
            this._databaseHandler = databaseHandler;
            this._customerRep = customerRep;
            this._userRep = userRep;
            this._dbcontext = dbcontext;
        }

        [HttpGet("View all Sales")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            var (IsSucess, mbokoDomain) = await _saleRep.GetSalesAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            var mbokoDTO = _mapper.Map<IEnumerable<saleModel>>(mbokoDomain);
            return Ok(mbokoDTO);
        }

        [HttpGet("View Specific Sale")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var saleOrder = await _saleRep.GetByIdAsync(id);
            if (saleOrder == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var saleDTO = _mapper.Map<saleModel>(saleOrder);

            return Ok(saleDTO);
        }

        [HttpPost("Generate Sales Report")]
        public async Task<IActionResult> GenerateReportAsync()
        {
            int userID;
            try
            {
                #region I intended to develop an algorithm that collects all information from the database table to generate a report in pdf format.
                #endregion
                
                var thisUser = GetCurrentUser();
                var users = await _dbcontext.Users.ToListAsync();
                foreach (var user in users)
                {
                    if (user.userName == thisUser.userName)
                    {
                        userID = user.userId;
                        //var myName = $"{thisUser.firstName} {thisUser.lastName}";
                        var customer = await _customerRep.GetByUserIdAsync(userID);
                        var customerID = customer.customerId;

                        var query = "Select * from  sales where customerID = @customerID";

                        SqlParameter[] Params = new SqlParameter[]
                        {
                            new SqlParameter("@customerID",customerID)
                        };

                        var salesTable = await _databaseHandler.retrieveRecords(query, CommandType.Text, Params);
                        if (!salesTable.queryIsSuccessful)
                        {
                            return Ok("Unable to retrieve sale records");
                        }

                        var thisTable = salesTable.objectValue;

                        string filePath = "./ReportTemplate/reports.html";
                        //string message = File().ReadAllText(filePath);
                        string receipthtml = System.IO.File.ReadAllText(filePath);

                        StringBuilder strHTMLBuilder = new StringBuilder(receipthtml);
                        strHTMLBuilder = strHTMLBuilder.Replace("{CustomerName}", $"{thisUser.firstName} {thisUser.lastName}");
                        strHTMLBuilder = strHTMLBuilder.Replace("{Table}", thisTable.ToString());
                        strHTMLBuilder = strHTMLBuilder.Replace("{userEmail}", thisUser.userName);
                        strHTMLBuilder = strHTMLBuilder.Replace("{Transdate}", DateTime.Now.ToString("yyyy-MM-dd"));

                        Document document = new Document(PageSize.A4, 25, 25, 25, 25);
                        PdfWriter.GetInstance(document, new FileStream("./GeneratedReports/" + thisUser.userId + "paymentsReport.html", FileMode.Create));

                        // Open the document
                        document.Open();

                        // Add the StringBuilder content to the PDF document
                        document.Add(new Paragraph(strHTMLBuilder.ToString()));

                        //cLose the document
                        document.Close();
                        
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok("Sales reports generated successfully");
        }

        [HttpPost("Generate All Report")]
        public async Task<IActionResult> GenerateAllReportAsync()
        {
            int userID;
            try
            {
                #region I intended to develop an algorithm that collects all information from the database table to generate a report in pdf format.
                #endregion

                var thisUser = GetCurrentUser();
                var users = await _dbcontext.Users.ToListAsync();
                foreach (var user in users)
                {
                    if (user.userName == thisUser.userName)
                    {
                        userID = user.userId;
                        var customer = await _customerRep.GetByUserIdAsync(userID);
                        var customerID = customer.customerId;

                        var (IsSucess, mbokoDomain) = await _saleRep.GetSalesAsync();
                        if (!IsSucess)
                        {
                            return NotFound();
                        }
                        var mbokoDTO = _mapper.Map<IEnumerable<saleModel>>(mbokoDomain);

                        var thisTable = mbokoDTO;

                        string filePath = "./ReportTemplate/reports.html";
                        //string message = File().ReadAllText(filePath);
                        string receipthtml = System.IO.File.ReadAllText(filePath);

                        StringBuilder strHTMLBuilder = new StringBuilder(receipthtml);
                        strHTMLBuilder = strHTMLBuilder.Replace("{CustomerName}", $"{thisUser.firstName} {thisUser.lastName}");
                        strHTMLBuilder = strHTMLBuilder.Replace("{Table}", thisTable.ToString());
                        strHTMLBuilder = strHTMLBuilder.Replace("{userEmail}", thisUser.userName);
                        strHTMLBuilder = strHTMLBuilder.Replace("{Transdate}", DateTime.Now.ToString("yyyy-MM-dd"));

                        Document document = new Document(PageSize.A4, 25, 25, 25, 25);
                        PdfWriter.GetInstance(document, new FileStream("./GeneratedReports/" + thisUser.userId + "paymentsReport.pdf", FileMode.Create));

                        // Open the document
                        document.Open();

                        // Add the StringBuilder content to the PDF document
                        document.Add(new Paragraph(strHTMLBuilder.ToString()));

                        //cLose the document
                        document.Close();

                        //string pdfFilePath = "./GeneratedReports/" + thisUser.userId + "paymentsReport.pdf";
                        //string pdfText = GetPdfText(pdfFilePath);
                        //string plainText = ConvertHtmlToPlainText(pdfText);

                        //Document documentConvert = new Document(PageSize.A4, 25, 25, 25, 25);
                        //PdfWriter.GetInstance(documentConvert, new FileStream("./GeneratedReports/" + thisUser.userId + "ConvertedpaymentsReport.pdf", FileMode.Create));

                        //// Open the document
                        //documentConvert.Open();

                        //// Add the StringBuilder content to the PDF document
                        //documentConvert.Add(new Paragraph(plainText.ToString()));

                        ////cLose the document
                        //documentConvert.Close();
                    }
                }                
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok("Sales reports generated successfully");
        }

        private users GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new users
                {
                    userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    //userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    firstName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    lastName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    role = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };

            }
            return null;
        }

        private string GetPdfText(string pdfFilePath)
        {
            StringBuilder text = new StringBuilder();

            var reader = new PdfReader(pdfFilePath);
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, i, new LocationTextExtractionStrategy());
                    text.Append(pageText);
                }
                reader.Close();
            return text.ToString();
        }

        private string ConvertHtmlToPlainText(string html)
        {
            var document = new iTextSharp.text.Document();
            document.Open();
            var writer = new StringWriter();
            var htmlWorker = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

            htmlWorker.Parse(new StringReader(html));
            document.Close();

            return document.ToString();
        }

    }
}
