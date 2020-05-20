using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Microsoft.VisualBasic;

namespace project_bioscooop
{
    internal class Program
    {
        private static int permission = 0;

        public static Dictionary<string, ConsoleGui.Element> movieList = new Dictionary<string, ConsoleGui.Element>();

        private static Dictionary<int, Ticket> ticketList = new Dictionary<int, Ticket>();

        private static Dictionary<string, ConsoleGui.Element>
            accountList = new Dictionary<string, ConsoleGui.Element>();

        public static Dictionary<string, ConsoleGui.Element> theaterList = new Dictionary<string, ConsoleGui.Element>();

        private static Dictionary<string, ConsoleGui.Element> menuItem = new Dictionary<string, ConsoleGui.Element>();

        private const int STATE_EXIT = -1;
        private const int STATE_MAIN = 0;
        private const int STATE_CREATE_ACCOUNT = 1;
        private const int STATE_LOG_IN = 2;
        private const int STATE_IS_LOGGED_IN = 3;
      
        private const int STATE_MANAGER_ADD_MOVIE = 11;
        private const int STATE_MANAGER_REMOVE_MOVIE = 12;
        private const int STATE_MANAGER_EDIT_MOVIE = 13;
        private const int STATE_MANAGER_ADD_THEATER = 14;
        private const int STATE_MANAGER_REMOVE_THEATER = 15;
        private const int STATE_MANAGER_MANAGE_THEATER = 16;
      
        private const int STATE_CATERER_ADD_MENU = 20;
        private const int STATE_CATERER_REMOVE_MENU = 22;
        private const int STATE_CATERER_CHANGE_MENU = 21;
        
        private const int STATE_CUSTOMER_SHOW_MOVIES = 31;
        private const int STATE_CUSTOMER_BUY_TICKET_MOVIE = 32;
        private const int STATE_CUSTOMER_SHOW_CATHERING = 33;
        private const int STATE_CUSTOMER_ADD_CATHERING_ITEM = 34;
        private const int STATE_CUSTOMER_SHOW_BASKET = 35;
        private const int STATE_CUSTOMER_PURCHASE_BASKET = 36;
        private const int STATE_CUSTOMER_VIEW_PURCHASE = 37;

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
                    case STATE_LOG_IN:
                        stateLogin();
                        break;
                    case STATE_IS_LOGGED_IN:
                        stateLoggedIn();
                        break;
                    
                    case STATE_MANAGER_ADD_MOVIE:
                        stateManagerAddMovie();
                        break;
                    case STATE_MANAGER_REMOVE_MOVIE:
                        stateManagerRemoveMovie();
                        break;
                    case STATE_MANAGER_EDIT_MOVIE:
                        stateManagerEditMovie();
                        break;
                    case STATE_MANAGER_ADD_THEATER:
                        stateManagerAddTheater();
                        break;
                    case STATE_MANAGER_REMOVE_THEATER:
                        stateManagerRemoveTheater();
                        break;
                    case STATE_CATERER_ADD_MENU:
                        StateCatererAddMenu();
                        break;
                    case STATE_CATERER_REMOVE_MENU:
                        stateCatererRemoveMenu();
                        break;
                    case STATE_MANAGER_MANAGE_THEATER:
                        stateManagerManageTheater();
                        break;
                    
