using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F_Delegation.Sticks
{
    public enum Player
    {
        Human,
        Computer
    }

    public enum GameStatus
    {
        NotStarted,
        InProgress,
        GameIsOver
    }

    public class SticksGame
    {
        private readonly Random randomaizer;

        public int InitialSticksNumber { get; } //Начальная количество палок она может быть различным
        public Player Turn { get; private set; }//Свойство которая возвращает текущую сторону  т.е кто длжен делать ход 
        public int RemainingSticks { get; private set; } //свойства которая возвращает оставушуюся количество палочек
        public GameStatus GameStatus { get; private set; } // Текущий статус игры
        public event Action<int> MashinePlayeed; // Количество палок которая взяла машина
        public event EventHandler<int> HumanTurnMakeMove; // Очередь человека

        public event Action<Player> EndOfGame; // Он будет посылать себя
                                               //int InitialSticksNumber начальное количество палок    //Player whoMakesFirstMove кто будет первым делать ход
        public SticksGame(int initialSticksNumber, Player whoMakesFirstMove)//Конструктор которая принимает начальное количество палок
        {
            if (initialSticksNumber < 7 || initialSticksNumber > 30)
                throw new ArgumentException("Initial number of sticks should be >=7 and <=30");


            randomaizer = new Random();
            GameStatus = GameStatus.NotStarted; // GameStatus по умолчанию NotStarted
            InitialSticksNumber = initialSticksNumber;
            RemainingSticks = InitialSticksNumber;
            Turn = whoMakesFirstMove;
        }

       public void HumanTakes(int sticks) // Метод который позволит взять палки из кучи
        {
            if(sticks < 1 || sticks > 3)
            {
                throw new AggregateException("Вы можете взять от 1 до палочек за один ход");
            }

            if(sticks > RemainingSticks) // Если количество взятых палок больше чем количество оставшиеся
            {
                throw new ArgumentException($"Вы не можете взять больше чем осталось: {RemainingSticks}");
            }

            TakeSticks(sticks);
        }

        public void Start()
        {
            if (GameStatus == GameStatus.GameIsOver) //Если игра закончилось 
            {
                RemainingSticks = InitialSticksNumber; //то перезапускаем его
            }

            if (GameStatus == GameStatus.InProgress)
            {
                throw new InvalidOperationException("Can' t call Start when game is already in progress");//Нельзя вызывать старт так как уже играют
            }


            GameStatus = GameStatus.InProgress;

            while (GameStatus == GameStatus.InProgress)
            {
                if (Turn == Player.Computer) //Если текущий ход за компьютером
                {
                    CompMakesMove();
                }

                else
                {
                    HumanMakesMove();
                }

                FireEndOfGameIfRequired(); // Метод которая вызывает событие EndOfGame если потребуется

                Turn = Turn == Player.Computer ? Player.Human : Player.Computer; //После каждого хода должны менять очередность 
                //Если текущий Turn (ход) равен компюьтеру то в  Turn присваиваем Human иначе Компьютеру делаем инверсию с помощью тернарного оператора
            }

        }

        private void FireEndOfGameIfRequired()
        {
            if (RemainingSticks == 0) //Если нету палок больше
            {
                GameStatus = GameStatus.GameIsOver; //Тогда выставляем статус в GameIsOver 

                if (EndOfGame != null) // Если хоть кто то подписался то вызываем EndOfGame 
                {
                    EndOfGame(Turn == Player.Computer ? Player.Human : Player.Computer); //Победитель определяется по тернарным выражениям Передаем победителя
                }
            }
        }

        private void HumanMakesMove()
        {

            if (HumanTurnMakeMove != null) // Если HumanTurnMakeMove не равен нулл
                HumanTurnMakeMove(this, RemainingSticks); // то передать управление верхнему коду передаем ссылку на экземпляр и оставшуюся палок
        }

        private void CompMakesMove() // Как делает компьютер ход
        {      // RemainingSticks >= 3 то максимальное число 3 а если меньше то максимальное число на RemainingSticks  ели осталось 2 палки то 3 взять невозмоо    
            int maxNumber = RemainingSticks >= 3 ? 3 : RemainingSticks; // maxNumber Максимальное число для генераций
            int sticks = randomaizer.Next(1, maxNumber); //Генерация числа

            RemainingSticks -= sticks;//  TakeSticks(sticks); //Вспомогательный метод TakeSticks (Возьми палочки)
            if (MashinePlayeed != null)
                MashinePlayeed(sticks);
        }

        private void TakeSticks(int sticks) //Вспомогательный метод TakeSticks (Возьми палочки)
        {
            RemainingSticks -= sticks;  // RemainingSticks свойства которая возвращает оставушуюся количество палочек отнимаем sticks т.е уменьшаем на один
        }
    }
}
