namespace ServerAlphaWebsite
{
    public enum MessageSender
    {
        User = 0,
        GPT = 1,
        Admin = 2,
        Unknown = 3
    }

    public enum RequestType
    {
        Text = 0,
        Image = 1
    }

    public enum SQLTable
    {
        conversations,
        answers,
        personalinfo
    }
    public enum GameStage
    {
        MainMenu,
        Disclaimer,
        Questionnaire,
        Game,
        Solution,
        Finish,
        Thanks
    }
    public enum AnswerKind
    {
        unknown,
        regular,
        botassisted,
        botgenerated
    }

    public enum CookieEnum
    {
        stage,
        culture,
        username,
        experimentid,
        loggedIn,
    }

    public enum DashboardPage
    {
        Dashboard,
        DownloadData,
        DeleteData
    }
}
