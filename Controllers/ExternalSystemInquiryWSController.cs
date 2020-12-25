using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Overthink.PolitiScout.Models;
using System.Text;

namespace Overthink.PolitiScout.Controllers
{
    [Route("ext-ws")]
    [ApiController]
    public class ExternalSystemInquiryWSController : ControllerBase
    {
        private readonly ILogger<ExternalSystemInquiryWSController> logger;
        private IConfiguration configuration;

        public ExternalSystemInquiryWSController(IConfiguration configuration, ILogger<ExternalSystemInquiryWSController> logger)
        {

            this.configuration = configuration;
            this.logger = logger;

        }

        [Route("masterCustomerEntry/endCustomerId/{endCustomerId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ESI_MasterCustomerEntry>> getMasterCustomerEntryByEndCustomerId(int endCustomerId)
        {

            System.Text.StringBuilder sqlStatement;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommandGetSubordinateCustomerEntryByEndCustomerId;
            SqlDataReader sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId;
            SqlCommand sqlCommandGetMasterCustomerEntry;
            SqlDataReader sqlDataReaderGetMasterCustomerEntry;
            SqlCommand sqlCommandGetSubordinateCustomers;
            SqlDataReader sqlDataReaderGetSubordinateCustomers;
            SqlCommand sqlCommandExternalSystemCustomerReferenceEntries;
            SqlDataReader sqlDataReaderGetExternalSystemCustomerReferenceEntries;
            SqlCommand sqlCommandGetCLCBalanceEntries;
            SqlDataReader sqlDataReaderGetCLCBalanceEntries;
            SqlCommand sqlCommandGetPreSalesBDM;
            SqlDataReader sqlDataReaderGetPreSalesBDM;
            SqlCommand sqlCommandGetPostSalesBDM;
            SqlDataReader sqlDataReaderGetPostSalesBDM;

            int daysPastAllowedForExpiration = int.Parse(configuration["AppSettings:DaysPastAllowedForExpiration"]);

            Models.ESI_MasterCustomerEntry returnValue = null;

            try
            {

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))
                {

                    returnValue = new Models.ESI_MasterCustomerEntry();

                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT MCE2.EntryID ");
                    sqlStatement.Append("   FROM MasterCustomerEntry MCE INNER JOIN SubordinateCustomerEntry SCE ON MCE.ExternalSystemCustomerName = SCE.SubordinateCustomerName ");
                    sqlStatement.Append("        INNER JOIN MasterCustomerEntry MCE2 ON SCE.MasterCustomerEntryID = MCE2.EntryID ");
                    sqlStatement.Append("  WHERE MCE.EndCustomerID = @EndCustomerID AND MCE.ProcessingStatus = '");
                    sqlStatement.Append(ApplicationValues.MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_MOVED_TO_SUBORDINATE_TABLE +  "' ");

                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId = sqlConnection.CreateCommand();
                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId.CommandText = sqlStatement.ToString();
                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId.CommandTimeout = 600;
                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId.Parameters.Add(new SqlParameter("@EndCustomerID", System.Data.SqlDbType.Int));

                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId.Parameters["@EndCustomerID"].Value = 0;
                    await sqlCommandGetSubordinateCustomerEntryByEndCustomerId.PrepareAsync();

                    //

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT MCE.EntryID, MCE.EndCustomerID, MCE.EndCustomerHeadquartersName, MCE.SalesLevel1, ");
                    sqlStatement.Append("       MCE.SalesLevel2, MCE.SalesLevel3, MCE.SalesLevel4, MCE.AccountManagerCEC_ID, MCE.AccountManagerName, ");
                    sqlStatement.Append("       MCE.AccountManagerTelephoneNumber, MCE.PreferredLearningPartnerName, MCE.ExpertCareCustomer, MCE.VirtualTeamHandled ");
                    sqlStatement.Append("   FROM MasterCustomerEntry MCE LEFT OUTER JOIN EmployeeView EV ON MCE.AccountManagerCEC_ID = EV.CEC_ID ");
                    sqlStatement.Append("  WHERE MCE.EndCustomerID = @EndCustomerID ");

                    sqlCommandGetMasterCustomerEntry = sqlConnection.CreateCommand();
                    sqlCommandGetMasterCustomerEntry.CommandText = sqlStatement.ToString();
                    sqlCommandGetMasterCustomerEntry.CommandTimeout = 600;
                    sqlCommandGetMasterCustomerEntry.Parameters.Add(new SqlParameter("@EndCustomerID", System.Data.SqlDbType.Int));

                    sqlCommandGetMasterCustomerEntry.Parameters["@EndCustomerID"].Value = 0;
                    await sqlCommandGetMasterCustomerEntry.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT SCE.SubordinateCustomerName ");
                    sqlStatement.Append("       FROM SubordinateCustomerEntry SCE ");
                    sqlStatement.Append("  WHERE SCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY SCE.SubordinateCustomerName ");

                    sqlCommandGetSubordinateCustomers = sqlConnection.CreateCommand();
                    sqlCommandGetSubordinateCustomers.CommandText = sqlStatement.ToString();
                    sqlCommandGetSubordinateCustomers.CommandTimeout = 600;
                    sqlCommandGetSubordinateCustomers.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT XRCE.ExternalSystemCustomerID, ExternalSystemCustomerName ");
                    sqlStatement.Append("       FROM ExternalSystemCustomerReferenceEntry XRCE ");
                    sqlStatement.Append("  WHERE XRCE.ExternalSystemID = @ExternalSystemID AND XRCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY XRCE.ExternalSystemID ");

                    sqlCommandExternalSystemCustomerReferenceEntries = sqlConnection.CreateCommand();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandTimeout = 600;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@ExternalSystemID", System.Data.SqlDbType.Int));
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = 0;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT CBE.SalesOrderNumber, CBE.TeamCaptainName, CBE.TeamCaptainEMailAddress, CBE.TeamCaptainTelephoneNumber, ");
                    sqlStatement.Append("       CBE.TeamCaptainOtherName, CBE.TeamCaptainOtherEMailAddress, CBE.TeamCaptainOtherTelephoneNumber, ");
                    sqlStatement.Append("       CBE.CreditsPurchased, CBE.CurrentBalance, CBE.ActivationDate, CBE.ExpirationDate, CBE.TCAcceptance, ");
                    sqlStatement.Append("       CBE.RevenueAlreadyRecognized, MCE.ExternalSystemCustomerName, CBE.ServiceBookingsNet, CBE.ServiceBookingsList ");
                    sqlStatement.Append("  FROM CLCBalanceEntry CBE LEFT OUTER JOIN MasterCustomerEntry MCE ON CBE.OriginatingMasterCustomerEntryID = MCE.EntryID ");
                    sqlStatement.Append("  WHERE CBE.MasterCustomerEntryID = @MasterCustomerEntryID AND CBE.ExpirationDate >= @ExpirationDate ");
                    sqlStatement.Append("  ORDER BY CBE.ActivationDate, CBE.SalesOrderNumber ");

                    sqlCommandGetCLCBalanceEntries = sqlConnection.CreateCommand();
                    sqlCommandGetCLCBalanceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandGetCLCBalanceEntries.CommandTimeout = 600;
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@ExpirationDate", System.Data.SqlDbType.DateTime));

                    sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = DateTime.MinValue;
                    await sqlCommandGetCLCBalanceEntries.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PreSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PreSalesBDMSalesLevelGroupingView GME ON GMEV.PreSalesBDMSalesLevelGroupingID = GME.PreSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PreSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPreSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPreSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPreSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPreSalesBDM.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PostSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PostSalesBDMSalesLevelGroupingView GME ON GMEV.PostSalesBDMSalesLevelGroupingID = GME.PostSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PostSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPostSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPostSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPostSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPostSalesBDM.PrepareAsync();



                    // PROGRAMMER'S NOTE:  Per request from B. Robinson if passing in an End Customer ID for a prior Master Customer Entry that has been subordinated,
                    //                     return the new Master Customer Entry and BDM dashboard will use the condition of the returned End Customer ID differning from the
                    //                     passed-in value as a trigger to indicate said subordination and update values on the BDM system accordingly.

                    sqlCommandGetSubordinateCustomerEntryByEndCustomerId.Parameters["@EndCustomerID"].Value = 0;
                    sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId = await sqlCommandGetSubordinateCustomerEntryByEndCustomerId.ExecuteReaderAsync();
                    if (sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId.Read())
                    {
                        endCustomerId = sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId.GetInt32(0);
                    }       // (sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId.Read())
                    sqlDataReaderGetSubordinateCustomerEntryByEndCustomerId.Close();

                    sqlCommandGetMasterCustomerEntry.Parameters["@EndCustomerID"].Value = endCustomerId;
                    sqlDataReaderGetMasterCustomerEntry = await sqlCommandGetMasterCustomerEntry.ExecuteReaderAsync();

                    if (await sqlDataReaderGetMasterCustomerEntry.ReadAsync())
                    {

                        returnValue.masterCustomerEntryId = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        returnValue.endCustomerId = endCustomerId;
                        returnValue.endCustomerHeadquartersName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME);
                        returnValue.salesLevel1 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        returnValue.salesLevel2 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        returnValue.salesLevel3 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);
                        returnValue.salesLevel4 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_4);

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_CEC_ID))
                        {
                            returnValue.accountManagerCEC_ID = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_CEC_ID);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME))
                        {
                            returnValue.accountManagerName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_TELEPHONE_NUMBER))
                        {
                            returnValue.accountManagerTelephoneNumber = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_TELEPHONE_NUMBER);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME))
                        {
                            returnValue.preferredLearningPartnerName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER))
                        {
                            returnValue.expertCareCustomer = sqlDataReaderGetMasterCustomerEntry.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER);
                        }
                        else
                        {
                            returnValue.expertCareCustomer = false;
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED))
                        {
                            returnValue.virtualTeamHandled = sqlDataReaderGetMasterCustomerEntry.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED);
                        }
                        else
                        {
                            returnValue.virtualTeamHandled = false;
                        }

                        // Get Pre-Sales BDM
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPreSalesBDM = await sqlCommandGetPreSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPreSalesBDM.ReadAsync())
                        {
                            returnValue.preSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            returnValue.preSalesBusinessDevelopmentManagerName = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPreSalesBDM.ReadAsync()
                        sqlDataReaderGetPreSalesBDM.Close();

                        // Get Post-Sales BDM
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPostSalesBDM = await sqlCommandGetPostSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPostSalesBDM.ReadAsync())
                        {
                            returnValue.postSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            returnValue.postSalesBusinessDevelopmentManagerName = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPostSalesBDM.ReadAsync()
                        sqlDataReaderGetPostSalesBDM.Close();

                        sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetSubordinateCustomers = await sqlCommandGetSubordinateCustomers.ExecuteReaderAsync();

                        while (await sqlDataReaderGetSubordinateCustomers.ReadAsync())
                        {
                            returnValue.subordinateCustomerNames.Add(sqlDataReaderGetSubordinateCustomers.GetString(ApplicationValues.ESI_SUBORDINATE_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SUBORDINATE_CUSTOMER_NAME));
                        }
                        await sqlDataReaderGetSubordinateCustomers.CloseAsync();

                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = ApplicationValues.EXTERNAL_SYSTEM_ID_BUSINESS_DEVELOPMENT_MANAGER_DASHBOARD;
                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetExternalSystemCustomerReferenceEntries = await sqlCommandExternalSystemCustomerReferenceEntries.ExecuteReaderAsync();
                        while (await sqlDataReaderGetExternalSystemCustomerReferenceEntries.ReadAsync())
                        {
                            returnValue.subordinateCustomerExternalSystemIDs.Add(sqlDataReaderGetExternalSystemCustomerReferenceEntries.GetInt32(ApplicationValues.ESI_EXTERNAL_SYSTEM_CUSTOMER_REFERENCE_ENTRIES_LIST_QUERY_RESULT_COLUMN_OFFSET_EXTERNAL_SYSTEM_CUSTOMER_ID));
                        }
                        await sqlDataReaderGetExternalSystemCustomerReferenceEntries.CloseAsync();

                        sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = System.DateTime.Now.Date.AddDays(-1 * daysPastAllowedForExpiration);
                        sqlDataReaderGetCLCBalanceEntries = await sqlCommandGetCLCBalanceEntries.ExecuteReaderAsync();

                        while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())
                        {

                            ESI_CLCBalanceEntry clcBalanceEntry = new ESI_CLCBalanceEntry();

                            clcBalanceEntry.salesOrderNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_ORDER_NUMBER);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME))
                            {
                                clcBalanceEntry.teamCaptainName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME))
                            {
                                clcBalanceEntry.teamCaptainOtherName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainOtherEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainOtherTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER);
                            }

                            clcBalanceEntry.creditsPurchased = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CREDITS_PURCHASED);
                            clcBalanceEntry.currentBalance = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CURRENT_BALANCE);
                            clcBalanceEntry.activationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACTIVATION_DATE);
                            clcBalanceEntry.expirationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPIRATION_DATE);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE))
                            {
                                clcBalanceEntry.termsConditionsAcceptance = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE);
                            }
                            else
                            {
                                clcBalanceEntry.termsConditionsAcceptance = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED))
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED);
                            }
                            else
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME))
                            {
                                clcBalanceEntry.lcmtCustomerName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME);
                            }

                            clcBalanceEntry.serviceBookingsNet = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_NET);
                            clcBalanceEntry.serviceBookingsList = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_LIST);

                            returnValue.clcBalanceEntries.Add(clcBalanceEntry);

                        }       // while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())

                    }       // while (await sqlDataReaderGetMasterCustomerEntry.ReadAsync())
                    await sqlDataReaderGetMasterCustomerEntry.CloseAsync();

                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))

                sqlConnection.Close();

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in ExternalSystemInquiryWSController::getMasterCustomerEntryByEndCustomerId().  Message is {0}", ex1.Message));

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

        }       // getMasterCustomerEntryByEndCustomerId()

        [Route("masterCustomerEntry/externalSystemCustomerName/{externalSystemCustomerName}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ESI_MasterCustomerEntry>> getMasterCustomerEntryByExternalSystemCustomerName(string externalSystemCustomerName)
        {

            System.Text.StringBuilder sqlStatement;
            int entryId;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName;
            SqlDataReader sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName;
            SqlCommand sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName;
            SqlDataReader sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName;
            SqlCommand sqlCommandGetMasterCustomerEntry;
            SqlDataReader sqlDataReaderGetMasterCustomerEntry;
            SqlCommand sqlCommandGetSubordinateCustomers;
            SqlDataReader sqlDataReaderGetSubordinateCustomers;
            SqlCommand sqlCommandExternalSystemCustomerReferenceEntries;
            SqlDataReader sqlDataReaderGetExternalSystemCustomerReferenceEntries;
            SqlCommand sqlCommandGetCLCBalanceEntries;
            SqlDataReader sqlDataReaderGetCLCBalanceEntries;
            SqlCommand sqlCommandGetPreSalesBDM;
            SqlDataReader sqlDataReaderGetPreSalesBDM;
            SqlCommand sqlCommandGetPostSalesBDM;
            SqlDataReader sqlDataReaderGetPostSalesBDM;

            byte[] data = Convert.FromBase64String(externalSystemCustomerName);
            externalSystemCustomerName = Encoding.UTF8.GetString(data);


            int daysPastAllowedForExpiration = int.Parse(configuration["AppSettings:DaysPastAllowedForExpiration"]);

            Models.ESI_MasterCustomerEntry returnValue = null;

            try
            {

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))
                {

                    returnValue = new Models.ESI_MasterCustomerEntry();

                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT MCE2.EntryID ");
                    sqlStatement.Append("   FROM MasterCustomerEntry MCE INNER JOIN SubordinateCustomerEntry SCE ON MCE.ExternalSystemCustomerName = SCE.SubordinateCustomerName ");
                    sqlStatement.Append("        INNER JOIN MasterCustomerEntry MCE2 ON SCE.MasterCustomerEntryID = MCE2.EntryID ");
                    sqlStatement.Append("  WHERE MCE.ExternalSystemCustomerName = @ExternalSystemCustomerName AND MCE.ProcessingStatus = '");
                    sqlStatement.Append(ApplicationValues.MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_MOVED_TO_SUBORDINATE_TABLE + "' ");

                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName = sqlConnection.CreateCommand();
                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.CommandText = sqlStatement.ToString();
                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.CommandTimeout = 600;
                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.Parameters.Add(new SqlParameter("@ExternalSystemCustomerName", System.Data.SqlDbType.VarChar, 255));

                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.Parameters["@ExternalSystemCustomerName"].Value = "";
                    await sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT MCE.EntryID ");
                    sqlStatement.Append("   FROM MasterCustomerEntry MCE  ");
                    sqlStatement.Append("  WHERE MCE.ExternalSystemCustomerName = @ExternalSystemCustomerName AND MCE.ProcessingStatus = '");
                    sqlStatement.Append(ApplicationValues.MASTER_CUSTOMER_ENTRY_PROCESSING_STATUS_PROCESSED + "' ");

                    sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName = sqlConnection.CreateCommand();
                    sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.CommandText = sqlStatement.ToString();
                    sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.CommandTimeout = 600;
                    sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.Parameters.Add(new SqlParameter("@ExternalSystemCustomerName", System.Data.SqlDbType.VarChar, 255));

                    sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.Parameters["@ExternalSystemCustomerName"].Value = "";
                    await sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT MCE.EntryID, MCE.EndCustomerID, MCE.EndCustomerHeadquartersName, MCE.SalesLevel1, ");
                    sqlStatement.Append("       MCE.SalesLevel2, MCE.SalesLevel3, MCE.SalesLevel4, MCE.AccountManagerCEC_ID, MCE.AccountManagerName, ");
                    sqlStatement.Append("       MCE.AccountManagerTelephoneNumber, MCE.PreferredLearningPartnerName, MCE.ExpertCareCustomer, MCE.VirtualTeamHandled ");
                    sqlStatement.Append("   FROM MasterCustomerEntry MCE LEFT OUTER JOIN EmployeeView EV ON MCE.AccountManagerCEC_ID = EV.CEC_ID ");
                    sqlStatement.Append("  WHERE MCE.EntryID = @EntryID ");

                    sqlCommandGetMasterCustomerEntry = sqlConnection.CreateCommand();
                    sqlCommandGetMasterCustomerEntry.CommandText = sqlStatement.ToString();
                    sqlCommandGetMasterCustomerEntry.CommandTimeout = 600;
                    sqlCommandGetMasterCustomerEntry.Parameters.Add(new SqlParameter("@EntryID", System.Data.SqlDbType.Int));

                    sqlCommandGetMasterCustomerEntry.Parameters["@EntryID"].Value = 0;
                    await sqlCommandGetMasterCustomerEntry.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT SCE.SubordinateCustomerName ");
                    sqlStatement.Append("       FROM SubordinateCustomerEntry SCE ");
                    sqlStatement.Append("  WHERE SCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY SCE.SubordinateCustomerName ");

                    sqlCommandGetSubordinateCustomers = sqlConnection.CreateCommand();
                    sqlCommandGetSubordinateCustomers.CommandText = sqlStatement.ToString();
                    sqlCommandGetSubordinateCustomers.CommandTimeout = 600;
                    sqlCommandGetSubordinateCustomers.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT XRCE.ExternalSystemCustomerID, ExternalSystemCustomerName ");
                    sqlStatement.Append("       FROM ExternalSystemCustomerReferenceEntry XRCE ");
                    sqlStatement.Append("  WHERE XRCE.ExternalSystemID = @ExternalSystemID AND XRCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY XRCE.ExternalSystemID ");

                    sqlCommandExternalSystemCustomerReferenceEntries = sqlConnection.CreateCommand();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandTimeout = 600;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@ExternalSystemID", System.Data.SqlDbType.Int));
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = 0;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT CBE.SalesOrderNumber, CBE.TeamCaptainName, CBE.TeamCaptainEMailAddress, CBE.TeamCaptainTelephoneNumber, ");
                    sqlStatement.Append("       CBE.TeamCaptainOtherName, CBE.TeamCaptainOtherEMailAddress, CBE.TeamCaptainOtherTelephoneNumber, ");
                    sqlStatement.Append("       CBE.CreditsPurchased, CBE.CurrentBalance, CBE.ActivationDate, CBE.ExpirationDate, CBE.TCAcceptance, ");
                    sqlStatement.Append("       CBE.RevenueAlreadyRecognized, MCE.ExternalSystemCustomerName, CBE.ServiceBookingsNet, CBE.ServiceBookingsList ");
                    sqlStatement.Append("  FROM CLCBalanceEntry CBE LEFT OUTER JOIN MasterCustomerEntry MCE ON CBE.OriginatingMasterCustomerEntryID = MCE.EntryID ");
                    sqlStatement.Append("  WHERE CBE.MasterCustomerEntryID = @MasterCustomerEntryID AND CBE.ExpirationDate >= @ExpirationDate ");
                    sqlStatement.Append("  ORDER BY CBE.ActivationDate, CBE.SalesOrderNumber ");

                    sqlCommandGetCLCBalanceEntries = sqlConnection.CreateCommand();
                    sqlCommandGetCLCBalanceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandGetCLCBalanceEntries.CommandTimeout = 600;
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@ExpirationDate", System.Data.SqlDbType.DateTime));

                    sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = DateTime.MinValue;
                    await sqlCommandGetCLCBalanceEntries.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PreSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PreSalesBDMSalesLevelGroupingView GME ON GMEV.PreSalesBDMSalesLevelGroupingID = GME.PreSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PreSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPreSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPreSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPreSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPreSalesBDM.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PostSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PostSalesBDMSalesLevelGroupingView GME ON GMEV.PostSalesBDMSalesLevelGroupingID = GME.PostSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PostSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPostSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPostSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPostSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPostSalesBDM.PrepareAsync();

                    entryId = -1;

                    // Check for subordinated customer name first
                    sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.Parameters["@ExternalSystemCustomerName"].Value = externalSystemCustomerName;
                    sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName = await sqlCommandGetSubordinateCustomerEntryByExternalSystemCustomerName.ExecuteReaderAsync();
                    if (sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName.Read())
                    {
                        entryId = sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName.GetInt32(0);
                    } 
                    else
                    {

                        sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.Parameters["@ExternalSystemCustomerName"].Value = externalSystemCustomerName;
                        sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName = await sqlCommandGetMasterCustomerEntryByExternalSystemCustomerName.ExecuteReaderAsync();
                        if (sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName.Read())
                        {
                            entryId = sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName.GetInt32(0);
                        }       // (sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName.Read())
                        sqlDataReaderGetMasterCustomerEntryByExternalSystemCustomerName.Close();

                    }       // (sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName.Read())
                    sqlDataReaderGetSubordinateCustomerEntryByExternalSystemCustomerName.Close();

                    sqlCommandGetMasterCustomerEntry.Parameters["@EntryID"].Value = entryId;
                    sqlDataReaderGetMasterCustomerEntry = await sqlCommandGetMasterCustomerEntry.ExecuteReaderAsync();

                    if (await sqlDataReaderGetMasterCustomerEntry.ReadAsync())
                    {

                        returnValue.masterCustomerEntryId = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        returnValue.endCustomerId = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_ID);
                        returnValue.endCustomerHeadquartersName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME);
                        returnValue.salesLevel1 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        returnValue.salesLevel2 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        returnValue.salesLevel3 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);
                        returnValue.salesLevel4 = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_4);

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_CEC_ID))
                        {
                            returnValue.accountManagerCEC_ID = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_CEC_ID);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME))
                        {
                            returnValue.accountManagerName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_TELEPHONE_NUMBER))
                        {
                            returnValue.accountManagerTelephoneNumber = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_TELEPHONE_NUMBER);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME))
                        {
                            returnValue.preferredLearningPartnerName = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER))
                        {
                            returnValue.expertCareCustomer = sqlDataReaderGetMasterCustomerEntry.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER);
                        }
                        else
                        {
                            returnValue.expertCareCustomer = false;
                        }

                        if (!sqlDataReaderGetMasterCustomerEntry.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED))
                        {
                            returnValue.virtualTeamHandled = sqlDataReaderGetMasterCustomerEntry.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED);
                        }
                        else
                        {
                            returnValue.virtualTeamHandled = false;
                        }

                        // Get Pre-Sales BDM
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPreSalesBDM = await sqlCommandGetPreSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPreSalesBDM.ReadAsync())
                        {
                            returnValue.preSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            returnValue.preSalesBusinessDevelopmentManagerName = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPreSalesBDM.ReadAsync()
                        sqlDataReaderGetPreSalesBDM.Close();

                        // Get Post-Sales BDM
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntry.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPostSalesBDM = await sqlCommandGetPostSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPostSalesBDM.ReadAsync())
                        {
                            returnValue.postSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            returnValue.postSalesBusinessDevelopmentManagerName = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPostSalesBDM.ReadAsync()
                        sqlDataReaderGetPostSalesBDM.Close();

                        sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetSubordinateCustomers = await sqlCommandGetSubordinateCustomers.ExecuteReaderAsync();

                        while (await sqlDataReaderGetSubordinateCustomers.ReadAsync())
                        {
                            returnValue.subordinateCustomerNames.Add(sqlDataReaderGetSubordinateCustomers.GetString(ApplicationValues.ESI_SUBORDINATE_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SUBORDINATE_CUSTOMER_NAME));
                        }
                        await sqlDataReaderGetSubordinateCustomers.CloseAsync();

                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = ApplicationValues.EXTERNAL_SYSTEM_ID_BUSINESS_DEVELOPMENT_MANAGER_DASHBOARD;
                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetExternalSystemCustomerReferenceEntries = await sqlCommandExternalSystemCustomerReferenceEntries.ExecuteReaderAsync();
                        while (await sqlDataReaderGetExternalSystemCustomerReferenceEntries.ReadAsync())
                        {
                            returnValue.subordinateCustomerExternalSystemIDs.Add(sqlDataReaderGetExternalSystemCustomerReferenceEntries.GetInt32(ApplicationValues.ESI_EXTERNAL_SYSTEM_CUSTOMER_REFERENCE_ENTRIES_LIST_QUERY_RESULT_COLUMN_OFFSET_EXTERNAL_SYSTEM_CUSTOMER_ID));
                        }
                        await sqlDataReaderGetExternalSystemCustomerReferenceEntries.CloseAsync();

                        sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntry.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = System.DateTime.Now.Date.AddDays(-1 * daysPastAllowedForExpiration);
                        sqlDataReaderGetCLCBalanceEntries = await sqlCommandGetCLCBalanceEntries.ExecuteReaderAsync();

                        while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())
                        {

                            ESI_CLCBalanceEntry clcBalanceEntry = new ESI_CLCBalanceEntry();

                            clcBalanceEntry.salesOrderNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_ORDER_NUMBER);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME))
                            {
                                clcBalanceEntry.teamCaptainName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME))
                            {
                                clcBalanceEntry.teamCaptainOtherName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainOtherEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainOtherTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER);
                            }

                            clcBalanceEntry.creditsPurchased = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CREDITS_PURCHASED);
                            clcBalanceEntry.currentBalance = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CURRENT_BALANCE);
                            clcBalanceEntry.activationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACTIVATION_DATE);
                            clcBalanceEntry.expirationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPIRATION_DATE);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE))
                            {
                                clcBalanceEntry.termsConditionsAcceptance = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE);
                            }
                            else
                            {
                                clcBalanceEntry.termsConditionsAcceptance = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED))
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED);
                            }
                            else
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME))
                            {
                                clcBalanceEntry.lcmtCustomerName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME);
                            }

                            clcBalanceEntry.serviceBookingsNet = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_NET);
                            clcBalanceEntry.serviceBookingsList = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_LIST);

                            returnValue.clcBalanceEntries.Add(clcBalanceEntry);

                        }       // while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())

                    }       // while (await sqlDataReaderGetMasterCustomerEntry.ReadAsync())
                    await sqlDataReaderGetMasterCustomerEntry.CloseAsync();

                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))

                sqlConnection.Close();

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in ExternalSystemInquiryWSController::getMasterCustomerEntryByExternalSystemCustomerName().  Message is {0}", ex1.Message));

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

        }       // getMasterCustomerEntryByExternalSystemCustomerName()



        [Route("masterCustomerEntries/{endCustomerIdGreaterThan}/{numberOfRecordsToReturn}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.ESI_MasterCustomerEntry>>> getMasterCustomers(int endCustomerIdGreaterThan, int numberOfRecordsToReturn)
        {

            System.Text.StringBuilder sqlStatement;
            SqlConnection sqlConnection = null;
            SqlCommand sqlCommandGetMasterCustomerEntries = null;
            SqlDataReader sqlDataReaderGetMasterCustomerEntries = null;
            SqlCommand sqlCommandGetSubordinateCustomers = null;
            SqlDataReader sqlDataReaderGetSubordinateCustomers;
            SqlCommand sqlCommandExternalSystemCustomerReferenceEntries = null;
            SqlDataReader sqlDataReaderGetExternalSystemCustomerReferenceEntries;
            SqlCommand sqlCommandGetCLCBalanceEntries = null;
            SqlDataReader sqlDataReaderGetCLCBalanceEntries;
            SqlCommand sqlCommandGetPreSalesBDM = null;
            SqlDataReader sqlDataReaderGetPreSalesBDM;
            SqlCommand sqlCommandGetPostSalesBDM = null;
            SqlDataReader sqlDataReaderGetPostSalesBDM;

            int daysPastAllowedForExpiration = int.Parse(configuration["AppSettings:DaysPastAllowedForExpiration"]);

            List<Models.ESI_MasterCustomerEntry> returnValue = new List<ESI_MasterCustomerEntry>();

            try
            {

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))
                {

                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT TOP (@NumberOfRecordsToReturn) MCE.EntryID, MCE.EndCustomerID, MCE.EndCustomerHeadquartersName, MCE.SalesLevel1, ");
                    sqlStatement.Append("           MCE.SalesLevel2, MCE.SalesLevel3, MCE.SalesLevel4, MCE.AccountManagerCEC_ID, MCE.AccountManagerName, ");
                    sqlStatement.Append("           MCE.AccountManagerTelephoneNumber, MCE.PreferredLearningPartnerName, MCE.ExpertCareCustomer, MCE.VirtualTeamHandled ");
                    sqlStatement.Append("       FROM MasterCustomerEntry MCE LEFT OUTER JOIN EmployeeView EV ON MCE.AccountManagerCEC_ID = EV.CEC_ID ");
                    sqlStatement.Append("  WHERE MCE.EndCustomerID > @EndCustomerID ");
                    sqlStatement.Append("  ORDER BY MCE.EndCustomerID ");

                    sqlCommandGetMasterCustomerEntries = sqlConnection.CreateCommand();
                    sqlCommandGetMasterCustomerEntries.CommandText = sqlStatement.ToString();
                    sqlCommandGetMasterCustomerEntries.CommandTimeout = 600;
                    sqlCommandGetMasterCustomerEntries.Parameters.Add(new SqlParameter("@EndCustomerID", System.Data.SqlDbType.Int));
                    sqlCommandGetMasterCustomerEntries.Parameters.Add(new SqlParameter("@NumberOfRecordsToReturn", System.Data.SqlDbType.Int));

                    sqlCommandGetMasterCustomerEntries.Parameters["@EndCustomerID"].Value = 0;
                    sqlCommandGetMasterCustomerEntries.Parameters["@NumberOfRecordsToReturn"].Value = 0;
                    await sqlCommandGetMasterCustomerEntries.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT SCE.SubordinateCustomerName ");
                    sqlStatement.Append("       FROM SubordinateCustomerEntry SCE ");
                    sqlStatement.Append("  WHERE SCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY SCE.SubordinateCustomerName ");

                    sqlCommandGetSubordinateCustomers = sqlConnection.CreateCommand();
                    sqlCommandGetSubordinateCustomers.CommandText = sqlStatement.ToString();
                    sqlCommandGetSubordinateCustomers.CommandTimeout = 600;
                    sqlCommandGetSubordinateCustomers.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT XRCE.ExternalSystemCustomerID, ExternalSystemCustomerName ");
                    sqlStatement.Append("       FROM ExternalSystemCustomerReferenceEntry XRCE ");
                    sqlStatement.Append("  WHERE XRCE.ExternalSystemID = @ExternalSystemID AND XRCE.MasterCustomerEntryID = @MasterCustomerEntryID ");
                    sqlStatement.Append("  ORDER BY XRCE.ExternalSystemID ");

                    sqlCommandExternalSystemCustomerReferenceEntries = sqlConnection.CreateCommand();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandExternalSystemCustomerReferenceEntries.CommandTimeout = 600;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@ExternalSystemID", System.Data.SqlDbType.Int));
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));

                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = 0;
                    sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    await sqlCommandGetSubordinateCustomers.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT CBE.SalesOrderNumber, CBE.TeamCaptainName, CBE.TeamCaptainEMailAddress, CBE.TeamCaptainTelephoneNumber, ");
                    sqlStatement.Append("       CBE.TeamCaptainOtherName, CBE.TeamCaptainOtherEMailAddress, CBE.TeamCaptainOtherTelephoneNumber, ");
                    sqlStatement.Append("       CBE.CreditsPurchased, CBE.CurrentBalance, CBE.ActivationDate, CBE.ExpirationDate, CBE.TCAcceptance, ");
                    sqlStatement.Append("       CBE.RevenueAlreadyRecognized, MCE.ExternalSystemCustomerName, CBE.ServiceBookingsNet, CBE.ServiceBookingsList ");
                    sqlStatement.Append("  FROM CLCBalanceEntry CBE LEFT OUTER JOIN MasterCustomerEntry MCE ON CBE.OriginatingMasterCustomerEntryID = MCE.EntryID ");
                    sqlStatement.Append("  WHERE CBE.MasterCustomerEntryID = @MasterCustomerEntryID AND CBE.ExpirationDate >= @ExpirationDate ");
                    sqlStatement.Append("  ORDER BY CBE.ActivationDate, CBE.SalesOrderNumber ");

                    sqlCommandGetCLCBalanceEntries = sqlConnection.CreateCommand();
                    sqlCommandGetCLCBalanceEntries.CommandText = sqlStatement.ToString();
                    sqlCommandGetCLCBalanceEntries.CommandTimeout = 600;
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@MasterCustomerEntryID", System.Data.SqlDbType.Int));
                    sqlCommandGetCLCBalanceEntries.Parameters.Add(new SqlParameter("@ExpirationDate", System.Data.SqlDbType.DateTime));

                    sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = 0;
                    sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = DateTime.MinValue;
                    await sqlCommandGetCLCBalanceEntries.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PreSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PreSalesBDMSalesLevelGroupingView GME ON GMEV.PreSalesBDMSalesLevelGroupingID = GME.PreSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PreSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPreSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPreSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPreSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPreSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPreSalesBDM.PrepareAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT BDR.CEC_ID, BDR.ReferenceName ");
                    sqlStatement.Append("       FROM PostSalesBDMSalesLevelGroupingMappingEntryView GMEV ");
                    sqlStatement.Append("            INNER JOIN PostSalesBDMSalesLevelGroupingView GME ON GMEV.PostSalesBDMSalesLevelGroupingID = GME.PostSalesBDMSalesLevelGroupingID ");
                    sqlStatement.Append("            INNER JOIN BusinessDevelopmentResourceView BDR ON GME.PostSalesBusinessDevelopmentResourceEntryID = BDR.EntryID ");
                    sqlStatement.Append("  WHERE GMEV.SalesLevel1 = @SalesLevel1 AND GMEV.SalesLevel2 = @SalesLevel2 AND GMEV.SalesLevel3 = @SalesLevel3 ");

                    sqlCommandGetPostSalesBDM = sqlConnection.CreateCommand();
                    sqlCommandGetPostSalesBDM.CommandText = sqlStatement.ToString();
                    sqlCommandGetPostSalesBDM.CommandTimeout = 600;
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel1", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel2", System.Data.SqlDbType.VarChar, 50));
                    sqlCommandGetPostSalesBDM.Parameters.Add(new SqlParameter("@SalesLevel3", System.Data.SqlDbType.VarChar, 50));

                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = "";
                    sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = "";
                    await sqlCommandGetPostSalesBDM.PrepareAsync();

                    sqlCommandGetMasterCustomerEntries.Parameters["@EndCustomerID"].Value = endCustomerIdGreaterThan;

                    int maximumMasterCustomerEntryRecordsToReturn = int.Parse(configuration["AppSettings:MaximumMasterCustomerEntryRecordsToReturn"]);

                    if (numberOfRecordsToReturn > 0 || numberOfRecordsToReturn < maximumMasterCustomerEntryRecordsToReturn)
                    {
                        sqlCommandGetMasterCustomerEntries.Parameters["@NumberOfRecordsToReturn"].Value = numberOfRecordsToReturn;
                    }
                    else
                    {
                        sqlCommandGetMasterCustomerEntries.Parameters["@NumberOfRecordsToReturn"].Value = maximumMasterCustomerEntryRecordsToReturn;
                    }

                    sqlDataReaderGetMasterCustomerEntries = await sqlCommandGetMasterCustomerEntries.ExecuteReaderAsync();
                    while (await sqlDataReaderGetMasterCustomerEntries.ReadAsync())
                    {

                        Models.ESI_MasterCustomerEntry masterCustomerEntry = new ESI_MasterCustomerEntry();

                        masterCustomerEntry.masterCustomerEntryId = sqlDataReaderGetMasterCustomerEntries.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        masterCustomerEntry.endCustomerId = sqlDataReaderGetMasterCustomerEntries.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_ID);
                        masterCustomerEntry.endCustomerHeadquartersName = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_END_CUSTOMER_HEADQUARTERS_NAME);
                        masterCustomerEntry.salesLevel1 = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        masterCustomerEntry.salesLevel2 = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        masterCustomerEntry.salesLevel3 = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);
                        masterCustomerEntry.salesLevel4 = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_4);
                        masterCustomerEntry.accountManagerCEC_ID = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_CEC_ID);

                        if (!sqlDataReaderGetMasterCustomerEntries.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME))
                        {
                            masterCustomerEntry.accountManagerName = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACCOUNT_MANAGER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntries.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME))
                        {
                            masterCustomerEntry.preferredLearningPartnerName = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_LEARNING_PARTNER_NAME);
                        }

                        if (!sqlDataReaderGetMasterCustomerEntries.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER))
                        {
                            masterCustomerEntry.expertCareCustomer = sqlDataReaderGetMasterCustomerEntries.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPERT_CARE_CUSTOMER);
                        }
                        else
                        {
                            masterCustomerEntry.expertCareCustomer = false;
                        }

                        if (!sqlDataReaderGetMasterCustomerEntries.IsDBNull(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED))
                        {
                            masterCustomerEntry.virtualTeamHandled = sqlDataReaderGetMasterCustomerEntries.GetBoolean(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_VIRTUAL_TEAM_HANDLED);
                        }
                        else
                        {
                            masterCustomerEntry.virtualTeamHandled = false;
                        }

                        sqlCommandGetSubordinateCustomers.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntries.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetSubordinateCustomers = await sqlCommandGetSubordinateCustomers.ExecuteReaderAsync();

                        while (await sqlDataReaderGetSubordinateCustomers.ReadAsync())
                        {
                            masterCustomerEntry.subordinateCustomerNames.Add(sqlDataReaderGetSubordinateCustomers.GetString(ApplicationValues.ESI_SUBORDINATE_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SUBORDINATE_CUSTOMER_NAME));
                        }       
                        await sqlDataReaderGetSubordinateCustomers.CloseAsync();

                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@ExternalSystemID"].Value = ApplicationValues.EXTERNAL_SYSTEM_ID_BUSINESS_DEVELOPMENT_MANAGER_DASHBOARD;
                        sqlCommandExternalSystemCustomerReferenceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntries.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlDataReaderGetExternalSystemCustomerReferenceEntries = await sqlCommandExternalSystemCustomerReferenceEntries.ExecuteReaderAsync();
                        while (await sqlDataReaderGetExternalSystemCustomerReferenceEntries.ReadAsync())
                        {
                            masterCustomerEntry.subordinateCustomerExternalSystemIDs.Add(sqlDataReaderGetExternalSystemCustomerReferenceEntries.GetInt32(ApplicationValues.ESI_EXTERNAL_SYSTEM_CUSTOMER_REFERENCE_ENTRIES_LIST_QUERY_RESULT_COLUMN_OFFSET_EXTERNAL_SYSTEM_CUSTOMER_ID));
                        }
                        await sqlDataReaderGetExternalSystemCustomerReferenceEntries.CloseAsync();

                        sqlCommandGetCLCBalanceEntries.Parameters["@MasterCustomerEntryID"].Value = sqlDataReaderGetMasterCustomerEntries.GetInt32(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ENTRY_ID);
                        sqlCommandGetCLCBalanceEntries.Parameters["@ExpirationDate"].Value = System.DateTime.Now.Date.AddDays(-1 * daysPastAllowedForExpiration);
                        sqlDataReaderGetCLCBalanceEntries = await sqlCommandGetCLCBalanceEntries.ExecuteReaderAsync();

                        while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())
                        {

                            ESI_CLCBalanceEntry clcBalanceEntry = new ESI_CLCBalanceEntry();

                            clcBalanceEntry.salesOrderNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_ORDER_NUMBER);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME))
                            {
                                clcBalanceEntry.teamCaptainName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_TELEPHONE_NUMBER);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME))
                            {
                                clcBalanceEntry.teamCaptainOtherName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_NAME);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS))
                            {
                                clcBalanceEntry.teamCaptainOtherEMailAddress = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_EMAIL_ADDRESS);
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER))
                            {
                                clcBalanceEntry.teamCaptainOtherTelephoneNumber = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TEAM_CAPTAIN_OTHER_TELEPHONE_NUMBER);
                            }

                            clcBalanceEntry.creditsPurchased = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CREDITS_PURCHASED);
                            clcBalanceEntry.currentBalance = sqlDataReaderGetCLCBalanceEntries.GetInt32(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_CURRENT_BALANCE);
                            clcBalanceEntry.activationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_ACTIVATION_DATE);
                            clcBalanceEntry.expirationDate = sqlDataReaderGetCLCBalanceEntries.GetDateTime(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_EXPIRATION_DATE);

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE))
                            {
                                clcBalanceEntry.termsConditionsAcceptance = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_TERMS_CONDITIONS_ACCEPTANCE);
                            }
                            else
                            {
                                clcBalanceEntry.termsConditionsAcceptance = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED))
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = sqlDataReaderGetCLCBalanceEntries.GetBoolean(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_REVENUE_ALREADY_RECOGNIZED);
                            }
                            else
                            {
                                clcBalanceEntry.revenueAlreadyRecognized = false;
                            }

                            if (!sqlDataReaderGetCLCBalanceEntries.IsDBNull(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME))
                            {
                                clcBalanceEntry.lcmtCustomerName = sqlDataReaderGetCLCBalanceEntries.GetString(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_LCMT_CUSTOMER_NAME);
                            }

                            clcBalanceEntry.serviceBookingsNet = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_NET);
                            clcBalanceEntry.serviceBookingsList = sqlDataReaderGetCLCBalanceEntries.GetDecimal(ApplicationValues.ESI_CLC_BALANCE_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SERVICE_BOOKINGS_LIST);

                            masterCustomerEntry.clcBalanceEntries.Add(clcBalanceEntry);

                        }       // while (await sqlDataReaderGetCLCBalanceEntries.ReadAsync())

                        // Get Pre-Sales BDM
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPreSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPreSalesBDM = await sqlCommandGetPreSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPreSalesBDM.ReadAsync())
                        {
                            masterCustomerEntry.preSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            masterCustomerEntry.preSalesBusinessDevelopmentManagerName = sqlDataReaderGetPreSalesBDM.GetString(ApplicationValues.ESI_PRE_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPreSalesBDM.ReadAsync()
                        sqlDataReaderGetPreSalesBDM.Close();

                        // Get Post-Sales BDM
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel1"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_1);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel2"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_2);
                        sqlCommandGetPostSalesBDM.Parameters["@SalesLevel3"].Value = sqlDataReaderGetMasterCustomerEntries.GetString(ApplicationValues.ESI_MASTER_CUSTOMER_ENTRY_QUERY_RESULT_COLUMN_OFFSET_SALES_LEVEL_3);

                        sqlDataReaderGetPostSalesBDM = await sqlCommandGetPostSalesBDM.ExecuteReaderAsync();
                        if (await sqlDataReaderGetPostSalesBDM.ReadAsync())
                        {
                            masterCustomerEntry.postSalesBusinessDevelopmentManagerCEC_ID = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_CEC_ID);
                            masterCustomerEntry.postSalesBusinessDevelopmentManagerName = sqlDataReaderGetPostSalesBDM.GetString(ApplicationValues.ESI_POST_SALES_BUSINESS_DEVELOPMENT_MANAGER_NAME);
                        }       // await sqlDataReaderGetPostSalesBDM.ReadAsync()
                        sqlDataReaderGetPostSalesBDM.Close();


                        returnValue.Add(masterCustomerEntry);

                    }       // while (await sqlDataReaderGetMasterCustomerEntries.ReadAsync())
                    await sqlDataReaderGetMasterCustomerEntries.CloseAsync();

                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))

                sqlConnection.Close();

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in ExternalSystemInquiryWSController::getMasterCustomerEntries().  Message is {0}", ex1.Message));

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

        }       // getMasterCustomers()


    }
}
