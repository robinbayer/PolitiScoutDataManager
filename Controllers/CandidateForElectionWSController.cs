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

                    /*
                     *         public int candidateForElectionId { get; set; }
                            public int territoryLevelId { get; set; }
                            public string territoryLevelDescription { get; set; }
                            public int territoryId { get; set; }
                            public string territoryDescription { get; set; }
                            public string electedOfficeDescription { get; set; }
                            public int distinctElectedOfficeForTerritoryId { get; set; }
                            public string distinctOfficeDesignator { get; set; }
                            public DateTime electionDate { get; set; }
                            public int politicalPartyId { get; set; }
                            public string politicalPartyDescription { get; set; }
                            public int resultOfCandidacyId { get; set; }
                            public int resultOfCandidacyDescription { get; set; }

                     * */

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

                    sqlCommandGetCandidateForElection.Parameters["@candidate_for_election_id"].Value = candidateForElectionId;
                    await sqlCommandGetCandidateForElection.PrepareAsync();

                    using (sqlDataReaderGetCandidateForElection = await sqlCommandGetCandidateForElection.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection))
                    {
                        if (await sqlDataReaderGetCandidateForElection.ReadAsync())
                        {
                            returnValue.personId = personId;
                            returnValue.lastName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_LAST_NAME);
                            returnValue.firstName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_FIRST_NAME);

                            if (!await sqlDataReaderGetPerson.IsDBNullAsync(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME))
                            {
                                returnValue.middleName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_MIDDLE_NAME);
                            }

                            if (!await sqlDataReaderGetPerson.IsDBNullAsync(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX))
                            {
                                returnValue.generationSuffix = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_GENERATION_SUFFIX);
                            }

                            returnValue.preferredFirstName = sqlDataReaderGetPerson.GetString(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_PREFERRED_FIRST_NAME);

                            if (!await sqlDataReaderGetPerson.IsDBNullAsync(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH))
                            {
                                returnValue.dateOfBirth = sqlDataReaderGetPerson.GetDateTime(ApplicationValues.PERSON_QUERY_RESULT_COLUMN_OFFSET_DATE_OF_BIRTH);
                            }

                        };

                        await sqlDataReaderGetPerson.CloseAsync();
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



        // Add

        // Update

        // Delete

    }
}
