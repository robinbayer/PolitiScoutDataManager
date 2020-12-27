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
    public class PersonWSController : ControllerBase
    {

        private ILogger<PersonWSController> logger;
        private IConfiguration configuration;

        public PersonWSController(IConfiguration configuration, ILogger<PersonWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }


        [Route("person/{personId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.Person>> GetPerson(int personId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetPerson;
            NpgsqlDataReader sqlDataReaderGetPerson;

            try
            {

                Models.Person returnValue = new Models.Person();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT p.last_name, p.first_name, p.middle_name, p.generation_suffix, p.preferred_first_name, p.date_of_birth ");
                    sqlStatement.Append("  FROM person p ");
                    sqlStatement.Append("  WHERE p.person_id = @person_id ");

                    sqlCommandGetPerson = sqlConnection.CreateCommand();
                    sqlCommandGetPerson.CommandText = sqlStatement.ToString();
                    sqlCommandGetPerson.CommandTimeout = 600;
                    sqlCommandGetPerson.Parameters.Add(new NpgsqlParameter("@person_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetPerson.Parameters["@person_id"].Value = personId;
                    await sqlCommandGetPerson.PrepareAsync();

                    using (sqlDataReaderGetPerson = await sqlCommandGetPerson.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetPerson.ReadAsync())
                        {
                            returnValue.personId = personId;
                            returnValue.lastName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME);
                            returnValue.firstName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME);
                            returnValue.middleName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME);
                            returnValue.generationSuffix = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX);
                            returnValue.preferredFirstName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME);
                            returnValue.dateOfBirth = sqlDataReaderGetPerson.GetDateTime(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH);
                        };

                        await sqlDataReaderGetPerson.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PersonWSController::GetPerson().  Message is {0}", ex1.Message));

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

        }       // GetPerson()

        [Route("person/search/")]
        [HttpPost]      // RJB 2020-12-26  Considered incorrect to have data in body of GET method 
        public async Task<ActionResult<List<Models.Person>>> SearchPerson([FromBody]Models.PersonSearchParameterSet personSearchParamterSet)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandSearchPerson;
            NpgsqlDataReader sqlDataReaderSearchPerson;

            try
            {

                List<Models.Person> returnValue = new List<Models.Person>();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT p.person_id, p.last_name, p.first_name, p.middle_name, p.generation_suffix, p.preferred_first_name, p.date_of_birth ");
                    sqlStatement.Append("  FROM person p ");
                    sqlStatement.Append("  WHERE UPPER(p.last_name) LIKE UPPER(@last_name_search_mask) AND UPPER(p.first_name) LIKE UPPER(@first_name_search_mask) AND ");
                    sqlStatement.Append("        UPPER(p.preferred_first_name) LIKE UPPER(@preferred_first_name_search_mask) ");
                    sqlStatement.Append("  ORDER BY p.last_name, p.first_name, p.preferred_first_name ");
                    sqlStatement.Append("  LIMIT @return_limit ");

                    sqlCommandSearchPerson = sqlConnection.CreateCommand();
                    sqlCommandSearchPerson.CommandText = sqlStatement.ToString();
                    sqlCommandSearchPerson.CommandTimeout = 600;
                    sqlCommandSearchPerson.Parameters.Add(new NpgsqlParameter("@last_name_search_mask", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandSearchPerson.Parameters.Add(new NpgsqlParameter("@first_name_search_mask", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandSearchPerson.Parameters.Add(new NpgsqlParameter("@preferred_first_name_search_mask", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandSearchPerson.Parameters.Add(new NpgsqlParameter("@return_limit", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandSearchPerson.Parameters["@last_name_search_mask"].Value = "";
                    sqlCommandSearchPerson.Parameters["@first_name_search_mask"].Value = "";
                    sqlCommandSearchPerson.Parameters["@preferred_first_name_search_mask"].Value = "";
                    sqlCommandSearchPerson.Parameters["@return_limit"].Value = 0;
                    await sqlCommandSearchPerson.PrepareAsync();

                    sqlCommandSearchPerson.Parameters["@last_name_search_mask"].Value = personSearchParamterSet.lastNameSearchMask;
                    sqlCommandSearchPerson.Parameters["@first_name_search_mask"].Value = personSearchParamterSet.firstNameSearchMask;
                    sqlCommandSearchPerson.Parameters["@preferred_first_name_search_mask"].Value = personSearchParamterSet.preferredFirstNameSearchMask;
                    sqlCommandSearchPerson.Parameters["@return_limit"].Value = int.Parse(configuration["AppSettings:SearchReturnLimit"]);

                    using (sqlDataReaderSearchPerson = await sqlCommandSearchPerson.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderSearchPerson.ReadAsync())
                        {

                            Models.Person person = new Models.Person();

                            person.personId = sqlDataReaderSearchPerson.GetInt32(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_PERSON_ID);
                            person.lastName = sqlDataReaderSearchPerson.GetString(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME);
                            person.firstName = sqlDataReaderSearchPerson.GetString(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME);

                            if (!await sqlDataReaderSearchPerson.IsDBNullAsync(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME))
                            {
                                person.middleName = sqlDataReaderSearchPerson.GetString(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME);
                            }

                            if (!await sqlDataReaderSearchPerson.IsDBNullAsync(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX))
                            {
                                person.generationSuffix = sqlDataReaderSearchPerson.GetString(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX);
                            }

                            if (!await sqlDataReaderSearchPerson.IsDBNullAsync(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME))
                            {
                                person.preferredFirstName = sqlDataReaderSearchPerson.GetString(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME);
                            }

                            if (!await sqlDataReaderSearchPerson.IsDBNullAsync(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH))
                            {
                                person.dateOfBirth = sqlDataReaderSearchPerson.GetDateTime(ApplicationValues.PERSON_SEARCH_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH);
                            }

                            returnValue.Add(person);

                        };

                        await sqlDataReaderSearchPerson.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PersonWSController::GetPerson().  Message is {0}", ex1.Message));

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

        }       // GetPerson()

        [Route("person")]
        [HttpPost]
        public async Task<ActionResult<Models.Person>> AddPerson([FromBody]Models.Person person)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandInsertPerson;

            try
            {

                Models.Person returnValue = new Models.Person();

                processingDateTime = System.DateTime.Now; 

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("INSERT INTO person ");
                    sqlStatement.Append("  (last_name, first_name, middle_name, generation_suffix, preferred_first_name, date_of_birth, ");
                    sqlStatement.Append("   record_added_date_time, record_last_updated_date_time) ");
                    sqlStatement.Append("  VALUES (@last_name, @first_name, @middle_name, @generation_suffix, @preferred_first_name, @date_of_birth, ");
                    sqlStatement.Append("          @record_added_date_time, @record_last_updated_date_time) RETURNING person_id ");

                    sqlCommandInsertPerson = sqlConnection.CreateCommand();
                    sqlCommandInsertPerson.CommandText = sqlStatement.ToString();
                    sqlCommandInsertPerson.CommandTimeout = 600;
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@last_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@first_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@middle_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@generation_suffix", NpgsqlTypes.NpgsqlDbType.Varchar, 10));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@preferred_first_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@date_of_birth", NpgsqlTypes.NpgsqlDbType.Date));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@record_added_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandInsertPerson.Parameters.Add(new NpgsqlParameter("@record_last_updated_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));

                    sqlCommandInsertPerson.Parameters["@last_name"].Value = person.lastName;
                    sqlCommandInsertPerson.Parameters["@first_name"].Value = person.firstName;
                    sqlCommandInsertPerson.Parameters["@middle_name"].Value = person.middleName;
                    sqlCommandInsertPerson.Parameters["@generation_suffix"].Value = person.generationSuffix;
                    sqlCommandInsertPerson.Parameters["@preferred_first_name"].Value = person.preferredFirstName;
                    sqlCommandInsertPerson.Parameters["@date_of_birth"].Value = person.dateOfBirth;
                    sqlCommandInsertPerson.Parameters["@record_added_date_time"].Value = DateTime.MinValue;
                    sqlCommandInsertPerson.Parameters["@record_last_updated_date_time"].Value = DateTime.MinValue;

                    using (var sqlDataReader = await sqlCommandInsertPerson.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        await sqlDataReader.ReadAsync();

                        returnValue.personId = sqlDataReader.GetInt32(0);
                        returnValue.lastName = person.lastName;
                        returnValue.firstName = person.firstName;
                        returnValue.middleName = person.middleName;
                        returnValue.generationSuffix = person.generationSuffix;
                        returnValue.preferredFirstName = person.preferredFirstName;
                        returnValue.dateOfBirth = person.dateOfBirth;

                        await sqlDataReader.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PersonWSController::AddPerson().  Message is {0}", ex1.Message));

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

        }       // AddPerson()

        [Route("person")]
        [HttpPut]
        public async Task<ActionResult<Models.APICallResult>> UpdatePerson([FromBody] Models.Person person)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandUpdatePerson;

            try
            {

                Models.APICallResult returnValue = new Models.APICallResult();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("UPDATE person ");
                    sqlStatement.Append("  SET last_name = @last_name, first_name = @first_name, middle_name = @middle_name, generation_suffix = @generation_suffix, ");
                    sqlStatement.Append("      preferred_first_name = @preferred_first_name, date_of_birth = @date_of_birth, ");
                    sqlStatement.Append("      record_last_updated_date_time = @record_last_updated_date_time) ");
                    sqlStatement.Append("  WHERE person_id = @person_id ");

                    sqlCommandUpdatePerson = sqlConnection.CreateCommand();
                    sqlCommandUpdatePerson.CommandText = sqlStatement.ToString();
                    sqlCommandUpdatePerson.CommandTimeout = 600;
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@last_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@first_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@middle_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@generation_suffix", NpgsqlTypes.NpgsqlDbType.Varchar, 10));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@preferred_first_name", NpgsqlTypes.NpgsqlDbType.Varchar, 50));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@date_of_birth", NpgsqlTypes.NpgsqlDbType.Date));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@record_last_updated_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandUpdatePerson.Parameters.Add(new NpgsqlParameter("@person_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandUpdatePerson.Parameters["@last_name"].Value = person.lastName;
                    sqlCommandUpdatePerson.Parameters["@first_name"].Value = person.firstName;
                    sqlCommandUpdatePerson.Parameters["@middle_name"].Value = person.middleName;
                    sqlCommandUpdatePerson.Parameters["@generation_suffix"].Value = person.generationSuffix;
                    sqlCommandUpdatePerson.Parameters["@preferred_first_name"].Value = person.preferredFirstName;
                    sqlCommandUpdatePerson.Parameters["@date_of_birth"].Value = person.dateOfBirth;
                    sqlCommandUpdatePerson.Parameters["@record_last_updated_date_time"].Value = DateTime.MinValue;
                    sqlCommandUpdatePerson.Parameters["@person_id"].Value = person.personId;

                    await sqlCommandUpdatePerson.ExecuteNonQueryAsync();

                    returnValue.resultCode = Models.APICallResult.RESULT_CODE_SUCCESS;

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PersonWSController::UpdatePerson().  Message is {0}", ex1.Message));

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

        }       // UpdatePerson()

        [Route("person/{personId}/")]
        [HttpDelete]
        public async Task<ActionResult<Models.APICallResult>> DeletePerson(int personId)
        {

            System.Text.StringBuilder sqlStatement;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandDeletePerson;

            try
            {

                Models.APICallResult returnValue = new Models.APICallResult();

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("DELETE FROM person ");
                    sqlStatement.Append("  WHERE person_id = @person_id ");

                    sqlCommandDeletePerson = sqlConnection.CreateCommand();
                    sqlCommandDeletePerson.CommandText = sqlStatement.ToString();
                    sqlCommandDeletePerson.CommandTimeout = 600;
                    sqlCommandDeletePerson.Parameters.Add(new NpgsqlParameter("@person_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandDeletePerson.Parameters["@person_id"].Value = 0;
                    await sqlCommandDeletePerson.PrepareAsync();

                    sqlCommandDeletePerson.Parameters["@person_id"].Value = personId;

                    await sqlCommandDeletePerson.ExecuteNonQueryAsync();

                    returnValue.resultCode = Models.APICallResult.RESULT_CODE_SUCCESS;

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in PersonWSController::DeletePerson().  Message is {0}", ex1.Message));

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

        }       // DeletePerson()


    }
}
