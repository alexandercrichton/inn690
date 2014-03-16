using System;
using System.Collections.Generic;
using AIMLbot;
using AimlBot = AIMLbot.Bot;

namespace Veis.Chat
{
    public class AimlChatProvider : ChatProvider
    {
        private Dictionary<string, User> _users; // The people chatting to this bot
        private readonly AimlBot _bot;
        private bool _chatActive;
        private readonly string _ownerFirstName;

        private const string ChatCommand = "CHAT";

        public AimlChatProvider(string ownerFirstName)
        {
            System.Console.WriteLine("[AILMCharProvider] CHAT:STARTLISTENING, CHAT:STOPLISTENING, CHAT:RELOAD_NLP are acceptable commands from client chat channel.");
            _bot = new AimlBot();
            _bot.loadSettings();
            _bot.isAcceptingUserInput = false;
            _bot.loadAIMLFromFiles();
            
            _bot.isAcceptingUserInput = true;

            _users = new Dictionary<string, User>();
            _chatActive = false;
            _ownerFirstName = ownerFirstName;
        }

        private void Print(String s)
        {
            System.Console.WriteLine("[AIMLChatProvider]" + s);
        }

        public override string ProcessChat(string message, string fromName, string fromId)
        {
            bool sayAnyway = _chatActive;
            string output = string.Empty;

            //Print("IN PROCESS CHAT");

            if (!_chatActive && !message.Contains(_ownerFirstName))
            {
                Print( _ownerFirstName + " is waiting for chating, say \"Hi " + _ownerFirstName + "\" to begin chat.");
                // The conversation has not started yet.
                // The bots only respond to basic politeness
                // This can be changed...
                _bot.Chat(new Request("", _users[fromId], _bot));
            }
            else
            {
                //Print("Here 456, " + fromId + " is needed.");
                // Start a new conversation if necessary
                if (!_users.ContainsKey(fromId))
                {
                    _users.Add(fromId, new User(fromId, _bot));
                    _bot.Chat(new Request("MY NAME IS " + fromName, _users[fromId], _bot));
                }

                Request req = new Request(message, _users[fromId], _bot);
                Result res = _bot.Chat(req);
                output = res.Output;

                while (output.Contains("#%") && output.Contains("%#"))
                {
                    int location = output.IndexOf("#%", StringComparison.Ordinal);
                    int end = output.IndexOf("%#", StringComparison.Ordinal);

                    string pre = output.Substring(0, location);
                    string post = output.Substring(end + 2);
                    string answer = output.Substring(location + 2, end - (location + 2));

                    if (answer.Split(':')[0] == ChatCommand)
                    {
                        if (answer.Split(':')[1] == "STARTLISTENING")
                        {
                            _chatActive = true;
                            output = pre + post;
                        }
                        else if (answer.Split(':')[1] == "STOPLISTENING")
                        {
                            _chatActive = false;
                            output = pre + post;
                        }
                        else if (answer.Split(':')[1] == "RELOAD_NLP")
                        {
                            //Reload
                            _bot.isAcceptingUserInput = false;
                            _bot.loadAIMLFromFiles();

                            _bot.isAcceptingUserInput = true;

                            output = post;
                        }
                    }

                    // Now handle the chat using all chat handlers
                    foreach (ChatHandler handler in chatHandlers)
                    {
                        // Currently if there are multiple handlers, only one of their outputs will be said
                        if (handler.CanHandleMessage(answer))
                            output = handler.HandleMessage(answer, pre, post); 
                    }
                }

                if (sayAnyway || _chatActive)
                {
                    return output;
                }
                            
            }

            return output;
        }
    }
}
