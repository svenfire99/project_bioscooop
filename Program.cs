using System;
using System.Collections.Generic;

namespace project_bioscooop
{
    internal class Program
    {
        private static int permission = 0;
        
        private Dictionary<int, Movie> movieList = new Dictionary<int,Movie>();
        private Dictionary<int, Ticket> ticketList = new Dictionary<int,Ticket>();
        private Dictionary<int, Account> accountList = new Dictionary<int,Account>();
        private Dictionary<int, Theater> theaterList = new Dictionary<int,Theater>();

        
        
        
        public static void Main(string[] args)
        {
            Console.WriteLine("corona!");
        }
        
        public class Account
        {
            public readonly int permission;
            public readonly string name;
            public readonly int accountId;
            public readonly string email;
                    
                    
        
            public Account(string inp_name, int inp_accountId, int inp_permission, string inp_email)
            {
                permission = inp_permission;
                email = inp_email;
                accountId = inp_accountId;
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
