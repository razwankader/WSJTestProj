using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widespace.AdSpaceProvisioner;

namespace Widespace.TestConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                AdSpace adSpace1 = new AdSpace();
                adSpace1.RunAd(ShowRunAdResult);
                adSpace1.PrefetchAd(ShowPrefetchAdResult);
                adSpace1.PrefetchAd(ShowPrefetchAdResult);
                adSpace1.PrefetchAd(ShowPrefetchAdResult);
                adSpace1.PrefetchAd(ShowPrefetchAdResult);
                //System.Threading.Thread.Sleep(5000);
                AdSpace adSpace2 = new AdSpace();
                adSpace2.RunAd(ShowRunAdResult);
                adSpace2.PrefetchAd(ShowPrefetchAdResult);
                adSpace2.RunAd(ShowRunAdResult);
                adSpace2.RunAd(ShowRunAdResult);
                adSpace2.RunAd(ShowRunAdResult);
                //System.Threading.Thread.Sleep(5000);
                AdSpace adSpace3 = new AdSpace();
                adSpace3.RunAd(ShowRunAdResult);
                adSpace3.PrefetchAd(ShowPrefetchAdResult);
                adSpace3.RunAd(ShowRunAdResult);
                adSpace3.RunAd(ShowRunAdResult);
                adSpace3.RunAd(ShowRunAdResult);
                adSpace3.PrefetchAd(ShowPrefetchAdResult);
                //adSpace.RunAd(ShowRunAdResult);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();

        }

        public static void ShowRunAdResult()
        {
            Console.WriteLine("RunAd() completed");
        }

        public static void ShowPrefetchAdResult()
        {
            Console.WriteLine("PrefetchAd() completed");
        }
    }
}
