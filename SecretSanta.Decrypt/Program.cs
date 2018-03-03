using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSanta.Data;

namespace SecretSanta.Decrypt
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
            long id;
            do
            {
                Console.WriteLine("Input user id: ");
            } while (!long.TryParse(Console.ReadLine(), out id));

            var config = new ConfigProvider();
            var repo = new UserRepository(config, new EncryptionProvider(config), new TriStateAssignmentAlgorithm());
            var user = repo.GetUser(id);
                if (user == null)
                    Console.WriteLine("Not found.");
                else
                {
                    Console.WriteLine($"user.DisplayName={user.DisplayName}");
                    Console.WriteLine($"user.FacebookProfileUrl={user.FacebookProfileUrl}");
                    Console.WriteLine($"={user.Email}");
                    
                    Console.WriteLine($"user.CreateDate={user.CreateDate}");
                    Console.WriteLine($"user.SendAbroad={user.SendAbroad}");

                    Console.WriteLine($"user.FullName={user.FullName}");
                    Console.WriteLine($"user.IsAdult ={user.IsAdult}");
                    Console.WriteLine($"user.Note={user.Note}");
                    Console.WriteLine($"user.AddressLine1={user.AddressLine1}");
                    Console.WriteLine($"user.AddressLine2={user.AddressLine2}");
                    Console.WriteLine($"user.City={user.City}");
                    Console.WriteLine($"user.PostalCode={user.PostalCode}");
                    Console.WriteLine($"user.Country={user.Country}");

                    Console.WriteLine($"user.AdminConfirmed={user.AdminConfirmed}");
                    Console.WriteLine($"user.EmailConfirmed={user.EmailConfirmed}");
                }
            

            Console.WriteLine("Press any key to make another query");
                Console.ReadKey();
            }
        }
    }
}
