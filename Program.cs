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
        
        private static int currentState = 0;


        public static void Main(string[] args)
        {
            //main loop
            while (currentState != STATE_EXIT) {
                switch (currentState)
                {
                    case STATE_MAIN: stateMain(); break;

                }
            }
        }
        
        //main methods
        public static void stateMain()
        {
            //intro
            Console.WriteLine("Welcome to Cinema, What would you like to do?\n");
            Console.WriteLine("[L]ogin");
            Console.WriteLine("[C]reate account");
            Console.WriteLine("[E]xit\n");

            String input = Console.ReadLine();
            
            //Choice menu
            switch (input)
            {
                case "L": /*shit*/ ; break;
                case "C": stateCreateAccount(); break;
                case "E": currentState = STATE_EXIT ; break; 
                default: Console.WriteLine("\nunknown command"); stateMain(); break;
            }
            
            
            

        }
        
        public static void stateCreateAccount()
        {
            string name;
            string password;
            string email;
            int age;

            bool creating = true;
            
            
            //TODO add checks
            //intro
            Console.WriteLine("Cool! thanks for making an account with us :)\n");

            while (creating)
            {
             Console.WriteLine("Lets begin with your name!\n");
             name = Console.ReadLine();
             Console.WriteLine("Now an easy to remember, hard to guess password:\n");
             password = Console.ReadLine();

             
             //check email
             email = "default";
             bool check = true;
             while(!email.Contains("@"))
             {
                string output = !email.Contains("@") || email.Equals("default")?  "Great! on which email-adress can we reach you?\n" : "that's not a  real email! :(\n";
                Console.WriteLine(output);
                email = Console.ReadLine();
             }
             
             //check age
             Console.WriteLine("Finally, and don't lie, How old are you?\n");
             if (int.TryParse(Console.ReadLine(), out int inAge))
             {
                 age = inAge;
             }
             else
             {
                 
             }


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



    }
}
