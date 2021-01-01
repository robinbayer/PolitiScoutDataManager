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
    public class CandidateForElectionWSController : ControllerBase
    {

        private ILogger<CandidateForElectionWSController> logger;
        private IConfiguration configuration;

        public CandidateForElectionWSController(IConfiguration configuration, ILogger<CandidateForElectionWSController> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [Route("candidateForElection/{candidateForElectionId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.CandidateForElection>> GetCandidateForElection(int candidateForElectionId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetCandidateForElection;
            NpgsqlDataReader sqlDataReaderGetCandidateForElection;

            try
            {

                Models.CandidateForElection returnValue = new Models.CandidateForElection();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT tl.territory_level_id, tl.reference_name, t.territory_id, t.full_name, eo.reference_name, deot.distinct_elected_office_for_territory_id, ");
                    sqlStatement.Append("       deot.distinct_office_designator, elft.election_date, p.political_party_id, p.reference_name, roc.result_of_candidacy_id, roc.description ");
                    sqlStatement.Append("  FROM candidate_for_election cfe INNER JOIN distinct_elected_office_for_territory deot ");
                    sqlStatement.Append("                                             ON cfe.distinct_elected_office_for_territory_id = deot.distinct_elected_office_for_territory_id ");
                    sqlStatement.Append("                                  INNER JOIN elected_office_for_territory eot ");
                    sqlStatement.Append("                                             ON deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("                                  INNER JOIN elected_office eo ON eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("                                  INNER JOIN territory t ON eot.territory_id = t.territory_id ");
                    sqlStatement.Append("                                  INNER JOIN territory_level tl ON t.territory_level_id = t.territory_level_id ");
                    sqlStatement.Append("                                  INNER JOIN political_party p ON cfe.political_party_id = p.political_party_id ");
                    sqlStatement.Append("                                  INNER JOIN result_of_candidacy roc ON cfe.result_of_candidacy_id = roc.result_of_candidacy_id ");
                    sqlStatement.Append("                                  INNER JOIN election_for_territory elft on cfe.election_for_territory_id = elft.election_for_territory_id ");
                    sqlStatement.Append("  WHERE cfe.candidate_for_election_id = @candidate_for_election_id ");

                    sqlCommandGetCandidateForElection = sqlConnection.CreateCommand();
                    sqlCommandGetCandidateForElection.CommandText = sqlStatement.ToString();
                    sqlCommandGetCandidateForElection.CommandTimeout = 600;
                    sqlCommandGetCandidateForElection.Parameters.Add(new NpgsqlParameter("@candidate_for_election_id", NpgsqlTypes.NpgsqlDbType.Integer));

                    sqlCommandGetCandidateForElection.Parameters["@candidate_for_election_id"].Value = 0;
                    await sqlCommandGetCandidateForElection.PrepareAsync();

                    sqlCommandGetCandidateForElection.Parameters["@candidate_for_election_id"].Value = candidateForElectionId;
                    using (sqlDataReaderGetCandidateForElection = await sqlCommandGetCandidateForElection.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetCandidateForElection.ReadAsync())
                        {

                            returnValue.candidateForElectionId = candidateForElectionId;
                            returnValue.territoryLevelId = sqlDataReaderGetCandidateForElection.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            returnValue.territoryLevelDescription = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_DESCRIPTION);
                            returnValue.territoryId = sqlDataReaderGetCandidateForElection.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            returnValue.territoryFullName = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_FULL_NAME);
                            returnValue.electedOfficeDescription = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            returnValue.distinctElectedOfficeForTerritoryId = sqlDataReaderGetCandidateForElection.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            returnValue.distinctOfficeDesignator = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DESIGNATOR);
                            returnValue.electionDate = sqlDataReaderGetCandidateForElection.GetDateTime(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_ELECTION_DATE);
                            returnValue.politicalPartyId = sqlDataReaderGetCandidateForElection.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_ID);
                            returnValue.politicalPartyReferenceName = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_REFERENCE_NAME);
                            returnValue.resultOfCandidacyId = sqlDataReaderGetCandidateForElection.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_RESULT_OF_CANDIDACY_ID);
                            returnValue.resultOfCandidacyDescription = sqlDataReaderGetCandidateForElection.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_QUERY_RESULT_COLUMN_OFFSET_RESULT_OF_CANDIDACY_DESCRIPTION);

                        };

                        await sqlDataReaderGetCandidateForElection.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in CandidateForElectionWSController::GetCandidateForElection().  Message is {0}", ex1.Message));

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

        [Route("candidateForElection/forPerson/{personId}/")]
        [HttpGet]
        public async Task<ActionResult<Models.CandidateForElection>> GetCandidateForElectionForPerson(int personId)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandGetCandidateForElectionForPerson;
            NpgsqlDataReader sqlDataReaderGetCandidateForElectionForPerson;

            try
            {

                Models.CandidateForElection returnValue = new Models.CandidateForElection();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("SELECT cfe.candidate_for_election_id, tl.territory_level_id, tl.reference_name, t.territory_id, t.full_name, eo.reference_name, ");
                    sqlStatement.Append("       deot.distinct_elected_office_for_territory_id, deot.distinct_office_designator, elft.election_date, p.political_party_id, ");
                    sqlStatement.Append("       p.reference_name, roc.result_of_candidacy_id, roc.description ");
                    sqlStatement.Append("  FROM candidate_for_election cfe INNER JOIN distinct_elected_office_for_territory deot ");
                    sqlStatement.Append("                                             ON cfe.distinct_elected_office_for_territory_id = deot.distinct_elected_office_for_territory_id ");
                    sqlStatement.Append("                                  INNER JOIN elected_office_for_territory eot ");
                    sqlStatement.Append("                                             ON deot.elected_office_for_territory_id = eot.elected_office_for_territory_id ");
                    sqlStatement.Append("                                  INNER JOIN elected_office eo ON eot.elected_office_id = eo.elected_office_id ");
                    sqlStatement.Append("                                  INNER JOIN territory t ON eot.territory_id = t.territory_id ");
                    sqlStatement.Append("                                  INNER JOIN territory_level tl ON t.territory_level_id = t.territory_level_id ");
                    sqlStatement.Append("                                  INNER JOIN political_party p ON cfe.political_party_id = p.political_party_id ");
                    sqlStatement.Append("                                  INNER JOIN result_of_candidacy roc ON cfe.result_of_candidacy_id = roc.result_of_candidacy_id ");
                    sqlStatement.Append("                                  INNER JOIN election_for_territory elft on cfe.election_for_territory_id = elft.election_for_territory_id ");
                    sqlStatement.Append("  ORDER BY elft.election_date DESC ");

                    sqlCommandGetCandidateForElectionForPerson = sqlConnection.CreateCommand();
                    sqlCommandGetCandidateForElectionForPerson.CommandText = sqlStatement.ToString();
                    sqlCommandGetCandidateForElectionForPerson.CommandTimeout = 600;

                    await sqlCommandGetCandidateForElectionForPerson.PrepareAsync();

                    using (sqlDataReaderGetCandidateForElectionForPerson = await sqlCommandGetCandidateForElectionForPerson.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        while (await sqlDataReaderGetCandidateForElectionForPerson.ReadAsync())
                        {

                            returnValue.candidateForElectionId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_CANDIDATE_FOR_ELECTION_ID); ;
                            returnValue.territoryLevelId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_ID);
                            returnValue.territoryLevelDescription = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_LEVEL_DESCRIPTION);
                            returnValue.territoryId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_ID);
                            returnValue.territoryFullName = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_TERRITORY_FULL_NAME);
                            returnValue.electedOfficeDescription = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTED_OFFICE_REFERENCE_NAME);
                            returnValue.distinctElectedOfficeForTerritoryId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_ELECTED_OFFICE_FOR_TERRITORY_ID);
                            returnValue.distinctOfficeDesignator = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_DISTINCT_OFFICE_DESIGNATOR);
                            returnValue.electionDate = sqlDataReaderGetCandidateForElectionForPerson.GetDateTime(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_ELECTION_DATE);
                            returnValue.politicalPartyId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_ID);
                            returnValue.politicalPartyReferenceName = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_POLITICAL_PARTY_REFERENCE_NAME);
                            returnValue.resultOfCandidacyId = sqlDataReaderGetCandidateForElectionForPerson.GetInt32(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_RESULT_OF_CANDIDACY_ID);
                            returnValue.resultOfCandidacyDescription = sqlDataReaderGetCandidateForElectionForPerson.GetString(ApplicationValues.CANDIDATE_FOR_ELECTION_LIST_QUERY_RESULT_COLUMN_OFFSET_RESULT_OF_CANDIDACY_DESCRIPTION);

                        };

                        await sqlDataReaderGetCandidateForElectionForPerson.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in CandidateForElectionWSController::GetCandidateForElectionForPerson().  Message is {0}", ex1.Message));

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

        }       // GetCandidateForElectionForPerson()

        [Route("candidateForElection")]
        [HttpPost]
        public async Task<ActionResult<Models.CandidateForElection>> AddCandidateForElection([FromBody] Models.CandidateForElection candidateForElection)
        {

            System.Text.StringBuilder sqlStatement;
            DateTime processingDateTime;

            NpgsqlConnection sqlConnection;
            NpgsqlCommand sqlCommandInsertCandidateForElection;

            try
            {

                Models.CandidateForElection returnValue = new Models.CandidateForElection();

                processingDateTime = System.DateTime.Now;

                using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))
                {
                    await sqlConnection.OpenAsync();

                    sqlStatement = new System.Text.StringBuilder();
                    sqlStatement.Append("INSERT INTO candidate_for_election ");
                    sqlStatement.Append("  (person_id, election_for_territory_id, distinct_elected_office_for_territory_id, political_party_id, ");
                    sqlStatement.Append("   record_added_date_time, record_last_updated_date_time) ");
                    sqlStatement.Append("  VALUES (@person_id, @election_for_territory_id, @distinct_elected_office_for_territory_id, @political_party_id,");
                    sqlStatement.Append("          @record_added_date_time, @record_last_updated_date_time) RETURNING candidate_for_election_id ");

                    sqlCommandInsertCandidateForElection = sqlConnection.CreateCommand();
                    sqlCommandInsertCandidateForElection.CommandText = sqlStatement.ToString();
                    sqlCommandInsertCandidateForElection.CommandTimeout = 600;
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@person_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@election_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@distinct_elected_office_for_territory_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@political_party_id", NpgsqlTypes.NpgsqlDbType.Integer));
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@record_added_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));
                    sqlCommandInsertCandidateForElection.Parameters.Add(new NpgsqlParameter("@record_last_updated_date_time", NpgsqlTypes.NpgsqlDbType.Timestamp));

                    sqlCommandInsertCandidateForElection.Parameters["@person_id"].Value = 0;
                    sqlCommandInsertCandidateForElection.Parameters["@election_for_territory_id"].Value = 0;
                    sqlCommandInsertCandidateForElection.Parameters["@distinct_elected_office_for_territory_id"].Value = 0;
                    sqlCommandInsertCandidateForElection.Parameters["@political_party_id"].Value = 0;
                    sqlCommandInsertCandidateForElection.Parameters["@record_added_date_time"].Value = DateTime.MinValue;
                    sqlCommandInsertCandidateForElection.Parameters["@record_last_updated_date_time"].Value = DateTime.MinValue;
                    await sqlCommandInsertCandidateForElection.PrepareAsync();

                    sqlCommandInsertCandidateForElection.Parameters["@person_id"].Value = candidateForElection.personId;
                    sqlCommandInsertCandidateForElection.Parameters["@election_for_territory_id"].Value = candidateForElection.electionForTerritoryId;
                    sqlCommandInsertCandidateForElection.Parameters["@distinct_elected_office_for_territory_id"].Value = candidateForElection.distinctElectedOfficeForTerritoryId;
                    sqlCommandInsertCandidateForElection.Parameters["@political_party_id"].Value = candidateForElection.politicalPartyId;
                    sqlCommandInsertCandidateForElection.Parameters["@record_added_date_time"].Value = processingDateTime;
                    sqlCommandInsertCandidateForElection.Parameters["@record_last_updated_date_time"].Value = processingDateTime;

                    using (var sqlDataReader = await sqlCommandInsertCandidateForElection.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        await sqlDataReader.ReadAsync();

                        returnValue.candidateForElectionId = sqlDataReader.GetInt32(0);
                        returnValue.personId = candidateForElection.personId;
                        returnValue.distinctElectedOfficeForTerritoryId = candidateForElection.distinctElectedOfficeForTerritoryId;
                        returnValue.electionForTerritoryId = candidateForElection.electionForTerritoryId;
                        returnValue.politicalPartyId = candidateForElection.politicalPartyId;

                        await sqlDataReader.CloseAsync();
                    };

                    await sqlConnection.CloseAsync();
                }       // using (sqlConnection = new NpgsqlConnection(configuration["ConnectionStrings:PolitiScout"]))

                return Ok(returnValue);

            }
            catch (Exception ex1)
            {
                logger.LogError(string.Format("Unhandled exception occurred in CandidateForElectionWSController::AddCandidateForElection().  Message is {0}", ex1.Message));

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

        }       // AddCandidateForElection()


        // Update


        /*
         *     candidate_for_election_id integer NOT NULL,
            person_id integer NOT NULL,
            election_for_territory_id integer NOT NULL,
            distinct_elected_office_for_territory_id integer NOT NULL,
            political_party_id integer NOT NULL,

         * */

        // Delete

    }
}
