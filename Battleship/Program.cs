using Ninject;
using System;
using System.Reflection;

namespace Battleship
{

    public class Program
    {       
        public static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel();
                kernel.Load(Assembly.GetExecutingAssembly());

                var battleshipService = kernel.Get<IBattleshipService>();
                battleshipService.CreatePositionShips();
                battleshipService.StartBattle();
            }
            catch (Exception)
            {
                Console.WriteLine("Sorry, an unexpected error occurred.");                
            }
                   
        }
    }
}
