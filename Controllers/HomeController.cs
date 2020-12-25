using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;

namespace Overthink.PolitiScout.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> logger;
        private IConfiguration configuration;

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("/")]
        public IActionResult Index()
        {

            Models.SinglePageAppParameters singlePageAppParameters;

            singlePageAppParameters = new Models.SinglePageAppParameters($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}",
                                                                         configuration["AppSettings:SystemVersion"], configuration["AppSettings:DeployedEnvironmentName"], null, null);


            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return View(singlePageAppParameters);

        }

        [Route("/redirectToSSO")]
        [Authorize]
        public IActionResult SecuredAfterRedirect()
        {

            SqlConnection sqlConnection = null;
            System.Text.StringBuilder sqlStatement;
            SqlCommand sqlCommandGetSystemRoleMapping;
            SqlDataReader sqlDataReaderGetSystemRoleMapping;
            SqlCommand sqlCommandGetMenuOption;
            SqlDataReader sqlDataReaderGetMenuOption;
            SqlCommand sqlCommandInsertSystemSession;

            string systemSessionExternalKey;
            bool userAuthorized = false;
            Models.SecuredMenuPageAppParameters securedMenuPageAppParameters;

            securedMenuPageAppParameters = new Models.SecuredMenuPageAppParameters($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}",
                                                                                   configuration["AppSettings:SystemVersion"], configuration["AppSettings:DeployedEnvironmentName"], null, null);

            var claimUserId = User.Claims.Where(c => c.Type == "sub").Single();

            try
            {

                userAuthorized = true;

                if (userAuthorized)
                {

                    using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:LAtC_BDM_CLCProcessing"]))
                    {

                        sqlConnection.Open();

                        sqlStatement = new System.Text.StringBuilder();
                        sqlStatement.Append("INSERT INTO SystemSessionView ");
                        sqlStatement.Append("  (BusinessDevelopmentSystemID, SystemSessionExternalKey, CEC_ID, BusinessDevelopmentSystemRoleID, RecordAddedDateTime, ");
                        sqlStatement.Append("   RecordLastUpdatedDateTime) ");
                        sqlStatement.Append("  VALUES (@BusinessDevelopmentSystemID, @SystemSessionExternalKey, @CEC_ID, @BusinessDevelopmentSystemRoleID, @RecordAddedDateTime, ");
                        sqlStatement.Append("          @RecordLastUpdatedDateTime) ");

                        sqlCommandInsertSystemSession = sqlConnection.CreateCommand();
                        sqlCommandInsertSystemSession.CommandText = sqlStatement.ToString();
                        sqlCommandInsertSystemSession.CommandTimeout = 600;
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@BusinessDevelopmentSystemID", System.Data.SqlDbType.Int));
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@SystemSessionExternalKey", System.Data.SqlDbType.VarChar, 36));
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@CEC_ID", System.Data.SqlDbType.VarChar, 20));
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@BusinessDevelopmentSystemRoleID", System.Data.SqlDbType.Int));
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@RecordAddedDateTime", System.Data.SqlDbType.DateTime));
                        sqlCommandInsertSystemSession.Parameters.Add(new SqlParameter("@RecordLastUpdatedDateTime", System.Data.SqlDbType.DateTime));

                        sqlCommandInsertSystemSession.Parameters["@BusinessDevelopmentSystemID"].Value = 0;
                        sqlCommandInsertSystemSession.Parameters["@SystemSessionExternalKey"].Value = "";
                        sqlCommandInsertSystemSession.Parameters["@CEC_ID"].Value = "";
                        sqlCommandInsertSystemSession.Parameters["@BusinessDevelopmentSystemRoleID"].Value = 0;
                        sqlCommandInsertSystemSession.Parameters["@RecordAddedDateTime"].Value = DateTime.MinValue;
                        sqlCommandInsertSystemSession.Parameters["@RecordLastUpdatedDateTime"].Value = DateTime.MinValue;
                        sqlCommandInsertSystemSession.Prepare();

                        sqlStatement = new System.Text.StringBuilder();
                        sqlStatement.Append("SELECT BDSME.EntryID, BDSME.BusinessDevelopmentSystemRoleID, BDSME.Active ");
                        sqlStatement.Append("  FROM BusinessDevelopmentSystemRoleMappingEntryView BDSME ");
                        sqlStatement.Append("  WHERE BDSME.CEC_ID = @CEC_ID ");

                        sqlCommandGetSystemRoleMapping = sqlConnection.CreateCommand();
                        sqlCommandGetSystemRoleMapping.CommandText = sqlStatement.ToString();
                        sqlCommandGetSystemRoleMapping.CommandTimeout = 600;
                        sqlCommandGetSystemRoleMapping.Parameters.Add(new SqlParameter("@CEC_ID", System.Data.SqlDbType.VarChar, 20));

                        sqlCommandGetSystemRoleMapping.Parameters["@CEC_ID"].Value = "";
                        sqlCommandGetSystemRoleMapping.Prepare();

                        sqlStatement = new System.Text.StringBuilder();
                        sqlStatement.Append("SELECT M.Enabled ");
                        sqlStatement.Append("  FROM MenuOption M ");
                        sqlStatement.Append("  WHERE M.MenuOptionID = @MenuOptionID");

                        sqlCommandGetMenuOption = sqlConnection.CreateCommand();
                        sqlCommandGetMenuOption.CommandText = sqlStatement.ToString();
                        sqlCommandGetMenuOption.CommandTimeout = 600;
                        sqlCommandGetMenuOption.Parameters.Add(new SqlParameter("@MenuOptionID", System.Data.SqlDbType.Int));

                        sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = 0;
                        sqlCommandGetMenuOption.Prepare();

                        // Look up user record to determine role mapping and set appropriate menu opions

                        sqlCommandGetSystemRoleMapping.Parameters["@CEC_ID"].Value = claimUserId.Value;
                        sqlDataReaderGetSystemRoleMapping = sqlCommandGetSystemRoleMapping.ExecuteReader();

                        if (sqlDataReaderGetSystemRoleMapping.Read())
                        {
                            // check for active user

                            if (sqlDataReaderGetSystemRoleMapping.GetBoolean(ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ROLE_MAPPING_ENTRY_QUERY_REUSLT_COLUMN_OFFSET_ACTIVE))
                            {

                                // Determine role and pass into parameter set to properly set menu options

                                sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_AFFLIATED_SALES_RESOURCE_LOOKUP;
                                sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                if (sqlDataReaderGetMenuOption.Read())
                                {
                                    securedMenuPageAppParameters.ShowMenuOptionAffiliatedSalesResourceLookupMenuItem = sqlDataReaderGetMenuOption.GetBoolean(0);
                                }
                                else
                                {
                                    securedMenuPageAppParameters.ShowMenuOptionAffiliatedSalesResourceLookupMenuItem = false;
                                }
                                sqlDataReaderGetMenuOption.Close();

                                // RJB 2020-10-22
                                sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_VIEW_MASTER_CUSTOMER_INFORMATION;
                                sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                if (sqlDataReaderGetMenuOption.Read())
                                {
                                    securedMenuPageAppParameters.ShowMenuOptionViewMasterCustomerInformation = sqlDataReaderGetMenuOption.GetBoolean(0);
                                }
                                else
                                {
                                    securedMenuPageAppParameters.ShowMenuOptionViewMasterCustomerInformation = false;
                                }
                                sqlDataReaderGetMenuOption.Close();

                                if (sqlDataReaderGetSystemRoleMapping.GetInt32(ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ROLE_MAPPING_ENTRY_QUERY_REUSLT_COLUMN_OFFSET_ROLE_ID) ==
                                    ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ROLE_THIS_SYSTEM_ADMINISTRATOR)
                                {

                                    sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_MAP_NEW_CUSTOMERS_FROM_EXTERNAL_SYSTEMS;
                                    sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                    if (sqlDataReaderGetMenuOption.Read())
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionMaintainMasterSubordinateCustomerMenuItem = sqlDataReaderGetMenuOption.GetBoolean(0);
                                    }
                                    else
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionMaintainMasterSubordinateCustomerMenuItem = true;
                                    }
                                    sqlDataReaderGetMenuOption.Close();

                                    sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_SUBORDINATE_EXISTING_CUSTOMER;
                                    sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                    if (sqlDataReaderGetMenuOption.Read())
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionSubordinateExistingCustomerMenuItem = sqlDataReaderGetMenuOption.GetBoolean(0);
                                    }
                                    else
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionSubordinateExistingCustomerMenuItem = true;
                                    }
                                    sqlDataReaderGetMenuOption.Close();

                                    sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_REMAP_EXISTING_CUSTOMER_TO_DIFFERENT_CORPORATE_END_CUSTOMER;
                                    sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                    if (sqlDataReaderGetMenuOption.Read())
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionRemapToDifferentCorporateEndCustomer = sqlDataReaderGetMenuOption.GetBoolean(0);
                                    }
                                    else
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionRemapToDifferentCorporateEndCustomer = true;
                                    }
                                    sqlDataReaderGetMenuOption.Close();

                                    sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_PROMOTE_SUBORDINATE_CUSTOMER;
                                    sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                    if (sqlDataReaderGetMenuOption.Read())
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionPromoteSubordinateCustomer = sqlDataReaderGetMenuOption.GetBoolean(0);
                                    }
                                    else
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionPromoteSubordinateCustomer = true;
                                    }
                                    sqlDataReaderGetMenuOption.Close();

                                    sqlCommandGetMenuOption.Parameters["@MenuOptionID"].Value = ApplicationValues.MENU_OPTION_UPDATE_MASTER_CUSTOMER_INFORMATION;
                                    sqlDataReaderGetMenuOption = sqlCommandGetMenuOption.ExecuteReader();
                                    if (sqlDataReaderGetMenuOption.Read())
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionUpdateMasterCustomerInformation = sqlDataReaderGetMenuOption.GetBoolean(0);
                                    }
                                    else
                                    {
                                        securedMenuPageAppParameters.ShowMenuOptionUpdateMasterCustomerInformation = true;
                                    }
                                    sqlDataReaderGetMenuOption.Close();

                                }
                                else
                                {
                                    securedMenuPageAppParameters.ShowMenuOptionMaintainMasterSubordinateCustomerMenuItem = false;
                                    securedMenuPageAppParameters.ShowMenuOptionSubordinateExistingCustomerMenuItem = false;
                                    securedMenuPageAppParameters.ShowMenuOptionRemapToDifferentCorporateEndCustomer = false;
                                }

                                // Add session entry
                                systemSessionExternalKey = System.Guid.NewGuid().ToString().Replace("-", "");

                                sqlCommandInsertSystemSession.Parameters["@BusinessDevelopmentSystemID"].Value = ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ID_DEVCX_NAME_REFINERY;
                                sqlCommandInsertSystemSession.Parameters["@SystemSessionExternalKey"].Value = systemSessionExternalKey;
                                sqlCommandInsertSystemSession.Parameters["@CEC_ID"].Value = claimUserId.Value;
                                sqlCommandInsertSystemSession.Parameters["@BusinessDevelopmentSystemRoleID"].Value = sqlDataReaderGetSystemRoleMapping.GetInt32(ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ROLE_MAPPING_ENTRY_QUERY_REUSLT_COLUMN_OFFSET_ROLE_ID);
                                sqlCommandInsertSystemSession.Parameters["@RecordAddedDateTime"].Value = System.DateTime.Now;
                                sqlCommandInsertSystemSession.Parameters["@RecordLastUpdatedDateTime"].Value = sqlCommandInsertSystemSession.Parameters["@RecordAddedDateTime"].Value;
                                sqlCommandInsertSystemSession.ExecuteNonQuery();

                                Response.Cookies.Append(ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY, systemSessionExternalKey);

                                userAuthorized = true;
                            }
                            else
                            {
                                logger.LogWarning("User ID {0} is authorized in Cisco Groups but is flagged as inactive system role mapping table.  The user should be removed from Cisco Groups if permanently inactive.", claimUserId.Value);
                            }

                        }
                        else
                        {
                            logger.LogWarning("User ID {0} is authorized in Cisco Groups but a record was not found in the system role mapping table.", claimUserId.Value);

                        }       // (sqlDataReaderGetSystemRoleMapping.Read())
                        sqlDataReaderGetSystemRoleMapping.Close();

                        sqlConnection.Close();

                        if (userAuthorized)
                        {
                            return View("SecuredMenu", securedMenuPageAppParameters);
                        }
                        else
                        {
                            return View("NotAuthorized", securedMenuPageAppParameters);
                        }
                    }       // using (sqlConnection = new SqlConnection(this._configuration["ConnectionStrings:DevCXMain"]))

                }
                else
                {
                    return View("NotAuthorized", securedMenuPageAppParameters);
                }

            }
            catch (Exception ex1)
            {

                this.logger.LogError(ex1, "Error occurred in HomeController::SecuredAfterRedirect().  Message is {0}", ex1.Message);
                securedMenuPageAppParameters.systemErrors.Add(ex1.Message);

                return View("ErrorOccured", securedMenuPageAppParameters);

            }

        }       // SecuredAfterRedirect()

        [Route("/logout")]
        [HttpGet]
        public IActionResult Logout()
        {

            // Perform any client-side cleanup functions

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return Redirect(configuration["AppSettings:SSOGlobalLogoutUrl"]);

        }       // Logout()

    }
}