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
    public class TerritoryWSController : ControllerBase
    {

        private ILogger<TerritoryWSController> logger;
        private IConfiguration configuration;

        public TerritoryWSController(IConfiguration configuration, ILogger<TerritoryWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("territory/{territoryId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.Territory>> GetTerritory(int territoryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritory;
            NpgsqlDataReader sqlDataReaderGetTerritory;

            try
            {

                Models.Territory returnValue = new Models.Territory();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT t.territory_level_id, t.full_name, t.short_name_abbreviation ");
                    sqlStatement.Append("  FROM territory t ");
                    sqlStatement.Append("  WHERE t.territory_id = @territory_id ");

                    sqlCommandGetTerritory = sqlConnection.CreateCommand();
                    sqlCommandGetTerritory.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritory.CommandTimeout = 600;
                    sqlCommandGetTerritory.Parameters.Add(new NpgsqlParameter("@territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritory.Parameters["@territory_id"].Value = 0;
                    await sqlCommandGetTerritory.PrepareAsync();

                    sqlCommandGetTerritory.Parameters["@territory_id"].Value = territoryId;
                    using (sqlDataReaderGetTerritory = await sqlCommandGetTerritory.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetTerritory.ReadAsync())
                        {
                            returnValue.territoryId = territoryId;
                            returnValue.territoryLevelId = sqlDataReaderGetTerritory.GetInt32(ApplicationValues.TERRITORY_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            returnValue.fullName = sqlDataReaderGetTerritory.GetString(ApplicationValues.TERRITORY_QUERY_RESULT_COLUMN_OFFSET_FULL_NAME);
                            returnValue.shortName = sqlDataReaderGetTerritory.GetString(ApplicationValues.TERRITORY_QUERY_RESULT_COLUMN_OFFSET_SHORT_NAME);
                        };

                        await sqlDataReaderGetTerritory.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in TerritoryWSController::GetTerritory().  Message is {0}", ex1.Message));

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

        }       // GetTerritory()

        [Route("territory/forTerritoryLevel/{territoryLevelId}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.Territory>>> GetTerritoriesForLevel(int territoryLevelId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetTerritories;
            NpgsqlDataReader sqlDataReaderGetTerritories;

            try
            {

                List<Models.Territory> returnValue = new List<Models.Territory>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT t.territory_id, t.territory_level_id, t.full_name, t.short_name_abbreviation ");
                    sqlStatement.Append("  FROM territory t ");
                    sqlStatement.Append("  WHERE t.territory_level_id = @territory_level_id ");
                    sqlStatement.Append("  ORDER BY t.full_name ");

                    sqlCommandGetTerritories = sqlConnection.CreateCommand();
                    sqlCommandGetTerritories.CommandText = sqlStatement.ToString();
                    sqlCommandGetTerritories.CommandTimeout = 600;
                    sqlCommandGetTerritories.Parameters.Add(new NpgsqlParameter("@territory_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = 0;
                    await sqlCommandGetTerritories.PrepareAsync();

                    sqlCommandGetTerritories.Parameters["@territory_id"].Value = territoryLevelId;
                    using (sqlDataReaderGetTerritories = await sqlCommandGetTerritories.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetTerritories.ReadAsync())
                        {

                            Models.Territory territory = new Models.Territory();

                            territory.territoryId = sqlDataReaderGetTerritories.GetInt32(ApplicationValues.TERRITORY_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            territory.territoryLevelId = territoryLevelId;
                            territory.fullName = sqlDataReaderGetTerritories.GetString(ApplicationValues.TERRITORY_QUERY_RESULT_COLUMN_OFFSET_FULL_NAME);
                            territory.shortName = sqlDataReaderGetTerritories.GetString(ApplicationValues.TERRITORY_QUERY_RESULT_COLUMN_OFFSET_SHORT_NAME);

                            returnValue.Add(territory);
                        };

                        await sqlDataReaderGetTerritories.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in TerritoryWSController::GetTerritoriesForLevel().  Message is {0}", ex1.Message));

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

        }       // GetTerritoriesForLevel()



    }
}
