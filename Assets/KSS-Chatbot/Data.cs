using System;

public class Chat
{
    public int fromPersonID;
    public DateTime date;
    public float cellWidth = -1;
    public float cellHeight = -1;
    public string text;
}

public class RequestAIChat
{
    public RequestAIChatMessage[] messages;
}

public class RequestAIChatMessage
{
    public string role;
    public string content;
}

public class ResultAIChat
{
    public string output;
}