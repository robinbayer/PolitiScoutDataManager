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
    public class DictionaryWSController : ControllerBase
    {

        private ILogger<DictionaryWSController> logger;
        private IConfiguration configuration;

        public DictionaryWSController(IConfiguration configuration, ILogger<DictionaryWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("dictionary/resultOfCandidacy/{resultOfCandidacyId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ResultOfCandidacy>> GetResultOfCandidacy(int resultOfCandidacyId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetResultOfCandidacy;
            NpgsqlDataReader sqlDataReaderGetResultOfCandidacy;

            try
            {

                Models.ResultOfCandidacy returnValue = new Models.ResultOfCandidacy();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.description ");
                    sqlStatement.Append("  FROM result_of_candidacy r ");
                    sqlStatement.Append("  WHERE r.result_of_candidacy_id = @result_of_candidacy_id ");

                    sqlCommandGetResultOfCandidacy = sqlConnection.CreateCommand();
                    sqlCommandGetResultOfCandidacy.CommandText = sqlStatement.ToString();
                    sqlCommandGetResultOfCandidacy.CommandTimeout = 600;
                    sqlCommandGetResultOfCandidacy.Parameters.Add(new NpgsqlParameter("@result_of_candidacy_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetResultOfCandidacy.Parameters["@result_of_candidacy_id"].Value = 0;
                    await sqlCommandGetResultOfCandidacy.PrepareAsync();

                    sqlCommandGetResultOfCandidacy.Parameters["@result_of_candidacy_id"].Value = resultOfCandidacyId;
                    using (sqlDataReaderGetResultOfCandidacy = await sqlCommandGetResultOfCandidacy.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetResultOfCandidacy.ReadAsync())
                        {
                            returnValue.resultOfCandidacyId = resultOfCandidacyId;
                            returnValue.description = sqlDataReaderGetResultOfCandidacy.GetString(ApplicationValues.RESULT_OF_CANDIDACY_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);
                        };

                        await sqlDataReaderGetResultOfCandidacy.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetResultOfCandidacy().  Message is {0}", ex1.Message));

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

        }       // GetResultOfCandidacy()


        [Route("dictionary/resultOfCandidacy/list/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.ResultOfCandidacy>>> GetResultOfCandidacyList()
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetResultOfCandidacyList;
            NpgsqlDataReader sqlDataReaderGetResultOfCandidacyList;

            try
            {

                List<Models.ResultOfCandidacy> returnValue = new List<Models.ResultOfCandidacy>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.result_of_candidacy_id, r.description ");
                    sqlStatement.Append("  FROM result_of_candidacy r ");
                    sqlStatement.Append("  ORDER BY r.description ");

                    sqlCommandGetResultOfCandidacyList = sqlConnection.CreateCommand();
                    sqlCommandGetResultOfCandidacyList.CommandText = sqlStatement.ToString();
                    sqlCommandGetResultOfCandidacyList.CommandTimeout = 600;
                    await sqlCommandGetResultOfCandidacyList.PrepareAsync();

                    using (sqlDataReaderGetResultOfCandidacyList = await sqlCommandGetResultOfCandidacyList.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetResultOfCandidacyList.ReadAsync())
                        {

                            Models.ResultOfCandidacy resultOfCandidacy = new Models.ResultOfCandidacy();

                            resultOfCandidacy.resultOfCandidacyId = sqlDataReaderGetResultOfCandidacyList.GetInt32(ApplicationValues.RESULT_OF_CANDIDACY_LIST_QUERY_RESULT_COLUMN_OFFSET_RESULT_OF_CANDIDACY_ID);
                            resultOfCandidacy.description = sqlDataReaderGetResultOfCandidacyList.GetString(ApplicationValues.RESULT_OF_CANDIDACY_LIST_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);

                            returnValue.Add(resultOfCandidacy);

                        };

                        await sqlDataReaderGetResultOfCandidacyList.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetResultOfCandidacyList().  Message is {0}", ex1.Message));

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

        }       // GetResultOfCandidacyList()


        [Route("dictionary/reasonForEntry/{reasonForEntryId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ReasonForEntry>> GetReasonForEntry(int reasonForEntryId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetReasonForEntry;
            NpgsqlDataReader sqlDataReaderGetReasonForEntry;

            try
            {

                Models.ReasonForEntry returnValue = new Models.ReasonForEntry();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.description ");
                    sqlStatement.Append("  FROM reason_for_entry r ");
                    sqlStatement.Append("  WHERE r.reason_for_entry_id = @reason_for_entry_id ");

                    sqlCommandGetReasonForEntry = sqlConnection.CreateCommand();
                    sqlCommandGetReasonForEntry.CommandText = sqlStatement.ToString();
                    sqlCommandGetReasonForEntry.CommandTimeout = 600;
                    sqlCommandGetReasonForEntry.Parameters.Add(new NpgsqlParameter("@reason_for_entry_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetReasonForEntry.Parameters["@reason_for_entry_id"].Value = 0;
                    await sqlCommandGetReasonForEntry.PrepareAsync();

                    sqlCommandGetReasonForEntry.Parameters["@reason_for_entry_id"].Value = reasonForEntryId;
                    using (sqlDataReaderGetReasonForEntry = await sqlCommandGetReasonForEntry.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetReasonForEntry.ReadAsync())
                        {
                            returnValue.reasonForEntryId = reasonForEntryId;
                            returnValue.description = sqlDataReaderGetReasonForEntry.GetString(ApplicationValues.REASON_FOR_ENTRY_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);
                        };

                        await sqlDataReaderGetReasonForEntry.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetReasonForEntry().  Message is {0}", ex1.Message));

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

        }       // GetReasonForEntry()


        [Route("dictionary/reasonForEntry/list/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.ReasonForEntry>>> GetReasonForEntryList()
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetResultOfCandidacyList;
            NpgsqlDataReader sqlDataReaderGetResultOfCandidacyList;

            try
            {

                List<Models.ReasonForEntry> returnValue = new List<Models.ReasonForEntry>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.reason_for_entry_id, r.description ");
                    sqlStatement.Append("  FROM reason_for_entry r ");
                    sqlStatement.Append("  ORDER BY r.description ");

                    sqlCommandGetResultOfCandidacyList = sqlConnection.CreateCommand();
                    sqlCommandGetResultOfCandidacyList.CommandText = sqlStatement.ToString();
                    sqlCommandGetResultOfCandidacyList.CommandTimeout = 600;
                    await sqlCommandGetResultOfCandidacyList.PrepareAsync();

                    using (sqlDataReaderGetResultOfCandidacyList = await sqlCommandGetResultOfCandidacyList.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetResultOfCandidacyList.ReadAsync())
                        {

                            Models.ReasonForEntry reasonForEntry = new Models.ReasonForEntry();

                            reasonForEntry.reasonForEntryId = sqlDataReaderGetResultOfCandidacyList.GetInt32(ApplicationValues.REASON_FOR_ENTRY_LIST_QUERY_RESULT_COLUMN_OFFSET_REASON_FOR_ENTRY_ID);
                            reasonForEntry.description = sqlDataReaderGetResultOfCandidacyList.GetString(ApplicationValues.REASON_FOR_ENTRY_LIST_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);

                            returnValue.Add(reasonForEntry);

                        };

                        await sqlDataReaderGetResultOfCandidacyList.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetReasonForEntryList().  Message is {0}", ex1.Message));

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

        }       // GetReasonForEntryList()


        [Route("dictionary/reasonForDeparture/{reasonForDepartureId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.ReasonForDeparture>> GetReasonForDeparture(int reasonForDepartureId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetReasonForDeparture;
            NpgsqlDataReader sqlDataReaderGetReasonForDeparture;

            try
            {

                Models.ReasonForDeparture returnValue = new Models.ReasonForDeparture();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.description ");
                    sqlStatement.Append("  FROM reason_for_departure r ");
                    sqlStatement.Append("  WHERE r.reason_for_departure_id = @reason_for_departure_id ");

                    sqlCommandGetReasonForDeparture = sqlConnection.CreateCommand();
                    sqlCommandGetReasonForDeparture.CommandText = sqlStatement.ToString();
                    sqlCommandGetReasonForDeparture.CommandTimeout = 600;
                    sqlCommandGetReasonForDeparture.Parameters.Add(new NpgsqlParameter("@reason_for_departure_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetReasonForDeparture.Parameters["@reason_for_departure_id"].Value = 0;
                    await sqlCommandGetReasonForDeparture.PrepareAsync();

                    sqlCommandGetReasonForDeparture.Parameters["@reason_for_departure_id"].Value = reasonForDepartureId;
                    using (sqlDataReaderGetReasonForDeparture = await sqlCommandGetReasonForDeparture.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetReasonForDeparture.ReadAsync())
                        {
                            returnValue.reasonForDepartureId = reasonForDepartureId;
                            returnValue.description = sqlDataReaderGetReasonForDeparture.GetString(ApplicationValues.reason_for_departure_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);
                        };

                        await sqlDataReaderGetReasonForDeparture.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetReasonForDeparture().  Message is {0}", ex1.Message));

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

        }       // GetReasonForDeparture()


        [Route("dictionary/reasonForDeparture/list/")]
        [HttpGet]
        public async Task<ActionResult<List<Models.ReasonForDeparture>>> GetReasonForDepartureList()
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetResultOfCandidacyList;
            NpgsqlDataReader sqlDataReaderGetResultOfCandidacyList;

            try
            {

                List<Models.ReasonForDeparture> returnValue = new List<Models.ReasonForDeparture>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT r.reason_for_departure_id, r.description ");
                    sqlStatement.Append("  FROM reason_for_departure r ");
                    sqlStatement.Append("  ORDER BY r.description ");

                    sqlCommandGetResultOfCandidacyList = sqlConnection.CreateCommand();
                    sqlCommandGetResultOfCandidacyList.CommandText = sqlStatement.ToString();
                    sqlCommandGetResultOfCandidacyList.CommandTimeout = 600;
                    await sqlCommandGetResultOfCandidacyList.PrepareAsync();

                    using (sqlDataReaderGetResultOfCandidacyList = await sqlCommandGetResultOfCandidacyList.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetResultOfCandidacyList.ReadAsync())
                        {

                            Models.ReasonForDeparture reasonForDeparture = new Models.ReasonForDeparture();

                            reasonForDeparture.reasonForDepartureId = sqlDataReaderGetResultOfCandidacyList.GetInt32(ApplicationValues.reason_for_departure_LIST_QUERY_RESULT_COLUMN_OFFSET_reason_for_departure_ID);
                            reasonForDeparture.description = sqlDataReaderGetResultOfCandidacyList.GetString(ApplicationValues.reason_for_departure_LIST_QUERY_RESULT_COLUMN_OFFSET_DESCRIPTION);

                            returnValue.Add(reasonForDeparture);

                        };

                        await sqlDataReaderGetResultOfCandidacyList.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in DictionaryWSController::GetReasonForDepartureList().  Message is {0}", ex1.Message));

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

        }       // GetReasonForDepartureList()

    }
}
