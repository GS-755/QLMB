using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace QLMB.Models
{
    public class RandomID
    {
        private static Random random = new Random();
        private static readonly string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static database db = new database();

        public static string Get(int size = 6)
        {
            StringBuilder builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                int randomIndex = random.Next(characters.Length);
                char randomChar = characters[randomIndex];
                builder.Append(randomChar);
            }

            return builder.ToString();
        }
        public static bool ExistPropertyID(string propertyId)
        {
            try
            {
                List<MatBang> checkProperty = db.MatBangs.
                    Where(
                        k => k.MaMB.Trim() == propertyId
                    ).ToList();
                if (checkProperty.Count > 0) 
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;    
            }
        }
    }
}