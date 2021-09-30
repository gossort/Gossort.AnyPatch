using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Gossort.AnyPatch
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "ЛИЦЕНЗИЯ.txt";
            
            Console.WriteLine("=============================================================");
            Console.WriteLine("Заплатка для файла ЛИЦЕНЗИЯ.txt (номер договора дробь версия)");
            Console.WriteLine("=============================================================");
            Console.WriteLine();
            //Console.WriteLine("Для продолжения нажмите лююбую клавишу:");
            //Console.ReadKey();
            if (File.Exists(fileName))
            {
                string[] lines = File.ReadAllLines(fileName, Encoding.GetEncoding(1251));

                Regex regex = new Regex(@"|{\d}");

                List<string> listChange = new List<string>();
                List<string> list = new List<string>();
                List<string> emptyNumberLines = new List<string>();
                List<string> outs = new List<string>();
                int scip1 = -1;
                int scip2 = -1;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (scip1 == i)
                    {
                        continue;
                    }

                    string line = lines[i];

                    if (line.Trim() == "") // если есть пустая строка
                    {
                        list.RemoveAt(list.Count - 1); // удаляем из списка последний элемент
                        emptyNumberLines.Add((i + 1).ToString());
                        line = lines[i - 1] + lines[i + 1]; // строим строку как надо
                        scip1 = i + 1;
                    }

                    if (line.StartsWith("|"))
                    {
                        line = lines[i - 1] + lines[i];
                        list.RemoveAt(list.Count - 1);
                    }

                    string[] arr = line.Split('|');
                    int lastIndex = arr.Length - 1;

                    if (lastIndex == 16)
                    {
                        bool isChange = false;

                        if (arr[lastIndex - 1].Trim().Length > 0)   // т.е. есть версия договора, договор расширялся
                        {
                            arr[4] = $"{arr[4]}/{arr[lastIndex - 1]}"; // номеру договора присваеваем черех слеш номер версии
                            isChange = true;
                        }

                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < arr.Length - 1; j++)
                        {
                            sb.Append(arr[j] + "|");
                        }

                        list.Add(sb.ToString());

                        if (isChange)
                        {
                            listChange.Add(sb.ToString());
                            Console.WriteLine(sb);
                        }
                    }
                    else
                    {
                        outs.Add(line);
                    }
                }

                if (!Directory.Exists("ЛИЦЕНЗИЯ"))
                    Directory.CreateDirectory("ЛИЦЕНЗИЯ");

                string fileOld = "ЛИЦЕНЗИЯ\\ЛИЦЕНЗИЯ_СТАР.txt";
                if (File.Exists(fileOld))
                {
                    File.Delete(fileOld);
                }

                File.Move(fileName, fileOld);
                File.WriteAllLines(fileName, list.ToArray(), Encoding.GetEncoding(1251));
                File.WriteAllLines("ЛИЦЕНЗИЯ\\ЛИЦЕНЗИЯ_ИЗМ.txt", listChange.ToArray(), Encoding.GetEncoding(1251));
                File.WriteAllLines("ЛИЦЕНЗИЯ\\ЛИЦЕНЗИЯ_ПУСТЫЕ_СТРОКИ.txt", emptyNumberLines, Encoding.GetEncoding(1251));
                File.WriteAllLines("ЛИЦЕНЗИЯ\\ЛИЦЕНЗИЯ_НЕ_ВОШЕДШЕЕ.txt", outs, Encoding.GetEncoding(1251));
            }
            else 
            {
                Console.WriteLine("Файл ЛИЦЕНЗИЯ.txt должен быть в той же директории, что и программа");
            }
            Console.WriteLine("Файл ЛИЦЕНЗИЯ.txt изменен и перезаписан");
            Console.WriteLine("Все изменения находятся в файле ЛИЦЕНЗИЯ_ИЗМ.txt (можно удалить)");
            Console.WriteLine("Старая версия файла находится в файле ЛИЦЕНЗИЯ_СТАР.txt (можно удалить");
            Console.WriteLine("Для выхода из программы нажмите любую клавишу");
            Console.ReadKey();
        }
    }
}
