using Npgsql;
using ServerAlphaWebsite.Models.DTO;
using System.Data;
using Dapper;
using System.Security.Permissions;
using System;
using System.IO;
using System.Security.Cryptography;
using ServerAlphaWebsite.Models.DTOs;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;

namespace ServerAlphaWebsite.DB;

public class DbCommunicationProvider
{
    /*
	 * DbCommuncationProvider
	 * Class that allows easy communication with the database.
	 * 
	 * --- Values ---
	 * 
	 * CONNSTRING: Connection string to the database.
	 * 
	 * --- Constructor --
	 * 
	 * Assigns values to CONNSTRING depending on whether the debugger is attached or not.
	 * 
	 * --- Methods ---
	 * 
	 * InsertConversation: Insert a ConversationDto to the conversations table. Returns void.
	 * InsertAnswer: Insert an AnswerDto to the answers table. Returns void.
	 * InsertPersonalInfo: Insert a PersonalInfoDto to the personalinfo table. Returns void.
	 * FetchConversations: Fetch all conversations from the conversations table. Returns a list.
	 * FetchAnswers: Fetch all answers from the answers table. Returns a list.
	 * FetchPersonalInfo: Fetch all personal info from the personalinfo table. Returns a list.
	 * GetFileStream: Fetch data from enum SQLTable and return it as a stream. Returns a Stream.
	 */

    private string? CONNSTRING;

    public DbCommunicationProvider()
    {
        CONNSTRING = Environment.GetEnvironmentVariable("MARTIAN_DB_CONNSTRING");
    }

    public void InsertConversation(ConversationDto conversation)
    {
        try
        {
            if (CONNSTRING == null)
                return;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                List<ConversationDto> conversations = new List<ConversationDto> { conversation };
                connection.Execute("spconversations_add", conversations, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Npgsql.PostgresException pex)
        {
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void InsertAnswer(AnswerDto answer)
    {
        try
        {
            using (IDbConnection connection = new NpgsqlConnection(CONNSTRING))
            {
                List<AnswerDto> answers = new List<AnswerDto> { answer };
                connection.Execute("spanswers_add", answers, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Npgsql.PostgresException pex)
        {
            Console.WriteLine(pex.Data);
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }

    public void InsertPersonalInfo(PersonalInfoDto form)
    {
        try
        {
            if (CONNSTRING == null)
                return;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                List<PersonalInfoDto> forms = new List<PersonalInfoDto> { form };

                connection.Execute("sppersonalinfo_add", forms, commandType: CommandType.StoredProcedure);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }

    public List<dynamic>? FetchConversations()
    {
        try
        {
            if (CONNSTRING == null)
                return null;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                var result = connection.Query("SELECT * FROM conversations").ToList();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

	public List<ConversationDto>? FetchConversationDtos()
	{
		try
		{
			if (CONNSTRING == null)
				return null;

			using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
			{
				List<ConversationDto> result = connection.Query<ConversationDto>(@"
                SELECT
                    id AS p_id,
                    userid AS p_userid,
                    request AS p_request,
                    response AS p_response,
                    requesttime AS p_requesttime,
                    complexitylevel AS p_complexitylevel,
                    complexitylevelscore AS p_complexitylevelscore,
                    questiontype AS p_questiontype,
                    questiontypescore AS p_questiontypescore,
                    relevance AS p_relevance,
                    consistency AS p_consistency,
                    representativeness AS p_representativeness,
                    totalscore AS p_totalscore,
                    user_entry_order AS p_user_entry_order
                    FROM conversations;
                ").ToList();

				return result;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return null;
		}
	}

	public List<dynamic>? FetchAnswers()
    {
        try
        {
            if (CONNSTRING == null)
                return null;
            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                var result = connection.Query("SELECT * FROM answers").ToList();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public List<AnswerDto>? FetchAnswerDtos()
    {
        try
        {
            if (CONNSTRING == null)
                return null;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                List<AnswerDto> result = connection.Query<AnswerDto>(@"
                SELECT
                    id AS p_id,
                    userid AS p_userid,
                    answer AS p_answer,
                    submittime AS p_submittime,
                    solutioncategory AS p_solutioncategory,
                    coverage AS p_coverage,
                    coveragescore AS p_coveragescore,
                    answerkind AS p_answerkind,
                    triesleft AS p_triesleft,
                    totalscore AS p_totalscore,
                    dsi AS p_dsi,
                    distancefromoptimal AS p_distancefromoptimal
                FROM answers;
            ").ToList();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public List<dynamic>? FetchPersonalInfo()
    {
        try
        {
            if (CONNSTRING == null)
                return null;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                var result = connection.Query("SELECT * FROM personalinfo").ToList();

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public string FetchUserAnswer(string user)
    {
        try
        {
            if (CONNSTRING == null)
                return null;

            using (IDbConnection connection = new NpgsqlConnection(connectionString: CONNSTRING))
            {
                string query = "SELECT answer FROM answers WHERE userid = @userid";
                string result = connection.QueryFirstOrDefault<string>(query, new { userid = user });

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public Stream GetFileStream(SQLTable table)
    {
        using (var connection = new NpgsqlConnection(CONNSTRING))
        {
            string sql = $"SELECT * FROM \"{table.ToString().ToLower()}\"";
            using (var command = new NpgsqlCommand(sql, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    var memoryStream = new MemoryStream();
                    var writer = new StreamWriter(memoryStream, Encoding.UTF8);

                    bool firstRow = true;

                    while (reader.Read())
                    {
                        if (firstRow)
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var columnName = reader.GetName(i);
                                writer.Write("\"" + columnName.Replace("\"", "\"\"") + "\"");
                                if (i < reader.FieldCount - 1)
                                    writer.Write(",");
                            }
                            writer.WriteLine();
                            firstRow = false;
                        }

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnValue = reader.GetValue(i);
                            var columnText = columnValue.ToString();

                            writer.Write("\"" + columnText.Replace("\"", "\"\"") + "\"");
                            if (i < reader.FieldCount - 1)
                                writer.Write(",");
                        }
                        writer.WriteLine();
                    }

                    writer.Flush();
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }
        }
    }


}