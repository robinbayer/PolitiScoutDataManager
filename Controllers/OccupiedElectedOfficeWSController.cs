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
    public class OccupiedElectedOfficeWSController : ControllerBase
    {

        private ILogger<OccupiedElectedOfficeWSController> logger;
        private IConfiguration configuration;

        public OccupiedElectedOfficeWSController(IConfiguration configuration, ILogger<OccupiedElectedOfficeWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }


        [Route("occupiedElectedOffice/{occupiedElectedOfficeId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.OccupiedElectedOffice>> GetOccupiedElectedOffice(int occupiedElectedOfficeId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetOccupiedElectedOffice;
            NpgsqlDataReader sqlDataReaderGetOccupiedElectedOffice;

            try
            {

                Models.OccupiedElectedOffice returnValue = new Models.OccupiedElectedOffice();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT tl.territory_level_id, tl.reference_name, t.territory_id, t.full_name, eo.reference_name, ");
                    sqlStatement.Append("       deot.distinct_elected_office_for_territory_id, deot.distinct_office_designator, rfe.reason_for_entry_id,");
                    sqlStatement.Append("       rfe.description, rfd.reason_for_departure_id, rfd.description, oeo.start_date, oeo.end_date ");
                    sqlStatement.Append("  FROM occupied_elected_office oeo INNER JOIN distinct_elected_office_for_territory deot ");
                    sqlStatement.Append("                                              ON oeo.distinct_elected_office_for_territory_id = deot.distinct_elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN territory t ON deot.territory_id = t.territory_id ");
                    sqlStatement.Append("       INNER JOIN territory_level tl ON t.territory_level_id = tl.territory_level_id ");
                    sqlStatement.Append("       INNER JOIN elected_office_for_territory eot on deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN elected_office eo ON eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("       INNER JOIN reason_for_entry rfe oeo.reason_for_entry_id = rfe.reason_for_entry_id ");
                    sqlStatement.Append("       INNER JOIN reason_for_departure rfd oeo.reason_for_departure_id = rfe.reason_for_departure_id ");
                    sqlStatement.Append("  WHERE oeo.occupied_elected_office_id = @occupied_elected_office_id ");


                    sqlCommandGetOccupiedElectedOffice = sqlConnection.CreateCommand();
                    sqlCommandGetOccupiedElectedOffice.CommandText = sqlStatement.ToString();
                    sqlCommandGetOccupiedElectedOffice.CommandTimeout = 600;
                    sqlCommandGetOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@occupied_elected_office_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetOccupiedElectedOffice.Parameters["@occupied_elected_office_id"].Value = 0;
                    await sqlCommandGetOccupiedElectedOffice.PrepareAsync();

                    sqlCommandGetOccupiedElectedOffice.Parameters["@occupied_elected_office_id"].Value = occupiedElectedOfficeId;
                    using (sqlDataReaderGetOccupiedElectedOffice = await sqlCommandGetOccupiedElectedOffice.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetOccupiedElectedOffice.ReadAsync())
                        {

                            returnValue.occupiedElectedOfficeId = occupiedElectedOfficeId;
                            returnValue.territoryLevelId = sqlDataReaderGetOccupiedElectedOffice.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            returnValue.territoryLevelDescription = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_DESCRIPTION);
                            returnValue.territoryId = sqlDataReaderGetOccupiedElectedOffice.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            returnValue.territoryFullName = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_FULL_NAME);
                            returnValue.electedOfficeDescription = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            returnValue.distinctElectedOfficeForTerritoryId = sqlDataReaderGetOccupiedElectedOffice.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            returnValue.distinctOfficeDesignator = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DESIGNATOR);
                            returnValue.reasonForEntryId = sqlDataReaderGetOccupiedElectedOffice.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_ENTRY_ID);
                            returnValue.reasonForEntryDescription = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_ENTRY_DESCRIPTION);
                            returnValue.reasonForDepartureId = sqlDataReaderGetOccupiedElectedOffice.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_DEPARTURE_ID);
                            returnValue.reasonForDepartureDescription = sqlDataReaderGetOccupiedElectedOffice.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_DEPARTURE_DESCRIPTION);
                            returnValue.startDate = sqlDataReaderGetOccupiedElectedOffice.GetDateTime(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_START_DATE);
                            returnValue.endDate = sqlDataReaderGetOccupiedElectedOffice.GetDateTime(ApplicationValues.OCCUPIED_ELECTED_OFFICE_QUERY_RESULT_COLUMN_OFFSET_END_DATE);

                        };

                        await sqlDataReaderGetOccupiedElectedOffice.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in OccupiedElectedOfficeWSController::GetOccupiedElectedOffice().  Message is {0}", ex1.Message));

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

        }       // GetCandidateForElection()

        [Route("occupiedElectedOffice/forPerson/{personId}/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.OccupiedElectedOffice>>> GetOccupiedElectedOfficeForPerson(int personId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetOccupiedElectedOfficeForPerson;
            NpgsqlDataReader sqlDataReaderGetGetOccupiedElectedOfficeForPerson;

            try
            {

                List<Models.OccupiedElectedOffice> returnValue = new List<Models.OccupiedElectedOffice>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT oeo.occupied_elected_office_id, tl.territory_level_id, tl.reference_name, t.territory_id, t.full_name, eo.reference_name, ");
                    sqlStatement.Append("       deot.distinct_elected_office_for_territory_id, deot.distinct_office_designator, rfe.reason_for_entry_id,");
                    sqlStatement.Append("       rfe.description, rfd.reason_for_departure_id, rfd.description, oeo.start_date, oeo.end_date ");
                    sqlStatement.Append("  FROM occupied_elected_office oeo INNER JOIN distinct_elected_office_for_territory deot ");
                    sqlStatement.Append("                                              ON oeo.distinct_elected_office_for_territory_id = deot.distinct_elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN territory t ON deot.territory_id = t.territory_id ");
                    sqlStatement.Append("       INNER JOIN territory_level tl ON t.territory_level_id = tl.territory_level_id ");
                    sqlStatement.Append("       INNER JOIN elected_office_for_territory eot on deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("       INNER JOIN elected_office eo ON eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("       INNER JOIN reason_for_entry rfe oeo.reason_for_entry_id = rfe.reason_for_entry_id ");
                    sqlStatement.Append("       INNER JOIN reason_for_departure rfd oeo.reason_for_departure_id = rfe.reason_for_departure_id ");
                    sqlStatement.Append("  ORDER BY oeo.start_date DESC ");

                    sqlCommandGetOccupiedElectedOfficeForPerson = sqlConnection.CreateCommand();
                    sqlCommandGetOccupiedElectedOfficeForPerson.CommandText = sqlStatement.ToString();
                    sqlCommandGetOccupiedElectedOfficeForPerson.CommandTimeout = 600;

                    await sqlCommandGetOccupiedElectedOfficeForPerson.PrepareAsync();

                    using (sqlDataReaderGetGetOccupiedElectedOfficeForPerson = await sqlCommandGetOccupiedElectedOfficeForPerson.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetGetOccupiedElectedOfficeForPerson.ReadAsync())
                        {

                            Models.OccupiedElectedOffice occupiedElectedOffice = new Models.OccupiedElectedOffice();

                            occupiedElectedOffice.occupiedElectedOfficeId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_OCCUPIED_ELECTED_OFFICE_ID); ;
                            occupiedElectedOffice.territoryLevelId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            occupiedElectedOffice.territoryLevelDescription = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_DESCRIPTION);
                            occupiedElectedOffice.territoryId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            occupiedElectedOffice.territoryFullName = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_FULL_NAME);
                            occupiedElectedOffice.electedOfficeDescription = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            occupiedElectedOffice.distinctElectedOfficeForTerritoryId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            occupiedElectedOffice.distinctOfficeDesignator = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DESIGNATOR);
                            occupiedElectedOffice.reasonForEntryId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_ENTRY_ID);
                            occupiedElectedOffice.reasonForEntryDescription = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_ENTRY_DESCRIPTION);
                            occupiedElectedOffice.reasonForDepartureId = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetInt32(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_DEPARTURE_ID);
                            occupiedElectedOffice.reasonForDepartureDescription = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetString(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_DEPARTURE_DESCRIPTION);
                            occupiedElectedOffice.startDate = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetDateTime(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_START_DATE);
                            occupiedElectedOffice.endDate = sqlDataReaderGetGetOccupiedElectedOfficeForPerson.GetDateTime(ApplicationValues.OCCUPIED_ELECTED_OFFICE_LIST_QUERY_RESULT_COLUMN_OFFSET_END_DATE);

                            returnValue.Add(occupiedElectedOffice);

                        };

                        await sqlDataReaderGetGetOccupiedElectedOfficeForPerson.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in OccupiedElectedOfficeWSController::GetOccupiedElectedOfficeForPerson().  Message is {0}", ex1.Message));

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

        }       // GetOccupiedElectedOfficeForPerson()


        [Route("occupiedElectedOffice")]
        [HttpPost]
        public async Task<ActionResult<Models.OccupiedElectedOffice>> AddOccupiedElectedOffice([FromBody] Models.OccupiedElectedOffice occupiedElectedOffice)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandInsertOccupiedElectedOffice;

            try
            {

                Models.OccupiedElectedOffice returnValue = new Models.OccupiedElectedOffice();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("INSERT INTO occupied_elected_office ");
                    sqlStatement.Append("  (person_id, distinct_elected_office_for_territory_id, start_date, end_date, reason_for_entry_id, reason_for_departure_id ");
                    sqlStatement.Append("   record_added_date_time, record_last_updated_date_time) ");
                    sqlStatement.Append("  VALUES (@person_id, @distinct_elected_office_for_territory_id, @start_date, @end_date, @reason_for_entry_id, @reason_for_departure_id,");
                    sqlStatement.Append("          @record_added_date_time, @record_last_updated_date_time) RETURNING occupied_elected_office_id ");

                    sqlCommandInsertOccupiedElectedOffice = sqlConnection.CreateCommand();
                    sqlCommandInsertOccupiedElectedOffice.CommandText = sqlStatement.ToString();
                    sqlCommandInsertOccupiedElectedOffice.CommandTimeout = 600;
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@person_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@distinct_elected_office_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@start_date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@end_date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@reason_for_entry_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@reason_for_departure_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@record_added_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandInsertOccupiedElectedOffice.Parameters.Add(new NpgsqlParameter("@record_last_updated_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));

                    sqlCommandInsertOccupiedElectedOffice.Parameters["@person_id"].Value = 0;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@distinct_elected_office_for_territory_id"].Value = 0;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@start_date"].Value = DateTime.MinValue;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@end_date"].Value = DateTime.MinValue;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@reason_for_entry_id"].Value = 0;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@reason_for_departure_id"].Value = 0;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@record_added_date_time"].Value = DateTime.MinValue;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@record_last_updated_date_time"].Value = DateTime.MinValue;
                    await sqlCommandInsertOccupiedElectedOffice.PrepareAsync();

                    sqlCommandInsertOccupiedElectedOffice.Parameters["@person_id"].Value = occupiedElectedOffice.personId;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@distinct_elected_office_for_territory_id"].Value = occupiedElectedOffice.distinctElectedOfficeForTerritoryId;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@start_date"].Value = occupiedElectedOffice.startDate;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@end_date"].Value = occupiedElectedOffice.endDate;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@reason_for_entry_id"].Value = occupiedElectedOffice.reasonForEntryId;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@reason_for_departure_id"].Value = occupiedElectedOffice.reasonForDepartureId;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@record_added_date_time"].Value = processingDateTime;
                    sqlCommandInsertOccupiedElectedOffice.Parameters["@record_last_updated_date_time"].Value = processingDateTime;

                    using (var sqlDataReader = await sqlCommandInsertOccupiedElectedOffice.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        await sqlDataReader.ReadAsync();

                        returnValue.occupiedElectedOfficeId = sqlDataReader.GetInt32(0);
                        returnValue.personId = occupiedElectedOffice.personId;
                        returnValue.distinctElectedOfficeForTerritoryId = occupiedElectedOffice.distinctElectedOfficeForTerritoryId;
                        returnValue.startDate = occupiedElectedOffice.startDate;
                        returnValue.endDate = occupiedElectedOffice.endDate;
                        returnValue.reasonForEntryId = occupiedElectedOffice.reasonForEntryId;
                        returnValue.reasonForDepartureId = occupiedElectedOffice.reasonForDepartureId;
                        await sqlDataReader.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in OccupiedElectedOfficeWSController::AddOccupiedElectedOffice().  Message is {0}", ex1.Message));

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

        }       // AddOccupiedElectedOffice()

        /*
         *     occupied_elected_office_id integer NOT NULL,
            distinct_elected_office_for_territory_id integer NOT NULL,
            person_id integer NOT NULL,
            start_date date NOT NULL,
            end_date date,
            reason_for_departure_id integer,
            reason_for_entry_id integer NOT NULL,

         * */





    }
}
