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
    public class ElectionForTerritoryWSController : ControllerBase
    {
        private ILogger<ElectionForTerritoryWSController> logger;
        private IConfiguration configuration;

        public ElectionForTerritoryWSController(IConfiguration configuration, ILogger<ElectionForTerritoryWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }


        [Route("electionForTerritory/{electionForTerritoryId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ElectionForTerritory>> GetElectionForTerritory(int electionForTerritoryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritory;
            NpgsqlDataReader sqlDataReaderGetTerritory;

            try
            {

                Models.ElectionForTerritory returnValue = new Models.ElectionForTerritory();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT eft.territory_id, eft.election_date, et.description ");
                    sqlStatement.Append("  FROM election_for_territory eft inner join election_type et on eft.election_type_id = et.election_type_id ");
                    sqlStatement.Append("  WHERE eft.election_for_territory_id = @election_for_territory_id ");

                    sqlCommandGetTerritory = sqlConnection.CreateCommand();
                    sqlCommandGetTerritory.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritory.CommandTimeout = 600;
                    sqlCommandGetTerritory.Parameters.Add(new NpgsqlParameter("@election_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritory.Parameters["@election_for_territory_id"].Value = 0;
                    await sqlCommandGetTerritory.PrepareAsync();

                    sqlCommandGetTerritory.Parameters["@election_for_territory_id"].Value = electionForTerritoryId;
                    using (sqlDataReaderGetTerritory = await sqlCommandGetTerritory.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetTerritory.ReadAsync())
                        {
                            returnValue.electionForTerritoryId = electionForTerritoryId;
                            returnValue.territoryId = sqlDataReaderGetTerritory.GetInt32(ApplicationValues.ELECTION_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            returnValue.electionDate = sqlDataReaderGetTerritory.GetDateTime(ApplicationValues.ELECTION_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_ELECTION_DATE);
                            returnValue.electionTypeDescription = sqlDataReaderGetTerritory.GetString(ApplicationValues.ELECTION_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_ELECTION_TYPE_DESCRIPTION);
                        };

                        await sqlDataReaderGetTerritory.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in ElectionForTerritoryWSController::GetElectionForTerritory().  Message is {0}", ex1.Message));

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

        }       // GetElectionForTerritory()

        [Route("electionForTerritory/byTerritory/{territoryId}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.ElectionForTerritory>>> GetElectionsForTerritory(int territoryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritories;
            NpgsqlDataReader sqlDataReaderGetTerritories;

            try
            {

                List<Models.ElectionForTerritory> returnValue = new List<Models.ElectionForTerritory>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT eft.election_for_territory_id, eft.territory_id, eft.election_date, et.description ");
                    sqlStatement.Append("  FROM election_for_territory eft inner join election_type et on eft.election_type_id = et.election_type_id ");
                    sqlStatement.Append("  WHERE eft.territory_id = @territory_id ");
                    sqlStatement.Append("  ORDER BY eft.election_date DESC ");

                    sqlCommandGetTerritories = sqlConnection.CreateCommand();
                    sqlCommandGetTerritories.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritories.CommandTimeout = 600;
                    sqlCommandGetTerritories.Parameters.Add(new NpgsqlParameter("@election_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = 0;
                    await sqlCommandGetTerritories.PrepareAsync();

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = territoryId;
                    using (sqlDataReaderGetTerritories = await sqlCommandGetTerritories.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetTerritories.ReadAsync())
                        {
                            Models.ElectionForTerritory electionForTerritory = new Models.ElectionForTerritory();

                            electionForTerritory.electionForTerritoryId = territoryId;
                            electionForTerritory.territoryId = sqlDataReaderGetTerritories.GetInt32(ApplicationValues.ELECTION_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            electionForTerritory.electionDate = sqlDataReaderGetTerritories.GetDateTime(ApplicationValues.ELECTION_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTION_DATE);
                            electionForTerritory.electionTypeDescription = sqlDataReaderGetTerritories.GetString(ApplicationValues.ELECTION_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTION_TYPE_DESCRIPTION);

                            returnValue.Add(electionForTerritory);

                        };

                        await sqlDataReaderGetTerritories.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in ElectionForTerritoryWSController::GetElectionsForTerritory().  Message is {0}", ex1.Message));

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

        }       // GetElectionsForTerritory()




    }
}
