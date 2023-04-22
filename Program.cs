using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodingTime
{
    internal class Program
    {
        static readonly int[] ConsoleSize = { 110, 41 };
        static readonly int[] AreaSize = { 63, 29 };
        static readonly int[] SlotSize = { 10, 5 };

        static int[,] map = new int[200, 200];
        static char[,] area = new char[AreaSize[0], AreaSize[1]];

        static List<string> code = new List<string>() { "" };
        static Dictionary<string, int> vars = new Dictionary<string, int>() {
            { "r", 0 }, { "x", 0 }, { "y", 0 }, { "i", 0 }, { "a", 0 }, { "b", 0 } };

        static Queue<string> console = new Queue<string>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            Console.SetWindowSize(ConsoleSize[0], ConsoleSize[1]);
            Console.CursorVisible = false;
            DrawMenu();

            FillMap();

            Player.DrawPlayer(AreaSize);

            List<string[]> menu = FillMenuList();
            WriteInfo(menu[0]);

            Console.SetCursorPosition(0, 0);
            MainCycle(menu);
        }

        static void MainCycle(List<string[]> menu)
        {

            while (true)
            {
                DrawMap();

                Console.SetCursorPosition(0, 0);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        Environment.Exit(1);
                        break;

                    case ConsoleKey.W:
                    case ConsoleKey.UpArrow:
                        Player.Move(0, -1, map);
                        break;

                    case ConsoleKey.S:
                    case ConsoleKey.DownArrow:
                        Player.Move(0, 1, map);
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.LeftArrow:
                        Player.Move(-1, 0, map);
                        break;

                    case ConsoleKey.D:
                    case ConsoleKey.RightArrow:
                        Player.Move(1, 0, map);
                        break;

                    case ConsoleKey.C:
                        if (!Coding(menu))
                        {
                            WriteInfo(menu[0]);
                            break;
                        }
                        goto case ConsoleKey.F5;

                    case ConsoleKey.F5:
                        Game(menu[2]);

                        WriteInfo(menu[0]);
                        break;

                    case ConsoleKey.Spacebar:
                        if (Player.breakTree(map)) WriteInv();
                        break;

                    case ConsoleKey.R:

                        break;

                    case ConsoleKey.K:

                        break;
                }
            }
        }

        static void Game(string[] menu)
        {
            WriteInfo(menu);
            float speed = 1000;

            Thread gameCycle = new Thread(() => GameCycle(ref speed));
            gameCycle.Start();

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.Escape:
                        gameCycle.Abort();

                        return;

                    case ConsoleKey.RightArrow:
                        if (speed > 100) speed /= 1.3f;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (speed < 5000) speed *= 1.3f;
                        break;
                }
            }
        }

        private static void GameCycle(ref float speed)
        {
            string[] newCode = code.Where(s => !string.IsNullOrEmpty(s)).Select(s => Regex.Replace(s.Trim(), @"\s+", " ")).ToArray();

            bool isDone = false;
            vars["i"] = -1;

            while (true)
            {
                DrawMap();
                isDone = false;

                vars["i"]++;
                vars["x"] = Player.X;
                vars["y"] = Player.Y;
                vars["r"] = new Random().Next(10);

                for (int i = 0; i < newCode.Length; i++)
                {
                    if (isDone) break;

                    string[] array = newCode[i].Split();

                    switch (array[0])
                    {
                        case "если":
                        case "if":
                            bool returnValue = false;
                            int firstNumber = 0;
                            int secondNumber = 0;
                            i++;

                            if (array.Length == 1) break;

                            string conditionLine = array[1];
                            for (int j = 2; j < array.Length; j++) conditionLine += array[j];

                            if (conditionLine.Contains("=="))
                            {
                                string[] condition = conditionLine.Split('=');
                                if (!getInt(condition[0], out firstNumber)) break;
                                if (!getInt(condition[2], out secondNumber)) break;

                                returnValue = firstNumber == secondNumber;
                            }

                            if (conditionLine.Contains("!="))
                            {
                                string[] condition = conditionLine.Split('=');
                                if (!getInt(condition[0].Remove(condition[0].Length - 1), out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber != secondNumber;
                            }

                            if (conditionLine.Contains("%="))
                            {
                                string[] condition = conditionLine.Split('=');
                                if (!getInt(condition[0].Remove(condition[0].Length - 1), out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber % 2 == secondNumber;
                            }

                            if (conditionLine.Contains(">="))
                            {
                                string[] condition = conditionLine.Split('=');
                                if (!getInt(condition[0].Remove(condition[0].Length - 1), out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber >= secondNumber;
                            }

                            if (conditionLine.Contains("<="))
                            {
                                string[] condition = conditionLine.Split('=');
                                if (!getInt(condition[0].Remove(condition[0].Length - 1), out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber <= secondNumber;
                            }

                            if (conditionLine.Contains(">"))
                            {
                                string[] condition = conditionLine.Split('>');
                                if (!getInt(condition[0], out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber > secondNumber;
                            }

                            if (conditionLine.Contains("<"))
                            {
                                string[] condition = conditionLine.Split('>');
                                if (!getInt(condition[0], out firstNumber)) break;
                                if (!getInt(condition[1], out secondNumber)) break;

                                returnValue = firstNumber < secondNumber;
                            }

                            if (returnValue) i--;
                            break;

                        case "идти":
                        case "go":
                            if (array.Length == 1) isDone = Player.Move(0, -1, map);
                            else switch (array[1])
                                {
                                    case "вперед":
                                    case "forward":
                                    case "вверх":
                                    case "up":
                                        isDone = Player.Move(0, -1, map);
                                        break;

                                    case "назад":
                                    case "back":
                                    case "вниз":
                                    case "down":
                                        isDone = Player.Move(0, 1, map);
                                        break;

                                    case "впрво":
                                    case "вон":
                                    case "направо":
                                    case "правее_не_будет":
                                    case "right":
                                        isDone = Player.Move(1, 0, map);
                                        break;

                                    case "влево":
                                    case "налево":
                                    case "left":
                                        isDone = Player.Move(-1, 0, map);
                                        break;

                                    default:
                                        isDone = Player.Move(0, -1, map);
                                        break;
                                }
                            break;

                        case "cut":
                        case "рубить":
                        case "срубить":
                            if (Player.breakTree(map))
                            {
                                isDone = true;
                                WriteInv();
                            }
                            break;

                        case "break":
                        case "ломать":
                        case "сломать":
                            if (Player.breakBlock(map))
                            {
                                isDone = true;
                                WriteInv();
                            }
                            break;

                        case "place":
                        case "ставить":
                        case "build":
                        case "строить":
                            if (Player.build(map))
                            {
                                isDone = true;
                                WriteInv();
                            }
                            break;
                    }
                }

                Thread.Sleep((int)speed);
            }
        }

        static bool getInt(string val, out int value)
        {
            if (int.TryParse(val, out value)) return true;
            if (vars.ContainsKey(val))
            {
                value = vars[val];
                return true;
            }

            return false;
        }

        private static bool Coding(List<string[]> menu)
        {
            WriteInfo(menu[1]);

            Console.CursorVisible = true;
            bool isHints = false;

            int[] position = { 0, 0 };
            int[] cursorPos = new int[2];
            int maxCodeLength = ConsoleSize[0] - (AreaSize[0] + 8) - 5;

            while (true)
            {
                cursorPos[0] = AreaSize[0] + 8 + position[0];
                cursorPos[1] = 3 + position[1];

                Console.SetCursorPosition(cursorPos[0], cursorPos[1]);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Escape:
                        if (isHints)
                        {
                            isHints = false;
                            WriteInfo(menu[1]);
                            break;
                        }
                        else
                        {
                            Console.CursorVisible = false;

                            return false;
                        }

                    case ConsoleKey.F5:
                        Console.CursorVisible = false;
                        return true;

                    case ConsoleKey.F1:
                        if (isHints) WriteInfo(menu[1]);
                        else WriteInfo(menu[3]);
                        isHints = !isHints;
                        break;

                    case ConsoleKey.Backspace:
                        if (position[0] != 0)
                        {
                            code[position[1]] = code[position[1]].Substring(0, position[0] - 1) + code[position[1]].Substring(position[0]); ;

                            position[0]--;
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (position[0] != 0)
                        {
                            position[0]--;


                        }
                        break;

                    case ConsoleKey.RightArrow:
                        position[0]++;
                        if (code[position[1]].Length <= position[0])
                        {
                            if (code[position[1]].Length < maxCodeLength)
                                code[position[1]] += ' ';
                            else position[0]--;
                        }

                        break;

                    case ConsoleKey.UpArrow:
                        if (position[1] != 0)
                        {
                            position[1]--;
                            code[position[1]] = code[position[1]].Trim();
                            position[0] = Math.Min(position[0], code[position[1]].Length);
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        position[1]++;
                        if (code.Count > position[1])
                        {
                            code[position[1]] = code[position[1]].Trim();
                            position[0] = Math.Min(position[0], code[position[1]].Length);
                        }
                        else position[0] = 0;
                        break;

                    case ConsoleKey.Enter:
                        if (code.Count < 20)
                        {
                            code.Insert(++position[1], "");
                            position[0] = 0;
                        }

                        for (int i = position[1] + 1; i < code.Count; i++)
                        {
                            Console.SetCursorPosition(AreaSize[0] + 8, 3 + i);
                            Console.Write(code[i] + new string(' ', maxCodeLength - code[i].Length));
                        }

                        break;

                    default:
                        if (keyInfo.Modifiers == ConsoleModifiers.Control)
                        {
                            continue;
                        }

                        if (code[position[1]].Length < maxCodeLength)
                        {
                            code[position[1]] = code[position[1]].Substring(0, position[0]) + keyInfo.KeyChar + code[position[1]].Substring(position[0]);
                            position[0]++;
                        }

                        break;

                }

                if (position[1] == code.Count)
                {
                    if (position[1] < 20) code.Add("");
                    else position[1]--;
                }

                Console.SetCursorPosition(AreaSize[0] + 8, 3 + position[1]);
                Console.Write(code[position[1]] + new string(' ', maxCodeLength - code[position[1]].Length));

            }
        }

        static void DrawMap()
        {
            char[,] newArea = new char[AreaSize[0], AreaSize[1]];
            for (int i = 0; i < AreaSize[0]; i++)
            {
                for (int j = 0; j < AreaSize[1]; j++)
                {
                    int x = Player.X + i - (AreaSize[0] - 1) / 2;
                    int y = Player.Y + j - (AreaSize[1] - 1) / 2;

                    bool check = true;

                    if (x < -1 || x > map.GetLength(0)) check = false;
                    if (y < -1 || y > map.GetLength(1)) check = false;

                    if (check)
                    {
                        if (x == -1 || x == map.GetLength(0) || y == -1 || y == map.GetLength(1))
                        {
                            newArea[i, j] = '+';
                            check = false;
                        }
                    }

                    if (check && map[x, y] == 1)
                    {
                        newArea[i, j] = 'j';
                        if (j > 0)
                        {
                            newArea[i, j - 1] = 'X';
                            if (i > 0) newArea[i - 1, j - 1] = '_';
                            if (i > 1) newArea[i - 2, j - 1] = '(';
                            if (i < AreaSize[0] - 1) newArea[i + 1, j - 1] = '_';
                            if (i < AreaSize[0] - 2) newArea[i + 2, j - 1] = ')';
                        }
                        if (j > 1)
                        {
                            if (i > 0) newArea[i - 1, j - 2] = '(';
                            if (i < AreaSize[0] - 1) newArea[i + 1, j - 2] = ')';
                        }
                        if (j > 2) newArea[i, j - 3] = '_';
                    }
                    else if (check && map[x, y] == 2) newArea[i, j] = '&';
                }
            }

            for (int i = 0; i < AreaSize[0]; i++)
            {
                for (int j = 0; j < AreaSize[1]; j++)
                {
                    if (newArea[i, j] != area[i, j] &&
                        (i < AreaSize[0] / 2 - 2 || i > AreaSize[0] / 2 + 2 || j < AreaSize[1] / 2 - 2 || j > AreaSize[1] / 2))
                    {
                        area[i, j] = newArea[i, j];
                        Console.SetCursorPosition(i + 2, j + 2);
                        Console.Write(area[i, j]);
                    }
                }
            }
        }

        private static void FillMap()
        {
            Random random = new Random();

            for (int x = 2; x < map.GetLength(0) - 2; x++)
            {
                for (int y = 3; y < map.GetLength(1) - 2; y++)
                {
                    if (!Player.isDeadZone(x, y)) continue;

                    bool flag = true;
                    for (int i = -2; i < 3; i++)
                    {
                        if (!flag) break;
                        for (int j = -3; j < 3; j++)
                        {
                            if (map[x + i, y + j] > 0)
                            {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (!flag) continue;

                    map[x, y] = Convert.ToInt32(random.Next(150) >= 149);
                }
            }
        }

        static void DrawMenu()
        {
            Console.SetCursorPosition(1, 1);
            Console.WriteLine(new string('█', AreaSize[0] + 2));

            for (int i = 0; i < AreaSize[1]; i++)
            {
                Console.WriteLine(" █" + new string(' ', AreaSize[0]) + "█");
            }

            Console.WriteLine(" " + new string('█', AreaSize[0] + 2));

            Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2, AreaSize[1] + 4);
            Console.Write("┌" + string.Concat(Enumerable.Repeat(new string('—', SlotSize[0]) + "┬", Player.Inv.Length)) + "\b┐");

            for (int i = 0; i < SlotSize[1]; i++)
            {
                Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2, AreaSize[1] + 5 + i);
                Console.Write("|" + string.Concat(Enumerable.Repeat(new string(' ', SlotSize[0]) + "|", Player.Inv.Length)));
            }

            Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2, AreaSize[1] + 5 + SlotSize[1]);
            Console.Write("└" + string.Concat(Enumerable.Repeat(new string('—', SlotSize[0]) + "┴", Player.Inv.Length)) + "\b┘");

            DrawTextures();

            Console.SetCursorPosition(AreaSize[0] + 8, 1);
            Console.Write("Код:");

            Console.SetCursorPosition(AreaSize[0] + 8, 25);
            Console.Write("Консоль:");

            Console.SetCursorPosition(AreaSize[0] + 8, AreaSize[1] + 3);
            Console.Write("Управление:");
        }

        static void DrawTextures()
        {
            Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2 + 1, AreaSize[1] + 6);
            Console.Write("/‾\\‾‾‾‾/‾\\");
            Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2 + 1, AreaSize[1] + 7);
            Console.Write("\\_/____\\_/");

            WriteInv();
        }

        public static void WriteInv()
        {
            Console.SetCursorPosition((AreaSize[0] - (Player.Inv.Length * SlotSize[0] + 1)) / 2 + 5, AreaSize[1] + 9);

            Console.Write(Player.Inv[0] + " ");
        }

        static void WriteInfo(string[] menu)
        {
            for (int i = 0; i < 6; i++)
            {
                Console.SetCursorPosition(AreaSize[0] + 8, AreaSize[1] + 5 + i);

                if (menu.Length <= i) Console.Write(new string(' ', 32));
                else Console.Write(menu[i] + new string(' ', 32 - menu[i].Length));
            }
        }

        static List<string[]> FillMenuList()
        {
            List<string[]> menu = new List<string[]>();

            menu.Add(new string[] {
                "F5 - запустить игру",
                "C - изменить код",
                "R - перезапустить игру",
                "Esc - выйти",
                "WASD - перемещение",
                "Space - срубить дерево"
            });

            menu.Add(new string[]
            {
                "Esc - завершить изменение кода",
                "Arrows - перемещение курсора",
                "F5 - запустить игру",
                "F1 - посмотреть список команд",
            });

            menu.Add(new string[]
            {
                "Esc - завершить игру",
                "RightArrow - ускорение",
                "LeftArrow - замедление",
            });

            menu.Add(new string[]
            {
                "r, x, y, i, a, b - переменные",
                "place - поставить блок",
                "break - сломать блок",
                "cut - срубить дерево",
                "go <up/down/right/left> - идти",
                "if - условие"
            });

            return menu;
        }
    }

    static class Player
    {
        public static int X = 100;
        public static int Y = 100;
        public static int[] Inv = new int[3];

        public static void DrawPlayer(int[] AreaSize)
        {
            int[] playerPosition = { AreaSize[0] / 2, AreaSize[1] / 2 };

            Console.SetCursorPosition(playerPosition[0], playerPosition[1]);
            Console.WriteLine("(•_•)");
            Console.SetCursorPosition(playerPosition[0], playerPosition[1] + 1);
            Console.WriteLine("\\( )/");
            Console.SetCursorPosition(playerPosition[0], playerPosition[1] + 2);
            Console.WriteLine("/ ︶\\");
        }

        static int mapValue(int x, int y, int[,] map)
        {
            if (X + x < 0 || X + x >= map.GetLength(0) || Y + y < 0 || Y + y >= map.GetLength(1)) return -1;
            return map[X + x, Y + y];
        }

        public static bool isDeadZone(int x, int y)
        {
            return x > X + 6 || x < X - 6 || y > Y + 4 || y < Y - 4;
        }

        static int[] getTree(int x, int y, int[,] map)
        {
            if (x != 0)
            {
                if (mapValue(3 * x, -2, map) == 1) return new int[] { X + (3 * x), Y - 2 };

                for (int i = -1; i < 3; i++)
                {
                    if (mapValue(5 * x, i, map) == 1) return new int[] { X + (5 * x), Y + i };
                }
            }
            if (y == -1)
            {
                for (int i = -4; i < 5; i++)
                {
                    if (i == -2) i = 3;
                    if (mapValue(i, -2, map) == 1) return new int[] { X + i, Y - 2 };
                }



                for (int i = -2; i < 3; i++)
                {
                    if (mapValue(i, -3, map) == 1) return new int[] { X + i, Y - 3 };
                }
            }
            if (y == 1)
            {
                for (int i = -4; i < 5; i++)
                {
                    if (mapValue(i, 3, map) == 1) return new int[] { X + i, Y + 3 };
                }
            }

            return null;
        }

        static bool canGo(int x, int y, int[,] map)
        {
            if (X + x < 2 || X + x > map.GetLength(0) - 3 || Y + y < 2 || Y + y > map.GetLength(1) - 1) return false;

            if (getTree(x, y, map) == null) return true;
            return false;
        }

        internal static bool Move(int x, int y, int[,] map)
        {
            if (canGo(x, y, map))
            {
                X += x;
                Y += y;
                return true;
            }

            return false;
        }

        internal static bool breakTree(int[,] map)
        {
            int[] tree = getTree(0, -1, map);
            if (tree == null) return false;

            map[tree[0], tree[1]] = 0;

            Inv[0] += new Random().Next(3, 6);
            return true;
        }

        internal static bool build(int[,] map)
        {
            if (Inv[0] < 1) return false;
            if (Y < 3) return false;
            if (map[X, Y - 3] != 0) return false;

            map[X, Y - 3] = 2;
            Inv[0]--;
            return true;
        }

        internal static bool breakBlock(int[,] map)
        {
            if (Y < 3) return false;
            if (map[X, Y - 3] < 2) return false;

            map[X, Y - 3] = 0;
            Inv[0]++;
            return true;
        }
    }
}
