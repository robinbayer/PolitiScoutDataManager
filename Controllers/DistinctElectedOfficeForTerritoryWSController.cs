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
    public class DistinctElectedOfficeForTerritoryWSController : ControllerBase
    {

        private ILogger<DistinctElectedOfficeForTerritoryWSController> logger;
        private IConfiguration configuration;

        public DistinctElectedOfficeForTerritoryWSController(IConfiguration configuration, ILogger<DistinctElectedOfficeForTerritoryWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("distinctElectedOfficeForTerritory/{distinctElectedOfficeForTerritoryId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ElectionForTerritory>> GetDistinctElectedOfficeForTerritory(int distinctElectedOfficeForTerritoryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritory;
            NpgsqlDataReader sqlDataReaderGetTerritory;

            try
            {

                Models.DistinctElectedOfficeForTerritory returnValue = new Models.DistinctElectedOfficeForTerritory();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT deot.electedOfficeForTerritoryId, eot.territory_id, eo.reference_name, deot.distinctive_office_designator ");
                    sqlStatement.Append("  FROM distinct_elected_office_for_territory deot inner join elected_office_for_territory eot on deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN elected_office eo on eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("  WHERE deot.distinct_elected_office_for_territory_id = @distinct_elected_office_for_territory_id ");

                    sqlCommandGetTerritory = sqlConnection.CreateCommand();
                    sqlCommandGetTerritory.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritory.CommandTimeout = 600;
                    sqlCommandGetTerritory.Parameters.Add(new NpgsqlParameter("@election_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritory.Parameters["@distinct_elected_office_for_territory_id"].Value = 0;
                    await sqlCommandGetTerritory.PrepareAsync();

                    sqlCommandGetTerritory.Parameters["@distinct_elected_office_for_territory_id"].Value = distinctElectedOfficeForTerritoryId;
                    using (sqlDataReaderGetTerritory = await sqlCommandGetTerritory.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetTerritory.ReadAsync())
                        {
                            returnValue.distinctElectedOfficeForTerritoryId = distinctElectedOfficeForTerritoryId;
                            returnValue.electedOfficeForTerritoryId = sqlDataReaderGetTerritory.GetInt32(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            returnValue.electedOfficeReferenceName = sqlDataReaderGetTerritory.GetString(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            returnValue.distinctOfficeDesignator = sqlDataReaderGetTerritory.GetString(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DEISGNATOR);

                        };

                        await sqlDataReaderGetTerritory.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DistinctElectedOfficeForTerritoryWSController::GetDistinctElectedOfficeForTerritory().  Message is {0}", ex1.Message));

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

        }       // GetDistinctElectedOfficeForTerritory()


        [Route("distinctElectedOfficeForTerritory/byTerritory/{territoryId}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.DistinctElectedOfficeForTerritory>>> GetDistinctElectedOfficesForTerritory(int territoryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritories;
            NpgsqlDataReader sqlDataReaderGetTerritories;

            try
            {

                List<Models.DistinctElectedOfficeForTerritory> returnValue = new List<Models.DistinctElectedOfficeForTerritory>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT deot.distinct_elected_office_for_territory_id, deot.electedOfficeForTerritoryId, eot.territory_id, eo.reference_name, deot.distinctive_office_designator ");
                    sqlStatement.Append("  FROM distinct_elected_office_for_territory deot inner join elected_office_for_territory eot on deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN elected_office eo on eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("  WHERE deot.distinct_elected_office_for_territory_id = @distinct_elected_office_for_territory_id ");

                    sqlCommandGetTerritories = sqlConnection.CreateCommand();
                    sqlCommandGetTerritories.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritories.CommandTimeout = 600;
                    sqlCommandGetTerritories.Parameters.Add(new NpgsqlParameter("@election_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = 0;
                    await sqlCommandGetTerritories.PrepareAsync();

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = territoryId;
                    using (sqlDataReaderGetTerritories = await sqlCommandGetTerritories.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetTerritories.ReadAsync())
                        {
                            Models.DistinctElectedOfficeForTerritory distinctElectedOfficeForTerritory = new Models.DistinctElectedOfficeForTerritory();

                            distinctElectedOfficeForTerritory.distinctElectedOfficeForTerritoryId = sqlDataReaderGetTerritories.GetInt32(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            distinctElectedOfficeForTerritory.electedOfficeForTerritoryId = sqlDataReaderGetTerritories.GetInt32(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            distinctElectedOfficeForTerritory.territoryId = territoryId;
                            distinctElectedOfficeForTerritory.electedOfficeReferenceName = sqlDataReaderGetTerritories.GetString(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            distinctElectedOfficeForTerritory.distinctOfficeDesignator = sqlDataReaderGetTerritories.GetString(ApplicationValues.DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DEISGNATOR);
                        };

                        await sqlDataReaderGetTerritories.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DistinctElectedOfficeForTerritoryWSController::GetDistinctElectedOfficesForTerritory().  Message is {0}", ex1.Message));

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

        }       // GetDistinctElectedOfficesForTerritory()


    }
}
