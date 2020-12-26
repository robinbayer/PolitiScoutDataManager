using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Overthink.PolitiScout.Controllers
{
    [Route("ui")]
    public class PersonUIController : Controller
    {

        private readonly ILogger<PersonUIController> logger;
        private IConfiguration configuration;

        public PersonUIController(IConfiguration configuration, ILogger<PersonUIController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }


        [Route("maintainPersonList")]
#if (!DEBUG)
        //[Authorize]
#endif
        public IActionResult MaintainPersonList()
        {


            Models.SinglePageAppParameters singlePageAppParameters;
            bool validSession = false;

            singlePageAppParameters = new Models.SinglePageAppParameters($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}",
                                                                         configuration["AppSettings:SystemVersion"], configuration["AppSettings:DeployedEnvironmentName"], null, null);

#if (!DEBUG)
/*
            if (Request.Cookies[ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY] != null)
            {

                using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))
                {

                    sqlConnection.Open();

                    sqlStatement = new System.Text.StringBuilder();

                    sqlStatement.Append("SELECT SS.BusinessDevelopmentSystemRoleID, SS.RecordLastUpdatedDateTime ");
                    sqlStatement.Append("  FROM SystemSession SS ");
                    sqlStatement.Append("  WHERE SS.SystemSessionExternalKey = @SystemSessionExternalKey ");

                    sqlCommandGetSystemSession = sqlConnection.CreateCommand();
                    sqlCommandGetSystemSession.CommandText = sqlStatement.ToString();
                    sqlCommandGetSystemSession.CommandTimeout = 600;
                    sqlCommandGetSystemSession.Parameters.Add(new SqlParameter("@SystemSessionExternalKey", System.Data.SqlDbType.VarChar, 36));

                    sqlCommandGetSystemSession.Parameters["@SystemSessionExternalKey"].Value = "";
                    sqlCommandGetSystemSession.Prepare();

                    sqlCommandGetSystemSession.Parameters["@SystemSessionExternalKey"].Value = Request.Cookies[ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY];
                    sqlDataReaderGetSystemSession = sqlCommandGetSystemSession.ExecuteReader();
                    if (sqlDataReaderGetSystemSession.Read())
                    {

                        // Found session - Check to ensure not greater than maximum application-enforced session length

                        if (sqlDataReaderGetSystemSession.GetDateTime(ApplicationValues.SYSTEM_SESSION_QUERY_RESULT_COLUMN_OFFSET_RECORD_LAST_UPDATED_DATE_TIME)
                            .AddSeconds(int.Parse(configuration["AppSettings:MaximumSessionLengthInSeconds"])) < System.DateTime.Now)
                        {

                            Response.Cookies.Delete(ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY);

                            return View("NotLoggedIntoSystem", singlePageAppParameters);
                        }
                        else
                        {

                            // PROGRAMMER'S NOTE:  Ensure that Business Development System Role ID for session is applicable for access to functionality
                            if (sqlDataReaderGetSystemSession.GetInt32(ApplicationValues.SYSTEM_SESSION_QUERY_RESULT_COLUMN_OFFSET_BUSINESS_DEVELOPMENT_SYSTEM_ROLE_ID) == ApplicationValues.BUSINESS_DEVELOPMENT_SYSTEM_ROLE_THIS_SYSTEM_ADMINISTRATOR)
                            {
                                validSession = true;
                            }
                        }

                    }

                    sqlConnection.Close();
*/
#endif

#if (DEBUG)
            validSession = true;
#endif


            if (validSession)
            {

                // PROGRAMMER'S NOTE:  May want to log access to function for odometer purposes or auditing

                return View("MaintainPersonList", singlePageAppParameters);

            }
            else
            {
                logger.LogWarning("Attempted to connect with invalid session key = '{0}'", Request.Cookies[ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY]);
                return View("NotLoggedIntoSystem", singlePageAppParameters);
            }


#if (!DEBUG)
/*
                }       // using (sqlConnection = new SqlConnection(this.configuration["ConnectionStrings:DevCXMain"]))

            }
            else
            {
                // External System Session Key cookie value not found
                logger.LogWarning("Attempted to connect without a session key");
                return View("NotLoggedIntoSystem", singlePageAppParameters);

            }       // (Request.Cookies[ApplicationValues.COOKIE_NAME_SYSTEM_SESSION_EXTERNAL_KEY] != null)
*/
#endif

        }       // MaintainPersonList()

    }
}
