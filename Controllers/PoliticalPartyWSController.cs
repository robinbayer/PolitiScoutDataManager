using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Npgsql;

namespace Overthink.PolitiScout.Controllers
{
    [Route("ws")]
    [ApiController]
    public class PoliticalPartyWSController : ControllerBase
    {

        private ILogger<PoliticalPartyWSController> logger;
        private IConfiguration configuration;

        public PoliticalPartyWSController(IConfiguration configuration, ILogger<PoliticalPartyWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("politicalParty/{politicalPartyId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.PoliticalParty>> GetPoliticalParty(int politicalPartyId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetPoliticalParty;
            NpgsqlDataReader sqlDataReaderGetPoliticalParty;

            try
            {

                Models.PoliticalParty returnValue = new Models.PoliticalParty();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT p.reference_name, p.abbreviation ");
                    sqlStatement.Append("  FROM political_party p ");
                    sqlStatement.Append("  WHERE p.political_party_id = @political_party_id ");

                    sqlCommandGetPoliticalParty = sqlConnection.CreateCommand();
                    sqlCommandGetPoliticalParty.CommandText = sqlStatement.ToString();
                    sqlCommandGetPoliticalParty.CommandTimeout = 600;
                    sqlCommandGetPoliticalParty.Parameters.Add(new NpgsqlParameter("@political_party_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetPoliticalParty.Parameters["@political_party_id"].Value = 0;
                    await sqlCommandGetPoliticalParty.PrepareAsync();

                    sqlCommandGetPoliticalParty.Parameters["@political_party_id"].Value = politicalPartyId;
                    using (sqlDataReaderGetPoliticalParty = await sqlCommandGetPoliticalParty.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetPoliticalParty.ReadAsync())
                        {
                            returnValue.politicalPartyId = politicalPartyId;
                            returnValue.referenceName = sqlDataReaderGetPoliticalParty.GetString(ApplicationValues.POLITICAL_PARTY_QUERY_RESULT_COLUMN_OFFSET_REFERENCE_NAME);
                            returnValue.abbreviation = sqlDataReaderGetPoliticalParty.GetString(ApplicationValues.POLITICAL_PARTY_QUERY_RESULT_COLUMN_OFFSET_ABBREVIATION);
                        };

                        await sqlDataReaderGetPoliticalParty.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PoliticalPartyWSController::GetPoliticalParty().  Message is {0}", ex1.Message));

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

        }       // GetPoliticalParty()


        [Route("politicalParty/list/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.PoliticalParty>>> GetPoliticalPartyList()
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetPolicitalPartyList;
            NpgsqlDataReader sqlDataReaderGetPolicitalPartyList;

            try
            {

                List<Models.PoliticalParty> returnValue = new List<Models.PoliticalParty>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT p.reference_name, p.abbreviation ");
                    sqlStatement.Append("  FROM political_party p ");
                    sqlStatement.Append("  ORDER BY p.reference_name ");

                    sqlCommandGetPolicitalPartyList = sqlConnection.CreateCommand();
                    sqlCommandGetPolicitalPartyList.CommandText = sqlStatement.ToString();
                    sqlCommandGetPolicitalPartyList.CommandTimeout = 600;
                    await sqlCommandGetPolicitalPartyList.PrepareAsync();

                    using (sqlDataReaderGetPolicitalPartyList = await sqlCommandGetPolicitalPartyList.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetPolicitalPartyList.ReadAsync())
                        {

                            Models.PoliticalParty politicalParty = new Models.PoliticalParty();

                            politicalParty.politicalPartyId = sqlDataReaderGetPolicitalPartyList.GetInt32(ApplicationValues.POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_ID);
                            politicalParty.referenceName = sqlDataReaderGetPolicitalPartyList.GetString(ApplicationValues.POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_REFERENCE_NAME);
                            politicalParty.abbreviation = sqlDataReaderGetPolicitalPartyList.GetString(ApplicationValues.POLITICAL_PARTY_LIST_QUERY_RESULT_COLUMN_OFFSET_ABBREVIATION);

                            returnValue.Add(politicalParty);

                        };

                        await sqlDataReaderGetPolicitalPartyList.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PoliticalPartyWSController::GetPoliticalParty().  Message is {0}", ex1.Message));

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

        }       // GetPoliticalPartyList()


    }
}
