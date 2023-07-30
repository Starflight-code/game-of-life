using System.Text;

namespace Conway_s_Game_Of_Life {
    public struct Point {
        public bool state;
        public bool buffer;
        public Point(bool state, bool isBuffer) {
            if (isBuffer) {
                this.state = false;
                buffer = true;
            } else {
                this.state = state;
                buffer = false;
            }
        }
    }

    public struct options {
        public int waitTime;
        public double changeForLifePerCell;
        public int[] gridSize;

        public options(int timeToWait, double lifeChance, int[] gridSize) {
            waitTime = timeToWait;
            changeForLifePerCell = lifeChance;
            this.gridSize = gridSize;
        }
    }
    internal class Program {

        public static options settings() { // - USER CONFIGURABLE OPTIONS -

            int waitTime = 300; // Time in milliseconds to wait before printing the next scene
            double changeForLife = 0.08; // Chance that life will be generated in each available cell (values from 0 to 1 allowed, ex: 0.112)
            int[] gridSize = new int[2] { 70, 40 }; // The amount of X values and Y values on the grid (max X and Y, ex: 50 by 50 with a grid of 2500 cells)

            // -- END OF USER CONFIGURABLE OPTIONS - 
            return new options(waitTime, changeForLife, gridSize);
        }

        public static List<Point> generateArray(int[] grid, double activeCellChance) {

            Random rand = new Random();
            List<Point> points = new List<Point>();
            for (int i = 0; i < (grid[0] + 2); i++) {
                points.Add(new Point(false, true));
            }

            for (int i = 0; i < grid[1]; i++) {
                points.Add(new Point(false, true));
                for (int j = 0; j < grid[0]; j++) {
                    points.Add(new Point(rand.NextDouble() <= activeCellChance, false));
                }
                points.Add(new Point(false, true));
            }

            for (int i = 0; i < (grid[0] + 2); i++) {
                points.Add(new Point(false, true));
            }
            return points;
        }

        public static int cellsNearby(List<Point> points, int currentPoint, int[] currentGrid) {
            if (points[currentPoint].buffer) {
                return 0;
            }
            List<int> checkValues = new List<int>();
            int pointsNearby = 0;

            for (int i = 0; i < 3; i++) {
                checkValues.Add(currentPoint - currentGrid[0] - 1 + i);
            }
            checkValues.Add(currentPoint - 1);
            checkValues.Add(currentPoint + 1);

            for (int i = 0; i < 3; i++) {
                checkValues.Add((currentPoint - 1) + i + currentGrid[0]);
            }
            for (int i = 0; i < checkValues.Count; i++) {
                if (!(points[checkValues[i]].buffer)) {
                    if (points[checkValues[i]].state) {
                        pointsNearby++;
                    }
                }

            }

            /*
            for (int i = 0; i < points.Count; i++) {
                if (Math.Abs(points[i].x - points[currentPoint].x) <= 1 && Math.Abs(points[i].y - points[currentPoint].y) <= 1) {
                    pointsNearby++;
                }
            }*/
            return pointsNearby;
        }
        public static bool findCellState(List<Point> points, int currentPoint, int[] currentGrid) {
            if (points[currentPoint].buffer) {
                return false;
            }
            int pointsNearby = cellsNearby(points, currentPoint, currentGrid);
            bool currentState = points[currentPoint].state;
            if (pointsNearby < 2 || 3 < pointsNearby) {
                return false;
            }
            if ((pointsNearby == 3 && !currentState) || currentState) {
                return true;
            }
            /*if (currentState) {
                return true;
            }*/
            return false;
        }
        public static void writeGUI(List<Point> points, int[] currentGrid) {
            int iterator = 0;

            for (int i = 0; i < (currentGrid[1] + 2); i++) {
                for (int j = 0; j < (currentGrid[0] + 2); j++) {
                    if (!points[iterator].buffer) {
                        if (points[iterator].state) {
                            Console.Write("X ");
                        } else {
                            Console.Write("  ");
                        }
                    }
                    iterator++;
                }
                Console.Write("\n");
            }
        }

        public static string buildGUI(List<Point> points, int[] currentGrid) {
            int iterator = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < (currentGrid[1] + 2); i++) {
                for (int j = 0; j < (currentGrid[0] + 2); j++) {
                    if (!points[iterator].buffer) {
                        if (points[iterator].state) {
                            sb.Append("X ");
                        } else {
                            sb.Append("  ");
                        }
                    }
                    iterator++;
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

        static void Main(string[] args) {
            Console.CursorVisible = false;

            options currentSettings = settings();
            int[] gridSize = currentSettings.gridSize; // x, y values
            int gridArea = gridSize[0] * gridSize[1];

            List<Point> points = generateArray(gridSize, currentSettings.changeForLifePerCell);

            for (int i = 0; i < gridSize[1]; i++) {
                Console.WriteLine();
            }

            string print = "";

            while (true) {

                for (int i = 0; i < (gridSize[0] + 2) * (gridSize[1] + 2); i++) {
                    Point updatedPoint = points[i];
                    updatedPoint.state = findCellState(points, i, gridSize);
                    points[i] = updatedPoint;
                }

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.Write(print);

                Task.Run(() => {
                    print = buildGUI(points, gridSize);
                });

                Thread.Sleep(currentSettings.waitTime);
                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.Write(print);

            }
        }
    }
}