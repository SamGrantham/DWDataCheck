using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace DWDataCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Dictionary<string, int> rows = new Dictionary<string, int>();
                Console.WriteLine("Application Start:");
                string conStr = "";
                string server = "(local)";
                string file = @"C:\Temp\DWDataCheckLog.log";
                string db = "TFS_Warehouse";
                if (args.Length == 1)
                {
                    server = args[0].ToString();
                }
                if (args.Length == 2)
                {
                    db = args[1].ToString();

                }
                if (args.Length == 3)
                {
                    file = args[2].ToString();

                }
                conStr = "Data Source=" + server + "  ;Initial Catalog = " + db + "; Integrated Security = SSPI";
                using (SqlConnection connection = new SqlConnection(conStr))
                using (SqlCommand cmd = new SqlCommand("  SELECT lower([System_Title]), min (len([System_Title])) FROM " + db + ".[dbo].[DimWorkItem] group by [System_Title] ", connection))
                {
                    connection.Open();
                    File.WriteAllText(file, "Start: "  + '\r' + '\n' + '\r' + '\n');
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                rows.Add(reader[0].ToString(), int.Parse(reader[1].ToString()));
                            }
                        }
                    }
                }
                int i = 0;
                foreach (var row in rows)
                {
                    i++;
                    Console.Clear();
                    Console.WriteLine(((i / rows.Count) * 100).ToString() + "%");
                    List<string> subsets = new List<string>();
                    using (SqlConnection connection = new SqlConnection(conStr))
                    using (SqlCommand cmd = new SqlCommand(@"SELECT [WorkItemSK],[System_Id],lower([System_Title]) FROM " + db + ".[dbo].[DimWorkItem] where [System_Title] like '" + row.Key.Substring(0, row.Value).Replace("'", "''") + @"%'", connection))
                    {
                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    if (!subsets.Contains(reader[2].ToString()) && !rows.ContainsKey(reader[2].ToString()))
                                    {
                                        subsets.Add(reader[2].ToString());
                                    }
                                }
                                if (subsets.Count > 1)
                                {
                                    using (System.IO.StreamWriter openFile = new System.IO.StreamWriter(file, true))
                                    {
                                        openFile.WriteLine("Suspect Title variations read by SQL as the same title:");
                                    }
                                    foreach (var item in subsets)
                                    {
                                        using (System.IO.StreamWriter openFile = new System.IO.StreamWriter(file, true))
                                        {
                                            openFile.WriteLine(item);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
                Console.WriteLine(" ");
                Console.WriteLine("end of run...");
                Process.Start("notepad.exe", file);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                File.WriteAllText("{0} Exception caught.", e.ToString());
                Console.ReadLine();
            }
        }
    }

}

