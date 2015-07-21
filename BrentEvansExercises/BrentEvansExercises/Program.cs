using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

namespace BrentEvansExercises
{
    #region Items
    public class Book
    {
        public double Price { get; set; }
        public string[] Chapters { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }

    public class CD
    {
        public double Price { get; set; }
        public List<Dictionary<object, object>> Tracks { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }

    public class DVD
    {
        public double Price { get; set; }
        public int Minutes { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string Director { get; set; }
    }
    #endregion

    public class Inventory
    {
        private List<Book> _books = new List<Book>();
        private List<CD> _cds = new List<CD>();
        private List<DVD> _dvds = new List<DVD>();
    
        public List<Book> Books 
        { 
            get { return _books; }
            set { _books = value; }
        }

        public List<CD> CDs
        {
            get { return _cds; }
            set { _cds = value; }
        }

        public List<DVD> DVDs 
        { 
            get { return _dvds; }
            set { _dvds = value; }
        }
    }

    public class Program
    {
        #region Init
        public static Inventory Init(JArray items)
        {
            Inventory inventory = new Inventory();

            //Separate books, cds, dvds
            if (items != null && items.Count > 0)
            {
                IList<JObject> booklist = items.Values<JObject>().Where(x => x["type"].Value<string>() == "book").ToList();
                IList<JObject> dvdlist = items.Values<JObject>().Where(x => x["type"].Value<string>() == "dvd").ToList();
                IList<JObject> cdlist = items.Values<JObject>().Where(x => x["type"].Value<string>() == "cd").ToList();

                try
                {
                    foreach (var book in booklist)
                        inventory.Books.Add(JsonConvert.DeserializeObject<Book>(book.ToString()));
                    foreach (var dvd in dvdlist)
                        inventory.DVDs.Add(JsonConvert.DeserializeObject<DVD>(dvd.ToString()));
                    foreach (var cd in cdlist)
                        inventory.CDs.Add(JsonConvert.DeserializeObject<CD>(cd.ToString()));
                }
                catch ( Exception ex){
                }

            }

            return inventory;
        }
        #endregion

        #region Question 1
        public static void Q1(Inventory inventory)
        {
            //1.	What are the 5 most expensive items from each category?
            var topBooks = inventory.Books.OrderByDescending(x => x.Price).ToList().Take(5);
            var topDvds = inventory.DVDs.OrderByDescending(x => x.Price).ToList().Take(5);
            var topCds = inventory.CDs.OrderByDescending(x => x.Price).ToList().Take(5);

            Console.WriteLine("Top 5 Book Prices:");
            foreach (var item in topBooks)
                Console.WriteLine("Name: " + item.Title + "Price " + item.Price);

            Console.WriteLine("Top 5 CD Prices:");
            foreach (var item in topDvds)
                Console.WriteLine("Name: " + item.Title + "Price " + item.Price);

            Console.WriteLine("Top 5 DVD Prices:");
            foreach (var item in topCds)
                Console.WriteLine("Name: " + item.Title + "Price " + item.Price);
        }
        #endregion

        #region Question 2
        public static void Q2(Inventory inventory)
        {
            //2.	Which cds have a total running time longer than 60 minutes?
            foreach (var cd in inventory.CDs)
            {
                var runningTotal = 0;

                foreach (Dictionary<object, object> track in cd.Tracks)
                {
                    runningTotal += Convert.ToInt32(track["seconds"]);

                    if (runningTotal > 300)
                    {
                        Console.WriteLine("CD: '" + cd.Title + "' has a running time longer than 60 minutes");
                    }
                }
            }
        }
        #endregion

        #region Question 3
        public static void Q3(Inventory inventory)
        {
            //3.	Which authors have also released cds?

            var matches = inventory.Books.Where(x => inventory.CDs.Any(y => x.Author.Equals(y.Author)));

            foreach (var match in matches)
                Console.WriteLine(match.Author + " has also released a CD");
        }
        #endregion

        #region Question 4
        public static void Q4(Inventory inventory)
        {
            int tempYear = 0;
            int minYear = 1900, maxYear = 2100;

            //4.	Which items have a title, track, or chapter that contains a year?
            foreach (var book in inventory.Books)
            {
                tempYear = 0;
                if (int.TryParse(book.Title, out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                {
                    Console.WriteLine(book.Title + " contains a year");
                }

                foreach (var chapter in book.Chapters)
                {
                    tempYear = 0;
                    if (int.TryParse(chapter, out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                        Console.WriteLine(chapter + " contains a year");
                }
            }

            foreach (var cd in inventory.CDs)
            {
                tempYear = 0;
                if (int.TryParse(cd.Title, out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                {
                    Console.WriteLine(cd.Title + " contains a year");
                }
                foreach (var track in cd.Tracks)
                {
                    foreach (var key in track.Keys.ToArray())
                    {
                        tempYear = 0;
                        if (int.TryParse(key.ToString(), out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                            Console.WriteLine(key + " contains a year");
                    }
                    foreach (var value in track.Values)
                    {
                        tempYear = 0;
                        if (int.TryParse(value.ToString(), out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                            Console.WriteLine(value + " contains a year");
                    }
                }
            }

            foreach (var dvd in inventory.DVDs)
            {
                tempYear = 0;
                if (int.TryParse(dvd.Title, out tempYear) && tempYear >= minYear && tempYear <= maxYear)
                {
                    Console.WriteLine(dvd.Title + " contains a year");
                }
            }
        }
        #endregion

        public static void Main(string[] args)
        {

            string json;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            TextWriter tmp = Console.Out;

            while ((json = Console.ReadLine()) != null)
            {
                sb.Append(json);
            }

            json = sb.ToString();

            if (!string.IsNullOrEmpty(json))
            {
                //Array of JSON objects 
                JArray items = JArray.Parse(json);
                Inventory inventory = new Inventory();

                inventory = Init(items);

                Q1(inventory);

                Q2(inventory);

                Q3(inventory);

                Q4(inventory);
            }
        }
    }
}
