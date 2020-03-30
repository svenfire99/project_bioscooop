using System;
using System.Collections.Generic;

namespace project_bioscooop
{
    internal class Program
    {
        private static int permission = 0;
        
        private Dictionary<int, Movie> movieList = new Dictionary<int,Movie>();
        private Dictionary<int, Ticket> ticketList = new Dictionary<int,Ticket>();
        private Dictionary<string, Account> accountList = new Dictionary<string,Account>();
        private Dictionary<int, Theater> theaterList = new Dictionary<int,Theater>();

        private const int STATE_EXIT = -1;
        private const int STATE_MAIN = 0;
        private const int STATE_CREATE_ACCOUNT = 1;
        private const int STATE_LOG_IN = 2;
        private const int STATE_IS_LOGGED_IN = 3;
        
        
        private static int currentState = 0;


        public static void Main(string[] args)
        {
            //main loop
            while (currentState != STATE_EXIT) {
                switch (currentState)
                {
                    case STATE_MAIN: stateMain(); break;
                    case STATE_CREATE_ACCOUNT: stateCreateAccount(); break;
                    //TODO add cases for LOG_IN, IS_LOGGED_IN
                }
            }
        }
        
        //states
        public static void stateMain()
        {
            currentState = consoleGui.multipleChoice("Welcome to Cinema, What would you like to do?", "llogin", "ccreate account");
            consoleGui.debugLine("currentstate: " + currentState);
        }
        
        public static void stateCreateAccount()
        {
            
            //intro
            Console.WriteLine("Cool! thanks for making an account with us :)\n");
            bool creating = true;
            while (creating)
            {
             
             
             string name = consoleGui.openQuestion("Lets begin with your name!");
             string password = consoleGui.openQuestion("Now an easy to remember, hard to guess password:");
             string email = consoleGui.openQuestion("Great! on which email-adress can we reach you?", new string[]{"@","."},"that's not a  real email! :(");
             int age = consoleGui.getInteger("Finally, and don't lie, How old are you?");

             int isRight = consoleGui.multipleChoice("so you are " + name + " with password " + password +
                                                     "\nthat we can reach on: " + email + " and you're " + age +
                                                     " years old", "yyes", "nno");
             
             if (isRight == 0 && consoleGui.noErrorsInValue(name,password,email,age.ToString()))
             {
                 //TODO add customer
                 //TODO set state to IS_LOGGED_IN
             }
             else
             {
                 Console.Out.WriteLine("\nWell then, let's try again\n");
             }

             //TODO add exit maybe by asking question of go back when finding error
             
            }
        }
        

        
        

        //classes
        public class Account
        {
            public readonly int permission;
            public readonly string name;
            public readonly int accountId;
            public readonly string email;
            
            private static int AccountIDCounter = 0;

            private static int getAccountID()
            {
                AccountIDCounter++;
                return AccountIDCounter;
            }        
            
            
            public Account(string inp_name, int inp_permission, string inp_email)
            {
                permission = inp_permission;
                email = inp_email;
                accountId = getAccountID();
                name = inp_name;
            }
                    
        
        }
        
        public static class Menu
        {
            static List<FoodItem> foodItems = new List<FoodItem>();
            

            public class FoodItem
            {
                private String[] ingredients;
                private int price;
            }
        }

        public class Ticket
        {
            public readonly int ticketID;
            public readonly Theater Theater;
            public readonly Movie movie;
            public int price;
        }

        public class Movie
        {
            private DateTime time;
            private int price;

        }

        public class Theater
        {
            public readonly int theaterId;
            private int[] prices;
        }


        
        
        //the engine
        public static class consoleGui
        {
            
            //method that will create a choice dialog
            public static string openQuestion(string question, string[]? checks, string? negativeResponse)
            {
                 string output = "";
                 
                //base output: 
                if (checks == null && negativeResponse == null)
                {
                    //just returns answer
                    Console.Out.WriteLine("\n" + question);
                    output = Console.ReadLine();
                    return output;
                }
                //go into code that see if the checks are met
                else
                {
                    //will keep asking same question until checks are met
                    //will return ERROR when exit is typed
                    Console.Out.WriteLine("\n" + question);
                    Console.Out.WriteLine("(type exit if you don't know)");
                    while (true)
                    {
                        output = Console.ReadLine();
                        bool satisfactitory = true;
                        
                        //exit if exit command is given
                        if (output.Equals("exit"))
                        {
                            return "ERROR";
                        }
                        
                        //loop throught the checks and see if they're met
                        foreach (string check in checks)
                        {
                            if (!output.Contains(check))
                            {
                                satisfactitory = false;
                                break;
                            }
                        }

                        //act base on the outcome of the checks
                        if (satisfactitory)
                        {
                            return output;
                        }
                        else if(negativeResponse != null)
                        {
                            Console.Out.WriteLine(negativeResponse);
                            Console.Out.WriteLine("try again or type exit if you don't know");
                        }
                        else
                        {
                            Console.Out.WriteLine("that's not right, try again or type exit if you don't know");
                        }
                        
                    }
                }
                
                

                
                return output;
            }

            //shorthand way of above method for no checks
            public static string openQuestion(string question)
            {
                return openQuestion(question, null, null);
            }

            //method that returns an int corresponding to answer given
            //returns -1 when exited by exit option
            public static int multipleChoice(string question,params string[] options)
            {
                Console.Out.WriteLine("\n" + question);
                while (true)
                {
                    
                    //list all possible inputs and prepare for answer
                    string[] possibleInputs = new string[options.Length];
                    for(int i = 0; i < options.Length; i++)
                    {
                        //distill info
                        string option = options[i];
                        string newAns = option.Substring(0, 1).ToUpper();
                        string firstChar = option.Substring(1, 1).ToUpper();
                        string listOption = "[" + newAns + "]" + " " + firstChar + option.Substring(2);

                        //list option and save possible answer
                        possibleInputs[i] = newAns;
                        Console.Out.WriteLine(listOption);
                    }
                    
                    //add exit for escape
                    Console.Out.WriteLine("[X] Exit\n");
                    string ans = Console.ReadLine().ToUpper();
                    
                    
                    //get and check answer against possible answers
                    if (ans.Equals("X"))
                    {
                        return -1;
                    }
                    else
                    {
                        for (int i = 0; i < possibleInputs.Length; i++)
                        {
                            if (ans.Equals(possibleInputs[i]))
                            {
                                return i;
                            }
                        }
                        Console.Out.WriteLine("Thats not an option, type \"X\" if you don't know");
                    }
                    
                    
                }//end of mpq while(true) loop
            }// end of mpq method

            public static int getInteger(string question)
            {
                Console.Out.WriteLine("\n" + question);
                
                
                while(true){
                    string input = Console.ReadLine();
                    
                    //create escape
                    if (input.Equals("exit"))
                    {
                        return int.MinValue;
                    }

                    if (int.TryParse(input, out int output))
                    {
                        return output;
                    }
                    else
                    {
                        Console.Out.WriteLine("That's not a valid aswer, please try again" +
                                              "\nor type exit to go back");
                    }
                }
            }

            public static void debugLine(string line)
            {
                Console.Out.WriteLine("# DEBUG #" + line);
            }

            public static bool noErrorsInValue(params string[] values)
            {
                foreach (var value in values)
                {
                    if (value.Equals("ERROR") || value.Equals("-1"))
                    {
                        return false;
                    }
                }
                return true;
            }
            
            
        }
        
        

    }
}
