using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using F_Delegation.Sticks;

namespace F_Delegation
{
    class Program
    {
        static void Main(string[] args)
        {                                     //Первым будет ходить Human
            var game = new SticksGame(10, Player.Human); // Создаем экземпляр SticksGame
            game.MashinePlayeed += Game_MashinePlayeed; // Подписываемся и генерируем обработчик
            game.HumanTurnMakeMove += Game_HumanTurnMakeMove; // Подписываемся и генерируем обработчик
            game.EndOfGame += Game_EndOfGame;
            game.Start(); // Вызываем метод Start

            Console.ReadKey();

        }

        private static void Game_EndOfGame(Player player)
        {
            Console.WriteLine($"Winner:{player}");
        }

        private static void Game_HumanTurnMakeMove(object sender, int remainingSticks)
        {
            Console.WriteLine($"Remaining sticks: {remainingSticks}");
            Console.WriteLine("Take some sticks"); // Возьмите сколько то палок

            bool takenCorrectly = false; // Защита от некорректного взятия
            while (!takenCorrectly) // пока не станет takenCorrectly true
            {
                if(int.TryParse(Console.ReadLine(),out int takenSticks)) // Количество палочек который хочет получить пользователь
                {
                    var game = (SticksGame)sender; //Вытаскиваем экземпляр game из sender

                    try
                    {
                        game.HumanTakes(takenSticks);
                        takenCorrectly = true; // если HumanTakes не выбросил никакого исключения то Выходим из цикла 
                    }

                    catch(ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
        }

        private static void Game_MashinePlayeed(int SticksTaken)
        {
            Console.WriteLine($"Mashine took: {SticksTaken}"); //Машина взяла вверх
        }
    }
}
