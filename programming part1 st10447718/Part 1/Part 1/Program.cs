﻿using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityBot
{

    internal class Program
    {


        private static string currentTopic = "";
        private static int followUpCount = 0;
        private static List<string> recentTopics = new List<string>();
        private static Random random = new Random();
        private static string userName = "";
        private static List<string> userInterests = new List<string>();
        private static Dictionary<string, string> userDetails = new Dictionary<string, string>();

        // Dictionaries for keyword responses
        private static Dictionary<string, string> topicResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"password safety", "Always use strong, unique passwords for each account. Consider using a password manager to securely store your passwords."},
            {"two-step authentication", "Two-Step Authentication (2FA) adds an extra layer of security by requiring a second form of verification, like a code sent to your phone."},
            {"2fa", "Two-Step Authentication (2FA) adds an extra layer of security by requiring a second form of verification, like a code sent to your phone."},
            {"social engineering", "Social engineering attacks trick users into revealing confidential information. Always be cautious with unsolicited requests and verify identities."},
            {"data breach", "A data breach can expose sensitive information. Use strong passwords and monitor your accounts regularly for suspicious activity."},
            {"phishing", "Phishing is a type of scam where attackers try to trick you into revealing sensitive information, like passwords or credit card numbers. Always verify the sender's identity before clicking on links."},
            {"malware", "Malware refers to malicious software like viruses or spyware. Protect yourself by keeping your software updated and using reliable antivirus software."},
            {"encryption", "Encryption protects data by encoding it so only authorized parties can read it. Always use encrypted connections and storage where possible."},
            {"vpn", "A VPN (Virtual Private Network) helps secure your internet connection by encrypting your data and masking your IP address."},
            {"firewall", "Firewalls monitor and control incoming and outgoing network traffic to protect your devices from unauthorized access."},
            {"backup", "Regular backups are essential to recover your data in case of hardware failure, cyberattacks, or accidental deletion."},
            {"data backup", "Regular backups are essential to recover your data in case of hardware failure, cyberattacks, or accidental deletion."},
            {"scam", "Be cautious of scams, especially phishing attempts. Always verify the sender's identity before clicking on links or providing personal information."},
            {"privacy", "To protect your privacy online, be mindful of the information you share on social media and use privacy settings to control who can see your data."}
        };

        private static Dictionary<string, string> exampleResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"password safety", "WEAK PASSWORD EXAMPLES:\n- password123\n- 123456\n- qwerty\n- yourname1980\n\nSTRONG EXAMPLES:\n- B1ue\\$ky!Runn3r\n- 7Dolphins@Moon\n- C@tL0v3r42!"},
            {"2fa", "EXAMPLE 2FA SCENARIO:\n1. You enter your password\n2. A code is sent to your phone\n3. You enter the code\n4. Only then are you logged in\n\nEven if someone steals your password, they can't login without the code!"},
            {"two-step authentication", "EXAMPLE 2FA SCENARIO:\n1. You enter your password\n2. A code is sent to your phone\n3. You enter the code\n4. Only then are you logged in\n\nEven if someone steals your password, they can't login without the code!"},
            {"phishing", "EXAMPLE PHISHING EMAIL:\n\nSubject: Urgent - Your Account Suspension\n\nDear User,\n\nWe've detected suspicious activity. Verify your account now to avoid suspension:\nhttp://fake-bank-login.com\n\n[This is fake - the link goes to a scam site]"},
            {"malware", "EXAMPLE MALWARE SCENARIO:\nYou download a free game from an untrusted site\nIt secretly installs a keylogger\nAll your keystrokes (including passwords) are recorded\nHackers now have access to your accounts"},
            {"social engineering", "EXAMPLE SOCIAL ENGINEERING ATTACK:\n1. Caller claims to be from IT support\n2. Says they need your password to fix an 'urgent issue'\n3. Once you provide it, they access your accounts\n\nAlways verify such requests through official channels!"}
        };

        private static Dictionary<string, List<string>> followUpResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            {"phishing", new List<string> {
                "Common signs of phishing include:\n- Urgent or threatening language\n- Misspelled URLs or email addresses\n- Requests for sensitive information\n- Generic greetings like 'Dear Customer'\nWould you like to see an example?",
                "Example phishing email:\n'Urgent! Your account will be suspended. Click here to verify your details.'\n\nAlways hover over links to check the real URL before clicking.",
                "You can report phishing attempts to:\n1. Your email provider\n2. The Anti-Phishing Working Group (reportphishing@apwg.org)\n3. The FTC at reportfraud.ftc.gov\nShould we discuss how to protect yourself from phishing?"
            }},
            {"password safety", new List<string> {
                "A strong password should:\n- Be at least 12 characters\n- Include uppercase, lowercase, numbers, and symbols\n- Avoid personal information\n- Be unique for each account\nWant to know about password managers?",
                "Password managers can:\n- Generate strong passwords\n- Store them securely\n- Auto-fill them for you\n\nPopular options: LastPass, 1Password, Bitwarden\nWould you like installation tips?"
            }},
            {"passwords", new List<string> {
                "A strong password should:\n- Be at least 12 characters\n- Include uppercase, lowercase, numbers, and symbols\n- Avoid personal information\n- Be unique for each account\nWant to know about password managers?",
                "Password managers can:\n- Generate strong passwords\n- Store them securely\n- Auto-fill them for you\n\nPopular options: LastPass, 1Password, Bitwarden\nWould you like installation tips?"
            }},
            {"malware", new List<string> {
                "Common malware types:\n- Viruses (spread by infecting files)\n- Ransomware (locks your files)\n- Spyware (steals information)\n- Trojans (disguised as legitimate software)\nNeed prevention tips?"
            }},
            {"privacy", new List<string> {
                "Privacy protection tips:\n1. Review app permissions regularly\n2. Use private browsing when needed\n3. Check privacy settings on social media\n4. Be cautious with location services\nWould you like more detailed advice?"
            }}
        };

        private static List<string[]> mythsAndFacts = new List<string[]>
        {
            new string[] { "Myth: Using a simple password is enough.", "Fact: Strong, complex passwords are essential for security." },
            new string[] { "Myth: Antivirus software is all you need.", "Fact: Regular updates and safe browsing habits are also crucial." },
            new string[] { "Myth: Public Wi-Fi is safe to use without precautions.", "Fact: Public Wi-Fi can expose you to security risks; use a VPN." },
            new string[] { "Myth: Cybersecurity is only a concern for large companies.", "Fact: Individuals are often targeted by cybercriminals as well." },
            new string[] { "Myth: You can always trust emails from known contacts.", "Fact: Email accounts can be compromised; verify unexpected requests." }
        };

        private static Dictionary<string, ConsoleColor> topicColors = new Dictionary<string, ConsoleColor>(StringComparer.OrdinalIgnoreCase)
        {
            {"password safety", ConsoleColor.Red},
            {"2fa", ConsoleColor.Yellow},
            {"two-step authentication", ConsoleColor.Yellow},
            {"social engineering", ConsoleColor.Magenta},
            {"data breach", ConsoleColor.Red},
            {"phishing", ConsoleColor.Magenta},
            {"malware", ConsoleColor.Red},
            {"encryption", ConsoleColor.Cyan},
            {"vpn", ConsoleColor.Green},
            {"firewall", ConsoleColor.Yellow},
            {"backup", ConsoleColor.Green},
            {"data backup", ConsoleColor.Green},
            {"scam", ConsoleColor.Magenta},
            {"privacy", ConsoleColor.Cyan}
        };

        static void Main()
        {
            try
            {
                Thread displayAsciiThread = new Thread(DisplayCyberDroidAscii);
                displayAsciiThread.Start();

                SpeechSynthesizer synth = new SpeechSynthesizer
                {
                    Volume = 100,
                    Rate = 0
                };

                synth.Speak("Hello User! I am Cyber Droid, your friendly Cyber Security Awareness Bot, and I am here to help you stay safe online!");

                GetUserName();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"\nHello, {userName}! That's a cool name! Welcome to the cybersecurity awareness bot.");
                Console.ResetColor();

                Console.WriteLine("Feel free to ask me any cybersecurity questions! To exit, type 'exit' at any time.");

                ProcessUserInput(synth);

                displayAsciiThread.Join();
                string goodbye = "Goodbye! Stay safe online.";
                Console.WriteLine(goodbye);
                synth.Speak(goodbye);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"A critical error occurred: {ex.Message}");
                Console.WriteLine("The program will now exit. Please restart the application.");
                Console.ResetColor();
            }
        }

        static void GetUserName()
        {
            do
            {
                Console.Write("Please Enter your name to start: ");
                userName = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(userName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter your name to continue.");
                    Console.ResetColor();
                }
            } while (string.IsNullOrEmpty(userName));

            userDetails["name"] = userName;
        }

        static void ProcessUserInput(SpeechSynthesizer synth)
        {
            while (true)
            {
                try
                {
                    Console.Write("You: ");
                    string userInput = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(userInput))
                    {
                        HandleEmptyInput();
                        continue;
                    }

                    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Goodbye! Stay safe online.");
                        Console.ResetColor();
                        break;
                    }

                    Console.Write("Cyber Droid: ");
                    string response = GetBasicResponse(userInput);
                    SimulateTypingEffect(response);
                    Console.WriteLine("\n");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("I encountered an issue processing your request. Please try again.");
                    Console.ResetColor();
                    continue;
                }
            }
        }

        static void HandleEmptyInput()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (!string.IsNullOrEmpty(currentTopic))
            {
                Console.WriteLine($"Cyber Droid: I'm still discussing {currentTopic}. Did you want to continue or switch topics?");
            }
            else
            {
                Console.WriteLine("Cyber Droid: I didn't hear anything. You can ask about cybersecurity topics or type 'exit' to leave.");
            }
            Console.ResetColor();
        }

        static string DetectSentiment(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) return "neutral";

                input = input.ToLower();

                var negativeSentiments = new Dictionary<string, string[]>
                {
                    {"worried", new[] {"worried", "concerned", "anxious", "nervous", "uneasy", "apprehensive", "stressed"}},
                    {"frustrated", new[] {"frustrated", "angry", "annoyed", "mad", "irritated", "aggravated", "bothered"}},
                    {"confused", new[] {"confused", "unsure", "don't understand", "don't know", "puzzled", "bewildered", "perplexed"}},
                    {"scared", new[] {"scared", "afraid", "frightened", "terrified", "panicked", "alarmed", "intimidated"}},
                    {"overwhelmed", new[] {"overwhelmed", "too much", "can't handle", "too hard", "swamped", "burdened", "snowed under"}}
                };

                var positiveSentiments = new Dictionary<string, string[]>
                {
                    {"happy", new[] {"happy", "excited", "great", "good", "pleased", "delighted", "thrilled"}},
                    {"curious", new[] {"curious", "interested", "want to learn", "wondering", "inquisitive", "eager", "keen"}}
                };

                foreach (var sentiment in negativeSentiments)
                {
                    if (sentiment.Value.Any(keyword => input.Contains(keyword)))
                    {
                        return sentiment.Key;
                    }
                }

                foreach (var sentiment in positiveSentiments)
                {
                    if (sentiment.Value.Any(keyword => input.Contains(keyword)))
                    {
                        return sentiment.Key;
                    }
                }

                return "neutral";
            }
            catch
            {
                return "neutral";
            }
        }

        static string GetEmpatheticResponse(string sentiment, string topic)
        {
            try
            {
                switch (sentiment)
                {
                    case "worried":
                        return $"It's completely understandable to feel that way about {topic}. Let me share some information to help put your mind at ease.";
                    case "frustrated":
                        return $"I hear your frustration about {topic}. Cybersecurity can be challenging, but I'll try to explain it clearly.";
                    case "confused":
                        return $"{topic} can be confusing at first. Let me break it down into simpler terms for you.";
                    case "scared":
                        return $"It's okay to feel concerned about {topic}. The good news is there are ways to protect yourself effectively.";
                    case "overwhelmed":
                        return $"I understand {topic} might seem overwhelming right now. Let's take it one step at a time.";
                    case "curious":
                        return $"I'm glad you're curious about {topic}! It's great that you want to learn more about cybersecurity.";
                    case "happy":
                        return $"It's wonderful that you're feeling positive about learning {topic}! Let's dive in.";
                    default:
                        return $"Let me tell you about {topic}.";
                }
            }
            catch
            {
                return $"Let me tell you about {topic}.";
            }
        }

        static void SimulateTypingEffect(string message)
        {
            try
            {
                foreach (char c in message)
                {
                    Console.Write(c);
                    Thread.Sleep(30);
                }
            }
            catch { /* Ignore typing effect errors */ }
        }

        static string GetBasicResponse(string input)
        {
            try
            {
                string normalizedInput = input?.Trim().ToLower() ?? "";
                string sentiment = DetectSentiment(normalizedInput);
                bool topicFound = false;

                // Handle name recognition
                if (normalizedInput.Contains("my name is") && !normalizedInput.Contains("what is"))
                {
                    return HandleNameChange(normalizedInput);
                }

                // Handle interest detection
                if (normalizedInput.Contains("i'm interested in") || normalizedInput.Contains("i like") ||
                    normalizedInput.Contains("i care about") || normalizedInput.Contains("tell me about"))
                {
                    foreach (var topic in topicResponses.Keys)
                    {
                        if (normalizedInput.Contains(topic))
                        {
                            topicFound = true;
                            bool isNewInterest = RememberUserInterest(topic);
                            string response = isNewInterest ? $"I'll remember you're interested in {topic}. " : "";

                            if (sentiment != "neutral")
                            {
                                response += GetEmpatheticResponse(sentiment, topic) + " ";
                            }

                            return response + GetTopicResponse(topic);
                        }
                    }
                }

                // Handle topic switching
                if (normalizedInput.Contains("switch") || normalizedInput.Contains("change") || normalizedInput.Contains("new topic"))
                {
                    topicFound = true;
                    string oldTopic = currentTopic;
                    currentTopic = "";
                    followUpCount = 0;
                    SetTextColor(ConsoleColor.DarkCyan);
                    return $"Okay, let's switch from {oldTopic}. What would you like to discuss instead?";
                }

                // Handle going back to previous topic
                if ((normalizedInput.Contains("go back") || normalizedInput.Contains("previous")) && recentTopics.Count > 1)
                {
                    topicFound = true;
                    currentTopic = recentTopics[recentTopics.Count - 2];
                    recentTopics.RemoveAt(recentTopics.Count - 1);
                    SetTextColor(ConsoleColor.DarkCyan);
                    return $"Let's go back to {currentTopic}. What would you like to know?";
                }

                // Handle current topic follow-ups
                if (!string.IsNullOrEmpty(currentTopic))
                {
                    if (normalizedInput.Contains("more") || normalizedInput.Contains("yes") ||
                        normalizedInput.Contains("explain") || normalizedInput.Contains("detail"))
                    {
                        topicFound = true;
                        followUpCount++;
                        return GetFollowUpResponse(currentTopic, followUpCount);
                    }

                    if (normalizedInput.Contains("example") || normalizedInput.Contains("show me"))
                    {
                        topicFound = true;
                        SetTextColor(GetTopicColor(currentTopic));
                        return GetExampleForTopic(currentTopic);
                    }

                    if (normalizedInput.Contains("no") || normalizedInput.Contains("enough"))
                    {
                        topicFound = true;
                        string oldTopic = currentTopic;
                        currentTopic = "";
                        SetTextColor(ConsoleColor.DarkCyan);
                        return $"Okay, we can stop discussing {oldTopic}. What else would you like to know about?";
                    }
                }

                // Handle core topics
                foreach (var topic in topicResponses.Keys)
                {
                    if (normalizedInput.Contains(topic))
                    {
                        topicFound = true;
                        bool isNewInterest = RememberUserInterest(topic);
                        string response = isNewInterest ? $"I'll remember you're interested in {topic}. " : "";

                        if (sentiment != "neutral")
                        {
                            response += GetEmpatheticResponse(sentiment, topic) + " ";
                        }

                        return response + GetTopicResponse(topic);
                    }
                }

                // Handle special topics
                if (normalizedInput.Contains("common myth") || normalizedInput.Contains("cybersecurity myth"))
                {
                    topicFound = true;
                    currentTopic = "myths";
                    recentTopics.Add(currentTopic);
                    followUpCount = 0;
                    SetTextColor(ConsoleColor.Yellow);
                    int index = random.Next(mythsAndFacts.Count);
                    return $"{mythsAndFacts[index][0]}\n{mythsAndFacts[index][1]}";
                }

                // Handle personal information
                if (normalizedInput.Contains("what do you know about me") || normalizedInput.Contains("remember about me"))
                {
                    topicFound = true;
                    return GetRememberedInfo();
                }

                // Handle small talk
                topicFound = HandleSmallTalk(normalizedInput, out string smallTalkResponse);
                if (topicFound)
                {
                    return smallTalkResponse;
                }

                // Default responses
                if (!string.IsNullOrEmpty(currentTopic) && topicFound)
                {
                    SetTextColor(ConsoleColor.Gray);
                    return $"{userName}, I was discussing {currentTopic}. Did you want more information about that, or should we switch topics?";
                }

                SetTextColor(ConsoleColor.Gray);
                return $"{userName}, I'm not sure I understand. Can you try rephrasing or ask about cybersecurity topics like phishing or password safety?";
            }
            catch
            {
                return "I'm having trouble understanding that. Could you please rephrase your question?";
            }
        }

        static string HandleNameChange(string normalizedInput)
        {
            string newName = normalizedInput.Replace("my name is", "").Trim();
            userDetails["name"] = newName;
            userName = newName;
            return $"Nice to meet you, {newName}! I'll remember that.";
        }

        static bool HandleSmallTalk(string normalizedInput, out string response)
        {
            response = "";

            if (normalizedInput.Contains("how are you"))
            {
                SetTextColor(ConsoleColor.Cyan);
                response = $"{userName}, I don't have emotions, but I'm always here to assist you in staying safe online!";
                return true;
            }

            if (normalizedInput.Contains("what's your purpose") || normalizedInput.Contains("what is your purpose"))
            {
                SetTextColor(ConsoleColor.Green);
                response = $"{userName}, my purpose is to provide you with valuable cybersecurity tips and help you stay protected while browsing the internet.";
                return true;
            }

            if (normalizedInput.Contains("what can i ask") || normalizedInput.Contains("what can you do"))
            {
                SetTextColor(ConsoleColor.Yellow);
                response = $"{userName}, you can ask about password safety, phishing attacks, malware, encryption, VPNs, and much more!";
                return true;
            }

            return false;
        }

        static string GetTopicResponse(string topic)
        {
            if (topicResponses.TryGetValue(topic, out string response))
            {
                currentTopic = topic;
                recentTopics.Add(currentTopic);
                followUpCount = 0;
                SetTextColor(GetTopicColor(topic));
                return response;
            }

            currentTopic = topic;
            recentTopics.Add(currentTopic);
            followUpCount = 0;
            SetTextColor(ConsoleColor.Gray);
            return "I have information about that topic. Would you like me to explain more?";
        }

        static ConsoleColor GetTopicColor(string topic)
        {
            if (topicColors.TryGetValue(topic, out ConsoleColor color))
            {
                return color;
            }
            return ConsoleColor.Gray;
        }

        static string GetExampleForTopic(string topic)
        {
            if (exampleResponses.TryGetValue(topic, out string example))
            {
                return $"{userName}, {example}";
            }
            return $"{userName}, here's an example related to {topic}... (I'll need more specific examples added for this topic)";
        }

        static bool RememberUserInterest(string topic)
        {
            if (!userInterests.Contains(topic))
            {
                userInterests.Add(topic);
                return true;
            }
            return false;
        }

        static string GetRememberedInfo()
        {
            if (userDetails.Count == 0 && userInterests.Count == 0)
            {
                return "I don't remember any specific details about you yet. Tell me something about yourself!";
            }

            string response = "";
            if (userDetails.ContainsKey("name"))
            {
                response += $"I know your name is {userDetails["name"]}. ";
            }
            if (userInterests.Count > 0)
            {
                response += $"You're interested in {string.Join(", ", userInterests)}. ";
            }
            return response.Trim();
        }

        static string GetFollowUpResponse(string topic, int depth)
        {
            if (followUpResponses.TryGetValue(topic, out List<string> responses))
            {
                if (depth <= responses.Count)
                {
                    return $"{userName}, {responses[depth - 1]}";
                }
            }

            currentTopic = "";
            followUpCount = 0;
            return $"{userName}, is there another cybersecurity topic you'd like to discuss?";
        }

        static void SetTextColor(ConsoleColor color)
        {
            try
            {
                Console.ForegroundColor = color;
            }
            catch { /* Ignore color setting errors */ }
        }

        static void DisplayCyberDroidAscii()
        {
            string cyberDroidAscii = @"
 CCCC   Y   Y  BBBBB   EEEEE  RRRR       DDDD   RRRR    OOO   III   DDDD  
C       Y Y   B    B  E       R   R      D   D  R   R  O   O   I    D   D
C        Y    BBBBB   EEEE    RRRR       D   D  RRRR   O   O   I    D   D
C        Y    B    B  E       R  R       D   D  R  R   O   O   I    D   D
 CCCC    Y    BBBBB   EEEEE   R   R      DDDD   R   R   OOO   III   DDDD  
";

            ConsoleColor[] rainbowColors = {
                ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green,
                ConsoleColor.Cyan, ConsoleColor.Blue, ConsoleColor.Magenta,
                ConsoleColor.White
            };

            try
            {
                int colorIndex = 0;
                foreach (char c in cyberDroidAscii)
                {
                    if (c == '\n')
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.ForegroundColor = rainbowColors[colorIndex % rainbowColors.Length];
                        Console.Write(c);
                        colorIndex++;
                    }
                }
                Console.ResetColor();
            }
            catch { /* Ignore ASCII art display errors */ }
        }
    }
}