                    case STATE_CUSTOMER_SHOW_CATERER_MENU:
                        stateCustomerShowCatererMenu();
                        break;
                    case STATE_CUSTOMER_SHOW_MOVIES:
                        showMovies();
                        break;
                    case STATE_CUSTOMER_BUY_TICKET_MOVIE:
                        buyTicket();
                        break;
                }
            }
        }

        //everything to start the program
        public static void setup()
        {
            //create admin account
            accountList.Add("admin", new Account("admin", "admin", 420, "admin", Account.ROLE_ADMIN));
            accountList.Add("caterer", new Account("caterer", "caterer", 420, "caterer@gmail.com", Account.ROLE_CATERING));
            accountList.Add("frontend", new Account("frontend","frontend", 420, "frontend", Account.ROLE_USER));
            
            // Generator.generateMovieData(100, movieList);
          
            Theater testTheater = new Theater(new Theater.SeatGroup(420, 69, "testSeats"));
            Theater testTheater2 = new Theater(new Theater.SeatGroup(420, 69, "testSeats"));
            theaterList.Add(testTheater.getId(), testTheater);
            theaterList.Add(testTheater2.getId(), testTheater2);

            movieList.Add("-1", Movie.getNoneMovie());
            movieList.Add("0", new Movie("frozen 5", new TimeSpan(4, 20, 69)));
            movieList.Add("1", new Movie("frozen 6", new TimeSpan(4, 20, 69)));

            menuItem.Add("-1", MenuItem.GetNoneMenuItem());
            menuItem.Add("0", new MenuItem("Hot Dogs", 5.80));
            menuItem.Add("1", new MenuItem("Fries", 3.50));

            // When application starts system makes new folder structure with a .json file in it
            string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\Json folder"));
            DirectoryInfo di = Directory.CreateDirectory(path);

            string subFolder = System.IO.Path.Combine(path, "SubFolder");
            DirectoryInfo su = Directory.CreateDirectory(subFolder);

            string fileName = "MyNewFile.json";
            subFolder = System.IO.Path.Combine(subFolder, fileName);

            if (!System.IO.File.Exists(subFolder))
            {
                using (System.IO.FileStream fs = System.IO.File.Create(subFolder))
                {
                    Console.WriteLine("File created!");
                }
            }
        }

        //states
        public static void stateMain()
        {
            int choice = ConsoleGui.multipleChoice("Welcome to Cinema, What would you like to do?", "llogin",
                "ccreate account");
            switch (choice)
            {
                case -1:
                    currentState = STATE_EXIT;
                    break;
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
                    if (!accountList.ContainsKey(email))
                    {
                        accountList.Add(email, newAcc);
                        activeUser = newAcc;
                        currentState = STATE_IS_LOGGED_IN;
                        return;
                    }
                    else
                    {
                        while (accountList.ContainsKey(email))
                        {
                            email = ConsoleGui.openQuestion("Uh-oh that email is already taken." +
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
            //TODO talk about the 'See my account' function what it is suppose to do? (AHMET)
            Action<int> customerMenu = (int role) =>
            {
                if (role == Account.ROLE_USER)
                {
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "ccheck available movies", "ssee my account", "msee menu", "llog out"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                        case 0:
                            currentState = STATE_CUSTOMER_SHOW_MOVIES;
                            break;
                        case 2:
                            currentState = STATE_CUSTOMER_SHOW_CATERER_MENU;
                            break;
                        case 3:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                    }
                }
                else
                {
                    ConsoleGui.debugLine("Something went horribly wrong with the permissions.");
                    activeUser = null;
                    currentState = STATE_MAIN;
                }
            };

            Action<int> catererMenu = (int role) =>
            {
                if (role == Account.ROLE_CATERING)
                {
                    switch (ConsoleGui.multipleChoice("Hi " + activeUser.name + " what would you like to do?",
                        "aAdd food items", "rRemove FoodItem"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                        case 0:
                            currentState = STATE_CATERER_ADD_MENU;
                            break;
                        case 1:
                            currentState = STATE_CATERER_REMOVE_MENU;
                        case 3:
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
                        "ccheck available movies", "ssee my account","llog out"))
                    {
                        case -1:
                            activeUser = null;
                            currentState = STATE_MAIN;
                            break;
                        case 2:
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
                        "1add movie", "2remove movie", "3edit movie", "4add theater", "5remove theater",
                        "6manage theater"))
                        "1add movie", "2remove movie", "3edit movie", "4add theater", "5remove theater", "6manage theater","llog out"))
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
                            currentState = STATE_MANAGER_EDIT_MOVIE;
                            break;

                        case 3:
                            currentState = STATE_MANAGER_ADD_THEATER;
                            break;

                        case 4:
                            currentState = STATE_MANAGER_REMOVE_THEATER;
                            break;

                        case 5:
                            currentState = STATE_MANAGER_MANAGE_THEATER;
                            break;
                        case 6:
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

            if (activeUser == null)
            {
                Console.Out.WriteLine("Active user is: null");
            }

            switch (activeUser.role)
            {
                case 0:
                    customerMenu(activeUser.role);
                    break;
                case 1:
                    catererMenu(activeUser.role);
                    break;
                case 2:
                    employeeMenu(activeUser.role);
                    break;
                case 3:
                    adminMenu(activeUser.role);
                    break;
            }
        }

        public static void stateCustomerShowMovies()
        {
            Movie showMovie =
                (Movie) ConsoleGui.getElementByMultipleChoice("Which movie would you like to choose?", movieList);
            
            
            int ans = ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno");
            
            switch (ans)
            {
                case 1:
                    currentState = STATE_IS_LOGGED_IN;
                    break;
            }
            
            ConsoleGui.list(movieList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateCustomerShowCatererMenu()
        {
            MenuItem showFoodItem =
                (MenuItem) ConsoleGui.getElementByMultipleChoice("Which food item would you like to choose?", menuItem);
            
            
            int ans = ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno");
            
            switch (ans)
            {
             case 1:
                 currentState = STATE_IS_LOGGED_IN;
                 break;
            }
            
            ConsoleGui.list(menuItem);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void StateCatererAddMenu()
        {
            string name = ConsoleGui.openQuestion("Please give the name of the food item you want to add: ");
            if (name == "exit")
            {
                currentState = STATE_IS_LOGGED_IN;
                return;
            }

            int price = ConsoleGui.getInteger("Please give the price of the food item: ");

            MenuItem newFoodItem = new MenuItem(name, price);

            int check = ConsoleGui.multipleChoice(
                "Do you want to add the food item : " + newFoodItem.getName() + "(" + newFoodItem.getId() + ")" +
                " with the price of " + newFoodItem.getPrice() + " euro",
                "yyes", "nno");
            switch (check)
            {
                case 1:
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                case 0:
                    menuItem.Add(newFoodItem.getId(), newFoodItem);
                    break;
            }

            Console.Out.WriteLine("Current food items : \n");
            ConsoleGui.list(menuItem);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateCatererRemoveMenu()
        {
            MenuItem removeFoodItem =
                (MenuItem) ConsoleGui.getElementByMultipleChoice("Which food item would you like to remove?", menuItem);
            int ans = ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno");
            if (ans == 0 && menuItem != null)
            {
                if (removeFoodItem.getName() != "None")
                {
                    menuItem.Remove(removeFoodItem.getId());
                }
            }
            else
            {
                currentState = STATE_IS_LOGGED_IN;
                return;
            }

            ConsoleGui.list(menuItem);
            currentState = STATE_IS_LOGGED_IN;
            return;
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
                else if (!accountList.ContainsKey(logInEmail))
                {
                    Console.Out.WriteLine("We don't know that one");
                }
            }

            //verify using password
            Account potentialUserAcc = (Account) accountList[logInEmail];
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
            string name = ConsoleGui.openQuestion("Please give the name of the movie: ");
            if (name == "exit")
            {
                currentState = STATE_IS_LOGGED_IN;
                return;
            }

            int time = ConsoleGui.getInteger("Please give the duration of the movie in minutes: ");

            Movie newMovie = new Movie(name, TimeSpan.FromMinutes(time));

            int check = ConsoleGui.multipleChoice(
                "Do you want to add the movie : " + newMovie.getTitle() + "(" + newMovie.getId() + ")" +
                " with the duration of " + newMovie.getTime(),
                "yyes", "nno");
            switch (check)
            {
                case -1:
                case 1:
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                case 0:
                    movieList.Add(newMovie.getId(), newMovie);
                    break;
            }

            Console.Out.WriteLine("Current Movies : \n");
            ConsoleGui.list(movieList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateManagerRemoveMovie()
        {
            Movie movie =
                (Movie) ConsoleGui.getElementByMultipleChoice("Which movie would you like to remove?", movieList);
            int ans = ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno");
            if (ans == 0 && movie != null)
            {
                if (movie.getTitle() != "None")
                {
                    theaterList.Remove(movie.getId());
                }
            }
            else
            {
                currentState = STATE_IS_LOGGED_IN;
                return;
            }

            ConsoleGui.list(movieList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateManagerEditMovie()
        {
            if (movieList.Count() == 0)
            {
                Console.Out.WriteLine("There are no movies yet! So we redirected you to the movies add!");
                currentState = STATE_MANAGER_ADD_MOVIE;
            }
            else
            {
                Movie movie =
                    (Movie) ConsoleGui.getElementByMultipleChoice("Which movie would you like to edit?", movieList);
                Movie oldMovie = movie;
                Boolean edit = true;
                while (edit)
                {
                    string name =
                        ConsoleGui.openQuestion("Please give the name of the movie (" + movie.getTitle() + "): ");
                    if (name == "exit")
                    {
                        currentState = STATE_IS_LOGGED_IN;
                        return;
                    }

                    if (!(name == null || name == ""))
                    {
                        movie.setTitle(name);
                    }

                    int time = ConsoleGui.getInteger("Please give the duration of the movie in minutes: ");
                    if (!(time == null))
                    {
                        movie.setTime(TimeSpan.FromMinutes(time));
                    }

                    if (ConsoleGui.multipleChoice(
                        "Do you want to add the movie : " + movie.getTitle() + "(" + movie.getId() + ")" +
                        " with the duration of " + movie.getTime(),
                        "yyes", "nno") == 0)
                    {
                        edit = false;
                        Console.Out.WriteLine("Movie edited");
                        ConsoleGui.list(movieList);
                    }
                    else
                    {
                        movie = oldMovie;
                        Console.Out.WriteLine("Aborted");
                    }
                }

                currentState = STATE_IS_LOGGED_IN;
            }

            return;
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
                string description = ConsoleGui.openQuestion("please write a description for seatgroup " + index);
                //return to menu if exit
                if (price < 0)
                {
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                }

                newSeatGroups.Add(new Theater.SeatGroup(seats, price, description));

                int ans = ConsoleGui.multipleChoice("add another price group?", "yyes", "nno");
                switch (ans)
                {
                    case -1:
                        currentState = STATE_IS_LOGGED_IN;
                        return;
                    case 1:
                        adding = false;
                        break;
                }
            }


            Theater newTheater = new Theater(newSeatGroups.ToArray());


            int check = ConsoleGui.multipleChoice(
                "You want to add Theater with id: " + newTheater.getId() + " with " + newTheater.getAmountOfSeats() +
                newTheater.seatGroupsToString(),
                "yyes", "nno");
            switch (check)
            {
                case -1:
                case 1:
                    currentState = STATE_IS_LOGGED_IN;
                    return;
                case 0:
                    theaterList.Add(newTheater.getId(), newTheater);
                    break;
            }

            Console.Out.WriteLine("Current theaters: \n");
            ConsoleGui.list(theaterList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateManagerRemoveTheater()
        {
            // Console.Out.WriteLine("theaterlist type: " + typeof(theaterList));

            Theater theater =
                (Theater) ConsoleGui.getElementByMultipleChoice("Which theater would you like to remove?", theaterList);
            int ans = ConsoleGui.multipleChoice("Are you sure?", "yyes", "nno");
            if (ans == 0 && theater != null)
            {
                theaterList.Remove(theater.getId());
            }
            else if (ans == -1)
            {
                currentState = STATE_IS_LOGGED_IN;
            }

            ConsoleGui.list(theaterList);
            currentState = STATE_IS_LOGGED_IN;
            return;
        }

        public static void stateManagerManageTheater()
        {
            // choose theater to manage
            Theater theater =
                (Theater) ConsoleGui.getElementByMultipleChoice("Which theater would you manage", theaterList);
            if (theater != null)
            {
                Theater.TimeSlot[] timeSlots = theater.GetTimeSlots();
                Theater.TimeSlot timeSlot =
                    (Theater.TimeSlot) ConsoleGui.getElementByMultipleChoice("which timeslot do you want to manage?",
                        timeSlots);
                if (timeSlot != null)
                {
                    Movie newMovie =
                        (Movie) ConsoleGui.getElementByMultipleChoice("What movie should be set to this theater? ",
                            movieList);
                    if (newMovie != null)
                    {
                        timeSlot.setMovie(newMovie);
                        // recursively call stateManagerManageTheater() for smoother operation
                        // while managing multiple timeslots
                        stateManagerManageTheater();
                    }
                }
            }

            //if no theater or timeslot is chosen, go back to logged in menu
            currentState = STATE_IS_LOGGED_IN;
        }

        public static void showMovies()
        {
            Dictionary<string, ConsoleGui.Element> runningMovies = getAllRunningMovies();
            if (runningMovies.Count == 0)
            {
                Console.Out.WriteLine("There are no active movies now! \n Come back later.");
                currentState = STATE_IS_LOGGED_IN;
                return;
            }
            Console.Out.WriteLine("Currently running movies : \n");
            ConsoleGui.list(runningMovies);
            
            
            int choice = ConsoleGui.multipleChoice("", "tbuy a ticket",
                "bback");
            switch (choice)
            {
                case -1:
                    currentState = STATE_EXIT;
                    break;
                case 0: 
                    currentState = STATE_CUSTOMER_BUY_TICKET_MOVIE;
                    break;
                
                case 1:
                    currentState = STATE_IS_LOGGED_IN;
                    break;
            }
            return;
        }

        public static void buyTicket()
        {
            Movie movie = null;
            Theater theater = null;
            Theater.TimeSlot timeSlot = null;
            while (movie == null)
            {
                movie = (Movie)ConsoleGui.getElementByMultipleChoice("For which movie do you want to order a ticket?", getAllRunningMovies());
            }
            while (theater == null)
            {
                theater = (Theater)ConsoleGui.getElementByMultipleChoice("For which theather do you want to order a ticket?", getAllTheatersByMovie(movie));
            }
            while (timeSlot == null)
            {
                timeSlot = (Theater.TimeSlot)ConsoleGui.getElementByMultipleChoice("For which movie do you want to order a ticket?", getAllTimeSlotsByTheaterAndMovie(theater, movie));
            }
            int ans = ConsoleGui.multipleChoice("Do you want to buy a ticket for "+movie.getTitle()+" in theater "+theater.getId()+" with the time slot "+timeSlot.__toString()+" ?", "yyes", "nno");
            if (ans == 0 && movie != null)
            {
                // TODO INTERGRATE SEATGROUPS
                // With a small front-end thingy 
                Ticket ticket = new Ticket(theater, movie, 25);
                Console.Out.WriteLine("You purchased a ticket for the Movie: " + movie.getTitle() + "! \n   See you soon!");
                currentState = STATE_IS_LOGGED_IN;
            }
            else
            {
                currentState = STATE_IS_LOGGED_IN;
                return;
            }
            return;
        }
        
        
        
        private static Dictionary<string, ConsoleGui.Element> getAllRunningMovies()
        {
            int count = -1;
            List<Movie> currentMovieList = new List<Movie>();
            foreach (Theater theater in theaterList.Values.ToList())
            {
                foreach (Theater.TimeSlot timeSlot in theater.GetTimeSlots())
                {
                    if (!currentMovieList.Contains(timeSlot.getMovie()) && timeSlot.getMovie().getTitle() != "None")
                    {
                        count++;
                        currentMovieList.Add(timeSlot.getMovie());
                    }
                }
            }
            
            Dictionary<string, ConsoleGui.Element> allCurrentMovies = new Dictionary<string, ConsoleGui.Element>();
            count = -1;
            
            foreach (Movie currentMovie in currentMovieList)
            {
                count++;
                allCurrentMovies.Add(count.ToString(),currentMovie);
            }
            
            return allCurrentMovies;
        }

        private static Dictionary<string, ConsoleGui.Element> getAllTheatersByMovie(Movie movie)
        {
            int count = -1;
            Dictionary<string, ConsoleGui.Element> theatersWithMovie = new Dictionary<string, ConsoleGui.Element>();
            foreach (Theater theater in theaterList.Values.ToList())
            {
                foreach (Theater.TimeSlot timeSlot in theater.GetTimeSlots())
                {
                    if (timeSlot.getMovie() == movie)
                    {
                        count++;
                        theatersWithMovie.Add(count.ToString(),theater);
                        break;
                    }
                }
            }

            return theatersWithMovie;
        }

        private static Dictionary<string, ConsoleGui.Element> getAllTimeSlotsByTheaterAndMovie(Theater theater, Movie movie)
        {
            int count = -1;
            Dictionary<string, ConsoleGui.Element> timeSlotsWithMovie = new Dictionary<string, ConsoleGui.Element>();
            foreach (Theater.TimeSlot timeSlot in theater.GetTimeSlots())
            {
                if (timeSlot.getMovie() == movie)
                {
                    count++;
                    timeSlotsWithMovie.Add(count.ToString(),timeSlot);
                    break;
                }
            }

            return timeSlotsWithMovie;
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


            public class FoodItem : ConsoleGui.Element
            {
                private String[] ingredients;
                private int price;

                public override void list()
                {
                    //TODO(Ali) welk zinnetje er in het menu moet komen waar je niks hoeft te kiezen
                }

                public override string getMPQListing()
                {
                    //TODO(Ali) welk zinnetje er in de multiplechoice moet komen
                    return "";
                }
            }
        }

        public class Ticket
        {
            public readonly int ticketID;
            public readonly Theater theater;
            public readonly Movie movie;
            public readonly Account account;
            public int price;
        }
      
        public void list()
        {
            throw new NotImplementedException();
        }

        public string getMPQListing()
        {
            throw new NotImplementedException();
        }

        public Ticket(Theater theater, Movie movie, int price)
        {
            this.theater = theater;
            this.movie = movie;
            this.account = activeUser;
            this.price = price;
        }
    }

    public class MenuItem : ConsoleGui.Element
    {
        private readonly int id;
        private string name;
        private double price;

        private static int menuItemIdCount = -1;
        private ConsoleGui.Element _elementImplementation;

        public string getId()
        {
            return id.ToString();
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return name;
        }

        public double getPrice()
        {
            return price;
        }

        public void setPrice(double price)
        {
            this.price = price;
        }

        public MenuItem(string mIname, double mIprice)
        {
            menuItemIdCount++;
            id = menuItemIdCount;

            name = mIname;
            price = mIprice;
        }

        public static MenuItem GetNoneMenuItem()
        {
            return new MenuItem("None", 00.00);
        }

        public override void list()
        {
            Console.Out.WriteLine("Id: " + getId() + " food item: " + getName());
        }

        public override string getMPQListing()
        {
            return ("Id: " + getId() + " food item: " + getName() + " price: " + getPrice() + " euro");
        }
    }

    public class Movie : ConsoleGui.Element
    {
        private readonly int id;
        private string title;

        private TimeSpan time;
        //private int price; removed because theaters have prices movies don't?

        private static int movieIdCount = -1;

        public string getId()
        {
            return id.ToString();
        }

        public void setTitle(string title)
        {
            this.title = title;
        }

        public string getTitle()
        {
            return title;
        }

        public string getTime()
        {
            return new DateTime(time.Ticks).ToString("HH:mm");
        }

        public void setTime(TimeSpan time)
        {
            this.time = time;
        }

        public Movie(string inp_title, TimeSpan inp_timeSpan)
        {
            movieIdCount++;
            id = movieIdCount;

            title = inp_title;
            time = inp_timeSpan;
        }

        public static Movie getNoneMovie()
        {
            return new Movie("None", new TimeSpan(90));
        }

        // abstract methods
        public override void list()
        {
            Console.Out.WriteLine(" Id: " + getId() + " movie: " + getTitle());
        }

        public override string getMPQListing()
        {
            return (" Id: " + getId() + " movie: " + getTitle() + " duration: " + getTime());
        }
    }

    public class Theater : ConsoleGui.Element
    {
        private readonly string theaterId;
        private readonly int amountOfSeats;
        private SeatGroup[] seatGroups;
        private TimeSlot[] timeSlots;

        private Movie currentMovie;

        private static int idCount = -1;


        // abstract methods
        public override void list()
        {
            Console.Out.WriteLine(getMPQListing());
        }

        public override string getMPQListing()
        {
            return (" Id: " + theaterId + " available amount of timeslots: " + getAmountOfavailableTimeslots());
        }


        // methods
        public int getAvailableSeats()
        {
            int output = 0;
            foreach (SeatGroup seatGroup in this.seatGroups)
            {
                output += seatGroup.getAvailableAmountOfSeats();
            }

            return output;
        }

        public string seatGroupsToString()
        {
            string output = "Seatgroups: \n";
            foreach (var seatGroup in seatGroups)
            {
                output += seatGroup.getDescription() + "\n";
            }

            return output;
        }

        public string getId()
        {
            return theaterId;
        }

        public int getAmountOfSeats()
        {
            return amountOfSeats;
        }

        // returns amount of available timeslots
        public int getAmountOfavailableTimeslots()
        {
            int output = 0;
            foreach (TimeSlot timeSlot in timeSlots)
            {
                if (timeSlot.getMovie().getTitle().Equals("None"))
                {
                    output++;
                }
            }

            return output;
        }


        public TimeSlot[] GetTimeSlots()
        {
            return timeSlots;
        }

        // constructor
        public Theater(params SeatGroup[] inp_seatGroups)
        {
            idCount++;
            theaterId = idCount.ToString();
            amountOfSeats = 0;
            foreach (SeatGroup seatGroup in inp_seatGroups)
            {
                amountOfSeats += seatGroup.getAmountOfSeats();
            }

            //add given seatgroup to instance variables
            seatGroups = inp_seatGroups;

            // theater will have a 'none' movie by default
            currentMovie = Movie.getNoneMovie();

            // initialize timeslots
            timeSlots = new[]
            {
                new TimeSlot(new TimeSpan(19, 0, 0), new TimeSpan(20, 0, 0), inp_seatGroups),
                new TimeSlot(new TimeSpan(20, 0, 0), new TimeSpan(21, 0, 0), inp_seatGroups),
                new TimeSlot(new TimeSpan(21, 0, 0), new TimeSpan(22, 0, 0), inp_seatGroups),
            };
        }

        //data holder for SeatGroup seats and prices
        public class SeatGroup
        {
            private int amountOfSeats;
            private int amountOfAvailableSeats;
            private int priceOfGroup;
            private string description;

            public SeatGroup(int inp_amountOfSeats, int inp_priceOfSeats, string inp_description)
            {
                amountOfSeats = inp_amountOfSeats;
                amountOfAvailableSeats = amountOfSeats;
                priceOfGroup = inp_priceOfSeats;
                description = inp_description;
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

            public string getDescription()
            {
                return description;
            }
        }

        public class TimeSlot : ConsoleGui.Element
        {
            private Movie runningMovie = Movie.getNoneMovie();
            private TimeSpan begin;
            private TimeSpan end;
            private SeatGroup[] seatgroups;


            public override void list()
            {
                Console.Out.WriteLine(getMPQListing());
            }

            public override string getMPQListing()
            {
                return "from: " + begin + " to: " + end + " Currently runs: " + runningMovie.getTitle();
            }

            public Movie getMovie()
            {
                return runningMovie;
            }

            public void setMovie(Movie inp_movie)
            {
                runningMovie = inp_movie;
            }
            
            public TimeSlot(TimeSpan inp_begin, TimeSpan inp_end, SeatGroup[] inp_seatgroups)
            {
                begin = inp_begin;
                end = inp_end;
                runningMovie = Movie.getNoneMovie();
                seatgroups = inp_seatgroups;
            }

            public string __toString()
            {
                return begin + " - " + end;
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
                        else if (j + 1 > lenghtOfExpectedUserInput)
                        {
                            lenghtOfExpectedUserInput = j + 1;
                        }
                    }

                    //distill info
                    string newAns = option.Substring(0, lenghtOfExpectedUserInput).ToUpper();
                    string firstChar = option.Substring(lenghtOfExpectedUserInput, 1).ToUpper();
                    string listOption = "[" + newAns + "]" + " " + firstChar +
                                        option.Substring(lenghtOfExpectedUserInput + 1);

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

                    Console.Out.WriteLine("That's not an option, type \"X\" if you don't know");
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
                    Console.Out.WriteLine("That's not a valid answer, please try again" +
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
                foreach (Element element in (IEnumerable) iterable)
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

        public static Element
            getElementByMultipleChoice(String question,
                Dictionary<string, Element> inputDict) // was IDictionary inputDict
        {
            return getElementByMultipleChoice(question, (inputDict.Values.ToList()));
        }

        public static Element getElementByMultipleChoice(String question, Element[] inputArray)
        {
            return getElementByMultipleChoice(question, inputArray.ToList());
        }
    }

    // public static class Generator
    // {
    //     public static void generateMovieData(int amountOfDataEntries, Dictionary<string, ConsoleGui.Element> inp_movieDict)
    //     {
    //         // code to generate 'amountOfDataEntries' x random movie and add them to inp_movieDict
    //     }
    //
    //     public static void generateUserData(int amountOfDataEntries, Dictionary<string, Account> inp_userDict)
    //     {
    //         // code to generate 'amountOfDataEntries' x random user and add them to inp_userDict
    //     }
    // }
}