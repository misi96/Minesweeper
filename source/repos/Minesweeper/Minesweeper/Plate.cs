using System;

namespace Minesweeper.WPF
{
    public class Plate
    {
        public MinesGrid GameGrid { get; private set; }

        public int RowPosition { get; private set; }
        public int ColPosition { get; private set; }

        public bool IsFlagged { get; set; }
        public bool IsMined { get; set; }
        public bool IsRevealed { get; private set; }

        public Plate(MinesGrid grid, int rowPosition, int colPosition)
        {
            this.GameGrid = grid;
            this.RowPosition = rowPosition;
            this.ColPosition = colPosition;
        }

        public int Check()
        {
            int counter = 0;

            if (!IsRevealed && !IsFlagged)
            {
                IsRevealed = true;

                for (int i = 0; i < 9; i++)
                {
                    if (i == 4)
                        continue;
                    if (GameGrid.IsBomb(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1))
                        counter++;
                }

                if (counter == 0)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (i == 4)
                            continue;
                        GameGrid.OpenPlate(RowPosition + i / 3 - 1, ColPosition + i % 3 - 1);
                    }
                }
            }

            return counter;
        }
    }
}
