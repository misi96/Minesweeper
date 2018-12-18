using System;
using System.Windows.Threading;

namespace Minesweeper.WPF
{
    public class MinesGrid
    {
        public event EventHandler CounterChanged;
        public event EventHandler TimerThresholdReached;
        public event EventHandler<PlateEventArgs> ClickPlate;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Mines { get; private set; }
        public int TimeElapsed { get; private set; }

        private Plate[,] plates;

        private int correctFlags;
        private int wrongFlags;

        public int FlaggedMines { get { return (this.correctFlags + this.wrongFlags); } }

        private DispatcherTimer gameTimer;

        public MinesGrid(int width, int height, int mines)
        {
            this.Width = width;
            this.Height = height;
            this.Mines = mines;
        }

        public bool IsInGrid(int rowPosition, int colPosition)
        {
            return ((rowPosition >= 0) && (rowPosition < this.Width) && (colPosition >= 0) && (colPosition < this.Height));
        }

        public bool IsBomb(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsMined;
            }
            return false;
        }

        public bool IsFlagged(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                return this.plates[rowPosition, colPosition].IsFlagged;
            }
            return false;
        }
       
        public int RevealPlate(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition))
            {
                int result = this.plates[rowPosition, colPosition].Check();
                CheckFinish();
                return result;
            }
            throw new MinesweeperException("Invalid MinesGrid reference call [row, column] on reveal");
        }

        public void FlagMine(int rowPosition, int colPosition)
        {
            if (!this.IsInGrid(rowPosition, colPosition))
            {
                throw new MinesweeperException("Invalid MinesGrid reference call [row, column] on flag");
            }

            Plate currPlate = this.plates[rowPosition, colPosition];
            if (!currPlate.IsFlagged)
            {

                if (currPlate.IsMined)
                {
                    this.correctFlags++;
                }
                else
                {
                    this.wrongFlags++;
                }
            }
            else
            {
                if (currPlate.IsMined)
                {
                    this.correctFlags--;
                }
                else
                {
                    this.wrongFlags--;
                }
            }

            currPlate.IsFlagged = !currPlate.IsFlagged;
            CheckFinish();
            this.OnCounterChanged(new EventArgs());
        }

        public void OpenPlate(int rowPosition, int colPosition)
        {
            if (this.IsInGrid(rowPosition, colPosition) && !this.plates[rowPosition, colPosition].IsRevealed)
            {
                this.OnClickPlate(new PlateEventArgs(rowPosition, colPosition));
            }
        }

        private void CheckFinish()
        {
            bool hasFinished = false;
            if (this.wrongFlags == 0 && this.FlaggedMines == this.Mines)
            {
                hasFinished = true;
                foreach (Plate item in this.plates)
                {
                    if (!item.IsRevealed && !item.IsMined)
                    {
                        hasFinished = false;
                        break;
                    }
                }
            }

            if (hasFinished) gameTimer.Stop();
        }

        public void Run()
        {
            this.correctFlags = 0;
            this.wrongFlags = 0;
            this.TimeElapsed = 0;

            this.plates = new Plate[Width, Height];

            for (int row = 0; row < Width; row++)
            {
                for (int col = 0; col < Height; col++)
                {
                    Plate cell = new Plate(this, row, col);
                    this.plates[row, col] = cell;
                }
            }

            int minesCounter = 0;
            Random minesPosition = new Random();

            while (minesCounter < Mines)
            {
                int row = minesPosition.Next(Width);
                int col = minesPosition.Next(Height);

                Plate cell = this.plates[row, col];

                if (!cell.IsMined)
                {
                    cell.IsMined = true;
                    minesCounter++;
                }
            }

            gameTimer = new DispatcherTimer();
            gameTimer.Tick += new EventHandler(OnTimeElapsed);
            gameTimer.Interval = new TimeSpan(0, 0, 1);
            gameTimer.Start();
        }

        public void Stop()
        {
            gameTimer.Stop();
        }

        protected virtual void OnCounterChanged(EventArgs e)
        {
            EventHandler handler = CounterChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnTimeElapsed(object sender, EventArgs e)
        {
            this.TimeElapsed++;
            EventHandler handler = TimerThresholdReached;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnClickPlate(PlateEventArgs e)
        {
            EventHandler<PlateEventArgs> handler = ClickPlate;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
