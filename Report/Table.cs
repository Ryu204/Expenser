namespace Expenser.Report
{
    public class Table
    {
        public readonly struct Pair
        {
            public readonly uint Row;
            public readonly uint Column;
            public Pair(uint row, uint col)
            {
                Row = row;
                Column = col;
            }
        }

        public string[,] Data;
        public readonly Pair Size;
        public readonly Pair Cell;
        public Table(Pair size, Pair cell)
        {
            Size = size;
            Cell = cell;
            Data = new string[size.Row, size.Column];
        }

        public void Print()
        {
        }
    }
}
