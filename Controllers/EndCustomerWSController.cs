using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace Overthink.PolitiScout.Controllers
{
    [Route("ws")]
    [ApiController]
    public class EndCustomerWSController : ControllerBase
    {

        private readonly ILogger<ResourceWSController> logger;
        private IConfiguration configuration;

        public EndCustomerWSController(IConfiguration Configuration, ILogger<ResourceWSController> logger)
        {

            this.configuration = Configuration;
            this.logger = logger;

        }

        [Route("customer/list/{salesLevel1}/{salesLevel2}/{salesLevel3}/{salesLevel4}/{salesLevel5}/{salesLevel6}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.EndCustomer>>> GetEndCustomersForSalesLevels(string salesLevel1, string salesLevel2, string salesLevel3, 
                                                                                                string salesLevel4, string salesLevel5, string salesLevel6)
        {

            int maxCustomersToDisplay = int.Parse(this.configuration["AppSettings:MaxCustomersToDisplay"]);
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommandGetEndCustomersForSalesLevels;
            SqlDataReader sqlDataReaderGetEndCustomersForSalesLevels;
            System.Text.StringBuilder sqlStatement;
            Dictionary<string, int> previouslyReferencedEndCustomer = new Dictionary<string, int>();

            try
            {

                List<Models.EndCustomer> returnValue = new List<Models.EndCustomer>();

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT DISTINCT EC.EndCustomerID, EC.EndCustomerHeadquartersName, EC.HistoricalBookingsAmount ");
                    sqlStatement.Append("  FROM EndCustomerView EC ");
                    sqlStatement.Append("  WHERE EC.TaggedSalesLevel1 = @SalesLevel1 AND EC.TaggedSalesLevel2 = @SalesLevel2 AND EC.TaggedSalesLevel3 = @SalesLevel3 AND ");
                    sqlStatement.Append("        EC.TaggedSalesLevel4 = @SalesLevel4 AND EC.TaggedSalesLevel5 LIKE @SalesLevel5 AND EC.TaggedSalesLevel6 LIKE @SalesLevel6 ");
                    sqlStatement.Append("  ORDER BY EC.EndCustomerHeadquartersName, EC.HistoricalBookingsAmount DESC ");

                    sqlCommandGetEndCustomersForSalesLevels = sqlConnection.CreateCommand();
                    sqlCommandGetEndCustomersForSalesLevels.CommandText = sqlStatement.ToString();
                    sqlCommandGetEndCustomersForSalesLevels.CommandTimeout = 20;
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel4", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel5", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetEndCustomersForSalesLevels.Parameters.Add(new SqlParameter("@SalesLevel6", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel3"].Value = "";
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel4"].Value = "";
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel5"].Value = "";
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel6"].Value = "";
                    await sqlCommandGetEndCustomersForSalesLevels.PrepareAsync();

                    // 1.)  Start with direct mapping at Sales Level 6
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel1"].Value = System.Web.HttpUtility.UrlDecode(salesLevel1);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel2"].Value = System.Web.HttpUtility.UrlDecode(salesLevel2);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel3"].Value = System.Web.HttpUtility.UrlDecode(salesLevel3);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel4"].Value = System.Web.HttpUtility.UrlDecode(salesLevel4);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel5"].Value = System.Web.HttpUtility.UrlDecode(salesLevel5);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel6"].Value = System.Web.HttpUtility.UrlDecode(salesLevel6);
                    sqlDataReaderGetEndCustomersForSalesLevels = await sqlCommandGetEndCustomersForSalesLevels.ExecuteReaderAsync();

                    while (await sqlDataReaderGetEndCustomersForSalesLevels.ReadAsync())
                    {
                        if (!previouslyReferencedEndCustomer.ContainsKey(sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME)))
                        {
                            Models.EndCustomer endCustomer = new Models.EndCustomer(sqlDataReaderGetEndCustomersForSalesLevels.GetInt32(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_ID),
                                                                                    sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME));
                            endCustomer.matchedAtSalesLevel = 6;

                            previouslyReferencedEndCustomer.Add(sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME), endCustomer.customerId);
                            returnValue.Add(endCustomer);
                        }
                    }       // while (await sqlCommandGetEndCustomersForSalesLevels.ReadAsync())
                    await sqlDataReaderGetEndCustomersForSalesLevels.CloseAsync();

                    // 1.)  Start with direct mapping at Sales Level 5
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel1"].Value = System.Web.HttpUtility.UrlDecode(salesLevel1);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel2"].Value = System.Web.HttpUtility.UrlDecode(salesLevel2);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel3"].Value = System.Web.HttpUtility.UrlDecode(salesLevel3);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel4"].Value = System.Web.HttpUtility.UrlDecode(salesLevel4);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel5"].Value = System.Web.HttpUtility.UrlDecode(salesLevel5);
                    sqlCommandGetEndCustomersForSalesLevels.Parameters["@SalesLevel6"].Value = ApplicationValues.SQL_WILDCARD_ALL;
                    sqlDataReaderGetEndCustomersForSalesLevels = await sqlCommandGetEndCustomersForSalesLevels.ExecuteReaderAsync();

                    while (await sqlDataReaderGetEndCustomersForSalesLevels.ReadAsync())
                    {
                        if (!previouslyReferencedEndCustomer.ContainsKey(sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME)))
                        {
                            Models.EndCustomer endCustomer = new Models.EndCustomer(sqlDataReaderGetEndCustomersForSalesLevels.GetInt32(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_ID),
                                                                                    sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME));
                            endCustomer.matchedAtSalesLevel = 5;
                            previouslyReferencedEndCustomer.Add(sqlDataReaderGetEndCustomersForSalesLevels.GetString(ApplicationValues.END_CUSTOMERS_FOR_SALES_LEVELS_LIST_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME), endCustomer.customerId);
                            returnValue.Add(endCustomer);
                        }
                    }       // while (await sqlCommandGetEndCustomersForSalesLevels.ReadAsync())
                    await sqlDataReaderGetEndCustomersForSalesLevels.CloseAsync();

                }       // using (sqlConnection = new SqlConnection(this._configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in EndCustomerController::GetEndCustomersForSalesLevels().  Message is {0}", ex1.Message));

                if (ex1.InnerException != null)
                {
                    logger.LogError(string.Format("  -- Inner exception message is {0}", ex1.InnerException.Message));

                    if (ex1.InnerException.InnerException != null)
                    {
                        logger.LogError(string.Format("  -- --  Inner exception message is {0}", ex1.InnerException.InnerException.Message));
                    }

                }

                logger.LogError(string.Format("{0}", ex1.StackTrace));

                return StatusCode(StatusCodes.Status500InternalServerError, ex1.Message);
            }

        }       // GetEndCustomersForSalesLevels()

        [Route("customer/searchNameMask/{nameMask}/{startOnly}/")]
        [HttpGet]
        public async Task<ActionResult<Models.EndCustomer>> GetEndCustomersByNameMask(string nameMask, int startOnly)
        {

            int maxCustomersToDisplay;
            SqlConnection sqlConnection = null;
            System.Text.StringBuilder sqlStatement;
            SqlCommand sqlCommandGetEndCustomersByNameMask;
            SqlDataReader sqlDataReaderGetEndCustomersByNameMask;
            List<Models.EndCustomer> returnValue;

            try
            {
                maxCustomersToDisplay = int.Parse(this.configuration["AppSettings:MaxCustomersToDisplay"]);

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))
                {

                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT DISTINCT TOP (@MaxCustomersToDisplay) EC.EndCustomerID, EC.EndCustomerHeadquartersName, EC.TaggedSalesLevel1, EC.TaggedSalesLevel2, ");
                    sqlStatement.Append("           EC.TaggedSalesLevel3, EC.TaggedSalesLevel4, EC.TaggedSalesLevel5, EC.TaggedSalesLevel6, EC.LatestBookingsDate, EC.HistoricalBookingsAmount ");

                    // RJB 2020-04-06  Handle conditions like GES where goaling does not travel down to Level 6
                    //sqlStatement.Append("  FROM EndCustomerView EC INNER JOIN PersonSalesLevelMappingEntry E ON EC.TaggedSalesLevel1 = E.SalesLevel1 AND ");
                    sqlStatement.Append("  FROM CorporateEndCustomerView EC LEFT OUTER JOIN PersonSalesLevelMappingEntryView E ON EC.TaggedSalesLevel1 = E.SalesLevel1 AND ");

                    sqlStatement.Append("       EC.TaggedSalesLevel2 = E.SalesLevel2 AND EC.TaggedSalesLevel3 = E.SalesLevel3 AND EC.TaggedSalesLevel4 = E.SalesLevel4 AND ");
                    sqlStatement.Append("       EC.TaggedSalesLevel5 = E.SalesLevel5 ");
                    sqlStatement.Append("  WHERE EC.EndCustomerHeadquartersName LIKE @NameMask ");
                    sqlStatement.Append("  ORDER BY EC.HistoricalBookingsAmount DESC, EC.EndCustomerHeadquartersName,  EC.EndCustomerID ");

                    sqlCommandGetEndCustomersByNameMask = sqlConnection.CreateCommand();
                    sqlCommandGetEndCustomersByNameMask.CommandText = sqlStatement.ToString();
                    sqlCommandGetEndCustomersByNameMask.CommandTimeout = 600;
                    sqlCommandGetEndCustomersByNameMask.Parameters.Add(new SqlParameter("@MaxCustomersToDisplay", System.Data.SqlDbType.Int));
                    sqlCommandGetEndCustomersByNameMask.Parameters.Add(new SqlParameter("@NameMask", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetEndCustomersByNameMask.Parameters["@MaxCustomersToDisplay"].Value = 0;
                    sqlCommandGetEndCustomersByNameMask.Parameters["@NameMask"].Value = "";
                    await sqlCommandGetEndCustomersByNameMask.PrepareAsync();

                    returnValue = new List<Models.EndCustomer>();

                    nameMask = nameMask + ApplicationValues.SQL_WILDCARD_ALL;

                    if (startOnly != 1)
                    {
                        nameMask = ApplicationValues.SQL_WILDCARD_ALL + nameMask;
                    }

                    sqlCommandGetEndCustomersByNameMask.Parameters["@MaxCustomersToDisplay"].Value = maxCustomersToDisplay;
                    sqlCommandGetEndCustomersByNameMask.Parameters["@NameMask"].Value = nameMask;
                    sqlDataReaderGetEndCustomersByNameMask = await sqlCommandGetEndCustomersByNameMask.ExecuteReaderAsync();

                    DateTime datLatestBookingDate = new DateTime(1990, 1, 1);
                    int index = 0;
                    int latestBookingDateIndex = 0;

                    while (await sqlDataReaderGetEndCustomersByNameMask.ReadAsync())
                    {

                        returnValue.Add(new Models.EndCustomer(sqlDataReaderGetEndCustomersByNameMask.GetInt32(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_CUSTOMER_ID),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_CUSTOMER_NAME),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_4),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_5),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetString(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_6),
                                                               sqlDataReaderGetEndCustomersByNameMask.GetDateTime(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_LATEST_BOOKINGS_DATE)));

                        if (sqlDataReaderGetEndCustomersByNameMask.GetDateTime(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_LATEST_BOOKINGS_DATE) > datLatestBookingDate)
                        {
                            latestBookingDateIndex = index;
                            datLatestBookingDate = sqlDataReaderGetEndCustomersByNameMask.GetDateTime(ApplicationValues.END_CUSTOMER_LIST_QUERY_RESULT_COLUMN_OFFSET_LATEST_BOOKINGS_DATE);
                        }

                        index++;
                    }       // while (await sqlCommandGetEndCustomersByNameMask.ReadAsync())
                    await sqlDataReaderGetEndCustomersByNameMask.CloseAsync();

                    if (datLatestBookingDate > new DateTime(1990, 1, 1))
                    {
                        returnValue[latestBookingDateIndex].displayFlags = ApplicationValues.DISPLAY_FLAG_LATEST_BOOKINGS_DATE;
                    }

                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))

                sqlConnection.Close();

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in CustomerController::GetEndCustomersByNameMask().  Message is {0}", ex1.Message));

                if (ex1.InnerException != null)
                {
                    logger.LogError(string.Format("  -- Inner exception message is {0}", ex1.InnerException.Message));

                    if (ex1.InnerException.InnerException != null)
                    {
                        logger.LogError(string.Format("  -- --  Inner exception message is {0}", ex1.InnerException.InnerException.Message));
                    }

                }

                logger.LogError(string.Format("{0}", ex1.StackTrace));

                return StatusCode(StatusCodes.Status500InternalServerError, ex1.Message);
            }

        }       // GetEndCustomersByNameMask()


        [Route("customer/{endCustomerId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.EndCustomer>> GetEndCustomerById(int endCustomerId)
        {

            SqlConnection sqlConnection = null;
            System.Text.StringBuilder sqlStatement;
            SqlCommand sqlCommandGetEndCustomerById = null;
            SqlDataReader sqlDataReaderGetEndCustomerById;
            Models.EndCustomer returnValue = null;

            try
            {

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT EC.EndCustomerID, EC.EndCustomerHeadquartersName, EC.TaggedSalesLevel1, EC.TaggedSalesLevel2, EC.TaggedSalesLevel3, EC.TaggedSalesLevel4, ");
                    sqlStatement.Append("       EC.TaggedSalesLevel5, EC.TaggedSalesLevel6, LatestBookingsDate ");
                    sqlStatement.Append("  FROM CorporateEndCustomerView EC ");
                    sqlStatement.Append("  WHERE EC.EndCustomerID = @EndCustomerID ");

                    sqlCommandGetEndCustomerById = sqlConnection.CreateCommand();
                    sqlCommandGetEndCustomerById.CommandText = sqlStatement.ToString();
                    sqlCommandGetEndCustomerById.CommandTimeout = 600;
                    sqlCommandGetEndCustomerById.Parameters.Add(new SqlParameter("@EndCustomerID", System.Data.SqlDbType.Int));

                    sqlCommandGetEndCustomerById.Parameters["@EndCustomerID"].Value = 0;
                    await sqlCommandGetEndCustomerById.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlCommandGetEndCustomerById.Parameters["@EndCustomerID"].Value = endCustomerId;
                    sqlDataReaderGetEndCustomerById = await sqlCommandGetEndCustomerById.ExecuteReaderAsync();

                    if (await sqlDataReaderGetEndCustomerById.ReadAsync())
                    {

                        returnValue = new Models.EndCustomer(sqlDataReaderGetEndCustomerById.GetInt32(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_ID),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_4),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_5),
                                                          sqlDataReaderGetEndCustomerById.GetString(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_6),
                                                          sqlDataReaderGetEndCustomerById.GetDateTime(ApplicationValues.END_CUSTOMER_QUERY_RESULT_COLUMN_OFFSET_LATEST_BOOKINGS_DATE));

                    }       // while (await sqlCommandGetEndCustomerById.ReadAsync())
                    await sqlDataReaderGetEndCustomerById.CloseAsync();

                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in CustomerController::GetEndCustomerById().  Message is {0}", ex1.Message));

                if (ex1.InnerException != null)
                {
                    logger.LogError(string.Format("  -- Inner exception message is {0}", ex1.InnerException.Message));

                    if (ex1.InnerException.InnerException != null)
                    {
                        logger.LogError(string.Format("  -- --  Inner exception message is {0}", ex1.InnerException.InnerException.Message));
                    }

                }

                logger.LogError(string.Format("{0}", ex1.StackTrace));

                return StatusCode(StatusCodes.Status500InternalServerError, ex1.Message);
            }

        }       // GetEndCustomerById()

    }
}