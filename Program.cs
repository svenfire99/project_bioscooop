using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace project_bioscooop
{
    internal class Program
    {
        private static int permission = 0;
        
        private static Dictionary<int, Movie> movieList = new Dictionary<int,Movie>();
        private static Dictionary<int, Ticket> ticketList = new Dictionary<int,Ticket>();
        private static Dictionary<string, ConsoleGui.Element> accountList = new Dictionary<string, ConsoleGui.Element>();
        private static Dictionary<int, ConsoleGui.Element> theaterList = new Dictionary<int, ConsoleGui.Element>();
        private static Dictionary<int, Menu.FoodItem> menuItem = new Dictionary<int, Menu.FoodItem>();

        private const int STATE_EXIT = -1;
        private const int STATE_MAIN = 0;
        private const int STATE_CREATE_ACCOUNT = 1;
        private const int STATE_LOG_IN = 2;
        private const int STATE_IS_LOGGED_IN = 3;

        private const int STATE_MANAGER_ADD_MOVIE = 11;
        private const int STATE_MANAGER_REMOVE_MOVIE = 12;
        private const int STATE_MANAGER_ADD_THEATER = 13;
        private const int STATE_MANAGER_REMOVE_THEATER = 14;
        
        
        private const int STATE_CATERER_CHANGE_MENU = 21;


        private static int currentState = 0;
        private static Account activeUser = null;


        public static void Main(string[] args)
        {
            //setup
            setup();
            
            //main loop
            while (currentState != STATE_EXIT)
            {
                switch (currentState)
                {
                    case STATE_MAIN:
                        stateMain();
                        break;
                    case STATE_CREATE_ACCOUNT:
                        stateCreateAccount();
                        break;
                    case STATE_IS_LOGGED_IN:
                        stateLoggedIn();
                        break;
                    case STATE_MANAGER_ADD_MOVIE:
                        stateManagerAddMovie();
                        break;
                    case STATE_LOG_IN:
                        stateLogin();
                        break;
                    case STATE_MANAGER_ADD_THEATER:
                        stateManagerAddTheater();
                        break;
                    case STATE_MANAGER_REMOVE_THEATER:
                        stateManagerRemoveTheater();
                        break;
                    // case STATE_CATERER_CHANGE_MENU:
                    //     
                    //     break;
                }
            }
        }

        //everything to start the program
        public static void setup()
        {
            //create admin account
            accountList.Add("admin", new Account("admin","admin", 420, "admin", Account.ROLE_ADMIN));
            accountList.Add("caterer", new Account("caterer", "caterer", 420, "caterer@gmail.com", Account.ROLE_CATERING));
            Generator.generateMovieData(100, movieList);

            Theater testTheater = new Theater(new Theater.SeatGroup(420, 69));
            theaterList.Add(testTheater.getId(), testTheater);
        }
        
        //states
        public static void stateMain()
        {
            int choice = ConsoleGui.multipleChoice("Welcome to Cinema, What would you like to do?", "llogin",
                "ccreate account");
            switch (choice)
            {
                case 0: 
                    currentState = STATE_LOG_IN;
                    break;
                
                case 1:
                    currentState = STATE_CREATE_ACCOUNT;
                    break;
            }
        }

        public static void stateCreateAccount()
        {
            //intro
            Console.WriteLine("Cool! thanks for making an account with us :)\n");
            bool creating = true;
            while (creating)
            {
                string name = ConsoleGui.openQuestion("Lets begin with your name!");
                
                //exit strategy
                if (name.Equals("exit"))
                {
                    currentState = STATE_MAIN;
                    return;
                }
                
                //get values
                string password = ConsoleGui.openQuestion("Now an easy to remember, hard to guess password:");
                string email = ConsoleGui.openQuestion("Great! on which email-adress can we reach you?",
                    new string[] {"@", "."}, "that's not a  real email! :(");
                int age = ConsoleGui.getInteger("Finally, and don't lie, How old are you?");

                int isRight = ConsoleGui.multipleChoice("So you are " + name + " with password " + password +
                                                        "\nthat we can reach on: " + email + " and you're " + age +
                                                        " years old", "yyes", "nno");

                if (isRight == 0 && ConsoleGui.noErrorsInValue(name, password, email, age.ToString()))
                {
                    var newAcc = new Account(name, password, age, email, 0);
                    if (!accountList.ContainsKey(email)){
                        accountList.Add(email, newAcc);
                        activeUser = newAcc;
                        currentState = STATE_IS_LOGGED_IN;
                        return;
                    }
                    else
                    {
                        while (accountList.ContainsKey(email))
                        {
                            email =ConsoleGui.openQuestion("Uh-oh that email is already taken." +
                                                    "\n type exit or let's find one that isn't");
                            if (email.Equals("exit"))
                            {
                                currentState = STATE_MAIN;
                                return;
                            }
                        }
                    }
                }
                else
                {
                    Console.Out.WriteLine("\nWell then, let's try again or type exit to go back\n");
                }

                
            }
        }

        public static void stateLoggedIn()
        {
            Action<int> userMenu = (int role) =>
            {
                if (role == Account.ROLE_USER)
                {
                    //TODO add switch case for user menu
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "ccheck available movies", "ssee my account"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                    }
                }
                else
                {
                    ConsoleGui.debugLine("something went horribly wrong with the permissions");
                    activeUser = null;
                    currentState = STATE_MAIN;
                }

            };
            
            Action<int> catererMenu = (int role) =>
            {
                if (role == Account.ROLE_CATERING)
                {
                    //TODO add switch case for catering menu
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "llist food items ", "aadd food items"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                    }
                }
                else
                {
                    ConsoleGui.debugLine("something went horribly wrong with the permissions");
                    activeUser = null;
                    currentState = STATE_MAIN;
                }

            };
            
            Action<int> employeeMenu = (int role) =>
            {
                if (role == Account.ROLE_EMPLOYEE)
                {
                    //TODO add switch case for employee menu
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "ccheck available movies", "ssee my account"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                    }
                }
                else
                {
                    ConsoleGui.debugLine("something went horribly wrong with the permissions");
                    activeUser = null;
                    currentState = STATE_MAIN;
                }

            };
            
            Action<int> adminMenu = (int role) =>
            {
                if (role == Account.ROLE_ADMIN)
                {
                    //TODO add switch case for Admin menu
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "1add movie", "2remove", "3addTheater", "4removeTheater"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                        
                        case 0:
                            currentState = STATE_MANAGER_ADD_MOVIE;
                            break;
                        
                        case 1:
                            currentState = STATE_MANAGER_REMOVE_MOVIE;
                            break;
                        
                        case 2:
                            currentState = STATE_MANAGER_ADD_THEATER;
                            break;
                        
                        case 3:
                            currentState = STATE_MANAGER_REMOVE_THEATER;
                            break;
                        
                    }
                }
                else
                {
                    ConsoleGui.debugLine("something went horribly wrong with the permissions");
                    activeUser = null;
                    currentState = STATE_MAIN;
                }

            };

            switch (activeUser.role)
            {
                case 0: userMenu(activeUser.role); break; 
                case 1: catererMenu(activeUser.role); break; 
                case 2: employeeMenu(activeUser.role); break; 
                case 3: adminMenu(activeUser.role); break; 
            }



        }

        public static void stateLogin()
        {
            Console.Out.WriteLine("\nWith what email address do you want to log in?");
            
            //get account corresponding to email adress
            string logInEmail = "";
            while (!accountList.ContainsKey(logInEmail))
            {
                
                //fetch account corresponding to email
                logInEmail = ConsoleGui.openQuestion("Please enter your email or type exit if you don't know: ");
                
                
                if (logInEmail == "exit")
                {
                    currentState = STATE_MAIN;
                    return;
                }
                else if(!accountList.ContainsKey(logInEmail))
                {
                    Console.Out.WriteLine("We don't know that one");
                }
            }

            //verify using password
            Account potentialUserAcc = (Account)accountList[logInEmail];
            if (ConsoleGui.openQuestion("Please enter your password", new string[] {potentialUserAcc.password}
                    , " password is wrong, please try again") == "ERROR")
            {
                activeUser = null;
                currentState = STATE_MAIN;
            }
            else
            {
                activeUser = potentialUserAcc;
                currentState = STATE_IS_LOGGED_IN;
            }
            




        }

        public static void stateManagerAddMovie()
        {
            //TODO please make sure there is an "exit" option that returns:  "ERROR" :)  tools can be found in consoleGui
        }

        public static void stateManagerRemoveMovie()
        {
            //TODO write stateManagerRemoveMovie()
            Console.Out.WriteLine("stateManagerRemoveMovie");
        }

        public static void stateManagerAddTheater()
        {
            List<Theater.SeatGroup> newSeatGroups = new List<Theater.SeatGroup>();
            int index = 0;
            bool adding = true;
            while (adding)
            {
                int seats = ConsoleGui.getInteger("please specify the amount of seats in price group " + index);
                //return to menu if exit
                if (seats < 0)
                {
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                }
                int price = ConsoleGui.getInteger("please specify the price of seats in price group " + index);
                //return to menu if exit
                if (price < 0)
                {
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                }
                newSeatGroups.Add(new Theater.SeatGroup(seats, price));

                int ans = ConsoleGui.multipleChoice("add another price group?", "yyes", "nno");
                switch (ans)
                {
                    case -1: currentState = STATE_IS_LOGGED_IN; return;
                    case 1: adding = false; break;
                }
            }

            Console.Out.WriteLine(newSeatGroups.ToString());
            Theater newTheater = new Theater(newSeatGroups.ToArray());
            
            int check = ConsoleGui.multipleChoice(
                "You want to add Theater with id: " + newTheater.getId() + " with " + newTheater.getAmountOfSeats(),
                "yyes", "nno");
            switch (check)
            {
                case -1: case 1: currentState = STATE_IS_LOGGED_IN; return;
                case 0: theaterList.Add(newTheater.getId(), newTheater); break;
            }

            Console.Out.WriteLine("Current theaters: \n");
            ConsoleGui.list(theaterList);
            currentState = STATE_IS_LOGGED_IN;
            return;

        }
        
        public static void stateManagerRemoveTheater()
        {
            Theater theater = (Theater)ConsoleGui.getElementByMultipleChoice("Which theater would you like to remove?", theaterList);
            if (ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno") == 0)
            {
                theaterList.Remove(theater.getId());
            }
            
            ConsoleGui.list(theaterList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }
        
        
        

        //classes
        public class Account : ConsoleGui.Element
        {
            public static readonly int ROLE_USER = 0;
            public static readonly int ROLE_CATERING = 1;
            public static readonly int ROLE_EMPLOYEE = 2;
            public static readonly int ROLE_ADMIN = 3;
                
            public readonly int role;
            public readonly string name;
            public readonly int accountId;
            public readonly string email;
            public readonly string password;
            public readonly int age;

            private static int AccountIDCounter = 0;

            private static int getAccountID()
            {
                AccountIDCounter++;
                return AccountIDCounter;
            }


            public Account(string inp_name, string inp_password, int inp_age, string inp_email, int inp_permission)
            {
                role = inp_permission;
                email = inp_email;
                accountId = getAccountID();
                name = inp_name;
                password = inp_password;
                age = inp_age;
            }

            public override void list()
            {
                Console.Out.WriteLine("id: " + accountId + "   name: " + name + "   email: " + email);
            }

            public override string getMPQListing()
            {
                return "id: " + accountId + "   name: " + name;
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
            private string title;
            private DateTime time;
            //private int price; removed because theaters have prices movies don't?

            public string getTitle()
            {
                return title;
            }

            public Movie(string inp_title, DateTime inp_dateTime)
            {
                title = inp_title;
                time = inp_dateTime;
            }
            
            public static Movie getNoneMovie()
            {
                return new Movie("None", new DateTime(2240,8,7,6,5,4));
            }
        }
        
        public class Theater : ConsoleGui.Element
        {
            private readonly int theaterId;
            private readonly int amountOfSeats;
            private SeatGroup[] seatGroups;
            private Movie currentMovie;

            private static int idCount = -1;

            
            // abstract methods
            public override void list()
            {
                Console.Out.WriteLine(" Id: " + theaterId + " running: " + currentMovie.getTitle());
            }

            public override string getMPQListing()
            {
                return (" Id: " + theaterId + " running: " + currentMovie.getTitle() + " seats available: " + getAvailableSeats() +
                        " of " + amountOfSeats);
            }

            
            // methods
            public int getAvailableSeats()
            {
                int output = 0;
                foreach (SeatGroup seatGroup in this.seatGroups )
                {
                    output += seatGroup.getAvailableAmountOfSeats();
                }

                return output;
            }

            public void setCurrentMovie()
            {
                //TODO add method for setting the movie runnning in theater
            }

            public Movie getCurrentMovie()
            {
                return currentMovie;
            }

            public void removeCurrentMovie()
            {
                //TODO add method to remove current movie
            }

            public int getId()
            {
                return theaterId;
            }

            public int getAmountOfSeats()
            {
                return amountOfSeats;
            }


            // constructor
            public Theater(params SeatGroup[] seatGroups)
            {
                idCount++;
                theaterId = idCount;
                amountOfSeats = 0;
                foreach (SeatGroup seatGroup in seatGroups )
                {
                    amountOfSeats += seatGroup.getAmountOfSeats();
                }
                
                // theater will have a 'none' movie by default
                currentMovie = Movie.getNoneMovie();
            }
            
            //data holder for SeatGroup seats and prices
            public class SeatGroup
            {
                private int amountOfSeats;
                private int amountOfAvailableSeats;
                private int priceOfGroup;

                public SeatGroup(int inp_amountOfSeats, int inp_priceOfSeats)
                {
                    amountOfSeats = inp_amountOfSeats;
                    amountOfAvailableSeats = amountOfSeats;
                    priceOfGroup = inp_priceOfSeats;
                }

                public void sellTicket()
                {
                    amountOfAvailableSeats--;
                    //TODO change state to ticketSellingState
                }

                
                //getters
                public int getAvailableAmountOfSeats()
                {
                    return amountOfAvailableSeats;
                }

                public int getAmountOfSeats()
                {
                    return amountOfSeats;
                }

                public int getTicketPrice()
                {
                    return priceOfGroup;
                }
                
            }
            
        }
        

        

        //the engine
        public static class ConsoleGui
        {
            //method that will create a choice dialog
            public static string openQuestion(string question, string[]? checks, string? negativeResponse)
            {
                string output = "";

                //base output: 
                if (checks == null && negativeResponse == null)
                {
                    //just returns answer
                    Console.Out.WriteLine("\n" + question + " \ntype exit if you don't know");
                    output = Console.ReadLine();
                    if (output.Equals("exit"))
                    {
                        return "ERROR";
                    }
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
                        else if (negativeResponse != null)
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

            
            //TODO change multipleChoice to accept numbers as first input by taking a substring until first non numeral char
            
            public static int multipleChoice(string question, params string[] options)
            {
                Console.Out.WriteLine("\n" + question);
                while (true)
                {
                    //list all possible inputs and prepare for answer
                    string[] possibleInputs = new string[options.Length];
                    for (int i = 0; i < options.Length; i++)
                    {
                        string option = options[i];
                        
                        //count how many numeral chars are in this possible answer to account for inputs beginning with numbers
                        int lenghtOfExpectedUserInput = 1;
                        for (int j = 0; j < option.Length; j++)
                        {
                            if (!Char.IsDigit(option[j]))
                            {
                                break;
                            }
                            else if(j+1 > lenghtOfExpectedUserInput)
                            {
                                lenghtOfExpectedUserInput = j+1;
                            }
                        }
                        
                        //distill info
                        string newAns = option.Substring(0, lenghtOfExpectedUserInput).ToUpper();
                        string firstChar = option.Substring(lenghtOfExpectedUserInput, 1).ToUpper();
                        string listOption = "[" + newAns + "]" + " " + firstChar + option.Substring(lenghtOfExpectedUserInput+1);

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
                }
            } 
            
            
            
            public static int getInteger(string question)
            {
                Console.Out.WriteLine("\n" + question);


                while (true)
                {
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
            
            
            public abstract class Element
            {
                public abstract void list();

                public abstract string getMPQListing();

            }

            
            public static void list(IEnumerable iterable)
            {

                
                if (iterable is IDictionary)
                {
                    IDictionary dict = (IDictionary) iterable;
                    foreach (Element element in dict.Values)
                    {
                        element.list();
                    }
                }
                else
                {
                    foreach (Element element in (IEnumerable)iterable)
                    {
                        element.list();
                    }
                }
                
                
            }
            
            public static Element getElementByMultipleChoice(String question, List<Element> inputList)
            {
                // extract possible ans as string from elements
                string[] elements = new string[inputList.Count];
                for (int i = 0; i < elements.Length; i++)
                {
                    elements[i] = i + inputList[i].getMPQListing();
                }

                int ans = multipleChoice(question, elements);

                if (ans >= 0)
                {
                    return inputList[ans];
                }
                return null;
            }

            public static Element getElementByMultipleChoice(String question, IDictionary inputDict)
            {
                return getElementByMultipleChoice(question, (List<Element>)inputDict.Values);
            }
            
            public static Element getElementByMultipleChoice(String question, Element[] inputArray)
            {
                return getElementByMultipleChoice(question, inputArray.ToList());
            }
            
        }

        public static class Generator
        {
            public static void generateMovieData(int amountOfDataEntries, Dictionary<int, Movie> inp_movieDict)
            {
                // code to generate 'amountOfDataEntries' x random movie and add them to inp_movieDict
            }
            
            public static void generateUserData(int amountOfDataEntries, Dictionary<string, Account> inp_userDict)
            {
                // code to generate 'amountOfDataEntries' x random user and add them to inp_userDict
            }
            
        }
        
        
    }
}