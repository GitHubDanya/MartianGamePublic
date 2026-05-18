using ServerAlphaWebsite.Classes;
using System.Runtime.ConstrainedExecution;

namespace ServerAlphaWebsite.ServerStorage
{
    public class UserInfoStorage
    {
        public List<User> Users = new List<User>();

        public void LogUser(string name, string experimentId)
        {
            User user = new()
            {
                Username = name,
                Experiment = experimentId

            };

            if (GetUser(name) == null)
                Users.Add(user);
        }

        public void SetPID(string name, string prolificID)
        {
            User? user = GetUser(name);
            if (user != null) user.ProlificID = prolificID;
        }

        public string GetExperimentId(string name)
        {
            User? user = GetUser(name);
            if (user != null) return user.Experiment;
            return string.Empty;
        }

        public float GetScore(string name)
        {
            User? user = GetUser(name);
            if (user != null) return user.Score;
            return 0;
        }
        public void LoadRawScore(string name)
        {
            User? user = GetUser(name);
            if (user != null) user.Score = user.RawScore;
        }

        public void IncrementScore(string name, float score)
        {
            User? user = GetUser(name);
            if (user != null) user.Score += score;
        }

        public int GetQuestionNum(string name)
        {
            User? user = GetUser(name);
            if (user != null) return user.QuestionNum;
            return 0;
        }

        public void IncrementQuestionNum(string name)
        {
            User? user = GetUser(name);
            if (user != null) user.QuestionNum++;
        }

        public void SetScore(string name, float score)
        {
            User? user = GetUser(name);
            if (user != null)
            {
                user.RawScore = GetScore(name);
                user.Score = score;
            }
        }

        public void LogMessage(Message message, string name)
        {
            User? user = GetUser(name);
            if (user != null) user.ChatHistory.Add(message);
        }

        public void LogSolution(string solution, string name)
        {
            User? user = GetUser(name);
            if (user != null) user.Solution = solution;
        }

        public void LogImageUrl(string name, string imageUrl)
        {
            User? user = GetUser(name);
            if (user != null) user.ResultImageURL = imageUrl;
        }

        public List<Message> GetMessageHistory(string name)
        {
            User? user = GetUser(name);
            if (user != null) return user.ChatHistory;
            return new List<Message>();
        }

        public string GetSolution(string name)
        {
            User? user = GetUser(name);
            if (user != null) return user.Solution;
            return string.Empty;
        }

        public void DeleteUser(string name)
        {
            User? user = GetUser(name);
            if (user != null) Users.Remove(user);
        }

        public User? GetUser(string name)
        {
            return Users.Find(user => user.Username == name);
        }

        public void RemoveTry(string name)
        {
            User? user = GetUser(name);
            if (user != null && user.TriesLeft > 0) user.TriesLeft--;
        }
    }
}
