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
using System.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using static Org.Jsoup.Select.Evaluator;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Serilog.Formatting.Json;

namespace catalogueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrder orderRep;
        private readonly IUser userRep;
        private readonly IMapper _mapper;
        private readonly IFee _FeeRep;
        private readonly IConfiguration _configuration;
        private readonly ICustomer _customerRep;
        private readonly ILogger<OrdersController> _logger;
        private readonly ISqlprocess _databaseHandler;
        private readonly catalogueDBContext _dbcontext;
        private readonly IJsonFormatter _jsonFormatter;

        public OrdersController(IOrder orderRep, IMapper mapper, IUser userRep, IFee FeeRep, IConfiguration config,
        ICustomer customerRep, ILogger<OrdersController> logger, ISqlprocess databaseHandler, catalogueDBContext dbcontext, IJsonFormatter jsonFormatter)
        {
            this.orderRep = orderRep;
            this._mapper = mapper;
            this.userRep = userRep;
            this._FeeRep = FeeRep;
            this._configuration = config;
            this._customerRep = customerRep;
            this._logger = logger;
            this._databaseHandler = databaseHandler;
            this._dbcontext = dbcontext;
            this._jsonFormatter = jsonFormatter;
        }

        [HttpPost("Register Payment")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] orderRequest Mboko)
        {
            _logger.LogInformation($"Category Request collected to be added is {Mboko}");
            try
            {
                //Collect customer info from the database
                var customer = await _customerRep.GetByIdAsync(Mboko.customerId);
                if (customer == null)
                {
                    _logger.LogInformation($"No customer exists with the ID {Mboko.customerId}");
                    return Ok($"No such customer with ID: {Mboko.customerId}");
                }
                var customerFirstName = customer.firstName;
                var customerLastName = customer.lastName;
                var customerPhoneNumber = customer.phoneNumber;
                var customerUserID = customer.userId;

                //Collect user info from the database
                var user = await userRep.GetByIdAsync(customerUserID);
                if (user == null)
                {
                    _logger.LogInformation($"No user exists with the ID {customerUserID}");
                    return Ok($"No such user with ID: {customerUserID}");
                }

                //Check if this person exists, to carry on this order
                if (customerFirstName == user.firstName && customerLastName == user.lastName && customerPhoneNumber == user.phoneNumber)
                {
                    var FeeID = Mboko.FeeId;
                    var thisFee = await _FeeRep.GetByIdAsync(FeeID);
                    if (thisFee == null)
                    {
                        _logger.LogInformation($"No fees exist for feeID {FeeID}");
                        return Ok($"No such Fee with ID: {FeeID}");
                    }

                    //Get the total price
                    var FeeCost = (Mboko.quantity) * (thisFee.price);
                    var inputedCost = (Mboko.quantity) * (Mboko.amount);

                    //Validate if the amount inputed is enough to pay for this
                    if (inputedCost == FeeCost)
                    {
                        Mboko.amount = inputedCost;
                        var domainOrder = _mapper.Map<orders>(Mboko);

                        // Pass details to Repositpory
                        domainOrder = await orderRep.AddOrderAsync(domainOrder);
                        if (domainOrder == null)
                        {
                            return NotFound();
                        }

                        var orderStatus = "Not Paid";
                        _logger.LogInformation($"Current orderstatus for this Fee is {orderStatus}");

                        var orderID = domainOrder.orderID;
                        using var connection3 = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);

                        //First update the current status for this program payment
                        var query3 = "Update orders set orderStatus = @orderStatus where orderID = @orderID";
                        _logger.LogInformation($"SQL connection sucessfully opened to execute {query3}");
                        using var command3 = new SqlCommand(query3, connection3);

                        command3.Parameters.AddWithValue("@orderStatus", orderStatus);
                        command3.Parameters.AddWithValue("@orderID", orderID);

                        try
                        {
                            if (connection3.State != ConnectionState.Open)
                            {
                                await connection3.OpenAsync();
                            }
                            var rowsAffected = await command3.ExecuteNonQueryAsync();
                            await connection3.CloseAsync();
                            if (rowsAffected < 1) return Ok("Unable to execute sql command on updating order status");
                            _logger.LogInformation($"SQL Successfully with result affected: {rowsAffected}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.StackTrace, $"An error occured: {e}");
                            var errorMessage = e.Message;
                            return Ok(errorMessage);
                        }
                        finally
                        {
                            if (connection3.State != ConnectionState.Closed)
                            {
                                await connection3.CloseAsync();
                            }
                        }

                        // Convert back to DTO
                        var orderDTO = _mapper.Map<orderModel>(domainOrder);

                        _logger.LogInformation($"Order successfully placed with order ID of {orderDTO.orderID}");
                        return Ok(new Response { response = $"Order successfully placed with order ID of {orderDTO.orderID}" });
                    }
                    else
                    {
                        _logger.LogInformation($"Wrong Amount for the Fee '{thisFee.name}'");
                        return Ok($"Wrong Amount for the Fee '{thisFee.name}'");
                    }
                }
                else
                {
                    _logger.LogInformation($"{customer.customerId} is not correct for this user ID: {customerUserID}");
                    return Ok("Wrong customer ID, Kindly retry with your valid Customer Id");
                }                
            }
            catch (Exception oxg)
            {
                _logger.LogInformation(oxg.StackTrace, $"An error occurred: {oxg}");
                return BadRequest(oxg.Message);
            }
        }


        //View all program that users has shown interest for
        [HttpGet("View Registered Payments")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            //This is to ensure only the admin has access to all the list of intended payment
            var thisUser = GetCurrentUser();
            if (thisUser.role.ToString().ToUpper() == "SUPER ADMIN")
            {
                var (IsSucess, mbokoDomain) = await orderRep.GetOrdersAsync();
                if (!IsSucess)
                {
                    return NotFound();
                }
                var mbokoDTO = _mapper.Map<IEnumerable<orderModel>>(mbokoDomain);
                return Ok(mbokoDTO);
            }
            else
            {
                //Limit the list of intended payments only to ones done by this user, if they aren't an admin
                var customerID = (await _dbcontext.Customers.FirstOrDefaultAsync(x => x.userId == thisUser.userId)).customerId;
                
                //Get the records from the database using ADO.NET
                var query = "Select * from  orders where customerID = @customerID";

                SqlParameter[] Params = new SqlParameter[]
                {
                   new SqlParameter("@customerID",customerID)                                  
                };

                var ordersTable = await _databaseHandler.retrieveRecords(query, CommandType.Text, Params);
                if (!ordersTable.queryIsSuccessful)
                {
                    _logger.LogInformation($"An error occurres while retrieving order details with query: {query}");
                    return Ok("Unable to retrieve order details for customer");
                }
                _logger.LogInformation($"Order table successfully retrieved for query: {query}");

                var returnObject = await _jsonFormatter.JsonFormat(ordersTable.objectValue);

                if (returnObject == "" || returnObject == null)
                {
                    return Ok("There are no booked payments for this user");
                }

                return Ok(returnObject);

            }
        }

        [HttpGet("View Payments by ID")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var repoOrder = await orderRep.GetByIdAsync(id);
            if (repoOrder == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var orderDTO = _mapper.Map<orderModel>(repoOrder);

            return Ok(orderDTO);
        }
        

        [HttpPost("Checkout")]
        public async Task<IActionResult> PurchaseAsync(int orderID)
        {
            try
            {
                var domainOrder = await orderRep.PurchaseAsync(orderID);
                if (domainOrder != null)
                {
                    var orderAmount = domainOrder.amount;
                    var userId = GetCurrentUser().userId;
                    var thisUser = await userRep.GetByIdAsync(userId);
                    
                    //Check if this user exists at all
                    if (thisUser == null)
                    {
                        _logger.LogInformation($"No such user with ID: {userId}");
                        return Ok($"No such user with ID: {userId}");
                    }

                    //Check if the wallet is even funded for this transaction
                    if (thisUser.wallet == null)
                    {
                        _logger.LogInformation($"The userID {userId} Wallet balance is  empty for this transaction");
                        return Ok("Your wallet balance is empty, Kindly fund your wallet");
                    }
                    var availableBalanace = decimal.Parse(thisUser.wallet);

                    //Ensure this Program has not be paid for from this user
                    if (domainOrder.orderStatus.ToString().ToUpper() != "PAID")
                    {
                        //Check if the user has sufficient funds for this transaction
                        if (availableBalanace < orderAmount)
                        {
                            _logger.LogInformation($"The userID {userId} has Insufficient funds");
                            return Ok("Insufficient funds, kindly fund your wallet");
                        }
                        else
                        {
                            var FeeID = domainOrder.FeeId;
                            var thisFee = await _FeeRep.GetByIdAsync(FeeID);
                            var datePaid = DateTime.Now;
                            var salesType = thisFee.name;
                            var amount = domainOrder.amount;
                            var customerID = domainOrder.customerId;
                           
                            //Check if there are sufficient slots left for the User's demand
                            if (thisFee.quantity > domainOrder.quantity)
                            {
                                //Update the sales table, to reflect this product is bought
                                var query = "Insert Into sales(datePaid, salesType, amount, customerID) Values(@datePaid, @salesType, @amount, @customerID)";
                                
                                SqlParameter[] Params = new SqlParameter[]
                                {
                                  new SqlParameter("@datePaid",datePaid),
                                  new SqlParameter("@salesType",salesType),
                                  new SqlParameter("@amount",amount),
                                  new SqlParameter("@customerID",customerID)
                                };                                

                                var salesTableUpdate = await _databaseHandler.insert_Update(query, CommandType.Text, Params);
                                if (!salesTableUpdate.queryIsSuccessful)
                                {
                                    _logger.LogInformation($"An error occurres while updating Sales information for query: {query}");
                                    return Ok("Unable to execute sql command during sales table update");
                                }
                                _logger.LogInformation($"Sales table successfully updated for query: {query}");


                                //Deduct the cost of this transaction from the User's wallet
                                var wallet = (availableBalanace - orderAmount).ToString();

                                var query2 = "Update users set wallet = @wallet where userId = @userId";
                                SqlParameter[] Params1 = new SqlParameter[]
                                {
                                  new SqlParameter("@wallet",wallet),
                                  new SqlParameter("@userId",userId)                                  
                                };

                                var walletUpdate = await _databaseHandler.insert_Update(query2, CommandType.Text, Params1);
                                if (!walletUpdate.queryIsSuccessful)
                                {
                                    _logger.LogInformation($"An error occurres while updating customer's wallet for query: {query2}");
                                    return Ok("Unable to execute sql command on updating user's balance");
                                }
                                _logger.LogInformation($"Customer's wallet successfully updated for query: {query2}");                             

                                //Update the order status to convey this program has been successfully paid for by the User
                                var orderStatus = "Paid";
                                var query3 = "Update orders set orderStatus = @orderStatus where orderID = @orderID";

                                SqlParameter[] Params3 = new SqlParameter[]
                                {
                                  new SqlParameter("@orderStatus",orderStatus),
                                  new SqlParameter("@orderID",orderID)
                                };

                                var orderStatusUpdate = await _databaseHandler.insert_Update(query3, CommandType.Text, Params3);
                                if (!orderStatusUpdate.queryIsSuccessful)
                                {
                                    _logger.LogInformation($"An error occurred while updating order status for query: {query3}");
                                    return Ok("Unable to execute sql command on updating order status");
                                }
                                _logger.LogInformation($"Order status successfully updated for query: {query3}");                               

                                
                                //Update the slots of program that is left
                                var quantity = (thisFee.quantity) - (await orderRep.GetByIdAsync(orderID)).quantity;

                                var query4 = "Update Fees set quantity = @quantity where FeeID = @FeeID";                              

                                SqlParameter[] Params4 = new SqlParameter[]
                                {
                                  new SqlParameter("@quantity",quantity),
                                  new SqlParameter("@FeeID",FeeID)
                                };

                                var feeSlotUpdate = await _databaseHandler.insert_Update(query4, CommandType.Text, Params4);
                                if (!feeSlotUpdate.queryIsSuccessful)
                                {
                                    _logger.LogInformation($"An error occurred while updating slots left for query: {query4}");
                                    return Ok("Unable to execute sql command on updating fee slots left");
                                }
                                _logger.LogInformation($"Slots left successfully updated for query: {query4}");

                                //Generate the receipt for this transaction
                                _logger.LogInformation($"About generating reciept for customer: {customerID}");
                                string filePath = "./ReportTemplate/receipts.html";
                                string receipthtml = System.IO.File.ReadAllText(filePath);

                                //Stringbuilder to match datas to the file template
                                StringBuilder strHTMLBuilder = new StringBuilder(receipthtml);
                                strHTMLBuilder = strHTMLBuilder.Replace("{CustomerName}", $"{thisUser.firstName} {thisUser.lastName}");
                                strHTMLBuilder = strHTMLBuilder.Replace("{productName}", $"'{thisFee.name}'");
                                strHTMLBuilder = strHTMLBuilder.Replace("{TransDate}", domainOrder.dateReceived.ToString());
                                strHTMLBuilder = strHTMLBuilder.Replace("{TransRef}", $"{thisFee.FeeId}{thisUser.userId}LPT{thisFee.categoryId}");
                                strHTMLBuilder = strHTMLBuilder.Replace("{Amount}", (domainOrder.quantity * thisFee.price).ToString());
                                strHTMLBuilder = strHTMLBuilder.Replace("{price}", thisFee.price.ToString());
                                strHTMLBuilder = strHTMLBuilder.Replace("{OrderStatus}", "PAID");
                                strHTMLBuilder = strHTMLBuilder.Replace("{Quantity}", domainOrder.quantity.ToString());

                                Document document = new Document(PageSize.A4, 25, 25, 25, 25);
                                PdfWriter.GetInstance(document, new FileStream("./GeneratedReports/" + "00" + customerID + "paymentsReceipt.pdf", FileMode.Create));

                                // Open the document
                                document.Open();

                                // Add the StringBuilder content to the PDF document
                                document.Add(new Paragraph(strHTMLBuilder.ToString()));

                                //cLose the document
                                document.Close();

                                _logger.LogInformation($"Payment successfully made for the orderID: {orderID}");
                                return Ok("Payment Successful");
                            }
                            _logger.LogInformation($"User {thisUser.userName} with ID {thisUser.userId} was unable to pay due to limited slots");
                            return Ok($"Only {thisFee.quantity} slots are available for this Program");
                        }
                    }
                    _logger.LogInformation($"User {thisUser.userName} with ID {thisUser.userId} alraedy made payment");
                    return Ok("You've already paid for this Program, Kindly place a new payment");
                }
                _logger.LogInformation($"No existing order for the order ID: {orderID}");
                return Ok($"No existing order for the order ID: {orderID}");
            }
            catch (Exception oxg)
            {
                _logger.LogError(oxg.StackTrace, $"An error occured:\r\n{oxg}");
                return BadRequest(oxg.Message);
            }
        }

        //After showing interest for a program, Cancell your interest from here
        [HttpPost("Cancel Payment")]
        public async Task<IActionResult> CancelOrderAsync(int orderID)
        {
            _logger.LogInformation($"About Cancelling order for the orderID: {orderID}");
            using var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);

            var query = "delete from orders where orderID = @orderID";

            using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@orderID", orderID);        

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }
                var rowsAffected = await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                if (rowsAffected < 1) return Ok("Unable to execute sql command on deleting an order");
                //return rowsAffected;
            }
            catch (Exception e)
            {
                _logger.LogError(e.StackTrace, $"An error occured:\r\n{e}");
                var exceptionMessage = e.Message;
                return Ok(exceptionMessage);
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    await connection.CloseAsync();
                }
            }
            _logger.LogInformation($"Payment was successfully cancelled for orderID: {orderID}");
            return Ok("Payment cancelled successfully");
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
                    userId = Convert.ToInt16(userClaim.FirstOrDefault(o => o.Type == ClaimTypes.UserData)?.Value),
                    firstName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    lastName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    role = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };

            }
            return null;
        }
    }
}
